namespace CRM_Endpoint_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IUserService _userService;

        public AccountsController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound("کاربر پیدا نشد");
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (await _userService.ValidateLoginAsync(model.UserName, model.Password))
            {
                string role = await _userService.GetRoleNameAsync(model.UserName);
                return Ok(new { message = "ورود موفقیت‌آمیز", role });
            }
            return Unauthorized("نام کاربری یا رمز عبور اشتباه است");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            try
            {
                await _userService.RegisterUserAsync(user);
                return Ok("ثبت‌نام با موفقیت انجام شد.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(int id, [FromBody] User user)
        {
            if (id != user.Id)
                return BadRequest("شناسه کاربر معتبر نیست.");

            try
            {
                await _userService.UpdateUserAsync(user);
                return Ok("ویرایش با موفقیت انجام شد.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _userService.DeleteUserAsync(id);
                return Ok("کاربر حذف شد.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
