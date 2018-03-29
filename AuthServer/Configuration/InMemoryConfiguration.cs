using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using IdentityServer4.Models;
using IdentityServer4.Test;

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
                }
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
                    Password = "bugc"
                }
            };
        }
    }
}
