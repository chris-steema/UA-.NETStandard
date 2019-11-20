using System;
using Opc.Ua;
using Opc.Ua.Bindings.Custom;
using Opc.Ua.Client.Controls;
using Opc.Ua.Configuration;
using Opc.Ua.Sample;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Opc.Ua.Client;
using Opc.Ua.Sample.Controls;

namespace TestAsLibrary
{
	public partial class Form1 : Form
	{

		ApplicationInstance _application;
		public Form1()
		{
			void AddButton1()
			{
				var button = new Button
				{
					Width = 100,
					Text = "Editor ..."
				};
				button.Click += button1_Click;

				Controls.Add(button);
			}

			void InitializeServer()
			{
				ApplicationInstance.MessageDlg = new ApplicationMessageDlg();
				_application = new ApplicationInstance
				{
					ApplicationName = "UA Sample Client",
					ApplicationType = ApplicationType.ClientAndServer,
					ConfigSectionName = "Opc.Ua.SampleClient"
				};

				// use a custom transport channel
				WcfChannelBase.g_CustomTransportChannel = new CustomTransportChannelFactory();

				try
				{
					_application.LoadApplicationConfiguration(false).Wait();

					// check the application certificate.
					bool certOK = _application.CheckApplicationInstanceCertificate(false, 0).Result;
					if (!certOK)
					{
						throw new Exception("Application instance certificate invalid!");
					}

					// start the server.
					_application.Start(new SampleServer()).Wait();
				}
				catch (Exception ex)
				{
					ExceptionDlg.Show(_application.ApplicationName, ex);
				}
			}

			InitializeServer();

			InitializeComponent();

			AddButton1();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			var sampleForm = new SampleClientForm(_application, null, _application.ApplicationConfiguration);
			if(sampleForm.ShowDialog() == DialogResult.OK)
			{
				bool stop = true;
			}
		}

	}
}
