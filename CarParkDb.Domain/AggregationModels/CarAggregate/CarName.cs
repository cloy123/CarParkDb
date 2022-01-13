using System.Collections.Generic;
using CarParkDb.Domain.Models;

namespace CarParkDb.Domain.AggregationModels.CarAggregate
{
    public class CarName : ValueObject
    {
        public string Firm { get; }
        public string Model { get; }

        public CarName(string firm, string model)
        {
            Firm = firm;
            Model = model;
        }
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Firm;
            yield return Model;
        }
    }
}