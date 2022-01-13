using CarParkDb.Domain.AggregationModels.EmployeeAggregate;
using System.Collections.Generic;
using System.Windows;

namespace CarParkDb.UI.Requests
{
    public partial class SelectEmployeeWindow : Window
    {
        public bool IsOk = false;
        public Employee Employee;
        public SelectEmployeeWindow(List<Employee> employees)
        {
            InitializeComponent();
            ListViewEmployee.ItemsSource = employees;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (ListViewEmployee.SelectedItem == null)
            {
                MessageBox.Show("Ошибка: Водитель не выбран.");
            }
            else
            {
                IsOk = true;
                Employee = (Employee)ListViewEmployee.SelectedItem;
                Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}