namespace CRM_Endpoint_WebApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly INoteService _noteService;

        public NotesController(INoteService noteService)
        {
            _noteService = noteService;
        }

        // GET: api/Notes
        [HttpGet]
        public async Task<IActionResult> GetNotes()
        {
            // گرفتن یوزر لاگین شده از Claims
            var userClaim = User.FindFirst("user");
            if (userClaim == null)
                return Unauthorized();

            // همه نوت‌های کاربر
            var notes = await _noteService.GetUserNotesAsync(int.Parse(userClaim.Value));
            return Ok(notes);
        }

        // GET: api/Notes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNote(int id)
        {
            var userClaim = User.FindFirst("user");
            if (userClaim == null)
                return Unauthorized();

            var note = await _noteService.GetNoteByIdAsync(id, int.Parse(userClaim.Value));
            if (note == null)
                return NotFound();

            return Ok(note);
        }

        // POST: api/Notes
        [HttpPost]
        public async Task<IActionResult> CreateNote([FromBody] Note note)
        {
            var userClaim = User.FindFirst("user");
            if (userClaim == null)
                return Unauthorized();

            await _noteService.AddNoteAsync(note, int.Parse(userClaim.Value));
            return Ok("یادداشت با موفقیت ثبت شد.");
        }

        // PUT: api/Notes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNote(int id, [FromBody] Note note)
        {
            var userClaim = User.FindFirst("user");
            if (userClaim == null)
                return Unauthorized();

            if (id != note.Id)
                return BadRequest("شناسه یادداشت معتبر نیست.");

            try
            {
                await _noteService.UpdateNoteAsync(note, int.Parse(userClaim.Value));
                return Ok("یادداشت ویرایش شد.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        // DELETE: api/Notes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNote(int id)
        {
            var userClaim = User.FindFirst("user");
            if (userClaim == null)
                return Unauthorized();

            try
            {
                await _noteService.DeleteNoteAsync(id, int.Parse(userClaim.Value));
                return Ok("یادداشت حذف شد.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }
    }
}
