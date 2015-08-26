using Microsoft.Win32;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace itdelatrisu.BlockIdle {
	/// <summary>
	/// The application context.
	/// </summary>
	public class AppContext : ApplicationContext {
		private NotifyIcon notifyIcon;
		private ToolStripMenuItem
			actionMenuItem, exitMenuItem,
			disableOnBatteryMenuItem, enableOnPowerMenuItem, startupMenuItem;
		private Blocker blocker;
		private RegistryKey rk;
		private const int BALLOON_TIP_PERIOD = 2000;

		/// <summary>
		/// Builds the application context.
		/// </summary>
		public AppContext() {
			Application.ApplicationExit += new EventHandler(this.OnExit);
			this.blocker = new Blocker();
			InitializeComponent();
			notifyIcon.Visible = true;
			checkPowerLineStatus();
		}

		/// <summary>
		/// Initializes the component.
		/// </summary>
		private void InitializeComponent() {
			// create NotifyIcon instance
			this.notifyIcon = new NotifyIcon() {
				Icon = SystemIcons.Application,
				ContextMenuStrip = new ContextMenuStrip(),
				Text = "Idle blocking (OFF)"
			};

			// registry key
			string rkName = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
			this.rk = Registry.CurrentUser.OpenSubKey(rkName, true);

			// context menu items
			this.actionMenuItem = new ToolStripMenuItem("Enable");
			actionMenuItem.Click += new EventHandler(this.ActionMenuItem_Click);
			actionMenuItem.Font = new Font(actionMenuItem.Font, actionMenuItem.Font.Style | FontStyle.Bold);
			notifyIcon.ContextMenuStrip.Items.Add(actionMenuItem);
			notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
			this.disableOnBatteryMenuItem = new ToolStripMenuItem("Disable on battery");
			disableOnBatteryMenuItem.Click += new EventHandler(this.DisableOnBatteryMenuItem_Click);
			disableOnBatteryMenuItem.Checked = Properties.Settings.Default.DisableOnBattery;
			notifyIcon.ContextMenuStrip.Items.Add(disableOnBatteryMenuItem);
			this.enableOnPowerMenuItem = new ToolStripMenuItem("Enable on power");
			enableOnPowerMenuItem.Click += new EventHandler(this.EnableOnPowerMenuItem_Click);
			enableOnPowerMenuItem.Checked = Properties.Settings.Default.EnableOnPower;
			notifyIcon.ContextMenuStrip.Items.Add(enableOnPowerMenuItem);
			this.startupMenuItem = new ToolStripMenuItem("Launch on startup");
			startupMenuItem.Click += new EventHandler(this.StartupMenuItem_Click);
			startupMenuItem.Checked = rk.GetValue(Application.ProductName) != null;
			notifyIcon.ContextMenuStrip.Items.Add(startupMenuItem);
			notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
			this.exitMenuItem = new ToolStripMenuItem("Exit");
			exitMenuItem.Click += new EventHandler(ExitMenuItem_Click);
			notifyIcon.ContextMenuStrip.Items.Add(this.exitMenuItem);

			// event handlers
			notifyIcon.DoubleClick += NotifyIcon_DoubleClick;
			SystemEvents.PowerModeChanged += new PowerModeChangedEventHandler(this.SystemEvents_PowerModeChanged);
		}

		/// <summary>
		/// Notification that the application is about to shut down.
		/// </summary>
		private void OnExit(object sender, EventArgs e) {
			notifyIcon.Visible = false;
			SystemEvents.PowerModeChanged -= new PowerModeChangedEventHandler(this.SystemEvents_PowerModeChanged);
		}

		/// <summary>
		/// Notification that the power mode changed.
		/// </summary>
		private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e) {
			if (e.Mode == PowerModes.StatusChange)
				checkPowerLineStatus();
		}

		/// <summary>
		/// Checks the power line status, and turns the idle blocker on or off if necessary.
		/// </summary>
		private void checkPowerLineStatus() {
			PowerLineStatus powerLineStatus = SystemInformation.PowerStatus.PowerLineStatus;
			if (powerLineStatus == PowerLineStatus.Online) {  // on power
				if (!blocker.Active && Properties.Settings.Default.EnableOnPower)
					SetProgramState(true);
			} else if (powerLineStatus == PowerLineStatus.Offline) {  // on battery
				if (blocker.Active && Properties.Settings.Default.DisableOnBattery)
					SetProgramState(false);
			}
		}

		/// <summary>
		/// Notification that the tray icon was double-clicked.
		/// </summary>
		private void NotifyIcon_DoubleClick(object sender, EventArgs e) {
			SetProgramState(!blocker.Active);
		}

		/// <summary>
		/// Notification that the action item was clicked.
		/// </summary>
		private void ActionMenuItem_Click(object sender, EventArgs e) {
			SetProgramState(!blocker.Active);
		}

		/// <summary>
		/// Notification that the "disable on battery" item was clicked.
		/// </summary>
		private void DisableOnBatteryMenuItem_Click(object sender, EventArgs e) {
			bool newVal = !disableOnBatteryMenuItem.Checked;
			Properties.Settings.Default.DisableOnBattery = newVal;
			Properties.Settings.Default.Save();
			disableOnBatteryMenuItem.Checked = newVal;
		}

		/// <summary>
		/// Notification that the "enable on power" item was clicked.
		/// </summary>
		private void EnableOnPowerMenuItem_Click(object sender, EventArgs e) {
			bool newVal = !enableOnPowerMenuItem.Checked;
			Properties.Settings.Default.EnableOnPower = newVal;
			Properties.Settings.Default.Save();
			enableOnPowerMenuItem.Checked = newVal;
		}

		/// <summary>
		/// Notification that the "launch on startup" item was clicked.
		/// </summary>
		private void StartupMenuItem_Click(object sender, EventArgs e) {
			bool newVal = !startupMenuItem.Checked;
			if (newVal)
				rk.SetValue(Application.ProductName, Application.ExecutablePath.ToString());
			else
				rk.DeleteValue(Application.ProductName, false);
			startupMenuItem.Checked = newVal;
		}

		/// <summary>
		/// Notification that the exit item was clicked.
		/// </summary>
		private void ExitMenuItem_Click(object sender, EventArgs e) {
			Application.Exit();
		}

		/// <summary>
		/// Turns idle blocking on or off.
		/// </summary>
		/// <param name="active">Whether to enable or disable idle blocking.</param>
		private void SetProgramState(bool active) {
			if (active == blocker.Active)
				return;

			if (active) {
				blocker.Active = true;
				notifyIcon.Text = "Idle blocking (ON)";
				actionMenuItem.Text = "Disable";
				notifyIcon.ShowBalloonTip(
					BALLOON_TIP_PERIOD,
					"Idle blocking enabled",
					"Your computer will no longer become idle.",
					ToolTipIcon.Info
				);
			} else {
				blocker.Active = false;
				notifyIcon.Text = "Idle blocking (OFF)";
				actionMenuItem.Text = "Enable";
				notifyIcon.ShowBalloonTip(
					BALLOON_TIP_PERIOD,
					"Idle blocking disabled",
					"Your computer will become idle as usual.",
					ToolTipIcon.Info
				);
			}
		}
	}
}
