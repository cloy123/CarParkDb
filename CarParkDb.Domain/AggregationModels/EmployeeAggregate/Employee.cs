using System;
using CarParkDb.Domain.Models;

namespace CarParkDb.Domain.AggregationModels.EmployeeAggregate
{
    public class Employee : Entity
    {
        public EmployeeName Name { get; private set; }
        
        public EmployeeStatus Status { get; set; }
        
        public DateTime EmploymentDate { get; set; }

        public string EmploymentDateStr { get
            {
                return EmploymentDate.Day + "/" + EmploymentDate.Month + "/" + EmploymentDate.Year;
            } }

        public decimal Salary { get; set; }
        
        public void SetName(EmployeeName name)
        {
            Name = name;
        }
        
        public Employee(EmployeeName name, DateTime employmentDate, decimal salary)
        {
            if (employmentDate == null)
            {
                throw new ArgumentNullException("В аргумент employmentDate передано null");
            }
            Name = name;
            EmploymentDate = employmentDate;
            Status = EmployeeStatus.Free;
            Salary = salary;
        }

        public int Experience
        {
            get
            {
                if (DateTime.Now.Month < EmploymentDate.Month)
                {
                    return DateTime.Now.Year - EmploymentDate.Year - 1;
                }
                else if (DateTime.Now.Month == EmploymentDate.Month)
                {
                    if (DateTime.Now.Day < EmploymentDate.Day)
                    {
                        return DateTime.Now.Year - EmploymentDate.Year - 1;
                    }
                    else
                    {
                        return DateTime.Now.Year - EmploymentDate.Year;
                    }
                }
                else
                {
                    return DateTime.Now.Year - EmploymentDate.Year;
                }
            }
        }
    }
}