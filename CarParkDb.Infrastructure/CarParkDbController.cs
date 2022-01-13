using CarParkDb.Infrastructure.Repositories;
using System.Data.SqlClient;

namespace CarParkDb.Infrastructure
{
    public class CarParkDbController
    {
        public string ConnectionString;

        public CarRepository CarRepository;

        public EmployeeRepository EmployeeRepository;

        public RequestRepository RequestRepository;

        public PointRepository PointRepository;

        public CarParkDbController(string connectionString)
        {
            ConnectionString = connectionString;
            CarRepository = new CarRepository(ConnectionString);
            EmployeeRepository = new EmployeeRepository(connectionString);
            RequestRepository = new RequestRepository(connectionString);
            PointRepository = new PointRepository(connectionString);
        }
    }
}