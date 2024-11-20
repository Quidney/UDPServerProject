using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using UDPServerLibrary.Model;
using UDPServerLibrary.Utils;
using UDPTesterConsole;

namespace UDPTesterGUI
{
    public partial class GameWindow : Form
    {
        IPEndPoint ServerEP = new(IPAddress.Loopback, 23345);
        UdpNode Client = null!;
        Color backgroundColor = Color.Black;
        IEncoder Encoder;


        int receivedPackages = 0;

        public GameWindow()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            Encoder = new Encoder(System.Text.Encoding.UTF8);
        }

        protected override async void OnShown(EventArgs e)
        {
            base.OnShown(e);

            
            Client = new(Encoder);
            Client.MessageReceived += Client_MessageReceived;

            Package pkg = new(Command.CONNECT, "23346");
            string dataJson = pkg.Serialize();
            await Client.SendMessage(ServerEP, dataJson);

            _ = Task.Run(() => Client.StartListening(23346));
        }

        private void Client_MessageReceived(IPEndPoint sender, byte[] message)
        {
            Debug.WriteLine(message);

            receivedPackages++;

            string dataJson = Encoder.BytesToString(message);

            Color color = Color.FromName(dataJson);
            backgroundColor = color;
            Refresh();
        }

        override protected void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            //e.Graphics.Clear(backgroundColor);

            string text = receivedPackages.ToString();
            Font font = new("Arial", 24, FontStyle.Bold);
            Brush brush = new SolidBrush(Color.FromArgb(backgroundColor.ToArgb() ^ 0xffffff));
            Size strSize = e.Graphics.MeasureString(text, font).ToSize();
            int x = ClientSize.Width / 2 - strSize.Width / 2;
            int y = ClientSize.Height / 2 - strSize.Height / 2;

            e.Graphics.DrawString(text, font, brush, new Point(x, y));
        }

        protected override async void OnClosing(CancelEventArgs e)
        {
            await Client.SendMessage(ServerEP, Command.DISCONNECT, "23346");
            base.OnClosing(e);
        }
    }
}