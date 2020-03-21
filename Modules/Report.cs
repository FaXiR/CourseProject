using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Word;

namespace CourseProject.Modules
{
    class Report
    {
        UsingAccess UsAc;

        public Report(UsingAccess UsAc)
        {
            this.UsAc = UsAc;
        }

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

        public void OutToWord(string NumOfDeal)
        {
            //Получение записи из таблицы
            var DealTable = UsAc.Execute($@"SELECT * FROM Дело WHERE Номер_дела = ""{NumOfDeal}""");
            string DateStart = DealTable.Table.Rows[0]["Дата_открытия"].ToString().Substring(0, 10);
            string DateEnd = DealTable.Table.Rows[0]["Дата_закрытия"].ToString().Substring(0, 10);
            string DateStorage = DealTable.Table.Rows[0]["Дата_введения_на_хранение"].ToString().Substring(0, 10);
            string Zaveritel = DealTable.Table.Rows[0]["Заверитель"].ToString();

            //Получение списка документов
            var DocumentsInDealTable = UsAc.Execute($@"SELECT * FROM Документ Where Номер_дела = ""{NumOfDeal}""");
            string DocCount = DocumentsInDealTable.Count.ToString() + "      ";

            //Число страниц для документа
            int DocNum = 1;

            var wordApp = new Microsoft.Office.Interop.Word.Application();
            wordApp.Visible = false;
            object missing = System.Reflection.Missing.Value;
            var wordDocument = wordApp.Documents.Open(Environment.CurrentDirectory + "/model/register.docx");
            try
            {
                ReplaceWordStub("{Num}", NumOfDeal, wordDocument);
                ReplaceWordStub("{DocCount}", DocCount, wordDocument);
                ReplaceWordStub("{DateStart}", DateStart, wordDocument);
                ReplaceWordStub("{DateEnd}", DateEnd, wordDocument);
                ReplaceWordStub("{DateStorage}", DateStorage, wordDocument);
                ReplaceWordStub("{Zav}", Zaveritel, wordDocument);

                Microsoft.Office.Interop.Word.Table wordTable = wordDocument.Tables[1];
                for (int x = 2; x < DocumentsInDealTable.Table.Rows.Count + 2; x++)
                {
                    //Добавляем строку таблицы
                    object oMissing = System.Reflection.Missing.Value;
                    wordTable.Rows.Add(ref oMissing);

                    //# п/п
                    wordTable.Cell(x, 1).Range.Text = (x - 1).ToString();
                    wordTable.Cell(x, 1).Range.Bold = 0;
                    wordTable.Cell(x, 1).Range.Cells.SetHeight(1, WdRowHeightRule.wdRowHeightAuto);

                    //Наименование
                    wordTable.Cell(x, 2).Range.Text = DocumentsInDealTable.Table.Rows[x - 2]["Название_документа"].ToString();
                    wordTable.Cell(x, 2).Range.Bold = 0;

                    //Номера листов
                    if (DocumentsInDealTable.Table.Rows[x - 2]["Число_страниц"].ToString() == "1")
                    {
                        wordTable.Cell(x, 3).Range.Text = (DocNum++).ToString();

                    }
                    else if (DocumentsInDealTable.Table.Rows[x - 2]["Число_страниц"].ToString() == "0")
                    {
                        wordTable.Cell(x, 3).Range.Text = DocNum.ToString();
                    }
                    else
                    {
                        string num = (DocNum + 1).ToString() + "-";
                        var count = Convert.ToInt32(DocumentsInDealTable.Table.Rows[x - 2]["Число_страниц"]) - 1;

                        DocNum += count;
                        num += DocNum.ToString();

                        wordTable.Cell(x, 3).Range.Text = num;
                    }
                    wordTable.Cell(x, 3).Range.Bold = 0;
                    wordTable.Cell(x, 3).Range.Cells.SetHeight(1, WdRowHeightRule.wdRowHeightAuto);
                }
            }
            catch (Exception ex)
            {
                object doNotSaveChanges = Microsoft.Office.Interop.Word.WdSaveOptions.wdDoNotSaveChanges;
                wordDocument.Close(ref doNotSaveChanges, ref missing, ref missing);
                throw ex;
            }

            wordApp.Visible = true;
        }

        private void ReplaceWordStub(string stubToReplace, string text, Microsoft.Office.Interop.Word.Document wordDocument)
        {
            var range = wordDocument.Content;
            range.Find.ClearFormatting();
            range.Find.Execute(FindText: stubToReplace, ReplaceWith: text);
        }
    }
}

