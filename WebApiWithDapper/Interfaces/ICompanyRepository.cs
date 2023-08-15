using WebApiWithDapper.DTOs;
using WebApiWithDapper.Entity;

namespace WebApiWithDapper.Interfaces
{
    public interface ICompanyRepository
    {
        Task<IEnumerable<Company>> GetCompanies();
        Task<Company> GetCompany(int id);
        Task<Company> CreateCompany(CreateCompanyDTO company);
        Task UpdateCompany(int id, UpdateCompanyDTO companyDTO);
        Task DeleteCompany(int id);
        Task<Company> GetCompanyByEmployeeId(int employeeId);
        Task<Company> GetCompanyEmployeesMultipleResults(int id);
        Task<List<Company>> GetCompaniesEmployeesMultipleMapping();
        Task CreateMultipleCompanies(List<CreateCompanyDTO> companies);
    }
}
