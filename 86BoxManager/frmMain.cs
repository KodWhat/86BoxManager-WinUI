using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;

using EightySixBoxManager.Core.Settings;
using EightySixBoxManager.Core.VirtualMachines;
using EightySixBoxManager.Extensions;
using EightySixBoxManager.Properties;

using FluentResults;

using IWshRuntimeLibrary;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;

namespace EightySixBoxManager;

public partial class frmMain : Form
{
	//Win32 API imports
	//Posts a message to the window with specified handle - DOES NOT WAIT FOR THE RECIPIENT TO PROCESS THE MESSAGE!!!
	[DllImport("user32.dll")]
	public static extern int PostMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

	//Focus a window
	[DllImport("user32.dll")]
	public static extern int SetForegroundWindow(IntPtr hwnd);

	[StructLayout(LayoutKind.Sequential)]
	public struct COPYDATASTRUCT
	{
		public IntPtr dwData;
		public int cbData;
		public IntPtr lpData;
	}
	private string hWndHex = "";  //Window handle of this window  

	private readonly ISettingsProvider _settingsProvider;
	private readonly IVirtualMachineManager _virtualMachineManager;
	private readonly IServiceProvider _serviceProvider;

	public frmMain(ISettingsProvider settingsProvider, IVirtualMachineManager virtualMachineManager, IServiceProvider serviceProvider)
	{
		_settingsProvider = settingsProvider;
		_virtualMachineManager = virtualMachineManager;
		_serviceProvider = serviceProvider;

		InitializeComponent();
	}

	private void frmMain_Load(object sender, EventArgs e)
	{
		LoadSettings();
		LoadVMs();

		//Load main window's state, size and position
		//BUGBUG: Restoring these causes anchor problems, so we're not doing it anymore...
		/*WindowState = Settings.Default.WindowState;
		Size = Settings.Default.WindowSize;
		Location = Settings.Default.WindowPosition;*/

		//Load listview column widths
		clmName.Width = Settings.Default.NameColWidth;
		clmStatus.Width = Settings.Default.StatusColWidth;
		clmDesc.Width = Settings.Default.DescColWidth;
		clmPath.Width = Settings.Default.PathColWidth;

		//Convert the current window handle to a form that's expected by 86Box
		hWndHex = string.Format("{0:X}", Handle.ToInt64());
		hWndHex = hWndHex.PadLeft(16, '0');

		//Check if command line arguments for starting a VM are OK
		if (Program.args.Length == 3 && Program.args[1] == "-S" && Program.args[2] != null)
		{
			//Find the VM with given name
			ListViewItem? lvi = lstVMs.FindItemWithText(Program.args[2], false, 0, false);

			//Then select and start it if it's found
			if (lvi != null)
			{
				lvi.Focused = true;
				lvi.Selected = true;
				VMStart();
			}
			else
			{
				MessageBox.Show("The virtual machine \"" + Program.args[2] + "\" could not be found. It may have been removed or the specified name is incorrect.", "Virtual machine not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}

	private void btnStart_Click(object sender, EventArgs e)
	{
		if (lstVMs.SelectedItems[0].Tag is not VirtualMachineInfo vm)
		{
			return;
		}

		if (vm.Status is VirtualMachineStatus.Stopped)
		{
			VMStart();
		}
		else if (vm.Status is VirtualMachineStatus.Running or VirtualMachineStatus.Paused)
		{
			VMRequestStop();
		}
	}

	private void btnConfigure_Click(object sender, EventArgs e)
	{
		VMConfigure();
	}

	private void btnSettings_Click(object sender, EventArgs e)
	{
		dlgSettings dlg = _serviceProvider.GetRequiredService<dlgSettings>();
		dlg.ShowDialog();
		LoadSettings(); //Reload the settings due to potential changes    
		dlg.Dispose();
	}

	private void lstVMs_SelectedIndexChanged(object sender, EventArgs e)
	{
		//Disable relevant buttons if no VM is selected
		if (lstVMs.SelectedItems.Count == 0)
		{
			btnConfigure.Enabled = false;
			btnStart.Enabled = false;
			btnEdit.Enabled = false;
			btnDelete.Enabled = false;
			btnReset.Enabled = false;
			btnCtrlAltDel.Enabled = false;
			btnPause.Enabled = false;
		}
		else if (lstVMs.SelectedItems.Count == 1)
		{
			//Disable relevant buttons if VM is running
			if (lstVMs.SelectedItems[0].Tag is not VirtualMachineInfo vm)
			{
				return;
			}

			if (vm.Status is VirtualMachineStatus.Running)
			{
				btnStart.Enabled = true;
				btnStart.Text = "Stop";
				toolTip.SetToolTip(btnStart, "Stop this virtual machine");
				btnEdit.Enabled = false;
				btnDelete.Enabled = false;
				btnConfigure.Enabled = true;
				btnPause.Enabled = true;
				btnPause.Text = "Pause";
				btnReset.Enabled = true;
				btnCtrlAltDel.Enabled = true;
			}
			else if (vm.Status is VirtualMachineStatus.Stopped)
			{
				btnStart.Enabled = true;
				btnStart.Text = "Start";
				toolTip.SetToolTip(btnStart, "Start this virtual machine");
				btnEdit.Enabled = true;
				btnDelete.Enabled = true;
				btnConfigure.Enabled = true;
				btnPause.Enabled = false;
				btnPause.Text = "Pause";
				btnReset.Enabled = false;
				btnCtrlAltDel.Enabled = false;
			}
			else if (vm.Status is VirtualMachineStatus.Paused)
			{
				btnStart.Enabled = true;
				btnStart.Text = "Stop";
				toolTip.SetToolTip(btnStart, "Stop this virtual machine");
				btnEdit.Enabled = false;
				btnDelete.Enabled = false;
				btnConfigure.Enabled = true;
				btnPause.Enabled = true;
				btnPause.Text = "Resume";
				btnReset.Enabled = true;
				btnCtrlAltDel.Enabled = true;
			}
			else if (vm.Status is VirtualMachineStatus.Waiting)
			{
				btnStart.Enabled = false;
				btnStart.Text = "Stop";
				toolTip.SetToolTip(btnStart, "Stop this virtual machine");
				btnEdit.Enabled = false;
				btnDelete.Enabled = false;
				btnReset.Enabled = false;
				btnCtrlAltDel.Enabled = false;
				btnPause.Enabled = false;
				btnPause.Text = "Pause";
				btnConfigure.Enabled = false;
			}
		}
		else
		{
			btnConfigure.Enabled = false;
			btnStart.Enabled = false;
			btnEdit.Enabled = false;
			btnDelete.Enabled = true;
			btnReset.Enabled = false;
			btnCtrlAltDel.Enabled = false;
			btnPause.Enabled = false;
		}
	}

	private void btnAdd_Click(object sender, EventArgs e)
	{
		dlgAddVM dlg = _serviceProvider.GetRequiredService<dlgAddVM>();
		dlg.ShowDialog();
		dlg.Dispose();
	}

	private void btnEdit_Click(object sender, EventArgs e)
	{
		dlgEditVM dlg = _serviceProvider.GetRequiredService<dlgEditVM>();
		dlg.ShowDialog();
		dlg.Dispose();
	}

	//Load the settings from the registry
	private void LoadSettings()
	{
		Result loadSettingsResult = _settingsProvider.LoadSettings();

		if (loadSettingsResult.IsFailed)
		{
			MessageBox.Show("An error occured trying to load the 86Box Manager registry keys and/or values. Make sure you have the required permissions and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			Application.Exit();
		}
	}

	//TODO: Rewrite
	//Load the VMs from the registry
	private void LoadVMs()
	{
		lstVMs.Items.Clear();
		VMCountRefresh();

		Result<IReadOnlyCollection<VirtualMachineInfo>> listVirtualMachinesResult = _virtualMachineManager.ListVirtualMachines();

		if (listVirtualMachinesResult.IsFailed)
		{
			MessageBox.Show("The Virtual Machines registry key could not be opened, so no stored virtual machines can be used. Make sure you have the required permissions and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			return;
		}

		foreach (VirtualMachineInfo vmInfo in listVirtualMachinesResult.Value)
		{
			ListViewItem newLvi = new ListViewItem(vmInfo.Name)
			{
				Tag = vmInfo,
				ImageIndex = 0
			};

			string displayFriendlyStatus = GetDisplayFriendlyStatus(vmInfo.Status);

			newLvi.SubItems.Add(new ListViewItem.ListViewSubItem(newLvi, displayFriendlyStatus));
			newLvi.SubItems.Add(new ListViewItem.ListViewSubItem(newLvi, vmInfo.Description));
			newLvi.SubItems.Add(new ListViewItem.ListViewSubItem(newLvi, vmInfo.Path));
			lstVMs.Items.Add(newLvi);
		}

		lstVMs.SelectedItems.Clear();
		btnStart.Enabled = false;
		btnPause.Enabled = false;
		btnEdit.Enabled = false;
		btnDelete.Enabled = false;
		btnConfigure.Enabled = false;
		btnCtrlAltDel.Enabled = false;
		btnReset.Enabled = false;

		VMCountRefresh();
	}

	private static string GetDisplayFriendlyStatus(VirtualMachineStatus status)
	{
		return status switch
		{
			VirtualMachineStatus.Stopped => "Stopped",
			VirtualMachineStatus.Running => "Running",
			VirtualMachineStatus.Waiting => "Waiting",
			VirtualMachineStatus.Paused => "Paused",
			_ => "Invalid status",
		};
	}

	//Wait for the associated window of a VM to close
	private void backgroundWorker_DoWork(object? sender, DoWorkEventArgs e)
	{
		if (e.Argument is not VirtualMachineInfo vm)
		{
			return;
		}

		try
		{
			Process p = Process.GetProcessById(vm.RunningProcessId); //Find the process associated with the VM
			p.WaitForExit(); //Wait for it to exit
		}
		catch (Exception ex)
		{
			MessageBox.Show("An error has occurred. Please provide the following details to the developer:\n" + ex.Message + "\n" + ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		e.Result = vm;
	}

	//Update the UI once the VM's window is closed
	private void backgroundWorker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
	{
		if (e.Result is not VirtualMachineInfo vm)
		{
			return;
		}

		//Go through the listview, find the item representing the VM and update things accordingly
		foreach (ListViewItem item in lstVMs.Items)
		{
			if (item.Tag != null && item.Tag.Equals(vm))
			{
				vm.Status = VirtualMachineStatus.Stopped;
				vm.RunningWindowHandle = IntPtr.Zero;
				item.SubItems[1].Text = GetDisplayFriendlyStatus(vm.Status);
				item.ImageIndex = 0;
				if (lstVMs.SelectedItems.Count > 0 && lstVMs.SelectedItems[0].Equals(item))
				{
					btnEdit.Enabled = true;
					btnDelete.Enabled = true;
					btnStart.Enabled = true;
					btnStart.Text = "Start";
					toolTip.SetToolTip(btnStart, "Start this virtual machine");
					btnConfigure.Enabled = true;
					btnPause.Enabled = false;
					btnPause.Text = "Pause";
					btnCtrlAltDel.Enabled = false;
					btnReset.Enabled = false;
				}
			}
		}

		VMCountRefresh();
	}

	//Enable/disable relevant menu items depending on selected VM's status
	private void cmsVM_Opening(object sender, CancelEventArgs e)
	{
		//Available menu option differs based on the number of selected VMs
		if (lstVMs.SelectedItems.Count == 0)
		{
			e.Cancel = true;
		}
		else if (lstVMs.SelectedItems.Count == 1)
		{
			if (lstVMs.SelectedItems[0].Tag is not VirtualMachineInfo vm)
			{
				return;
			}

			switch (vm.Status)
			{
				case VirtualMachineStatus.Running:
					{
						startToolStripMenuItem.Text = "Stop";
						startToolStripMenuItem.Enabled = true;
						startToolStripMenuItem.ToolTipText = "Stop this virtual machine";
						editToolStripMenuItem.Enabled = false;
						deleteToolStripMenuItem.Enabled = false;
						hardResetToolStripMenuItem.Enabled = true;
						resetCTRLALTDELETEToolStripMenuItem.Enabled = true;
						pauseToolStripMenuItem.Enabled = true;
						pauseToolStripMenuItem.Text = "Pause";
						configureToolStripMenuItem.Enabled = true;
					}
					break;
				case VirtualMachineStatus.Stopped:
					{
						startToolStripMenuItem.Text = "Start";
						startToolStripMenuItem.Enabled = true;
						startToolStripMenuItem.ToolTipText = "Start this virtual machine";
						editToolStripMenuItem.Enabled = true;
						deleteToolStripMenuItem.Enabled = true;
						hardResetToolStripMenuItem.Enabled = false;
						resetCTRLALTDELETEToolStripMenuItem.Enabled = false;
						pauseToolStripMenuItem.Enabled = false;
						pauseToolStripMenuItem.Text = "Pause";
						configureToolStripMenuItem.Enabled = true;
					}
					break;
				case VirtualMachineStatus.Waiting:
					{
						startToolStripMenuItem.Enabled = false;
						startToolStripMenuItem.Text = "Stop";
						startToolStripMenuItem.ToolTipText = "Stop this virtual machine";
						editToolStripMenuItem.Enabled = false;
						deleteToolStripMenuItem.Enabled = false;
						hardResetToolStripMenuItem.Enabled = false;
						resetCTRLALTDELETEToolStripMenuItem.Enabled = false;
						pauseToolStripMenuItem.Enabled = false;
						pauseToolStripMenuItem.Text = "Pause";
						pauseToolStripMenuItem.ToolTipText = "Pause this virtual machine";
						configureToolStripMenuItem.Enabled = false;
					}
					break;
				case VirtualMachineStatus.Paused:
					{
						startToolStripMenuItem.Enabled = true;
						startToolStripMenuItem.Text = "Stop";
						startToolStripMenuItem.ToolTipText = "Stop this virtual machine";
						editToolStripMenuItem.Enabled = false;
						deleteToolStripMenuItem.Enabled = false;
						hardResetToolStripMenuItem.Enabled = true;
						resetCTRLALTDELETEToolStripMenuItem.Enabled = true;
						pauseToolStripMenuItem.Enabled = true;
						pauseToolStripMenuItem.Text = "Resume";
						pauseToolStripMenuItem.ToolTipText = "Resume this virtual machine";
						configureToolStripMenuItem.Enabled = true;
					}
					break;
			};
		}
		//Multiple VMs selected => disable most options
		else
		{
			startToolStripMenuItem.Text = "Start";
			startToolStripMenuItem.Enabled = false;
			startToolStripMenuItem.ToolTipText = "Start this virtual machine";
			editToolStripMenuItem.Enabled = false;
			deleteToolStripMenuItem.Enabled = true;
			hardResetToolStripMenuItem.Enabled = false;
			resetCTRLALTDELETEToolStripMenuItem.Enabled = false;
			pauseToolStripMenuItem.Enabled = false;
			pauseToolStripMenuItem.Text = "Pause";
			killToolStripMenuItem.Enabled = true;
			configureToolStripMenuItem.Enabled = false;
			cloneToolStripMenuItem.Enabled = false;
		}
	}

	//Closing 86Box Manager before closing all the VMs can lead to weirdness if 86Box Manager is then restarted. So let's warn the user just in case and request confirmation.
	private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
	{
		int vmCount = 0; //Number of running VMs

		//Close to tray
		if (e.CloseReason == CloseReason.UserClosing && _settingsProvider.SettingsValues.CloseToTray)
		{
			e.Cancel = true;
			trayIcon.Visible = true;
			WindowState = FormWindowState.Minimized;
			Hide();
		}
		else
		{
			foreach (ListViewItem item in lstVMs.Items)
			{
				VirtualMachineInfo? vm = item.Tag as VirtualMachineInfo;

				if (vm is not null && vm.Status is not VirtualMachineStatus.Stopped && Visible)
				{
					vmCount++;
				}
			}
		}

		//If there are running VMs, display the warning and stop the VMs if user says so
		if (vmCount > 0)
		{
			e.Cancel = true;
			DialogResult = MessageBox.Show("Some virtual machines are still running. It's recommended you stop them first before closing 86Box Manager. Do you want to stop them now?", "Virtual machines are still running", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
			if (DialogResult == DialogResult.Yes)
			{
				foreach (ListViewItem lvi in lstVMs.Items)
				{
					lstVMs.SelectedItems.Clear(); //To prevent weird stuff
					VirtualMachineInfo? vm = (VirtualMachineInfo?)lvi.Tag;
					if (vm is not null && vm.Status is not VirtualMachineStatus.Stopped)
					{
						lvi.Focused = true;
						lvi.Selected = true;
						VMForceStop(); //Tell the VM to shut down without confirmation
						Process p = Process.GetProcessById(vm.RunningProcessId);
						p.WaitForExit(500); //Wait 500 milliseconds for each VM to close
					}
				}

			}
			else if (DialogResult == DialogResult.Cancel)
			{
				return;
			}

			e.Cancel = false;
		}

		//Save main window's state, size and position
		//BUGBUG: Restoring these on startup causes anchor problems, so we're not doing it anymore...
		/*Settings.Default.WindowState = WindowState;
		Settings.Default.WindowSize = Size;
		Settings.Default.WindowPosition = Location;*/

		//Save listview column widths
		Settings.Default.NameColWidth = clmName.Width;
		Settings.Default.StatusColWidth = clmStatus.Width;
		Settings.Default.DescColWidth = clmDesc.Width;
		Settings.Default.PathColWidth = clmPath.Width;

		Settings.Default.Save();
	}

	private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (lstVMs.SelectedItems[0].Tag is not VirtualMachineInfo vm)
		{
			return;
		}

		if (vm.Status is VirtualMachineStatus.Paused)
		{
			VMResume();
		}
		else if (vm.Status is VirtualMachineStatus.Running)
		{
			VMPause();
		}
	}

	//Pauses the selected VM
	private void VMPause()
	{
		if (lstVMs.SelectedItems[0].Tag is not VirtualMachineInfo vm)
		{
			return;
		}

		PostMessage(vm.RunningWindowHandle, 0x8890, IntPtr.Zero, IntPtr.Zero);
		lstVMs.SelectedItems[0].SubItems[1].Text = GetDisplayFriendlyStatus(vm.Status);
		lstVMs.SelectedItems[0].ImageIndex = 2;
		pauseToolStripMenuItem.Text = "Resume";
		btnPause.Text = "Resume";
		toolTip.SetToolTip(btnStart, "Stop this virtual machine");
		btnStart.Enabled = true;
		btnStart.Text = "Stop";
		startToolStripMenuItem.Text = "Stop";
		startToolStripMenuItem.ToolTipText = "Stop this virtual machine";
		btnConfigure.Enabled = true;
		pauseToolStripMenuItem.ToolTipText = "Resume this virtual machine";
		toolTip.SetToolTip(btnPause, "Resume this virtual machine");

		VMSort(_settingsProvider.SettingsValues.SortColumn, _settingsProvider.SettingsValues.SortOrder.ToWinFormsSortOrder());
		VMCountRefresh();
	}

	//Resumes the selected VM
	private void VMResume()
	{
		if (lstVMs.SelectedItems[0].Tag is not VirtualMachineInfo vm)
		{
			return;
		}

		PostMessage(vm.RunningWindowHandle, 0x8890, IntPtr.Zero, IntPtr.Zero);
		vm.Status = VirtualMachineStatus.Running;
		lstVMs.SelectedItems[0].SubItems[1].Text = GetDisplayFriendlyStatus(vm.Status);
		lstVMs.SelectedItems[0].ImageIndex = 1;
		pauseToolStripMenuItem.Text = "Pause";
		btnPause.Text = "Pause";
		btnStart.Enabled = true;
		startToolStripMenuItem.Text = "Stop";
		startToolStripMenuItem.ToolTipText = "Stop this virtual machine";
		btnConfigure.Enabled = true;
		pauseToolStripMenuItem.ToolTipText = "Pause this virtual machine";
		toolTip.SetToolTip(btnStart, "Stop this virtual machine");
		toolTip.SetToolTip(btnPause, "Pause this virtual machine");

		VMSort(_settingsProvider.SettingsValues.SortColumn, _settingsProvider.SettingsValues.SortOrder.ToWinFormsSortOrder());
		VMCountRefresh();
	}

	//Starts the selected VM
	private void VMStart()
	{
		try
		{
			if (lstVMs.SelectedItems[0].Tag is not VirtualMachineInfo vm)
			{
				return;
			}

			/* This generates a VM ID on the fly from the VM path. The reason it's done this way is it doesn't break existing VMs and doesn't require
			 * extensive modifications to this legacy version for it to work with newer 86Box versions...
			 * 
			 * IDs also have to be unsigned for 86Box, but GetHashCode() returns signed and result can be negative, so shift it up by int.MaxValue to
			 * ensure it's always positive. */
			int tempid = vm.Path.GetHashCode();
			uint id = 0;

			if (tempid < 0)
				id = (uint)(tempid + int.MaxValue);
			else
				id = (uint)tempid;

			string idString = string.Format("{0:X}", id).PadLeft(16, '0');

			if (vm.Status is VirtualMachineStatus.Stopped)
			{
				Process p = new Process();
				p.StartInfo.FileName = Path.Combine(_settingsProvider.SettingsValues.BoxExePath, "86Box.exe");
				p.StartInfo.Arguments = "--vmpath \"" + lstVMs.SelectedItems[0].SubItems[3].Text + "\" --hwnd " + idString + "," + hWndHex;

				if (_settingsProvider.SettingsValues.LoggingEnabled)
				{
					p.StartInfo.Arguments += " --logfile \"" + _settingsProvider.SettingsValues.LogPath + "\"";
				}
				if (!_settingsProvider.SettingsValues.ShowConsole)
				{
					p.StartInfo.RedirectStandardOutput = true;
					p.StartInfo.UseShellExecute = false;
				}

				p.Start();
				vm.RunningProcessId = p.Id;
				vm.Status = VirtualMachineStatus.Running;
				lstVMs.SelectedItems[0].SubItems[1].Text = GetDisplayFriendlyStatus(vm.Status);
				lstVMs.SelectedItems[0].ImageIndex = 1;

				//Minimize the main window if the user wants this
				if (_settingsProvider.SettingsValues.MinimizeOnVMStart)
				{
					WindowState = FormWindowState.Minimized;
				}

				//Create a new background worker which will wait for the VM's window to close, so it can update the UI accordingly
				BackgroundWorker bgw = new BackgroundWorker
				{
					WorkerReportsProgress = false,
					WorkerSupportsCancellation = false
				};
				bgw.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);
				bgw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
				bgw.RunWorkerAsync(vm);

				btnStart.Enabled = true;
				btnStart.Text = "Stop";
				toolTip.SetToolTip(btnStart, "Stop this virtual machine");
				btnEdit.Enabled = false;
				btnDelete.Enabled = false;
				btnPause.Enabled = true;
				btnPause.Text = "Pause";
				btnReset.Enabled = true;
				btnCtrlAltDel.Enabled = true;
				btnConfigure.Enabled = true;

				VMCountRefresh();
			}
		}
		catch (InvalidOperationException)
		{
			MessageBox.Show("The process failed to initialize or its window handle could not be obtained.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		catch (Win32Exception)
		{
			MessageBox.Show("Cannot find 86Box.exe. Make sure your settings are correct and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		catch (Exception ex)
		{
			MessageBox.Show("An error has occurred. Please provide the following information to the developer:\n" + ex.Message + "\n" + ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		VMSort(_settingsProvider.SettingsValues.SortColumn, _settingsProvider.SettingsValues.SortOrder.ToWinFormsSortOrder());
		VMCountRefresh();
	}

	//Sends a running/pause VM a request to stop without asking the user for confirmation
	private void VMForceStop()
	{
		if (lstVMs.SelectedItems[0].Tag is not VirtualMachineInfo vm)
		{
			return;
		}

		try
		{
			if (vm.Status is VirtualMachineStatus.Running or VirtualMachineStatus.Paused)
			{
				PostMessage(vm.RunningWindowHandle, 0x8893, new IntPtr(1), IntPtr.Zero);
			}
		}
		catch (Exception)
		{
			MessageBox.Show("An error occurred trying to stop the selected virtual machine.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		VMSort(_settingsProvider.SettingsValues.SortColumn, _settingsProvider.SettingsValues.SortOrder.ToWinFormsSortOrder());
		VMCountRefresh();
	}

	//Sends a running/paused VM a request to stop and asking the user for confirmation
	private void VMRequestStop()
	{
		if (lstVMs.SelectedItems[0].Tag is not VirtualMachineInfo vm)
		{
			return;
		}

		try
		{
			if (vm.Status is VirtualMachineStatus.Running or VirtualMachineStatus.Paused)
			{
				PostMessage(vm.RunningWindowHandle, 0x8893, IntPtr.Zero, IntPtr.Zero);
				SetForegroundWindow(vm.RunningWindowHandle);
			}
		}
		catch (Exception)
		{
			MessageBox.Show("An error occurred trying to stop the selected virtual machine.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		VMSort(_settingsProvider.SettingsValues.SortColumn, _settingsProvider.SettingsValues.SortOrder.ToWinFormsSortOrder());
		VMCountRefresh();
	}

	//Start VM if it's stopped or stop it if it's running/paused
	private void startToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (lstVMs.SelectedItems[0].Tag is not VirtualMachineInfo vm)
		{
			return;
		}

		if (vm.Status is VirtualMachineStatus.Stopped)
		{
			VMStart();
		}
		else if (vm.Status is VirtualMachineStatus.Running or VirtualMachineStatus.Paused)
		{
			VMRequestStop();
		}
	}

	private void configureToolStripMenuItem_Click(object sender, EventArgs e)
	{
		VMConfigure();
	}

	//Opens the settings window for the selected VM
	private void VMConfigure()
	{
		if (lstVMs.SelectedItems[0].Tag is not VirtualMachineInfo vm)
		{
			return;
		}

		//If the VM is already running, only send the message to open the settings window. Otherwise, start the VM with the -S parameter
		if (vm.Status is VirtualMachineStatus.Running or VirtualMachineStatus.Paused)
		{
			PostMessage(vm.RunningWindowHandle, 0x8889, IntPtr.Zero, IntPtr.Zero);
			SetForegroundWindow(vm.RunningWindowHandle);
		}
		else if (vm.Status is VirtualMachineStatus.Stopped)
		{
			try
			{
				Process p = new Process();
				p.StartInfo.FileName = Path.Combine(_settingsProvider.SettingsValues.BoxExePath, "86Box.exe");
				p.StartInfo.Arguments = "--settings --vmpath \"" + lstVMs.SelectedItems[0].SubItems[3].Text + "\"";
				if (!_settingsProvider.SettingsValues.ShowConsole)
				{
					p.StartInfo.RedirectStandardOutput = true;
					p.StartInfo.UseShellExecute = false;
				}
				p.Start();
				p.WaitForInputIdle();

				vm.Status = VirtualMachineStatus.Waiting;
				vm.RunningWindowHandle = p.MainWindowHandle;
				vm.RunningProcessId = p.Id;
				lstVMs.SelectedItems[0].SubItems[1].Text = GetDisplayFriendlyStatus(vm.Status);
				lstVMs.SelectedItems[0].ImageIndex = 2;

				BackgroundWorker bgw = new BackgroundWorker
				{
					WorkerReportsProgress = false,
					WorkerSupportsCancellation = false
				};
				bgw.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);
				bgw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
				bgw.RunWorkerAsync(vm);

				btnStart.Enabled = false;
				btnStart.Text = "Stop";
				toolTip.SetToolTip(btnStart, "Stop this virtual machine");
				startToolStripMenuItem.Text = "Stop";
				startToolStripMenuItem.ToolTipText = "Stop this virtual machine";
				btnEdit.Enabled = false;
				btnDelete.Enabled = false;
				btnConfigure.Enabled = false;
				btnReset.Enabled = false;
				btnPause.Enabled = false;
				btnPause.Text = "Pause";
				toolTip.SetToolTip(btnPause, "Pause this virtual machine");
				pauseToolStripMenuItem.Text = "Pause";
				pauseToolStripMenuItem.ToolTipText = "Pause this virtual machine";
				btnCtrlAltDel.Enabled = false;
			}
			catch (Win32Exception)
			{
				MessageBox.Show("Cannot find 86Box.exe. Make sure your settings are correct and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch (Exception ex)
			{
				//Revert to stopped status and alert the user
				vm.Status = VirtualMachineStatus.Stopped;
				vm.RunningWindowHandle = IntPtr.Zero;
				vm.RunningProcessId = -1;
				MessageBox.Show("This virtual machine could not be started. Please provide the following information to the developer:\n" + ex.Message + "\n" + ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		VMSort(_settingsProvider.SettingsValues.SortColumn, _settingsProvider.SettingsValues.SortOrder.ToWinFormsSortOrder());
		VMCountRefresh();
	}

	private void resetCTRLALTDELETEToolStripMenuItem_Click(object sender, EventArgs e)
	{
		VMCtrlAltDel();
	}

	//Sends the CTRL+ALT+DEL keystroke to the VM, result depends on the guest OS
	private void VMCtrlAltDel()
	{
		if (lstVMs.SelectedItems[0].Tag is not VirtualMachineInfo vm)
		{
			return;
		}

		if (vm.Status is VirtualMachineStatus.Running or VirtualMachineStatus.Paused)
		{
			PostMessage(vm.RunningWindowHandle, 0x8894, IntPtr.Zero, IntPtr.Zero);
			vm.Status = VirtualMachineStatus.Running;
			lstVMs.SelectedItems[0].SubItems[1].Text = GetDisplayFriendlyStatus(vm.Status);
			btnPause.Text = "Pause";
			toolTip.SetToolTip(btnPause, "Pause this virtual machine");
			pauseToolStripMenuItem.Text = "Pause";
			pauseToolStripMenuItem.ToolTipText = "Pause this virtual machine";
		}
		VMCountRefresh();
	}

	private void hardResetToolStripMenuItem_Click(object sender, EventArgs e)
	{
		VMHardReset();
	}

	//Performs a hard reset for the selected VM
	private void VMHardReset()
	{
		if (lstVMs.SelectedItems[0].Tag is not VirtualMachineInfo vm)
		{
			return;
		}

		if (vm.Status is VirtualMachineStatus.Running or VirtualMachineStatus.Paused)
		{
			PostMessage(vm.RunningWindowHandle, 0x8892, IntPtr.Zero, IntPtr.Zero);
			SetForegroundWindow(vm.RunningWindowHandle);
		}
		VMCountRefresh();
	}

	//For double clicking an item, do something based on VM status
	private void lstVMs_MouseDoubleClick(object sender, MouseEventArgs e)
	{
		if (e.Button != MouseButtons.Left)
		{
			return;
		}

		if (!lstVMs.SelectedItems[0].Bounds.Contains(e.Location))
		{
			return;
		}

		if (lstVMs.SelectedItems[0].Tag is not VirtualMachineInfo vm)
		{
			return;
		}

		if (vm.Status is VirtualMachineStatus.Stopped)
		{
			VMStart();
		}
		else if (vm.Status is VirtualMachineStatus.Running)
		{
			VMRequestStop();
		}
		else if (vm.Status is VirtualMachineStatus.Paused)
		{
			VMResume();
		}
	}

	//Creates a new VM from the data recieved and adds it to the listview
	public void VMAdd(string name, string desc, bool openCFG, bool startVM)
	{
		Result<VirtualMachineInfo> newVmResult = _virtualMachineManager.CreateVirtualMachine(name, desc);

		if (newVmResult.IsFailed)
		{
			MessageBox.Show($"Error while creating virtual machine \"{name}\": \n {newVmResult} ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			return;
		}

		MessageBox.Show($"Virtual machine \"{name}\" was successfully created!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

		VirtualMachineInfo newVm = newVmResult.Value;

		ListViewItem newLvi = new ListViewItem(newVm.Name)
		{
			Tag = newVm,
			ImageIndex = 0
		};
		newLvi.SubItems.Add(new ListViewItem.ListViewSubItem(newLvi, GetDisplayFriendlyStatus(newVm.Status)));
		newLvi.SubItems.Add(new ListViewItem.ListViewSubItem(newLvi, newVm.Description));
		newLvi.SubItems.Add(new ListViewItem.ListViewSubItem(newLvi, newVm.Path));
		lstVMs.Items.Add(newLvi);

		//Select the newly created VM
		foreach (ListViewItem lvi in lstVMs.SelectedItems)
		{
			lvi.Selected = false;
		}
		newLvi.Focused = true;
		newLvi.Selected = true;

		//Start the VM and/or open settings window if the user chose this option
		if (startVM)
		{
			VMStart();
		}
		if (openCFG)
		{
			VMConfigure();
		}

		VMSort(_settingsProvider.SettingsValues.SortColumn, _settingsProvider.SettingsValues.SortOrder.ToWinFormsSortOrder());
		VMCountRefresh();
	}

	//Checks if a VM with this name already exists
	public bool VMCheckIfExists(string name)
	{
		try
		{
			RegistryKey? regkey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\86Box\Virtual Machines", true);
			if (regkey == null) //Regkey doesn't exist yet
			{
				regkey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\86Box", true);
				if (regkey == null)
				{
					throw new ArgumentNullException(); ;
				}

				regkey.CreateSubKey(@"Virtual Machines");
				return false;
			}

			//VM's registry value doesn't exist yet
			if (regkey.GetValue(name) == null)
			{
				regkey.Close();
				return false;
			}
			else
			{
				regkey.Close();
				return true;
			}
		}
		catch (Exception)
		{
			MessageBox.Show("Could not load the virtual machine information from the registry. Make sure you have the required permissions and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			return false;
		}
	}

	//Changes a VM's name and/or description
	public void VMEdit(string newName, string newDesc)
	{
		if (lstVMs.SelectedItems[0].Tag is not VirtualMachineInfo vm)
		{
			return;
		}

		string oldName = vm.Name;
		if (!vm.Name.Equals(newName))
		{
			string oldPath = vm.Path;
			string newPath = Path.Combine(_settingsProvider.SettingsValues.VmPath, newName);

			try
			{ //Move the actual VM files too. This will invalidate any paths inside the cfg, but the user is informed to update those manually.
				Directory.Move(oldPath, newPath);
			}
			catch (Exception)
			{
				MessageBox.Show("An error has occurred while trying to move the files for this virtual machine. Please try to move them manually.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			vm.Name = newName;
		}
		vm.Description = newDesc;

		//Create a new registry value with new info, delete the old one
		RegistryKey? regkey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\86Box\Virtual Machines", true);
		if (regkey == null)
		{
			return;
		}

		using (var ms = new MemoryStream())
		{
			regkey.DeleteValue(oldName);
			var formatter = new BinaryFormatter();
			formatter.Serialize(ms, vm);
			var data = ms.ToArray();
			regkey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\86Box\Virtual Machines", true);
			if (regkey == null)
			{
				return;
			}

			regkey.SetValue(vm.Name, data, RegistryValueKind.Binary);
		}
		regkey.Close();

		MessageBox.Show("Virtual machine \"" + vm.Name + "\" was successfully modified. Please update its configuration so that any absolute paths (e.g. for hard disk images) point to the new folder.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

		VMSort(_settingsProvider.SettingsValues.SortColumn, _settingsProvider.SettingsValues.SortOrder.ToWinFormsSortOrder());
		LoadVMs();
	}

	private void btnDelete_Click(object sender, EventArgs e)
	{
		VMRemove();
	}

	//Removes the selected VM. Confirmations for maximum safety
	private void VMRemove()
	{
		foreach (ListViewItem lvi in lstVMs.SelectedItems)
		{
			if (lvi.Tag is not VirtualMachineInfo vm)
			{
				continue;
			}

			DialogResult confirmDeletionResult = MessageBox.Show("Are you sure you want to remove the virtual machine \"" + vm.Name + "\"?", "Remove virtual machine", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

			if (confirmDeletionResult != DialogResult.Yes)
			{
				continue;
			}

			if (vm.Status is not VirtualMachineStatus.Stopped)
			{
				MessageBox.Show("Virtual machine \"" + vm.Name + "\" is currently running and cannot be removed. Please stop virtual machines before attempting to remove them.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				continue;
			}
			try
			{
				lstVMs.Items.Remove(lvi);
				RegistryKey? regkey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\86Box\Virtual Machines", true);

				if (regkey == null)
				{
					return;
				}

				regkey.DeleteValue(vm.Name);
				regkey.Close();
			}
			catch (Exception ex) //Catches "regkey doesn't exist" exceptions and such
			{
				MessageBox.Show("Virtual machine \"" + vm.Name + "\" could not be removed due to the following error:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				continue;
			}

			DialogResult alsoDeleteFilesResult = MessageBox.Show("Virtual machine \"" + vm.Name + "\" was successfully removed. Would you like to delete its files as well?", "Virtual machine removed", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			if (alsoDeleteFilesResult != DialogResult.Yes)
			{
				continue;
			}

			try
			{
				Directory.Delete(vm.Path, true);
			}
			catch (UnauthorizedAccessException) //Files are read-only or protected by privileges
			{
				MessageBox.Show("86Box Manager was unable to delete the files of this virtual machine because they are read-only or you don't have sufficient privileges to delete them.\n\nMake sure the files are free for deletion, then remove them manually.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				continue;
			}
			catch (DirectoryNotFoundException) //Directory not found
			{
				MessageBox.Show("86Box Manager was unable to delete the files of this virtual machine because they no longer exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				continue;
			}
			catch (IOException) //Files are in use by another process
			{
				MessageBox.Show("86Box Manager was unable to delete some files of this virtual machine because they are currently in use by another process.\n\nMake sure the files are free for deletion, then remove them manually.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				continue;
			}
			catch (Exception ex)
			{ //Other exceptions
				MessageBox.Show("The following error occurred while trying to remove the files of this virtual machine:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				continue;
			}
			MessageBox.Show("Files of virtual machine \"" + vm.Name + "\" were successfully deleted.", "Virtual machine files removed", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
		VMSort(_settingsProvider.SettingsValues.SortColumn, _settingsProvider.SettingsValues.SortOrder.ToWinFormsSortOrder());
		VMCountRefresh();
	}

	private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		VMRemove();
	}

	private void editToolStripMenuItem_Click(object sender, EventArgs e)
	{
		dlgEditVM dlg = _serviceProvider.GetRequiredService<dlgEditVM>();
		dlg.ShowDialog();
		dlg.Dispose();
	}

	private void btnCtrlAltDel_Click(object sender, EventArgs e)
	{
		VMCtrlAltDel();
	}

	private void btnReset_Click(object sender, EventArgs e)
	{
		VMHardReset();
	}

	private void btnPause_Click(object sender, EventArgs e)
	{
		if (lstVMs.SelectedItems[0].Tag is not VirtualMachineInfo vm)
		{
			return;
		}

		if (vm.Status is VirtualMachineStatus.Paused)
		{
			VMResume();
		}
		else if (vm.Status is VirtualMachineStatus.Running)
		{
			VMPause();
		}
	}

	//This function monitors recieved window messages
	protected override void WndProc(ref Message m)
	{
		// 0x8891 - Main window init complete, wparam = VM ID, lparam = VM window handle
		// 0x8895 - VM paused/resumed, wparam = 1: VM paused, wparam = 0: VM resumed
		// 0x8896 - Dialog opened/closed, wparam = 1: opened, wparam = 0: closed
		// 0x8897 - Shutdown confirmed
		if (m.Msg == 0x8891)
		{
			if (m.LParam != IntPtr.Zero && m.WParam.ToInt64() >= 0)
			{
				foreach (ListViewItem lvi in lstVMs.Items)
				{
					if (lvi.Tag is not VirtualMachineInfo vm)
					{
						continue;
					}

					int tempid = vm.Path.GetHashCode(); //This can return negative integers, which is a no-no for 86Box, hence the shift up by int.MaxValue
					uint id = tempid < 0 ? (uint)(tempid + int.MaxValue) : (uint)tempid;
					if (id == (uint)m.WParam.ToInt32())
					{
						vm.RunningWindowHandle = m.LParam;
						break;
					}
				}
			}
		}
		if (m.Msg == 0x8895)
		{
			if (m.WParam.ToInt32() == 1) //VM was paused
			{
				foreach (ListViewItem lvi in lstVMs.Items)
				{
					if (lvi.Tag is not VirtualMachineInfo vm)
					{
						continue;
					}

					if (vm.RunningWindowHandle.Equals(m.LParam) && vm.Status is not VirtualMachineStatus.Paused)
					{
						vm.Status = VirtualMachineStatus.Paused;
						lvi.SubItems[1].Text = GetDisplayFriendlyStatus(vm.Status);
						lvi.ImageIndex = 2;
						pauseToolStripMenuItem.Text = "Resume";
						btnPause.Text = "Resume";
						pauseToolStripMenuItem.ToolTipText = "Resume this virtual machine";
						toolTip.SetToolTip(btnPause, "Resume this virtual machine");
						btnStart.Enabled = true;
						btnStart.Text = "Stop";
						startToolStripMenuItem.Text = "Stop";
						startToolStripMenuItem.ToolTipText = "Stop this virtual machine";
						toolTip.SetToolTip(btnStart, "Stop this virtual machine");
						btnConfigure.Enabled = true;
					}
				}
				VMCountRefresh();
			}
			else if (m.WParam.ToInt32() == 0) //VM was resumed
			{
				foreach (ListViewItem lvi in lstVMs.Items)
				{
					if (lvi.Tag is not VirtualMachineInfo vm)
					{
						continue;
					}

					if (vm.RunningWindowHandle == m.LParam && vm.Status is not VirtualMachineStatus.Running)
					{
						vm.Status = VirtualMachineStatus.Running;
						lvi.SubItems[1].Text = GetDisplayFriendlyStatus(vm.Status);
						lvi.ImageIndex = 1;
						pauseToolStripMenuItem.Text = "Pause";
						btnPause.Text = "Pause";
						toolTip.SetToolTip(btnPause, "Pause this virtual machine");
						pauseToolStripMenuItem.ToolTipText = "Pause this virtual machine";
						btnStart.Enabled = true;
						btnStart.Text = "Stop";
						toolTip.SetToolTip(btnStart, "Stop this virtual machine");
						startToolStripMenuItem.Text = "Stop";
						startToolStripMenuItem.ToolTipText = "Stop this virtual machine";
						btnConfigure.Enabled = true;
					}
				}
				VMCountRefresh();
			}
		}
		if (m.Msg == 0x8896)
		{
			if (m.WParam.ToInt32() == 1)  //A dialog was opened
			{
				foreach (ListViewItem lvi in lstVMs.Items)
				{
					if (lvi.Tag is not VirtualMachineInfo vm)
					{
						continue;
					}

					if (vm.RunningWindowHandle == m.LParam && vm.Status is not VirtualMachineStatus.Waiting)
					{
						vm.Status = VirtualMachineStatus.Waiting;
						lvi.SubItems[1].Text = GetDisplayFriendlyStatus(vm.Status);
						lvi.ImageIndex = 2;
						btnStart.Enabled = false;
						btnStart.Text = "Stop";
						toolTip.SetToolTip(btnStart, "Stop this virtual machine");
						startToolStripMenuItem.Text = "Stop";
						startToolStripMenuItem.ToolTipText = "Stop this virtual machine";
						btnEdit.Enabled = false;
						btnDelete.Enabled = false;
						btnConfigure.Enabled = false;
						btnReset.Enabled = false;
						btnPause.Enabled = false;
						btnCtrlAltDel.Enabled = false;
					}
				}
				VMCountRefresh();
			}
			else if (m.WParam.ToInt32() == 0) //A dialog was closed
			{
				foreach (ListViewItem lvi in lstVMs.Items)
				{
					if (lvi.Tag is not VirtualMachineInfo vm)
					{
						continue;
					}

					if (vm.RunningWindowHandle == m.LParam && vm.Status is not VirtualMachineStatus.Running)
					{
						vm.Status = VirtualMachineStatus.Running;
						lvi.SubItems[1].Text = GetDisplayFriendlyStatus(vm.Status);
						lvi.ImageIndex = 1;
						btnStart.Enabled = true;
						btnStart.Text = "Stop";
						toolTip.SetToolTip(btnStart, "Stop this virtual machine");
						startToolStripMenuItem.Text = "Stop";
						startToolStripMenuItem.ToolTipText = "Stop this virtual machine";
						btnEdit.Enabled = false;
						btnDelete.Enabled = false;
						btnConfigure.Enabled = true;
						btnReset.Enabled = true;
						btnPause.Enabled = true;
						btnPause.Text = "Pause";
						pauseToolStripMenuItem.Text = "Pause";
						pauseToolStripMenuItem.ToolTipText = "Pause this virtual machine";
						toolTip.SetToolTip(btnPause, "Pause this virtual machine");
						btnCtrlAltDel.Enabled = true;
					}
				}
				VMCountRefresh();
			}
		}

		if (m.Msg == 0x8897) //Shutdown confirmed
		{
			foreach (ListViewItem lvi in lstVMs.Items)
			{
				if (lvi.Tag is not VirtualMachineInfo vm)
				{
					continue;
				}

				if (vm.RunningWindowHandle.Equals(m.LParam) && vm.Status is not VirtualMachineStatus.Stopped)
				{
					vm.Status = VirtualMachineStatus.Stopped;
					vm.RunningWindowHandle = IntPtr.Zero;
					lvi.SubItems[1].Text = GetDisplayFriendlyStatus(vm.Status);
					lvi.ImageIndex = 0;

					btnStart.Text = "Start";
					startToolStripMenuItem.Text = "Start";
					startToolStripMenuItem.ToolTipText = "Start this virtual machine";
					toolTip.SetToolTip(btnStart, "Start this virtual machine");
					btnPause.Text = "Pause";
					pauseToolStripMenuItem.ToolTipText = "Pause this virtual machine";
					pauseToolStripMenuItem.Text = "Pause";
					toolTip.SetToolTip(btnPause, "Pause this virtual machine");
					if (lstVMs.SelectedItems.Count == 1)
					{
						btnEdit.Enabled = true;
						btnDelete.Enabled = true;
						btnStart.Enabled = true;
						btnConfigure.Enabled = true;
						btnPause.Enabled = false;
						btnReset.Enabled = false;
						btnCtrlAltDel.Enabled = false;
					}
					else if (lstVMs.SelectedItems.Count == 0)
					{
						btnEdit.Enabled = false;
						btnDelete.Enabled = false;
						btnStart.Enabled = false;
						btnConfigure.Enabled = false;
						btnPause.Enabled = false;
						btnReset.Enabled = false;
						btnCtrlAltDel.Enabled = false;
					}
					else
					{
						btnEdit.Enabled = false;
						btnDelete.Enabled = true;
						btnStart.Enabled = false;
						btnConfigure.Enabled = false;
						btnPause.Enabled = false;
						btnReset.Enabled = false;
						btnCtrlAltDel.Enabled = false;
					}
				}
			}
			VMCountRefresh();
		}
		//This is the WM_COPYDATA message, used here to pass command line args to an already running instance
		//NOTE: This code will have to be modified in case more command line arguments are added in the future.
		if (m.Msg == 0x004A)
		{
			//Get the VM name and find the associated LVI and VM object
			COPYDATASTRUCT? ds = (COPYDATASTRUCT?)m.GetLParam(typeof(COPYDATASTRUCT));
			if (ds == null)
			{
				return;
			}
			string vmName = Marshal.PtrToStringAnsi(ds.Value.lpData, ds.Value.cbData);
			ListViewItem? lvi = lstVMs.FindItemWithText(vmName);

			//This check is necessary in case the specified VM was already removed but the shortcut remains
			if (lvi != null)
			{
				if (lvi.Tag is not VirtualMachineInfo vm)
				{
					return;
				}

				//If the VM is already running, display a message, otherwise, start it
				if (vm.Status is not VirtualMachineStatus.Stopped)
				{
					MessageBox.Show("The virtual machine \"" + vmName + "\" is already running.", "Virtual machine already running", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				else
				{
					//This is needed so that we start the correct VM in case multiple items are selected
					lstVMs.SelectedItems.Clear();

					lvi.Focused = true;
					lvi.Selected = true;
					VMStart();
				}
			}
			else
			{
				MessageBox.Show("The virtual machine \"" + vmName + "\" could not be found. It may have been removed or the specified name is incorrect.", "Virtual machine not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		base.WndProc(ref m);
	}

	private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
	{
		VMOpenFolder();
	}

	//Opens the folder containg the selected VM
	private void VMOpenFolder()
	{
		foreach (ListViewItem lvi in lstVMs.SelectedItems)
		{
			if (lvi.Tag is not VirtualMachineInfo vm)
			{
				return;
			}

			try
			{
				ProcessStartInfo startInfo = new(vm.Path)
				{
					UseShellExecute = true
				};
				Process.Start(startInfo);
			}
			catch (Exception)
			{
				MessageBox.Show("The folder for the virtual machine \"" + vm.Name + "\" could not be opened. Make sure it still exists and that you have sufficient privileges to access it.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}

	private void createADesktopShortcutToolStripMenuItem_Click(object sender, EventArgs e)
	{
		foreach (ListViewItem lvi in lstVMs.SelectedItems)
		{
			if (lvi.Tag is not VirtualMachineInfo vm)
			{
				continue;
			}

			try
			{
				WshShell shell = new WshShell();
				string shortcutAddress = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\" + vm.Name + ".lnk";
				IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);
				shortcut.Description = vm.Description;
				shortcut.IconLocation = Application.StartupPath + @"\86manager.exe,0";
				shortcut.TargetPath = Application.StartupPath + @"\86manager.exe";
				shortcut.Arguments = "-S \"" + vm.Name + "\"";
				shortcut.Save();

				MessageBox.Show("A desktop shortcut for the virtual machine \"" + vm.Name + "\" was successfully created.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception)
			{
				MessageBox.Show("A desktop shortcut for the virtual machine \"" + vm.Name + "\" could not be created.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}

	//Starts/stops selected VM when enter is pressed
	private void lstVMs_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Enter && lstVMs.SelectedItems.Count == 1)
		{
			if (lstVMs.SelectedItems[0].Tag is not VirtualMachineInfo vm)
			{
				return;
			}

			if (vm.Status is VirtualMachineStatus.Running)
			{
				VMRequestStop();
			}
			else if (vm.Status is VirtualMachineStatus.Stopped)
			{
				VMStart();
			}
		}
		if (e.KeyCode == Keys.Delete && lstVMs.SelectedItems.Count == 1)
		{
			VMRemove();
		}
	}

	private void trayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
	{
		//Restore the window and hide the tray icon
		Show();
		WindowState = FormWindowState.Normal;
		BringToFront();
		trayIcon.Visible = false;
	}

	private void exitToolStripMenuItem_Click(object sender, EventArgs e)
	{
		int vmCount = 0;
		foreach (ListViewItem item in lstVMs.Items)
		{
			if (item.Tag is VirtualMachineInfo vm && vm.Status is not VirtualMachineStatus.Stopped)
			{
				vmCount++;
			}
		}

		//If there are running VMs, display the warning and stop the VMs if user says so
		if (vmCount > 0)
		{
			DialogResult = MessageBox.Show("Some virtual machines are still running. It's recommended you stop them first before closing 86Box Manager. Do you want to stop them now?", "Virtual machines are still running", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
			if (DialogResult == DialogResult.Yes)
			{
				foreach (ListViewItem lvi in lstVMs.Items)
				{
					lstVMs.SelectedItems.Clear();

					if (lvi.Tag is not VirtualMachineInfo vm)
					{
						return;
					}

					if (vm.Status is not VirtualMachineStatus.Stopped)
					{
						lvi.Focused = true;
						lvi.Selected = true;
						VMForceStop(); //Tell the VMs to stop without asking for user confirmation
					}
				}

				Thread.Sleep(vmCount * 500); //Wait just a bit to make sure everything goes as planned
			}
			else if (DialogResult == DialogResult.Cancel)
			{
				return;
			}
		}
		Application.Exit();
	}

	//Handles things when WindowState changes
	private void frmMain_Resize(object sender, EventArgs e)
	{
		if (WindowState == FormWindowState.Minimized && _settingsProvider.SettingsValues.MinimizeToTray)
		{
			trayIcon.Visible = true;
			Hide();
		}
		if (WindowState == FormWindowState.Normal)
		{
			Show();
			trayIcon.Visible = false;
		}
	}

	private void open86BoxManagerToolStripMenuItem_Click(object sender, EventArgs e)
	{
		Show();
		WindowState = FormWindowState.Normal;
		BringToFront();
		trayIcon.Visible = false;
	}

	private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
	{
		Show();
		WindowState = FormWindowState.Normal;
		BringToFront();
		trayIcon.Visible = false;
		dlgSettings ds = _serviceProvider.GetRequiredService<dlgSettings>();
		ds.ShowDialog();
		LoadSettings();
		ds.Dispose();
	}

	//Kills the process associated with the selected VM
	private void VMKill()
	{
		foreach (ListViewItem lvi in lstVMs.SelectedItems)
		{
			if (lvi.Tag is not VirtualMachineInfo vm)
			{
				continue;
			}

			//Ask the user to confirm
			DialogResult = MessageBox.Show("Killing a virtual machine can cause data loss. Only do this if 86Box.exe process gets stuck.\n\nDo you really wish to kill the virtual machine \"" + vm.Name + "\"?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
			if (DialogResult == DialogResult.Yes)
			{
				try
				{
					Process p = Process.GetProcessById(vm.RunningProcessId);
					p.Kill();
				}
				catch (Exception)
				{
					MessageBox.Show("Could not kill 86Box.exe process for virtual machine \"" + vm.Name + "\". The process may have already ended on its own or access was denied.", "Could not kill process", MessageBoxButtons.OK, MessageBoxIcon.Error);
					continue;
				}

				//We need to cleanup afterwards to make sure the VM is put back into a valid state
				vm.Status = VirtualMachineStatus.Stopped;
				vm.RunningWindowHandle = IntPtr.Zero;
				lstVMs.SelectedItems[0].SubItems[1].Text = GetDisplayFriendlyStatus(vm.Status);
				lstVMs.SelectedItems[0].ImageIndex = 0;

				btnStart.Text = "Start";
				toolTip.SetToolTip(btnStart, "Stop this virtual machine");
				btnPause.Text = "Pause";
				if (lstVMs.SelectedItems.Count > 0)
				{
					btnEdit.Enabled = true;
					btnDelete.Enabled = true;
					btnStart.Enabled = true;
					btnConfigure.Enabled = true;
					btnPause.Enabled = false;
					btnReset.Enabled = false;
					btnCtrlAltDel.Enabled = false;
				}
				else
				{
					btnEdit.Enabled = false;
					btnDelete.Enabled = false;
					btnStart.Enabled = false;
					btnConfigure.Enabled = false;
					btnPause.Enabled = false;
					btnReset.Enabled = false;
					btnCtrlAltDel.Enabled = false;
				}
			}
		}

		VMSort(_settingsProvider.SettingsValues.SortColumn, _settingsProvider.SettingsValues.SortOrder.ToWinFormsSortOrder());
		VMCountRefresh();
	}

	private void killToolStripMenuItem_Click(object sender, EventArgs e)
	{
		VMKill();
	}

	//Sort the VM list by specified column and order
	private void VMSort(int column, SortOrder order)
	{
		const string ascArrow = " ▲";
		const string descArrow = " ▼";

		if (lstVMs.SelectedItems.Count > 1)
		{
			lstVMs.SelectedItems.Clear(); //Just in case so we don't end up with weird selection glitches
		}

		int previousSortColumn = _settingsProvider.SettingsValues.SortColumn;

		//Remove the arrows from the current column text if they exist
		if (previousSortColumn > -1 && (lstVMs.Columns[previousSortColumn].Text.EndsWith(ascArrow) || lstVMs.Columns[previousSortColumn].Text.EndsWith(descArrow)))
		{
			lstVMs.Columns[previousSortColumn].Text = lstVMs.Columns[previousSortColumn].Text.Substring(0, lstVMs.Columns[previousSortColumn].Text.Length - 2);
		}

		//Then append the appropriate arrow to the new column text
		if (order == SortOrder.Ascending)
		{
			lstVMs.Columns[column].Text += ascArrow;
		}
		else if (order == SortOrder.Descending)
		{
			lstVMs.Columns[column].Text += descArrow;
		}

		lstVMs.Sorting = order;
		lstVMs.ListViewItemSorter = new ListViewItemComparer(column, lstVMs.Sorting);
		lstVMs.Sort();
	}

	//Handles the click event for the listview column headers, allowing to sort the items by columns
	private void lstVMs_ColumnClick(object sender, ColumnClickEventArgs e)
	{
		SortOrder newSortOrder = lstVMs.Sorting == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;

		VMSort(e.Column, newSortOrder);

		// Save new sort settings
		Result saveSortSettingsResult = _settingsProvider.SaveSortSettings(e.Column, newSortOrder.ToCoreSortOrder());

		if (saveSortSettingsResult.IsFailed)
		{
			MessageBox.Show("Could not save the column sorting state to the registry. Make sure you have the required permissions and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}

	private void wipeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		VMClearCMOS();
	}

	//Deletes the nvr of selected VMs
	private void VMClearCMOS()
	{
		foreach (ListViewItem lvi in lstVMs.SelectedItems)
		{
			if (lvi.Tag is not VirtualMachineInfo vm)
			{
				continue;
			}

			DialogResult = MessageBox.Show($"Clearing the CMOS will delete BIOS configuration (nvr files). This is effectively like removing the battery on a real motherboard. You'll have to reconfigure the BIOS.\n\n" +
				$"Are you sure you want to clear the CMOS of virtual machine \"{vm.Name}\"?", "Warning",
				MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
			if (DialogResult != DialogResult.Yes)
			{
				continue;
			}

			if (vm.Status is not VirtualMachineStatus.Stopped)
			{
				MessageBox.Show("The virtual machine \"" + vm.Name + "\" is currently running and cannot has its CMOS cleared. Please stop virtual machines before attempting to clear CMOS.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Error);
				continue;
			}
			try
			{
				_virtualMachineManager.ClearCmos(vm);
				MessageBox.Show("The CMOS for virtual machine \"" + vm.Name + "\" was successfully cleared.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception)
			{
				MessageBox.Show("An error occurred trying to clear CMOS for the virtual machine \"" + vm.Name + "\".", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				continue;
			}
		}
	}

	//Imports existing VM files to a new VM
	public void VMImport(string name, string desc, string importPath, bool openCFG, bool startVM)
	{
		Result<VirtualMachineInfo> newVmResult = _virtualMachineManager.ImportVirtualMachine(importPath, name, desc);

		if (newVmResult.IsFailed)
		{
			MessageBox.Show($"Error while creating virtual machine \"{name}\". " +
				$"Files could not be imported. Make sure the path you selected was correct and valid.\n\n" +
				$"If the VM is already located in your VMs folder, you don't need to select the Import option, just add a new VM with the same name.:" +
				$"\n{newVmResult}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			return;
		}
		else
		{
			MessageBox.Show($"Virtual machine \"{name}\" was successfully created, files were imported. Remember to update any paths pointing to disk images in your config!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		VirtualMachineInfo newVm = newVmResult.Value;

		ListViewItem newLvi = new ListViewItem(newVm.Name)
		{
			Tag = newVm,
			ImageIndex = 0
		};
		newLvi.SubItems.Add(new ListViewItem.ListViewSubItem(newLvi, GetDisplayFriendlyStatus(newVm.Status)));
		newLvi.SubItems.Add(new ListViewItem.ListViewSubItem(newLvi, newVm.Description));
		newLvi.SubItems.Add(new ListViewItem.ListViewSubItem(newLvi, newVm.Path));
		lstVMs.Items.Add(newLvi);

		//Select the newly created VM
		foreach (ListViewItem lvi in lstVMs.SelectedItems)
		{
			lvi.Selected = false;
		}
		newLvi.Focused = true;
		newLvi.Selected = true;

		//Start the VM and/or open settings window if the user chose this option
		if (startVM)
		{
			VMStart();
		}
		if (openCFG)
		{
			VMConfigure();
		}

		VMSort(_settingsProvider.SettingsValues.SortColumn, _settingsProvider.SettingsValues.SortOrder.ToWinFormsSortOrder());
		VMCountRefresh();
	}

	private void cloneToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (lstVMs.SelectedItems[0].Tag is not VirtualMachineInfo vm)
		{
			return;
		}

		dlgCloneVM dc = _serviceProvider.GetRequiredService<dlgCloneVM>();
		dc.OldPath = vm.Path;
		dc.ShowDialog();
		dc.Dispose();
	}

	//Refreshes the VM counter in the status bar
	private void VMCountRefresh()
	{
		int runningVMs = 0;
		int pausedVMs = 0;
		int waitingVMs = 0;
		int stoppedVMs = 0;

		foreach (ListViewItem lvi in lstVMs.Items)
		{
			if (lvi.Tag is not VirtualMachineInfo vm)
			{
				continue;
			}
			switch (vm.Status)
			{
				case VirtualMachineStatus.Paused: pausedVMs++; break;
				case VirtualMachineStatus.Running: runningVMs++; break;
				case VirtualMachineStatus.Stopped: stoppedVMs++; break;
				case VirtualMachineStatus.Waiting: waitingVMs++; break;
			}
		}

		lblVMCount.Text = "All VMs: " + lstVMs.Items.Count + " | Running: " + runningVMs + " | Paused: " + pausedVMs + " | Waiting: " + waitingVMs + " | Stopped: " + stoppedVMs;
	}

	private void openConfigFileToolStripMenuItem_Click(object sender, EventArgs e)
	{
		VMOpenConfig();
	}

	private void VMOpenConfig()
	{
		foreach (ListViewItem lvi in lstVMs.SelectedItems)
		{
			if (lvi.Tag is not VirtualMachineInfo vm)
			{
				continue;
			}
			try
			{
				ProcessStartInfo startInfo = new(vm.Path, "86box.cfg")
				{
					UseShellExecute = true
				};
				Process.Start(startInfo);
			}
			catch (Exception)
			{
				MessageBox.Show("The config file for the virtual machine \"" + vm.Name + "\" could not be opened. Make sure it still exists and that you have sufficient privileges to access it.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}