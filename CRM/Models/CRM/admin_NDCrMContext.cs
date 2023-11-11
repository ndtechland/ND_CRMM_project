using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CRM.Models.Crm
{
    public partial class admin_NDCrMContext : DbContext
    {
        public admin_NDCrMContext()
        {
        }

        public admin_NDCrMContext(DbContextOptions<admin_NDCrMContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AccountTypeMaster> AccountTypeMasters { get; set; } = null!;
        public virtual DbSet<AdminLogin> AdminLogins { get; set; } = null!;
        public virtual DbSet<BillingDetail> BillingDetails { get; set; } = null!;
        public virtual DbSet<BillingHistory> BillingHistories { get; set; } = null!;
        public virtual DbSet<CustomerRegistration> CustomerRegistrations { get; set; } = null!;
        public virtual DbSet<DateFormatMaster> DateFormatMasters { get; set; } = null!;
        public virtual DbSet<DeductorNameMaster> DeductorNameMasters { get; set; } = null!;
        public virtual DbSet<DeductorTypeMaster> DeductorTypeMasters { get; set; } = null!;
        public virtual DbSet<DepartmentMaster> DepartmentMasters { get; set; } = null!;
        public virtual DbSet<EmployeeBankDetail> EmployeeBankDetails { get; set; } = null!;
        public virtual DbSet<EmployeeLeaveMaster> EmployeeLeaveMasters { get; set; } = null!;
        public virtual DbSet<EmployeeLogin> EmployeeLogins { get; set; } = null!;
        public virtual DbSet<EmployeePersonalAddress> EmployeePersonalAddresses { get; set; } = null!;
        public virtual DbSet<EmployeeRegistration> EmployeeRegistrations { get; set; } = null!;
        public virtual DbSet<EmployeeRole> EmployeeRoles { get; set; } = null!;
        public virtual DbSet<EmployeeSalaryDetail> EmployeeSalaryDetails { get; set; } = null!;
        public virtual DbSet<GenderMaster> GenderMasters { get; set; } = null!;
        public virtual DbSet<GstMaster> GstMasters { get; set; } = null!;
        public virtual DbSet<HeadOfficeAddress> HeadOfficeAddresses { get; set; } = null!;
        public virtual DbSet<IndustryMaster> IndustryMasters { get; set; } = null!;
        public virtual DbSet<OrganisationProfile> OrganisationProfiles { get; set; } = null!;
        public virtual DbSet<OrganisationTaxDetail> OrganisationTaxDetails { get; set; } = null!;
        public virtual DbSet<PayMethodMaster> PayMethodMasters { get; set; } = null!;
        public virtual DbSet<Payroll> Payrolls { get; set; } = null!;
        public virtual DbSet<ProductMaster> ProductMasters { get; set; } = null!;
        public virtual DbSet<Quation> Quations { get; set; } = null!;
        public virtual DbSet<StateMaster> StateMasters { get; set; } = null!;
        public virtual DbSet<TaxDeductor> TaxDeductors { get; set; } = null!;
        public virtual DbSet<TransactionDetail> TransactionDetails { get; set; } = null!;
        public virtual DbSet<WorkLocation> WorkLocations { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("server=103.83.81.251;database=admin_NDCrM;User ID=admin_NDCrM;Password=NDCrM@12345#;Trusted_Connection=False;TrustServerCertificate=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("admin_NDCrM");

            modelBuilder.Entity<AccountTypeMaster>(entity =>
            {
                entity.ToTable("Account_Type_Master", "dbo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AccountType)
                    .HasMaxLength(50)
                    .HasColumnName("Account_Type");
            });

            modelBuilder.Entity<AdminLogin>(entity =>
            {
                entity.ToTable("AdminLogin");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Password).HasMaxLength(120);

                entity.Property(e => e.Role).HasMaxLength(120);

                entity.Property(e => e.UserName).HasMaxLength(120);
            });

            modelBuilder.Entity<BillingDetail>(entity =>
            {
                entity.ToTable("Billing_Details", "dbo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AddressLine1)
                    .HasMaxLength(255)
                    .HasColumnName("Address_Line_1");

                entity.Property(e => e.AddressLine2)
                    .HasMaxLength(255)
                    .HasColumnName("Address_Line_2");

                entity.Property(e => e.City).HasMaxLength(120);

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.StateId).HasColumnName("State_ID");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.HasOne(d => d.State)
                    .WithMany(p => p.BillingDetails)
                    .HasForeignKey(d => d.StateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Customer_Billing_Details_State_ID");
            });

            modelBuilder.Entity<BillingHistory>(entity =>
            {
                entity.ToTable("Billing_History", "dbo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.BillingDetailsId).HasColumnName("Billing_Details_ID");

                entity.Property(e => e.CustomerId).HasColumnName("Customer_ID");

                entity.Property(e => e.ProductDetailsId).HasColumnName("Product_Details_ID");

                entity.HasOne(d => d.BillingDetails)
                    .WithMany(p => p.BillingHistories)
                    .HasForeignKey(d => d.BillingDetailsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Customer_Fact_Table_Billing_Details_ID");

                entity.HasOne(d => d.ProductDetails)
                    .WithMany(p => p.BillingHistories)
                    .HasForeignKey(d => d.ProductDetailsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Customer_Fact_Table_Product_Details_ID");
            });

            modelBuilder.Entity<CustomerRegistration>(entity =>
            {
                entity.ToTable("Customer_Registration");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AlternateNumber)
                    .HasMaxLength(255)
                    .HasColumnName("Alternate_number");

                entity.Property(e => e.BillingAddress)
                    .HasMaxLength(255)
                    .HasColumnName("Billing_Address");

                entity.Property(e => e.CompanyName)
                    .HasMaxLength(255)
                    .HasColumnName("Company_Name");

                entity.Property(e => e.ContactPersonName)
                    .HasMaxLength(255)
                    .HasColumnName("Contact_person_name");

                entity.Property(e => e.Email).HasMaxLength(255);

                entity.Property(e => e.GstNumber)
                    .HasMaxLength(255)
                    .HasColumnName("GST_Number");

                entity.Property(e => e.MobileNumber)
                    .HasMaxLength(255)
                    .HasColumnName("Mobile_number");

                entity.Property(e => e.ProductDetails)
                    .HasMaxLength(255)
                    .HasColumnName("Product_Details");

                entity.Property(e => e.RenewDate)
                    .HasColumnType("date")
                    .HasColumnName("Renew_Date");

                entity.Property(e => e.StartDate)
                    .HasColumnType("date")
                    .HasColumnName("Start_date");
            });

            modelBuilder.Entity<DateFormatMaster>(entity =>
            {
                entity.ToTable("Date_Format_Master", "dbo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DateFormat)
                    .HasMaxLength(120)
                    .HasColumnName("Date_Format");
            });

            modelBuilder.Entity<DeductorNameMaster>(entity =>
            {
                entity.ToTable("Deductor_Name_Master", "dbo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DeductorName)
                    .HasMaxLength(120)
                    .HasColumnName("Deductor_Name");
            });

            modelBuilder.Entity<DeductorTypeMaster>(entity =>
            {
                entity.ToTable("Deductor_Type_Master", "dbo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DeductorTypeName)
                    .HasMaxLength(120)
                    .HasColumnName("Deductor_Type_Name");
            });

            modelBuilder.Entity<DepartmentMaster>(entity =>
            {
                entity.ToTable("Department_Master", "dbo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DepartmentName)
                    .HasMaxLength(255)
                    .HasColumnName("Department_Name");
            });

            modelBuilder.Entity<EmployeeBankDetail>(entity =>
            {
                entity.ToTable("Employee_Bank_Details", "dbo");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.AccountHolderName)
                    .HasMaxLength(255)
                    .HasColumnName("Account_Holder_Name");

                entity.Property(e => e.AccountNumber).HasColumnName("Account_Number");

                entity.Property(e => e.AccountTypeId).HasColumnName("Account_Type_ID");

                entity.Property(e => e.BankName)
                    .HasMaxLength(255)
                    .HasColumnName("Bank_Name");

                entity.Property(e => e.EmployeeRegistrationId).HasColumnName("Employee_Registration_ID");

                entity.Property(e => e.Ifsc)
                    .HasMaxLength(10)
                    .HasColumnName("IFSC");

                entity.Property(e => e.ReEnterAccountNumber).HasColumnName("Re_Enter_Account_Number");

                entity.HasOne(d => d.EmployeeRegistration)
                    .WithMany(p => p.EmployeeBankDetails)
                    .HasForeignKey(d => d.EmployeeRegistrationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Employee_Bank_Details_Employee_Registration_ID");
            });

            modelBuilder.Entity<EmployeeLeaveMaster>(entity =>
            {
                entity.ToTable("Employee_Leave_Master", "dbo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.FromDate)
                    .HasColumnType("date")
                    .HasColumnName("From_Date");

                entity.Property(e => e.Reason).HasMaxLength(255);

                entity.Property(e => e.ToDate)
                    .HasColumnType("date")
                    .HasColumnName("To_Date");
            });

            modelBuilder.Entity<EmployeeLogin>(entity =>
            {
                entity.ToTable("Employee_Login", "dbo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.EmployeeId).HasColumnName("Employee_ID");

                entity.Property(e => e.Password).HasMaxLength(120);

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeLogins)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Employee_Login_Employee_ID");
            });

            modelBuilder.Entity<EmployeePersonalAddress>(entity =>
            {
                entity.ToTable("Employee_Personal_Address", "dbo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AddressLine1)
                    .HasMaxLength(255)
                    .HasColumnName("Address_Line_1");

                entity.Property(e => e.AddressLine2)
                    .HasMaxLength(255)
                    .HasColumnName("Address_Line_2");

                entity.Property(e => e.City).HasMaxLength(255);

                entity.Property(e => e.DateOfBirth)
                    .HasColumnType("date")
                    .HasColumnName("Date_Of_Birth");

                entity.Property(e => e.EmployeeRegistrationId).HasColumnName("Employee_Registration_ID");

                entity.Property(e => e.FatherName)
                    .HasMaxLength(255)
                    .HasColumnName("Father_Name");

                entity.Property(e => e.MobileNumber)
                    .HasColumnType("numeric(10, 0)")
                    .HasColumnName("Mobile_Number");

                entity.Property(e => e.Pan)
                    .HasMaxLength(50)
                    .HasColumnName("PAN");

                entity.Property(e => e.PersonalEmailAddress)
                    .HasMaxLength(120)
                    .HasColumnName("Personal_Email_Address");

                entity.Property(e => e.StateId).HasColumnName("State_ID");

                entity.HasOne(d => d.EmployeeRegistration)
                    .WithMany(p => p.EmployeePersonalAddresses)
                    .HasForeignKey(d => d.EmployeeRegistrationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Employee_Personal_Address_Employee_Registration_ID");

                entity.HasOne(d => d.State)
                    .WithMany(p => p.EmployeePersonalAddresses)
                    .HasForeignKey(d => d.StateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Employee_Personal_Address_State_ID");
            });

            modelBuilder.Entity<EmployeeRegistration>(entity =>
            {
                entity.ToTable("Employee_Registration", "dbo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DateOfJoining)
                    .HasColumnType("date")
                    .HasColumnName("Date_Of_Joining");

                entity.Property(e => e.DepartmentId).HasColumnName("Department_ID");

                entity.Property(e => e.DesignationId).HasColumnName("Designation_ID");

                entity.Property(e => e.EmployeeId)
                    .HasMaxLength(95)
                    .HasColumnName("Employee_ID")
                    .HasComputedColumnSql("((((('ND-'+CONVERT([nvarchar],datepart(month,getdate())))+'/')+CONVERT([nvarchar],datepart(year,getdate())))+'-')+CONVERT([nvarchar],[ID]))", false);

                entity.Property(e => e.FirstName)
                    .HasMaxLength(120)
                    .HasColumnName("First_Name");

                entity.Property(e => e.GenderId).HasColumnName("Gender_ID");

                entity.Property(e => e.LastName)
                    .HasMaxLength(120)
                    .HasColumnName("Last_Name");

                entity.Property(e => e.MiddleName)
                    .HasMaxLength(120)
                    .HasColumnName("Middle_Name");

                entity.Property(e => e.WorkEmail)
                    .HasMaxLength(120)
                    .HasColumnName("Work_Email");

                entity.Property(e => e.WorkLocationId).HasColumnName("Work_Location_ID");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.EmployeeRegistrations)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Employee_Registration_Department_ID");

                entity.HasOne(d => d.Gender)
                    .WithMany(p => p.EmployeeRegistrations)
                    .HasForeignKey(d => d.GenderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Employee_Registration_Gender_ID");

                entity.HasOne(d => d.WorkLocation)
                    .WithMany(p => p.EmployeeRegistrations)
                    .HasForeignKey(d => d.WorkLocationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Employee_Registration_Work_Location_ID");
            });

            modelBuilder.Entity<EmployeeRole>(entity =>
            {
                entity.ToTable("Employee_Role", "dbo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.EmployeeRegistrationId).HasColumnName("Employee_Registration_ID");

                entity.Property(e => e.EmployeeRole1)
                    .HasMaxLength(120)
                    .HasColumnName("Employee_Role");

                entity.HasOne(d => d.EmployeeRegistration)
                    .WithMany(p => p.EmployeeRoles)
                    .HasForeignKey(d => d.EmployeeRegistrationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Employee_Role_Employee_Registration_ID");
            });

            modelBuilder.Entity<EmployeeSalaryDetail>(entity =>
            {
                entity.ToTable("Employee_Salary_Details", "dbo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AnnualCtc).HasColumnName("Annual_CTC");

                entity.Property(e => e.BasicAnnualAmount)
                    .HasColumnName("Basic_Annual_Amount")
                    .HasComputedColumnSql("([Annual_CTC]/(2))", false);

                entity.Property(e => e.BasicMonthlyAmount)
                    .HasColumnName("Basic_Monthly_Amount")
                    .HasComputedColumnSql("(([Annual_CTC]/(2))/(12))", false);

                entity.Property(e => e.ConveyanceAllowanceAnnualAmount)
                    .HasColumnName("Conveyance_Allowance_Annual_Amount")
                    .HasComputedColumnSql("([Conveyance_Allowance_Monthly_Amount]*(12))", false);

                entity.Property(e => e.ConveyanceAllowanceMonthlyAmount)
                    .HasColumnName("Conveyance_Allowance_Monthly_Amount")
                    .HasDefaultValueSql("((2000))");

                entity.Property(e => e.FixedAllowanceAnnualAmount)
                    .HasColumnName("Fixed_Allowance_Annual_Amount")
                    .HasComputedColumnSql("([Fixed_Allowance_Monthly_Amount]*(12))", false);

                entity.Property(e => e.FixedAllowanceMonthlyAmount)
                    .HasColumnName("Fixed_Allowance_Monthly_Amount")
                    .HasDefaultValueSql("((1400))");

                entity.Property(e => e.HouseRentAllowanceAnnualAmount)
                    .HasColumnName("House_Rent_Allowance_Annual_Amount")
                    .HasComputedColumnSql("(([Annual_CTC]/(2))/(2))", false);

                entity.Property(e => e.HouseRentAllowanceMonthlyAmount)
                    .HasColumnName("House_Rent_Allowance_Monthly_Amount")
                    .HasComputedColumnSql("((([Annual_CTC]/(2))/(2))/(12))", false);

                entity.Property(e => e.IncomeTax).HasColumnName("Income_Tax");

                entity.Property(e => e.NetPay)
                    .HasColumnName("Net_Pay")
                    .HasComputedColumnSql("(isnull([Annual_CTC],(0))-isnull([Income_Tax],(0)))", false);

                entity.Property(e => e.TotalDeduction)
                    .HasColumnName("Total_Deduction")
                    .HasComputedColumnSql("(isnull([Income_Tax],(0)))", false);
            });

            modelBuilder.Entity<GenderMaster>(entity =>
            {
                entity.ToTable("Gender_Master", "dbo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.GenderName)
                    .HasMaxLength(50)
                    .HasColumnName("Gender_Name");
            });

            modelBuilder.Entity<GstMaster>(entity =>
            {
                entity.ToTable("GST_Master", "dbo");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.Cgst)
                    .HasMaxLength(50)
                    .HasColumnName("CGST");

                entity.Property(e => e.GstPercentagen)
                    .HasMaxLength(50)
                    .HasColumnName("GST_Percentagen");

                entity.Property(e => e.Igst)
                    .HasMaxLength(50)
                    .HasColumnName("IGST");

                entity.Property(e => e.Scgst)
                    .HasMaxLength(50)
                    .HasColumnName("SCGST");
            });

            modelBuilder.Entity<HeadOfficeAddress>(entity =>
            {
                entity.ToTable("Head_Office_Address", "dbo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AddressLine1)
                    .HasMaxLength(255)
                    .HasColumnName("Address_Line_1");

                entity.Property(e => e.AddressLine2)
                    .HasMaxLength(255)
                    .HasColumnName("Address_Line_2");

                entity.Property(e => e.City).HasMaxLength(120);

                entity.Property(e => e.StateId).HasColumnName("State_ID");

                entity.HasOne(d => d.State)
                    .WithMany(p => p.HeadOfficeAddresses)
                    .HasForeignKey(d => d.StateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Head_Office_Address_State_ID");
            });

            modelBuilder.Entity<IndustryMaster>(entity =>
            {
                entity.ToTable("Industry_Master", "dbo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.IndustryName)
                    .HasMaxLength(255)
                    .HasColumnName("Industry_Name");
            });

            modelBuilder.Entity<OrganisationProfile>(entity =>
            {
                entity.ToTable("Organisation_Profile", "dbo");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.AddressLine1)
                    .HasMaxLength(120)
                    .HasColumnName("Address_Line_1");

                entity.Property(e => e.AddressLine2)
                    .HasMaxLength(120)
                    .HasColumnName("Address_Line_2");

                entity.Property(e => e.BusinessLocation)
                    .HasMaxLength(255)
                    .HasColumnName("Business_Location")
                    .HasDefaultValueSql("(N'INDIA')");

                entity.Property(e => e.City).HasMaxLength(120);

                entity.Property(e => e.DateFormatId).HasColumnName("Date_Format_ID");

                entity.Property(e => e.HeadOfficeAddressId).HasColumnName("Head_Office_Address_ID");

                entity.Property(e => e.IndustryId).HasColumnName("Industry_ID");

                entity.Property(e => e.OrganizationName)
                    .HasMaxLength(255)
                    .HasColumnName("Organization_Name");

                entity.Property(e => e.StateId).HasColumnName("State_ID");

                entity.HasOne(d => d.DateFormat)
                    .WithMany(p => p.OrganisationProfiles)
                    .HasForeignKey(d => d.DateFormatId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Organisation_Profile_Date_Format_ID");

                entity.HasOne(d => d.HeadOfficeAddress)
                    .WithMany(p => p.OrganisationProfiles)
                    .HasForeignKey(d => d.HeadOfficeAddressId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Organisation_Profile_Head_Office_Address_ID");

                entity.HasOne(d => d.Industry)
                    .WithMany(p => p.OrganisationProfiles)
                    .HasForeignKey(d => d.IndustryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Organisation_Profile_Industry_ID");

                entity.HasOne(d => d.State)
                    .WithMany(p => p.OrganisationProfiles)
                    .HasForeignKey(d => d.StateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Organisation_Profile_State_ID");
            });

            modelBuilder.Entity<OrganisationTaxDetail>(entity =>
            {
                entity.ToTable("Organisation_Tax_Details", "dbo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.OrganisationProfileId).HasColumnName("Organisation_Profile_ID");

                entity.Property(e => e.Pan)
                    .HasMaxLength(50)
                    .HasColumnName("PAN")
                    .HasDefaultValueSql("('AAHCN4627Q')");

                entity.Property(e => e.Tan)
                    .HasMaxLength(50)
                    .HasColumnName("TAN")
                    .HasDefaultValueSql("('MRTN05215A')");

                entity.Property(e => e.TaxDeductorId).HasColumnName("Tax_Deductor_ID");

                entity.Property(e => e.TaxPaymentFrequency)
                    .HasMaxLength(50)
                    .HasColumnName("Tax_Payment_Frequency")
                    .HasDefaultValueSql("('Monthly')");

                entity.Property(e => e.TdsCircleCode)
                    .HasMaxLength(50)
                    .HasColumnName("TDS_Circle_Code");

                entity.HasOne(d => d.OrganisationProfile)
                    .WithMany(p => p.OrganisationTaxDetails)
                    .HasForeignKey(d => d.OrganisationProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Organisation_Tax_Details_Organisation_Profile_ID");

                entity.HasOne(d => d.TaxDeductor)
                    .WithMany(p => p.OrganisationTaxDetails)
                    .HasForeignKey(d => d.TaxDeductorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Organisation_Tax_Details_Tax_Deductor_ID");
            });

            modelBuilder.Entity<PayMethodMaster>(entity =>
            {
                entity.ToTable("Pay_Method_Master", "dbo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.PayMethod)
                    .HasMaxLength(120)
                    .HasColumnName("Pay_Method");
            });

            modelBuilder.Entity<Payroll>(entity =>
            {
                entity.ToTable("Payroll", "dbo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.BankDetailsId).HasColumnName("Bank_Details_ID");

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.EmployeeId).HasColumnName("Employee_ID");

                entity.Property(e => e.LeaveId).HasColumnName("Leave_ID");

                entity.Property(e => e.Report).HasColumnType("text");

                entity.Property(e => e.SalaryId).HasColumnName("Salary_ID");

                entity.Property(e => e.TotalAmount).HasColumnName("Total_Amount");

                entity.HasOne(d => d.BankDetails)
                    .WithMany(p => p.Payrolls)
                    .HasForeignKey(d => d.BankDetailsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Payroll_Bank_Details_ID");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.Payrolls)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Payroll_Employee_ID");

                entity.HasOne(d => d.Leave)
                    .WithMany(p => p.Payrolls)
                    .HasForeignKey(d => d.LeaveId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Payroll_Leave_ID");

                entity.HasOne(d => d.Salary)
                    .WithMany(p => p.Payrolls)
                    .HasForeignKey(d => d.SalaryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Payroll_Salary_ID");
            });

            modelBuilder.Entity<ProductMaster>(entity =>
            {
                entity.ToTable("Product_Master", "dbo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Category).HasMaxLength(255);

                entity.Property(e => e.Gst)
                    .HasMaxLength(255)
                    .HasColumnName("GST");

                entity.Property(e => e.HsnSacCode)
                    .HasMaxLength(255)
                    .HasColumnName("HSN_SAC_Code");

                entity.Property(e => e.ProductName)
                    .HasMaxLength(255)
                    .HasColumnName("Product_Name");
            });

            modelBuilder.Entity<Quation>(entity =>
            {
                entity.ToTable("Quation", "dbo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CompanyName)
                    .HasMaxLength(255)
                    .HasColumnName("Company_Name");

                entity.Property(e => e.CustomerName)
                    .HasMaxLength(255)
                    .HasColumnName("Customer_Name");

                entity.Property(e => e.Email).HasMaxLength(255);

                entity.Property(e => e.Mobile).HasColumnType("numeric(10, 0)");

                entity.Property(e => e.ProductId).HasColumnName("Product_ID");

                entity.Property(e => e.SalesPersonName)
                    .HasMaxLength(255)
                    .HasColumnName("Sales_Person_Name");

                entity.Property(e => e.Subject).HasMaxLength(255);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Quations)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Quote_Product_ID");
            });

            modelBuilder.Entity<StateMaster>(entity =>
            {
                entity.ToTable("State_Master", "dbo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.StateName)
                    .HasMaxLength(255)
                    .HasColumnName("State_Name");
            });

            modelBuilder.Entity<TaxDeductor>(entity =>
            {
                entity.ToTable("Tax_Deductor", "dbo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DeductorFatherName)
                    .HasMaxLength(120)
                    .HasColumnName("Deductor_Father_Name");

                entity.Property(e => e.DeductorNameId).HasColumnName("Deductor_Name_ID");

                entity.Property(e => e.DeductorTypeId).HasColumnName("Deductor_Type_ID");

                entity.HasOne(d => d.DeductorName)
                    .WithMany(p => p.TaxDeductors)
                    .HasForeignKey(d => d.DeductorNameId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Tax_Deductor_Deductor_Name_ID");

                entity.HasOne(d => d.DeductorType)
                    .WithMany(p => p.TaxDeductors)
                    .HasForeignKey(d => d.DeductorTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Tax_Deductor_Deductor_Type_ID");
            });

            modelBuilder.Entity<TransactionDetail>(entity =>
            {
                entity.ToTable("Transaction_Details", "dbo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.BillingId).HasColumnName("Billing_ID");

                entity.Property(e => e.DateOfIssue)
                    .HasColumnType("date")
                    .HasColumnName("Date_Of_Issue");

                entity.Property(e => e.PaidAmount).HasColumnName("Paid_Amount");

                entity.Property(e => e.PayAmount).HasColumnName("Pay_Amount");

                entity.Property(e => e.PayMethod).HasColumnName("Pay_Method");

                entity.HasOne(d => d.Billing)
                    .WithMany(p => p.TransactionDetails)
                    .HasForeignKey(d => d.BillingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Transaction_Details_Billing_ID");

                entity.HasOne(d => d.PayMethodNavigation)
                    .WithMany(p => p.TransactionDetails)
                    .HasForeignKey(d => d.PayMethod)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Transaction_Details_Pay_Method_ID");
            });

            modelBuilder.Entity<WorkLocation>(entity =>
            {
                entity.ToTable("Work_Location", "dbo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AddressLine1)
                    .HasMaxLength(255)
                    .HasColumnName("Address_Line_1");

                entity.Property(e => e.AddressLine2)
                    .HasMaxLength(255)
                    .HasColumnName("Address_Line_2");

                entity.Property(e => e.City).HasMaxLength(255);

                entity.Property(e => e.StateId).HasColumnName("State_ID");

                entity.HasOne(d => d.State)
                    .WithMany(p => p.WorkLocations)
                    .HasForeignKey(d => d.StateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Work_Location_State_ID");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
