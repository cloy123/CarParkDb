using CarParkDb.Domain.Models;

namespace CarParkDb.Domain.AggregationModels.CarAggregate
{
    public class CarStatus : Enumeration
    {
        public static CarStatus Free = new CarStatus(1, "Свободен");

        public static CarStatus InTransit = new CarStatus(2, "В пути");

        public CarStatus(int id, string name) : base(id, name)
        {
        }
    }
}