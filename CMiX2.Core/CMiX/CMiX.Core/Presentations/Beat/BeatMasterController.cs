// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CMiX.Core.Presentation.Controls
{
    public class MasterBeatController : Control
    {
        public MasterBeatController()
        {
            OnApplyTemplate();
        }


        public override void OnApplyTemplate()
        {
            BeatDisplay = GetTemplateChild("BeatDisplay") as Border;
            BeatDisplayCanvas = GetTemplateChild("BeatDisplayCanvas") as Canvas;

            if (BeatDisplay != null)
            {
                BeatDisplay.Width = 10;
                BeatDisplayTranslate = new TranslateTransform(0, 0);
                BeatDisplay.RenderTransform = BeatDisplayTranslate;

                sb = new Storyboard();
                da = new DoubleAnimation();
                Storyboard.SetTarget(da, BeatDisplay);
                Storyboard.SetTargetProperty(da, new PropertyPath("RenderTransform.X"));
                sb.Children.Add(da);
                sb.RepeatBehavior = RepeatBehavior.Forever;
                da.IsAdditive = false;
                SetStoryBoard(new TimeSpan(0, 0, 0, 0, Convert.ToInt32(Period)));
            }
            base.OnApplyTemplate();
        }


        //private void SetNextTickTime()
        //{
        //    if(Period > 0)
        //        NextTickTime = Stopwatch.ElapsedMilliseconds + Convert.ToInt32(Period);
        //}

        //private double NextTickTime = 0;



        //private void Animate()
        //{
        //    BeatTick++;
        //    if (BeatTick > 3)
        //    {
        //        BeatTick = 0;
        //        //StartAnimation();
        //    }
                
        //    BeatDisplay.Width = BeatDisplayCanvas.ActualWidth / 4;
        //    BeatDisplayTranslate.X = BeatDisplayCanvas.ActualWidth / 4 * BeatTick;
        //}

        private Storyboard sb { get; set; }
        private DoubleAnimation da { get; set; }
        private void SetStoryBoard(TimeSpan timeSpan)
        {
            if(sb != null)
            {
                sb.Stop();
                da.From = 0;
                da.To = 100;
                da.Duration = new Duration(timeSpan);
                sb.Begin();
            }
        }

        private TranslateTransform BeatDisplayTranslate { get; set; }
        private Canvas BeatDisplayCanvas { get; set; }
        private double BeatDisplayCanvasActualWidth { get; set; }
        private Border BeatDisplay { get; set; }

        public static readonly DependencyProperty PeriodProperty =
        DependencyProperty.Register("Period", typeof(double), typeof(MasterBeatController), new FrameworkPropertyMetadata(1000.0, new PropertyChangedCallback(OnPeriodChange)));

        private static void OnPeriodChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var mbc = (MasterBeatController)d;
            var val = (double)e.NewValue;
            if (val != double.NaN && val > 0)
            {
                mbc.sb.Stop();
                mbc.da.Duration = new Duration(TimeSpan.FromMilliseconds(Convert.ToInt32(val)));
                mbc.sb.Begin();
            }
        }

        public double Period
        {
            get { return (double)GetValue(PeriodProperty); }
            set { SetValue(PeriodProperty, value); }
        }
    }

}