using CourseProject.Modules;
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
    /// Логика взаимодействия для ToReport.xaml
    /// </summary>
    public partial class ToReport : Window
    {
        UsingAccess UsAc;
        Report report = new Report();

        public ToReport(UsingAccess UsAc, string Deal)
        {
            InitializeComponent();

            this.UsAc = UsAc;

            if (Deal == null)
            {
                F_Grid_Word.IsEnabled = false;
            }
            else
            {
                F_TextBlock_Deal.Text = Deal;
            }
        }

        private void F_OutToExcell(object sender, RoutedEventArgs e)
        {
            if (F_Date_start.Text == "" && F_Date_end.Text == "")
            {
                var table = UsAc.Execute("Select Номер_дела as [Номер дела], Дата_введения_на_хранение as [Введено на хранение], Причина_открытия as [Причина открытия], Дата_открытия as [Дата открытия], Дата_закрытия as [Дата закрытия], Заверитель FROM Дело");
                report.OutToExcell("Список дел", table);
            }
            else if (F_Date_start.Text != "" && F_Date_end.Text != "")
            {
                var table = UsAc.Execute($@"Select Номер_дела as [Номер дела], Дата_введения_на_хранение as [Введено на хранение], Причина_открытия as [Причина открытия], Дата_открытия as [Дата открытия], Дата_закрытия as [Дата закрытия], Заверитель FROM Дело WHERE Дата_введения_на_хранение BETWEEN ""{F_Date_start.Text}"" AND ""{F_Date_end.Text}""");
                report.OutToExcell($@"Список дел от {F_Date_start.Text} до {F_Date_end.Text}", table);
            }
            else if (F_Date_start.Text != "")
            {
               
            }
            else if (F_Date_end.Text != "")
            {
                MessageBox.Show("DateEnd void");
            }

            this.DialogResult = true;
        }

        private void F_OutToWord(object sender, RoutedEventArgs e)
        {

        }
    }
}
