using CRM.Domain.Entities;
using CRM_Domain.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Endpoint_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompaniesController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetCompanies([FromQuery] string start, [FromQuery] string end, [FromQuery] string category,
                                              [FromQuery] int[] selected, [FromQuery] int page = 1,
                                              [FromQuery] int pageSize = 10, [FromQuery] string sortExpression = "Id")
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _companyService.GetCompaniesAsync(start, end, category, selected, page, pageSize, sortExpression);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetCompany(int id)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var company = await _companyService.GetCompanyAsync(id, userId.Value);
            if (company == null) return NotFound();

            return Ok(company);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateCompany([FromBody] Company company)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var created = await _companyService.CreateCompanyAsync(company, userId.Value);
            return CreatedAtAction(nameof(GetCompany), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateCompany(int id, [FromBody] Company company)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var success = await _companyService.UpdateCompanyAsync(id, company, userId.Value);
            if (!success) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var success = await _companyService.DeleteCompanyAsync(id, userId.Value);
            if (!success) return NotFound();

            return NoContent();
        }

        private int? GetUserId()
        {
            var userClaim = User.FindFirst("user");
            if (userClaim == null || string.IsNullOrEmpty(userClaim.Value))
                return null;

            // در اینجا باید UserId واقعی از دیتابیس لود بشه 
            // یا اگر داخل Claim ذخیره کردی مستقیم برگردونی
            return int.TryParse(userClaim.Value, out var userId) ? userId : null;
        }
    }
}
