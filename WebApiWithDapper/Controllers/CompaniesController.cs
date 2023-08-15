using Microsoft.AspNetCore.Mvc;
using WebApiWithDapper.DTOs;
using WebApiWithDapper.Interfaces;

namespace WebApiWithDapper.Controllers
{
    //[Route("api/[Controller]")]
    [Route("api/Companies")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepo;

        public CompaniesController(ICompanyRepository companyRepo)
        {
            _companyRepo = companyRepo;
        }

        [HttpGet("GetCompanies")]
        public async Task<IActionResult> GetCompanies()
        {
            try
            {
                var companies = await _companyRepo.GetCompanies();
                return Ok(companies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetCompany/{id:int}")]
        public async Task<IActionResult> GetCompany(int id)
        {
            try
            {
                var company = await _companyRepo.GetCompany(id);
                if (company == null)
                    return NotFound();

                return Ok(company);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCompany(CreateCompanyDTO company)
        {
            try
            {
                var createdCompany = await _companyRepo.CreateCompany(company);
                if (createdCompany is null)
                    return NotFound();

                return CreatedAtAction(nameof(GetCompany), new { Id = createdCompany.Id }, createdCompany);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCompany(int id, UpdateCompanyDTO company)
        {
            try
            {
                var existCompany = await _companyRepo.GetCompany(id);
                if (existCompany is null)
                    return NotFound();

                await _companyRepo.UpdateCompany(id, company);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            try
            {
                var existCompany = await _companyRepo.GetCompany(id);
                if (existCompany is null)
                    return NotFound();

                await _companyRepo.DeleteCompany(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCompanyByEmployeeId(int employeeId)
        {
            try
            {
                var existCompany = await _companyRepo.GetCompanyByEmployeeId(employeeId);
                if (existCompany is null)
                    return NotFound();

                return Ok(existCompany);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}/MultipleResult")]
        public async Task<IActionResult> GetCompanyEmployeesMultipleResult(int id)
        {
            try
            {
                var company = await _companyRepo.GetCompanyEmployeesMultipleResults(id);
                if (company is null)
                    return NotFound();
                return Ok(company);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("MultipleMapping")]
        public async Task<IActionResult> GetCompaniesEmployeesMultipleMapping()
        {
            try
            {
                var companies = await _companyRepo.GetCompaniesEmployeesMultipleMapping();
                if (companies is null)
                    return NotFound();
                return Ok(companies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("CreateMultipleCompanies")]
        public async Task<IActionResult> CreateMultipleCompanies([FromBody] List<CreateCompanyDTO> companies)
        {
            try
            {
                await _companyRepo.CreateMultipleCompanies(companies);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
