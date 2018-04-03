using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MvcNewClient.Data;
using MvcNewClient.Models;
using MvcNewClient.Services;
using MvcNewClient.Extensions;
using System.Reflection;


namespace MvcNewClient
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 2;
                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
                // Signin settings
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                // User settings
                options.User.RequireUniqueEmail = false;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddTransient<IEmailSender, EmailSender>();
            services.AddMvc();

            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                // .AddSigningCredential(new X509Certificate2(@"D:\Projects\test\socialnetwork.pfx", "password"))
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(connectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(connectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                    options.EnableTokenCleanup = true;
                    options.TokenCleanupInterval = 30;
                })
                // nuget package : IdentityServer4.AspNetIdentity:
                // diff new add
                .AddAspNetIdentity<ApplicationUser>();


             //   services.AddAuthentication(o =>
             //   {
             //       //使用Cookie作为验证用户的首选方式
             //       o.DefaultScheme = "Cookies";
             //       //当用户需要登陆的时候, 将使用的是OpenId Connect Scheme.
             //       o.DefaultChallengeScheme = "oidc";
             //   })
             ////表示添加了可以处理Cookie的处理器(handler).
             //.AddCookie("Cookies")
             ////让上面的handler来执行OpenId Connect 协议
             ////.AddOpenIdConnect("oidc", o=>
             ////{
             ////    //信任的Identity Server ( Authorization Server).
             ////    o.Authority = "http://localhost:5000";
             ////    o.RequireHttpsMetadata = false;
             ////    //ClientId是Client的识别标志. 目前Authorization Server还没有配置这个Client, 一会我们再弄.
             ////    //Client名字也暗示了我们要使用的是implicit flow, 这个flow主要应用于客户端应用程序, 这里的客户端应用程序主要是指javascript应用程序. 
             ////    //implicit flow是很简单的重定向flow, 它允许我们重定向到authorization server, 然后带着id token重定向回来,
             ////    //这个 id token就是openid connect 用来识别用户是否已经登陆了. 同时也可以获得access token. 很明显, 我们不希望access token出现在那个重定向中.
             ////    o.ClientId = "mvc_implicit";
             ////    //一旦OpenId Connect协议完成, SignInScheme使用Cookie Handler来发布Cookie 
             ////    //(中间件告诉我们已经重定向回到MvcClient了, 这时候有token了, 使用Cookie handler来处理).
             ////    //SaveTokens为true表示要把从Authorization Server的Reponse中返回的token们持久化在cookie中.
             ////    o.SaveTokens = true;

             ////    //有个地方写到返回类型是id_token. 这表示我们要进行的是Authentication.
             ////    //而我们想要的是既做Authentication又做Authorization.也就是说我们既要id_token还要token本身.
             ////    o.ResponseType = "id_token token";
             ////});

             //.AddOpenIdConnect("oidc", options =>
             //{
             //    options.SignInScheme = "Cookies";
             //    options.Authority = "http://localhost:5000";
             //    options.RequireHttpsMetadata = false;
             //    options.ClientId = "mvc_code";
             //       //首先改ClientId和Authorization server一致. 这样用户访问的时候和implicit差不多, 只不过重定向回来的时候, 获取了一个code, 使用这个code可以换取secret然后获取access token.
             //       //所以需要在网站(MvcClient)上指定Client Secret. 这个不要泄露出去.
             //       options.ClientSecret = "secret";
             //       //还需要改变reponse type, 不需要再获取access token了, 而是code, 这意味着使用的是Authorization Code flow.
             //       options.ResponseType = "id_token code";
             //       //还需要指定请求访问的scopes: 包括 socialnetwork api和离线访问
             //       options.Scope.Add("socialnetwork");
             //    options.Scope.Add("offline_access");

             //       //还需要添加一个新的Email scope, 因为我想改变api来允许我基于email来创建用户的数据, 因为authorization server 和 web api是分开的, 所以用户的数据库也是分开的. Api使用用户名(email)来查询数据库中的数据.
             //       options.Scope.Add("email");
             //    options.Scope.Add("phone");

             //    options.SaveTokens = true;
             //       //最后还可以告诉它从UserInfo节点获取用户的Claims.
             //       options.GetClaimsFromUserInfoEndpoint = true;
             //});
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //DdExtension.InitializeDatabase(app);


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            //注意在Configure方法里面不要使用app.UseAuthentication(), 因为app.UseIdentityServer()方法已经包含了这个中间件.. 然后使用命令行执行:
            //dotnet ef database update
            // diff new add
            app.UseIdentityServer();

            //注意在管道配置的位置一定要在useMVC之前.
            //app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
