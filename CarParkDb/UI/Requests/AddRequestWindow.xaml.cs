using CarParkDb.Domain.AggregationModels.CarAggregate;
using CarParkDb.Domain.AggregationModels.EmployeeAggregate;
using CarParkDb.Domain.AggregationModels.RequestAggregate;
using CarParkDb.Domain.AggregationModels.PointAggregate;
using System.Collections.Generic;
using System.Windows;
using Point = CarParkDb.Domain.AggregationModels.PointAggregate.Point;
using System.Linq;

namespace CarParkDb.UI.Requests
{
    public partial class AddRequestWindow : Window
    {
        public bool IsOk = false;
        List<Car> Cars;
        List<Employee> Employees;
        List<Point> Points;

        public Request request;
        public Point FromPoint;
        public Point ToPoint;
        public Car car;
        public Employee employee;

        public AddRequestWindow(List<Car> cars, List<Employee> employees, List<Point> points)
        {
            InitializeComponent();
            Cars = cars;
            Employees = employees;
            Points = points;
        }

        public AddRequestWindow(List<Car> cars, List<Employee> employees, List<Point> points, Request request)
        {
            InitializeComponent();
            Cars = cars;
            Employees = employees;
            Points = points;
            this.request = request;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if(FromPoint == null || ToPoint == null || car == null || employee == null)
            {
                return;
            }
            IsOk = true;
            if(request == null)
            {
                request = new Request(FromPoint.Id, ToPoint.Id);
            }
            else
            {
                request.PointFromId = FromPoint.Id;
                request.PointToId = ToPoint.Id;
            }
            request.SetEmployee(employee);
            request.SetCar(car);
            Close();
        }

        private void SelectFrom_Click(object sender, RoutedEventArgs e)
        {
            SelectPointWindow window = new SelectPointWindow(Points);
            window.ShowDialog();
            if (window.IsOk)
            {
                FromPoint = window.Point;
                From.Content = FromPoint.PointName.Trim();
            }
        }

        private void SelectDestination_Click(object sender, RoutedEventArgs e)
        {
            SelectPointWindow window = new SelectPointWindow(Points);
            window.ShowDialog();
            if (window.IsOk)
            {
                ToPoint = window.Point;
                Destination.Content = ToPoint.PointName.Trim();
            }
        }

        private void SelectCar_Click(object sender, RoutedEventArgs e)
        {
            SelectCarWindow window = new SelectCarWindow(Cars);
            window.ShowDialog();
            if (window.IsOk)
            {
                car = window.Car;
                Car.Content = car.Name.Firm.Trim() + " " + car.Name.Model.Trim() + " " + car.Number.Trim();
            }
        }

        private void SelectEmployee_Click(object sender, RoutedEventArgs e)
        {
            SelectEmployeeWindow window = new SelectEmployeeWindow(Employees);
            window.ShowDialog();
            if (window.IsOk)
            {
                employee = window.Employee;
                Employee.Content = employee.Name.MiddleNameAndInitials;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(request != null)
            {
                if(request.Status == RequestStatus.InTransit)
                {
                    Cars = Cars.Where(it => it.Status == CarStatus.Free).ToList();
                    Employees = Employees.Where(it => it.Status == EmployeeStatus.Free).ToList();
                }

                car = request.Car;
                employee = request.Employee;
                FromPoint = new Point(request.SentFrom);
                FromPoint.SetId(request.PointFromId);
                ToPoint = new Point(request.SentTo);
                ToPoint.SetId(request.PointToId);
                Title = "Изменить заявку";
                Ok.Content = "Сохранить";
              
                Employee.Content = employee.Name.MiddleNameAndInitials;
                Destination.Content = ToPoint.PointName.Trim();
                From.Content = FromPoint.PointName.Trim();
                Car.Content = car.Name.Firm.Trim() + " " + car.Name.Model.Trim() + " " + car.Number.Trim();
            }
        }
    }
}