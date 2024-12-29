﻿using System.Collections.Generic;

using FluentResults;

namespace EightySixBoxManager.Core.VirtualMachines;

public interface IVirtualMachineManager
{
	Result<IReadOnlyCollection<VirtualMachineInfo>> ListVirtualMachines();

	Result<VirtualMachineInfo> CreateVirtualMachine(string name, string description, bool createDirectory = true);

	Result<VirtualMachineInfo> EditVirtualMachine(VirtualMachineInfo virtualMachineInfo, string newName, string newDescription);

	Result DeleteVirtualMachine(VirtualMachineInfo virtualMachineInfo, bool deleteFiles);

	Result<VirtualMachineInfo> ImportVirtualMachine(string sourcePath, string name, string description);

	Result StartVirtualMachine();

	Result StopVirtualMachine();

	Result ForceStopVirtualMachine();

	Result PauseVirtualMachine();

	Result ResumeVirtualMachine();

	Result ClearCmos(VirtualMachineInfo virtualMachineInfo);
}
