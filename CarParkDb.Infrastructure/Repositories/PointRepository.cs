using CarParkDb.Domain.AggregationModels.PointAggregate;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarParkDb.Infrastructure.Repositories
{
    public class PointRepository
    {
        public string ConnectionString;

        public PointRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public void Insert(Point point)
        {
            var connection = new SqlConnection(ConnectionString);
            connection.Open();
            var command = $"INSERT INTO Points (PointName) " +
                          $"VALUES (N'{point.PointName}')";
            SqlCommand sqlCommand = new SqlCommand(command, connection);
            sqlCommand.ExecuteNonQuery();
            connection.Close();
        }

        public void Update(Point point)
        {
            var connection = new SqlConnection(ConnectionString);
            connection.Open();
            var command = $"UPDATE Points SET PointName = N'{point.PointName}'" +
                          $"WHERE Id = {point.Id}";
            SqlCommand sqlCommand = new SqlCommand(command, connection);
            sqlCommand.ExecuteNonQuery();
            connection.Close();
        }

        public void Delete(int id)
        {
            var connection = new SqlConnection(ConnectionString);
            connection.Open();
            var command = $"DELETE FROM Points WHERE Id = {id}";
            SqlCommand sqlCommand = new SqlCommand(command, connection);
            sqlCommand.ExecuteNonQuery();
            connection.Close();
        }

        public Point GetById(int id)
        {
            var connection = new SqlConnection(ConnectionString);
            connection.Open();
            Point point;
            using (IDbConnection db = connection)
            {
                var pointDto = db.QueryFirst<Models.Point>($"SELECT * FROM Points WHERE Id = {id}");
                point = new Domain.AggregationModels.PointAggregate.Point(pointDto.PointName);
                point.SetId(pointDto.Id);
            }
            connection.Close();
            return point;
        }

        public List<Point> GetAll()
        {
            var connection = new SqlConnection(ConnectionString);
            connection.Open();
            List<Point> points;
            using (IDbConnection db = connection)
            {
                var pointsDto = db.Query<Models.Point>($"SELECT * FROM Points");
                points = pointsDto.Select(x =>
                {
                    var point = new Point(x.PointName);
                    point.SetId(x.Id);
                    return point;
                }).ToList();
            }
            connection.Close();
            return points;
        }
    }
}
