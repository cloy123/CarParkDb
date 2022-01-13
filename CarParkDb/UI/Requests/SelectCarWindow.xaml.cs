using CarParkDb.Domain.AggregationModels.CarAggregate;
using System.Collections.Generic;
using System.Windows;

namespace CarParkDb.UI.Requests
{
    public partial class SelectCarWindow : Window
    {
        public bool IsOk = false;
        public Car Car;
        
        public SelectCarWindow(List<Car> cars)
        {
            InitializeComponent();
            Cars.ItemsSource = cars;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (Cars.SelectedItem == null)
            {
                MessageBox.Show("Ошибка: Автомобиль не выбран.");
            }
            else
            {
                IsOk = true;
                Car = (Car)Cars.SelectedItem;
                Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}