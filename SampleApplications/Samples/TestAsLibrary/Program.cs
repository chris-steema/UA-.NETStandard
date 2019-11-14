using Opc.Ua;
using Opc.Ua.Bindings.Custom;
using Opc.Ua.Client.Controls;
using Opc.Ua.Configuration;
using Opc.Ua.Sample;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestAsLibrary
{
	static class Program
	{
		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			//Application.SetHighDpiMode(HighDpiMode.SystemAware);
			//Application.EnableVisualStyles();
			//Application.SetCompatibleTextRenderingDefault(false);
			//Application.Run(new Form1());

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			ApplicationInstance.MessageDlg = new ApplicationMessageDlg();
			ApplicationInstance application = new ApplicationInstance();
			application.ApplicationName = "UA Sample Client";
			application.ApplicationType = ApplicationType.ClientAndServer;
			application.ConfigSectionName = "Opc.Ua.SampleClient";

			// use a custom transport channel
			WcfChannelBase.g_CustomTransportChannel = new CustomTransportChannelFactory();

			try
			{
				application.LoadApplicationConfiguration(false).Wait();

				// check the application certificate.
				bool certOK = application.CheckApplicationInstanceCertificate(false, 0).Result;
				if (!certOK)
				{
					throw new Exception("Application instance certificate invalid!");
				}

				// start the server.
				application.Start(new SampleServer()).Wait();

				// run the application interactively.
				Application.Run(new SampleClientForm(application, null, application.ApplicationConfiguration));
			}
			catch (Exception e)
			{
				ExceptionDlg.Show(application.ApplicationName, e);
			}
		}
	}
}
