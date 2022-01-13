using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace CarParkDb.UI.Requests
{
    /// <summary>
    /// Логика взаимодействия для SelectPointWindow.xaml
    /// </summary>
    public partial class SelectPointWindow : Window
    {
        public SelectPointWindow()
        {
            InitializeComponent();
        }

        public bool IsOk = false;
        public Domain.AggregationModels.PointAggregate.Point Point;
        List<Domain.AggregationModels.PointAggregate.Point> Points;
        public SelectPointWindow(List<Domain.AggregationModels.PointAggregate.Point> points)
        {
            InitializeComponent();
            Points = points;
            ListViewPoint.ItemsSource = points;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (ListViewPoint.SelectedItem == null)
            {
                MessageBox.Show("Ошибка: Пункт не выбран.");
            }
            else
            {
                IsOk = true;
                Point = (Domain.AggregationModels.PointAggregate.Point)ListViewPoint.SelectedItem;
                Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Filter_TextChanged(object sender, TextChangedEventArgs e)
        {
            Regex regex = new Regex($@"^{Filter.Text.Trim()}", RegexOptions.IgnoreCase);
            var points = Points.Where(it => regex.IsMatch(it.PointName)).ToList();
            ListViewPoint.ItemsSource = points;
        }
    }
}
