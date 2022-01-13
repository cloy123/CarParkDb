using CarParkDb.Domain.Models;

namespace CarParkDb.Domain.AggregationModels.CarAggregate
{
    public class Car : Entity
    {
        public CarName Name { get; set; }
        public string Number { get; set; }
        public CarStatus Status { get; set; }

        public Car(CarName name, string number)
        {
            Name = name;
            Number = number;
            Status = CarStatus.Free;
        }
        
        
    }
}