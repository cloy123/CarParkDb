using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CarParkDb.Domain.AggregationModels.PointAggregate;

namespace CarParkDb.UI.Points
{
    /// <summary>
    /// Логика взаимодействия для AddPointWindow.xaml
    /// </summary>
    public partial class AddPointWindow : Window
    {
        public bool IsOk = false;
        public Domain.AggregationModels.PointAggregate.Point Point;

        public AddPointWindow()
        {
            InitializeComponent();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (PointName.Text.Trim() == "")
            {
                MessageBox.Show("Ошибка: Не все поля заполнены.");
            }
            else
            {
                IsOk = true;
                Point = new Domain.AggregationModels.PointAggregate.Point(PointName.Text.Trim());
                Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        { Close(); }
    }
}
