using Microsoft.AspNetCore.Identity;
using Soccer.Common.Enums;
using Soccer.Common.Models;
using Soccer.Web.Data.Entities;
using Soccer.Web.Models;
using System;
using System.Threading.Tasks;

namespace Soccer.Web.Helpers
{
    public interface IUserHelper
    {
        Task LogoutAsync();
        Task CheckRoleAsync(string roleName);
        Task<UserEntity> GetUserByIdAsync(Guid userId);
        Task<UserEntity> GetUserByEmailAsync(string email);
        Task<SignInResult> LoginAsync(LoginViewModel model);
        Task<IdentityResult> UpdateUserAsync(UserEntity user);
        Task AddUserToRoleAsync(UserEntity user, string roleName);
        Task<string> GeneratePasswordResetTokenAsync(UserEntity user);
        Task<bool> IsUserInRoleAsync(UserEntity user, string roleName);
        Task<string> GenerateEmailConfirmationTokenAsync(UserEntity user);
        Task<IdentityResult> AddUserAsync(UserEntity user, string password);
        Task<IdentityResult> ConfirmEmailAsync(UserEntity user, string token);
        Task<SignInResult> ValidatePasswordAsync(UserEntity user, string password);
        Task<UserEntity> AddUserAsync(AddUserViewModel model, string path, UserType userType);
        Task<IdentityResult> ResetPasswordAsync(UserEntity user, string token, string password);
        Task<IdentityResult> ChangePasswordAsync(UserEntity user, string oldPassword, string newPassword);
        UserResponse ToUserResponse(UserEntity user);
    }
}