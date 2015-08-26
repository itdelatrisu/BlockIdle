using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace itdelatrisu.BlockIdle {
	/// <summary>
	/// Main class.
	/// </summary>
	static class Program {
		[STAThread]
		static void Main() {
			if (Environment.OSVersion.Version.Major >= 6)
				SetProcessDPIAware();

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			new SingleInstanceController().Run(Environment.GetCommandLineArgs());
		}

		[DllImport("user32.dll")]
		private static extern bool SetProcessDPIAware();
	}

	/// <summary>
	/// Restricts the application to a single instance.
	/// </summary>
	public class SingleInstanceController : WindowsFormsApplicationBase {
		public SingleInstanceController() { IsSingleInstance = true; }

		protected override bool OnStartup(StartupEventArgs e) {
			Application.Run(new AppContext());
			return false;
		}
	}
}
