using System;

namespace CarParkDb.Infrastructure.Models
{
    public class Request
    {
        public int RequestsId{ get; set; }
        
        public int Status { get; set; }
        
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }

        public int PointFromId { get; set; }
        public int PointToId { get; set; }

        public string SentFrom { get; set; }

        public string SentTo { get; set; }
        
        public int? CarId { get; set; }
        public int? EmployeeId { get; set; }

        //
        public int EmployeeStatus { get; set; }

        public DateTime EmploymentDate { get; set; }

        public decimal Salary { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }
        //
        public string Firm { get; set; }
        public string Model { get; set; }
        public string Number { get; set; }
        public int CarStatus { get; set; }



    }
}