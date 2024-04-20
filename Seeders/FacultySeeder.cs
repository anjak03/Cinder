using Cinder.Data;
using Cinder.Models;

namespace Cinder.Seeders{
    public class FacultySeeder{

        public static void SeedAllFaculties(ApplicationContext context){
            if(!context.Faculties.Any()){
                
                var faculties = new List<Faculty>
                    {
                        new Faculty { Name = "Academy of Music" },
                        new Faculty { Name = "Academy of Theatre, Radio, Film and Television" },
                        new Faculty { Name = "Academy of Fine Arts and Design" },
                        new Faculty { Name = "Biotechnical Faculty" },
                        new Faculty { Name = "School of Economics and Business" },
                        new Faculty { Name = "Faculty of Architecture" },
                        new Faculty { Name = "Faculty of Social Sciences" },
                        new Faculty { Name = "Faculty of Electrical Engineering" },
                        new Faculty { Name = "Faculty of Pharmacy" },
                        new Faculty { Name = "Faculty of Civil and Geodetic Engineering" },
                        new Faculty { Name = "Faculty of Chemistry and Chemical Technology" },
                        new Faculty { Name = "Faculty of Mathematics and Physics" },
                        new Faculty { Name = "Faculty of Maritime Studies and Transport" },
                        new Faculty { Name = "Faculty of Computer and Information Science" },
                        new Faculty { Name = "Faculty of Social Work" },
                        new Faculty { Name = "Faculty of Mechanical Engineering" },
                        new Faculty { Name = "Faculty of Sport" },
                        new Faculty { Name = "Faculty of Administration" },
                        new Faculty { Name = "Faculty of Arts" },
                        new Faculty { Name = "Faculty of Medicine" },
                        new Faculty { Name = "Faculty of Natural Sciences and Engineering" },
                        new Faculty { Name = "Faculty of Education" },
                        new Faculty { Name = "Faculty of Law" },
                        new Faculty { Name = "Faculty of Theology" },
                        new Faculty { Name = "Veterinary Faculty" },
                        new Faculty { Name = "Faculty of Health Sciences" }
                };

                context.Faculties.AddRange(faculties);
                context.SaveChanges();
            }
        }
    }   
}