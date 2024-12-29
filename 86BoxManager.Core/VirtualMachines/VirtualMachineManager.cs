﻿using System;
using System.Collections.Generic;
using System.IO;

using EightySixBoxManager.Core.VirtualMachines.List;

using FluentResults;

namespace EightySixBoxManager.Core.VirtualMachines;

public class VirtualMachineManager(IVirtualMachineListingProvider virtualMachineStorageProvider) : IVirtualMachineManager
{
	private readonly IVirtualMachineListingProvider _virtualMachineStorageProvider = virtualMachineStorageProvider;

	public Result<IReadOnlyCollection<VirtualMachineInfo>> ListVirtualMachines()
	{
		return _virtualMachineStorageProvider.GetVirtualMachines();
	}

	public Result<VirtualMachineInfo> CreateVirtualMachine(string name, string description)
	{
		VirtualMachineInfo virtualMachineInfo = new()
		{
			Name = name,
			Description = description,
			Path = _virtualMachineStorageProvider.ComputePath(name)
		};

		Result addListingResult = _virtualMachineStorageProvider.AddVirtualMachine(virtualMachineInfo);

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

	public Result DeleteVirtualMachine()
	{
		throw new System.NotImplementedException();
	}

	public Result<VirtualMachineInfo> ImportVirtualMachine(string sourcePath, string name, string description)
	{
		VirtualMachineInfo virtualMachineInfo = new()
		{
			Name = name,
			Description = description,
			Path = _virtualMachineStorageProvider.ComputePath(name)
		};

		Result addListingResult = _virtualMachineStorageProvider.AddVirtualMachine(virtualMachineInfo);

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

	public Result CloneVirtualMachine()
	{
		throw new System.NotImplementedException();
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
}
