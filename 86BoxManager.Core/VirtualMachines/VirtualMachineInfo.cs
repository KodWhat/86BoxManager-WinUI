using System;

namespace EightySixBoxManager.Core.VirtualMachines;
public record VirtualMachineInfo
{
	public required string Name { get; set; }

	public string Description { get; set; } = string.Empty;

	public required string Path { get; set; }

	public VirtualMachineStatus Status { get; set; } = VirtualMachineStatus.Stopped;

	public int RunningProcessId { get; set; } = 0;

	public IntPtr RunningWindowHandle { get; set; } = IntPtr.Zero;
}
