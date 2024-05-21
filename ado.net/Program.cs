using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Xml;
using Npgsql;

internal class Program
{
    class Employee {
        public int Id { get; set;}
        public string Surname { get; set;}
        public int Age { get; set; }
    }
    private static void Main(string[] args)
    {
        Console.WriteLine("Program start");
        string connectionString = "Host=localhost;Port=5432;Database=lab_dotnet;Username=user_lab_dotnet;Password=1111";

        Console.WriteLine("1. Выборка данных из таблицы");

        NpgsqlConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();
        string tableName = "employees";
        string selectEmployeesString = $"SELECT * FROM public.{tableName}";
        NpgsqlCommand selectEmployees = new NpgsqlCommand(selectEmployeesString, connection);
        NpgsqlDataReader reader = selectEmployees.ExecuteReader();

        for (int i = 0; i < reader.FieldCount; i++)
        {
            Console.Write(reader.GetName(i) + "\t");
        }
        Console.WriteLine();
        while (reader.Read())
        {
            writeValue(reader.GetValue(0));
            writeValue(reader.GetString(1));
            writeValue(reader.GetInt32(2));
            Console.WriteLine();
        }

        reader.Close();
        selectEmployees.Dispose();
        
        Console.WriteLine("2. Добавление нового пользователя");

        Employee employee = new Employee();

        Console.WriteLine("Введите фамилию сотрудника");
        employee.Surname = Console.ReadLine();
        Console.WriteLine("Введите возраст сотрудника");
        employee.Age = int.Parse(Console.ReadLine());

        NpgsqlCommand insertEmployee = new NpgsqlCommand($"INSERT INTO public.employees (surname, age) VALUES (@surname, @age);", connection);

        NpgsqlParameter surname = new NpgsqlParameter("@surname", employee.Surname);
        NpgsqlParameter age = new NpgsqlParameter("@age", employee.Age);

        insertEmployee.Parameters.Add(surname);
        insertEmployee.Parameters.Add(age);

        Console.WriteLine(insertEmployee.CommandText);

        int rowsAffected = insertEmployee.ExecuteNonQuery();

        insertEmployee.Dispose();

        Console.WriteLine($"Добавлено {rowsAffected} записей");

        Console.WriteLine("3. Выборка данных из таблицы после изменения");

        NpgsqlDataAdapter dataAdapter= new NpgsqlDataAdapter(selectEmployeesString, connection);
        DataSet employeesDataSet = new DataSet();
        dataAdapter.Fill(employeesDataSet); 

        printDataSet(employeesDataSet);

        DataTable employeesTable = employeesDataSet.Tables[0];
        DataRow newRow = employeesTable.NewRow();
        newRow["surname"]="NewRow";
        newRow["age"]=100;
        employeesTable.Rows.Add(newRow);

        NpgsqlCommandBuilder commandBuilder = new NpgsqlCommandBuilder(dataAdapter);
        dataAdapter.Update(employeesDataSet);
        // альтернативный способ - обновление только одной таблицы
        //adapter.Update(dt);
        // заново получаем данные из бд
        // очищаем полностью DataSet
        employeesDataSet.Clear();
        // перезагружаем данные
        dataAdapter.Fill(employeesDataSet);

        printDataSet(employeesDataSet);

        Console.WriteLine("4. Вызов хранимой процедуры");



        connection.Close();
        Console.WriteLine("Program finished");
    }

    static void writeValue<T>(T value) {
        Console.Write(value+"\t");
    }

    static void printDataSet(DataSet dataSet) {
        foreach (DataTable dt in dataSet.Tables)
        {
            Console.WriteLine(dt.TableName); // название таблицы
            // перебор всех столбцов
            foreach(DataColumn column in dt.Columns)
                Console.Write("{0}\t", column.ColumnName);
            Console.WriteLine();
            // перебор всех строк таблицы
            foreach (DataRow row in dt.Rows)
            {
                // получаем все ячейки строки
                var cells = row.ItemArray;
                foreach (object cell in cells)
                    Console.Write("{0}\t", cell);
                Console.WriteLine();
            }
        }
    }
}