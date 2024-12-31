using System.Collections;
using System.Windows.Forms;

namespace EightySixBoxManager;

class ListViewItemComparer : IComparer
{
	private readonly int _col;
	private readonly SortOrder _order;

	public ListViewItemComparer()
	{
		_col = 0;
		_order = SortOrder.Ascending;
	}

	public ListViewItemComparer(int column, SortOrder order)
	{
		_col = column;
		_order = order;
	}

	public int Compare(object? x, object? y)
	{
		int returnVal;

		if (x is null)
		{
			returnVal = -1;
		}
		else if (y is null)
		{
			returnVal = 1;
		}
		else
		{
			returnVal = string.Compare(((ListViewItem)x).SubItems[_col].Text, ((ListViewItem)y).SubItems[_col].Text);
		}

		if (_order == SortOrder.Descending)
		{
			returnVal *= -1;
		}

		return returnVal;
	}
}
