using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Cinder.Models;

namespace Cinder.Data
{
    /// <summary>
    /// The database context used for the application which includes all the models and their configurations.
    /// </summary>
    public class ApplicationContext : IdentityDbContext<User>
    {
        public DbSet<Message> Messages {get; set;}
        public DbSet<Language> Languages { get; set; }
        public DbSet<Faculty> Faculties {get; set;}
        public DbSet<Hobby> Hobbies {get; set;} 
        public DbSet<Property> Properties {get; set;}   
        public DbSet<UserLanguage> UserLanguages {get; set;}   
        public DbSet<UserHobby> UserHobbies {get; set;}
        public DbSet<Match> Matches {get; set;}

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationContext"/> class using the specified options.
        /// The options typically include configurations such as the connection string and database provider.
        /// </summary>
        /// <param name="options">The options to be used by a DbContext.</param>
        public ApplicationContext (DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        public DbSet<Cinder.Models.User> User { get; set; } = default!;


        /// <summary>
        /// This method is called by the framework when the model for a derived context has been initialized,
        /// but before the model has been locked down and used to initialize the context. 
        /// The default implementation of this method does nothing, but it can be overridden in a derived class 
        /// such that the model can be further configured before it is locked down.
        /// </summary>
        /// <param name="modelBuilder">Defines the model for the context being created.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure message relationships
            modelBuilder.Entity<Message>()
                .HasOne<User>(m => m.Sender)
                .WithMany() 
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Message>()
                .HasOne<User>(m => m.Receiver)
                .WithMany()
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict); 

            // Configure many-to-many relationships for UserLanguage
            modelBuilder.Entity<UserLanguage>()
                .HasKey(ul => new { ul.UserId, ul.LanguageId });

            modelBuilder.Entity<UserLanguage>()
                .HasOne(ul => ul.User)
                .WithMany(u => u.UserLanguages)
                .HasForeignKey(ul => ul.UserId);

            modelBuilder.Entity<UserLanguage>()
                .HasOne(ul => ul.Language)
                .WithMany(l => l.UserLanguages)
                .HasForeignKey(ul => ul.LanguageId);

            // Configure many-to-many relationships for UserHobby
            modelBuilder.Entity<UserHobby>()
                .HasKey(uh => new { uh.UserId, uh.HobbyId });

            modelBuilder.Entity<UserHobby>()    
                .HasOne(uh => uh.User)
                .WithMany(u => u.UserHobbies)
                .HasForeignKey(uh => uh.UserId);

            modelBuilder.Entity<UserHobby>()
                .HasOne(uh => uh.Hobby)
                .WithMany(h => h.UserHobbies)
                .HasForeignKey(uh => uh.HobbyId);
        }
    }
}
