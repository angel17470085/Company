using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Entities
{
    public static class MigrationsManager
    {
        private static int _numberOfRetries;

        public static IHost MigrateDatabase(this IHost host)
        {
          using (var scope = host.Services.CreateScope())
          {
            using var appContext = scope.ServiceProvider.GetRequiredService<RepositoryContext>();
            try 
            { 
                appContext.Database.Migrate(); 
            }
            catch(SqlException)
            {
                if (_numberOfRetries < 6) 
                { 
                    Thread.Sleep(10000); 
                    _numberOfRetries++; 
                    Console.WriteLine($"The server was not found or was not accessible. Retrying... for the #{_numberOfRetries} time"); 
                    MigrateDatabase(host); 
                }
                throw;
            }
          }
          return host;
        }
    }
}