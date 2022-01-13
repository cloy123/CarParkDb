using System;
using System.Windows;
using System.Windows.Controls;
using CarParkDb.UI.Cars;
using CarParkDb.UI.Employees;
using CarParkDb.UI.Requests;
using CarParkDb.Infrastructure;
using CarParkDb.Domain.AggregationModels.CarAggregate;
using CarParkDb.Domain.AggregationModels.EmployeeAggregate;
using CarParkDb.Domain.AggregationModels.RequestAggregate;
using System.Linq;
using System.Text.RegularExpressions;
using CarParkDb.UI.Points;
using System.Collections.Generic;
using Point = CarParkDb.Domain.AggregationModels.PointAggregate.Point;

namespace CarParkDb
{
    public partial class MainWindow : Window
    {
        readonly string ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\Desktop\CarParkDb\CarParkDB.mdf;Integrated Security=True;Connect Timeout=30";
        CarParkDbController CarParkController;

        public Request request;
        public Point FromPoint;
        public Point ToPoint;
        public Car car;
        public Employee employee;

        public Request OldRequest;

        public MainWindow()
        {
            InitializeComponent();
            CarParkController = new CarParkDbController(ConnectionString);
        }

        private void AddCar_Click(object sender, RoutedEventArgs e)
        {
            AddCarWindow addCarWindow = new AddCarWindow();
            addCarWindow.ShowDialog();
            if (addCarWindow.IsOk)
            {
                CarParkController.CarRepository.Insert(addCarWindow.Car);
                RefreshCars();
            }
        }

        private void RefreshCars()
        {
            var cars = CarParkController.CarRepository.GetAll();
            if (IsCarWithFilters.IsChecked != null && IsCarWithFilters.IsChecked == true)
            {
                if(CarFirmFilter.Text.Trim().Length > 0)
                {
                    Regex regex = new Regex($@"^{CarFirmFilter.Text.Trim()}", RegexOptions.IgnoreCase);
                    cars = cars.Where(it => regex.IsMatch(it.Name.Firm)).ToList();
                }

                if(CarModelFilter.Text.Trim().Length > 0)
                {
                    Regex regex = new Regex($@"^{CarModelFilter.Text.Trim()}", RegexOptions.IgnoreCase);
                    cars = cars.Where(it => regex.IsMatch(it.Name.Model)).ToList();
                }

                if (CarNumberFilter.Text.Trim().Length > 0)
                {
                    Regex regex = new Regex($@"^{CarNumberFilter.Text.Trim()}", RegexOptions.IgnoreCase);
                    cars = cars.Where(it => regex.IsMatch(it.Number)).ToList();
                }

                if((IsCarFreeFilter.IsChecked == true || IsCarInTransitFilter.IsChecked == true) && !(IsCarFreeFilter.IsChecked == true && IsCarInTransitFilter.IsChecked == true))
                {
                    if(IsCarFreeFilter.IsChecked == true)
                    {
                        cars = cars.Where(it => it.Status == CarStatus.Free).ToList();
                    }
                    if(IsCarInTransitFilter.IsChecked == true)
                    {
                        cars = cars.Where(it => it.Status == CarStatus.InTransit).ToList();
                    }
                }

            }
            ListViewCars.ItemsSource = cars;
        }

        private void RefreshEmployees()
        {
            var employees = CarParkController.EmployeeRepository.GetAll();
            if(IsEmployeeWithFilters.IsChecked == true)
            {
                if(EmployeeFirstNameFilter.Text.Trim().Length > 0)
                {
                    Regex regex = new Regex($@"^{EmployeeFirstNameFilter.Text.Trim()}", RegexOptions.IgnoreCase);
                    employees = employees.Where(it => regex.IsMatch(it.Name.FirstName)).ToList();
                }

                if (EmployeeMiddleNameFilter.Text.Trim().Length > 0)
                {
                    Regex regex = new Regex($@"^{EmployeeMiddleNameFilter.Text.Trim()}", RegexOptions.IgnoreCase);
                    employees = employees.Where(it => regex.IsMatch(it.Name.MiddleName)).ToList();
                }

                if(EmployeeExperienceFilter.Value > 0)
                {
                    employees = employees.Where(it => it.Experience >= EmployeeExperienceFilter.Value).ToList();
                }

                if ((IsEmployeeFreeFilter.IsChecked == true || IsEmployeeInTransitFilter.IsChecked == true) && !(IsEmployeeFreeFilter.IsChecked == true && IsEmployeeInTransitFilter.IsChecked == true))
                {
                    if (IsEmployeeFreeFilter.IsChecked == true)
                    {
                        employees = employees.Where(it => it.Status == EmployeeStatus.Free).ToList();
                    }
                    if (IsEmployeeInTransitFilter.IsChecked == true)
                    {
                        employees = employees.Where(it => it.Status == EmployeeStatus.InTransit).ToList();
                    }
                }
            }
            ListViewEmployees.ItemsSource = employees;
        }

        private void ChangeCar_Click(object sender, RoutedEventArgs e)
        {
            if (ListViewCars.SelectedItem == null)
            {
                return;
            }
            if (((Car)ListViewCars.SelectedItem).Status == CarStatus.InTransit)
            {
                MessageBox.Show("Невозможно изменить пока автомобиль в пути!");
                return;
            }
            var carName = new CarName(CarFirm.Text, CarModel.Text);
            var newCar = new Car(carName, CarNumber.Text);
            newCar.Status = ((Car)ListViewCars.SelectedItem).Status;
            newCar.SetId(((Car)ListViewCars.SelectedItem).Id);

            CarParkController.CarRepository.Update(newCar);
            var selected = ListViewCars.SelectedIndex;
            RefreshCars();
            ListViewCars.SelectedIndex = selected;
        }

        private void DeleteCar_Click(object sender, RoutedEventArgs e)
        {
            if (ListViewCars.SelectedItem == null)
            {
                return;
            }
            if (((Car)ListViewCars.SelectedItem).Status == CarStatus.InTransit)
            {
                MessageBox.Show("Невозможно удалить пока автомобиль в пути!");
                return;
            }
            CarParkController.CarRepository.Delete(((Car)ListViewCars.SelectedItem).Id);
            RefreshCars();
            ClearCurrentCar();
        }

        private void AddEmployee_Click(object sender, RoutedEventArgs e)
        {
            AddEmployeeWindow addEmployeeWindow = new AddEmployeeWindow();
            addEmployeeWindow.ShowDialog();
            if (addEmployeeWindow.IsOk)
            {
                CarParkController.EmployeeRepository.Insert(addEmployeeWindow.Employee);
                RefreshEmployees();
            }
        }

        private void ChangeEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (ListViewEmployees.SelectedItem == null)
            {
                return;
            }
            if (((Employee)ListViewEmployees.SelectedItem).Status == EmployeeStatus.InTransit)
            {
                MessageBox.Show("Невозможно изменить пока водитель в пути!");
                return;
            }
            var employee = (Employee)ListViewEmployees.SelectedItem;
            var employeeName = new EmployeeName(EmployeeFirstName.Text, EmployeeLastName.Text, EmployeeMiddleName.Text);
            employee.SetName(employeeName);
            employee.EmploymentDate = (DateTime)EmployeeEmploymentDate.SelectedDate;
            employee.Salary = (decimal)EmployeeSalary.Value;
            var selected = ListViewEmployees.SelectedIndex;
            CarParkController.EmployeeRepository.Update(employee);
            EmployeeExperience.Content = employee.Experience;
            RefreshEmployees();
            ListViewEmployees.SelectedIndex = selected;
        }

        private void DeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (ListViewEmployees.SelectedItem == null)
            {
                return;
            }
            if (((Employee)ListViewEmployees.SelectedItem).Status == EmployeeStatus.InTransit)
            {
                MessageBox.Show("Невозможно удалить пока водитель в пути!");
                return;
            }
            CarParkController.EmployeeRepository.Delete(((Employee)ListViewEmployees.SelectedItem).Id);
            RefreshEmployees();
            ClearCurrentEmployee();
        }

        private void RefreshRequests()
        {
            var requests = CarParkController.RequestRepository.GetAll();
            var newRequests = new List<Request>();
            if((IsRequestCompleteFilter.IsChecked == true || IsRequestCreatedFilter.IsChecked == true || IsRequestInTransitFilter.IsChecked == true) && 
                !(IsRequestCompleteFilter.IsChecked == true && IsRequestCreatedFilter.IsChecked == true && IsRequestInTransitFilter.IsChecked == true))
            {
                if(IsRequestCompleteFilter.IsChecked == true)
                {
                    newRequests.AddRange(requests.Where(it => it.Status == RequestStatus.Completed).ToList());
                }

                if (IsRequestCreatedFilter.IsChecked == true)
                {
                    newRequests.AddRange(requests.Where(it => it.Status == RequestStatus.AwaitingDispatch).ToList());
                }

                if (IsRequestInTransitFilter.IsChecked == true)
                {
                    newRequests.AddRange(requests.Where(it => it.Status == RequestStatus.InTransit).ToList());
                }
            }
            else
            {
                newRequests.AddRange(requests);
            }
            ListViewRequests.ItemsSource = newRequests;
        }

        private void RefreshPoints()
        {
            var points = CarParkController.PointRepository.GetAll();
            if(PointNameFilter.Text.Trim().Length > 0)
            {
                Regex regex = new Regex($@"^{PointNameFilter.Text.Trim()}", RegexOptions.IgnoreCase);
                points = points.Where(it => regex.IsMatch(it.PointName)).ToList();
            }
            ListViewPoints.ItemsSource = points;
        }

        private void AddRequest_Click(object sender, RoutedEventArgs e)
        {
            AddRequestWindow addRequestWindow = new AddRequestWindow(CarParkController.CarRepository.GetAll(),
                CarParkController.EmployeeRepository.GetAll(), CarParkController.PointRepository.GetAll());
            addRequestWindow.ShowDialog();
            if (addRequestWindow.IsOk)
            {
                CarParkController.RequestRepository.Insert(addRequestWindow.request);
                RefreshRequests();
            }
        }

        private void ChangeRequest_Click(object sender, RoutedEventArgs e)
        {
            if (ListViewRequests.SelectedItem == null)
            {
                return;
            }
            var newRequest = (Request)ListViewRequests.SelectedItem;

            OldRequest = CarParkController.RequestRepository.GetById(newRequest.Id);

            request = newRequest;

            FromPoint = new Point(request.SentFrom);
            FromPoint.SetId(request.PointFromId);

            ToPoint = new Point(request.SentTo);
            ToPoint.SetId(request.PointToId);

            Tabs.SelectedIndex = 4;
            car = request.Car;
            employee = request.Employee;
            CreateRequestCar.Content = car.Name.Firm.Trim() + " " + car.Name.Model.Trim() + " " + car.Number.Trim();
            CreateRequestEmployee.Content = employee.Name.MiddleNameAndInitials;
            CreateRequestFrom.Content = FromPoint.PointName.Trim();
            CreateRequestDestination.Content = ToPoint.PointName.Trim();
            CreateRequestOk.Content = "Изменить";

            #region старое
            //var newRequest = (Request)ListViewRequests.SelectedItem;
            //var newCar = newRequest.Car;
            //var newEmployee = newRequest.Employee;
            //AddRequestWindow addRequestWindow = new AddRequestWindow(CarParkController.CarRepository.GetAll(),
            //CarParkController.EmployeeRepository.GetAll(), CarParkController.PointRepository.GetAll(),
            //newRequest);
            //addRequestWindow.ShowDialog();
            //if (addRequestWindow.IsOk)
            //{
            //    CarParkController.RequestRepository.Update(addRequestWindow.request);
            //}

            //if(newRequest.Status != RequestStatus.InTransit)
            //{
            //    RefreshRequests();
            //    return;
            //}

            //    //предыдущему ставлю статус свободно 
            //    newCar.Status = CarStatus.Free;
            //    CarParkController.CarRepository.Update(newCar);

            //    //новому ставлю статус в пути
            //    var newSelectedCar = addRequestWindow.request.Car;
            //    newSelectedCar.Status = CarStatus.InTransit;
            //    CarParkController.CarRepository.Update(newSelectedCar);

            //    newEmployee.Status = EmployeeStatus.Free;
            //    CarParkController.EmployeeRepository.Update(newEmployee);

            //    //новому ставлю статус в пути
            //    var newSelectedEmployee = addRequestWindow.request.Employee;
            //    newSelectedEmployee.Status = EmployeeStatus.InTransit;
            //    CarParkController.EmployeeRepository.Update(newSelectedEmployee);
            //RefreshRequests();
            #endregion
        }


        //тут
        private void Send_Click(object sender, RoutedEventArgs e)
        {
            if (ListViewRequests.SelectedItem == null)
            {
                return;
            }
            var newRequest = ((Request)ListViewRequests.SelectedItem);
            
            
            if (newRequest.Status == RequestStatus.AwaitingDispatch && newRequest.Car.Status == CarStatus.InTransit)
            {
                MessageBox.Show("Невозможно отправить так как этот автомобиль сейчас занят.");
                return;
            }
            if (newRequest.Status == RequestStatus.AwaitingDispatch && newRequest.Employee.Status == EmployeeStatus.InTransit)
            {
                MessageBox.Show("Невозможно отправить так как этот водитель сейчас занят.");
                return;
            }

            if (newRequest.Status == RequestStatus.AwaitingDispatch)
            {
                newRequest.Sent(DateTime.Today);
            }
            else if (newRequest.Status == RequestStatus.InTransit)
            {
                newRequest.Сomplete(DateTime.Today);
            }

            CarParkController.EmployeeRepository.Update(newRequest.Employee);
            CarParkController.CarRepository.Update(newRequest.Car);
            CarParkController.RequestRepository.Update(newRequest);

            var selected = ListViewRequests.SelectedIndex;

            RefreshRequests();
            ListViewRequests.SelectedIndex = selected;
        }

        private void DeleteRequest_Click(object sender, RoutedEventArgs e)
        {
            if (ListViewRequests.SelectedItem == null)
            {
                return;
            }
            var newRequest = (Request)ListViewRequests.SelectedItem;

            if (newRequest.Status == RequestStatus.InTransit)
            {
                var newEmployee = newRequest.Employee;
                newEmployee.Status = EmployeeStatus.Free;
                CarParkController.EmployeeRepository.Update(newEmployee);

                var newCar = newRequest.Car;
                newCar.Status = CarStatus.Free;
                CarParkController.CarRepository.Update(newCar);
            }

            CarParkController.RequestRepository.Delete(newRequest.Id);
            RefreshRequests();
            ClearCurrentRequest();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e != null && e.Source is TabControl) || (sender != null && sender is Request))
            {
                if (TabCars.IsSelected)
                {
                    RefreshCars();
                    ClearCurrentCar();
                }
                if (TabEmployees.IsSelected)
                {
                    RefreshEmployees();
                    ClearCurrentEmployee();
                }
                if (TabRequests.IsSelected)
                {
                    RefreshRequests();
                    ClearCurrentRequest();
                }
                if (TabPoints.IsSelected)
                {
                    RefreshPoints();
                }
            }
        }

        private void ClearCurrentRequest()
        {
            //RequestId.Content = "";
            //SelectRequestStatus.Content = "";
            //From.Text = "";
            //Destination.Text = ""; ;
            //StartDate.Content = "";
            //FinishDate.Content = "";
            //RequestCar.Content = "";
            //RequestEmployee.Content = "";
        }

        private void ClearCurrentCar()
        {
            //CarId.Content = "";
            //CarFirm.Text = "";
            //CarModel.Text = "";
            //CarNumber.Text = "";
            //SelectCarStatus.Content = "";
        }

        private void ClearCurrentEmployee()
        {
            //try
            //{
            //    EmployeeEmploymentDate.ClearValue((DependencyProperty)DependencyProperty.UnsetValue);
            //}
            //catch { }
            //EmployeeId.Content = "";
            //EmployeeExperience.Content = "";
            //EmployeeFirstName.Text = "";
            //EmployeeLastName.Text = "";
            //EmployeeMiddleName.Text = "";
            //EmployeeSalary.Value = 0;
            //SelectEmployeeStatus.Content = "";
        }

        private void ListViewCars_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListViewCars.SelectedItem != null)
            {
                CarId.Content = ((Car)ListViewCars.SelectedItem).Id;
                CarFirm.Text = ((Car)ListViewCars.SelectedItem).Name.Firm.Trim();
                CarModel.Text = ((Car)ListViewCars.SelectedItem).Name.Model.Trim();
                CarNumber.Text = ((Car)ListViewCars.SelectedItem).Number.Trim();
                SelectCarStatus.Content = ((Car)ListViewCars.SelectedItem).Status.Name;
            }
        }

        private void ListViewEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListViewEmployees.SelectedItem != null)
            {
                EmployeeFirstName.Text = ((Employee)ListViewEmployees.SelectedItem).Name.FirstName.Trim();
                EmployeeMiddleName.Text = ((Employee)ListViewEmployees.SelectedItem).Name.MiddleName.Trim();
                EmployeeLastName.Text = ((Employee)ListViewEmployees.SelectedItem).Name.LastName.Trim();
                EmployeeExperience.Content = ((Employee)ListViewEmployees.SelectedItem).Experience;
                EmployeeSalary.Value = (int)((Employee)ListViewEmployees.SelectedItem).Salary;
                SelectEmployeeStatus.Content = ((Employee)ListViewEmployees.SelectedItem).Status.Name;
                EmployeeEmploymentDate.SelectedDate = ((Employee)ListViewEmployees.SelectedItem).EmploymentDate;
            }
        }

        private void ListViewRequests_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListViewRequests.SelectedItem != null)
            {
                SelectRequestStatus.Content = ((Request)ListViewRequests.SelectedItem).Status.Name;
                RequestId.Content = ((Request)ListViewRequests.SelectedItem).Id;
                From.Content = ((Request)ListViewRequests.SelectedItem).SentFrom.Trim();
                Destination.Content = ((Request)ListViewRequests.SelectedItem).SentTo.Trim();
                StartDate.Content = ((Request)ListViewRequests.SelectedItem).StartDateStr;
                FinishDate.Content = ((Request)ListViewRequests.SelectedItem).FinishDateStr;
                if(RequestCar != null)
                {
                    RequestCar.Content = ((Request)ListViewRequests.SelectedItem).CarName;
                }
                if(RequestEmployee != null)
                {
                    RequestEmployee.Content = ((Request)ListViewRequests.SelectedItem).EmployeeName;
                }
                if (((Request)ListViewRequests.SelectedItem).Status == RequestStatus.AwaitingDispatch)
                {
                    Send.Visibility = Visibility.Visible;
                    Send.Content = "Отправить";
                }
                else if (((Request)ListViewRequests.SelectedItem).Status == RequestStatus.InTransit)
                {
                    Send.Content = "Отметить доставленным";
                    Send.Visibility = Visibility.Visible;
                }
                else if (((Request)ListViewRequests.SelectedItem).Status == RequestStatus.Completed)
                {
                    Send.Visibility = Visibility.Hidden;
                }
            }
        }

        private void CarTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e != null && e.Source is TabControl))
            {
                if (SelectedCarTab.IsSelected)
                {
                    if (ListViewCars.SelectedItem != null)
                    {
                        CarId.Content = ((Car)ListViewCars.SelectedItem).Id;
                        CarFirm.Text = ((Car)ListViewCars.SelectedItem).Name.Firm.Trim();
                        CarModel.Text = ((Car)ListViewCars.SelectedItem).Name.Model.Trim();
                        CarNumber.Text = ((Car)ListViewCars.SelectedItem).Number.Trim();
                        SelectCarStatus.Content = ((Car)ListViewCars.SelectedItem).Status.Name;
                    }
                }
            }
        }

        private void IsCarWithFilters_Checked(object sender, RoutedEventArgs e)
        {
            RefreshCars();
        }

        private void IsCarWithFilters_Unchecked(object sender, RoutedEventArgs e)
        {
            RefreshCars();
        }

        private void CarFirmFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsCarWithFilters.IsChecked == true)
            {
                RefreshCars();
            }
        }

        private void IsCarFreeFilter_Checked(object sender, RoutedEventArgs e)
        {
            if(IsCarWithFilters.IsChecked == true)
            {
                RefreshCars();
            }
        }

        private void IsCarFreeFilter_Unchecked(object sender, RoutedEventArgs e)
        {
            if (IsCarWithFilters.IsChecked == true)
            {
                RefreshCars();
            }
        }

        private void EmployeeTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e != null && e.Source is TabControl))
            {
                if (SelectedEmployeeTab.IsSelected)
                {
                    if (ListViewEmployees.SelectedItem != null)
                    {
                        EmployeeFirstName.Text = ((Employee)ListViewEmployees.SelectedItem).Name.FirstName.Trim();
                        EmployeeMiddleName.Text = ((Employee)ListViewEmployees.SelectedItem).Name.MiddleName.Trim();
                        EmployeeLastName.Text = ((Employee)ListViewEmployees.SelectedItem).Name.LastName.Trim();
                        EmployeeExperience.Content = ((Employee)ListViewEmployees.SelectedItem).Experience;
                        EmployeeSalary.Value = (int)((Employee)ListViewEmployees.SelectedItem).Salary;
                        SelectEmployeeStatus.Content = ((Employee)ListViewEmployees.SelectedItem).Status.Name;
                        EmployeeEmploymentDate.SelectedDate = ((Employee)ListViewEmployees.SelectedItem).EmploymentDate;
                    }
                }
            }
        }

        private void IsEmployeeFreeFilter_Checked(object sender, RoutedEventArgs e)
        {
            if (IsEmployeeWithFilters.IsChecked == true)
            {
                RefreshEmployees();
            }
        }

        private void IsEmployeeFreeFilter_Unchecked(object sender, RoutedEventArgs e)
        {
            if (IsEmployeeWithFilters.IsChecked == true)
            {
                RefreshEmployees();
            }
        }

        private void IsEmployeeWithFilters_Checked(object sender, RoutedEventArgs e)
        {
            RefreshEmployees();
        }

        private void IsEmployeeWithFilters_Unchecked(object sender, RoutedEventArgs e)
        {
            RefreshEmployees();
        }

        private void EmployeeMiddleNameFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsEmployeeWithFilters.IsChecked == true)
            {
                RefreshEmployees();
            }
        }

        private void EmployeeExperienceFilter_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (IsEmployeeWithFilters.IsChecked == true)
            {
                RefreshEmployees();
            }
        }

        private void AddPoint_Click(object sender, RoutedEventArgs e)
        {
            AddPointWindow addPointWindow = new AddPointWindow();
            addPointWindow.ShowDialog();
            if (addPointWindow.IsOk)
            {
                CarParkController.PointRepository.Insert(addPointWindow.Point);
                RefreshPoints();
            }
        }

        private void ListViewPoints_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListViewPoints.SelectedItem != null)
            {
                PointId.Content = ((Domain.AggregationModels.PointAggregate.Point)ListViewPoints.SelectedItem).Id;
                PointName.Text = ((Domain.AggregationModels.PointAggregate.Point)ListViewPoints.SelectedItem).PointName.Trim();
            }
        }

        private void PointTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e != null && e.Source is TabControl))
            {
                if (SelectedPointTab.IsSelected)
                {
                    if (ListViewPoints.SelectedItem != null)
                    {
                        PointId.Content = ((Domain.AggregationModels.PointAggregate.Point)ListViewPoints.SelectedItem).Id;
                        PointName.Text = ((Domain.AggregationModels.PointAggregate.Point)ListViewPoints.SelectedItem).PointName.Trim();
                    }
                }
            }
        }

        private void ChangePoint_Click(object sender, RoutedEventArgs e)
        {
            if (ListViewPoints.SelectedItem == null)
            {
                return;
            }
            if(PointName.Text.Trim().Length == 0)
            {
                return;
            }
            var newPoint = new Domain.AggregationModels.PointAggregate.Point(PointName.Text.Trim());
            newPoint.SetId(((Domain.AggregationModels.PointAggregate.Point)ListViewPoints.SelectedItem).Id);

            CarParkController.PointRepository.Update(newPoint);
            var selected = ListViewCars.SelectedIndex;
            RefreshPoints();
            ListViewPoints.SelectedIndex = selected;
        }

        private void DeletePoint_Click(object sender, RoutedEventArgs e)
        {
            if (ListViewPoints.SelectedItem == null)
            {
                return;
            }
            CarParkController.PointRepository.Delete(((Domain.AggregationModels.PointAggregate.Point)ListViewPoints.SelectedItem).Id);
            RefreshPoints();
        }

        private void PointNameFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshPoints();
        }

        private void RequestTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e != null && e.Source is TabControl))
            {
                if (SelectedRequestTab.IsSelected)
                {
                    if (ListViewRequests.SelectedItem != null)
                    {
                        SelectRequestStatus.Content = ((Request)ListViewRequests.SelectedItem).Status.Name;
                        RequestId.Content = ((Request)ListViewRequests.SelectedItem).Id;
                        From.Content = ((Request)ListViewRequests.SelectedItem).SentFrom.Trim();
                        Destination.Content = ((Request)ListViewRequests.SelectedItem).SentTo.Trim();
                        StartDate.Content = ((Request)ListViewRequests.SelectedItem).StartDateStr;
                        FinishDate.Content = ((Request)ListViewRequests.SelectedItem).FinishDateStr;
                        if (RequestCar != null)
                        {
                            RequestCar.Content = ((Request)ListViewRequests.SelectedItem).CarName;
                        }
                        if (RequestEmployee != null)
                        {
                            RequestEmployee.Content = ((Request)ListViewRequests.SelectedItem).EmployeeName;
                        }
                        if (((Request)ListViewRequests.SelectedItem).Status == RequestStatus.AwaitingDispatch)
                        {
                            Send.Visibility = Visibility.Visible;
                            Send.Content = "Отправить";
                        }
                        else if (((Request)ListViewRequests.SelectedItem).Status == RequestStatus.InTransit)
                        {
                            Send.Content = "Отметить доставленным";
                            Send.Visibility = Visibility.Visible;
                        }
                        else if (((Request)ListViewRequests.SelectedItem).Status == RequestStatus.Completed)
                        {
                            Send.Visibility = Visibility.Hidden;
                        }
                    }
                }
            }
        }

        private void IsRequestCreatedFilter_Checked(object sender, RoutedEventArgs e)
        {
            RefreshRequests();
        }

        private void IsRequestCreatedFilter_Unchecked(object sender, RoutedEventArgs e)
        {
            RefreshRequests();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (FromPoint == null || ToPoint == null || car == null || employee == null)
            {
                return;
            }
            if (request == null)
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
            
            if (request.Id > 0)
            {
                CarParkController.RequestRepository.Update(request);
                
                if (request.Status == RequestStatus.InTransit)
                {
                    //предыдущему ставлю статус свободно 
                    OldRequest.Car.Status = CarStatus.Free;
                    CarParkController.CarRepository.Update(OldRequest.Car);

                    //новому ставлю статус в пути
                    var newSelectedCar = request.Car;
                    newSelectedCar.Status = CarStatus.InTransit;
                    CarParkController.CarRepository.Update(newSelectedCar);

                    OldRequest.Employee.Status = EmployeeStatus.Free;
                    CarParkController.EmployeeRepository.Update(OldRequest.Employee);

                    //новому ставлю статус в пути
                    var newSelectedEmployee = request.Employee;
                    newSelectedEmployee.Status = EmployeeStatus.InTransit;
                    CarParkController.EmployeeRepository.Update(newSelectedEmployee);
                }

            }
            else
            {
                CarParkController.RequestRepository.Insert(request);
            }
            
            request = null;
            FromPoint = null;
            ToPoint = null;
            car = null;
            employee = null;
            CreateRequestCar.Content = "";
            CreateRequestEmployee.Content = "";
            CreateRequestFrom.Content = "";
            CreateRequestDestination.Content = "";
            CreateRequestOk.Content = "Оформить";

            MessageBox.Show("Выполнено");
        }

        private void SelectFrom_Click(object sender, RoutedEventArgs e)
        {
            SelectPointWindow window = new SelectPointWindow(CarParkController.PointRepository.GetAll());
            window.ShowDialog();
            if (window.IsOk)
            {
                FromPoint = window.Point;
                CreateRequestFrom.Content = FromPoint.PointName.Trim();
            }
        }

        private void SelectDestination_Click(object sender, RoutedEventArgs e)
        {
            SelectPointWindow window = new SelectPointWindow(CarParkController.PointRepository.GetAll());
            window.ShowDialog();
            if (window.IsOk)
            {
                ToPoint = window.Point;
                CreateRequestDestination.Content = ToPoint.PointName.Trim();
            }
        }

        private void SelectCar_Click(object sender, RoutedEventArgs e)
        {
            var cars = CarParkController.CarRepository.GetAll();
            if (request != null && request.Status == RequestStatus.InTransit)
            {
                cars = cars.Where(it => it.Status == CarStatus.Free).ToList();
            }
            SelectCarWindow window = new SelectCarWindow(cars);
            window.ShowDialog();
            if (window.IsOk)
            {
                car = window.Car;
                CreateRequestCar.Content = car.Name.Firm.Trim() + " " + car.Name.Model.Trim() + " " + car.Number.Trim();
            }
        }

        private void SelectEmployee_Click(object sender, RoutedEventArgs e)
        {
            var employees = CarParkController.EmployeeRepository.GetAll();
            if (request != null && request.Status == RequestStatus.InTransit)
            {
                employees = employees.Where(it => it.Status == EmployeeStatus.Free).ToList();
            }
            SelectEmployeeWindow window = new SelectEmployeeWindow(employees);
            window.ShowDialog();
            if (window.IsOk)
            {
                employee = window.Employee;
                CreateRequestEmployee.Content = employee.Name.MiddleNameAndInitials;
            }
        }
    }
}
