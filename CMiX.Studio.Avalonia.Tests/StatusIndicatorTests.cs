using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The colors of the sync dots in the main window and in the sync settings.
    public class StatusIndicatorTests
    {
        private static Ellipse Dot(string themeKey)
        {
            var dot = new Ellipse
            {
                Width = 10,
                Height = 10,
                Theme = (ControlTheme)Application.Current!.FindResource(themeKey)!
            };
            var window = new Window { Content = dot, Width = 50, Height = 50 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return dot;
        }

        private static Color ColorOf(Ellipse dot)
        {
            Dispatcher.UIThread.RunJobs();
            return ((ISolidColorBrush)dot.Fill!).Color;
        }

        [AvaloniaFact]
        public void ConnectionDot_IsRedWithoutAConnection()
        {
            Assert.Equal(Colors.Red, ColorOf(Dot("EllipseSyncConnection")));
        }

        [AvaloniaFact]
        public void ConnectionDot_IsGreenWhenConnected()
        {
            var dot = Dot("EllipseSyncConnection");
            dot.Classes.Add("connected");

            Assert.Equal(Colors.Green, ColorOf(dot));
        }

        [AvaloniaFact]
        public void ConnectionDot_IsGrayWhileConnecting_EvenWhenConnected()
        {
            var dot = Dot("EllipseSyncConnection");
            dot.Classes.Add("connected");
            dot.Classes.Add("connecting");

            Assert.Equal(Colors.Gray, ColorOf(dot));
        }

        [AvaloniaFact]
        public void ConnectionDot_IsOrangeWhenConnectedButNotInSync()
        {
            var dot = Dot("EllipseSyncConnection");
            dot.Classes.Add("connected");
            dot.Classes.Add("unsynced");

            Assert.Equal(Colors.Orange, ColorOf(dot));
        }

        [AvaloniaFact]
        public void ConnectionDot_IsRed_WhenNotInSyncAndNotConnected()
        {
            var dot = Dot("EllipseSyncConnection");
            dot.Classes.Add("unsynced");

            Assert.Equal(Colors.Red, ColorOf(dot));
        }

        [AvaloniaFact]
        public void ConnectionDot_IsGrayWhileConnecting_EvenWhenNotInSync()
        {
            var dot = Dot("EllipseSyncConnection");
            dot.Classes.Add("connected");
            dot.Classes.Add("unsynced");
            dot.Classes.Add("connecting");

            Assert.Equal(Colors.Gray, ColorOf(dot));
        }

        [AvaloniaFact]
        public void StateDot_IsGrayUntilInSync_ThenBlue()
        {
            var dot = Dot("EllipseSyncState");
            Assert.Equal(Colors.Gray, ColorOf(dot));

            dot.Classes.Add("insync");

            Assert.Equal(Colors.DodgerBlue, ColorOf(dot));
        }
    }
}
