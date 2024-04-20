using Cinder.Data;
using Cinder.Models;
using System;
using System.Linq;

namespace Cinder.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationContext context)
        {
            context.Database.EnsureCreated();
        }
    }
}