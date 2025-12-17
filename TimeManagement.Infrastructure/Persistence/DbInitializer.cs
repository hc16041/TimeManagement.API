using TimeManagement.Domain.Entities;
using TimeManagement.Domain.Enums;

namespace TimeManagement.Infrastructure.Persistence;

public static class DbInitializer
{
    public static void Seed(AppDbContext context)
    {
        context.Database.EnsureCreated();

        // 1. Crear Departamentos
        if (!context.Departments.Any())
        {
            var depts = new List<Department>
            {
                new Department { Name = "Departamento de Tecnología", CostCenterCode = "IT-001" }, // 
                new Department { Name = "Recursos Humanos", CostCenterCode = "HR-001" }
            };
            context.Departments.AddRange(depts);
            context.SaveChanges();
        }

        // 2. Crear Empleados (Jerarquía del PDF)
        if (!context.Employees.Any())
        {
            var techDept = context.Departments.First(d => d.Name == "Departamento de Tecnología");

            // A. El Gerente de Área (Nivel 3) 
            var manager = new Employee
            {
                FullName = "Ing. William Vasquez",
                EmployeeCode = "0001",
                Email = "wvasquez@empresa.com",
                DepartmentId = techDept.Id,
                ReportsToId = null // Es el jefe máximo del área
            };
            context.Employees.Add(manager);
            context.SaveChanges(); // Guardamos para obtener su ID

            // Actualizamos el departamento con su Gerente
            techDept.ManagerId = manager.Id;
            context.Departments.Update(techDept);

            // B. El Jefe Inmediato (Nivel 2) 
            var boss = new Employee
            {
                FullName = "Emanuel Acevedo",
                EmployeeCode = "0002",
                Email = "eacevedo@empresa.com",
                DepartmentId = techDept.Id,
                ReportsToId = manager.Id // Reporta a William
            };
            context.Employees.Add(boss);
            context.SaveChanges();

            // C. El Empleado Solicitante (Nivel 1) 
            var employee = new Employee
            {
                FullName = "José Fernando Hernández Castellanos",
                EmployeeCode = "0011187", // 
                Email = "jhernandez@empresa.com",
                DepartmentId = techDept.Id,
                ReportsToId = boss.Id // Reporta a Emanuel 
            };
            context.Employees.Add(employee);
            context.SaveChanges();

            // 3. Crear Saldos Iniciales (Bolsa de horas)
            context.TimeBalances.Add(new TimeBalance
            {
                EmployeeId = employee.Id,
                Year = DateTime.Now.Year,
                VacationHoursAvailable = 120, // 15 días * 8h
                CompensatoryHoursAvailable = 0
            });
            context.SaveChanges();
        }
    }
}