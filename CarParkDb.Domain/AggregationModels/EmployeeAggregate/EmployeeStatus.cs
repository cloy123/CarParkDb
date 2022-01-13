using CarParkDb.Domain.Models;

namespace CarParkDb.Domain.AggregationModels.EmployeeAggregate
{
    public class EmployeeStatus : Enumeration
    {
        public static EmployeeStatus Free = new EmployeeStatus(1, "Свободен");

        public static EmployeeStatus InTransit = new EmployeeStatus(2, "В пути");

        public EmployeeStatus(int id, string name) : base(id, name)
        {
        }
    }
}