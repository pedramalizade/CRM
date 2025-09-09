namespace CRM_Endpoint_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ContactsController : ControllerBase
    {
        private readonly IContactService _contactService;

        public ContactsController(IContactService contactService)
        {
            _contactService = contactService;
        }

        private int? GetUserId()
        {
            var userIdClaim = User.FindFirst("userId");
            if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
            {
                return null;
            }
            return int.Parse(userIdClaim.Value);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? filter)
        {
            var contacts = await _contactService.GetAllAsync(filter);
            return Ok(contacts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var contact = await _contactService.GetByIdAsync(id);
            if (contact == null)
                return NotFound();
            return Ok(contact);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Contact contact)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var created = await _contactService.CreateAsync(contact, userId.Value);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Contact contact)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var updated = await _contactService.UpdateAsync(id, contact, userId.Value);
            if (updated == null) return NotFound();

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var deleted = await _contactService.DeleteAsync(id, userId.Value);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}
