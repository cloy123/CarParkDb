using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using CarParkDb.Domain.AggregationModels.EmployeeAggregate;
using CarParkDb.Domain.Models;
using CarParkDb.Infrastructure;
using Dapper;

namespace CarParkDb.Infrastructure.Repositories
{
    public class EmployeeRepository
    {
        public string ConnectionString;

        public EmployeeRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }
        
        public void Insert(Domain.AggregationModels.EmployeeAggregate.Employee employee)
        {
            var connection = new SqlConnection(ConnectionString);
            connection.Open();
            var command = $"INSERT INTO Employees (FirstName, LastName, MiddleName, Status, EmploymentDate, Salary) " +
                          $"VALUES (N'{employee.Name.FirstName}', " +
                          $"N'{employee.Name.LastName}', " +
                          $"N'{employee.Name.MiddleName}', " +
                          $"{employee.Status.Id}, " +
                          $"{ConvertDate(employee.EmploymentDate)}, " +
                          $"'{((int)employee.Salary)}')";
            SqlCommand sqlCommand = new SqlCommand(command, connection);
            sqlCommand.ExecuteNonQuery();
            connection.Close();
        }

        public void Update(Domain.AggregationModels.EmployeeAggregate.Employee employee)
        {
            var connection = new SqlConnection(ConnectionString);
            connection.Open();
            var command = $"UPDATE Employees SET FirstName = N'{employee.Name.FirstName}', " +
                          $"LastName = N'{employee.Name.LastName}', " +
                          $"MiddleName = N'{employee.Name.MiddleName}', " +
                          $"Status = {employee.Status.Id}, " +
                          $"EmploymentDate = {ConvertDate(employee.EmploymentDate)}, " +
                          $"Salary = '{((int)employee.Salary)}' " +
                          $"WHERE Id = {employee.Id}";
            SqlCommand sqlCommand = new SqlCommand(command, connection);
            sqlCommand.ExecuteNonQuery();
            connection.Close();
        }

        public void Delete(int id)
        {
            var connection = new SqlConnection(ConnectionString);
            connection.Open();
            var command = $"DELETE FROM Employees WHERE Id = {id}";
            SqlCommand sqlCommand = new SqlCommand(command, connection);
            sqlCommand.ExecuteNonQuery();
            connection.Close();
        }

        public Domain.AggregationModels.EmployeeAggregate.Employee GetById(int id)
        {
            var connection = new SqlConnection(ConnectionString);
            connection.Open();
            Domain.AggregationModels.EmployeeAggregate.Employee employee;
            using(IDbConnection db = connection)
            {
                var employeeDto = db.QueryFirst<Models.Employee>($"SELECT * FROM Employees WHERE Id = {id}");
                employee = new Domain.AggregationModels.EmployeeAggregate.Employee(
                    new EmployeeName(employeeDto.FirstName, employeeDto.LastName, employeeDto.MiddleName),
                    employeeDto.EmploymentDate,
                    employeeDto.Salary);
                employee.SetId(employeeDto.Id);
                employee.Status = Enumeration.GetAll<EmployeeStatus>()
                    .FirstOrDefault(it => it.Id.Equals(employeeDto.Status));
            }
            connection.Close();
            return employee;
        }

        public List<Domain.AggregationModels.EmployeeAggregate.Employee> GetAll()
        {
            var connection = new SqlConnection(ConnectionString);
            connection.Open();
            List<Domain.AggregationModels.EmployeeAggregate.Employee> employees;
            using(IDbConnection db = connection)
            {
                var employeesDto = db.Query<Models.Employee>($"SELECT * FROM Employees");
                employees = employeesDto.Select(x =>
                {
                    var employee = new Employee(new EmployeeName(x.FirstName, x.LastName, x.MiddleName),
                        x.EmploymentDate,
                        x.Salary);
                    employee.SetId(x.Id);
                    employee.Status = Enumeration.GetAll<EmployeeStatus>()
                        .FirstOrDefault(it => it.Id.Equals(x.Status));
                    return employee;
                }).ToList();
            }
            connection.Close();
            return employees;
        }

        string ConvertDate(DateTime date)
        {
            return $"CONVERT(date, '{date.Day + "/" + date.Month + "/" + date.Year}', 103)";
        }
    }
}