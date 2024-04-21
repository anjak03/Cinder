using Cinder.Data;
using Cinder.Models;

namespace Cinder.Seeders{
    /// <summary>
    /// Seeds hobbies into the database if they do not already exist.
    /// </summary>
    public class HobbySeeder{
        public static void SeedAllHobbies (ApplicationContext context){
            if(!context.Hobbies.Any()){

                var hobbies = new List<Hobby>{
                    new Hobby { Name = "Reading" },
                    new Hobby { Name = "Writing" },
                    new Hobby { Name = "Coding" },
                    new Hobby { Name = "Playing an instrument" },
                    new Hobby { Name = "Drawing" },
                    new Hobby { Name = "Cooking" },
                    new Hobby { Name = "Gaming" },
                    new Hobby { Name = "Hiking" },
                    new Hobby { Name = "Photography" },
                    new Hobby { Name = "Traveling" },
                    new Hobby { Name = "Painting" },
                    new Hobby { Name = "Gardening" },
                    new Hobby { Name = "Playing sports" },
                    new Hobby { Name = "Fishing" },
                    new Hobby { Name = "Watching movies" },
                    new Hobby { Name = "Listening to music" },
                    new Hobby { Name = "Yoga" },
                    new Hobby { Name = "Meditation" },
                    new Hobby { Name = "Crafting" },
                    new Hobby { Name = "Knitting" },
                    new Hobby { Name = "Collecting" },
                    new Hobby { Name = "Volunteering" },
                    new Hobby { Name = "Learning a new language" },
                    new Hobby { Name = "Birdwatching" },
                    new Hobby { Name = "Astronomy" },
                    new Hobby { Name = "Model building" },
                    new Hobby { Name = "DIY Projects" },
                    new Hobby { Name = "Geocaching" },
                    new Hobby { Name = "Surfing" },
                    new Hobby { Name = "Scuba diving" }
                };

                context.Hobbies.AddRange(hobbies);
                context.SaveChanges();
            }
        }

    }
}