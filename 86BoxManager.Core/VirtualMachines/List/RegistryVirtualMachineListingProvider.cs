using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using EightySixBoxManager.Core.Classic;
using EightySixBoxManager.Core.Settings;

using FluentResults;

using Microsoft.Win32;

namespace EightySixBoxManager.Core.VirtualMachines.List;

public class RegistryVirtualMachineListingProvider(ISettingsProvider settingsProvider) : IVirtualMachineListingProvider
{
	private const string VM_KEY = @"SOFTWARE\86Box\Virtual Machines";

	private readonly ISettingsProvider _settingsProvider = settingsProvider;

	public Result<IReadOnlyCollection<VirtualMachineInfo>> GetVirtualMachines()
	{
		// Disable warning for obsolete method as it is needed for compatibility purposes.
#pragma warning disable SYSLIB0011 // Type or member is obsolete
		try
		{
			RegistryKey? regKey = Registry.CurrentUser.CreateSubKey(VM_KEY);

			if (regKey == null)
			{
				return Result.Fail($"Can't open or create registry key {VM_KEY}");
			}

			List<VirtualMachineInfo> virtualMachineInfo = [];

			foreach (string value in regKey.GetValueNames())
			{
				using MemoryStream ms = new MemoryStream((byte[])regKey.GetValue(value)!);

				SerializationBinder binder = new ClassicSerializationBinder();

				BinaryFormatter bf = new BinaryFormatter
				{
					Binder = binder
				};
				VM vm = (VM)bf.Deserialize(ms);

				virtualMachineInfo.Add(new VirtualMachineInfo
				{
					Name = vm.Name,
					Description = vm.Desc,
					Path = Path.Combine(_settingsProvider.SettingsValues.VmPath, vm.Name),
					Status = VirtualMachineStatus.Stopped
				});
			}

			return virtualMachineInfo;
		}
		catch (Exception ex)
		{
			return new ExceptionalError(ex);
		}
#pragma warning restore SYSLIB0011 // Type or member is obsolete
	}

	public Result AddVirtualMachine(VirtualMachineInfo virtualMachineInfo)
	{
		// Disable warning for obsolete method as it is needed for compatibility purposes.
#pragma warning disable SYSLIB0011 // Type or member is obsolete
		using MemoryStream ms = new MemoryStream();

		SerializationBinder binder = new ClassicSerializationBinder();

		BinaryFormatter bf = new BinaryFormatter
		{
			Binder = binder
		};

		// Create classic VM object
		VM classicVM = new VM(virtualMachineInfo.Name, virtualMachineInfo.Description, virtualMachineInfo.Path);

		try
		{
			bf.Serialize(ms, classicVM);
			byte[] data = ms.ToArray();

			RegistryKey? registryKey = Registry.CurrentUser.OpenSubKey(VM_KEY, writable: true);

			if (registryKey is null)
			{
				return Result.Fail($"Can't open or create registry key {VM_KEY}");
			}

			registryKey.SetValue(virtualMachineInfo.Name, data, RegistryValueKind.Binary);

			return Result.Ok();
		}
		catch (Exception ex)
		{
			return new ExceptionalError(ex);
		}
#pragma warning restore SYSLIB0011 // Type or member is obsolete
	}

	public string ComputePath(string vmName)
	{
		return Path.Combine(_settingsProvider.SettingsValues.VmPath, vmName);
	}
}
