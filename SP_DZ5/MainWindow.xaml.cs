using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SP_DZ5
{
    public partial class MainWindow : Window
    {

        private BlockingCollection<string> mouseMovements;
        private CancellationTokenSource cancel = new();

        public MainWindow()
        {
            InitializeComponent();
            mouseMovements = new BlockingCollection<string>();
            Task.Run(() => ProcessMouseMovements(cancel.Token), cancel.Token);
        }
        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var position = e.GetPosition(this);
            string movementInfo = $"{DateTime.Now}: X={position.X}, Y={position.Y}";
            mouseMovements.Add(movementInfo);
        }

        private void ProcessMouseMovements(CancellationToken token)
        {
            using (StreamWriter writer = new StreamWriter("mouseMovements.txt", true))
            {
                while (true)
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                    string movementInfo = mouseMovements.Take();
                    writer.WriteLine(movementInfo);
                    writer.Flush();
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            cancel.Cancel();
        }
    }
}