using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CMiX.Studio.Views
{
    public partial class VideoPathSelector : UserControl
    {
        public VideoPathSelector()
        {
            InitializeComponent();
        }

        private void OnRequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Down) || Keyboard.IsKeyDown(Key.Up))
                return;

            e.Handled = true;
        }
    }
}
