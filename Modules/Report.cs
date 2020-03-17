using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseProject.Modules
{
    class Report
    {
        /// <summary>
        /// Вывод таблицы в эксель
        /// </summary>
        public void OutToExcell(string title, DataView table)
        {
            var excelapp = new Microsoft.Office.Interop.Excel.Application();
            var workbook = excelapp.Workbooks.Add();
            Microsoft.Office.Interop.Excel.Worksheet worksheet = workbook.ActiveSheet;

            //Получение названий колонок
            var ColumnName = new List<string>();
            for (int i = 0; i < table.Table.Columns.Count; i++)
            {
                ColumnName.Add(table.Table.Columns[i].ToString());
            }

            //Выводим название колонок
            for (int x = 0; x < ColumnName.Count; x++)
            {
                worksheet.Rows[2].Columns[x + 1] = ColumnName[x];
            }

            //заполням ячейки
            for (int y = 3; y < table.Count + 3; y++)
            {
                for (int x = 0; x < ColumnName.Count; x++)
                {
                    worksheet.Rows[y].Columns[x + 1] = table.Table.Rows[y - 3][ColumnName[x]];
                }
            }

            // (Титульник над содержимым) Выделяем диапазон ячеек от A1 до числа столбцов из DataView       
            Microsoft.Office.Interop.Excel.Range TitleRange = (Microsoft.Office.Interop.Excel.Range)worksheet.get_Range((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[1, 1], (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[1, ColumnName.Count]).Cells;

            // Производим объединение
            TitleRange.Merge(Type.Missing);

            //Размер текста
            TitleRange.Cells.Font.Size = 16;

            //Выравнивание по центру
            TitleRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

            //Задание bold для текста
            TitleRange.Font.Bold = true;

            //Задаем название титульника
            worksheet.Cells[1, 1] = title;

            //Выделение всех ячеек с данными
            Microsoft.Office.Interop.Excel.Range ContentRange = (Microsoft.Office.Interop.Excel.Range)worksheet.get_Range((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[1, 1], (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[table.Count + 2, ColumnName.Count]).Cells;

            //Выставление линий 
            ContentRange.Borders.get_Item(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeBottom).LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
            ContentRange.Borders.get_Item(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeRight).LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
            ContentRange.Borders.get_Item(Microsoft.Office.Interop.Excel.XlBordersIndex.xlInsideHorizontal).LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
            ContentRange.Borders.get_Item(Microsoft.Office.Interop.Excel.XlBordersIndex.xlInsideVertical).LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
            ContentRange.Borders.get_Item(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeTop).LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;

            //Выставление автоширины
            ContentRange.EntireColumn.AutoFit();

            //Отображаем Excel
            excelapp.AlertBeforeOverwriting = false;
            excelapp.Visible = true;
        }

        public void OutToWord()
        {

        }

    }
}
