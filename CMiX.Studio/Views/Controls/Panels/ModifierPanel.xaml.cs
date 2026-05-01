using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CMiX.Studio.Views.Controls.Panels
{
    public partial class ModifierPanel : UserControl
    {
        public ModifierPanel()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ResetModifierCommandProperty =
        DependencyProperty.Register("ResetModifierCommand", typeof(ICommand), typeof(ModifierPanel), new FrameworkPropertyMetadata());
        public ICommand ResetModifierCommand
        {
            get { return (ICommand)GetValue(ResetModifierCommandProperty); }
            set { SetValue(ResetModifierCommandProperty, value); }
        }

        public static readonly DependencyProperty ResetModifierCommandParameterProperty =
        DependencyProperty.Register("ResetModifierCommandParameter", typeof(ICommand), typeof(ModifierPanel), new FrameworkPropertyMetadata());
        public ICommand ResetModifierCommandParameter
        {
            get { return (ICommand)GetValue(ResetModifierCommandParameterProperty); }
            set { SetValue(ResetModifierCommandParameterProperty, value); }
        }


        public static readonly DependencyProperty CloseModifierCommandProperty =
        DependencyProperty.Register("CloseModifierCommand", typeof(ICommand), typeof(ModifierPanel), new FrameworkPropertyMetadata());
        public ICommand CloseModifierCommand
        {
            get { return (ICommand)GetValue(CloseModifierCommandProperty); }
            set { SetValue(CloseModifierCommandProperty, value); }
        }


        public static readonly DependencyProperty CloseModifierCommandParameterProperty =
        DependencyProperty.Register("CloseModifierCommandParameter", typeof(object), typeof(ModifierPanel), new FrameworkPropertyMetadata());
        public object CloseModifierCommandParameter
        {
            get { return (object)GetValue(CloseModifierCommandParameterProperty); }
            set { SetValue(CloseModifierCommandParameterProperty, value); }
        }


        public static readonly DependencyProperty DragHandlerIsPressedProperty =
        DependencyProperty.Register("DragHandlerIsPressed", typeof(bool), typeof(ModifierPanel), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public bool DragHandlerIsPressed
        {
            get { return (bool)GetValue(DragHandlerIsPressedProperty); }
            set { SetValue(DragHandlerIsPressedProperty, value); }
        }

        private void Button_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragHandlerIsPressed = true;
        }

        private void Button_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DragHandlerIsPressed = false;
        }
    }
}
