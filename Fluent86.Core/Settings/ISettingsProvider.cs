using Fluent86.Core.Enums;

using FluentResults;

namespace Fluent86.Core.Settings;

public interface ISettingsProvider
{
	SettingsValues SettingsValues { get; }

	Result CreateDefaults();

	Result LoadSettings();

	Result SaveSettings(SettingsValues newSettings);

	Result SaveSortSettings(int sortColumnIndex, SortOrder sortOrder);
}
