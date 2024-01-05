using EmployeeTimeSheetAPI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

//Create a WebApplicationBuilder and a WebApplication
//with preconfigured defaults 

var builder = WebApplication.CreateBuilder(args);
//adds the database context to the dependency injection (DI) container
var conString = builder.Configuration.GetConnectionString("EmployeeDb") ?? "Data Source = Timesheet.db";

builder.Services.AddDbContext<EmployeeTimeSheetDb>(opt => opt.UseInMemoryDatabase("EmployeeList"));
//builder.Services.AddSqlite<EmployeeTimeSheetDb>(conString);

//Add Swagger Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
//Enable the middleware for serving the generated JSON document and the Swagger UI
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

//Enable the Swagger UI in development mode only
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var employeeLists = new List<Employee>
{
    new Employee() { Id = 1, Name = "Jame"},
    new Employee() { Id = 2, Name = "Jone"},
    new Employee() { Id = 3, Name = "Smith"}
};

//create a group URL
var empItems = app.MapGroup("/employees");
empItems.MapGet("/", GetAllEmployees);

empItems.MapGet("/{id}", GetEmployeeById);
empItems.MapPost("/", CreateEmployee);
empItems.MapPut("/", UpdateEmployee);
empItems.MapDelete("/{id}", DeleteEmployee);

app.Run();


//Read all employee
static async Task<IResult> GetAllEmployees(EmployeeTimeSheetDb db)
{
    return TypedResults.Ok(await db.Employees.ToArrayAsync());
}
/*
app.MapGet("/employee", () =>{
    return employeeLists;
});*/

//Read employee by id
static async Task<IResult> GetEmployeeById(int id, EmployeeTimeSheetDb db)
{
    var emp = await db.Employees.FindAsync(id);
    return emp == null ? TypedResults.NotFound() : TypedResults.Ok(emp);
}
/*
app.MapGet("/employees/{id}", (int id) =>
{
    var employee = employeeLists.Find(s => s.Id == id);
    return employee == null ? Results.NotFound() : Results.Ok(employee);
});*/

//Add new employee
/*
app.MapPost("/employees", ([FromBody] Employee inputEmp) =>
{
    employeeLists.Add(inputEmp);
    return Results.Created($"/employees/{inputEmp.Id}", inputEmp);
});*/

static async Task<IResult> CreateEmployee([FromBody] Employee inputEmp,
EmployeeTimeSheetDb db)
{
    db.Employees.Add(inputEmp);
    await db.SaveChangesAsync();
    return TypedResults.Created($"/employees/{inputEmp.Id}", inputEmp);
};


//Update the employee
/*
app.MapPut("/employees", ([FromBody] Employee inputEmp) =>
{
    if (inputEmp is null) return Results.NotFound();

    int foundIndex = employeeLists.FindIndex(s => s.Id == inputEmp.Id);
    if (foundIndex < 0) return Results.NotFound();

    employeeLists[foundIndex] = inputEmp;
    return Results.NoContent();
});*/
static async Task<IResult> UpdateEmployee([FromBody] Employee inputEmp,
EmployeeTimeSheetDb db){
    if (inputEmp is null) return TypedResults.NotFound();

    var emp = await db.Employees.FindAsync(inputEmp.Id);
    if (emp is null) return TypedResults.NotFound();

    emp.Name = inputEmp.Name;
    await db.SaveChangesAsync();

    return TypedResults.NoContent();
}

//Delete the student
/*
app.MapDelete("/employees/{id}", (int? id) =>
{
    if (id is null) return Results.NoContent();

    int foundIndex = employeeLists.FindIndex(s => s.Id == id);
    if (foundIndex < 0) return Results.NotFound();

    employeeLists.RemoveAt(foundIndex);
    return Results.Ok(id);
});*/

static async Task<IResult> DeleteEmployee(int? id, EmployeeTimeSheetDb db)
{
    if (id is null) return TypedResults.NoContent();

    var emp = await db.Employees.FindAsync(id);
    if (emp is null) return TypedResults.NotFound();

    db.Employees.Remove(emp);
    await db.SaveChangesAsync();
    return TypedResults.Ok(id);
};

//Run the Rest API Server at Port: 7192, change in launchsetting.json
app.Run();

class Employee
{
    public int Id { get; set; }
    public string? Name { get; set; }
}

