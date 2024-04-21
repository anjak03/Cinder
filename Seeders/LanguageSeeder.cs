using Microsoft.AspNetCore.Identity;
using Cinder.Models;
using Cinder.Data;
using System.Threading.Tasks;

namespace Cinder.Seeders
{
    /// <summary>
    /// Seeds languages into the database if they do not already exist.
    /// </summary>
    public class LanguageSeeder
    {
        public static void SeedAllLanguages(ApplicationContext context)
        {
            if (!context.Languages.Any())
            {
                var languages = new List<Language>
                    {
                        new Language { Name = "Albanian" },
                        new Language { Name = "Basque" },
                        new Language { Name = "Belarusian" },
                        new Language { Name = "Bosnian" },
                        new Language { Name = "Breton" },
                        new Language { Name = "Bulgarian" },
                        new Language { Name = "Catalan" },
                        new Language { Name = "Croatian" },
                        new Language { Name = "Czech" },
                        new Language { Name = "Danish" },
                        new Language { Name = "Dutch" },
                        new Language { Name = "English" },
                        new Language { Name = "Estonian" },
                        new Language { Name = "Faroese" },
                        new Language { Name = "Finnish" },
                        new Language { Name = "French" },
                        new Language { Name = "Galician" },
                        new Language { Name = "German" },
                        new Language { Name = "Greek" },
                        new Language { Name = "Hungarian" },
                        new Language { Name = "Icelandic" },
                        new Language { Name = "Irish" },
                        new Language { Name = "Italian" },
                        new Language { Name = "Latvian" },
                        new Language { Name = "Lithuanian" },
                        new Language { Name = "Luxembourgish" },
                        new Language { Name = "Macedonian" },
                        new Language { Name = "Maltese" },
                        new Language { Name = "Manx" },
                        new Language { Name = "Norwegian" },
                        new Language { Name = "Occitan" },
                        new Language { Name = "Polish" },
                        new Language { Name = "Portuguese" },
                        new Language { Name = "Romanian" },
                        new Language { Name = "Russian" },
                        new Language { Name = "Scots Gaelic" },
                        new Language { Name = "Serbian" },
                        new Language { Name = "Slovak" },
                        new Language { Name = "Slovenian" },
                        new Language { Name = "Spanish" },
                        new Language { Name = "Swedish" },
                        new Language { Name = "Turkish" },
                        new Language { Name = "Ukrainian" },
                        new Language { Name = "Welsh" },
                        new Language { Name = "Yiddish" }
                        // Add more languages as needed
                    };
                context.Languages.AddRange(languages);
                context.SaveChanges();
            }
        }
    }
}