using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

using Fluent86.Core.Settings;
using Fluent86.Core.VirtualMachines;
using Fluent86.Core.VirtualMachines.List;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Fluent86;

static partial class Program
{
	public static string[] args = Environment.GetCommandLineArgs(); //Get command line arguments

	private enum ShowWindowEnum
	{
		Hide = 0,
		ShowNormal = 1, ShowMinimized = 2, ShowMaximized = 3,
		ShowNormalNoActivate = 4, Show = 5,
		Minimize = 6, ShowMinNoActivate = 7, ShowNoActivate = 8,
		Restore = 9, ShowDefault = 10, ForceMinimized = 11
	};

	[StructLayout(LayoutKind.Sequential)]
	public struct COPYDATASTRUCT
	{
		public IntPtr dwData;
		public int cbData;
		public IntPtr lpData;
	}

	[LibraryImport("user32.dll", EntryPoint = "ShowWindow")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static partial bool ShowWindow(IntPtr hWnd, ShowWindowEnum flags);

	[LibraryImport("user32.dll", EntryPoint = "SetForegroundWindow")]
	private static partial int SetForegroundWindow(IntPtr hwnd);

	[LibraryImport("user32.dll", EntryPoint = "FindWindowW")]
	private static partial IntPtr FindWindow(
		[MarshalAs(UnmanagedType.LPWStr)] string? className,
		[MarshalAs(UnmanagedType.LPWStr)] string? windowTitle);

	public const int WM_COPYDATA = 0x004A;

	[LibraryImport("user32.dll", EntryPoint = "SendMessageW", SetLastError = true)]
	private static partial int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, ref COPYDATASTRUCT lParam);

	[LibraryImport("shell32.dll", EntryPoint = "SetCurrentProcessExplicitAppUserModelID", SetLastError = true)]
	private static partial void SetCurrentProcessExplicitAppUserModelID([MarshalAs(UnmanagedType.LPWStr)] string AppID);

	[LibraryImport("user32.dll", EntryPoint = "SetProcessDPIAware")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static partial bool SetProcessDPIAware();

	private static readonly string AppID = "86Box.86Box"; //For grouping windows together in Win7+ taskbar

	public static IServiceProvider ServiceProvider { get; private set; } = null!;

	[STAThread]
	static void Main()
	{
		if (Environment.OSVersion.Version.Major >= 6)
			SetProcessDPIAware();

		SetCurrentProcessExplicitAppUserModelID(AppID);
		const string name = "86Box Manager";

		//Use a mutex to check if this is the first instance of Manager
		_ = new Mutex(true, name, out bool firstInstance);

		//If it's not, we need to restore and focus the existing window, as well as pass on any potential command line arguments
		if (!firstInstance)
		{
			//Finds the existing window, unhides it, restores it and sets focus to it
			IntPtr hWnd = FindWindow(null, "86Box Manager");
			ShowWindow(hWnd, ShowWindowEnum.Show);
			ShowWindow(hWnd, ShowWindowEnum.Restore);
			_ = SetForegroundWindow(hWnd);

			//If this second instance comes from a VM shortcut, we need to pass on the command line arguments so the VM will start
			//in the existing instance.
			//NOTE: This code will have to be modified in case more command line arguments are added in the future.
			if (args.Length == 3 && args[1] == "-S" && args[2] != null)
			{
				string message = args[2];
				COPYDATASTRUCT cds;
				cds.dwData = IntPtr.Zero;
				cds.lpData = Marshal.StringToHGlobalAnsi(message);
				cds.cbData = message.Length;
				_ = SendMessage(hWnd, WM_COPYDATA, IntPtr.Zero, ref cds);
			}

			return;
		}
		else
		{
			//Then check if any instances of 86Box are already running and warn the user
			Process[] pname = Process.GetProcessesByName("86box");
			if (pname.Length > 0)
			{
				DialogResult result = MessageBox.Show("At least one instance of 86Box.exe is already running. It's not recommended that you run 86Box.exe directly outside of Manager. Do you want to continue at your own risk?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
				if (result == DialogResult.No)
				{
					return;
				}
			}
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			IHost host = CreateHostBuilder().Build();
			ServiceProvider = host.Services;

			Application.Run(ServiceProvider.GetRequiredService<frmMain>());
		}
	}

	static IHostBuilder CreateHostBuilder()
	{
		return Host
			.CreateDefaultBuilder()
			.ConfigureServices((context, services) =>
			{
				services.AddTransient<frmMain>();
				services.AddTransient<dlgSettings>();
				services.AddTransient<dlgAddVM>();
				services.AddTransient<dlgEditVM>();
				services.AddTransient<dlgCloneVM>();

				services.AddSingleton<ISettingsProvider, RegistrySettingsProvider>();
				services.AddSingleton<IVirtualMachineListingProvider, RegistryVirtualMachineListingProvider>();
				services.AddSingleton<IVirtualMachineManager, VirtualMachineManager>();
			});
	}
}
