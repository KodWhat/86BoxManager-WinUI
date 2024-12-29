namespace EightySixBoxManager
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
			btnEdit = new System.Windows.Forms.Button();
			btnDelete = new System.Windows.Forms.Button();
			btnStart = new System.Windows.Forms.Button();
			lstVMs = new System.Windows.Forms.ListView();
			clmName = new System.Windows.Forms.ColumnHeader();
			clmStatus = new System.Windows.Forms.ColumnHeader();
			clmDesc = new System.Windows.Forms.ColumnHeader();
			clmPath = new System.Windows.Forms.ColumnHeader();
			cmsVM = new System.Windows.Forms.ContextMenuStrip(components);
			startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			configureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			pauseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			resetCTRLALTDELETEToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			hardResetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			killToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			wipeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			cloneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			openFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			openConfigFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			createADesktopShortcutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			img86box = new System.Windows.Forms.ImageList(components);
			btnAdd = new System.Windows.Forms.Button();
			btnConfigure = new System.Windows.Forms.Button();
			imgStatus = new System.Windows.Forms.ImageList(components);
			btnPause = new System.Windows.Forms.Button();
			btnCtrlAltDel = new System.Windows.Forms.Button();
			btnReset = new System.Windows.Forms.Button();
			trayIcon = new System.Windows.Forms.NotifyIcon(components);
			cmsTrayIcon = new System.Windows.Forms.ContextMenuStrip(components);
			open86BoxManagerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			statusBar = new System.Windows.Forms.StatusStrip();
			lblVMCount = new System.Windows.Forms.ToolStripStatusLabel();
			btnSettings = new System.Windows.Forms.Button();
			toolTip = new System.Windows.Forms.ToolTip(components);
			cmsVM.SuspendLayout();
			cmsTrayIcon.SuspendLayout();
			statusBar.SuspendLayout();
			SuspendLayout();
			// 
			// btnEdit
			// 
			btnEdit.Enabled = false;
			btnEdit.FlatStyle = System.Windows.Forms.FlatStyle.System;
			btnEdit.Font = new System.Drawing.Font("Segoe UI", 10F);
			btnEdit.Location = new System.Drawing.Point(72, 15);
			btnEdit.Margin = new System.Windows.Forms.Padding(4);
			btnEdit.Name = "btnEdit";
			btnEdit.Size = new System.Drawing.Size(56, 38);
			btnEdit.TabIndex = 1;
			btnEdit.Text = "Edit";
			toolTip.SetToolTip(btnEdit, "Edit the properties of this virtual machine");
			btnEdit.UseVisualStyleBackColor = true;
			btnEdit.Click += btnEdit_Click;
			// 
			// btnDelete
			// 
			btnDelete.Enabled = false;
			btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.System;
			btnDelete.Font = new System.Drawing.Font("Segoe UI", 10F);
			btnDelete.Location = new System.Drawing.Point(136, 15);
			btnDelete.Margin = new System.Windows.Forms.Padding(4);
			btnDelete.Name = "btnDelete";
			btnDelete.Size = new System.Drawing.Size(75, 38);
			btnDelete.TabIndex = 2;
			btnDelete.Text = "Remove";
			toolTip.SetToolTip(btnDelete, "Remove this virtual machine");
			btnDelete.UseVisualStyleBackColor = true;
			btnDelete.Click += btnDelete_Click;
			// 
			// btnStart
			// 
			btnStart.Enabled = false;
			btnStart.FlatStyle = System.Windows.Forms.FlatStyle.System;
			btnStart.Font = new System.Drawing.Font("Segoe UI", 10F);
			btnStart.Location = new System.Drawing.Point(246, 15);
			btnStart.Margin = new System.Windows.Forms.Padding(4);
			btnStart.Name = "btnStart";
			btnStart.Size = new System.Drawing.Size(56, 38);
			btnStart.TabIndex = 3;
			btnStart.Text = "Start";
			toolTip.SetToolTip(btnStart, "Start this virtual machine");
			btnStart.UseVisualStyleBackColor = true;
			btnStart.Click += btnStart_Click;
			// 
			// lstVMs
			// 
			lstVMs.AllowColumnReorder = true;
			lstVMs.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			lstVMs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { clmName, clmStatus, clmDesc, clmPath });
			lstVMs.ContextMenuStrip = cmsVM;
			lstVMs.Font = new System.Drawing.Font("Segoe UI", 10F);
			lstVMs.FullRowSelect = true;
			lstVMs.Location = new System.Drawing.Point(15, 60);
			lstVMs.Margin = new System.Windows.Forms.Padding(4);
			lstVMs.Name = "lstVMs";
			lstVMs.ShowGroups = false;
			lstVMs.ShowItemToolTips = true;
			lstVMs.Size = new System.Drawing.Size(824, 515);
			lstVMs.SmallImageList = img86box;
			lstVMs.Sorting = System.Windows.Forms.SortOrder.Ascending;
			lstVMs.TabIndex = 10;
			lstVMs.UseCompatibleStateImageBehavior = false;
			lstVMs.View = System.Windows.Forms.View.Details;
			lstVMs.ColumnClick += lstVMs_ColumnClick;
			lstVMs.SelectedIndexChanged += lstVMs_SelectedIndexChanged;
			lstVMs.KeyDown += lstVMs_KeyDown;
			lstVMs.MouseDoubleClick += lstVMs_MouseDoubleClick;
			// 
			// clmName
			// 
			clmName.Text = "Name";
			clmName.Width = 184;
			// 
			// clmStatus
			// 
			clmStatus.Text = "Status";
			clmStatus.Width = 107;
			// 
			// clmDesc
			// 
			clmDesc.Text = "Description";
			clmDesc.Width = 144;
			// 
			// clmPath
			// 
			clmPath.Text = "Path";
			clmPath.Width = 217;
			// 
			// cmsVM
			// 
			cmsVM.ImageScalingSize = new System.Drawing.Size(20, 20);
			cmsVM.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { startToolStripMenuItem, configureToolStripMenuItem, pauseToolStripMenuItem, resetCTRLALTDELETEToolStripMenuItem, hardResetToolStripMenuItem, toolStripSeparator3, killToolStripMenuItem, wipeToolStripMenuItem, toolStripSeparator1, editToolStripMenuItem, cloneToolStripMenuItem, deleteToolStripMenuItem, openFolderToolStripMenuItem, openConfigFileToolStripMenuItem, createADesktopShortcutToolStripMenuItem });
			cmsVM.Name = "cmsVM";
			cmsVM.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			cmsVM.Size = new System.Drawing.Size(210, 324);
			cmsVM.Opening += cmsVM_Opening;
			// 
			// startToolStripMenuItem
			// 
			startToolStripMenuItem.Name = "startToolStripMenuItem";
			startToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
			startToolStripMenuItem.Text = "Start";
			startToolStripMenuItem.ToolTipText = "Start this virtual machine";
			startToolStripMenuItem.Click += startToolStripMenuItem_Click;
			// 
			// configureToolStripMenuItem
			// 
			configureToolStripMenuItem.Name = "configureToolStripMenuItem";
			configureToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
			configureToolStripMenuItem.Text = "Configure";
			configureToolStripMenuItem.ToolTipText = "Change configuration for this virtual machine";
			configureToolStripMenuItem.Click += configureToolStripMenuItem_Click;
			// 
			// pauseToolStripMenuItem
			// 
			pauseToolStripMenuItem.Name = "pauseToolStripMenuItem";
			pauseToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
			pauseToolStripMenuItem.Text = "Pause";
			pauseToolStripMenuItem.ToolTipText = "Pause this virtual machine";
			pauseToolStripMenuItem.Click += pauseToolStripMenuItem_Click;
			// 
			// resetCTRLALTDELETEToolStripMenuItem
			// 
			resetCTRLALTDELETEToolStripMenuItem.Name = "resetCTRLALTDELETEToolStripMenuItem";
			resetCTRLALTDELETEToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
			resetCTRLALTDELETEToolStripMenuItem.Text = "Send CTRL+ALT+DEL";
			resetCTRLALTDELETEToolStripMenuItem.ToolTipText = "Send the CTRL+ALT+DEL keystroke to this virtual machine";
			resetCTRLALTDELETEToolStripMenuItem.Click += resetCTRLALTDELETEToolStripMenuItem_Click;
			// 
			// hardResetToolStripMenuItem
			// 
			hardResetToolStripMenuItem.Name = "hardResetToolStripMenuItem";
			hardResetToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
			hardResetToolStripMenuItem.Text = "Hard reset";
			hardResetToolStripMenuItem.ToolTipText = "Reset this virtual machine by simulating a power cycle";
			hardResetToolStripMenuItem.Click += hardResetToolStripMenuItem_Click;
			// 
			// toolStripSeparator3
			// 
			toolStripSeparator3.Name = "toolStripSeparator3";
			toolStripSeparator3.Size = new System.Drawing.Size(206, 6);
			// 
			// killToolStripMenuItem
			// 
			killToolStripMenuItem.Name = "killToolStripMenuItem";
			killToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
			killToolStripMenuItem.Text = "Kill";
			killToolStripMenuItem.ToolTipText = "Kill this virtual machine";
			killToolStripMenuItem.Click += killToolStripMenuItem_Click;
			// 
			// wipeToolStripMenuItem
			// 
			wipeToolStripMenuItem.Name = "wipeToolStripMenuItem";
			wipeToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
			wipeToolStripMenuItem.Text = "Clear CMOS";
			wipeToolStripMenuItem.ToolTipText = "Delete nvr for this virtual machine";
			wipeToolStripMenuItem.Click += wipeToolStripMenuItem_Click;
			// 
			// toolStripSeparator1
			// 
			toolStripSeparator1.Name = "toolStripSeparator1";
			toolStripSeparator1.Size = new System.Drawing.Size(206, 6);
			// 
			// editToolStripMenuItem
			// 
			editToolStripMenuItem.Name = "editToolStripMenuItem";
			editToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
			editToolStripMenuItem.Text = "Edit";
			editToolStripMenuItem.ToolTipText = "Edit the properties of this virtual machine";
			editToolStripMenuItem.Click += editToolStripMenuItem_Click;
			// 
			// cloneToolStripMenuItem
			// 
			cloneToolStripMenuItem.Name = "cloneToolStripMenuItem";
			cloneToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
			cloneToolStripMenuItem.Text = "Clone";
			cloneToolStripMenuItem.ToolTipText = "Clone this virtual machine";
			cloneToolStripMenuItem.Click += cloneToolStripMenuItem_Click;
			// 
			// deleteToolStripMenuItem
			// 
			deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
			deleteToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
			deleteToolStripMenuItem.Text = "Remove";
			deleteToolStripMenuItem.ToolTipText = "Remove this virtual machine";
			deleteToolStripMenuItem.Click += deleteToolStripMenuItem_Click;
			// 
			// openFolderToolStripMenuItem
			// 
			openFolderToolStripMenuItem.Name = "openFolderToolStripMenuItem";
			openFolderToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
			openFolderToolStripMenuItem.Text = "Open folder in Explorer";
			openFolderToolStripMenuItem.ToolTipText = "Open the folder for this virtual machine in Windows Explorer";
			openFolderToolStripMenuItem.Click += openFolderToolStripMenuItem_Click;
			// 
			// openConfigFileToolStripMenuItem
			// 
			openConfigFileToolStripMenuItem.Name = "openConfigFileToolStripMenuItem";
			openConfigFileToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
			openConfigFileToolStripMenuItem.Text = "Open config file";
			openConfigFileToolStripMenuItem.ToolTipText = "Open the config file for this virtual machine";
			openConfigFileToolStripMenuItem.Click += openConfigFileToolStripMenuItem_Click;
			// 
			// createADesktopShortcutToolStripMenuItem
			// 
			createADesktopShortcutToolStripMenuItem.Name = "createADesktopShortcutToolStripMenuItem";
			createADesktopShortcutToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
			createADesktopShortcutToolStripMenuItem.Text = "Create a desktop shortcut";
			createADesktopShortcutToolStripMenuItem.ToolTipText = "Create a shortcut for this virtual machine on the desktop";
			createADesktopShortcutToolStripMenuItem.Click += createADesktopShortcutToolStripMenuItem_Click;
			// 
			// img86box
			// 
			img86box.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			img86box.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("img86box.ImageStream");
			img86box.TransparentColor = System.Drawing.Color.Transparent;
			img86box.Images.SetKeyName(0, "status_stopped.png");
			img86box.Images.SetKeyName(1, "status_running.png");
			img86box.Images.SetKeyName(2, "status_paused.png");
			// 
			// btnAdd
			// 
			btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.System;
			btnAdd.Font = new System.Drawing.Font("Segoe UI", 10F);
			btnAdd.Location = new System.Drawing.Point(15, 15);
			btnAdd.Margin = new System.Windows.Forms.Padding(4);
			btnAdd.Name = "btnAdd";
			btnAdd.Size = new System.Drawing.Size(50, 38);
			btnAdd.TabIndex = 0;
			btnAdd.Text = "Add";
			toolTip.SetToolTip(btnAdd, "Add a new or an existing virtual machine");
			btnAdd.UseVisualStyleBackColor = true;
			btnAdd.Click += btnAdd_Click;
			// 
			// btnConfigure
			// 
			btnConfigure.Enabled = false;
			btnConfigure.FlatStyle = System.Windows.Forms.FlatStyle.System;
			btnConfigure.Font = new System.Drawing.Font("Segoe UI", 10F);
			btnConfigure.Location = new System.Drawing.Point(310, 15);
			btnConfigure.Margin = new System.Windows.Forms.Padding(4);
			btnConfigure.Name = "btnConfigure";
			btnConfigure.Size = new System.Drawing.Size(88, 38);
			btnConfigure.TabIndex = 4;
			btnConfigure.Text = "Configure";
			toolTip.SetToolTip(btnConfigure, "Change the configuration of this virtual machine");
			btnConfigure.UseVisualStyleBackColor = true;
			btnConfigure.Click += btnConfigure_Click;
			// 
			// imgStatus
			// 
			imgStatus.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			imgStatus.ImageSize = new System.Drawing.Size(16, 16);
			imgStatus.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// btnPause
			// 
			btnPause.Enabled = false;
			btnPause.FlatStyle = System.Windows.Forms.FlatStyle.System;
			btnPause.Font = new System.Drawing.Font("Segoe UI", 10F);
			btnPause.Location = new System.Drawing.Point(405, 15);
			btnPause.Margin = new System.Windows.Forms.Padding(4);
			btnPause.Name = "btnPause";
			btnPause.Size = new System.Drawing.Size(69, 38);
			btnPause.TabIndex = 5;
			btnPause.Text = "Pause";
			toolTip.SetToolTip(btnPause, "Pause this virtual machine");
			btnPause.UseVisualStyleBackColor = true;
			btnPause.Click += btnPause_Click;
			// 
			// btnCtrlAltDel
			// 
			btnCtrlAltDel.Enabled = false;
			btnCtrlAltDel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			btnCtrlAltDel.Font = new System.Drawing.Font("Segoe UI", 10F);
			btnCtrlAltDel.Location = new System.Drawing.Point(481, 15);
			btnCtrlAltDel.Margin = new System.Windows.Forms.Padding(4);
			btnCtrlAltDel.Name = "btnCtrlAltDel";
			btnCtrlAltDel.Size = new System.Drawing.Size(75, 38);
			btnCtrlAltDel.TabIndex = 6;
			btnCtrlAltDel.Text = "C+A+D";
			toolTip.SetToolTip(btnCtrlAltDel, "Send the CTRL+ALT+DEL keystroke to this virtual machine");
			btnCtrlAltDel.UseVisualStyleBackColor = true;
			btnCtrlAltDel.Click += btnCtrlAltDel_Click;
			// 
			// btnReset
			// 
			btnReset.Enabled = false;
			btnReset.FlatStyle = System.Windows.Forms.FlatStyle.System;
			btnReset.Font = new System.Drawing.Font("Segoe UI", 10F);
			btnReset.Location = new System.Drawing.Point(564, 15);
			btnReset.Margin = new System.Windows.Forms.Padding(4);
			btnReset.Name = "btnReset";
			btnReset.Size = new System.Drawing.Size(62, 38);
			btnReset.TabIndex = 7;
			btnReset.Text = "Reset";
			toolTip.SetToolTip(btnReset, "Reset this virtual machine by simulating a power cycle");
			btnReset.UseVisualStyleBackColor = true;
			btnReset.Click += btnReset_Click;
			// 
			// trayIcon
			// 
			trayIcon.ContextMenuStrip = cmsTrayIcon;
			trayIcon.Icon = (System.Drawing.Icon)resources.GetObject("trayIcon.Icon");
			trayIcon.Text = "86Box Manager";
			trayIcon.MouseDoubleClick += trayIcon_MouseDoubleClick;
			// 
			// cmsTrayIcon
			// 
			cmsTrayIcon.ImageScalingSize = new System.Drawing.Size(20, 20);
			cmsTrayIcon.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { open86BoxManagerToolStripMenuItem, settingsToolStripMenuItem, toolStripSeparator2, exitToolStripMenuItem });
			cmsTrayIcon.Name = "cmsVM";
			cmsTrayIcon.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			cmsTrayIcon.Size = new System.Drawing.Size(188, 76);
			// 
			// open86BoxManagerToolStripMenuItem
			// 
			open86BoxManagerToolStripMenuItem.Name = "open86BoxManagerToolStripMenuItem";
			open86BoxManagerToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
			open86BoxManagerToolStripMenuItem.Text = "Show 86Box Manager";
			open86BoxManagerToolStripMenuItem.ToolTipText = "Restore the 86Box Manager window";
			open86BoxManagerToolStripMenuItem.Click += open86BoxManagerToolStripMenuItem_Click;
			// 
			// settingsToolStripMenuItem
			// 
			settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
			settingsToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
			settingsToolStripMenuItem.Text = "Settings";
			settingsToolStripMenuItem.ToolTipText = "Open 86Box Manager settings";
			settingsToolStripMenuItem.Click += settingsToolStripMenuItem_Click;
			// 
			// toolStripSeparator2
			// 
			toolStripSeparator2.Name = "toolStripSeparator2";
			toolStripSeparator2.Size = new System.Drawing.Size(184, 6);
			// 
			// exitToolStripMenuItem
			// 
			exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			exitToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
			exitToolStripMenuItem.Text = "Exit";
			exitToolStripMenuItem.ToolTipText = "Close 86Box Manager";
			exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
			// 
			// statusBar
			// 
			statusBar.ImageScalingSize = new System.Drawing.Size(20, 20);
			statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { lblVMCount });
			statusBar.Location = new System.Drawing.Point(0, 597);
			statusBar.Name = "statusBar";
			statusBar.Padding = new System.Windows.Forms.Padding(1, 0, 18, 0);
			statusBar.Size = new System.Drawing.Size(855, 22);
			statusBar.TabIndex = 11;
			statusBar.Text = "statusStrip1";
			// 
			// lblVMCount
			// 
			lblVMCount.BackColor = System.Drawing.Color.Transparent;
			lblVMCount.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			lblVMCount.Name = "lblVMCount";
			lblVMCount.Size = new System.Drawing.Size(121, 17);
			lblVMCount.Text = "# of virtual machines:";
			// 
			// btnSettings
			// 
			btnSettings.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
			btnSettings.FlatStyle = System.Windows.Forms.FlatStyle.System;
			btnSettings.Font = new System.Drawing.Font("Segoe UI", 10F);
			btnSettings.Location = new System.Drawing.Point(759, 15);
			btnSettings.Margin = new System.Windows.Forms.Padding(4);
			btnSettings.Name = "btnSettings";
			btnSettings.Size = new System.Drawing.Size(81, 38);
			btnSettings.TabIndex = 8;
			btnSettings.Text = "Settings";
			toolTip.SetToolTip(btnSettings, "Open 86Box Manager settings");
			btnSettings.UseVisualStyleBackColor = true;
			btnSettings.Click += btnSettings_Click;
			// 
			// frmMain
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			BackColor = System.Drawing.SystemColors.Window;
			ClientSize = new System.Drawing.Size(855, 619);
			Controls.Add(statusBar);
			Controls.Add(btnReset);
			Controls.Add(btnCtrlAltDel);
			Controls.Add(btnPause);
			Controls.Add(btnConfigure);
			Controls.Add(btnAdd);
			Controls.Add(lstVMs);
			Controls.Add(btnSettings);
			Controls.Add(btnStart);
			Controls.Add(btnDelete);
			Controls.Add(btnEdit);
			Font = new System.Drawing.Font("Segoe UI", 10F);
			Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			Margin = new System.Windows.Forms.Padding(5);
			MinimumSize = new System.Drawing.Size(870, 613);
			Name = "frmMain";
			StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			Text = "86Box Manager";
			FormClosing += frmMain_FormClosing;
			Load += frmMain_Load;
			Resize += frmMain_Resize;
			cmsVM.ResumeLayout(false);
			cmsTrayIcon.ResumeLayout(false);
			statusBar.ResumeLayout(false);
			statusBar.PerformLayout();
			ResumeLayout(false);
			PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.ColumnHeader clmName;
        private System.Windows.Forms.ColumnHeader clmStatus;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnConfigure;
        private System.Windows.Forms.ColumnHeader clmPath;
        private System.Windows.Forms.ContextMenuStrip cmsVM;
        private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pauseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetCTRLALTDELETEToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hardResetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ImageList img86box;
        private System.Windows.Forms.ImageList imgStatus;
        public System.Windows.Forms.ListView lstVMs;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnCtrlAltDel;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.ToolStripMenuItem openFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createADesktopShortcutToolStripMenuItem;
        private System.Windows.Forms.NotifyIcon trayIcon;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ContextMenuStrip cmsTrayIcon;
        private System.Windows.Forms.ToolStripMenuItem open86BoxManagerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem killToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem wipeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cloneToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel lblVMCount;
        private System.Windows.Forms.ToolStripMenuItem openConfigFileToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader clmDesc;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
