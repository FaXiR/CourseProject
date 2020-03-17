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
                if (table.Count == 0)
                {
                    MessageBox.Show("Записей в таком промежутке несуществует");
                    return;
                }

                report.OutToExcell("Список дел", table);
            }
            else if (F_Date_start.Text != "" && F_Date_end.Text != "")
            {
                string StartDate = F_Date_start.Text.Substring(3, 2) + "/" + F_Date_start.Text.Substring(0, 2) + "/" + F_Date_start.Text.Substring(6, 4);
                string EndDate = F_Date_end.Text.Substring(3, 2) + "/" + F_Date_end.Text.Substring(0, 2) + "/" + F_Date_end.Text.Substring(6, 4);

                var table = UsAc.Execute($@"Select Номер_дела as [Номер дела], Дата_введения_на_хранение as [Введено на хранение], Причина_открытия as [Причина открытия], Дата_открытия as [Дата открытия], Дата_закрытия as [Дата закрытия], Заверитель FROM Дело WHERE Дата_введения_на_хранение > #{StartDate}# AND Дата_введения_на_хранение < #{EndDate}#");
                if (table.Count == 0)
                {
                    MessageBox.Show("Записей в таком промежутке несуществует");
                    return;
                }

                report.OutToExcell($@"Список дел от {F_Date_start.Text} до {F_Date_end.Text}", table);
            }
            else if (F_Date_start.Text != "")
            {
                string StartDate = F_Date_start.Text.Substring(3, 2) + "/" + F_Date_start.Text.Substring(0, 2) + "/" + F_Date_start.Text.Substring(6, 4);

                var table = UsAc.Execute($@"Select Номер_дела as [Номер дела], Дата_введения_на_хранение as [Введено на хранение], Причина_открытия as [Причина открытия], Дата_открытия as [Дата открытия], Дата_закрытия as [Дата закрытия], Заверитель FROM Дело WHERE Дата_введения_на_хранение > #{StartDate}#");
                if (table.Count == 0)
                {
                    MessageBox.Show("Записей в таком промежутке несуществует");
                    return;
                }

                report.OutToExcell($@"Список дел от {F_Date_start.Text}", table);
            }
            else if (F_Date_end.Text != "")
            {
                string EndDate = F_Date_end.Text.Substring(3, 2) + "/" + F_Date_end.Text.Substring(0, 2) + "/" + F_Date_end.Text.Substring(6, 4);

                var table = UsAc.Execute($@"Select Номер_дела as [Номер дела], Дата_введения_на_хранение as [Введено на хранение], Причина_открытия as [Причина открытия], Дата_открытия as [Дата открытия], Дата_закрытия as [Дата закрытия], Заверитель FROM Дело WHERE Дата_введения_на_хранение < #{EndDate}#");
                if (table.Count == 0)
                {
                    MessageBox.Show("Записей в таком промежутке несуществует");
                    return;
                }

                report.OutToExcell($@"Список дел до {F_Date_end.Text}", table);
            }
            this.DialogResult = true;
        }

        private void F_OutToWord(object sender, RoutedEventArgs e)
        {
            report.OutToWord();
            this.DialogResult = true;
        }
    }
}
