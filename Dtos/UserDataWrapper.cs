using Cinder.Dtos;

namespace YourProject.DTOs
{
    public class UserDataWrapper
    {
        public string? ExistingUserId { get; set; }
        public List<UserDto> Users { get; set; }
    }
}
