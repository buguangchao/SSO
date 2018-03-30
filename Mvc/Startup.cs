using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System.IdentityModel.Tokens.Jwt;

namespace Mvc
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            //关闭了JWT的Claim 类型映射, 以便允许well-known claims.
            //这样做, 就保证它不会修改任何从Authorization Server返回的Claims.
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();


            services.AddAuthentication(o=>
            {
                //使用Cookie作为验证用户的首选方式
                o.DefaultScheme = "Cookies";
                //当用户需要登陆的时候, 将使用的是OpenId Connect Scheme.
                o.DefaultChallengeScheme = "oidc";
            })
            //表示添加了可以处理Cookie的处理器(handler).
            .AddCookie("Cookies")
            //让上面的handler来执行OpenId Connect 协议
            //.AddOpenIdConnect("oidc", o=>
            //{
            //    //信任的Identity Server ( Authorization Server).
            //    o.Authority = "http://localhost:5000";
            //    o.RequireHttpsMetadata = false;
            //    //ClientId是Client的识别标志. 目前Authorization Server还没有配置这个Client, 一会我们再弄.
            //    //Client名字也暗示了我们要使用的是implicit flow, 这个flow主要应用于客户端应用程序, 这里的客户端应用程序主要是指javascript应用程序. 
            //    //implicit flow是很简单的重定向flow, 它允许我们重定向到authorization server, 然后带着id token重定向回来,
            //    //这个 id token就是openid connect 用来识别用户是否已经登陆了. 同时也可以获得access token. 很明显, 我们不希望access token出现在那个重定向中.
            //    o.ClientId = "mvc_implicit";
            //    //一旦OpenId Connect协议完成, SignInScheme使用Cookie Handler来发布Cookie 
            //    //(中间件告诉我们已经重定向回到MvcClient了, 这时候有token了, 使用Cookie handler来处理).
            //    //SaveTokens为true表示要把从Authorization Server的Reponse中返回的token们持久化在cookie中.
            //    o.SaveTokens = true;

            //    //有个地方写到返回类型是id_token. 这表示我们要进行的是Authentication.
            //    //而我们想要的是既做Authentication又做Authorization.也就是说我们既要id_token还要token本身.
            //    o.ResponseType = "id_token token";
            //});

            .AddOpenIdConnect("oidc", options =>
            {
                options.SignInScheme = "Cookies";
                options.Authority = "http://localhost:5000";
                options.RequireHttpsMetadata = false;
                options.ClientId = "mvc_code";
                //首先改ClientId和Authorization server一致. 这样用户访问的时候和implicit差不多, 只不过重定向回来的时候, 获取了一个code, 使用这个code可以换取secret然后获取access token.
                //所以需要在网站(MvcClient)上指定Client Secret. 这个不要泄露出去.
                options.ClientSecret = "secret";
                //还需要改变reponse type, 不需要再获取access token了, 而是code, 这意味着使用的是Authorization Code flow.
                options.ResponseType = "id_token code";
                //还需要指定请求访问的scopes: 包括 socialnetwork api和离线访问
                options.Scope.Add("socialnetwork");
                options.Scope.Add("offline_access");

                //还需要添加一个新的Email scope, 因为我想改变api来允许我基于email来创建用户的数据, 因为authorization server 和 web api是分开的, 所以用户的数据库也是分开的. Api使用用户名(email)来查询数据库中的数据.
                options.Scope.Add("email");
                options.Scope.Add("phone");

                options.SaveTokens = true;
                //最后还可以告诉它从UserInfo节点获取用户的Claims.
                options.GetClaimsFromUserInfoEndpoint = true;
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            //注意在管道配置的位置一定要在useMVC之前.
            app.UseAuthentication();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
