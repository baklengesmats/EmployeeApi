using UserApi.Common;
using UserApi.Repository.DTOs;
using UserApi.Repository.Model;

namespace UserApi.Services
{
    public interface IEmployeeService
    {
        public OperationResult AddEmployee(string mail, string firstName, string lastName);
        public EmployeeDto? GetEmployee(int employeeId);
        public EmployeeDto? GetEmployee(string mail);
        public List<EmployeeDto> GetEmployees();
        public List<EmployeeDto> GetActiveEmployees();
        public OperationResult ReactiveEmployee(int employeeId);
        public OperationResult ReactiveEmployee(string mail);
        public OperationResult DeactiveEmployee(int employeeId);
        public OperationResult DeactiveEmployee(string mail);
        public OperationResult DeleteEmployee(int employeeId);
    }
}
