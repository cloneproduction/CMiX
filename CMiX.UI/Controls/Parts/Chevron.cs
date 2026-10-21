// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using CMiX.UI.Themes;

namespace CMiX.UI.Controls.Parts
{
    // The one chevron of the application: an expander, a drop down and any other control that opens use it.
    // It is the same icon, size, color and opacity everywhere, and it turns when IsOpen changes.
    // It points right when closed and down when open. A drop down points down when closed and up when open:
    //     <ui:Chevron IsOpen="{Binding IsChecked, RelativeSource={RelativeSource TemplatedParent}}" ClosedAngle="0" OpenAngle="180" />
    public class Chevron : TemplatedControl
    {
        public static readonly StyledProperty<bool> IsOpenProperty =
            AvaloniaProperty.Register<Chevron, bool>(nameof(IsOpen));

        public bool IsOpen
        {
            get => GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }

        // The angle in degrees of the icon ChevronDown, which points down at 0.
        public static readonly StyledProperty<double> ClosedAngleProperty =
            AvaloniaProperty.Register<Chevron, double>(nameof(ClosedAngle), -90);

        public double ClosedAngle
        {
            get => GetValue(ClosedAngleProperty);
            set => SetValue(ClosedAngleProperty, value);
        }

        public static readonly StyledProperty<double> OpenAngleProperty =
            AvaloniaProperty.Register<Chevron, double>(nameof(OpenAngle), 0);

        public double OpenAngle
        {
            get => GetValue(OpenAngleProperty);
            set => SetValue(OpenAngleProperty, value);
        }

        // The angle that is shown. The theme reads it, and the turn is a transition of this property.
        public static readonly StyledProperty<double> AngleProperty =
            AvaloniaProperty.Register<Chevron, double>(nameof(Angle), -90);

        public double Angle
        {
            get => GetValue(AngleProperty);
            private set => SetValue(AngleProperty, value);
        }

        public static readonly StyledProperty<TimeSpan> DurationProperty =
            AvaloniaProperty.Register<Chevron, TimeSpan>(nameof(Duration), Motion.Duration);

        public TimeSpan Duration
        {
            get => GetValue(DurationProperty);
            set => SetValue(DurationProperty, value);
        }

        static Chevron()
        {
            IsOpenProperty.Changed.AddClassHandler<Chevron>((chevron, _) => chevron.UpdateAngle());
            ClosedAngleProperty.Changed.AddClassHandler<Chevron>((chevron, _) => chevron.UpdateAngle());
            OpenAngleProperty.Changed.AddClassHandler<Chevron>((chevron, _) => chevron.UpdateAngle());
            DurationProperty.Changed.AddClassHandler<Chevron>((chevron, _) => chevron.UpdateTransition());
            AngleProperty.Changed.AddClassHandler<Chevron>((chevron, _) => chevron.rotation.Angle = chevron.Angle);
        }

        private readonly RotateTransform rotation = new();

        public Chevron()
        {
            // The state of the first layout shows at once. The turn starts after that.
            Loaded += (_, _) => UpdateTransition();
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            rotation.Angle = Angle;
            if (e.NameScope.Find<AppIcon>("PART_Icon") is { } icon)
                icon.RenderTransform = rotation;
        }

        private void UpdateAngle() => Angle = IsOpen ? OpenAngle : ClosedAngle;

        private void UpdateTransition()
        {
            if (!IsLoaded)
                return;

            Transitions = new Transitions { new DoubleTransition { Property = AngleProperty, Duration = Duration, Easing = Motion.Easing } };
        }
    }
}
