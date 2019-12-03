using System;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;

namespace Lab2.Model
{
    public class ThreatTableModel
    {
        public const int entriesPerPage = 15;
        public const int fieldsCount = 8;
        public const int shortFieldsCount = 2;
        private string[] header = new string[] { "Идентификатор угрозы", "Наименование угрозы", "Описание угрозы", "Источник угрозы", "Объект взаимодействия угрозы", "Нарушение конфиденциальности", "Нарушение целостности", "Нарушение доступности" };
        private Dictionary<int, string[]> table;
        private List<string> messages;
        public string Path { get; set; }
        public List<string> Messages { get { return messages; } }
        public int PageCount
        {
            get
            {
                return (table.Count % entriesPerPage > 0 ? table.Count / entriesPerPage + 1 : table.Count / entriesPerPage);
            }
        }
        private int pagePointer;
        public int PagePointer 
        {
            get
            {
                return pagePointer;
            }
            set
            {
                if (PageCount < value)
                    pagePointer = PageCount;
                else 
                    if (value < 1)
                        pagePointer = 1;
                    else pagePointer = value;
            }
        }
        public ThreatTableModel()
        {
            table = new Dictionary<int, string[]>();
            messages = new List<string>();
            pagePointer = 1;
        }
        public static int ParseId(string id)
        {
            int index = -1;
            try
            {
                if (id.StartsWith("УБИ.")) index = Convert.ToInt32(id.Substring(4));
                else index = Convert.ToInt32(id);
            }
            catch (FormatException e1) { }
            return index;
        }
        public void FillTableFromFile(string path)
        {
            messages.Clear();
            table.Clear();
            Path = path;
            FileInfo file = new FileInfo(Path);
            using (ExcelPackage package = new ExcelPackage(file))
            {
                int id;
                object[,] entries = (object[,]) package.Workbook.Worksheets[1].Cells.Value;
                for (int k = 0; k < entries.GetLength(0); k++)
                {
                    try 
                    {
                        id = Convert.ToInt32(entries[k, 0]);
                        if (id > 0) table.Add(id, GetEntryArray(entries, k));
                    }
                    catch (FormatException ex) { }
                }
            }
        }
        public string[][] GetPageTable()
        {
            int index = (PagePointer - 1) * entriesPerPage;
            int size = table.Count - index < entriesPerPage ? table.Count - index : entriesPerPage;
            string[][] page = new string[size + 1][];
            int j = 0; int i = 1;
            page[0] = header;
            foreach (var entry in table.Values)
            {
                if (j < index) { j++; continue; }
                if (i > size) break;
                page[i] = new string[shortFieldsCount];
                Array.Copy(entry, 0, page[i], 0, shortFieldsCount);
                page[i][0] = "УБИ." + page[i][0];
                i++; j++;
            }
            return page;
        }
        public string[][] GetEntryTable(int id)
        {
            string[][] page = new string[2][];
            page[0] = header;
            table.TryGetValue(id, out page[1]);
            page[1][5] = page[1][5] == "0" ? "нет" : "да";
            page[1][6] = page[1][6] == "0" ? "нет" : "да";
            page[1][7] = page[1][7] == "0" ? "нет" : "да";
            return page;
        }
        public void DownloadTable()
        {
            using (var client = new System.Net.WebClient())
            {
                string url = "https://bdu.fstec.ru/documents/files/thrlist.xlsx";
                if (File.Exists(Path)) 
                {
                    File.Delete(Path);
                }
                client.DownloadFile(url, Path);
            }
        }
        public int UpdateTable()
        {
            messages.Clear();
            DownloadTable();
            FileInfo file = new FileInfo(Path);
            using (ExcelPackage package = new ExcelPackage(file))
            {
                Dictionary<int, string[]> newTable = new Dictionary<int, string[]>();
                int id; int count = 0;
                object[,] entries = (object[,]) package.Workbook.Worksheets[1].Cells.Value;
                for (int k = 0; k < entries.GetLength(0); k++)
                {
                    try
                    {
                        id = Convert.ToInt32(entries[k, 0]);
                        if (id > 0)
                        {
                            if (!table.ContainsKey(id))
                            {
                                newTable.Add(id, GetEntryArray(entries, k));
                                messages.Add("New threat is added: УБИ." + id);
                                count++;
                            }
                            else
                            {
                                table.TryGetValue(id, out string[] entry);
                                string[] newEntry = GetEntryArray(entries, k);
                                if (!Compare(entry, ref newEntry))
                                {
                                    newTable.Add(id, newEntry);
                                    table.Remove(id);
                                    count++;
                                }
                                else
                                {
                                    newTable.Add(id, table[id]);
                                    table.Remove(id);
                                }
                            }
                        }
                    }
                    catch (FormatException ex) { }
                }
                messages.Add("");
                foreach (var entry in table)
                {
                    messages.Add("This thread was removed: УБИ." + entry.Key);
                    count++;
                }
                table = newTable;
                return count;
            }
        }
        public void SaveTableToFile(string path)
        {
            using (var package = new ExcelPackage())
            {
                ExcelWorksheet sheet = package.Workbook.Worksheets.Add("Thrlist");
                int i = 1;
                foreach (string cell in header)
                {
                    sheet.Cells[1, i].Value = cell;
                    i++;
                }
                i = 2;
                foreach (var entry in table)
                {
                    for (int j = 0; j < header.Length; j++)
                    {
                        if (j >= 5)
                        {
                            sheet.Cells[i, j + 1].Value = entry.Value[j] == "да" ? "1" : "0";
                            continue;
                        }
                        sheet.Cells[i, j + 1].Value = entry.Value[j];
                    }
                    i++;
                }
                package.SaveAs(new FileInfo(path));
            }
        }
        private bool Compare(string[] oldEntry, ref string[] newEntry)
        {
            if (oldEntry.Length != newEntry.Length) return false;
            bool isEqual = true;
            for (int i = 0; i < oldEntry.Length; i++)
            {
                if ((oldEntry[i].Length != newEntry[i].Length) || (!oldEntry[i].Equals(newEntry[i])))
                {
                    messages.Add("\nУБИ." + oldEntry[0] + ":\nWas:\n\t" + oldEntry[i] + "\nChanged to:\n\t" + newEntry[i]);
                    isEqual = false;
                }
            }
            return isEqual;
        }
        private string[] GetEntryArray(object[,] entries, int row)
        {
            string[] entry = new string[header.Length];
            for (int j = 0; j < header.Length; j++)
            {
                try { entry[j] = entries[row, j].ToString(); } catch (NullReferenceException ex) { entry[j] = ""; }
                entry[j] = entry[j].Replace("_x000d_", "");
                entry[j] = entry[j].Replace("\r", "");
            }
            entry[5] = entry[5] == "1" ? "да" : "нет";
            entry[6] = entry[6] == "1" ? "да" : "нет";
            entry[7] = entry[7] == "1" ? "да" : "нет";
            return entry;
        }
    }
}
