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

namespace CourseProject.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddDeal.xaml
    /// </summary>
    public partial class AddDeal : Window
    {
        public AddDeal()
        {
            InitializeComponent();
        }
        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
        private void CanselButtonClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
        public string Deal
        {
            get { return F_Deal.Text; }
            set { F_Deal.Text = value; }
        }
    }
}
