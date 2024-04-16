﻿using BaseLibrary.Entities;
using Microsoft.EntityFrameworkCore;

namespace ServerLibrary.Data
{
    public class EmpDbContext(DbContextOptions<EmpDbContext> options) : DbContext(options)
    {
        public DbSet<Employee> Employees { get; set; }

        //General Department / Department / Branch
        public DbSet<GeneralDepartment> GeneralDepartments { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Branch> Branches { get; set; }

        //Country / City / Town
        public DbSet<Town> Towns { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }

        //Authentication / Role / SystemRoles
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<SystemRole> SystemRoles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RefreshTokenInfo> RefreshTokenInfos { get; set; }

        //Others / vacation / Sanction / Overtime / Doctor
        public DbSet<OverTime> OverTimes {get; set;}
        public DbSet<OvertimeType> OvertimeTypes {get; set;}
        public DbSet<Vacation> Vacations {get; set;}
        public DbSet<VacationType> VacationTypes {get; set;}
        public DbSet<Sanction> Sanctions {get; set;}
        public DbSet<SanctionType> SanctionTypes {get; set;}
        public DbSet<Doctor> Doctors {get; set;}
        
    }
}

        
    

