using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UserApi.Common;
using UserApi.Repository.Model;
using UserApi.Services;

namespace UserApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }


        [HttpPost("add")]
        [Authorize]
        public IActionResult AddEmployees(string firstname, string lastname, string mail)
        {
            var result = _employeeService.AddEmployee(mail, firstname, lastname);
            if (!string.IsNullOrEmpty(result.Errors))
            {
                return Problem(
                    title: result.Errors,
                    statusCode: result.StatusCode
                    );
            }

            return Created();
        }

        [HttpGet("{id}")]
        public IActionResult GetEmployee(int id)
        {
            var employee = _employeeService.GetEmployee(id);
            if(employee == null)
            {
                return NotFound(new { Message = $"Couldn't find employee with id: {id}." });
            }
            return Ok(employee);
        }

        [HttpGet]
        public IActionResult GetAllEmployees()
        {
            var employees = _employeeService.GetEmployees();
            return Ok(employees);
        }

        [HttpGet("active")]
        public IActionResult GetActiveEmployees()
        {
            var employees = _employeeService.GetActiveEmployees();
            return Ok(employees);
        }
        [HttpPatch("reactive")]
        [Authorize]
        public IActionResult ReactiveEmployeeById(int id)
        {
            var result = _employeeService.ReactiveEmployee(id);
            return ProcessOperationResult(result);
        }
        [HttpPatch("reactiveByMail")]
        [Authorize]
        public IActionResult ReactiveEmployeeByMail(string mail)
        {
            var result = _employeeService.ReactiveEmployee(mail);
            return ProcessOperationResult(result);
        }
        [HttpPatch("deactive")]
        [Authorize]
        public IActionResult SoftDeleteEmployeeById(int id)
        {
            var result = _employeeService.DeactiveEmployee(id);
            return ProcessOperationResult(result);
        }
        [HttpPatch("deactiveByMail")]
        [Authorize]
        public IActionResult SoftDeleteEmployeeByMail(string mail)
        {
            var result = _employeeService.DeactiveEmployee(mail);
            return ProcessOperationResult(result);
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult HardDeleteEmployeeById(int id)
        {
            var result = _employeeService.DeleteEmployee(id);
            return ProcessOperationResult(result);
        }

        private IActionResult ProcessOperationResult(OperationResult result)
        {
            if (result.StatusCode == StatusCodes.Status404NotFound)
            {
                return NotFound(new { Message = result.Errors });
            }
            else if (result.Errors != null && result.Errors.Any())
            {
                return Problem(
                    title: "An unexpected error occurred.",
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
            return NoContent();
        }

    }
}
