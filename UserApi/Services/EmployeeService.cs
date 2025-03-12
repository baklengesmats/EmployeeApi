using AutoMapper;
using UserApi.Common;
using UserApi.Repository;
using UserApi.Repository.DTOs;
using UserApi.Repository.Model;

namespace UserApi.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repository;
        private readonly IMapper _mapper;

        public EmployeeService(IEmployeeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public OperationResult AddEmployee(string mail, string firstName, string lastName)
        {
            if (string.IsNullOrEmpty(mail))
            {
                return OperationResult.Fail(("Email is missing."), 400);
            }
            if (string.IsNullOrEmpty(firstName))
            {
                return OperationResult.Fail(("First name is missing."), 400);
            }
            if (string.IsNullOrEmpty(lastName))
            {
                return OperationResult.Fail(("Last name is missing."), 400);
            }

            //can't add a new employee with the same mail
            var duplicateMail = _repository.GetEmployee(mail);
            if(duplicateMail!= null)
            {
                return OperationResult.Fail(("An employee with this email already exists."), 409);
            }
            var employee = new Employee()
            {
                Created = DateTime.Now,
                Deleted = null,
                Email = mail,
                FirstName = firstName,
                LastName = lastName,
                EmployeeId = 0,
            };

            _repository.Add(employee);
            return OperationResult.Ok();
        }

        public OperationResult DeactiveEmployee(int employeeId)
        {
            var employee = _repository.GetEmployee(employeeId);
            if(employee == null)
            {
                return OperationResult.Fail($"Couldn't find employee with id: {employeeId}.", 404);
            }
            _repository.SoftDelete(employeeId);
            return OperationResult.Ok();
        }

        public OperationResult DeactiveEmployee(string mail)
        {
            var employee = _repository.GetEmployee(mail);
            if (employee == null)
            {
                return OperationResult.Fail($"Couldn't find employee with mail: {mail}.", 404);
            }
            _repository.SoftDelete(mail);
            return OperationResult.Ok();
        }

        public OperationResult DeleteEmployee(int employeeId)
        {
            var employee = _repository.GetEmployee(employeeId);
            if (employee == null)
            {
                return OperationResult.Fail($"Couldn't find employee with id: {employeeId}.", 404);
            }
            _repository.HardDeleteById(employeeId);
            return OperationResult.Ok();
        }

        public List<EmployeeDto> GetActiveEmployees()
        {
            var employees = _repository.GetActiveEmployees();
            return _mapper.Map<List<EmployeeDto>>(employees);
        }

        public EmployeeDto? GetEmployee(int employeeId)
        {
            var employee = _repository.GetEmployee(employeeId);
            return _mapper.Map<EmployeeDto>(employee);
        }

        public EmployeeDto? GetEmployee(string mail)
        {
            var employee = _repository.GetEmployee(mail);
            return _mapper.Map<EmployeeDto>(employee);
        }

        public List<EmployeeDto> GetEmployees()
        {
            var employees = _repository.GetEmployees();
            return _mapper.Map<List<EmployeeDto>>(employees);
        }

        public OperationResult ReactiveEmployee(int employeeId)
        {
            var employee = _repository.GetEmployee(employeeId);
            if (employee == null)
            {
                return OperationResult.Fail($"Couldn't find employee with id: {employeeId}.", 404);
            }
            _repository.Reactive(employeeId);
            return OperationResult.Ok();
        }

        public OperationResult ReactiveEmployee(string mail)
        {
            var employee = _repository.GetEmployee(mail);
            if (employee == null)
            {
                return OperationResult.Fail($"Couldn't find employee with mail: {mail}.", 404);
            }
            _repository.Reactive(mail);
            return OperationResult.Ok();
        }
    }
}
