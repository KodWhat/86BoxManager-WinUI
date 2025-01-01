using System;

using EightySixBoxManager.Core.Enums;

using FluentResults;

using Microsoft.Win32;

namespace EightySixBoxManager.Core.Settings;

public class RegistrySettingsProvider : ISettingsProvider
{
	public SettingsValues SettingsValues { get; private set; } = new SettingsValues();

	private const string ROOT_KEY = @"SOFTWARE\86Box";

	public Result CreateDefaults()
	{
		RegistryKey? regkey = null;
		try
		{
			//Create the key and reopen it for write access
			Registry.CurrentUser.CreateSubKey(ROOT_KEY);
			regkey = Registry.CurrentUser.OpenSubKey(ROOT_KEY, true);

			if (regkey == null)
			{
				return Result.Fail($"Can't access created subkey {ROOT_KEY}");
			}

			SettingsValues defaults = new SettingsValues();

			SettingsValues = defaults;

			//Write defaults to the registry
			regkey.SetValue("EXEdir", defaults.BoxExePath, RegistryValueKind.String);
			regkey.SetValue("CFGdir", defaults.VmPath, RegistryValueKind.String);
			regkey.SetValue("MinimizeOnVMStart", defaults.MinimizeOnVMStart, RegistryValueKind.DWord);
			regkey.SetValue("ShowConsole", defaults.ShowConsole, RegistryValueKind.DWord);
			regkey.SetValue("MinimizeToTray", defaults.MinimizeToTray, RegistryValueKind.DWord);
			regkey.SetValue("CloseToTray", defaults.CloseToTray, RegistryValueKind.DWord);
			regkey.SetValue("EnableLogging", defaults.LoggingEnabled, RegistryValueKind.DWord);
			regkey.SetValue("LogPath", defaults.LogPath, RegistryValueKind.String);
			regkey.SetValue("EnableGridLines", defaults.ShowGridLines, RegistryValueKind.DWord);
			regkey.SetValue("SortColumn", defaults.SortColumn, RegistryValueKind.DWord);
			regkey.SetValue("SortOrder", defaults.SortOrder, RegistryValueKind.DWord);

			return Result.Ok();
		}
		catch (Exception ex)
		{
			return new ExceptionalError(ex);
		}
		finally
		{
			regkey?.Close();
		}
	}

	public Result LoadSettings()
	{
		RegistryKey? regkey = null;
		try
		{
			regkey = Registry.CurrentUser.OpenSubKey(ROOT_KEY, true);

			// If the registry key doesn't exist, populate with default values
			if (regkey == null)
			{
				return CreateDefaults();
			}
			else
			{
				SettingsValues = new SettingsValues
				{
					BoxExePath = regkey.GetValue("EXEdir")?.ToString() ?? string.Empty,
					VmPath = regkey.GetValue("CFGdir")?.ToString() ?? string.Empty,
					MinimizeOnVMStart = Convert.ToBoolean(regkey.GetValue("MinimizeOnVMStart")),
					ShowConsole = Convert.ToBoolean(regkey.GetValue("ShowConsole")),
					MinimizeToTray = Convert.ToBoolean(regkey.GetValue("MinimizeToTray")),
					CloseToTray = Convert.ToBoolean(regkey.GetValue("CloseToTray")),
					LogPath = regkey.GetValue("LogPath")?.ToString() ?? string.Empty,
					LoggingEnabled = Convert.ToBoolean(regkey.GetValue("EnableLogging")),
					ShowGridLines = Convert.ToBoolean(regkey.GetValue("EnableGridLines")),
					SortColumn = (int)(regkey.GetValue("SortColumn") ?? 0),
					SortOrder = (SortOrder)(regkey.GetValue("SortOrder") ?? 0)
				};
			}

			return Result.Ok();
		}
		catch (Exception ex)
		{
			return new ExceptionalError(ex);
		}
		finally
		{
			regkey?.Close();
		}
	}

	public Result SaveSettings(SettingsValues newSettings)
	{
		RegistryKey? regkey = null;
		try
		{
			regkey = Registry.CurrentUser.OpenSubKey(ROOT_KEY, true);

			if (regkey == null)
			{
				return Result.Fail($"Can't access created subkey {ROOT_KEY}");
			}

			regkey.SetValue("EXEdir", newSettings.BoxExePath, RegistryValueKind.String);
			regkey.SetValue("CFGdir", newSettings.VmPath, RegistryValueKind.String);
			regkey.SetValue("MinimizeOnVMStart", newSettings.MinimizeOnVMStart, RegistryValueKind.DWord);
			regkey.SetValue("ShowConsole", newSettings.ShowConsole, RegistryValueKind.DWord);
			regkey.SetValue("MinimizeToTray", newSettings.MinimizeToTray, RegistryValueKind.DWord);
			regkey.SetValue("CloseToTray", newSettings.CloseToTray, RegistryValueKind.DWord);
			regkey.SetValue("EnableLogging", newSettings.LoggingEnabled, RegistryValueKind.DWord);
			regkey.SetValue("LogPath", newSettings.LogPath, RegistryValueKind.String);
			regkey.SetValue("EnableGridLines", newSettings.ShowGridLines, RegistryValueKind.DWord);
			regkey.SetValue("SortColumn", newSettings.SortColumn, RegistryValueKind.DWord);
			regkey.SetValue("SortOrder", newSettings.SortOrder, RegistryValueKind.DWord);

			// Reload settings from registry;
			LoadSettings();

			return Result.Ok();
		}
		catch (Exception ex)
		{
			return new ExceptionalError(ex);
		}
		finally
		{
			regkey?.Close();
		}
	}

	public Result SaveSortSettings(int sortColumnIndex, SortOrder sortOrder)
	{
		SettingsValues newSettings = SettingsValues with { SortOrder = sortOrder, SortColumn = sortColumnIndex };

		return SaveSettings(newSettings);
	}
}
