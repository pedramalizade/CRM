namespace CRM_Endpoint_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BusinessApiController : ControllerBase
    {
        private readonly IBusinessService _service;

        public BusinessApiController(IBusinessService service)
        {
            _service = service;
        }

        // GET: api/Business
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var businesses = await _service.GetAllAsync();
            return Ok(businesses);
        }

        // GET: api/Business/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var business = await _service.GetByIdAsync(id);
            if (business == null)
                return NotFound();

            return Ok(business);
        }

        // POST: api/Business
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Business business)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _service.CreateAsync(business);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/Business/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Business business)
        {
            if (id != business.Id)
                return BadRequest();

            if (!await _service.ExistsAsync(id))
                return NotFound();

            var updated = await _service.UpdateAsync(business);
            return Ok(updated);
        }
    }
}
