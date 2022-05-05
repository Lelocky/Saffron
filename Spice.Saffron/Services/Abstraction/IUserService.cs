using Spice.Saffron.ViewModels;

namespace Spice.Saffron.Services
{
    public interface IUserService
    {
        Task<UserViewModel> GetUserAsync(string userName);
        Task<bool> DeleteUserDataAsync(string userName);
        Task<List<UserViewModel>> GetUsersAsync();
        Task<bool> UpdateUserDateOfBirthAsync(string userName, DateTime dateOfBirth);
        Task<bool> UpdateIngameNameAsync(string userName, string ingameName);
    }
}
