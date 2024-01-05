using Microsoft.EntityFrameworkCore;

namespace EmployeeTimeSheetAPI
{
    class EmployeeTimeSheetDb : DbContext
    {
        public EmployeeTimeSheetDb(DbContextOptions<EmployeeTimeSheetDb> options)
        : base(options) { }
        public DbSet<Employee> Employees => Set<Employee>();
    }
}
