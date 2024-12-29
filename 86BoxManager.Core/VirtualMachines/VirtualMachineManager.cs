using System;
using System.Collections.Generic;
using System.IO;

using EightySixBoxManager.Core.VirtualMachines.List;

using FluentResults;

namespace EightySixBoxManager.Core.VirtualMachines;

public class VirtualMachineManager(IVirtualMachineListingProvider virtualMachineStorageProvider) : IVirtualMachineManager
{
	private readonly IVirtualMachineListingProvider _virtualMachineListingProvider = virtualMachineStorageProvider;

	public Result<IReadOnlyCollection<VirtualMachineInfo>> ListVirtualMachines()
	{
		return _virtualMachineListingProvider.GetVirtualMachines();
	}

	public Result<VirtualMachineInfo> CreateVirtualMachine(string name, string description)
	{
		VirtualMachineInfo virtualMachineInfo = new()
		{
			Name = name,
			Description = description,
			Path = _virtualMachineListingProvider.ComputePath(name)
		};

		Result addListingResult = _virtualMachineListingProvider.AddVirtualMachine(virtualMachineInfo);

		if (addListingResult.IsFailed)
		{
			return addListingResult;
		}

		try
		{
			Directory.CreateDirectory(virtualMachineInfo.Path);
		}
		catch (Exception ex)
		{
			return new ExceptionalError(ex);
		}

		return virtualMachineInfo;
	}

	public Result EditVirtualMachine()
	{
		throw new System.NotImplementedException();
	}

	public Result DeleteVirtualMachine(VirtualMachineInfo virtualMachineInfo, bool deleteFiles)
	{
		if (virtualMachineInfo.Status is not VirtualMachineStatus.Stopped)
		{
			return Result.Fail("VM is not stopped");
		}

		Result removeResult = _virtualMachineListingProvider.RemoveVirtualMachine(virtualMachineInfo);

		if (removeResult.IsFailed)
		{
			return Result.Fail("Failed to remove vm from listing");
		}

		if (deleteFiles is false)
		{
			return Result.Ok();
		}

		try
		{
			Directory.Delete(virtualMachineInfo.Path, true);

			return Result.Ok();
		}
		catch (Exception ex)
		{
			return new ExceptionalError(ex);
		}
	}

	public Result<VirtualMachineInfo> ImportVirtualMachine(string sourcePath, string name, string description)
	{
		VirtualMachineInfo virtualMachineInfo = new()
		{
			Name = name,
			Description = description,
			Path = _virtualMachineListingProvider.ComputePath(name)
		};

		Result addListingResult = _virtualMachineListingProvider.AddVirtualMachine(virtualMachineInfo);

		if (addListingResult.IsFailed)
		{
			return addListingResult;
		}

		//Copy existing files to the new VM directory
		try
		{
			Directory.CreateDirectory(virtualMachineInfo.Path);

			foreach (string oldPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
			{
				Directory.CreateDirectory(oldPath.Replace(sourcePath, virtualMachineInfo.Path));
			}
			foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
			{
				File.Copy(newPath, newPath.Replace(sourcePath, virtualMachineInfo.Path), true);
			}
		}
		catch (Exception ex)
		{
			// Revert ?
			return new ExceptionalError(ex);
		}

		return virtualMachineInfo;
	}

	public Result StartVirtualMachine()
	{
		throw new System.NotImplementedException();
	}

	public Result StopVirtualMachine()
	{
		throw new System.NotImplementedException();
	}

	public Result ForceStopVirtualMachine()
	{
		throw new System.NotImplementedException();
	}

	public Result PauseVirtualMachine()
	{
		throw new System.NotImplementedException();
	}

	public Result ResumeVirtualMachine()
	{
		throw new System.NotImplementedException();
	}

	public Result ClearCmos(VirtualMachineInfo virtualMachineInfo)
	{
		try
		{
			Directory.Delete(Path.Combine(virtualMachineInfo.Path, "nvr"), true);
			return Result.Ok();
		}
		catch (Exception ex)
		{
			return new ExceptionalError(ex);
		}
	}
}
