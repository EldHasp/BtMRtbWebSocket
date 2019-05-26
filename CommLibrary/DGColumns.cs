using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CommLibrary
{
    public class DGColumns : OnPropertyChangedClass
    {

        Dictionary<string, DGColumn> columns = new Dictionary<string, DGColumn>();
        public IDictionary<string, DGColumn> Columns => columns;
        private string[] _columnHeaders;
        private string[] _rowHeaders;
        private string[,] _cells;
        public List<DateTime> rowHeadersSource;

        public string[] ColumnHeaders { get => _columnHeaders; private set { _columnHeaders = value; OnPropertyChanged(); } }
        public string[] RowHeaders { get => _rowHeaders; private set { _rowHeaders = value; OnPropertyChanged(); } }
        public string[,] Cells { get => _cells; private set { _cells = value; OnPropertyChanged(); } }

        public void Clear()
        {
            columns = new Dictionary<string, DGColumn>();
            ToUpdate();
        }

        private void ToUpdate()
        {
            if (columns.Count == 0)
            {
                ColumnHeaders = new string[0];
                RowHeaders = new string[0];
                Cells = new string[0, 0];
                return;
            }
            List<DGColumn> colList = columns.Values.ToList();
            int rowCont = colList.Min(col => col.Items.Count() + (addValue[col.Header] ? 0 : addCountRow));
            int[] rowCountArr = colList.Select(s => s.Items.Count()).ToArray();
            int rowCountMin = rowCountArr.Min();
            int colCount = colList.Count;
            string[] headers = new string[colCount];
            string[,] cells = new string[rowCont, colCount];

            for (int col = 0; col < colList.Count; col++)
            {
                int addRow = addValue[colList[col].Header] ? 0 : addCountRow; // Конечное смещение для колонки

                headers[col] = colList[col].Header + " " + colList[col].Items.Count(); // заголовок колонки

                int itemsCount = colList[col].Items.Count(); /* длина колонки с данными */

                int rowOffset = itemsCount
                                - rowCont /* количество строк вывода */
                                + addRow; /* количество пустых строк в конце */

                //if (addValue.TryGetValue(headers[col], out bool keyAdd) && !keyAdd)
                //    rowOffset -= addCountRow;
                for (int row = 0; row < rowCont && rowOffset + row < itemsCount; row++)
                    cells[rowCont - row - 1, col] = colList[col].Items.ElementAt(rowOffset + row);
            }
            ColumnHeaders = headers;
            RowHeaders = rowHeadersSource.Skip(rowHeadersSource.Count - rowCont).Select(x => x.ToString("dd.MM HH:mm")).Reverse().ToArray();
            Cells = cells;

        }

        int addCountRow = 0;
        Dictionary<string, bool> addValue;
        public void AddRowHeaders(IEnumerable<DateTime> Items)
        {
            if (rowHeadersSource == null || rowHeadersSource.Count == 0)
            {
                columns = new Dictionary<string, DGColumn>();
                addValue = new Dictionary<string, bool>();
                addCountRow = Items.Count();
                rowHeadersSource = Items.ToList();
            }
            else
            {
                TimeSpan stepTime = rowHeadersSource[1] - rowHeadersSource[0];
                DateTime rowHeaderLast = rowHeadersSource.Last();
                if (rowHeaderLast + stepTime < Items.First())
                {
                    columns = new Dictionary<string, DGColumn>();
                    addValue = new Dictionary<string, bool>();
                    addCountRow = Items.Count();
                    rowHeadersSource = Items.ToList();
                }
                else
                {
                    addCountRow = Items.Count(x => x > rowHeaderLast);
                    rowHeadersSource = rowHeadersSource.Concat(Items.Skip(Items.Count() - addCountRow)).ToList();
                    List<string> keyAdd = addValue.Keys.ToList();
                    addValue = new Dictionary<string, bool>();
                    keyAdd.ForEach(k => addValue.Add(k, false));
                }
            }
            ToUpdate();
        }

        public void Add(DGColumn column)
        {
            if (addValue.TryGetValue(column.Header, out bool keyAdd) && !keyAdd)
            {
                var items = columns[column.Header].Items.Concat(column.Items.Skip(column.Items.Count() - addCountRow));
                columns.Remove(column.Header);
                columns.Add(column.Header, new DGColumn(column.Header, items));
                addValue.Remove(column.Header);
            }
            else columns.Add(column.Header, column);
            addValue.Add(column.Header, true);

            ToUpdate();
        }
        public void Add<T>(string Header, IEnumerable<T> Items)
        {
            DGColumn column = DGColumn.Create(Header, Items);
            Add(column);
        }
        public void Add(string Header, IEnumerable<double> Items)
        {
            DGColumn column = DGColumn.Create(Header, Items);
            Add(column);
        }
        public void Add(string Header, IEnumerable<double?> Items)
        {
            DGColumn column = DGColumn.Create(Header, Items);
            Add(column);
        }
        public void Add(string Header, IEnumerable<bool> Items)
        {
            DGColumn column = DGColumn.Create(Header, Items);
            Add(column);
        }
        public bool Remove(string Header)
        {
            if (!columns.ContainsKey(Header))
                return false;
            columns.Remove(Header);
            ToUpdate();
            return true;
        }
    }
}
