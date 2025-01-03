﻿using System;

namespace EightySixBoxManager.Core.Classic;

[Serializable] //For serializing VMs so they can be stored in the registry
internal class VM
{
	public nint hWnd { get; set; } //Window handle for the VM once it's started
	public string Name { get; set; } //Name of the virtual machine
	public string Desc { get; set; } //Description
	public string Path { get; set; } //Path to config, nvr, etc.
	public int Status { get; set; } //Status
	public int Pid { get; set; } //Process ID of 86box.exe running the VM
	public const int STATUS_STOPPED = 0; //VM is not running
	public const int STATUS_RUNNING = 1; //VM is running
	public const int STATUS_WAITING = 2; //VM is waiting for user response
	public const int STATUS_PAUSED = 3; //VM is paused

	public VM()
	{
		Name = "defaultName";
		Desc = "defaultDesc";
		Path = "defaultPath";
		Status = STATUS_STOPPED;
		hWnd = nint.Zero;
	}

	public VM(string name, string desc, string path)
	{
		Name = name;
		Desc = desc;
		Path = path;
		Status = STATUS_STOPPED;
		hWnd = nint.Zero;
	}

	public override string ToString()
	{
		return "Name: " + Name + ", description: " + Desc + ", path: " + Path + ", status: " + Status;
	}

	//Returns a lovely status string for use in UI
	public string GetStatusString()
	{
		return Status switch
		{
			STATUS_STOPPED => "Stopped",
			STATUS_RUNNING => "Running",
			STATUS_PAUSED => "Paused",
			STATUS_WAITING => "Waiting",
			_ => "Invalid status",
		};
	}
}
