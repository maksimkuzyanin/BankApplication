using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;

namespace Bank.App.Helpers
{
	/// <summary>
	///     Хелпер сортировки данных в listview по клику на столбце
	/// </summary>
	public class ListViewColumnSorter
    {
        private readonly ListView _listView;
        private ListSortDirection _lastDirection = ListSortDirection.Ascending;

        private GridViewColumnHeader _lastHeaderClicked;

        public ListViewColumnSorter(ListView listView)
        {
            _listView = listView;
        }

        public void SortByColumn(GridViewColumnHeader headerClicked)
        {
            ListSortDirection direction;

            if (headerClicked != null)
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (headerClicked != _lastHeaderClicked)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (_lastDirection == ListSortDirection.Ascending)
                            direction = ListSortDirection.Descending;
                        else
                            direction = ListSortDirection.Ascending;
                    }

                    var columnBinding = headerClicked.Column.DisplayMemberBinding as Binding;
                    var sortBy = columnBinding?.Path.Path ?? headerClicked.Column.Header as string;

                    Sort(sortBy, direction);
                    DrawSortArrow(headerClicked, direction);

                    _lastHeaderClicked = headerClicked;
                    _lastDirection = direction;
                }
        }

        private void Sort(string sortBy, ListSortDirection direction)
        {
            var dataView = CollectionViewSource.GetDefaultView(_listView.ItemsSource);

            dataView.SortDescriptions.Clear();
            var sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }

        /// <summary>
        ///     https://stackoverflow.com/questions/73320/how-to-set-the-header-sort-glyph-in-a-net-listview
        /// </summary>
        /// <param name="headerClicked"></param>
        /// <param name="sortOrder"></param>
        public void DrawSortArrow(GridViewColumnHeader headerClicked, ListSortDirection sortOrder)
        {
            var upArrow = "▲   ";
            var downArrow = "▼   ";

            var viewColumns = ((GridView) _listView.View).Columns;

            foreach (var column in viewColumns)
            {
                var columnHeader = column.Header as string;

                if (string.IsNullOrEmpty(columnHeader)) continue;
                if (columnHeader.Contains(upArrow))
                    column.Header = columnHeader.Replace(upArrow, string.Empty);
                else if (columnHeader.Contains(downArrow))
                    column.Header = columnHeader.Replace(downArrow, string.Empty);
            }

            var colIndex = viewColumns.IndexOf(headerClicked.Column);
            var sortedColumn = viewColumns[colIndex];
            var sortedColumnHeader = sortedColumn.Header as string;

            if (sortOrder == ListSortDirection.Ascending)
                sortedColumn.Header = sortedColumnHeader.Insert(0, downArrow);
            else
                sortedColumn.Header = sortedColumnHeader.Insert(0, upArrow);
        }
    }
}