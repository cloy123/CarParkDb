using CarParkDb.Domain.AggregationModels.EmployeeAggregate;
using System;
using System.Windows;

namespace CarParkDb.UI.Employees
{
    public partial class AddEmployeeWindow : Window
    {
        public bool IsOk = false;
        public Employee Employee;
        
        public AddEmployeeWindow()
        {
            InitializeComponent();
        }
        
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (!Date.SelectedDate.HasValue || MiddleName.Text.Trim() == "" || FirstName.Text.Trim() == "" || LastName.Text.Trim() == "")
            {
                MessageBox.Show("Ошибка: Не все поля заполнены.");
            }
            else
            {
                IsOk = true;
                var employeeName = new EmployeeName(FirstName.Text.Trim(), LastName.Text.Trim(), MiddleName.Text.Trim());
                Employee = new Employee(employeeName, (DateTime)Date.SelectedDate, (decimal)Salary.Value);
                Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        { Close(); }
    }
}