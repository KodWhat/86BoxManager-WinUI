using System;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.Storage.Pickers;

using WinRT.Interop;

namespace EightySixBoxManager;

/// <summary>
/// Present the Windows Vista-style open file dialog to select a folder. Fall back for older Windows Versions
/// </summary>
public class FolderSelectDialog
{
	public string InitialDirectory { get; set; } = Environment.CurrentDirectory;

	public string Title { get; set; } = "Select a folder";

	public string FileName { get; private set; } = "";

	public async Task<bool> Show(IntPtr hWndOwner)
	{
		FolderPicker folderPicker = new();
		folderPicker.FileTypeFilter.Add("*");
		InitializeWithWindow.Initialize(folderPicker, hWndOwner);
		StorageFolder folder = await folderPicker.PickSingleFolderAsync();

		if (folder == null)
		{
			return false;
		}

		FileName = folder.Path;
		return true;
	}
}