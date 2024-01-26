using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Portal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string fileName = "piggybank-points-3b7c2eb5b3e4.json";
            FirebaseApp.Create(new AppOptions() { Credential = GoogleCredential.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName)) }, "PiggyBank");

            fileName = "portal-e7ad5-8e8da5d1f28e.json";
            FirebaseApp.Create(new AppOptions() { Credential = GoogleCredential.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName)) }, "Portal");

            var host = Host.CreateDefaultBuilder(args)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel().UseIIS().UseStartup<Startup>();
                }).Build();
            host.Run();
        }
    }
}
