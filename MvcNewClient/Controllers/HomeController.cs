using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MvcNewClient.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Globalization;
using IdentityModel.Client;

namespace MvcNewClient.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            await HttpContext.SignOutAsync("oidc");
        }

        [Authorize]
        public async Task<IActionResult> GetIdentity()
        {
            //正式生产环境中可不要这么做, 正式环境中应该在401之后, 调用这个方法, 如果再失败, 再返回错误.
            await RefreshTokensAsync();

            //和About页面显示的不一样, 说明刷新token了.
            var token = await HttpContext.GetTokenAsync("access_token");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var result = await client.GetStringAsync("http://localhost:5001/api/identity");
                return Ok(result);
            }
        }

        [Authorize]
        public async Task RefreshTokensAsync()
        {
            var authorizationServerInfo = await DiscoveryClient.GetAsync("http://localhost:5000/");
            var client = new TokenClient(authorizationServerInfo.TokenEndpoint, "mvc_code", "secret");
            var refreshToken = await HttpContext.GetTokenAsync("refresh_token");
            var response = await client.RequestRefreshTokenAsync(refreshToken);
            var identityToken = await HttpContext.GetTokenAsync("identity_token");
            var expiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(response.ExpiresIn);
            var tokens = new[]
            {
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.IdToken,
                    Value = identityToken
                },
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.AccessToken,
                    Value = response.AccessToken
                },
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.RefreshToken,
                    Value = response.RefreshToken
                },
                new AuthenticationToken
                {
                    Name = "expires_at",
                    Value = expiresAt.ToString("o", CultureInfo.InvariantCulture)
                }
            };
            var authenticationInfo = await HttpContext.AuthenticateAsync("Cookies");
            authenticationInfo.Properties.StoreTokens(tokens);
            //重登录, 把当前用户信息的Principal和Properties传进去. 这就会更新客户端的Cookies, 用户也就保持登陆并且刷新了tokens.
            await HttpContext.SignInAsync("Cookies", authenticationInfo.Principal, authenticationInfo.Properties);
        }
    }
}
