using CarParkDb.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarParkDb.Domain.AggregationModels.PointAggregate
{
    public class Point : Entity
    {
        public string PointName { get; set; }

        public Point(string pointName)
        {
            PointName = pointName;
        }
    }
}
