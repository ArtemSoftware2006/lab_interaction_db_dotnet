using System.Data;
using System.Reflection;
using Dapper;
using Npgsql;
internal class Program
{
    
    private static void Main(string[] args)
    {
        Console.WriteLine("Program start");
        string connectionString = "Host=localhost;Port=5432;Database=lab_dotnet;Username=user_lab_dotnet;Password=1111";

        Console.WriteLine("1. Выборка данных из таблицы");

        using(IDbConnection connection = new NpgsqlConnection(connectionString)) 
        {
            List<Employee> employees = connection.Query<Employee>("SELECT * FROM public.employees").ToList();

            Type employeeType = typeof(Employee);
            PropertyInfo[] properties = employeeType.GetProperties();

            foreach(PropertyInfo prop in properties)
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

        using (IDbConnection connection = new NpgsqlConnection(connectionString)) 
        {
            string insertUserSql = "INSERT INTO public.employees (surname, age) VALUES(@Surname, @Age) RETURNING id;;";

            int newId = connection.QuerySingle<int>(insertUserSql, employee);

            Console.WriteLine($"Id довленного пользователя: {newId}");
        }

        Console.WriteLine("3. Выборка данных из таблицы после изменения");

        using (IDbConnection connection = new NpgsqlConnection(connectionString))
        {
            string selectEmployeeSql = "SELECT * FROM public.employees";
            var reader = connection.ExecuteReader(selectEmployeeSql);
            DataTable TableEmployee = new DataTable();

            TableEmployee.Load(reader);
            foreach (DataColumn col in TableEmployee.Columns)
            {
                Console.Write($"\t{col}");
            }
            Console.WriteLine();
            foreach (DataRow row in TableEmployee.Rows)
            {
                // получаем все ячейки строки
                var cells = row.ItemArray;
                foreach (object cell in cells)
                    Console.Write("{0}\t", cell);
                Console.WriteLine();
            }
             
        }

        Console.WriteLine("4. Вызов хранимой процедуры");


        Console.WriteLine("Program finished");
    }


}
