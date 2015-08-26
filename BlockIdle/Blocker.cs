using System;
using System.Windows.Forms;

namespace itdelatrisu.BlockIdle {
	/// <summary>
	/// Idle blocker.
	/// </summary>
	public class Blocker {
		private Timer timer;
		private bool active = false;
		private const int TIMER_INTERVAL = 5000;

		/// <summary>
		/// Initializes the idle blocker.
		/// </summary>
		public Blocker() {
			this.timer = new Timer();
			timer.Interval = TIMER_INTERVAL;
			timer.Tick += new EventHandler(this.Timer_Tick);
		}

		/// <summary>
		/// Notification that a timer tick occurred.
		/// </summary>
		private void Timer_Tick(object sender, EventArgs e) {
			MouseSimulator.MoveMouse(1, 1);
			MouseSimulator.MoveMouse(-1, -1);
		}

		/// <summary>
		/// Whether the idle blocker is on or off.
		/// </summary>
		public bool Active {
			get { return active; }
			set {
				if (active != value) {
					if (value)
						timer.Start();
					else
						timer.Stop();
					active = value;
				}
			}
		}
	}
}
