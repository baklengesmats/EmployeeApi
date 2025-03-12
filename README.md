# EmployeeApi
Simple employee api.
That can fetch Employees, Add new Employee, Soft and hard delete employees.
The employees data is saved in memory.

I Added users also, admin and regular. To test the authorization.
The users will be loaded in through the user.json file in the TestData folder.
The password for both the users are "test123" in the json is hashed.

Use the http://localhost:5000/api/Auth endpoint to fetch the jwt bearer token. 
Use the jwt bearer token the ruqeust header. Can be used in swagger or postman. 
In swagger it need to be passed in into the autherize button on the right of the swagger page.
There is also a Employee API.postman_collection.json to import in your postman if you want to 
test the endpoints from there.


## Endpoints
The default port set in the appsettings.json is 5000
#### Only fetch employee and users will be open to all. The rest need some kind of authorization.

### Auth 
fetch the Jwt bearer token.
- http://localhost:5000/api/Auth?mail={username}&password={password} - parameter(string,string) admin (username: test1@test.se, password: test123)
regular user (username: test2@test.se, password: test123)  

### Employee
#### Authorize functions - need to be logged in as admin or regular
- http://localhost:5000/api/Employee/add  - add new employee - params(string,string,string)
(employee must have unique mail when adding a new employee.)
- http://localhost:5000/api/Employee/deactive?employeeId={id} - soft delete employee by id - params(int id)
- http://localhost:5000/api/Employee/deactiveByMail?mail={mail} - soft delete employee by mail - params(string mail)
- http://localhost:5000/api/Employee/Reactive?employeeId={id} - remove soft delete on employee - params(int id)
- http://localhost:5000/api/Employee/ReactiveByMail?mail={mail} - Remove soft delete employee by mail - params(string mail)
#### Admin rights
- http://localhost:5000/api/Employee/delete/{id} - Hard delete employee by id - params(int id)
#### No authorize
- http://localhost:5000/api/Employee/{id} - fetch specific employee
- http://localhost:5000/api/Employee/ - fetch all the employees
- http://localhost:5000/api/Employee/active - fetch all the active employees

### System users
#### Authorize functions - need to be logged in as admin or regular
- http://localhost:5000/api/Systemuser/deactive - soft delete System users by id - params(int)
- http://localhost:5000/api/Systemuser/deactiveByMail - soft delete System users by mail - params(string)
#### Admin rights
- http://localhost:5000/api/Systemuser/delete - Hard delete System users by id - params(int)
#### No authorize
- http://localhost:5000/api/Systemuser/ - fetch all the System users
- http://localhost:5000/api/Systemuser/active - fetch all the active System users

## Installation
- Needs .NET 8 https://dotnet.microsoft.com/en-us/download/dotnet/8.0
1. In the project folder, run "dotnet restore" to restore dependencies.
2. In the same project folder "dotnet build" to build the project.
3. To run the api, run "dotnet run" in the project folder.
4. To run the test, run "dotnet test" in the project folder.


## Key Points
### JWT Authentication:
JWT Bearer authentication. The token validation parameters (issuer, audience, key, lifetime, etc.) are read from the configuration (appsettings.json).

### Dependency Injection:
#### Registers the following services and repositories:
- IUserRepository is registered with the in-memory implementation InMemoryUserRespository.
- IEmployeeRepository is registered with the in-memory implementation InMemoryEmployeeRespository.
#### Services:
- IAuthService is registered with SystemUserAuthService.
- ISystemUserService and IEmployeeService are registered as scoped services.

### AutoMapper Registration:
AutoMapper is registered so that it automatically scans for mapping profiles within the assembly. This simplifies object-to-object mapping throughout the application.

### Data Seeding:
Calls DataSeeder.SeedAdminUsers(app) to seed initial admin user data from a JSON file located in the TestData folder. This ensures that a default admin user is available when the application starts.
