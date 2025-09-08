using CRM.Domain.Entities;
using CRM_Domain.Interfaces.Repository;
using CRM_Domain.Interfaces.Service;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace CRM_Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public UserService(IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        public async Task<bool> ValidateLoginAsync(string userName, string password)
        {
            var user = await _userRepository.GetByLoginAsync(userName);
            if (user == null) return false;

            string hashedPassword = HashPassword(password);
            return hashedPassword == user.Password;
        }

        public async Task<string> GetRoleNameAsync(string userName)
        {
            var user = await _userRepository.GetByLoginAsync(userName);
            if (user == null) return string.Empty;

            var role = await _roleRepository.GetByIdAsync(user.RoleId);
            return role?.Name ?? string.Empty;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<bool> RegisterUserAsync(User user)
        {
            if (await _userRepository.ExistsByLoginAsync(user.Login))
                return false;

            user.Password = HashPassword(user.Password);
            user.IsDeleted = 0;
            await _userRepository.AddAsync(user);
            return true;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            if (!IsMD5(user.Password))
                user.Password = HashPassword(user.Password);

            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task DeleteUserAsync(int id)
        {
            await _userRepository.DeleteAsync(id);
        }

        private string HashPassword(string password)
        {
            using var md5 = MD5.Create();
            byte[] hashed = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
            return string.Concat(hashed.Select(b => b.ToString("x2")));
        }

        private static bool IsMD5(string input)
        {
            return !string.IsNullOrEmpty(input) && Regex.IsMatch(input, "^[0-9a-fA-F]{32}$");
        }
    }
}
