using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlasticNotifyCenter.Data;
using PlasticNotifyCenter.Data.Identity;

[assembly: HostingStartup(typeof(PlasticNotifyCenter.Areas.Identity.IdentityHostingStartup))]
namespace PlasticNotifyCenter.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}