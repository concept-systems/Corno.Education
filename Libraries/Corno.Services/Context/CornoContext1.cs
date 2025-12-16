using System.Data.Entity;
using Corno.Models.Mapping;

namespace Corno.Models
{
    public partial class CornoContext : DbContext
    {
        static CornoContext()
        {
            Database.SetInitializer<CornoContext>(null);
        }

        public CornoContext(string _connectionString)
            : base(_connectionString)
        {
        }

        // Masters
        public DbSet<Company> Companies { get; set; }
        public DbSet<FinancialYear> FinancialYears { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<MachineType> MachineTypes { get; set; }
        public DbSet<Machine> Machines { get; set; }
        public DbSet<SubAssembly> SubAssemblies { get; set; }
        public DbSet<ProblemType> ProblemTypes { get; set; }
        public DbSet<Problem> Problems { get; set; }
        public DbSet<EmployeeType> EmployeeType { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<ConsumableType> ConsumableTypes { get; set; }

        // Transactions
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<ScanLog> ScanLogs { get; set; }

        // Notching Department
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<ManPowerAllocationMachine> ManPowerAllocationMachines { get; set; }
        public DbSet<ManPowerAllocationProduct> ManPowerAllocationProducts { get; set; }
        public DbSet<Production> Productions { get; set; }
        public DbSet<Scrap> Scraps { get; set; }
        public DbSet<Consumable> Consumables { get; set; }


        //// VBI
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<RadiusTolerance> Tolerances { get; set; }
        public DbSet<Scan> Scans { get; set; }
        public DbSet<Hole> Holes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Masters
            modelBuilder.Configurations.Add(new CompanyMap());
            modelBuilder.Configurations.Add(new FinancialYearMap());
            modelBuilder.Configurations.Add(new UnitMap());
            modelBuilder.Configurations.Add(new MachineTypeMap());
            modelBuilder.Configurations.Add(new MachineMap());
            modelBuilder.Configurations.Add(new SubAssemblyMap());
            modelBuilder.Configurations.Add(new ProblemTypeMap());
            modelBuilder.Configurations.Add(new ProblemMap());
            modelBuilder.Configurations.Add(new EmployeeTypeMap());
            modelBuilder.Configurations.Add(new EmployeeMap());
            modelBuilder.Configurations.Add(new DepartmentMap());
            modelBuilder.Configurations.Add(new ProductTypeMap());
            modelBuilder.Configurations.Add(new ConsumableTypeMap());

            // Transactions
            //modelBuilder.Configurations.Add(new BOMMap());
            modelBuilder.Configurations.Add(new ComplaintMap());
            //modelBuilder.Configurations.Add(new DailyPlanMap());

            // Notching Department
            modelBuilder.Configurations.Add(new AttendanceMap());
            modelBuilder.Configurations.Add(new ManPowerAllocationMachineMap());
            modelBuilder.Configurations.Add(new ManPowerAllocationProductMap());
            modelBuilder.Configurations.Add(new ProductionMap());
            modelBuilder.Configurations.Add(new ScrapMap());
            modelBuilder.Configurations.Add(new ConsumableMap());

            //modelBuilder.Configurations.Add(new CityMap());
            //modelBuilder.Configurations.Add(new CustomerMap());
            //modelBuilder.Configurations.Add(new ItemTypeMap());
            modelBuilder.Configurations.Add(new ScanLogMap());

            // VBI
            modelBuilder.Configurations.Add(new RecipeMap());
            modelBuilder.Configurations.Add(new RadiusToleranceMap());
            modelBuilder.Configurations.Add(new ScanMap());
            modelBuilder.Configurations.Add(new HoleMap());
        }
    }
}
