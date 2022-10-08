using System;
using System.Windows.Controls;

namespace CMiX.Studio.Views
{
    public partial class MaterialManager : UserControl
    {
        public MaterialManager()
        {
            InitializeComponent();
        }

        private void StackPanel_RequestBringIntoView(object sender, System.Windows.RequestBringIntoViewEventArgs e)
        {
            //e.Handled = true;
        }

        double extendHeight = 0;
        double verticalOffset = 0;
        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            //Console.WriteLine(" --------- ");
            //Console.WriteLine("VerticalOffset " + e.VerticalOffset);
            //Console.WriteLine("ViewportHeight " + e.ViewportHeight);
            //Console.WriteLine("ExtentHeight " + e.ExtentHeight);
            //Console.WriteLine("ExtentHeightChange " + e.ExtentHeightChange);
            //Console.WriteLine("ScrollableHeight " + scrollViewer.ScrollableHeight);

            //if (e.ExtentHeightChange == 0.0)
            //{
            //    return;
            //}

            //if(extendHeight <= e.ViewportHeight)
            //{
            //    scrollViewer.ScrollToTop();

            //}

            //if(verticalOffset == extendHeight - e.ViewportHeight)
            //{
            //    this.scrollViewer.ScrollToVerticalOffset(verticalOffset);
            //}

            //verticalOffset = e.VerticalOffset;
            //extendHeight = e.ExtentHeight;
        }
    }
}
