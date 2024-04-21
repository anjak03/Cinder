using Cinder.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Cinder.Data
{
    /// <summary>
    /// Provides methods to initialize and seed the database with essential data.
    /// </summary>
    public static class DbInitializer
    {
        public static void Initialize(ApplicationContext context)
        {
            context.Database.EnsureCreated();
        }
    }
}