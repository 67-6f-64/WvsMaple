using Common;
using Common.Net;
using Common.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using WvsGame.GUI;
using WvsGame.Maple.Data;
using WvsGame.Net;

namespace WvsGame
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
            this.Log("\t- WvsGame v{0}.{1} -", Constants.MapleVersion, Constants.PatchLocation);
            this.Log("");

            try
            {
                this.Loga("Initializing Game Server... ");
                GameServer.Initialize();
                this.Log("Done.");

                this.Loga("Initializing Maple Data... ");
                MapleData.Initialize();
                this.Log("Done.");

                new Delay(15 * 1000, () =>
                {
                    GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized);
                }).Execute();

                this.Log("");

                this.Invoke(new MethodInvoker(() =>
                {
                    this.Text += string.Format(" ({0})", Program.ConfigurationFile);
                }));
            }
            catch (Exception e)
            {
                this.Log("Failed.");
                this.Log("");
                this.Log(e.ToString());
            }
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
                this.textBoxLog.ScrollToCaret();
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
                this.textBoxLog.ScrollToCaret();
            }
        }

        public void LogPacket(Packet inPacket)
        {
            byte[] value = inPacket.ToArray();

            StringBuilder sb = new StringBuilder();

            sb.Append(string.Format("Unhandled packet received ({0}): ", Enum.IsDefined(typeof(ClientMessages), inPacket.OperationCode) ? Enum.GetName(typeof(ClientMessages), inPacket.OperationCode) : string.Format("0x{0:X2}", inPacket.OperationCode)));
            sb.Append('\n');

            if (value == null || value.Length == 0)
            {
                sb.Append("(Empty)");
            }
            else
            {
                int lineSeparation = 0;

                foreach (byte b in value)
                {
                    if (lineSeparation == 16)
                    {
                        sb.Append('\n');
                        lineSeparation = 0;
                    }

                    sb.AppendFormat("{0:X2} ", b);
                    lineSeparation++;
                }
            }

            this.Log(sb.ToString());

        }
    }
}
