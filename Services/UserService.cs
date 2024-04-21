using Cinder.Data;
using Cinder.Dtos;
using Microsoft.EntityFrameworkCore;

public class UserService
{
    private readonly ApplicationContext _context;

    public UserService(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<List<UserDto>> GetAllUserData()
    {
        return await _context.Users
            .Include(u => u.Faculty)
            .Select(u => new UserDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                MyBooleanProperty = u.MyBooleanProperty,
                Bio = u.Bio,
                Age = u.Age,
                Faculty = u.Faculty == null ? null : new FacultyDto
                {
                    Id_Faculty = u.Faculty.Id_Faculty,
                    Name = u.Faculty.Name
                },
                FacultyYear = u.FacultyYear,
                Rating = u.Rating,
                Sex = u.Sex,
                Employed = u.Employed,
                Employment = u.Employment,
                Smoker = u.Smoker,
                Pets = u.Pets,
                ImageURL = u.ImageURL,
                LeaseDuration = u.LeaseDuration,
                Languages = u.UserLanguages.Select(ul => new LanguageDto
                {
                    Id_Language = ul.Language.Id_Language,
                    Name = ul.Language.Name
                }).ToList(),
                Hobbies = u.UserHobbies.Select(uh => new HobbyDto
                {
                    Id_Hobby = uh.Hobby.Id_Hobby,
                    Name = uh.Hobby.Name
                }).ToList()
            })
            .ToListAsync();
    }
}
