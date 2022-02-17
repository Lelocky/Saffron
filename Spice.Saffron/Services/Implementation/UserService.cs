using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Spice.Saffron.Data;
using Spice.Saffron.ViewModels;

namespace Spice.Saffron.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserService> _logger;

        public UserService(UserManager<ApplicationUser> userManager, ILogger<UserService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<UserViewModel> GetUserAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) { throw new ArgumentNullException(nameof(userName)); }

            try
            {
                var user = await _userManager.FindByNameAsync(userName);
                if (user != null)
                {
                    return new UserViewModel
                    {
                        NickName = user.Nickname,
                        DateOfBirth = user.DateOfBirth,
                        ProfileImage = user.ProfileImage,
                        UserName = user.UserName,
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while getting user with userName [{userName}]");
            }

            return null;
        }

        public async Task<bool> DeleteUserDataAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) { throw new ArgumentNullException(nameof(userName)); }

            try
            {
                var user = await _userManager.FindByNameAsync(userName);
                if (user == null)
                {
                    return false;
                }

                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    _logger.LogInformation($"Removed user with id [{user.Id}]");
                    return true;
                }
                else
                {
                    _logger.LogCritical($"Failed to remove user with id [{user.Id}]");

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while deleting data of user with userName [{userName}]");
            }

            return false;
        }

        public async Task<List<UserViewModel>> GetUsersAsync()
        {
            var userViewModels = new List<UserViewModel>();

            try
            {
                var users = await _userManager.Users.ToListAsync();
                if (users != null)
                {
                    foreach (var user in users)
                    {
                        userViewModels.Add(new UserViewModel
                        {
                            NickName = user.Nickname,
                            DateOfBirth = user.DateOfBirth,
                            ProfileImage = user.ProfileImage,
                            UserName = user.UserName,
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while getting all users");
            }

            return userViewModels;
        }

        public async Task<bool> UpdateUserDateOfBirth(string userName, DateTime dateOfBirth)
        {
            if (string.IsNullOrWhiteSpace(userName)) { throw new ArgumentNullException(nameof(userName)); }

            try
            {
                var user = await _userManager.FindByNameAsync(userName);
                if (user == null)
                {
                    return false;
                }

                user.DateOfBirth = dateOfBirth.ClearDate();

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    _logger.LogInformation($"Updated user date of birth with id [{user.Id}]");
                    return true;
                }
                else
                {
                    _logger.LogCritical($"Failed to update user date of birth with id [{user.Id}]");

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while updating date of birth with userName [{userName}]");
            }

            return false;
        }
    }
}

