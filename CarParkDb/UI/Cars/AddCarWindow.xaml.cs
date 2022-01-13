using CarParkDb.Domain.AggregationModels.CarAggregate;
using System.Windows;

namespace CarParkDb.UI.Cars
{
    public partial class AddCarWindow : Window
    {
        public AddCarWindow()
        {
            InitializeComponent();
        }
        
        public bool IsOk = false;
        public Car Car;
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (CarFirm.Text.Trim() == "" || CarModel.Text.Trim() == "" || CarNumber.Text.Trim() == "")
            {
                MessageBox.Show("Ошибка: Не все поля заполнены.");
            }
            else
            {
                IsOk = true;
                var carName = new CarName(CarFirm.Text.Trim(), CarModel.Text.Trim());
                Car = new Car(carName, CarNumber.Text);
                Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        { Close(); }
    }
}