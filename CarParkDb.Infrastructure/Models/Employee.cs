using System;

namespace CarParkDb.Infrastructure.Models
{
    public class Employee
    {
        public int Id { get; set; }
        
        public int Status { get; set; }
        
        public DateTime EmploymentDate { get; set; }
        
        public decimal Salary { get; set; }
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public string MiddleName { get; set; }
    }
}