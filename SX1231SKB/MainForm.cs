using FTD2XX_NET;
using SemtechLib.Controls;
using SemtechLib.Devices.SX1231;
using SemtechLib.Devices.SX1231.Controls;
using SemtechLib.Devices.SX1231.Events;
using SemtechLib.Devices.SX1231.Forms;
using SemtechLib.General;
using SemtechLib.General.Events;
using SemtechLib.General.Interfaces;
using SemtechLib.Properties;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Threading;
using SemtechLib.Devices.SX1231.Enumerations;
using System.Runtime.InteropServices;
using SemtechLib.Ftdi;


namespace SX1231SKB
{
	public class MainForm : Form
	{
		private delegate void ConnectedDelegate();
		private delegate void DisconnectedDelegate();
		private delegate void ErrorDelegate(byte status, string message);
		private delegate void SX1231DataChangedDelegate(object sender, PropertyChangedEventArgs e);
		private delegate void SX1231PacketHandlerStartedDelegate(object sender, EventArgs e);
		private delegate void SX1231PacketHandlerStopedDelegate(object sender, EventArgs e);
		private delegate void SX1231PacketHandlerTransmittedDelegate(object sender, PacketStatusEventArg e);
        public delegate void PacketHandlerReceivedEventHandler(object sender, PacketStatusEventArg e);
        public delegate void PacketHandlerTransmittedEventHandler(object sender, PacketStatusEventArg e);
		#region Private variables
		private ToolStripMenuItem aboutToolStripMenuItem;
		private ToolStripMenuItem actionToolStripMenuItem;
		private const string ApplicationVersion = "";
		private ApplicationSettings appSettings;
		private bool appTestArg;
		private IContainer components;
		private string configFileName = "sx1231skb.cfg";
		private string configFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
		private FileStream configFileStream;
		private ToolStripMenuItem connectToolStripMenuItem;
		private ToolStripMenuItem exitToolStripMenuItem;
		private ToolStripMenuItem fileToolStripMenuItem;
		private HelpForm frmHelp;
		private PacketLogForm frmPacketLog;
		private RegistersForm frmRegisters;
		private RssiAnalyserForm frmRssiAnalyser;
		private SpectrumAnalyserForm frmSpectrumAnalyser;
		private TestForm frmTest;
		private ToolStripMenuItem helpToolStripMenuItem;
		private bool isConfigFileOpen;
		private ToolStripMenuItem loadToolStripMenuItem;
		private ToolStripSeparator mFileSeparator1;
		private ToolStripSeparator mFileSeparator2;
		private ToolStripSeparator mHelpSeparator1;
		private ToolStripSeparator mHelpSeparator2;
		private ToolStripMenuItem monitorOffToolStripMenuItem;
		private ToolStripMenuItem monitorOnToolStripMenuItem;
		private ToolStripMenuItem monitorToolStripMenuItem;
		private MenuStripEx msMainMenu;
		private OpenFileDialog ofConfigFileOpenDlg;
		private ToolStripMenuItem refreshToolStripMenuItem;
		private ToolStripMenuItem resetToolStripMenuItem;
        private const string RleaseCandidate = "";
		private ToolStripMenuItem saveAsToolStripMenuItem;
		private ToolStripMenuItem saveToolStripMenuItem;
		private SaveFileDialog sfConfigFileSaveDlg;
		private ToolStripMenuItem showHelpToolStripMenuItem;
        private ToolStripMenuItem showRegistersToolStripMenuItem;
		private StatusStrip ssMainStatus;
        private SemtechLib.Devices.SX1231.SX1231 sx1231 = new SemtechLib.Devices.SX1231.SX1231();
		private ToolStripSeparator tbFileSeparator1;
        private ToolTip tipMainForm;
		private ToolStripContainer toolStripContainer1;
		private ToolStripLabel toolStripLabel1;
		private ToolStripLabel toolStripLabel2;
		private ToolStripSeparator toolStripSeparator1;
		private ToolStripSeparator toolStripSeparator2;
		private ToolStripSeparator toolStripSeparator3;
		private ToolStripSeparator toolStripSeparator4;
		private ToolStripEx tsActionToolbar;
		private ToolStripButton tsBtnMonitorOff;
		private ToolStripButton tsBtnMonitorOn;
		private ToolStripButton tsBtnOpenDevice;
		private ToolStripButton tsBtnOpenFile;
		private ToolStripButton tsBtnRefresh;
		private ToolStripButton tsBtnReset;
		private ToolStripButton tsBtnSaveFile;
		private ToolStripButton tsBtnShowHelp;
		private ToolStripButton tsBtnShowRegisters;
		private ToolStripEx tsHelpToolbar;
		private ToolStripStatusLabel tsLblChipVersion;
		private ToolStripStatusLabel tsLblConfigFileName;
		private ToolStripStatusLabel tsLblSeparator1;
		private ToolStripStatusLabel tsLblSeparator2;
		private ToolStripStatusLabel tsLblStatus;
        private ToolStripEx tsMainToolbar;
        private RichTextBox Console;
        private Button b_ClearConsole;
        private Button b_Start;
        private ProgressBar pr_Load;
        private Label label1;
        private TextBox tb_MeterNoStart;
        private TextBox tb_MeterNoEnd;
        private Button b_Stop;
        private Button b_LoadFile;
        private GroupBox gr_Buttons;
        private GroupBox gr_Config;
        private ComboBox cb_MeterType;
        private Label label3;
        private Label lb_WaitTotal;
        private RichTextBox tb_WaitList;
        private Label lb_FinishTotal;
        private RichTextBox tb_FinishList;
        private Label label2;
        private Label label6;
        private Label label7;
        private OpenFileDialog openFileDialog1;
        private CheckBox checkBox1;
        private Label lb_percent;
        private ProgressBar pr_transmid;
		private ToolStripMenuItem usersGuideToolStripMenuItem;
		#endregion

		public MainForm(bool testMode)
		{
			appTestArg = testMode;
			InitializeComponent();
			try
			{
				appSettings = new ApplicationSettings();
			}
			catch (Exception ex)
			{
				tsLblStatus.Text = "ERROR: " + ex.Message;
			}
			sx1231.Test = testMode;
			//sx1231ViewControl.SX1231 = sx1231;

			if (!appTestArg)
				Text = AssemblyTitle ?? "";
			else
				Text = AssemblyTitle + " - ..::: TEST :::..";
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AboutBox box = new AboutBox();
			box.Version = sx1231.Version;
			box.ShowDialog();
		}

		public void DisableControls()
		{
			if (frmRegisters != null)
				frmRegisters.RegistersFormEnabled = false;

			loadToolStripMenuItem.Enabled = false;
			saveAsToolStripMenuItem.Enabled = false;
			saveToolStripMenuItem.Enabled = false;
			tsBtnOpenFile.Enabled = false;
			tsBtnSaveFile.Enabled = false;
			refreshToolStripMenuItem.Enabled = false;
			tsBtnRefresh.Enabled = false;
			monitorToolStripMenuItem.Enabled = false;
			toolStripLabel2.Enabled = false;
			tsBtnMonitorOff.Enabled = false;
			tsBtnMonitorOn.Enabled = false;
		}

		public void EnableControls()
		{
			if (frmRegisters != null)
				frmRegisters.RegistersFormEnabled = true;

			loadToolStripMenuItem.Enabled = true;
			saveAsToolStripMenuItem.Enabled = true;
			saveToolStripMenuItem.Enabled = true;
			tsBtnOpenFile.Enabled = true;
			tsBtnSaveFile.Enabled = true;
			refreshToolStripMenuItem.Enabled = true;
			tsBtnRefresh.Enabled = true;
			monitorToolStripMenuItem.Enabled = true;
			toolStripLabel2.Enabled = true;
			tsBtnMonitorOff.Enabled = true;
			tsBtnMonitorOn.Enabled = true;
		}

		protected override void Dispose(bool disposing)
		{
			appSettings.Dispose();
			//sx1231ViewControl.Dispose();
			if (sx1231 != null)
				sx1231.Dispose();
			if (disposing && (components != null))
				components.Dispose();
			base.Dispose(disposing);
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void frmHelp_Disposed(object sender, EventArgs e)
		{
			frmHelp = null;
		}

		private void frmHelp_FormClosed(object sender, FormClosedEventArgs e)
		{
			tsBtnShowHelp.Checked = false;
			showHelpToolStripMenuItem.Checked = false;
		}

		private void frmPacketLog_Disposed(object sender, EventArgs e)
		{
			frmPacketLog = null;
		}

		private void frmPacketLog_FormClosed(object sender, FormClosedEventArgs e)
		{
			sx1231.PropertyChanged -= new PropertyChangedEventHandler(sx1231_PropertyChanged);
			sx1231.Packet.LogEnabled = false;
			sx1231.PropertyChanged += new PropertyChangedEventHandler(sx1231_PropertyChanged);
		}

		private void frmRegisters_Disposed(object sender, EventArgs e)
		{
			frmRegisters = null;
		}

		private void frmRegisters_FormClosed(object sender, FormClosedEventArgs e)
		{
			tsBtnShowRegisters.Checked = false;
			showRegistersToolStripMenuItem.Checked = false;
		}

		private void frmRssiAnalyser_Disposed(object sender, EventArgs e)
		{
			frmRssiAnalyser = null;
		}

		private void frmRssiAnalyser_FormClosed(object sender, FormClosedEventArgs e)
		{
			//rssiAnalyserToolStripMenuItem.Checked = false;
		}

		private void frmSpectrumAnalyser_Disposed(object sender, EventArgs e)
		{
			frmSpectrumAnalyser = null;
		}

		private void frmSpectrumAnalyser_FormClosed(object sender, FormClosedEventArgs e)
		{
			//spectrumAnalyserToolStripMenuItem.Checked = false;
		}

		private void frmTest_Disposed(object sender, EventArgs e)
		{
			frmTest = null;
		}

		private void frmTest_FormClosed(object sender, FormClosedEventArgs e) { }

		#region InitializeComponent()
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.ssMainStatus = new System.Windows.Forms.StatusStrip();
            this.tsLblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsLblSeparator1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsLblChipVersion = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsLblSeparator2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsLblConfigFileName = new System.Windows.Forms.ToolStripStatusLabel();
            this.msMainMenu = new SemtechLib.Controls.MenuStripEx();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mFileSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mFileSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.actionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showRegistersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.monitorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.monitorOffToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.monitorOnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mHelpSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.usersGuideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mHelpSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMainToolbar = new SemtechLib.Controls.ToolStripEx();
            this.tsBtnOpenFile = new System.Windows.Forms.ToolStripButton();
            this.tsBtnSaveFile = new System.Windows.Forms.ToolStripButton();
            this.tbFileSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsBtnOpenDevice = new System.Windows.Forms.ToolStripButton();
            this.tsBtnRefresh = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.ofConfigFileOpenDlg = new System.Windows.Forms.OpenFileDialog();
            this.sfConfigFileSaveDlg = new System.Windows.Forms.SaveFileDialog();
            this.tipMainForm = new System.Windows.Forms.ToolTip(this.components);
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lb_WaitTotal = new System.Windows.Forms.Label();
            this.tb_WaitList = new System.Windows.Forms.RichTextBox();
            this.lb_FinishTotal = new System.Windows.Forms.Label();
            this.tb_FinishList = new System.Windows.Forms.RichTextBox();
            this.gr_Config = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cb_MeterType = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.pr_Load = new System.Windows.Forms.ProgressBar();
            this.tb_MeterNoStart = new System.Windows.Forms.TextBox();
            this.b_ClearConsole = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tb_MeterNoEnd = new System.Windows.Forms.TextBox();
            this.gr_Buttons = new System.Windows.Forms.GroupBox();
            this.b_LoadFile = new System.Windows.Forms.Button();
            this.b_Stop = new System.Windows.Forms.Button();
            this.b_Start = new System.Windows.Forms.Button();
            this.Console = new System.Windows.Forms.RichTextBox();
            this.tsHelpToolbar = new SemtechLib.Controls.ToolStripEx();
            this.tsBtnShowHelp = new System.Windows.Forms.ToolStripButton();
            this.tsActionToolbar = new SemtechLib.Controls.ToolStripEx();
            this.tsBtnReset = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsBtnShowRegisters = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.tsBtnMonitorOn = new System.Windows.Forms.ToolStripButton();
            this.tsBtnMonitorOff = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.lb_percent = new System.Windows.Forms.Label();
            this.pr_transmid = new System.Windows.Forms.ProgressBar();
            this.ssMainStatus.SuspendLayout();
            this.msMainMenu.SuspendLayout();
            this.tsMainToolbar.SuspendLayout();
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.gr_Config.SuspendLayout();
            this.gr_Buttons.SuspendLayout();
            this.tsHelpToolbar.SuspendLayout();
            this.tsActionToolbar.SuspendLayout();
            this.SuspendLayout();
            // 
            // ssMainStatus
            // 
            this.ssMainStatus.Dock = System.Windows.Forms.DockStyle.None;
            this.ssMainStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsLblStatus,
            this.tsLblSeparator1,
            this.tsLblChipVersion,
            this.tsLblSeparator2,
            this.tsLblConfigFileName});
            this.ssMainStatus.Location = new System.Drawing.Point(0, 0);
            this.ssMainStatus.Name = "ssMainStatus";
            this.ssMainStatus.ShowItemToolTips = true;
            this.ssMainStatus.Size = new System.Drawing.Size(1008, 22);
            this.ssMainStatus.SizingGrip = false;
            this.ssMainStatus.TabIndex = 3;
            this.ssMainStatus.Text = "statusStrip1";
            // 
            // tsLblStatus
            // 
            this.tsLblStatus.Name = "tsLblStatus";
            this.tsLblStatus.Size = new System.Drawing.Size(438, 17);
            this.tsLblStatus.Spring = true;
            this.tsLblStatus.Text = "-";
            this.tsLblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tsLblStatus.ToolTipText = "Shows EVK messages.";
            // 
            // tsLblSeparator1
            // 
            this.tsLblSeparator1.Name = "tsLblSeparator1";
            this.tsLblSeparator1.Size = new System.Drawing.Size(10, 17);
            this.tsLblSeparator1.Text = "|";
            // 
            // tsLblChipVersion
            // 
            this.tsLblChipVersion.Name = "tsLblChipVersion";
            this.tsLblChipVersion.Size = new System.Drawing.Size(97, 17);
            this.tsLblChipVersion.Text = "Chip version: --.-";
            // 
            // tsLblSeparator2
            // 
            this.tsLblSeparator2.Name = "tsLblSeparator2";
            this.tsLblSeparator2.Size = new System.Drawing.Size(10, 17);
            this.tsLblSeparator2.Text = "|";
            // 
            // tsLblConfigFileName
            // 
            this.tsLblConfigFileName.AutoToolTip = true;
            this.tsLblConfigFileName.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsLblConfigFileName.Name = "tsLblConfigFileName";
            this.tsLblConfigFileName.Size = new System.Drawing.Size(438, 17);
            this.tsLblConfigFileName.Spring = true;
            this.tsLblConfigFileName.Text = "Config File:";
            this.tsLblConfigFileName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tsLblConfigFileName.ToolTipText = "Shows the active Config file when File-> Open/Save is used";
            // 
            // msMainMenu
            // 
            this.msMainMenu.ClickThrough = true;
            this.msMainMenu.Dock = System.Windows.Forms.DockStyle.None;
            this.msMainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.actionToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.msMainMenu.Location = new System.Drawing.Point(0, 25);
            this.msMainMenu.Name = "msMainMenu";
            this.msMainMenu.Size = new System.Drawing.Size(1008, 24);
            this.msMainMenu.SuppressHighlighting = false;
            this.msMainMenu.TabIndex = 0;
            this.msMainMenu.Text = "File";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectToolStripMenuItem,
            this.mFileSeparator1,
            this.loadToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.mFileSeparator2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // connectToolStripMenuItem
            // 
            this.connectToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("connectToolStripMenuItem.Image")));
            this.connectToolStripMenuItem.Name = "connectToolStripMenuItem";
            this.connectToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.connectToolStripMenuItem.Text = "&Connect";
            this.connectToolStripMenuItem.Click += new System.EventHandler(this.tsBtnOpenDevice_Click);
            // 
            // mFileSeparator1
            // 
            this.mFileSeparator1.Name = "mFileSeparator1";
            this.mFileSeparator1.Size = new System.Drawing.Size(159, 6);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Enabled = false;
            this.loadToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("loadToolStripMenuItem.Image")));
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.loadToolStripMenuItem.Text = "&Open Config...";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Enabled = false;
            this.saveToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripMenuItem.Image")));
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.saveToolStripMenuItem.Text = "&Save Config";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Enabled = false;
            this.saveAsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveAsToolStripMenuItem.Image")));
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.saveAsToolStripMenuItem.Text = "Save Config &As...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // mFileSeparator2
            // 
            this.mFileSeparator2.Name = "mFileSeparator2";
            this.mFileSeparator2.Size = new System.Drawing.Size(159, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.exitToolStripMenuItem.Text = "&Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // actionToolStripMenuItem
            // 
            this.actionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetToolStripMenuItem,
            this.refreshToolStripMenuItem,
            this.showRegistersToolStripMenuItem,
            this.monitorToolStripMenuItem});
            this.actionToolStripMenuItem.Name = "actionToolStripMenuItem";
            this.actionToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.actionToolStripMenuItem.Text = "&Action";
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.Enabled = false;
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            this.resetToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.resetToolStripMenuItem.Text = "R&eset";
            this.resetToolStripMenuItem.Click += new System.EventHandler(this.resetToolStripMenuItem_Click);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Enabled = false;
            this.refreshToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("refreshToolStripMenuItem.Image")));
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.refreshToolStripMenuItem.Text = "&Refresh";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // showRegistersToolStripMenuItem
            // 
            this.showRegistersToolStripMenuItem.Enabled = false;
            this.showRegistersToolStripMenuItem.Name = "showRegistersToolStripMenuItem";
            this.showRegistersToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.showRegistersToolStripMenuItem.Text = "&Show registers";
            this.showRegistersToolStripMenuItem.Click += new System.EventHandler(this.showRegistersToolStripMenuItem_Click);
            // 
            // monitorToolStripMenuItem
            // 
            this.monitorToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.monitorOffToolStripMenuItem,
            this.monitorOnToolStripMenuItem});
            this.monitorToolStripMenuItem.Enabled = false;
            this.monitorToolStripMenuItem.Name = "monitorToolStripMenuItem";
            this.monitorToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.monitorToolStripMenuItem.Text = "&Monitor";
            // 
            // monitorOffToolStripMenuItem
            // 
            this.monitorOffToolStripMenuItem.Name = "monitorOffToolStripMenuItem";
            this.monitorOffToolStripMenuItem.Size = new System.Drawing.Size(95, 22);
            this.monitorOffToolStripMenuItem.Text = "OFF";
            this.monitorOffToolStripMenuItem.Click += new System.EventHandler(this.monitorToolStripMenuItem_Click);
            // 
            // monitorOnToolStripMenuItem
            // 
            this.monitorOnToolStripMenuItem.Checked = true;
            this.monitorOnToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.monitorOnToolStripMenuItem.Name = "monitorOnToolStripMenuItem";
            this.monitorOnToolStripMenuItem.Size = new System.Drawing.Size(95, 22);
            this.monitorOnToolStripMenuItem.Text = "&ON";
            this.monitorOnToolStripMenuItem.Click += new System.EventHandler(this.monitorToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showHelpToolStripMenuItem,
            this.mHelpSeparator1,
            this.usersGuideToolStripMenuItem,
            this.mHelpSeparator2,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // showHelpToolStripMenuItem
            // 
            this.showHelpToolStripMenuItem.Enabled = false;
            this.showHelpToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("showHelpToolStripMenuItem.Image")));
            this.showHelpToolStripMenuItem.Name = "showHelpToolStripMenuItem";
            this.showHelpToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.showHelpToolStripMenuItem.Text = "Help";
            this.showHelpToolStripMenuItem.Click += new System.EventHandler(this.showHelpToolStripMenuItem_Click);
            // 
            // mHelpSeparator1
            // 
            this.mHelpSeparator1.Name = "mHelpSeparator1";
            this.mHelpSeparator1.Size = new System.Drawing.Size(228, 6);
            // 
            // usersGuideToolStripMenuItem
            // 
            this.usersGuideToolStripMenuItem.Name = "usersGuideToolStripMenuItem";
            this.usersGuideToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.usersGuideToolStripMenuItem.Text = "&User\'s Guide...";
            this.usersGuideToolStripMenuItem.Click += new System.EventHandler(this.usersGuideToolStripMenuItem_Click);
            // 
            // mHelpSeparator2
            // 
            this.mHelpSeparator2.Name = "mHelpSeparator2";
            this.mHelpSeparator2.Size = new System.Drawing.Size(228, 6);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("aboutToolStripMenuItem.Image")));
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.aboutToolStripMenuItem.Text = "&About SX1231 Evaluation Kit...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // tsMainToolbar
            // 
            this.tsMainToolbar.ClickThrough = true;
            this.tsMainToolbar.Dock = System.Windows.Forms.DockStyle.None;
            this.tsMainToolbar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsMainToolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsBtnOpenFile,
            this.tsBtnSaveFile,
            this.tbFileSeparator1,
            this.tsBtnOpenDevice});
            this.tsMainToolbar.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.tsMainToolbar.Location = new System.Drawing.Point(3, 0);
            this.tsMainToolbar.Name = "tsMainToolbar";
            this.tsMainToolbar.Size = new System.Drawing.Size(78, 25);
            this.tsMainToolbar.SuppressHighlighting = false;
            this.tsMainToolbar.TabIndex = 1;
            this.tsMainToolbar.Text = "Main";
            // 
            // tsBtnOpenFile
            // 
            this.tsBtnOpenFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsBtnOpenFile.Enabled = false;
            this.tsBtnOpenFile.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnOpenFile.Image")));
            this.tsBtnOpenFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtnOpenFile.Name = "tsBtnOpenFile";
            this.tsBtnOpenFile.Size = new System.Drawing.Size(23, 22);
            this.tsBtnOpenFile.Text = "Open Config file";
            this.tsBtnOpenFile.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // tsBtnSaveFile
            // 
            this.tsBtnSaveFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsBtnSaveFile.Enabled = false;
            this.tsBtnSaveFile.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnSaveFile.Image")));
            this.tsBtnSaveFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtnSaveFile.Name = "tsBtnSaveFile";
            this.tsBtnSaveFile.Size = new System.Drawing.Size(23, 22);
            this.tsBtnSaveFile.Text = "Save Config file";
            this.tsBtnSaveFile.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // tbFileSeparator1
            // 
            this.tbFileSeparator1.Name = "tbFileSeparator1";
            this.tbFileSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsBtnOpenDevice
            // 
            this.tsBtnOpenDevice.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsBtnOpenDevice.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnOpenDevice.Image")));
            this.tsBtnOpenDevice.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtnOpenDevice.Name = "tsBtnOpenDevice";
            this.tsBtnOpenDevice.Size = new System.Drawing.Size(23, 22);
            this.tsBtnOpenDevice.Text = "Connect";
            this.tsBtnOpenDevice.Click += new System.EventHandler(this.tsBtnOpenDevice_Click);
            // 
            // tsBtnRefresh
            // 
            this.tsBtnRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsBtnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnRefresh.Image")));
            this.tsBtnRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtnRefresh.Name = "tsBtnRefresh";
            this.tsBtnRefresh.Size = new System.Drawing.Size(23, 22);
            this.tsBtnRefresh.Text = "Refresh";
            this.tsBtnRefresh.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // ofConfigFileOpenDlg
            // 
            this.ofConfigFileOpenDlg.DefaultExt = "*.cfg";
            this.ofConfigFileOpenDlg.Filter = "Config Files(*.cfg)|*.cfg|AllFiles(*.*)|*.*";
            // 
            // sfConfigFileSaveDlg
            // 
            this.sfConfigFileSaveDlg.DefaultExt = "*.cfg";
            this.sfConfigFileSaveDlg.Filter = "Config Files(*.cfg)|*.cfg|AllFiles(*.*)|*.*";
            // 
            // tipMainForm
            // 
            this.tipMainForm.ShowAlways = true;
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.BottomToolStripPanel
            // 
            this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.ssMainStatus);
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.AutoScroll = true;
            this.toolStripContainer1.ContentPanel.Controls.Add(this.label7);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.label6);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.lb_WaitTotal);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.tb_WaitList);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.lb_FinishTotal);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.tb_FinishList);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.gr_Config);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.gr_Buttons);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.Console);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(1008, 525);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(1008, 596);
            this.toolStripContainer1.TabIndex = 4;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.tsMainToolbar);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.tsHelpToolbar);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.tsActionToolbar);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.msMainMenu);
            this.toolStripContainer1.TopToolStripPanel.MaximumSize = new System.Drawing.Size(0, 50);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label7.Location = new System.Drawing.Point(904, 3);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 16);
            this.label7.TabIndex = 30;
            this.label7.Text = "Finish List";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label6.Location = new System.Drawing.Point(800, 3);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 16);
            this.label6.TabIndex = 27;
            this.label6.Text = "Wait List";
            // 
            // lb_WaitTotal
            // 
            this.lb_WaitTotal.AutoSize = true;
            this.lb_WaitTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lb_WaitTotal.Location = new System.Drawing.Point(807, 495);
            this.lb_WaitTotal.Name = "lb_WaitTotal";
            this.lb_WaitTotal.Size = new System.Drawing.Size(60, 16);
            this.lb_WaitTotal.TabIndex = 29;
            this.lb_WaitTotal.Text = "Total: 0";
            // 
            // tb_WaitList
            // 
            this.tb_WaitList.AcceptsTab = true;
            this.tb_WaitList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_WaitList.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.tb_WaitList.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.tb_WaitList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.tb_WaitList.Location = new System.Drawing.Point(782, 22);
            this.tb_WaitList.Name = "tb_WaitList";
            this.tb_WaitList.ReadOnly = true;
            this.tb_WaitList.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.tb_WaitList.Size = new System.Drawing.Size(104, 470);
            this.tb_WaitList.TabIndex = 28;
            this.tb_WaitList.Text = "";
            // 
            // lb_FinishTotal
            // 
            this.lb_FinishTotal.AutoSize = true;
            this.lb_FinishTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lb_FinishTotal.Location = new System.Drawing.Point(915, 495);
            this.lb_FinishTotal.Name = "lb_FinishTotal";
            this.lb_FinishTotal.Size = new System.Drawing.Size(60, 16);
            this.lb_FinishTotal.TabIndex = 26;
            this.lb_FinishTotal.Text = "Total: 0";
            // 
            // tb_FinishList
            // 
            this.tb_FinishList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_FinishList.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.tb_FinishList.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.tb_FinishList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.tb_FinishList.Location = new System.Drawing.Point(892, 22);
            this.tb_FinishList.Name = "tb_FinishList";
            this.tb_FinishList.ReadOnly = true;
            this.tb_FinishList.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.tb_FinishList.Size = new System.Drawing.Size(104, 470);
            this.tb_FinishList.TabIndex = 27;
            this.tb_FinishList.Text = "";
            // 
            // gr_Config
            // 
            this.gr_Config.Controls.Add(this.pr_transmid);
            this.gr_Config.Controls.Add(this.lb_percent);
            this.gr_Config.Controls.Add(this.checkBox1);
            this.gr_Config.Controls.Add(this.label2);
            this.gr_Config.Controls.Add(this.cb_MeterType);
            this.gr_Config.Controls.Add(this.label3);
            this.gr_Config.Controls.Add(this.pr_Load);
            this.gr_Config.Controls.Add(this.tb_MeterNoStart);
            this.gr_Config.Controls.Add(this.b_ClearConsole);
            this.gr_Config.Controls.Add(this.label1);
            this.gr_Config.Controls.Add(this.tb_MeterNoEnd);
            this.gr_Config.Enabled = false;
            this.gr_Config.Location = new System.Drawing.Point(180, 3);
            this.gr_Config.Name = "gr_Config";
            this.gr_Config.Size = new System.Drawing.Size(596, 184);
            this.gr_Config.TabIndex = 26;
            this.gr_Config.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label2.Location = new System.Drawing.Point(6, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 16);
            this.label2.TabIndex = 26;
            this.label2.Text = "Meter No End:";
            // 
            // cb_MeterType
            // 
            this.cb_MeterType.FormattingEnabled = true;
            this.cb_MeterType.Location = new System.Drawing.Point(103, 117);
            this.cb_MeterType.Name = "cb_MeterType";
            this.cb_MeterType.Size = new System.Drawing.Size(141, 21);
            this.cb_MeterType.TabIndex = 25;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label3.Location = new System.Drawing.Point(6, 118);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 16);
            this.label3.TabIndex = 24;
            this.label3.Text = "Meter Type:";
            // 
            // pr_Load
            // 
            this.pr_Load.Location = new System.Drawing.Point(6, 151);
            this.pr_Load.Name = "pr_Load";
            this.pr_Load.Size = new System.Drawing.Size(442, 27);
            this.pr_Load.TabIndex = 18;
            // 
            // tb_MeterNoStart
            // 
            this.tb_MeterNoStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.tb_MeterNoStart.Location = new System.Drawing.Point(123, 28);
            this.tb_MeterNoStart.MaxLength = 8;
            this.tb_MeterNoStart.Multiline = true;
            this.tb_MeterNoStart.Name = "tb_MeterNoStart";
            this.tb_MeterNoStart.Size = new System.Drawing.Size(92, 28);
            this.tb_MeterNoStart.TabIndex = 16;
            this.tb_MeterNoStart.Text = "00888888";
            this.tb_MeterNoStart.TextChanged += new System.EventHandler(this.tb_MeterNoStart_TextChanged);
            // 
            // b_ClearConsole
            // 
            this.b_ClearConsole.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.b_ClearConsole.Location = new System.Drawing.Point(500, 151);
            this.b_ClearConsole.Name = "b_ClearConsole";
            this.b_ClearConsole.Size = new System.Drawing.Size(90, 27);
            this.b_ClearConsole.TabIndex = 3;
            this.b_ClearConsole.Text = "Clear";
            this.b_ClearConsole.UseVisualStyleBackColor = true;
            this.b_ClearConsole.Click += new System.EventHandler(this.b_ClearConsole_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label1.Location = new System.Drawing.Point(6, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 16);
            this.label1.TabIndex = 17;
            this.label1.Text = "Meter No Start:";
            // 
            // tb_MeterNoEnd
            // 
            this.tb_MeterNoEnd.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.tb_MeterNoEnd.Location = new System.Drawing.Point(123, 68);
            this.tb_MeterNoEnd.MaxLength = 8;
            this.tb_MeterNoEnd.Multiline = true;
            this.tb_MeterNoEnd.Name = "tb_MeterNoEnd";
            this.tb_MeterNoEnd.Size = new System.Drawing.Size(92, 28);
            this.tb_MeterNoEnd.TabIndex = 20;
            this.tb_MeterNoEnd.TextChanged += new System.EventHandler(this.tb_MeterNoEnd_TextChanged);
            // 
            // gr_Buttons
            // 
            this.gr_Buttons.Controls.Add(this.b_LoadFile);
            this.gr_Buttons.Controls.Add(this.b_Stop);
            this.gr_Buttons.Controls.Add(this.b_Start);
            this.gr_Buttons.Enabled = false;
            this.gr_Buttons.Location = new System.Drawing.Point(12, 3);
            this.gr_Buttons.Name = "gr_Buttons";
            this.gr_Buttons.Size = new System.Drawing.Size(162, 184);
            this.gr_Buttons.TabIndex = 24;
            this.gr_Buttons.TabStop = false;
            // 
            // b_LoadFile
            // 
            this.b_LoadFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.b_LoadFile.Location = new System.Drawing.Point(13, 16);
            this.b_LoadFile.Name = "b_LoadFile";
            this.b_LoadFile.Size = new System.Drawing.Size(138, 35);
            this.b_LoadFile.TabIndex = 22;
            this.b_LoadFile.Text = "Load .bin File";
            this.b_LoadFile.UseVisualStyleBackColor = true;
            this.b_LoadFile.Click += new System.EventHandler(this.b_LoadFile_Click);
            // 
            // b_Stop
            // 
            this.b_Stop.Enabled = false;
            this.b_Stop.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.b_Stop.Location = new System.Drawing.Point(13, 117);
            this.b_Stop.Name = "b_Stop";
            this.b_Stop.Size = new System.Drawing.Size(138, 57);
            this.b_Stop.TabIndex = 23;
            this.b_Stop.Text = "Stop";
            this.b_Stop.UseVisualStyleBackColor = true;
            this.b_Stop.Click += new System.EventHandler(this.b_Stop_Click);
            // 
            // b_Start
            // 
            this.b_Start.Enabled = false;
            this.b_Start.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.b_Start.Location = new System.Drawing.Point(13, 57);
            this.b_Start.Name = "b_Start";
            this.b_Start.Size = new System.Drawing.Size(138, 54);
            this.b_Start.TabIndex = 19;
            this.b_Start.Text = "Start";
            this.b_Start.UseVisualStyleBackColor = true;
            this.b_Start.Click += new System.EventHandler(this.b_Start_Click);
            // 
            // Console
            // 
            this.Console.BackColor = System.Drawing.SystemColors.MenuText;
            this.Console.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.Console.ForeColor = System.Drawing.Color.LawnGreen;
            this.Console.Location = new System.Drawing.Point(12, 193);
            this.Console.Name = "Console";
            this.Console.ReadOnly = true;
            this.Console.Size = new System.Drawing.Size(764, 318);
            this.Console.TabIndex = 5;
            this.Console.Text = "";
            // 
            // tsHelpToolbar
            // 
            this.tsHelpToolbar.ClickThrough = true;
            this.tsHelpToolbar.Dock = System.Windows.Forms.DockStyle.None;
            this.tsHelpToolbar.Enabled = false;
            this.tsHelpToolbar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsHelpToolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsBtnShowHelp});
            this.tsHelpToolbar.Location = new System.Drawing.Point(81, 0);
            this.tsHelpToolbar.Name = "tsHelpToolbar";
            this.tsHelpToolbar.Size = new System.Drawing.Size(26, 25);
            this.tsHelpToolbar.SuppressHighlighting = false;
            this.tsHelpToolbar.TabIndex = 3;
            // 
            // tsBtnShowHelp
            // 
            this.tsBtnShowHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsBtnShowHelp.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnShowHelp.Image")));
            this.tsBtnShowHelp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtnShowHelp.Name = "tsBtnShowHelp";
            this.tsBtnShowHelp.Size = new System.Drawing.Size(23, 22);
            this.tsBtnShowHelp.Text = "Help";
            this.tsBtnShowHelp.Click += new System.EventHandler(this.showHelpToolStripMenuItem_Click);
            // 
            // tsActionToolbar
            // 
            this.tsActionToolbar.ClickThrough = true;
            this.tsActionToolbar.Dock = System.Windows.Forms.DockStyle.None;
            this.tsActionToolbar.Enabled = false;
            this.tsActionToolbar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsActionToolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsBtnReset,
            this.toolStripSeparator1,
            this.tsBtnRefresh,
            this.toolStripSeparator2,
            this.tsBtnShowRegisters,
            this.toolStripSeparator4,
            this.toolStripLabel2,
            this.tsBtnMonitorOn,
            this.tsBtnMonitorOff});
            this.tsActionToolbar.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.tsActionToolbar.Location = new System.Drawing.Point(109, 0);
            this.tsActionToolbar.Name = "tsActionToolbar";
            this.tsActionToolbar.Size = new System.Drawing.Size(230, 25);
            this.tsActionToolbar.SuppressHighlighting = false;
            this.tsActionToolbar.TabIndex = 2;
            this.tsActionToolbar.Text = "Action";
            // 
            // tsBtnReset
            // 
            this.tsBtnReset.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsBtnReset.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnReset.Image")));
            this.tsBtnReset.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtnReset.Name = "tsBtnReset";
            this.tsBtnReset.Size = new System.Drawing.Size(39, 22);
            this.tsBtnReset.Text = "Reset";
            this.tsBtnReset.Click += new System.EventHandler(this.resetToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // tsBtnShowRegisters
            // 
            this.tsBtnShowRegisters.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsBtnShowRegisters.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.tsBtnShowRegisters.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnShowRegisters.Image")));
            this.tsBtnShowRegisters.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtnShowRegisters.Name = "tsBtnShowRegisters";
            this.tsBtnShowRegisters.Size = new System.Drawing.Size(33, 22);
            this.tsBtnShowRegisters.Text = "Reg";
            this.tsBtnShowRegisters.ToolTipText = "Displays SX1231 raw registers window";
            this.tsBtnShowRegisters.Click += new System.EventHandler(this.showRegistersToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(53, 22);
            this.toolStripLabel2.Text = "Monitor:";
            // 
            // tsBtnMonitorOn
            // 
            this.tsBtnMonitorOn.Checked = true;
            this.tsBtnMonitorOn.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsBtnMonitorOn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsBtnMonitorOn.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnMonitorOn.Image")));
            this.tsBtnMonitorOn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtnMonitorOn.Name = "tsBtnMonitorOn";
            this.tsBtnMonitorOn.Size = new System.Drawing.Size(29, 22);
            this.tsBtnMonitorOn.Text = "ON";
            this.tsBtnMonitorOn.ToolTipText = "Enables the SX1231 monitor mode";
            this.tsBtnMonitorOn.Click += new System.EventHandler(this.monitorToolStripMenuItem_Click);
            // 
            // tsBtnMonitorOff
            // 
            this.tsBtnMonitorOff.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsBtnMonitorOff.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnMonitorOff.Image")));
            this.tsBtnMonitorOff.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtnMonitorOff.Name = "tsBtnMonitorOff";
            this.tsBtnMonitorOff.Size = new System.Drawing.Size(32, 22);
            this.tsBtnMonitorOff.Text = "OFF";
            this.tsBtnMonitorOff.ToolTipText = "Disables the SX1231 monitor mode";
            this.tsBtnMonitorOff.Click += new System.EventHandler(this.monitorToolStripMenuItem_Click);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(62, 22);
            this.toolStripLabel1.Text = "Product ID:";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(236, 34);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(98, 17);
            this.checkBox1.TabIndex = 27;
            this.checkBox1.Text = "Continue Mode";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // lb_percent
            // 
            this.lb_percent.AutoSize = true;
            this.lb_percent.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lb_percent.Location = new System.Drawing.Point(454, 157);
            this.lb_percent.Name = "lb_percent";
            this.lb_percent.Size = new System.Drawing.Size(33, 16);
            this.lb_percent.TabIndex = 28;
            this.lb_percent.Text = "% 0";
            // 
            // pr_transmid
            // 
            this.pr_transmid.Location = new System.Drawing.Point(348, 134);
            this.pr_transmid.Name = "pr_transmid";
            this.pr_transmid.Size = new System.Drawing.Size(100, 11);
            this.pr_transmid.TabIndex = 29;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 596);
            this.Controls.Add(this.toolStripContainer1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.msMainMenu;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "SX1232 Baylan BootLoader Kit V1.0";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Mainform_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Mainform_KeyDown);
            this.ssMainStatus.ResumeLayout(false);
            this.ssMainStatus.PerformLayout();
            this.msMainMenu.ResumeLayout(false);
            this.msMainMenu.PerformLayout();
            this.tsMainToolbar.ResumeLayout(false);
            this.tsMainToolbar.PerformLayout();
            this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.ContentPanel.PerformLayout();
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.gr_Config.ResumeLayout(false);
            this.gr_Config.PerformLayout();
            this.gr_Buttons.ResumeLayout(false);
            this.tsHelpToolbar.ResumeLayout(false);
            this.tsHelpToolbar.PerformLayout();
            this.tsActionToolbar.ResumeLayout(false);
            this.tsActionToolbar.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		private bool IsFormLocatedInScreen(Form frm, Screen[] screens)
		{
			int upperBound = screens.GetUpperBound(0);
			bool flag = false;
			for (int i = 0; i <= upperBound; i++)
			{
				if (frm.Left < screens[i].WorkingArea.Left
				|| frm.Top < screens[i].WorkingArea.Top
				|| frm.Left > screens[i].WorkingArea.Right
				|| frm.Top > screens[i].WorkingArea.Bottom
					)
					flag = false;
				else
					return true;
			}
			return flag;
		}

		private void loadToolStripMenuItem_Click(object sender, EventArgs e)
		{
			base.Validate();
			tsLblStatus.Text = "-";
			try
			{
				ofConfigFileOpenDlg.InitialDirectory = configFilePath;
				ofConfigFileOpenDlg.FileName = configFileName;
				if (ofConfigFileOpenDlg.ShowDialog() == DialogResult.OK)
				{
					string[] strArray = ofConfigFileOpenDlg.FileName.Split(new char[] { '\\' });
					configFileName = strArray[strArray.Length - 1];
					configFilePath = "";
					int index = 0;
					while (index < (strArray.Length - 2))
					{
						configFilePath = configFilePath + strArray[index] + @"\";
						index++;
					}
					configFilePath = configFilePath + strArray[index];
					configFileStream = new FileStream(configFilePath + @"\" + configFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
					sx1231.Open(ref configFileStream);
					isConfigFileOpen = true;
					tsLblConfigFileName.Text = "Config File: " + configFileName;
					saveToolStripMenuItem.Text = "Save Config \"" + configFileName + "\"";
				}
				else
					isConfigFileOpen = false;
			}
			catch (Exception exception)
			{
				isConfigFileOpen = false;
				tsLblStatus.Text = "ERROR: " + exception.Message;
			}
		}

		private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			try
			{
				appSettings.SetValue("Top", Top.ToString());
				appSettings.SetValue("Left", Left.ToString());
				appSettings.SetValue("ConfigFilePath", configFilePath);
				appSettings.SetValue("ConfigFileName", configFileName);
			}
			catch (Exception exception)
			{
				tsLblStatus.Text = "ERROR: " + exception.Message;
				Refresh();
			}
		}

		private void Mainform_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (sx1231 != null)
				sx1231.Close();
		}

		private void Mainform_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				e.Handled = true;
				SendKeys.Send("{TAB}");
			}
			else if (e.KeyData == (Keys.Alt | Keys.Control | Keys.N))
			{
				#region Connect/Disconnect 
				if (tsBtnOpenDevice.Text == "Connect")
				{
					//sx1231ViewControl.Enabled = !sx1231ViewControl.Enabled;
					//if (sx1231ViewControl.Enabled)
					{
						sx1231.ReadRegisters();
						tsBtnOpenFile.Enabled = true;
						loadToolStripMenuItem.Enabled = true;
						tsBtnSaveFile.Enabled = true;
						saveToolStripMenuItem.Enabled = true;
						saveAsToolStripMenuItem.Enabled = true;
						resetToolStripMenuItem.Enabled = false;
						refreshToolStripMenuItem.Enabled = false;
						showRegistersToolStripMenuItem.Enabled = true;
						monitorToolStripMenuItem.Enabled = false;
						tsActionToolbar.Enabled = true;
						tsBtnReset.Enabled = false;
						tsBtnRefresh.Enabled = false;
						tsBtnShowRegisters.Enabled = true;
						toolStripLabel2.Enabled = false;
						tsBtnMonitorOff.Enabled = false;
						tsBtnMonitorOn.Enabled = false;
					}
					//else
					{
						tsBtnOpenFile.Enabled = false;
						loadToolStripMenuItem.Enabled = false;
						tsBtnSaveFile.Enabled = false;
						saveToolStripMenuItem.Enabled = false;
						saveAsToolStripMenuItem.Enabled = false;
						resetToolStripMenuItem.Enabled = false;
						refreshToolStripMenuItem.Enabled = false;
						showRegistersToolStripMenuItem.Enabled = false;
						monitorToolStripMenuItem.Enabled = false;
						tsActionToolbar.Enabled = false;
						tsBtnReset.Enabled = true;
						tsBtnRefresh.Enabled = true;
						tsBtnShowRegisters.Enabled = true;
						toolStripLabel2.Enabled = true;
						tsBtnMonitorOff.Enabled = true;
						tsBtnMonitorOn.Enabled = true;
					}
				}
				#endregion
			}
			else if (e.KeyData == (Keys.Alt | Keys.Control | Keys.T))
			{
				#region Test Mode Window 
				if (frmTest == null)
				{
					frmTest = new TestForm();
					frmTest.FormClosed += new FormClosedEventHandler(frmTest_FormClosed);
					frmTest.Disposed += new EventHandler(frmTest_Disposed);
					frmTest.SX1231 = sx1231;
					frmTest.TestEnabled = false;
				}
				if (!frmTest.TestEnabled)
				{
					frmTest.TestEnabled = true;
					Point point = new Point();
					point.X = (Location.X + (Width / 2)) - (frmTest.Width / 2);
					point.Y = (Location.Y + (Height / 2)) - (frmTest.Height / 2);
					frmTest.Location = point;
					frmTest.Show();
				}
				else
				{
					frmTest.TestEnabled = false;
					frmTest.Hide();
				}
				#endregion
			}
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			try
			{
				string value = appSettings.GetValue("Top");
				if (value != null)
					try
					{
						Top = int.Parse(value);
					}
					catch
					{
						MessageBox.Show(this, "Error getting Top value.");
					}
				value = appSettings.GetValue("Left");
				if (value != null)
					try
					{
						Left = int.Parse(value);
					}
					catch
					{
						MessageBox.Show(this, "Error getting Left value.");
					}
				
				Screen[] allScreens = Screen.AllScreens;
				if (!IsFormLocatedInScreen(this, allScreens))
				{
					Top = allScreens[0].WorkingArea.Top;
					Left = allScreens[0].WorkingArea.Left;
				}
				value = appSettings.GetValue("ConfigFilePath");
				if (value == null)
				{
					value = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
					appSettings.SetValue("ConfigFilePath", value);
				}
				configFilePath = value;
				value = appSettings.GetValue("ConfigFileName");
				if (value == null)
				{
					value = "sx1231skb.cfg";
					appSettings.SetValue("ConfigFileName", value);
				}
				configFileName = value;
				tsLblConfigFileName.Text = "Config File: -";

				sx1231.Error += new SX1231.ErrorEventHandler(sx1231_Error);
				sx1231.Connected += new EventHandler(sx1231_Connected);
                sx1231.Connected += new EventHandler(Device_Connected);
                sx1231.Disconected += new EventHandler(Device_Disconnected);
				sx1231.Disconected += new EventHandler(sx1231_Disconected);
				sx1231.PropertyChanged += new PropertyChangedEventHandler(sx1231_PropertyChanged);
				sx1231.PacketHandlerStarted += new EventHandler(sx1231_PacketHandlerStarted);
				sx1231.PacketHandlerStoped += new EventHandler(sx1231_PacketHandlerStoped);     

				//sx1231ViewControl.SX1231 = sx1231;
				tsBtnOpenDevice_Click(tsBtnOpenDevice, EventArgs.Empty);
			}
			catch (Exception exception)
			{
				tsLblStatus.Text = "ERROR: " + exception.Message;
			}
		}

		private void monitorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				if ((sender == monitorOffToolStripMenuItem) || (sender == tsBtnMonitorOff))
					sx1231.Monitor = false;
				else
					sx1231.Monitor = true;
			}
			catch (Exception exception)
			{
				OnError(1, exception.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void OnConnected()
		{
			try
			{
				tsBtnOpenDevice.Text = "Disconnect";
				tsBtnOpenDevice.Image = Resources.Connected;

				connectToolStripMenuItem.Text = "Disconnect";
				connectToolStripMenuItem.Image = Resources.Connected;

				sx1231.Reset();

				tsBtnOpenFile.Enabled = true;
				tsBtnSaveFile.Enabled = true;
				tsActionToolbar.Enabled = true;
				tsBtnReset.Enabled = true;
				tsBtnRefresh.Enabled = true;
				tsBtnShowRegisters.Enabled = true;
				toolStripLabel2.Enabled = true;
				tsBtnMonitorOff.Enabled = true;
				tsBtnMonitorOn.Enabled = true;
				tsHelpToolbar.Enabled = true;
				loadToolStripMenuItem.Enabled = true;
				saveToolStripMenuItem.Enabled = true;
				saveAsToolStripMenuItem.Enabled = true;
				resetToolStripMenuItem.Enabled = true;
				refreshToolStripMenuItem.Enabled = true;
				showRegistersToolStripMenuItem.Enabled = true;
				monitorToolStripMenuItem.Enabled = true;
				//rssiAnalyserToolStripMenuItem.Enabled = true;
				//spectrumAnalyserToolStripMenuItem.Enabled = true;
				showHelpToolStripMenuItem.Enabled = true;
				//sx1231ViewControl.Enabled = true;

				if (frmTest != null)
					frmTest.SX1231 = sx1231;

				if (frmRegisters != null)
					frmRegisters.SX1231 = sx1231;
			}
			catch (Exception exception)
			{
				OnError(1, exception.Message);
			}
		}

		private void OnDisconnected()
		{
			try
			{
				tsBtnOpenDevice.Text = "Connect";
				tsBtnOpenDevice.Image = Resources.Disconnected;

				connectToolStripMenuItem.Text = "Connect";
				connectToolStripMenuItem.Image = Resources.Disconnected;

				tsBtnOpenFile.Enabled = false;
				tsBtnSaveFile.Enabled = false;
				tsActionToolbar.Enabled = false;
				tsHelpToolbar.Enabled = false;
				loadToolStripMenuItem.Enabled = false;
				saveToolStripMenuItem.Enabled = false;
				saveAsToolStripMenuItem.Enabled = false;
				resetToolStripMenuItem.Enabled = false;
				refreshToolStripMenuItem.Enabled = false;
				showRegistersToolStripMenuItem.Enabled = false;
				monitorToolStripMenuItem.Enabled = false;
				//rssiAnalyserToolStripMenuItem.Enabled = false;
				//spectrumAnalyserToolStripMenuItem.Enabled = false;
				showHelpToolStripMenuItem.Enabled = false;
				//sx1231ViewControl.Enabled = false;

				if (frmTest != null)
					frmTest.Close();
				if (frmRegisters != null)
					frmRegisters.Close();
				if (frmRssiAnalyser != null)
					frmRssiAnalyser.Close();
				if (frmSpectrumAnalyser != null)
					frmSpectrumAnalyser.Close();
			}
			catch (Exception exception)
			{
				OnError(1, exception.Message);
			}
		}

		private void OnError(byte status, string message)
		{
			if (status != 0)
				tsLblStatus.Text = "ERROR: " + message;
			else
				tsLblStatus.Text = message;
			Refresh();
		}

		private void OnSX1231PacketHandlerStarted(object sender, EventArgs e)
		{
			DisableControls();
		}

		private void OnSX1231PacketHandlerStoped(object sender, EventArgs e)
		{
			EnableControls();
		}

		private void OnSX1231PorpertyChanged(object sender, PropertyChangedEventArgs e)
		{
			string propertyName = e.PropertyName;
			switch (propertyName)
			{
				case "LogEnabled":
					if (!sx1231.Packet.LogEnabled)
					{
						if (frmPacketLog != null)
							frmPacketLog.Close();
					}
					else
					{
						if (frmPacketLog != null)
							frmPacketLog.Close();

						if (frmPacketLog == null)
						{
							frmPacketLog = new PacketLogForm();
							frmPacketLog.FormClosed += new FormClosedEventHandler(frmPacketLog_FormClosed);
							frmPacketLog.Disposed += new EventHandler(frmPacketLog_Disposed);
							frmPacketLog.SX1231 = sx1231;
							frmPacketLog.AppSettings = appSettings;
						}
						frmPacketLog.Show();
					}
					break;
				case "Monitor":
					if (sx1231.Monitor)
					{
						monitorOffToolStripMenuItem.Checked = false;
						tsBtnMonitorOff.Checked = false;
						monitorOnToolStripMenuItem.Checked = true;
						tsBtnMonitorOn.Checked = true;
					}
					else
					{
						monitorOffToolStripMenuItem.Checked = true;
						tsBtnMonitorOff.Checked = true;
						monitorOnToolStripMenuItem.Checked = false;
						tsBtnMonitorOn.Checked = false;
					}
					break;
				case "Version":
					tsLblChipVersion.Text = "Chip version: " + sx1231.Version;
					break;

				case "SpectrumOn":
					if (sx1231.SpectrumOn)
						DisableControls();
					else
						EnableControls();
					break;
			}
		}

		private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				sx1231.ReadRegisters();
			}
			catch (Exception exception)
			{
				OnError(1, exception.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void resetToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				sx1231.Reset();
			}
			catch (Exception exception)
			{
				OnError(1, exception.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		/*private void rssiAnalyserToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (rssiAnalyserToolStripMenuItem.Checked)
			{
				if (frmRssiAnalyser != null)
					frmRssiAnalyser.Close();

				rssiAnalyserToolStripMenuItem.Checked = false;
			}
			else
			{
				if (frmSpectrumAnalyser != null)
					frmSpectrumAnalyser.Close();

				if (frmRssiAnalyser == null)
				{
					frmRssiAnalyser = new RssiAnalyserForm();
					frmRssiAnalyser.FormClosed += new FormClosedEventHandler(frmRssiAnalyser_FormClosed);
					frmRssiAnalyser.Disposed += new EventHandler(frmRssiAnalyser_Disposed);
					frmRssiAnalyser.SX1231 = sx1231;
					frmRssiAnalyser.AppSettings = appSettings;
				}
				frmRssiAnalyser.Show();
				rssiAnalyserToolStripMenuItem.Checked = true;
			}
		}*/

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			base.Validate();
			try
			{
				sfConfigFileSaveDlg.InitialDirectory = configFilePath;
				sfConfigFileSaveDlg.FileName = configFileName;
				if (sfConfigFileSaveDlg.ShowDialog() == DialogResult.OK)
				{
					string[] strArray = sfConfigFileSaveDlg.FileName.Split(new char[] { '\\' });
					configFileName = strArray[strArray.Length - 1];
					configFilePath = "";
					int index = 0;
					while (index < (strArray.Length - 2))
					{
						configFilePath = configFilePath + strArray[index] + @"\";
						index++;
					}
					configFilePath = configFilePath + strArray[index];
					configFileStream = new FileStream(configFilePath + @"\" + configFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
					tsLblConfigFileName.Text = "Config File: " + configFileName;
					saveToolStripMenuItem.Text = "Save Config \"" + configFileName + "\"";
					sx1231.Save(ref configFileStream);
					isConfigFileOpen = true;
				}
			}
			catch (Exception exception)
			{
				tsLblStatus.Text = "ERROR: " + exception.Message;
				isConfigFileOpen = false;
			}
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			base.Validate();
			try
			{
				if (isConfigFileOpen)
				{
					if (MessageBox.Show("Do you want to overwrite the current config file?", "Save", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
					{
						configFileStream = new FileStream(configFilePath + @"\" + configFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
						sx1231.Save(ref configFileStream);
					}
				}
				else
					saveAsToolStripMenuItem_Click(sender, e);
			}
			catch (Exception exception)
			{
				tsLblStatus.Text = "ERROR: " + exception.Message;
			}
		}

		private void showHelpToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (showHelpToolStripMenuItem.Checked || tsBtnShowHelp.Checked)
			{
				showHelpToolStripMenuItem.Checked = false;
				tsBtnShowHelp.Checked = false;
				if (frmHelp != null)
					frmHelp.Hide();
			}
			else
			{
				showHelpToolStripMenuItem.Checked = true;
				tsBtnShowHelp.Checked = true;
				if (frmHelp == null)
				{
					frmHelp = new HelpForm();
					frmHelp.FormClosed += new FormClosedEventHandler(frmHelp_FormClosed);
					frmHelp.Disposed += new EventHandler(frmHelp_Disposed);
				}
				Point point = new Point();
				point.X = Location.X + Width;
				point.Y = Location.Y;
				frmHelp.Location = point;
				frmHelp.Show();
			}
		}

		private void showRegistersToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (showRegistersToolStripMenuItem.Checked || tsBtnShowRegisters.Checked)
			{
				showRegistersToolStripMenuItem.Checked = false;
				tsBtnShowRegisters.Checked = false;
				if (frmRegisters != null)
					frmRegisters.Hide();
				if (frmSpectrumAnalyser != null)
					frmRegisters.RegistersFormEnabled = true;
			}
			else
			{
				showRegistersToolStripMenuItem.Checked = true;
				tsBtnShowRegisters.Checked = true;
				if (frmRegisters == null)
				{
					frmRegisters = new RegistersForm();
					frmRegisters.FormClosed += new FormClosedEventHandler(frmRegisters_FormClosed);
					frmRegisters.Disposed += new EventHandler(frmRegisters_Disposed);
					frmRegisters.SX1231 = sx1231;
					frmRegisters.AppSettings = appSettings;
					if (frmSpectrumAnalyser != null)
						frmRegisters.RegistersFormEnabled = false;
				}
				frmRegisters.Show();
			}
		}

		/*private void spectrumAnalyserToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (spectrumAnalyserToolStripMenuItem.Checked)
			{
				if (frmSpectrumAnalyser != null)
					frmSpectrumAnalyser.Close();

				spectrumAnalyserToolStripMenuItem.Checked = false;
			}
			else
			{
				if (frmRssiAnalyser != null)
					frmRssiAnalyser.Close();

				if (frmSpectrumAnalyser == null)
				{
					frmSpectrumAnalyser = new SpectrumAnalyserForm();
					frmSpectrumAnalyser.FormClosed += new FormClosedEventHandler(frmSpectrumAnalyser_FormClosed);
					frmSpectrumAnalyser.Disposed += new EventHandler(frmSpectrumAnalyser_Disposed);
					frmSpectrumAnalyser.SX1231 = sx1231;
					frmSpectrumAnalyser.AppSettings = appSettings;
				}
				frmSpectrumAnalyser.Show();
				spectrumAnalyserToolStripMenuItem.Checked = true;
			}
		}*/

		private void sx1231_Connected(object sender, EventArgs e)
		{
			if (InvokeRequired)
				BeginInvoke(new ConnectedDelegate(OnConnected), null);
			else
				OnConnected();
		}

		private void sx1231_Disconected(object sender, EventArgs e)
		{
			if (InvokeRequired)
				BeginInvoke(new DisconnectedDelegate(OnDisconnected), null);
			else
				OnDisconnected();
		}

		private void sx1231_Error(object sender, SemtechLib.General.Events.ErrorEventArgs e)
		{
			if (InvokeRequired)
				BeginInvoke(new ErrorDelegate(OnError), new object[] { e.Status, e.Message });
			else
				OnError(e.Status, e.Message);
		}

		private void sx1231_PacketHandlerStarted(object sender, EventArgs e)
		{
			if (InvokeRequired)
				BeginInvoke(new SX1231PacketHandlerStartedDelegate(OnSX1231PacketHandlerStarted), new object[] { sender, e });
			else
				OnSX1231PacketHandlerStarted(sender, e);
		}

		private void sx1231_PacketHandlerStoped(object sender, EventArgs e)
		{
			if (InvokeRequired)
				BeginInvoke(new SX1231PacketHandlerStopedDelegate(OnSX1231PacketHandlerStoped), new object[] { sender, e });
			else
				OnSX1231PacketHandlerStoped(sender, e);
		}

		private void sx1231_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (InvokeRequired)
				BeginInvoke(new SX1231DataChangedDelegate(OnSX1231PorpertyChanged), new object[] { sender, e });
			else
				OnSX1231PorpertyChanged(sender, e);
		}

		private void sx1231ViewControl_DocumentationChanged(object sender, DocumentationChangedEventArgs e)
		{
			if (frmHelp != null)
				frmHelp.UpdateDocument(e);
		}

		private void sx1231ViewControl_Error(object sender, SemtechLib.General.Events.ErrorEventArgs e)
		{
			if (InvokeRequired)
				BeginInvoke(new ErrorDelegate(OnError), new object[] { e.Status, e.Message });
			else
				OnError(e.Status, e.Message);
		}

		private void tsBtnOpenDevice_Click(object sender, EventArgs e)
		{
			Cursor = Cursors.WaitCursor;

			tsBtnOpenDevice.Enabled = false;
			connectToolStripMenuItem.Enabled = false;
			Refresh();

			tsLblStatus.Text = "-";
			Refresh();

			try
			{
				if (tsBtnOpenDevice.Text == "Connect")
				{
                    if (!sx1231.Open("Dual RS232")
                    && !sx1231.Open("SX1231SKB-915")
                    && !sx1231.Open("SX1231SKB")
                    && !sx1231.Open("Semtech USB bridge")
                    && !sx1231.Open("USB <-> Dual Serial")
                        )
                        throw new Exception("Unable to open SX1231 " + sx1231.DeviceName + " device");
				}
				else if (sx1231 != null)
					sx1231.Close();
			}
			catch (Exception exception)
			{
				tsBtnOpenDevice.Text = "Connect";
				tsBtnOpenDevice.Image = Resources.Disconnected;
				connectToolStripMenuItem.Text = "Connect";
				connectToolStripMenuItem.Image = Resources.Disconnected;

				if (sx1231 != null)
					sx1231.Close();

				sx1231.ReadRegisters();
				tsLblStatus.Text = "ERROR: " + exception.Message;
				Refresh();
			}
			finally
			{
				tsBtnOpenDevice.Enabled = true;
				connectToolStripMenuItem.Enabled = true;
				Cursor = Cursors.Default;
			}
		}

		private void usersGuideToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (File.Exists(Application.StartupPath + @"\SX1231SKB_usersguide.pdf"))
				Process.Start(Application.StartupPath + @"\SX1231SKB_usersguide.pdf");
			else
				MessageBox.Show("Unable to find the user's guide document!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}

		public bool AppTestArg
		{
			get { return appTestArg; }
		}

		public string AssemblyTitle
		{
			get
			{
				object[] customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
				if (customAttributes.Length > 0)
				{
					AssemblyTitleAttribute attribute = (AssemblyTitleAttribute)customAttributes[0];
					if (attribute.Title != "")
						return attribute.Title;
				}
				return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
			}
		}

		public string AssemblyVersion
		{
			get
			{
				AssemblyName name = Assembly.GetExecutingAssembly().GetName();
				if (name.Version.ToString() != "")
					return name.Version.ToString();
				return "-.-.-.-";
			}
		}


        // CRC //////////////////////////////////////////////////////

        public class Crc32 : HashAlgorithm
        {
            public const UInt32 DefaultPolynomial = 0xedb88320;
            public const UInt32 DefaultSeed = 0xffffffff;

            private UInt32 hash;
            private UInt32 seed;
            private UInt32[] table;
            private static UInt32[] defaultTable;

            public Crc32()
            {
                table = InitializeTable(DefaultPolynomial);
                seed = DefaultSeed;
                Initialize();
            }

            public Crc32(UInt32 polynomial, UInt32 seed)
            {
                table = InitializeTable(polynomial);
                this.seed = seed;
                Initialize();
            }

            public override void Initialize()
            {
                hash = seed;
            }

            protected override void HashCore(byte[] buffer, int start, int length)
            {
                hash = CalculateHash(table, hash, buffer, start, length);
            }

            protected override byte[] HashFinal()
            {
                byte[] hashBuffer = UInt32ToBigEndianBytes(~hash);
                this.HashValue = hashBuffer;
                return hashBuffer;
            }

            public override int HashSize
            {
                get { return 32; }
            }

            public static UInt32 Compute(byte[] buffer)
            {
                return ~CalculateHash(InitializeTable(DefaultPolynomial), DefaultSeed, buffer, 0, buffer.Length);
            }

            public static UInt32 Compute(UInt32 seed, byte[] buffer)
            {
                return ~CalculateHash(InitializeTable(DefaultPolynomial), seed, buffer, 0, buffer.Length);
            }

            public static UInt32 Compute(UInt32 polynomial, UInt32 seed, byte[] buffer)
            {
                return ~CalculateHash(InitializeTable(polynomial), seed, buffer, 0, buffer.Length);
            }

            private static UInt32[] InitializeTable(UInt32 polynomial)
            {
                if (polynomial == DefaultPolynomial && defaultTable != null)
                    return defaultTable;

                UInt32[] createTable = new UInt32[256];
                for (int i = 0; i < 256; i++)
                {
                    UInt32 entry = (UInt32)i;
                    for (int j = 0; j < 8; j++)
                        if ((entry & 1) == 1)
                            entry = (entry >> 1) ^ polynomial;
                        else
                            entry = entry >> 1;
                    createTable[i] = entry;
                }

                if (polynomial == DefaultPolynomial)
                    defaultTable = createTable;

                return createTable;
            }

            private static UInt32 CalculateHash(UInt32[] table, UInt32 seed, byte[] buffer, int start, int size)
            {
                UInt32 crc = seed;
                for (int i = start; i < size; i++)
                    unchecked
                    {
                        crc = (crc >> 8) ^ table[buffer[i] ^ crc & 0xff];
                    }
                return crc;
            }

            private byte[] UInt32ToBigEndianBytes(UInt32 x)
            {
                return new byte[] {
			            (byte)((x >> 24) & 0xff),
			            (byte)((x >> 16) & 0xff),
			            (byte)((x >> 8) & 0xff),
			            (byte)(x & 0xff)
		            };
            }
        }
        /////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////
        ////////////////////// COSKUN ERGAN /////////////////////////
        /////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////
        byte[] BinFile = new byte[512000];
        long FileSize;
        int PacketIndex = 0;
        long FileIndex = 0;
        byte[] Crc_Buff = new byte[4];
        [DllImport("msvcrt.dll", CallingConvention=CallingConvention.Cdecl)]
        static extern int memcmp(byte[] b1, byte[] b2, long count);
        bool Rx_Packet_Done_Flag=false;
        bool Tx_Packet_Done_Flag = false;
        byte[] Meter_No_Buffer = new byte[4];
        int[] MeterList = new int[1000];
        int MeterListSize;
        int MeterNoStartInt;
        int MeterNoEndInt;
        int FinishTotal=0;
        int WaitTotal = 0;
        int Procces_MeterIndex;
        /*********************************************************/
        /*********************************************************/
        /*********************************************************/
        private void Device_Disconnected(object sender, EventArgs e)
        {
            gr_Config.Enabled = false;
            gr_Buttons.Enabled = false;
            Console.Clear();
        }
        private void Device_Connected(object sender, EventArgs e)
        {
            gr_Config.Enabled = true;
            gr_Buttons.Enabled = true;

            cb_MeterType.Items.Add("TK_AMR_LORA L451");
            cb_MeterType.Items.Add("TK_AMR_LORA L151");
            cb_MeterType.Items.Add("TK_1C_LORA L151");
            cb_MeterType.Items.Add("TK_1C_LORA L451");
            cb_MeterType.Items.Add("AK311 AK411 L151");
            cb_MeterType.Items.Add("AK311 L451");
            cb_MeterType.SelectedIndex = 0;
            pr_transmid.Maximum = 20;            
            sx1231.PacketHandlerTransmitted+= new SemtechLib.Devices.SX1231.SX1231.PacketHandlerTransmittedEventHandler(Radio_Transmit_Done);
            sx1231.PacketHandlerReceived += new SemtechLib.Devices.SX1231.SX1231.PacketHandlerReceivedEventHandler(Radio_Receive_Done);

             tb_MeterNoStart_TextChanged(null, null);
        }
        /*********************************************************/
        private void b_ClearConsole_Click(object sender, EventArgs e)
        {
            Console.Clear();
        }
        /*********************************************************/
        private void b_LoadFile_Click(object sender, EventArgs e)
        {
            byte[] buff = new byte[64];

            openFileDialog1.FileName = "*.bin";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {

                Console.Text += Timestamp_String() + openFileDialog1.FileName;                
                b_Start.Enabled = true;
                b_Start.Enabled = true;

                FileStream fs = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read);
                BinaryReader r = new BinaryReader(fs);
                r.Read(BinFile, 0, 512000);
                FileSize = (long)fs.Length;
                r.Close();

                Console.Text += "(";
                pr_Load.Maximum = (int)((FileSize + 200) / 100);                
                Console.Text += FileSize.ToString();
                Console.Text += " Byte) ";

                Crc32 crc32 = new Crc32();
                String hash = String.Empty;

                fs = File.Open(openFileDialog1.FileName, FileMode.Open);

                foreach (byte b in crc32.ComputeHash(fs))

                    hash += b.ToString("x2").ToLower();

                fs.Close();

                char[] temp;
                int temp_byte;
                int j = 7;

                temp = hash.ToCharArray();

                for (int i = 0; i < 4; i++) // CRC 
                {
                    Crc_Buff[i] = Convert.ToByte(temp[j--]);

                    if (Crc_Buff[i] > 0x60) Crc_Buff[i] -= 87; // kucuk harf ise
                    else if (Crc_Buff[i] > 0x40) Crc_Buff[i] -= 55; // buyuk harf ise
                    else Crc_Buff[i] -= 48;                         // sayi ise
                    temp_byte = Convert.ToByte(temp[j--]);

                    if (temp_byte > 0x60) temp_byte -= 87; // kucuk harf ise
                    else if (temp_byte > 0x40) temp_byte -= 55; // buyuk harf ise
                    else temp_byte -= 48;                       // sayi ise
                    Crc_Buff[i] |= (byte)(temp_byte << 4);
                }

                Console.Text += "CRC32: 0x";
                Console.Text += Convert.ToString(Crc_Buff[3], 16);
                Console.Text += Convert.ToString(Crc_Buff[2], 16);
                Console.Text += Convert.ToString(Crc_Buff[1], 16);
                Console.Text += Convert.ToString(Crc_Buff[0], 16);
                Console.Text += "\n";

                b_Start.Enabled = true;                
            }
            Console.SelectionStart = Console.Text.Length;
            Console.ScrollToCaret();
        }
        /*********************************************************/
        private string Timestamp_String()
        {
            string str =  DateTime.Now.Hour.ToString("D2") + ":" +
                         DateTime.Now.Minute.ToString("D2") + ":" +
                         DateTime.Now.Second.ToString("D2") + "." +
                         DateTime.Now.Millisecond.ToString("D2") + "->";
            Console.SelectionStart = Console.Text.Length;
            Console.ScrollToCaret();
            return str;
        }
        /*********************************************************/
        private int Convert_ToDec(byte[] buff)
        {
            int value;
            value = buff[0]&0xF;
            value += (buff[0]>>4)*10;
            value += (buff[1]&0xF) *100;
            value += (buff[1] >> 4) * 1000;
            value += (buff[2]&0xF) *10000;
            value += (buff[2] >> 4) * 100000;
            value += (buff[3]&0xF)*1000000;
            value += (buff[3] >> 4) * 10000000;
            return value;
        }
        /*********************************************************/
        private int Convert_ToDecSwap(byte[] buff)
        {
            int value;
            value = buff[3] & 0xF;
            value += (buff[3] >> 4) * 10;
            value += (buff[2] & 0xF) * 100;
            value += (buff[2] >> 4) * 1000;
            value += (buff[1] & 0xF) * 10000;
            value += (buff[1] >> 4) * 100000;
            value += (buff[0] & 0xF) * 1000000;
            value += (buff[0] >> 4) * 10000000;
            return value;
        }
        /*********************************************************/
        private void Convert_ToBCDByteArray(int value, ref byte[] buff)
        {
            buff[0] = (byte)(value % 10);
            buff[0] += (byte)(((value / 10) % 10) << 4);
            buff[1] = (byte)((value / 100) % 10);
            buff[1] += (byte)(((value / 1000) % 10) << 4);
            buff[2] = (byte)((value / 10000) % 10);
            buff[2] += (byte)(((value / 100000) % 10) << 4);
            buff[3] = (byte)((value / 1000000) % 10);
            buff[3] += (byte)(((value / 10000000) % 10) << 4);
        }
        /*********************************************************/
        private void b_Start_Click(object sender, EventArgs e)
        {
            byte[] Handshake_Packet = new byte[8];
            byte[] Wait_Packet = new byte[8];  

            b_Stop.Enabled = true;
            b_LoadFile.Enabled = false;
            b_Start.Enabled = false;
            gr_Config.Enabled = false;

            Console.Text += Timestamp_String() + "Start. Waiting Meter.\n";

            tb_MeterNoEnd_TextChanged(null, null);

            if (cb_MeterType.SelectedIndex == 0) // TK_AMR_LORA L451
            {
                Wait_Packet[4] = (byte)'b';
                Wait_Packet[5] = (byte)'o';
                Wait_Packet[6] = (byte)'o';
                Wait_Packet[7] = (byte)'t';
                Handshake_Packet[4] = (byte)'o';
                Handshake_Packet[5] = (byte)'k';
                Handshake_Packet[6] = (byte)'e';
                Handshake_Packet[7] = (byte)'y';
            }
            if (cb_MeterType.SelectedIndex == 1) // TK_AMR_LORA L151
            {
                Wait_Packet[4] = (byte)'B';
                Wait_Packet[5] = (byte)'O';
                Wait_Packet[6] = (byte)'O';
                Wait_Packet[7] = (byte)'T';
                Handshake_Packet[4] = (byte)'O';
                Handshake_Packet[5] = (byte)'K';
                Handshake_Packet[6] = (byte)'E';
                Handshake_Packet[7] = (byte)'Y';
            }
            if (cb_MeterType.SelectedIndex == 2) // TK_1C_LORA L151
            {
                Wait_Packet[4] = (byte)'B';
                Wait_Packet[5] = (byte)'O';
                Wait_Packet[6] = (byte)'O';
                Wait_Packet[7] = (byte)'T';
                Handshake_Packet[4] = (byte)'O';
                Handshake_Packet[5] = (byte)'K';
                Handshake_Packet[6] = (byte)'E';
                Handshake_Packet[7] = (byte)'Y';
            }
            if (cb_MeterType.SelectedIndex == 3) // TK_1C_LORA L451
            {
                Wait_Packet[4] = (byte)'b';
                Wait_Packet[5] = (byte)'o';
                Wait_Packet[6] = (byte)'o';
                Wait_Packet[7] = (byte)'t';
                Handshake_Packet[4] = (byte)'o';
                Handshake_Packet[5] = (byte)'k';
                Handshake_Packet[6] = (byte)'e';
                Handshake_Packet[7] = (byte)'y';
            }
            if (cb_MeterType.SelectedIndex == 4) // AK311 AK411 L151
            {
                Wait_Packet[4] = (byte)'B';
                Wait_Packet[5] = (byte)'O';
                Wait_Packet[6] = (byte)'O';
                Wait_Packet[7] = (byte)'T';
                Handshake_Packet[4] = (byte)'O';
                Handshake_Packet[5] = (byte)'K';
                Handshake_Packet[6] = (byte)'E';
                Handshake_Packet[7] = (byte)'Y';
            }
            if (cb_MeterType.SelectedIndex == 5) // AK311 L451
            {
                Wait_Packet[4] = (byte)'b';
                Wait_Packet[5] = (byte)'o';
                Wait_Packet[6] = (byte)'o';
                Wait_Packet[7] = (byte)'t';
                Handshake_Packet[4] = (byte)'o';
                Handshake_Packet[5] = (byte)'k';
                Handshake_Packet[6] = (byte)'e';
                Handshake_Packet[7] = (byte)'y';
            }

            while (FinishTotal!=WaitTotal || checkBox1.Checked == true)
            {
                sx1231.WriteRegisters();// reset registers
                Thread.Sleep(10);

                if (Radio_CatchAndSend_Packet(Wait_Packet, Handshake_Packet, 5000))
                {
                    Console.Text += Timestamp_String() + "Meter found and sending.\r\n";
                    if (Radio_Transfer_Packet(8000))
                    {
                        Console.Text += Timestamp_String() + "Transfer successfully finished.\r\n";
                        tb_FinishList.Text += Convert.ToString(Meter_No_Buffer[3] >> 4, 16);
                        tb_FinishList.Text += Convert.ToString(Meter_No_Buffer[3] & 0xF, 16);
                        tb_FinishList.Text += Convert.ToString(Meter_No_Buffer[2] >> 4, 16);
                        tb_FinishList.Text += Convert.ToString(Meter_No_Buffer[2] & 0xF, 16);
                        tb_FinishList.Text += Convert.ToString(Meter_No_Buffer[1] >> 4, 16);
                        tb_FinishList.Text += Convert.ToString(Meter_No_Buffer[1] & 0xF, 16);
                        tb_FinishList.Text += Convert.ToString(Meter_No_Buffer[0] >> 4, 16);
                        tb_FinishList.Text += Convert.ToString(Meter_No_Buffer[0] & 0xF, 16) + "\r\n";
                        FinishTotal++;
                        lb_FinishTotal.Text = "Total: " + FinishTotal.ToString();
                        if (checkBox1.Checked == false)
                        {
                            MeterList[Procces_MeterIndex] = 0;
                        }
                    }
                    else
                    {
                        Console.Text += Timestamp_String() + "Transfer Timeout.\r\n";
                    }
                }
                else
                {
                    Console.Text += Timestamp_String() + "Meter search timeout.\r\n";
                }
                if (b_Stop.Enabled == false)
                {
                    return;
                }
            }
            Console.Text += Timestamp_String() + "Finish updates.\r\n"; 
            b_Stop_Click(null, null);
        }
        /*********************************************************/
        private void b_Stop_Click(object sender, EventArgs e)
        {
            b_Stop.Enabled = false;
            b_LoadFile.Enabled = true;
            b_Start.Enabled = true;
            gr_Config.Enabled = true;
            pr_transmid.Value = 0;
            pr_Load.Value = 0;
            Console.Text += Timestamp_String() + "Stop!\n";
        }
        /*********************************************************/
        private void tb_MeterNoStart_TextChanged(object sender, EventArgs e)
        {
            byte[] MeterNoTemp = new byte[4];        

            MeterNoTemp[3] = Convert.ToByte(tb_MeterNoStart.Text.Substring(0, 1));
            MeterNoTemp[3] = (byte)(MeterNoTemp[3] << 4);
            MeterNoTemp[3] += Convert.ToByte(tb_MeterNoStart.Text.Substring(1, 1));
            MeterNoTemp[2] = Convert.ToByte(tb_MeterNoStart.Text.Substring(2, 1));
            MeterNoTemp[2] = (byte)(MeterNoTemp[2] << 4);
            MeterNoTemp[2] += Convert.ToByte(tb_MeterNoStart.Text.Substring(3, 1));
            MeterNoTemp[1] = Convert.ToByte(tb_MeterNoStart.Text.Substring(4, 1));
            MeterNoTemp[1] = (byte)(MeterNoTemp[1] << 4);
            MeterNoTemp[1] += Convert.ToByte(tb_MeterNoStart.Text.Substring(5, 1));
            MeterNoTemp[0] = Convert.ToByte(tb_MeterNoStart.Text.Substring(6, 1));
            MeterNoTemp[0] = (byte)(MeterNoTemp[0] << 4);
            MeterNoTemp[0] += Convert.ToByte(tb_MeterNoStart.Text.Substring(7, 1));

            Meter_No_Buffer[0]=MeterNoTemp[0];
            Meter_No_Buffer[1] = MeterNoTemp[1];
            Meter_No_Buffer[2] = MeterNoTemp[2];
            Meter_No_Buffer[3] = MeterNoTemp[3];

            MeterNoStartInt = 0;
            MeterNoStartInt += (MeterNoTemp[0] & 0xF);
            MeterNoStartInt += (MeterNoTemp[0] >> 4) * 10;
            MeterNoStartInt += (MeterNoTemp[1] & 0xF) * 100;
            MeterNoStartInt += (MeterNoTemp[1] >> 4) * 1000;
            MeterNoStartInt += (MeterNoTemp[2] & 0xF) * 10000;
            MeterNoStartInt += (MeterNoTemp[2] >> 4) * 100000;
            MeterNoStartInt += (MeterNoTemp[3] & 0xF) * 1000000;
            MeterNoStartInt += (MeterNoTemp[3] >> 4) * 10000000;

            tb_MeterNoEnd.Text = tb_MeterNoStart.Text;
        }
        /*********************************************************/
        private void tb_MeterNoEnd_TextChanged(object sender, EventArgs e)
        {
            byte[] MeterNoTemp = new byte[4];            

            MeterNoTemp[3] = Convert.ToByte(tb_MeterNoEnd.Text.Substring(0, 1));
            MeterNoTemp[3] = (byte)(MeterNoTemp[3] << 4);
            MeterNoTemp[3] += Convert.ToByte(tb_MeterNoEnd.Text.Substring(1, 1));
            MeterNoTemp[2] = Convert.ToByte(tb_MeterNoEnd.Text.Substring(2, 1));
            MeterNoTemp[2] = (byte)(MeterNoTemp[2] << 4);
            MeterNoTemp[2] += Convert.ToByte(tb_MeterNoEnd.Text.Substring(3, 1));
            MeterNoTemp[1] = Convert.ToByte(tb_MeterNoEnd.Text.Substring(4, 1));
            MeterNoTemp[1] = (byte)(MeterNoTemp[1] << 4);
            MeterNoTemp[1] += Convert.ToByte(tb_MeterNoEnd.Text.Substring(5, 1));
            MeterNoTemp[0] = Convert.ToByte(tb_MeterNoEnd.Text.Substring(6, 1));
            MeterNoTemp[0] = (byte)(MeterNoTemp[0] << 4);
            MeterNoTemp[0] += Convert.ToByte(tb_MeterNoEnd.Text.Substring(7, 1));

            MeterNoEndInt = 0;
            MeterNoEndInt += (MeterNoTemp[0] & 0xF);
            MeterNoEndInt += (MeterNoTemp[0] >> 4) * 10;
            MeterNoEndInt += (MeterNoTemp[1] & 0xF) * 100;
            MeterNoEndInt += (MeterNoTemp[1] >> 4) * 1000;
            MeterNoEndInt += (MeterNoTemp[2] & 0xF) * 10000;
            MeterNoEndInt += (MeterNoTemp[2] >> 4) * 100000;
            MeterNoEndInt += (MeterNoTemp[3] & 0xF) * 1000000;
            MeterNoEndInt += (MeterNoTemp[3] >> 4) * 10000000;

            tb_FinishList.Clear();
            FinishTotal = 0;
            tb_WaitList.Clear();
            MeterListSize = 0;
            WaitTotal = 0;
            if (MeterNoStartInt > MeterNoEndInt)
            {
                int temp = MeterNoEndInt;
                for (int i=0;((i<1000) && (MeterNoStartInt >= temp)) ;i++)
                {
                    MeterList[i] = temp;
                    MeterListSize++;
                    temp++;                   
                }
                
            }
            else if (MeterNoStartInt < MeterNoEndInt)
            {
                int temp = MeterNoStartInt;
                for (int i = 0; ((i < 1000) && (MeterNoEndInt >= temp)); i++)
                {
                    MeterList[i] = temp;
                    MeterListSize++;
                    temp++;
                }
            }
            else
            {
                MeterList[0] = MeterNoStartInt;
                MeterListSize++;
            }            
            for (int i = 0; i < MeterListSize; i++)
            {
                if (MeterList[i] != 0)
                {
                    tb_WaitList.Text += MeterList[i].ToString("D8") + "\r\n";
                    WaitTotal++;
                }
            }
            lb_WaitTotal.Text = "Total: " + WaitTotal.ToString();            
        }
        /*********************************************************/
        static bool ByteArrayCompare(byte[] b1, byte[] b2)
        {
            return b1.Length == b2.Length && memcmp(b1, b2, b1.Length) == 0;
        }
        /*********************************************************/
        private void Radio_Transmit_Done(object sender, EventArgs e)
        {
            Tx_Packet_Done_Flag = true;
        }
        /*********************************************************/
        private void Radio_Receive_Done(object sender, EventArgs e)
        {
            Rx_Packet_Done_Flag = true;                
        }
        /*********************************************************/
        private bool Radio_Send_Packet(byte[] buffer, int retry, int timeout_ms)
        {
            bool status = true;
            sx1231.SetMessage(buffer);
            sx1231.SetMessageLength(buffer.Length);

            sx1231.SetMaxPacketNumber(retry);

            sx1231.SetOperatingMode(OperatingModeEnum.Tx);
            sx1231.Mode = OperatingModeEnum.Tx;

            sx1231.SetPacketHandlerStartStop(true);

            DateTime desired_time = DateTime.Now.AddMilliseconds(timeout_ms);

            Tx_Packet_Done_Flag = false;

            while (sx1231.isPacketModeRunning == true)
            {
                if (DateTime.Now > desired_time)
                {
                    status = false;// timeout
                    break;
                }
                Thread.Sleep(1);
                System.Windows.Forms.Application.DoEvents();
            }
            sx1231.SetPacketHandlerStartStop(false);
            sx1231.SetOperatingMode(OperatingModeEnum.Stdby);
            sx1231.Mode = OperatingModeEnum.Stdby;
            return status;
        }
        /*********************************************************/
        private bool Radio_Get_Packet(int length, int timeout_ms)
        {                       
            byte[] buff_rx = new byte[length];
            bool status = true;
            sx1231.SetMessage(buff_rx);
            sx1231.SetMessageLength(length);

            sx1231.SetMaxPacketNumber(1);

            sx1231.SetOperatingMode(OperatingModeEnum.Rx);
            sx1231.Mode = OperatingModeEnum.Rx;

            sx1231.SetPacketHandlerStartStop(true);
            
            Rx_Packet_Done_Flag = false;
            DateTime desired_time = DateTime.Now.AddMilliseconds(timeout_ms);

            while (Rx_Packet_Done_Flag == false)
            {
                if (DateTime.Now > desired_time)
                {
                    status = false;// timeout
                    break;
                }
                Thread.Sleep(1);
                System.Windows.Forms.Application.DoEvents();
            }
            sx1231.SetPacketHandlerStartStop(false);
            sx1231.SetOperatingMode(OperatingModeEnum.Stdby);
            sx1231.Mode = OperatingModeEnum.Stdby;
            return status;            
        }
        /*********************************************************/
        private bool Radio_CatchAndSend_Packet(byte[] compare_buffer, byte[] send_buffer, int timeout_ms)
        {
            byte[] buff_rx = new byte[compare_buffer.Length];
            bool status = true;
            int meterno_temp;
            int i;
            SemtechLib.Devices.SX1231.General.Packet MyPacket = sx1231.Packet;

            sx1231.SetMessage(buff_rx);
            sx1231.SetMessageLength(buff_rx.Length);

            sx1231.SetMaxPacketNumber(1);

            sx1231.SetOperatingMode(OperatingModeEnum.Rx);
            sx1231.Mode = OperatingModeEnum.Rx;

            sx1231.SetPacketHandlerStartStop(true);

            Rx_Packet_Done_Flag = false;

            DateTime desired_time = DateTime.Now.AddMilliseconds(timeout_ms);

            while (true)
            {
                if (Rx_Packet_Done_Flag  == true)
                {
                    meterno_temp = Convert_ToDec(MyPacket.Message);
                    for (i = 0; i < MeterListSize; i++)
                    {
                        if (MeterList[i] == meterno_temp)
                        {
                            Procces_MeterIndex = i;
                            break;
                        }
                    }
                    if (i != MeterListSize && MyPacket.Message[4] == compare_buffer[4] && MyPacket.Message[5] == compare_buffer[5] &&
                        MyPacket.Message[6]== compare_buffer[6] && MyPacket.Message[7]== compare_buffer[7])
                    {
                        sx1231.SetPayloadLength(8);
                        send_buffer[0] = MyPacket.Message[0];
                        send_buffer[1] = MyPacket.Message[1];
                        send_buffer[2] = MyPacket.Message[2];
                        send_buffer[3] = MyPacket.Message[3];
                        sx1231.TransmitRfData(send_buffer);
                        Thread.Sleep(5);
                        sx1231.TransmitRfData(send_buffer);
                        Thread.Sleep(5);
                        sx1231.TransmitRfData(send_buffer);
                        Thread.Sleep(5);
                        sx1231.TransmitRfData(send_buffer);
                        Thread.Sleep(5);
                        sx1231.TransmitRfData(send_buffer);
                        Thread.Sleep(5);
                        sx1231.TransmitRfData(send_buffer);
                        Thread.Sleep(5);
                        break;
                    }
                    Rx_Packet_Done_Flag = false;
                }
                if (DateTime.Now > desired_time)
                {
                    status = false;// timeout
                    break;
                }
                Thread.Sleep(1);
                System.Windows.Forms.Application.DoEvents();
                if (b_Stop.Enabled == false)
                {
                    status = false;// timeout
                    break;
                }
            }
            sx1231.SetPacketHandlerStartStop(false);
            sx1231.SetOperatingMode(OperatingModeEnum.Stdby);
            sx1231.Mode = OperatingModeEnum.Stdby;
            return status;
        }
        /*********************************************************/
        private bool Radio_Transfer_Packet(int timeout_ms)
        {
            bool status = false;
            byte[] compare_buffer_same = new byte[8];
            byte[] compare_buffer_next = new byte[8];
            byte[] compare_buffer_finish = new byte[6];
            byte[] buff_rx = new byte[8];
            byte[] send_buffer = new byte[64];
            int transmid=0;
            Convert_ToBCDByteArray(MeterList[Procces_MeterIndex], ref Meter_No_Buffer);

            if (cb_MeterType.SelectedIndex < 4) // TK_AMR_LORA TK1C L451 L151
            {
                compare_buffer_next[0] = Meter_No_Buffer[3];
                compare_buffer_next[1] = Meter_No_Buffer[2];
                compare_buffer_next[2] = Meter_No_Buffer[1];
                compare_buffer_next[3] = Meter_No_Buffer[0];

                compare_buffer_same[0] = Meter_No_Buffer[3];
                compare_buffer_same[1] = Meter_No_Buffer[2];
                compare_buffer_same[2] = Meter_No_Buffer[1];
                compare_buffer_same[3] = Meter_No_Buffer[0];
            }
            else
            {
                compare_buffer_next[0] = Meter_No_Buffer[0];
                compare_buffer_next[1] = Meter_No_Buffer[1];
                compare_buffer_next[2] = Meter_No_Buffer[2];
                compare_buffer_next[3] = Meter_No_Buffer[3];

                compare_buffer_same[0] = Meter_No_Buffer[0];
                compare_buffer_same[1] = Meter_No_Buffer[1];
                compare_buffer_same[2] = Meter_No_Buffer[2];
                compare_buffer_same[3] = Meter_No_Buffer[3];
            }
                compare_buffer_next[4] = 0x00;
                compare_buffer_next[5] = 0x00;
                compare_buffer_next[6] = 0xAA;
                compare_buffer_next[7] = 0xAA;

                compare_buffer_finish[0] = Meter_No_Buffer[0];
                compare_buffer_finish[1] = Meter_No_Buffer[1];
                compare_buffer_finish[2] = Meter_No_Buffer[2];
                compare_buffer_finish[3] = Meter_No_Buffer[3];
                compare_buffer_finish[4] = 0xFF;
                compare_buffer_finish[5] = 0xFF;

                PacketIndex = 0;
                SemtechLib.Devices.SX1231.General.Packet MyPacket = sx1231.Packet;

                sx1231.SetMessage(buff_rx);
                sx1231.SetMessageLength(buff_rx.Length);

                sx1231.SetMaxPacketNumber(1);

                sx1231.SetOperatingMode(OperatingModeEnum.Rx);
                sx1231.Mode = OperatingModeEnum.Rx;

                sx1231.SetPacketHandlerStartStop(true);

                Rx_Packet_Done_Flag = false;

                DateTime desired_time = DateTime.Now.AddMilliseconds(timeout_ms);

                while (true)
                {
                    if (Rx_Packet_Done_Flag == true)
                    {
                        if (memcmp(MyPacket.Message, compare_buffer_same, 8) == 0)
                        {
                            desired_time = DateTime.Now.AddMilliseconds(timeout_ms);
                            sx1231.SetPayloadLength(64);
                            sx1231.TransmitRfData(send_buffer);
                            Thread.Sleep(12);
                            Console.Text += Timestamp_String() + "Lost Packet:" + PacketIndex.ToString() + "\r\n";
                        }
                        else if (memcmp(MyPacket.Message, compare_buffer_next, 8) == 0)
                        {
                            desired_time = DateTime.Now.AddMilliseconds(timeout_ms);

                            FileIndex = PacketIndex * 58;

                            if (FileSize + 58 < FileIndex) // son paket isteği
                            {
                                send_buffer[0] = Meter_No_Buffer[0];
                                send_buffer[1] = Meter_No_Buffer[1];
                                send_buffer[2] = Meter_No_Buffer[2];
                                send_buffer[3] = Meter_No_Buffer[3];
                                send_buffer[4] = 0xFF;
                                send_buffer[5] = 0xFF;
                                send_buffer[6] = Crc_Buff[3];
                                send_buffer[7] = Crc_Buff[2];
                                send_buffer[8] = Crc_Buff[1];
                                send_buffer[9] = Crc_Buff[0];
                                send_buffer[10] = (byte)(FileSize >> 24);
                                send_buffer[11] = (byte)(FileSize >> 16);
                                send_buffer[12] = (byte)(FileSize >> 8);
                                send_buffer[13] = (byte)FileSize;
                                sx1231.SetPayloadLength(64);
                                sx1231.TransmitRfData(send_buffer);
                                Thread.Sleep(12);
                            }
                            else
                            {
                                pr_Load.Value = (int)(FileIndex / 100);
                                int per = (int)(((float)FileIndex / (float)FileSize) * 100);
                                lb_percent.Text = "% " + per.ToString();

                                pr_transmid.Value = transmid;
                                transmid++;
                                transmid %= 20;

                                compare_buffer_same[4] = (byte)(PacketIndex >> 8);
                                compare_buffer_same[5] = (byte)(PacketIndex & 0xFF);
                                compare_buffer_same[6] = 0xAA;
                                compare_buffer_same[7] = 0xAA;

                                send_buffer[0] = Meter_No_Buffer[0];
                                send_buffer[1] = Meter_No_Buffer[1];
                                send_buffer[2] = Meter_No_Buffer[2];
                                send_buffer[3] = Meter_No_Buffer[3];
                                send_buffer[4] = (byte)(PacketIndex >> 8);
                                send_buffer[5] = (byte)(PacketIndex & 0xFF);

                                Buffer.BlockCopy(BinFile, (int)FileIndex, send_buffer, 6, 58);
                                FileIndex += 58;

                                sx1231.SetPayloadLength(64);
                                sx1231.TransmitRfData(send_buffer);
                                Thread.Sleep(12);

                                PacketIndex++;

                                compare_buffer_next[4] = (byte)(PacketIndex >> 8);
                                compare_buffer_next[5] = (byte)(PacketIndex & 0xFF);
                                compare_buffer_next[6] = 0xAA;
                                compare_buffer_next[7] = 0xAA;
                            }
                        }
                        else if (memcmp(MyPacket.Message, compare_buffer_finish, 6) == 0)
                        {
                            desired_time = DateTime.Now.AddMilliseconds(timeout_ms);
                            status = true;
                            break;
                        }
                        Rx_Packet_Done_Flag = false;
                        sx1231.SetPayloadLength(8);
                        sx1231.SetOperatingMode(OperatingModeEnum.Rx, true);
                    }
                    if (DateTime.Now > desired_time)
                    {
                        break;
                    }
                    Thread.Sleep(0);
                    System.Windows.Forms.Application.DoEvents();
                    if (b_Stop.Enabled == false)
                    {
                        status = false;// timeout
                        break;
                    }
                }
                sx1231.SetPacketHandlerStartStop(false);
                sx1231.SetOperatingMode(OperatingModeEnum.Stdby);
                sx1231.Mode = OperatingModeEnum.Stdby;
                return status;
        }
        /*********************************************************/
        /*********************************************************/
        /*********************************************************/


        /*********************************************************/
        /*********************************************************/
        /*********************************************************/
        /*********************************************************/
        /*********************************************************/
        /*********************************************************/
        /*********************************************************/
        /*********************************************************/







        /*********************************************************/
        /*********************************************************/
        /*********************************************************/

	}
}
