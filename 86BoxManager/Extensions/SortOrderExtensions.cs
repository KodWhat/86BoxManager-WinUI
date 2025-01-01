namespace Fluent86.Extensions;

internal static class SortOrderExtensions
{
	internal static System.Windows.Forms.SortOrder ToWinFormsSortOrder(this Core.Enums.SortOrder order)
	{
		return order switch
		{
			Core.Enums.SortOrder.None => System.Windows.Forms.SortOrder.None,
			Core.Enums.SortOrder.Ascending => System.Windows.Forms.SortOrder.Ascending,
			Core.Enums.SortOrder.Descending => System.Windows.Forms.SortOrder.Descending,
			_ => System.Windows.Forms.SortOrder.None
		};
	}

	internal static Core.Enums.SortOrder ToCoreSortOrder(this System.Windows.Forms.SortOrder order)
	{
		return order switch
		{
			System.Windows.Forms.SortOrder.None => Core.Enums.SortOrder.None,
			System.Windows.Forms.SortOrder.Ascending => Core.Enums.SortOrder.Ascending,
			System.Windows.Forms.SortOrder.Descending => Core.Enums.SortOrder.Descending,
			_ => Core.Enums.SortOrder.None
		};
	}
}
