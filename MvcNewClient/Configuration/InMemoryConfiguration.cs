using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

using System.Security.Claims;

namespace AuthServer.Configuration
{
    public class InMemoryConfiguration
    {
        /// <summary>
        /// 1. 哪些API可以使用这个authorization server.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource> {
                new ApiResource("socialnetwork", "社交网络")
                {
                    //我们需要把email添加到access token的数据里面, 这就需要告诉Authorization Server的Api Resource里面要包括User的Scope, 因为这是Identity Scope, 我们想要把它添加到access token里:
                    UserClaims = new []{ "email","phone"}
                }
            };
        }

        /// <summary>
        /// 2. 那些客户端Client(应用)可以使用这个authorization server.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Client> GetClients()
        {
            
            return new List<Client> {
                new Client{
                    ClientId ="socialnetwork",
                    ClientSecrets = new []{ new Secret("secret".Sha256())},
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AllowedScopes = new []{ "socialnetwork"}
                },
                new Client
                {
                    //OAuth是使用Scopes来划分Api的, 而OpenId Connect则使用Scopes来限制信息, 例如使用offline access时的Profile信息, 还有用户的其他细节信息.
                    ClientId = "mvc_implicit",
                    ClientName = "MVC Client",
                    //GrantType要改为Implicit. 使用Implicit flow时, 首先会重定向到Authorization Server, 然后登陆, 
                    //然后Identity Server需要知道是否可以重定向回到网站, 如果不指定重定向返回的地址的话, 我们的Session有可能就会被劫持. 
                    AllowedGrantTypes = GrantTypes.Implicit,
                    //RedirectUris就是登陆成功之后重定向的网址, 这个网址(http://localhost:5002/signin-oidc)在MvcClient里, openid connect中间件使用这个地址就会知道如何处理从authorization server返回的response. 这个地址将会在openid connect 中间件设置合适的cookies, 以确保配置的正确性.
                    RedirectUris = { "http://localhost:5002/signin-oidc" },
                    //而PostLogoutRedirectUris是登出之后重定向的网址. 有可能发生的情况是, 你登出网站的时候, 会重定向到Authorization Server, 并允许从Authorization Server也进行登出动作.
                    PostLogoutRedirectUris = { "http://localhost:5002/signout-callback-oidc" },
                    //最后还需要指定OpenId Connect使用的Scopes, 之前我们指定的socialnetwork是一个ApiResource. 而这里我们需要添加的是让我们能使用OpenId Connect的SCopes, 这里就要使用Identity Resources. Identity Server带了几个常量可以用来指定OpenId Connect预包装的Scopes. 上面的AllowedScopes设定的就是我们要用的scopes, 他们包括 openid Connect和用户的profile, 同时也包括我们之前写的api resource: "socialnetwork". 要注意区分, 这里有Api resources, 还有openId connect scopes(用来限定client可以访问哪些信息), 而为了使用这些openid connect scopes, 我们需要设置这些identity resoruces, 这和设置ApiResources差不多:
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "socialnetwork"
                    },
                    //可以看到id_token有了, 而access_token没有, 这是因为我们还没有告诉Authorization Server在使用implicit flow时可以允许返回Access token.
                    AllowAccessTokensViaBrowser = true
                },
                new Client
                {
                    ClientId = "mvc_code",
                    ClientName = "MVC Code Client",
                    //GrantType要改成Hybrid或者HybrdAndClientCredentials, 如果只使用Code Flow的话不行, 因为我们的网站使用Authorization Server来进行Authentication, 我们想获取Access token以便被授权来访问api. 所以这里用HybridFlow.
                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    RedirectUris = { "http://localhost:5002/signin-oidc" },
                    PostLogoutRedirectUris = { "http://localhost:5002/signout-callback-oidc" },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        //还需要添加一个新的Email scope, 因为我想改变api来允许我基于email来创建用户的数据, 因为authorization server 和 web api是分开的, 所以用户的数据库也是分开的. Api使用用户名(email)来查询数据库中的数据.
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Phone,
                        "socialnetwork"
                    },
                    //我们还需要获取Refresh Token, 这就要求我们的网站必须可以"离线"工作, 这里离线是指用户和网站之间断开了, 并不是指网站离线了.
                    //这就是说网站可以使用token来和api进行交互, 而不需要用户登陆到网站上. 
                    AllowOfflineAccess = true,
                    AllowAccessTokensViaBrowser = true
                }
            };
        }

        /// <summary>
        /// 最后还需要指定OpenId Connect使用的Scopes, 之前我们指定的socialnetwork是一个ApiResource. 而这里我们需要添加的是让我们能使用OpenId Connect的SCopes, 这里就要使用Identity Resources. Identity Server带了几个常量可以用来指定OpenId Connect预包装的Scopes. 上面的AllowedScopes设定的就是我们要用的scopes, 他们包括 openid Connect和用户的profile, 同时也包括我们之前写的api resource: "socialnetwork". 要注意区分, 这里有Api resources, 还有openId connect scopes(用来限定client可以访问哪些信息), 而为了使用这些openid connect scopes, 我们需要设置这些identity resoruces, 这和设置ApiResources差不多:
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                //还需要添加一个新的Email scope, 因为我想改变api来允许我基于email来创建用户的数据, 因为authorization server 和 web api是分开的, 所以用户的数据库也是分开的. Api使用用户名(email)来查询数据库中的数据.
                new IdentityResources.Email(),
                new IdentityResources.Phone()
            };
        }

        /// <summary>
        /// 3. 指定可以使用authorization server授权的用户.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<TestUser> GetTestUsers()
        {
            return new[] {
                new TestUser {
                    SubjectId = "1",
                    Username = "bugc",
                    Password = "bugc",
                    Claims = new []{
                        //还需要添加一个新的Email scope, 因为我想改变api来允许我基于email来创建用户的数据, 因为authorization server 和 web api是分开的, 所以用户的数据库也是分开的. Api使用用户名(email)来查询数据库中的数据.
                        new Claim("email","bugc@qq.com"),
                        new Claim("phone","15295764518")
                    }
                }
            };
        }
    }
}
