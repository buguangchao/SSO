using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

using AuthServer.Configuration;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using IdentityServer4.EntityFramework.DbContexts;
using AuthServer.Extension;

namespace AuthServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            //services.AddIdentityServer()
            //    //.AddDeveloperSigningCredential()
            //    .AddSigningCredential(new X509Certificate2(@"E:\bugc\CoreDemo\AuthServer\AuthServer\Pfx\socialnetwork.pfx", "123456"))
            //    //然后我们需要配置Authorization Server来允许使用这些Identity Resources, Statup的:
            //    .AddInMemoryIdentityResources(InMemoryConfiguration.GetIdentityResources())
            //    .AddTestUsers(InMemoryConfiguration.GetTestUsers().ToList())
            //    .AddInMemoryApiResources(InMemoryConfiguration.GetApiResources())
            //    .AddInMemoryClients(InMemoryConfiguration.GetClients());


            //https://elanderson.net/2017/07/identity-server-using-entity-framework-core-for-configuration-data/
            //请注意，以下内容已全部由AddConfigurationStore 和  AddOperationalStore取代  。
            //AddInMemoryPersistedGrants
            //AddInMemoryIdentityResources
            //AddInMemoryApiResources
            //AddInMemoryClients
            services.AddIdentityServer()
                // .AddDeveloperSigningCredential()
                .AddSigningCredential(new X509Certificate2(@"E:\bugc\CoreDemo\AuthServer\AuthServer\Pfx\socialnetwork.pfx", "123456"))
                .AddTestUsers(InMemoryConfiguration.GetTestUsers().ToList())
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(connectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                // this adds the operational data from DB (codes, tokens, consents)
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(connectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));

                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                    options.TokenCleanupInterval = 30;
                });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            DbExtensions.InitializeDatabase(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});

            app.UseIdentityServer();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }

        
    }
}
