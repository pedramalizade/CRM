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
                throw new Exception("این نام کاربری قبلاً ثبت شده است.");

            var role = await _roleRepository.GetByIdAsync(user.RoleId);
            if (role == null)
                throw new Exception("نقش انتخاب‌شده معتبر نیست.");

            user.Password = HashPassword(user.Password);
            user.IsDeleted = 0;

            await _userRepository.AddAsync(user);
            return true;
            //if (await _userRepository.ExistsByLoginAsync(user.Login))
            //    return false;

            //user.Password = HashPassword(user.Password);
            //user.IsDeleted = 0;
            //await _userRepository.AddAsync(user);
            //return true;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            // بررسی نقش معتبر
            var role = await _roleRepository.GetByIdAsync(user.RoleId);
            if (role == null)
                throw new Exception("نقش انتخاب‌شده معتبر نیست.");

            // جلوگیری از حذف آخرین ادمین
            var oldUser = await _userRepository.GetByIdAsync(user.Id);
            if (oldUser == null)
                throw new Exception("کاربر یافت نشد.");

            if (oldUser.RoleId == 1 && user.RoleId != 1) // تغییر نقش Admin به چیز دیگه
            {
                int adminCount = await _userRepository.CountAdminsAsync();
                if (adminCount <= 1)
                    throw new Exception("حداقل یک مدیر سیستم باید وجود داشته باشد.");
            }

            // هش کردن پسورد در صورت نیاز
            if (!IsMD5(user.Password))
                user.Password = HashPassword(user.Password);

            await _userRepository.UpdateAsync(user);
            return true;
            //if (!IsMD5(user.Password))
            //    user.Password = HashPassword(user.Password);

            //await _userRepository.UpdateAsync(user);
            //return true;
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new Exception("کاربر یافت نشد.");

            if (user.RoleId == 1) // ادمین
            {
                int adminCount = await _userRepository.CountAdminsAsync();
                if (adminCount <= 1)
                    throw new Exception("نمی‌توان آخرین مدیر سیستم را حذف کرد.");
            }

            await _userRepository.DeleteAsync(id);
            //await _userRepository.DeleteAsync(id);
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
