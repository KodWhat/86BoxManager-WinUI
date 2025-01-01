using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

using Fluent86.Core.Settings;

namespace Fluent86;

public partial class dlgCloneVM : Form
{
	private readonly ISettingsProvider _settingsProvider;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public string OldPath { get; set; } = string.Empty; //Path of the VM to be cloned

	private readonly frmMain main = (frmMain)Application.OpenForms["frmMain"]!; //Instance of frmMain

	public dlgCloneVM(ISettingsProvider settingsProvider)
	{
		_settingsProvider = settingsProvider;

		InitializeComponent();
	}

	private void dlgCloneVM_Load(object sender, EventArgs e)
	{
		lblPath1.Text = _settingsProvider.SettingsValues.VmPath;
		lblOldVM.Text = "Virtual machine \"" + Path.GetFileName(OldPath) + "\" will be cloned into:";
	}

	private void txtName_TextChanged(object sender, EventArgs e)
	{
		if (string.IsNullOrWhiteSpace(txtName.Text))
		{
			btnClone.Enabled = false;
			tipTxtName.Active = false;
		}
		else
		{
			if (txtName.Text.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
			{
				btnClone.Enabled = false;
				lblPath1.Text = "Invalid path";
				tipTxtName.Active = true;
				tipTxtName.Show("You cannot use the following characters in the name: \\ / : * ? \" < > |", txtName, 20000);
			}
			else
			{
				btnClone.Enabled = true;
				string vmPath = Path.Combine(_settingsProvider.SettingsValues.VmPath, txtName.Text);
				lblPath1.Text = vmPath;
				tipLblPath1.SetToolTip(lblPath1, vmPath);
			}
		}
	}

	private void btnClone_Click(object sender, EventArgs e)
	{
		if (main.VMCheckIfExists(txtName.Text))
		{
			MessageBox.Show("A virtual machine with this name already exists. Please pick a different name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			return;
		}
		if (txtName.Text.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
		{
			MessageBox.Show("There are invalid characters in the name you specified. You can't use the following characters: \\ / : * ? \" < > |", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			return;
		}

		//Just import stuff from the existing VM
		main.VMImport(txtName.Text, txtDescription.Text, OldPath, cbxOpenCFG.Checked, cbxStartVM.Checked);
		Close();
	}
}
