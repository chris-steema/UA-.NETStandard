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

			void AddButton2()
			{
				var button = new Button
				{
					Top = Top + 30,
					Width = 150,
					Text = "Subscribe to custom"
				};
				button.Click += button2_Click;

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

			AddButton2();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			var sampleForm = new SampleClientForm(_application, null, _application.ApplicationConfiguration);
			sampleForm.Show();
		}

		private async void button2_Click(object sender, EventArgs e)
		{

			void StandardClient_KeepAlive(Session sender, KeepAliveEventArgs e)
			{

			}

			var endPointDescription = new EndpointDescription("opc.tcp://opcua.demo-this.com:51211")
			{
				 EndpointUrl = "opc.tcp://opcua.demo-this.com:51211/UA/SampleServer",
				 SecurityMode = MessageSecurityMode.None,
				 //SecurityPolicyUri = "http://opcfoundation.org/UA/SecurityPolicy#Basic256Sha256",
				 TransportProfileUri = "http://opcfoundation.org/UA/profiles/transport/wsxmlorbinary",
			};

			endPointDescription.UserIdentityTokens.Add(new UserTokenPolicy { TokenType = UserTokenType.Anonymous });
			endPointDescription.UserIdentityTokens.Add(new UserTokenPolicy { TokenType = UserTokenType.UserName });
			endPointDescription.UserIdentityTokens.Add(new UserTokenPolicy { TokenType = UserTokenType.Certificate });
			endPointDescription.UserIdentityTokens.Add(new UserTokenPolicy { TokenType = UserTokenType.IssuedToken, IssuedTokenType = "urn:oasis:names:tc:SAML:1.0:assertion:Assertion" });

			var endPointCollection = new ConfiguredEndpointCollection(_application.ApplicationConfiguration);
			endPointCollection.DiscoveryUrls = _application.ApplicationConfiguration.ClientConfiguration.WellKnownDiscoveryUrls;

			var endPoint = new ConfiguredEndpoint(endPointCollection, endPointDescription, EndpointConfiguration.Create(_application.ApplicationConfiguration));
			endPoint.UpdateBeforeConnect = true;
			endPoint.SelectedUserTokenPolicyIndex = 0;

			var sessionTree = new SessionTreeCtrl
			{
				Configuration = _application.ApplicationConfiguration,
				MessageContext = _application.ApplicationConfiguration.CreateMessageContext()
			};

			var browseTree = new Opc.Ua.Sample.Controls.BrowseTreeCtrl();
			Session session = await sessionTree.Connect(endPoint);

			if (session != null)
			{
				//// stop any reconnect operation.
				//if (m_reconnectHandler != null)
				//{
				//	m_reconnectHandler.Dispose();
				//	m_reconnectHandler = null;
				//}

				session.KeepAlive += new KeepAliveEventHandler(StandardClient_KeepAlive);
				browseTree.SetView(session, BrowseViewType.Objects, null);
				StandardClient_KeepAlive(session, null);
			}

		}
	}
}
