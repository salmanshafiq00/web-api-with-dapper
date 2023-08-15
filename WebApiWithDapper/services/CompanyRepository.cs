using System.Data;
using Dapper;
using WebApiWithDapper.Data;
using WebApiWithDapper.DTOs;
using WebApiWithDapper.Entity;
using WebApiWithDapper.Interfaces;

namespace WebApiWithDapper.services
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly DapperContext _context;

        public CompanyRepository(DapperContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<Company>> GetCompanies()
        {
            var query = "SELECT Id, Name, Address, Country FROM Companies";

            using var connection = _context.CreateConnection();
            var companies = await connection.QueryAsync<Company>(query);
            return companies.ToList();
        }

        public async Task<Company> GetCompany(int id)
        {
            var query = "SELECT Id, Name, Address, Country  FROM Companies WHERE Id = @id";

            using var connection = _context.CreateConnection();
            var company = await connection.QuerySingleOrDefaultAsync<Company>(query, new { id });
            return company;
        }
        public async Task<Company> CreateCompany(CreateCompanyDTO company)
        {
            var query = "INSERT INTO Companies (Name, Address, Country) VALUES (@Name, @Address, @Country)";

            var parameters = new DynamicParameters();
            parameters.Add("Name", company.Name, DbType.String);
            parameters.Add("Address", company.Address, DbType.String);
            parameters.Add("Country", company.Country, DbType.String);

            using var conneciton = _context.CreateConnection();
            var id = await conneciton.QuerySingleAsync<int>(query, parameters);

            var createdCompany = new Company
            {
                Id = id,
                Name = company.Name,
                Address = company.Address,
                Country = company.Country
            };
            return createdCompany;
        }

        public async Task UpdateCompany(int id, UpdateCompanyDTO company)
        {
            var query = "UPDATE Companies SET Name = @Name, Address = @Address, Country = @Country WHERE Id = @id";

            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Int32);
            parameters.Add("Name", company.Name, DbType.String);
            parameters.Add("Address", company.Address, DbType.String);
            parameters.Add("Country", company.Country, DbType.String);

            using var conneciton = _context.CreateConnection();
            await conneciton.ExecuteAsync(query, parameters);
        }

        public async Task DeleteCompany(int id)
        {
            var query = "DELETE FROM Companies  WHERE Id = @id";
            using var conneciton = _context.CreateConnection();
            await conneciton.ExecuteAsync(query, new { id });
        }

        public async Task<Company> GetCompanyByEmployeeId(int employeeId)
        {
            var procedureName = "ShowCompanyForProvidedEmployeeId";

            var parameters = new DynamicParameters();
            parameters.Add("Id", employeeId, DbType.Int32);
            using var conneciton = _context.CreateConnection();
            var company = await conneciton.QueryFirstOrDefaultAsync<Company>(procedureName, parameters, commandType: CommandType.StoredProcedure);

            return company;
        }

        public async Task<Company?> GetCompanyEmployeesMultipleResults(int id)
        {
            var query = "SELECT * FROM Companies WHERE Id = @Id;" +
                        "SELECT * FROM Employees WHERE CompanyId = @Id";

            using var connection = _context.CreateConnection();
            using var multi = connection.QueryMultiple(query, new { id });
            var company = await multi.ReadSingleOrDefaultAsync<Company>();
            if (company is not null)
                company.Employees = (await multi.ReadAsync<Employee>()).ToList();

            return company;
        }

        public async Task<List<Company>> GetCompaniesEmployeesMultipleMapping()
        {
            var query = "SELECT * FROM Companies c JOIN Employees e ON c.Id = e.Id";

            using var connection = _context.CreateConnection();
            var companyDict = new Dictionary<int, Company>();

            var companies = await connection.QueryAsync<Company, Employee, Company>(
                query, (company, employee) =>
                {
                    if (!companyDict.TryGetValue(company.Id, out var currentCompany))
                    {
                        currentCompany = company;
                        companyDict.Add(company.Id, currentCompany);
                    }
                    currentCompany.Employees.Add(employee);
                    return currentCompany;

                }
             );

            return companies.Distinct().ToList();
        }

        public async Task CreateMultipleCompanies(List<CreateCompanyDTO> companies)
        {
            var query = "INSERT INTO Companies (Name, Address, Country) VALUES (@Name, @Address, @Country)";

            using var connection = _context.CreateConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();

            foreach(var company in companies)
            {
                var parameters = new DynamicParameters();
                parameters.Add("Name", company.Name, DbType.String);
                parameters.Add("Address", company.Address, DbType.String);
                parameters.Add("Country", company.Country, DbType.String);

                await connection.ExecuteAsync(query, parameters, transaction: transaction);
            }

            transaction.Commit();
        }

    }
}
