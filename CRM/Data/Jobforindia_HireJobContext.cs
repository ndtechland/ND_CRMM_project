using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using CRM.Models.Jobcontext;

namespace CRM.Data
{
    public partial class Jobforindia_HireJobContext : DbContext
    {
        public Jobforindia_HireJobContext()
        {
        }

        public Jobforindia_HireJobContext(DbContextOptions<Jobforindia_HireJobContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AboutU> AboutUs { get; set; } = null!;
        public virtual DbSet<AddContact> AddContacts { get; set; } = null!;
        public virtual DbSet<AggregatedCounter> AggregatedCounters { get; set; } = null!;
        public virtual DbSet<Applieddate> Applieddates { get; set; } = null!;
        public virtual DbSet<ApplyLeaveNews> ApplyLeaveNews { get; set; } = null!;
        public virtual DbSet<Applyjob> Applyjobs { get; set; } = null!;
        public virtual DbSet<ApprovedPresnolInfo> ApprovedPresnolInfos { get; set; } = null!;
        public virtual DbSet<Approvedbankdetail> Approvedbankdetails { get; set; } = null!;
        public virtual DbSet<BJob> BJobs { get; set; } = null!;
        public virtual DbSet<Blog> Blogs { get; set; } = null!;
        public virtual DbSet<Bookmark> Bookmarks { get; set; } = null!;
        public virtual DbSet<CJobOpen> CJobOpens { get; set; } = null!;
        public virtual DbSet<Carrier> Carriers { get; set; } = null!;
        public virtual DbSet<CarrierStatus> CarrierStatuses { get; set; } = null!;
        public virtual DbSet<City> Cities { get; set; } = null!;
        public virtual DbSet<Companycategory> Companycategories { get; set; } = null!;
        public virtual DbSet<Companytype> Companytypes { get; set; } = null!;
        public virtual DbSet<ContactUss> ContactUsses { get; set; } = null!;
        public virtual DbSet<Counter> Counters { get; set; } = null!;
        public virtual DbSet<Country> Countries { get; set; } = null!;
        public virtual DbSet<CreateProfile> CreateProfiles { get; set; } = null!;
        public virtual DbSet<DropIndustry> DropIndustries { get; set; } = null!;
        public virtual DbSet<Educationtype> Educationtypes { get; set; } = null!;
        public virtual DbSet<EmployeeRegistration> EmployeeRegistrations { get; set; } = null!;
        public virtual DbSet<Employeeattendance> Employeeattendances { get; set; } = null!;
        public virtual DbSet<Hash> Hashes { get; set; } = null!;
        public virtual DbSet<Job> Jobs { get; set; } = null!;
        public virtual DbSet<JobParameter> JobParameters { get; set; } = null!;
        public virtual DbSet<JobQueue> JobQueues { get; set; } = null!;
        public virtual DbSet<JobTitle> JobTitles { get; set; } = null!;
        public virtual DbSet<Leave> Leaves { get; set; } = null!;
        public virtual DbSet<LeaveApply> LeaveApplies { get; set; } = null!;
        public virtual DbSet<LeaveType> LeaveTypes { get; set; } = null!;
        public virtual DbSet<Leavemaster> Leavemasters { get; set; } = null!;
        public virtual DbSet<List> Lists { get; set; } = null!;
        public virtual DbSet<MLocation> MLocations { get; set; } = null!;
        public virtual DbSet<Middlebanner> Middlebanners { get; set; } = null!;
        public virtual DbSet<Organization> Organizations { get; set; } = null!;
        public virtual DbSet<Payment> Payments { get; set; } = null!;
        public virtual DbSet<Postedby> Postedbies { get; set; } = null!;
        public virtual DbSet<Qualification> Qualifications { get; set; } = null!;
        public virtual DbSet<Registration> Registrations { get; set; } = null!;
        public virtual DbSet<Rolecategory> Rolecategories { get; set; } = null!;
        public virtual DbSet<Salary> Salaries { get; set; } = null!;
        public virtual DbSet<Schema> Schemas { get; set; } = null!;
        public virtual DbSet<Server> Servers { get; set; } = null!;
        public virtual DbSet<Service> Services { get; set; } = null!;
        public virtual DbSet<Serviceslist> Serviceslists { get; set; } = null!;
        public virtual DbSet<Set> Sets { get; set; } = null!;
        public virtual DbSet<State> States { get; set; } = null!;
        public virtual DbSet<State1> States1 { get; set; } = null!;
        public virtual DbSet<TblAddExperTise> TblAddExperTises { get; set; } = null!;
        public virtual DbSet<TblAdminLogin> TblAdminLogins { get; set; } = null!;
        public virtual DbSet<TblUploadBanner> TblUploadBanners { get; set; } = null!;
        public virtual DbSet<Testimonial> Testimonials { get; set; } = null!;
        public virtual DbSet<WorkMode> WorkModes { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=103.154.184.118;Database=Jobforindia_HireJob;User ID=Jobforindia_HireJob;Password=Job@#123456#@$;Trusted_Connection=False;TrustServerCertificate=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Jobforindia_HireJob");

            modelBuilder.Entity<AboutU>(entity =>
            {
                entity.Property(e => e.Tittle).HasColumnName("tittle");
            });

            modelBuilder.Entity<AddContact>(entity =>
            {
                entity.Property(e => e.CallNumber).HasColumnName("Call_Number");
            });

            modelBuilder.Entity<AggregatedCounter>(entity =>
            {
                entity.HasKey(e => e.Key)
                    .HasName("PK_HangFire_CounterAggregated");

                entity.ToTable("AggregatedCounter", "HangFire");

                entity.HasIndex(e => e.ExpireAt, "IX_HangFire_AggregatedCounter_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Applieddate>(entity =>
            {
                entity.ToTable("Applieddate");

                entity.Property(e => e.AppliedDate1).HasColumnName("AppliedDate");

                entity.Property(e => e.Typeofleaveid).HasColumnName("typeofleaveid");
            });

            modelBuilder.Entity<ApplyLeaveNews>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CountLeave).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<Applyjob>(entity =>
            {
                entity.ToTable("Applyjob");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.UserId).HasColumnName("userID");
            });

            modelBuilder.Entity<ApprovedPresnolInfo>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Cityid).HasColumnName("cityid");

                entity.Property(e => e.Pan).HasColumnName("PAN");
            });

            modelBuilder.Entity<Approvedbankdetail>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");
            });

            modelBuilder.Entity<BJob>(entity =>
            {
                entity.ToTable("b_Jobs");

                entity.Property(e => e.Id).HasColumnName("ID");
            });

            modelBuilder.Entity<Blog>(entity =>
            {
                entity.ToTable("Blog");

                entity.Property(e => e.Phragraph).HasColumnName("phragraph");
            });

            modelBuilder.Entity<Bookmark>(entity =>
            {
                entity.ToTable("Bookmark");

                entity.Property(e => e.UserId).HasColumnName("userId");
            });

            modelBuilder.Entity<CJobOpen>(entity =>
            {
                entity.ToTable("C_job_Open");

                entity.Property(e => e.Cityid).HasColumnName("cityid");

                entity.Property(e => e.IsVendor)
                    .IsRequired()
                    .HasColumnName("isVendor")
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.JobDescription).HasColumnName("Job_Description");

                entity.Property(e => e.JobTitle).HasColumnName("Job_Title");

                entity.Property(e => e.RequiredExperience).HasColumnName("Required_Experience");

                entity.Property(e => e.Stateid).HasColumnName("stateid");

                entity.Property(e => e.Status).HasColumnName("status");
            });

            modelBuilder.Entity<Carrier>(entity =>
            {
                entity.ToTable("Carrier");

                entity.Property(e => e.CurrentCtc)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("CurrentCTC");

                entity.Property(e => e.ExpectedCtc)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("ExpectedCTC");
            });

            modelBuilder.Entity<CarrierStatus>(entity =>
            {
                entity.ToTable("CarrierStatus");

                entity.Property(e => e.Id).HasColumnName("id");
            });

            modelBuilder.Entity<City>(entity =>
            {
                entity.ToTable("city");
            });

            modelBuilder.Entity<Companycategory>(entity =>
            {
                entity.ToTable("Companycategory");
            });

            modelBuilder.Entity<Companytype>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Companytype1).HasColumnName("companytype");
            });

            modelBuilder.Entity<ContactUss>(entity =>
            {
                entity.ToTable("ContactUss");

                entity.Property(e => e.TextMessage).HasColumnName("Text_Message");
            });

            modelBuilder.Entity<Counter>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Id })
                    .HasName("PK_HangFire_Counter");

                entity.ToTable("Counter", "HangFire");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.ToTable("Country");

                entity.Property(e => e.Id).HasColumnName("id");
            });

            modelBuilder.Entity<CreateProfile>(entity =>
            {
                entity.ToTable("CreateProfile");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CurrentCtc)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("CurrentCTC");

                entity.Property(e => e.EmailId).HasColumnName("EmailID");

                entity.Property(e => e.ExpectedCtc)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("ExpectedCTC");
            });

            modelBuilder.Entity<DropIndustry>(entity =>
            {
                entity.ToTable("Drop_Industries");

                entity.Property(e => e.Id).HasColumnName("ID");
            });

            modelBuilder.Entity<Educationtype>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Educationtype1).HasColumnName("educationtype");
            });

            modelBuilder.Entity<EmployeeRegistration>(entity =>
            {
                entity.Property(e => e.AccountTypeId).HasColumnName("AccountTypeID");

                entity.Property(e => e.AnnualCtc)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("AnnualCTC");

                entity.Property(e => e.Appointmentletter).HasColumnName("appointmentletter");

                entity.Property(e => e.Basic).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CityId).HasColumnName("cityId");

                entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");

                entity.Property(e => e.EmployeeEpf)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("EmployeeEPF");

                entity.Property(e => e.EmployeeEsic)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("EmployeeESIC");

                entity.Property(e => e.EmployerEpf)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("EmployerEPF");

                entity.Property(e => e.EmployerEsic)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("EmployerESIC");

                entity.Property(e => e.EpfNumber).HasColumnName("EPF_Number");

                entity.Property(e => e.Gross).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.HouseRentAllowance).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Ifsc).HasColumnName("IFSC");

                entity.Property(e => e.JobTitle).HasColumnName("Job_Title");

                entity.Property(e => e.MonthlyCtc)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("MonthlyCTC");

                entity.Property(e => e.MonthlyGrossPay).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Pan).HasColumnName("PAN");

                entity.Property(e => e.Professionaltax).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Profileimage).HasColumnName("profileimage");

                entity.Property(e => e.SpecialAllowance).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Tdspercentage).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<Employeeattendance>(entity =>
            {
                entity.ToTable("Employeeattendance");

                entity.Property(e => e.GenerateSalary).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Lop).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<Hash>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Field })
                    .HasName("PK_HangFire_Hash");

                entity.ToTable("Hash", "HangFire");

                entity.HasIndex(e => e.ExpireAt, "IX_HangFire_Hash_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.Field).HasMaxLength(100);
            });

            modelBuilder.Entity<Job>(entity =>
            {
                entity.ToTable("Job", "HangFire");

                entity.HasIndex(e => e.ExpireAt, "IX_HangFire_Job_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.HasIndex(e => e.StateName, "IX_HangFire_Job_StateName")
                    .HasFilter("([StateName] IS NOT NULL)");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");

                entity.Property(e => e.StateName).HasMaxLength(20);
            });

            modelBuilder.Entity<JobParameter>(entity =>
            {
                entity.HasKey(e => new { e.JobId, e.Name })
                    .HasName("PK_HangFire_JobParameter");

                entity.ToTable("JobParameter", "HangFire");

                entity.Property(e => e.Name).HasMaxLength(40);

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.JobParameters)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK_HangFire_JobParameter_Job");
            });

            modelBuilder.Entity<JobQueue>(entity =>
            {
                entity.HasKey(e => new { e.Queue, e.Id })
                    .HasName("PK_HangFire_JobQueue");

                entity.ToTable("JobQueue", "HangFire");

                entity.Property(e => e.Queue).HasMaxLength(50);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.FetchedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<JobTitle>(entity =>
            {
                entity.ToTable("Job_Title");

                entity.Property(e => e.JobTitle1).HasColumnName("JobTitle");

                entity.Property(e => e.Jobamount).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<Leave>(entity =>
            {
                entity.ToTable("Leave");

                entity.Property(e => e.Createddate).HasColumnName("createddate");
            });

            modelBuilder.Entity<LeaveApply>(entity =>
            {
                entity.ToTable("LeaveApply");

                entity.Property(e => e.Leavetypeid).HasColumnName("leavetypeid");
            });

            modelBuilder.Entity<LeaveType>(entity =>
            {
                entity.ToTable("LeaveType");

                entity.Property(e => e.Createddate).HasColumnName("createddate");

                entity.Property(e => e.Leavetype1).HasColumnName("leavetype");
            });

            modelBuilder.Entity<Leavemaster>(entity =>
            {
                entity.ToTable("Leavemaster");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Value).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<List>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Id })
                    .HasName("PK_HangFire_List");

                entity.ToTable("List", "HangFire");

                entity.HasIndex(e => e.ExpireAt, "IX_HangFire_List_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<MLocation>(entity =>
            {
                entity.ToTable("M_Location");
            });

            modelBuilder.Entity<Organization>(entity =>
            {
                entity.Property(e => e.BgCompanyImage).HasColumnName("bgCompanyImage");

                entity.Property(e => e.CityId).HasColumnName("CityID");

                entity.Property(e => e.CompanytypeId).HasColumnName("CompanytypeID");

                entity.Property(e => e.StateId).HasColumnName("StateID");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payment");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Count).HasColumnName("count");
            });

            modelBuilder.Entity<Postedby>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");
            });

            modelBuilder.Entity<Qualification>(entity =>
            {
                entity.ToTable("Qualification");

                entity.Property(e => e.Id).HasColumnName("ID");
            });

            modelBuilder.Entity<Registration>(entity =>
            {
                entity.ToTable("Registration");

                entity.Property(e => e.CPassword).HasColumnName("C_Password");
            });

            modelBuilder.Entity<Rolecategory>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");
            });

            modelBuilder.Entity<Salary>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");
            });

            modelBuilder.Entity<Schema>(entity =>
            {
                entity.HasKey(e => e.Version)
                    .HasName("PK_HangFire_Schema");

                entity.ToTable("Schema", "HangFire");

                entity.Property(e => e.Version).ValueGeneratedNever();
            });

            modelBuilder.Entity<Server>(entity =>
            {
                entity.ToTable("Server", "HangFire");

                entity.HasIndex(e => e.LastHeartbeat, "IX_HangFire_Server_LastHeartbeat");

                entity.Property(e => e.Id).HasMaxLength(200);

                entity.Property(e => e.LastHeartbeat).HasColumnType("datetime");
            });

            modelBuilder.Entity<Service>(entity =>
            {
                entity.Property(e => e.ServiceBannerImage).HasColumnName("Service_Banner_Image");

                entity.Property(e => e.ServiceDescription).HasColumnName("Service_Description");

                entity.Property(e => e.ServiceImage).HasColumnName("Service_Image");
            });

            modelBuilder.Entity<Serviceslist>(entity =>
            {
                entity.ToTable("Serviceslist");
            });

            modelBuilder.Entity<Set>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Value })
                    .HasName("PK_HangFire_Set");

                entity.ToTable("Set", "HangFire");

                entity.HasIndex(e => e.ExpireAt, "IX_HangFire_Set_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.HasIndex(e => new { e.Key, e.Score }, "IX_HangFire_Set_Score");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.Value).HasMaxLength(256);

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<State>(entity =>
            {
                entity.HasKey(e => new { e.JobId, e.Id })
                    .HasName("PK_HangFire_State");

                entity.ToTable("State", "HangFire");

                entity.HasIndex(e => e.CreatedAt, "IX_HangFire_State_CreatedAt");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(20);

                entity.Property(e => e.Reason).HasMaxLength(100);

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.States)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK_HangFire_State_Job");
            });

            modelBuilder.Entity<State1>(entity =>
            {
                entity.ToTable("state");

                entity.Property(e => e.Sname).HasColumnName("SName");
            });

            modelBuilder.Entity<TblAddExperTise>(entity =>
            {
                entity.ToTable("Tbl_AddExperTises");

                entity.Property(e => e.Id).HasColumnName("id");
            });

            modelBuilder.Entity<TblUploadBanner>(entity =>
            {
                entity.Property(e => e.Base64Value).HasColumnName("base64Value");
            });

            modelBuilder.Entity<Testimonial>(entity =>
            {
                entity.ToTable("Testimonial");

                entity.Property(e => e.Id).HasColumnName("ID");
            });

            modelBuilder.Entity<WorkMode>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
