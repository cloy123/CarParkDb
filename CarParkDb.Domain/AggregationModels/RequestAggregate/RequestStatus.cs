using CarParkDb.Domain.Models;

namespace CarParkDb.Domain.AggregationModels.RequestAggregate
{
    public class RequestStatus : Enumeration
    {
        public static RequestStatus AwaitingDispatch = new RequestStatus(1, "Ожидает отправки");

        public static RequestStatus Completed = new RequestStatus(3, "Выполнено");

        public static RequestStatus InTransit = new RequestStatus(2, "В пути");

        public RequestStatus(int id, string name) : base(id, name)
        {
        }
    }
}