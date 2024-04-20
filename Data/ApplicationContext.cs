using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cinder.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Cinder.Data
{
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

        public ApplicationContext (DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        public DbSet<Cinder.Models.User> User { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
