using System.Collections.Generic;

using FluentResults;

namespace EightySixBoxManager.Core.VirtualMachines.List;

public interface IVirtualMachineListingProvider
{
	Result<IReadOnlyCollection<VirtualMachineInfo>> GetVirtualMachines();

	Result AddVirtualMachine(VirtualMachineInfo virtualMachineInfo);

	string ComputePath(string vmName);

	Result RemoveVirtualMachine(VirtualMachineInfo virtualMachineInfo);

	Result<bool> IsNameInUse(string name);
}
