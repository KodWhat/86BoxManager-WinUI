using System;

using EightySixBoxManager.Core.Enums;

namespace EightySixBoxManager.Core.Settings;

public record SettingsValues
{
	/// <summary>
	/// Path to the VM storage folder.
	/// </summary>
	public string VmPath { get; init; } = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\86Box VMs\";

	/// <summary>
	/// Path to the folder containing the 86box executable.
	/// </summary>
	public string BoxExePath { get; init; } = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\86Box\";

	/// <summary>
	/// Minimize the main window when a VM is started?
	/// </summary>
	public bool MinimizeOnVMStart { get; init; } = false;

	/// <summary>
	/// Show the console window when a VM is started?
	/// </summary>
	public bool ShowConsole { get; init; } = true;

	/// <summary>
	/// Minimize the Manager window to the tray icon?
	/// </summary>
	public bool MinimizeToTray { get; init; } = false;

	/// <summary>
	/// Close the Manager window to the tray icon?
	/// </summary>
	public bool CloseToTray { get; init; } = false;

	/// <summary>
	/// Logging enabled for 86box.exe? Enables the -L parameter of 86box.
	/// </summary>
	public bool LoggingEnabled { get; init; } = false;

	/// <summary>
	/// Path to 86box.exe log file.
	/// </summary>
	public string LogPath { get; init; } = string.Empty;

	/// <summary>
	/// Grid lines visible in the VM list?
	/// </summary>
	public bool ShowGridLines { get; init; } = false;

	/// <summary>
	/// The column for sorting.
	/// </summary>
	public int SortColumn { get; init; } = 0;

	/// <summary>
	/// The sorting order of the VM list.
	/// </summary>
	public SortOrder SortOrder { get; init; } = SortOrder.Ascending;
}