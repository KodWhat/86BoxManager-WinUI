using System.Collections;
using System.Windows.Forms;

namespace EightySixBoxManager;

class ListViewItemComparer : IComparer
{
	private int col;
	private SortOrder order;

	public ListViewItemComparer()
	{
		col = 0;
		order = SortOrder.Ascending;
	}

	public ListViewItemComparer(int column, SortOrder order)
	{
		col = column;
		this.order = order;
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
			returnVal = string.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);
		}

		if (order == SortOrder.Descending)
		{
			returnVal *= -1;
		}

		return returnVal;
	}
}
