using Microsoft.EntityFrameworkCore;

public class ApplicationContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public ApplicationContext(DbContextOptions<ApplicationContext> options) 
            :base(options)
        { }
         
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=lab_dotnet;Username=user_lab_dotnet;Password=1111");
        }
    }