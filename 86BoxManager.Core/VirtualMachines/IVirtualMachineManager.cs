using System.Collections.Generic;

using FluentResults;

namespace EightySixBoxManager.Core.VirtualMachines;

public interface IVirtualMachineManager
{
	Result<IReadOnlyCollection<VirtualMachineInfo>> ListVirtualMachines();

	Result<VirtualMachineInfo> CreateVirtualMachine(string name, string description);

	Result EditVirtualMachine();

	Result DeleteVirtualMachine();

	Result<VirtualMachineInfo> ImportVirtualMachine(string sourcePath, string name, string description);

	Result CloneVirtualMachine();

	Result StartVirtualMachine();

	Result StopVirtualMachine();

	Result ForceStopVirtualMachine();

	Result PauseVirtualMachine();

	Result ResumeVirtualMachine();
}
