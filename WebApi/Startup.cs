using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;

namespace WebApi
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

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("V1", new Swashbuckle.AspNetCore.Swagger.Info { Title = "My API", Version = "v1" });
            });

            //webapi配置identity server就需要对token进行验证, 这个库就是对access token进行验证的.
            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(o=> {
                    o.RequireHttpsMetadata = false;
                    o.Authority = "http://localhost:5000";
                    o.ApiName = "socialnetwork";
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(o =>
            {
                o.SwaggerEndpoint("/swagger/V1/swagger.json", "MY API V1");
            });

            //这句话就是在把验证中间件添加到管道里, 这样每次请求就会调用验证服务了. 一定要在UserMvc()之前调用.
            //当在controller或者Action使用[Authorize]属性的时候, 这个中间件就会基于传递给api的Token来验证Authorization, 
            //如果没有token或者token不正确, 这个中间件就会告诉我们这个请求是UnAuthorized(未授权的).
            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
