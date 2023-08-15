namespace WebApiWithDapper.Entity
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }

        public virtual IList<Employee> Employees { get; set; } = new List<Employee>();
    }
}
