/**
 * for using "Microsoft.Office.Interop.Excel;" you need to add referance
 * Microsoft Excel 15.0 Object Library
 */
using Microsoft.Office.Interop.Excel;
using System.Linq;
using System.Runtime.InteropServices;
using BasicCRM.Models;

namespace BasicCRM.Common
{
    public static class DbHelper
    {
        //public static string FilePath = @"C:\Users\hwhov\Desktop\a.xlsx";

        public static void DbImportFromExcel(int testId, string PathFile)
        {
            string[,] content = ContentParsing(PathFile);
            Question question;
            using (BasicCRMEntities db = new BasicCRMEntities())
            {
                decimal AnswerPoint;
                for (int i = 1; i < content.GetLength(0); i+=5)
                {
                    if (string.IsNullOrEmpty(content[i, 1]))
                        continue;

                    question = new Question() { QuestionText = content[i, 1], QuestionDifficultyLevel = 0, TestID = testId };

                    db.Questions.Add(question);
                    db.SaveChanges();
                    for (int l = 1; l < 4 && l < content.GetLength(1); l += 2)
                        for (int k = 0, j = i + 1; k < 4 && j < content.GetLength(0); j++, k++)
                            if (!string.IsNullOrEmpty(content[j, l]))
                                try
                                {
                                    db.Answers.Add(new Answer()
                                    {
                                        AnswerPoint = decimal.TryParse(content[j, l + 1].Replace(".", ","), out AnswerPoint) ? AnswerPoint : 0m,
                                        AnswerText = content[j, l],
                                        QuestionID = question.QuestionID
                                    });
                                }
                                catch
                                { }
                }

                db.SaveChanges();
            }
        }

        private static object[,] ExcelImport(string FilePath)
        {

            Application xlApp;
            Workbook xlWorkBook;
            Worksheet xlWorkSheet;
            Range range;
            //Dictionary<string, string> Dic_Obj = new Dictionary<string, string>();

            //string str;
            int rCnt;
            int cCnt;
            int rw = 0;
            int cl = 0;

            xlApp = new Application();
            xlWorkBook = xlApp.Workbooks.Open(FilePath, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.get_Item(1);

            range = xlWorkSheet.UsedRange;
            rw = range.Rows.Count;
            cl = range.Columns.Count;

            // Console.OutputEncoding = new UTF8Encoding();

            object[,] Dic_Obj = new object[rw + 1, cl + 1];


            for (rCnt = 1; rCnt <= rw; rCnt++)
            {
                for (cCnt = 1; cCnt <= cl; cCnt++)
                {
                    //  object str1 = (range.Cells[rCnt, cCnt] as Range).Value2;
                    Dic_Obj[rCnt, cCnt] = (range.Cells[rCnt, cCnt] as Range).Value2;
                    //     Console.WriteLine(str1 ?? "");
                    //   MessageBox.Show(str);
                }
            }

            xlWorkBook.Close(true, null, null);
            xlApp.Quit();

            Marshal.ReleaseComObject(xlWorkSheet);
            Marshal.ReleaseComObject(xlWorkBook);
            Marshal.ReleaseComObject(xlApp);

            return Dic_Obj;
        }

        private static string[,] ContentParsing(string FilePath)
        {
            object[,] a = ExcelImport(FilePath);
            string[,] b = new string[a.GetLength(0), a.GetLength(1)];

            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < a.GetLength(1); j++)
                {
                    // Console.WriteLine(i + " - " + j + " --- " + (a[i, j] ?? ""));
                    b[i, j] = (a[i, j] ?? "").ToString().Trim();
                }
            }
            return b;
        }

        public static string DbNameValidator(string name)
        {
            string str = "";

            name = name.Replace("#", "Sharp").Replace("++", "Plus");


            foreach (var item in name)
                if (item >= 'a' && item <= 'z' || item >= 'A' && item <= 'Z' || item >= '0' && item <= '9' || item == ' ' || item == '_')
                    str += item.ToString();

            return str;
        }
    }
}