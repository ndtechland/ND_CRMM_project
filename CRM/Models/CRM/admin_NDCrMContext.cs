﻿using System;
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
        public virtual DbSet<Additonalcontribution> Additonalcontributions { get; set; } = null!;
        public virtual DbSet<AdminLogin> AdminLogins { get; set; } = null!;
        public virtual DbSet<BannerMaster> BannerMasters { get; set; } = null!;
        public virtual DbSet<BillingDetail> BillingDetails { get; set; } = null!;
        public virtual DbSet<BillingHistory> BillingHistories { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<City> Cities { get; set; } = null!;
        public virtual DbSet<CustomerRegistration> CustomerRegistrations { get; set; } = null!;
        public virtual DbSet<DateFormatMaster> DateFormatMasters { get; set; } = null!;
        public virtual DbSet<DeductorNameMaster> DeductorNameMasters { get; set; } = null!;
        public virtual DbSet<DeductorTypeMaster> DeductorTypeMasters { get; set; } = null!;
        public virtual DbSet<DepartmentMaster> DepartmentMasters { get; set; } = null!;
        public virtual DbSet<DesignationMaster> DesignationMasters { get; set; } = null!;
        public virtual DbSet<Empattendance> Empattendances { get; set; } = null!;
        public virtual DbSet<EmployeeBankDetail> EmployeeBankDetails { get; set; } = null!;
        public virtual DbSet<EmployeeLeaveMaster> EmployeeLeaveMasters { get; set; } = null!;
        public virtual DbSet<EmployeeLogin> EmployeeLogins { get; set; } = null!;
        public virtual DbSet<EmployeePersonalDetail> EmployeePersonalDetails { get; set; } = null!;
        public virtual DbSet<EmployeeRegistration> EmployeeRegistrations { get; set; } = null!;
        public virtual DbSet<EmployeeRole> EmployeeRoles { get; set; } = null!;
        public virtual DbSet<EmployeeSalaryDetail> EmployeeSalaryDetails { get; set; } = null!;
        public virtual DbSet<EmployeerEpf> EmployeerEpfs { get; set; } = null!;
        public virtual DbSet<EmployeerTd> EmployeerTds { get; set; } = null!;
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
        public virtual DbSet<State> States { get; set; } = null!;
        public virtual DbSet<StateMaster> StateMasters { get; set; } = null!;
        public virtual DbSet<TErrorLog> TErrorLogs { get; set; } = null!;
        public virtual DbSet<TaxDeductor> TaxDeductors { get; set; } = null!;
        public virtual DbSet<TransactionDetail> TransactionDetails { get; set; } = null!;
        public virtual DbSet<WorkLocation> WorkLocations { get; set; } = null!;
        public virtual DbSet<WorkLocation1> WorkLocations1 { get; set; } = null!;
        public virtual DbSet<EmployeeImportExcel> EmpMultiforms { get; set; } = null!;
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

            modelBuilder.Entity<Additonalcontribution>(entity =>
            {
                entity.ToTable("additonalcontribution");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ContributionName)
                    .HasMaxLength(255)
                    .HasColumnName("contribution_name");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("created_date");

                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                entity.Property(e => e.WorkLocationId).HasColumnName("Work_Location_ID");
            });

            modelBuilder.Entity<AdminLogin>(entity =>
            {
                entity.ToTable("AdminLogin");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Emailid)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("emailid");

                entity.Property(e => e.Password).HasMaxLength(120);

                entity.Property(e => e.Role).HasMaxLength(120);

                entity.Property(e => e.UserName).HasMaxLength(120);
            });

            modelBuilder.Entity<BannerMaster>(entity =>
            {
                entity.ToTable("BannerMaster");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AddedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.AddedOn).HasColumnType("datetime");

                entity.Property(e => e.Bannerdescription).HasMaxLength(200);

                entity.Property(e => e.Imagepath)
                    .IsUnicode(false)
                    .HasColumnName("IMAGEPATH");
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

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CategoryName).HasMaxLength(50);
            });

            modelBuilder.Entity<City>(entity =>
            {
                entity.Property(e => e.City1)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("city");

                entity.Property(e => e.StateId).HasColumnName("State_id");
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

                entity.Property(e => e.State).HasMaxLength(255);

                entity.Property(e => e.StateId).HasColumnName("stateId");

                entity.Property(e => e.WorkLocation)
                    .HasMaxLength(255)
                    .HasColumnName("Work_Location");
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
                entity.ToTable("Department_Master");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DepartmentName)
                    .HasMaxLength(120)
                    .HasColumnName("Department_Name");
            });

            modelBuilder.Entity<DesignationMaster>(entity =>
            {
                entity.ToTable("Designation_Master");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DesignationName)
                    .HasMaxLength(150)
                    .HasColumnName("Designation_Name");
            });

            modelBuilder.Entity<Empattendance>(entity =>
            {
                entity.ToTable("Empattendance");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Attendance).HasColumnName("attendance");

                entity.Property(e => e.EmployeeId)
                    .HasMaxLength(95)
                    .HasColumnName("Employee_ID");

                entity.Property(e => e.Entry).HasColumnType("date");

                entity.Property(e => e.GenerateSalary).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Incentive).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Lop).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TravellingAllowance).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Year).HasColumnName("year");
            });

            modelBuilder.Entity<EmployeeBankDetail>(entity =>
            {
                entity.ToTable("Employee_Bank_Details", "dbo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AccountHolderName)
                    .HasMaxLength(255)
                    .HasColumnName("Account_Holder_Name");

                entity.Property(e => e.AccountNumber)
                    .HasMaxLength(50)
                    .HasColumnName("Account_Number");

                entity.Property(e => e.AccountTypeId).HasColumnName("Account_Type_ID");

                entity.Property(e => e.BankName)
                    .HasMaxLength(255)
                    .HasColumnName("Bank_Name");

                entity.Property(e => e.Chequeimage).HasColumnName("chequeimage");

                entity.Property(e => e.DeductionCycle)
                    .HasMaxLength(120)
                    .HasColumnName("Deduction_Cycle");

                entity.Property(e => e.EmpId)
                    .HasMaxLength(100)
                    .HasColumnName("EmpID");

                entity.Property(e => e.EmployeeContributionRate)
                    .HasMaxLength(120)
                    .HasColumnName("Employee_Contribution_Rate");

                entity.Property(e => e.EmployeeRegistrationId).HasColumnName("Employee_Registration_ID");

                entity.Property(e => e.EpfNumber)
                    .HasMaxLength(120)
                    .HasColumnName("EPF_Number");

                entity.Property(e => e.Ifsc)
                    .HasMaxLength(11)
                    .HasColumnName("IFSC");

                entity.Property(e => e.Nominee)
                    .HasMaxLength(255)
                    .HasColumnName("nominee");

                entity.Property(e => e.ReEnterAccountNumber)
                    .HasMaxLength(50)
                    .HasColumnName("Re_Enter_Account_Number");
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

                entity.Property(e => e.EmployeeId)
                    .HasMaxLength(200)
                    .HasColumnName("Employee_ID");

                entity.Property(e => e.Password).HasMaxLength(120);
            });

            modelBuilder.Entity<EmployeePersonalDetail>(entity =>
            {
                entity.ToTable("Employee_Personal_Details");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AadharOne).HasColumnName("Aadhar_One");

                entity.Property(e => e.AadharTwo).HasColumnName("Aadhar_Two");

                entity.Property(e => e.Aadharcard)
                    .HasMaxLength(50)
                    .HasColumnName("aadharcard");

                entity.Property(e => e.AddressLine1)
                    .HasMaxLength(255)
                    .HasColumnName("Address_Line_1");

                entity.Property(e => e.AddressLine2)
                    .HasMaxLength(255)
                    .HasColumnName("Address_Line_2");

                entity.Property(e => e.City).HasMaxLength(255);

                entity.Property(e => e.DateOfBirth)
                    .HasColumnType("datetime")
                    .HasColumnName("Date_Of_Birth");

                entity.Property(e => e.EmpRegId).HasMaxLength(100);

                entity.Property(e => e.FatherName)
                    .HasMaxLength(255)
                    .HasColumnName("Father_Name");

                entity.Property(e => e.MobileNumber)
                    .HasMaxLength(50)
                    .HasColumnName("Mobile_Number");

                entity.Property(e => e.Pan)
                    .HasMaxLength(50)
                    .HasColumnName("PAN");

                entity.Property(e => e.Panimg).HasColumnName("panimg");

                entity.Property(e => e.PersonalEmailAddress)
                    .HasMaxLength(120)
                    .HasColumnName("Personal_Email_Address");

                entity.Property(e => e.Pincode).HasMaxLength(50);

                entity.Property(e => e.StateId)
                    .HasMaxLength(120)
                    .HasColumnName("State_ID");
            });

            modelBuilder.Entity<EmployeeRegistration>(entity =>
            {
                entity.ToTable("Employee_Registration", "dbo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                entity.Property(e => e.DateOfJoining)
                    .HasColumnType("datetime")
                    .HasColumnName("Date_Of_Joining");

                entity.Property(e => e.DepartmentId)
                    .HasMaxLength(120)
                    .HasColumnName("Department_ID");

                entity.Property(e => e.DesignationId)
                    .HasMaxLength(120)
                    .HasColumnName("Designation_ID");

                entity.Property(e => e.EmpProfile).HasColumnName("Emp_profile");

                entity.Property(e => e.EmployeeId)
                    .HasMaxLength(100)
                    .HasColumnName("Employee_ID");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(120)
                    .HasColumnName("First_Name");

                entity.Property(e => e.GenderId)
                    .HasMaxLength(120)
                    .HasColumnName("Gender_ID");

                entity.Property(e => e.LastName)
                    .HasMaxLength(120)
                    .HasColumnName("Last_Name");

                entity.Property(e => e.MiddleName)
                    .HasMaxLength(120)
                    .HasColumnName("Middle_Name");

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.StateId).HasColumnName("stateId");

                entity.Property(e => e.WorkEmail)
                    .HasMaxLength(120)
                    .HasColumnName("Work_Email");

                entity.Property(e => e.WorkLocationId)
                    .HasMaxLength(120)
                    .HasColumnName("Work_Location_ID");
            });

            modelBuilder.Entity<EmployeeRole>(entity =>
            {
                entity.ToTable("Employee_Role", "dbo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.EmployeeRegistrationId)
                    .HasMaxLength(200)
                    .HasColumnName("Employee_Registration_ID");

                entity.Property(e => e.EmployeeRole1)
                    .HasMaxLength(120)
                    .HasColumnName("Employee_Role");
            });

            modelBuilder.Entity<EmployeeSalaryDetail>(entity =>
            {
                entity.ToTable("Employee_Salary_Details");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("amount");

                entity.Property(e => e.AnnualCtc)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("AnnualCTC");

                entity.Property(e => e.Basic).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.EmpId).HasColumnName("EmpID");

                entity.Property(e => e.EmployeeId)
                    .HasMaxLength(120)
                    .HasColumnName("EmployeeID");

                entity.Property(e => e.EmployeeName).HasMaxLength(120);

                entity.Property(e => e.Epf)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("EPF");

                entity.Property(e => e.Esic)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("ESIC")
                    .HasDefaultValueSql("((1400.00))");

                entity.Property(e => e.Gross)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("gross");

                entity.Property(e => e.HouseRentAllowance).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Incentive).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.MonthlyCtc)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("MonthlyCTC");

                entity.Property(e => e.MonthlyGrossPay).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Professionaltax).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Servicecharge)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("servicecharge");

                entity.Property(e => e.SpecialAllowance).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Tdspercentage)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("tdspercentage");

                entity.Property(e => e.TravellingAllowance).HasColumnType("decimal(18, 0)");
            });

            modelBuilder.Entity<EmployeerEpf>(entity =>
            {
                entity.ToTable("Employeer_EPF");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.DeductionCycle)
                    .HasMaxLength(120)
                    .HasColumnName("Deduction_Cycle");

                entity.Property(e => e.EmployerContributionRate)
                    .HasMaxLength(120)
                    .HasColumnName("Employer_Contribution_Rate");

                entity.Property(e => e.EpfNumber)
                    .HasMaxLength(120)
                    .HasColumnName("EPF_Number");
            });

            modelBuilder.Entity<EmployeerTd>(entity =>
            {
                entity.ToTable("EmployeerTDS");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("amount");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                entity.Property(e => e.Isactive).HasColumnName("ISactive");

                entity.Property(e => e.Tdspercentage)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("tdspercentage");

                entity.Property(e => e.WorkLocationId)
                    .HasMaxLength(120)
                    .HasColumnName("Work_Location_ID");
            });

            modelBuilder.Entity<GenderMaster>(entity =>
            {
                entity.ToTable("Gender_Master", "dbo");

                entity.Property(e => e.Id)
                    .HasMaxLength(120)
                    .HasColumnName("ID");

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
                entity.ToTable("Quation");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CompanyName)
                    .HasMaxLength(255)
                    .HasColumnName("Company_Name");

                entity.Property(e => e.CustomerName)
                    .HasMaxLength(255)
                    .HasColumnName("Customer_Name");

                entity.Property(e => e.Email).HasMaxLength(255);

                entity.Property(e => e.Mobile).HasMaxLength(255);

                entity.Property(e => e.ProductId)
                    .HasMaxLength(255)
                    .HasColumnName("Product_ID");

                entity.Property(e => e.SalesPersonName)
                    .HasMaxLength(255)
                    .HasColumnName("Sales_Person_Name");

                entity.Property(e => e.Subject).HasMaxLength(255);
            });

            modelBuilder.Entity<State>(entity =>
            {
                entity.Property(e => e.SName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("S_Name");
            });

            modelBuilder.Entity<StateMaster>(entity =>
            {
                entity.ToTable("State_Master", "dbo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.StateName)
                    .HasMaxLength(255)
                    .HasColumnName("State_Name");
            });

            modelBuilder.Entity<TErrorLog>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("T_ErrorLog");

                entity.Property(e => e.EntryDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Method).HasMaxLength(500);

                entity.Property(e => e.Role)
                    .HasMaxLength(6)
                    .IsUnicode(false);

                entity.Property(e => e.UserId)
                    .HasMaxLength(50)
                    .HasColumnName("User_ID");
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
                entity.ToTable("Work_Location");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AddressLine1)
                    .HasMaxLength(255)
                    .HasColumnName("Address_Line_1");

                entity.Property(e => e.Commissoninpercentage)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("commissoninpercentage");
            });

            modelBuilder.Entity<WorkLocation1>(entity =>
            {
                entity.ToTable("WorkLocation");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CityId).HasColumnName("CityID");

                entity.Property(e => e.Commissoninpercentage)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("commissoninpercentage");

                entity.Property(e => e.Createdate)
                    .HasColumnType("datetime")
                    .HasColumnName("createdate");

                entity.Property(e => e.StateId).HasColumnName("stateId");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
