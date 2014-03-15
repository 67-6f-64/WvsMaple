using Common;
using Common.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using WvsCenter.GUI;
using WvsCenter.Maple;
using WvsCenter.Net;

namespace WvsCenter
{
    public partial class MainForm : Form
    {
        public static MainForm Instance { get; private set; }

        public MainForm()
        {
            Instance = this;

            InitializeComponent();

            DoubleBuffered = true;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            InitializeUserInterface();
        }

        private void InitializeUserInterface()
        {
            AeroRenderer renderer = new AeroRenderer(ToolbarTheme.CommunicationsToolbar, true);

            this.menuMain.Renderer = renderer;

            Thread mainThread = new Thread(new ThreadStart(this.Initialize));

            mainThread.IsBackground = true;
            mainThread.Start();
        }

        private void Initialize()
        {
            this.Log("");
            this.Log("\t- WvsCenter v{0}.{1} -", Constants.MapleVersion, Constants.PatchLocation);
            this.Log("");

            try
            {
                this.Loga("Initializing Center Server... ");
                CenterServer.Initialize();
                this.Log("Done.");

                this.Loga("Initializing Ranking Calculator... ");
                this.Log("Done.");

                new Delay(15 * 1000, () =>
                {
                    GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized);
                }).Execute();

                this.Log("");

                this.Invoke(new MethodInvoker(() =>
                {
                    this.Text += string.Format(" ({0})", Program.ConfigurationFile);
                    this.RefreshServerList();
                }));

            }
            catch (Exception e)
            {
                this.Log("Unable to initialize.");
                this.Log(e.ToString());
            }
        }

        public void RefreshServerList()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => RefreshServerList()));
                return;
            }

            int totalConnections = 0;

            foreach (KeyValuePair<string, Server> server in CenterServer.Servers)
            {
                ListViewItem item;

                totalConnections += server.Value.Connections;

                if (this.listView1.Items.ContainsKey(server.Key))
                {
                    item = this.listView1.Items[server.Key];

                    item.SubItems[2].Text = server.Value.Connections.ToString();
                    item.ImageIndex = server.Value.IsConnected ? 1 : 0;

                    return;
                }

                item = new ListViewItem(new string[] {
                    server.Value.Name,
                    server.Value.PublicIP.ToString() + ":" + server.Value.Port.ToString(),
                    server.Value.Connections.ToString()
                });

                item.Name = server.Key;
                item.ImageIndex = 0;

                this.AppendServer(item);
            }

            // TODO: Append total connections to the textbox.
        }

        private void AppendServer(ListViewItem lvi)
        {
            this.listView1.BeginInvoke((MethodInvoker)delegate
            {
                this.listView1.Items.Add(lvi);
            });
        }

        public void Log(string text, params object[] args)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => Log(text, args)));
            }
            else
            {
                string message = string.Concat(string.Format(text, args),
                                    Environment.NewLine);

                this.textBoxLog.AppendText(message);
            }
        }

        public void Loga(string text, params object[] args)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => Loga(text, args)));
            }
            else
            {
                string message = string.Concat(string.Format(text, args));

                this.textBoxLog.AppendText(message);
            }
        }
    }
}
