using System;
using CarParkDb.Domain.AggregationModels.CarAggregate;
using CarParkDb.Domain.AggregationModels.EmployeeAggregate;
using CarParkDb.Domain.Models;

namespace CarParkDb.Domain.AggregationModels.RequestAggregate
{
    public class Request : Entity
    {
        public int CarId { get; private set; }
        public Car Car { get; private set; }

        public string CarName { get
            {
                if(Car == null || Car.Name == null)
                {
                    return "не задано";
                }
                else
                {
                    try
                    {
                        return Car.Name.Firm.Trim() + " " + Car.Name.Model.Trim() + " " + Car.Number.Trim();

                    }
                    catch
                    {
                        return "";
                    }
                }
            } }

        public int EmployeeId { get; private set; }
        public Employee Employee { get; private set; }

        public string EmployeeName
        {
            get
            {
                if (Employee == null)
                {
                    return "не задано";
                }
                else
                {
                    return Employee.Name.MiddleNameAndInitials;
                }
            }
        }

        public DateTime StartDate { get; set; }

        public string StartDateStr { get
            {
                return StartDate.Day + "/" + StartDate.Month + "/" + StartDate.Year;
            } }

        public string FinishDateStr
        {
            get
            {
                return FinishDate.Day + "/" + FinishDate.Month + "/" + FinishDate.Year;
            }
        }

        public DateTime FinishDate { get; set; }

        public int PointFromId { get; set; }
        public int PointToId { get; set; }
        public string SentFrom { get; set; }
        
        public string SentTo { get; set; }
        
        public RequestStatus Status { get; set; }

        public Request(int pointFromId, int pointToId)
        {
            PointFromId = pointFromId;
            PointToId = pointToId;
            Status = RequestStatus.AwaitingDispatch;
        }

        public void SetCar(Car car)
        {
            Car = car;
            CarId = car.Id;
        }

        public void SetEmployee(Employee employee)
        {
            Employee = employee;
            EmployeeId = employee.Id;
        }
        
        public void Sent(DateTime startDate)
        {
            StartDate = startDate;
            Status = RequestStatus.InTransit;
            Car.Status = CarStatus.InTransit;
            Employee.Status = EmployeeStatus.InTransit;
        }

        public void Сomplete(DateTime finishdate)
        {
            FinishDate = finishdate;
            Status = RequestStatus.Completed;
            Car.Status = CarStatus.Free;
            Employee.Status = EmployeeStatus.Free;
        }
    }
}