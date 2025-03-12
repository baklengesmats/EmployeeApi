using UserApi.Repository.Model;

namespace UserApi.Repository
{
    public class InMemoryEmployeeRespository : IEmployeeRepository
    {
        private readonly List<Employee> _employees = new();
        private int _nextId = 1;

        public void Add(Employee employee)
        {
            // If the EmployeeId is 0 (default), assign a new auto-incremented Id.
            if (employee.EmployeeId == 0)
            {
                employee.EmployeeId = _nextId++;
            }
            _employees.Add(employee);
        }

        public Employee? GetEmployee(int employeeId)
        {
           return _employees.FirstOrDefault(u => u.EmployeeId == employeeId);
        }

        public List<Employee> GetEmployees()
        {
            return _employees;
        }
        public Employee? GetEmployee(string mail)
        {
            return _employees.FirstOrDefault(u => u.Email == mail);
        }

        public List<Employee> GetActiveEmployees()
        {
            return _employees.FindAll(u => u.Deleted == null);
        }

        public bool HardDeleteById(int employeeId)
        {
            var user = _employees.FirstOrDefault(u => u.EmployeeId == employeeId);
            if (user != null)
            {
                _employees.Remove(user);
                return true;
            }
            return false;
        }

        public bool SoftDelete(int employeeId)
        {
            var index = _employees.FindIndex(u => u.EmployeeId == employeeId);
            if(index == -1)
            {
                return false;
            }
            _employees[index].Deleted = DateTime.Now;
            return true;
        }

        public bool SoftDelete(string mail)
        {
            var index = _employees.FindIndex(u => u.Email == mail);
            if (index == -1)
            {
                return false;
            }
            _employees[index].Deleted = DateTime.Now;
            return true;
        }

        public bool Reactive(int employeeId)
        {
            var index = _employees.FindIndex(u => u.EmployeeId == employeeId);
            if (index == -1)
            {
                return false;
            }
            _employees[index].Deleted = null;
            return true;
        }

        public bool Reactive(string mail)
        {
            var index = _employees.FindIndex(u => u.Email == mail);
            if (index == -1)
            {
                return false;
            }
            _employees[index].Deleted = null;
            return true;
        }
    }
}
