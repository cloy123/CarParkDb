using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using CarParkDb.Domain.AggregationModels.CarAggregate;
using CarParkDb.Domain.AggregationModels.EmployeeAggregate;
using CarParkDb.Domain.AggregationModels.RequestAggregate;
using CarParkDb.Domain.Models;
using CarParkDb.Infrastructure;
using Dapper;

namespace CarParkDb.Infrastructure.Repositories
{
    public class RequestRepository
    {
        public string ConnectionString;

        public RequestRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public void Insert(Domain.AggregationModels.RequestAggregate.Request request)
        {
            var connection = new SqlConnection(ConnectionString);
            connection.Open();
            var command = $"INSERT INTO Requests (PointFromId, PointToId, Status";
            var commandValues = $"({request.PointFromId}, " +
                                $"{request.PointToId}, " +
                                $"{request.Status.Id}";

            if(request.StartDate.Year != 1)
            {
                command += $", StartDate";
                commandValues += $", {ConvertDate(request.StartDate)}";
            }

            if (request.FinishDate.Year != 1)
            {
                command += $", FinishDate";
                commandValues += $", {ConvertDate(request.FinishDate)}";
            }

            if (request.Car != null)
            {
                command += $", CarId";
                commandValues += $", {request.CarId}";
            }

            if (request.Employee != null)
            {
                command += $", EmployeeId";
                commandValues += $", {request.EmployeeId}";
            }

            command += ") VALUES " + commandValues + ")";
            
            SqlCommand sqlCommand = new SqlCommand(command, connection);
            sqlCommand.ExecuteNonQuery();
            connection.Close();
        }

        public void Update(Domain.AggregationModels.RequestAggregate.Request request)
        {
            var connection = new SqlConnection(ConnectionString);
            connection.Open();
            var command = $"UPDATE Requests SET PointFromId = {request.PointFromId}, " +
                          $"PointToId = {request.PointToId}, " +
                          $"Status = '{request.Status.Id}'";
            
            if(request.StartDate.Year != 1)
            {
                command += $", StartDate = {ConvertDate(request.StartDate)}";
            }

            if (request.FinishDate.Year != 1)
            {
                command += $", FinishDate = {ConvertDate(request.FinishDate)}";
            }

            if (request.Car != null)
            {
                command += $", CarId = {request.CarId}";
            }

            if (request.Employee != null)
            {
                command += $", EmployeeId = {request.EmployeeId}";
            }

            command += $" WHERE Id = {request.Id}";
            SqlCommand sqlCommand = new SqlCommand(command, connection);
            sqlCommand.ExecuteNonQuery();
            connection.Close();
        }
        
        public void Delete(int id)
        {
            var connection = new SqlConnection(ConnectionString);
            connection.Open();
            var command = $"DELETE FROM Requests WHERE Id = {id}";
            SqlCommand sqlCommand = new SqlCommand(command, connection);
            sqlCommand.ExecuteNonQuery();
            connection.Close();
        }

        public Request GetById(int id)
        {
            
            Request request;
            var command = $"SELECT Requests.Id AS RequestsId, " +
                          $"Requests.CarId, " +
                          $"Requests.EmployeeId, " +
                          $"Requests.StartDate, " +
                          $"Requests.FinishDate, " +
                          $"Requests.PointFromId, " +
                          $"Requests.PointToId, " +
                          $"Requests.Status, " +
                          $"Cars.Id, " +
                          $"Cars.Firm, " +
                          $"Cars.Model, " +
                          $"Cars.Number, " +
                          $"Cars.Status AS CarStatus, " +
                          $"Employees.Id, " +
                          $"Employees.Status AS EmployeeStatus, " +
                          $"Employees.EmploymentDate, " +
                          $"Employees.Salary, " +
                          $"Employees.FirstName, " +
                          $"Employees.LastName, " +
                          $"Employees.MiddleName, " +
                          $"PointsFrom.PointName AS SentFrom, " +
                          $"PointsTo.PointName AS SentTo " +
                      $"FROM Requests LEFT JOIN Cars ON (Cars.Id = Requests.CarId) " +
                      $"LEFT JOIN Points AS PointsFrom ON (PointsFrom.Id = Requests.PointFromId) " +
                      $"LEFT JOIN Points AS PointsTo ON (PointsTo.Id = Requests.PointToId) " +
                      $"LEFT JOIN Employees ON (Employees.Id = Requests.EmployeeId) " +
                      $"WHERE Requests.Id = {id}";

            var connection = new SqlConnection(ConnectionString);
            connection.Open();

            using (IDbConnection db = connection)
            {
                var requestsDto = db.Query<Models.Request>(command);
                request = requestsDto.Select(requestDto =>
                    {
                        var newRequest = new Domain.AggregationModels.RequestAggregate.Request(requestDto.PointFromId, requestDto.PointToId);
                        newRequest.SetId(requestDto.RequestsId);
                        newRequest.SentFrom = requestDto.SentFrom;
                        newRequest.SentTo = requestDto.SentTo;
                        newRequest.StartDate = requestDto.StartDate;
                        newRequest.FinishDate = requestDto.FinishDate;
                        newRequest.Status = Enumeration.GetAll<RequestStatus>()
                            .FirstOrDefault(it => it.Id.Equals(requestDto.Status));

                        if (requestDto.CarId != null)
                        {
                            var car = new Car(new CarName(requestDto.Firm, requestDto.Model), requestDto.Number);
                            car.SetId((int)requestDto.CarId);
                            car.Status = Enumeration.GetAll<CarStatus>().FirstOrDefault(it => it.Id.Equals(requestDto.CarStatus));
                            newRequest.SetCar(car);
                        }

                        if (requestDto.EmployeeId != null)
                        {
                            var employee = new Employee(
                                new EmployeeName(requestDto.FirstName, requestDto.LastName, requestDto.MiddleName),
                                requestDto.EmploymentDate,
                                requestDto.Salary);
                            employee.SetId((int)requestDto.EmployeeId);
                            employee.Status = Enumeration.GetAll<EmployeeStatus>()
                                .FirstOrDefault(it => it.Id.Equals(requestDto.EmployeeStatus));
                            newRequest.SetEmployee(employee);
                        }
                        return newRequest;
                    }).ToList()[0];
            }
            connection.Close();
            return request;
        }

        public List<Domain.AggregationModels.RequestAggregate.Request> GetAll()
        {
            var connection = new SqlConnection(ConnectionString);
            connection.Open();
            List<Domain.AggregationModels.RequestAggregate.Request> requests;
            var command = $"SELECT Requests.Id AS RequestsId, " +
                          $"Requests.CarId, " +
                          $"Requests.EmployeeId, " +
                          $"Requests.StartDate, " +
                          $"Requests.FinishDate, " +
                          $"Requests.PointFromId, " +
                          $"Requests.PointToId, " +
                          $"Requests.Status, " +
                          $"Cars.Id, " +
                          $"Cars.Firm, " +
                          $"Cars.Model, " +
                          $"Cars.Number, " +
                          $"Cars.Status AS CarStatus, " +
                          $"Employees.Id, " +
                          $"Employees.Status AS EmployeeStatus, " +
                          $"Employees.EmploymentDate, " +
                          $"Employees.Salary, " +
                          $"Employees.FirstName, " +
                          $"Employees.LastName, " +
                          $"Employees.MiddleName, " +
                          $"PointsFrom.PointName AS SentFrom, " +
                          $"PointsTo.PointName AS SentTo " +
                      $"FROM Requests LEFT JOIN Cars ON (Cars.Id = Requests.CarId) " +
                      $"LEFT JOIN Points AS PointsFrom ON (PointsFrom.Id = Requests.PointFromId) " +
                      $"LEFT JOIN Points AS PointsTo ON (PointsTo.Id = Requests.PointToId) " +
                      $"LEFT JOIN Employees ON (Employees.Id = Requests.EmployeeId)";
            
            using (IDbConnection db = connection)
            {
                var requestsDto = db.Query<Models.Request>(command);
                requests = requestsDto.Select(requestDto =>
                {
                    var newRequest = new Domain.AggregationModels.RequestAggregate.Request(requestDto.PointFromId, requestDto.PointToId);
                    newRequest.SetId(requestDto.RequestsId);
                    newRequest.SentFrom = requestDto.SentFrom;
                    newRequest.SentTo = requestDto.SentTo;
                    newRequest.StartDate = requestDto.StartDate;
                    newRequest.FinishDate = requestDto.FinishDate;
                    newRequest.Status = Enumeration.GetAll<RequestStatus>()
                        .FirstOrDefault(it => it.Id.Equals(requestDto.Status));

                    if (requestDto.CarId != null)
                    {
                        var car = new Car(new CarName(requestDto.Firm, requestDto.Model), requestDto.Number);
                        car.SetId((int)requestDto.CarId);
                        car.Status = Enumeration.GetAll<CarStatus>().FirstOrDefault(it => it.Id.Equals(requestDto.CarStatus));
                        newRequest.SetCar(car);
                    }

                    if (requestDto.EmployeeId != null)
                    {
                        var employee = new Employee(
                            new EmployeeName(requestDto.FirstName, requestDto.LastName, requestDto.MiddleName),
                            requestDto.EmploymentDate,
                            requestDto.Salary);
                        employee.SetId((int)requestDto.EmployeeId);
                        employee.Status = Enumeration.GetAll<EmployeeStatus>()
                            .FirstOrDefault(it => it.Id.Equals(requestDto.EmployeeStatus));
                        newRequest.SetEmployee(employee);
                    }
                    return newRequest;
                }).ToList();
            }
           // throw new Exception(re)
            connection.Close();
            return requests;
        }

        string ConvertDate(DateTime date)
        {
            return $"CONVERT(date, '{date.Day + "/" + date.Month + "/" + date.Year}', 103)";
        }
    }
}