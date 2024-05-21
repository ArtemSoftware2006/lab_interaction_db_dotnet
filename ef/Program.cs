using System.Reflection;
using Microsoft.EntityFrameworkCore;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Program start");
        string connectionString = "Host=localhost;Port=5432;Database=lab_dotnet;Username=user_lab_dotnet;Password=1111";
        DbContextOptionsBuilder<ApplicationContext>
        optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
 
        DbContextOptions<ApplicationContext> options = optionsBuilder
                        .UseNpgsql(connectionString)
                        .Options;

        Console.WriteLine("1. Выборка данных из таблицы");

        using (ApplicationContext context = new ApplicationContext(options)) {
            List<Employee> employees = context.Employees.ToList();

            Type type = typeof(Employee);
            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo prop in properties)
            {
                Console.Write($"\t{prop.Name}");
            }

            Console.WriteLine();

            foreach (Employee empl in employees)
            {
                Console.WriteLine($"\t{empl.Id}\t{empl.Surname}\t{empl.Age}");
            }
        }

        Console.WriteLine("2. Добавление нового пользователя");

        Employee employee = new Employee();

        Console.WriteLine("Введите фамилию сотрудника");
        employee.Surname = Console.ReadLine();
        Console.WriteLine("Введите возраст сотрудника");
        employee.Age = int.Parse(Console.ReadLine());

        using (ApplicationContext context = new ApplicationContext(options)) {
            context.Employees.Add(employee);
            int rowsAffected = context.SaveChanges();
            Console.WriteLine($"Добавлено {rowsAffected} записей");
        }

        Console.WriteLine("3. Выборка данных из таблицы после изменения");

        using (ApplicationContext context = new ApplicationContext(options))
        {
            List<Employee> employees = context.Employees.AsNoTracking().ToList();
            employees.FirstOrDefault(x => x.Id == 1).Surname = "First";
            context.SaveChanges();

            employees = context.Employees.ToList();
            foreach (Employee empl in employees)
            {
                Console.WriteLine($"\t{empl.Id}\t{empl.Surname}\t{empl.Age}");
            }
        }

        Console.WriteLine("4. Вызов хранимой процедуры");


        Console.WriteLine("Program finished");
    }
}