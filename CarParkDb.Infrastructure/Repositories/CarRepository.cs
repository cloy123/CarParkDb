

using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using CarParkDb.Domain.AggregationModels.CarAggregate;
using CarParkDb.Domain.Models;
using CarParkDb.Infrastructure;
using Dapper;

namespace CarParkDb.Infrastructure.Repositories
{
    public class CarRepository
    {
        public string ConnectionString;

        public CarRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public void Insert(Domain.AggregationModels.CarAggregate.Car car)
        {
            var connection = new SqlConnection(ConnectionString);
            connection.Open();
            var command = $"INSERT INTO Cars (Firm, Model, Number, Status) " +
                          $"VALUES (N'{car.Name.Firm}', N'{car.Name.Model}', N'{car.Number}', {car.Status.Id})";
            SqlCommand sqlCommand = new SqlCommand(command, connection);
            sqlCommand.ExecuteNonQuery();
            connection.Close();
        }

        public void Update(Domain.AggregationModels.CarAggregate.Car car)
        {
            var connection = new SqlConnection(ConnectionString);
            connection.Open();
            var command = $"UPDATE Cars SET Firm = N'{car.Name.Firm}', " +
                          $"Model = N'{car.Name.Model}', " +
                          $"Number = N'{car.Number}', " +
                          $"Status = {car.Status.Id} " +
                          $"WHERE Id = {car.Id}";
            SqlCommand sqlCommand = new SqlCommand(command, connection);
            sqlCommand.ExecuteNonQuery();
            connection.Close();
        }

        public void Delete(int id)
        {
            var connection = new SqlConnection(ConnectionString);
            connection.Open();
            var command = $"DELETE FROM Cars WHERE Id = {id}";
            SqlCommand sqlCommand = new SqlCommand(command, connection);
            sqlCommand.ExecuteNonQuery();
            connection.Close();
        }

        public Domain.AggregationModels.CarAggregate.Car GetById(int id)
        {
            var connection = new SqlConnection(ConnectionString);
            connection.Open();
            Domain.AggregationModels.CarAggregate.Car car;
            using(IDbConnection db = connection)
            {
                var carDto = db.QueryFirst<Models.Car>($"SELECT * FROM Cars WHERE Id = {id}");
                car = new Domain.AggregationModels.CarAggregate.Car(new CarName(carDto.Firm, carDto.Model), carDto.Number);
                car.SetId(carDto.Id);
                car.Status = Enumeration.GetAll<CarStatus>().FirstOrDefault(it => it.Id.Equals(carDto.Status));
            }
            connection.Close();
            return car;
        }

        public List<Domain.AggregationModels.CarAggregate.Car> GetAll()
        {
            var connection = new SqlConnection(ConnectionString);
            connection.Open();
            List<Domain.AggregationModels.CarAggregate.Car> cars;
            using(IDbConnection db = connection)
            {
                var carsDto = db.Query<Models.Car>($"SELECT * FROM Cars");
                cars = carsDto.Select(x =>
                {
                    var car = new Car(new CarName(x.Firm, x.Model), x.Number);
                    car.SetId(x.Id);
                    car.Status = Enumeration.GetAll<CarStatus>().FirstOrDefault(it => it.Id.Equals(x.Status));
                    return car;
                }).ToList();
            }
            connection.Close();
            return cars;
        }
    }
}