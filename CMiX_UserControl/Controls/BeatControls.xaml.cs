﻿using System.Windows.Controls;

namespace CMiX
{
    public partial class BeatControls : UserControl
    {
        public BeatControls()
        {
            InitializeComponent();
        }

        #region Properties

        /*public static readonly DependencyProperty MultiplierProperty =
        DependencyProperty.Register("Multiplier", typeof(double), typeof(BeatControls), new PropertyMetadata(1.0, new PropertyChangedCallback(PropertyChanged)));
        [Bindable(true)]
        [Obsolete("Use viewmodel")]
        public double Multiplier
        {
            get { return (double)GetValue(MultiplierProperty); }
            set { SetValue(MultiplierProperty, value); }
        }

        public static readonly DependencyProperty MasterPeriodProperty =
        DependencyProperty.Register("MasterPeriod", typeof(double), typeof(BeatControls), new PropertyMetadata(0.0, new PropertyChangedCallback(PropertyChanged)));
        [Bindable(true)]
        [Obsolete("Use viewmodel")]
        public double MasterPeriod
        {
            get { return (double)GetValue(MasterPeriodProperty); }
            set { SetValue(MasterPeriodProperty , value) ; }
        }

        public static readonly DependencyProperty ControlPeriodProperty =
        DependencyProperty.Register("ControlPeriod", typeof(double), typeof(BeatControls), new PropertyMetadata(0.0));
        [Bindable(true)]
        [Obsolete("Use viewmodel")]
        public double ControlPeriod
        {
            get { return (double)GetValue(ControlPeriodProperty); }
            set { SetValue(ControlPeriodProperty, value); }
        }*/
        #endregion

        #region Events
        /*private static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var beatcontrol = d as BeatControls;
            if(beatcontrol != null)
            {
                beatcontrol.ControlPeriod = beatcontrol.MasterPeriod * beatcontrol.Multiplier;
            }
        }*/
        /*

        private void BeatMultiply_Click(object sender, RoutedEventArgs e)
        {
            Multiplier /= 2.0;
            //OnBeatControlChanged(e);
        }

        private void BeatDivide_Click(object sender, RoutedEventArgs e)
        {
            Multiplier *= 2.0;
            //OnBeatControlChanged(e);
        }

        private void ResetBPM_Click(object sender, RoutedEventArgs e)
        {
            Multiplier = 1.0;
            //OnBeatControlChanged(e);
        }
        */
        #endregion
    }
}
