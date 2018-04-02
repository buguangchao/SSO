﻿using AuthServer.Configuration;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static IdentityModel.OidcConstants;

namespace AuthServer.Extension
{
    public static class DatabaseExtensions
    {
        //public static void InitializeDatabase(this IApplicationBuilder app)
        //{
        //    using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
        //    {
        //        serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();
        //        serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
        //        var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
        //        context.Database.Migrate();
        //        if (!context.Clients.Any())
        //        {
        //            foreach (var client in Config.GetClients())
        //            {
        //                context.Clients.Add(client.ToEntity());
        //            }
        //            context.SaveChanges();
        //        }

        //        if (!context.IdentityResources.Any())
        //        {
        //            foreach (var resource in Config.GetIdentityResources())
        //            {
        //                context.IdentityResources.Add(resource.ToEntity());
        //            }
        //            context.SaveChanges();
        //        }

        //        if (!context.ApiResources.Any())
        //        {
        //            foreach (var resource in Config.GetApiResources())
        //            {
        //                context.ApiResources.Add(resource.ToEntity());
        //            }
        //            context.SaveChanges();
        //        }
        //    }
        //}

    }
    //public static class DatabaseExtensions
    //{
    /// <summary>
    /// 添加自动迁移,
    /// </summary>
    /// <param name="app"></param>
    //public static void InitializeDatabase(this IApplicationBuilder app)
    //{
    //    using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
    //    {
    //        serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

    //        var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
    //        context.Database.Migrate();
    //        if (!context.Clients.Any())
    //        {
    //            foreach (var client in InMemoryConfiguration.GetClients())
    //            {
    //                context.Clients.Add(client.ToEntity());
    //            }
    //            context.SaveChanges();
    //        }

    //        if (!context.IdentityResources.Any())
    //        {
    //            foreach (var resource in InMemoryConfiguration.GetIdentityResources())
    //            {
    //                context.IdentityResources.Add(resource.ToEntity());
    //            }
    //            context.SaveChanges();
    //        }

    //        if (!context.ApiResources.Any())
    //        {
    //            foreach (var resource in InMemoryConfiguration.GetApiResources())
    //            {
    //                context.ApiResources.Add(resource.ToEntity());
    //            }
    //            context.SaveChanges();
    //        }
    //    }
    //}
}

