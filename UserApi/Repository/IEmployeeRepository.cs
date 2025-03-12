using UserApi.Repository.Model;

namespace UserApi.Repository
{
    public interface IEmployeeRepository
    {
        public void Add(Employee employee);
        public bool SoftDelete(int employeeId);
        public bool SoftDelete(string mail);
        public bool Reactive(int employeeId);
        public bool Reactive(string mail);
        public bool HardDeleteById(int employeeId);
        public Employee? GetEmployee(int employeeId);
        public Employee? GetEmployee(string mail);
        public List<Employee> GetEmployees();
        public List<Employee> GetActiveEmployees();

    }
}
