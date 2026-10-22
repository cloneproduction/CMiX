// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using CMiX.UI.Themes;

namespace CMiX.UI.Panels
{
    // Opens and closes its content with an animation. Bind IsExpanded and the panel does the rest:
    //     <ui:CollapsePanel IsExpanded="{TemplateBinding IsExpanded}"> ... </ui:CollapsePanel>
    // The panel measures its content once at full size, and its own height is that height times Progress.
    // So the animation does not lay out the content again, also when the content is large. The content is clipped while the panel moves.
    // The panel owns the IsVisible of its content: the content is hidden when the panel is closed, so it takes no focus
    // and no pointer. It is shown before an opening starts, because a hidden control has no size.
    // The first state shows at once. The animation starts after the panel is loaded.
    public class CollapsePanel : Decorator
    {
        public static readonly StyledProperty<bool> IsExpandedProperty =
            AvaloniaProperty.Register<CollapsePanel, bool>(nameof(IsExpanded));

        public bool IsExpanded
        {
            get => GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }

        // How much of the content shows, from 0 (closed) to 1 (open). The animation is a transition of this property.
        public static readonly StyledProperty<double> ProgressProperty =
            AvaloniaProperty.Register<CollapsePanel, double>(nameof(Progress));

        public double Progress
        {
            get => GetValue(ProgressProperty);
            set => SetValue(ProgressProperty, value);
        }

        public static readonly StyledProperty<TimeSpan> DurationProperty =
            AvaloniaProperty.Register<CollapsePanel, TimeSpan>(nameof(Duration), Motion.Duration);

        public TimeSpan Duration
        {
            get => GetValue(DurationProperty);
            set => SetValue(DurationProperty, value);
        }

        static CollapsePanel()
        {
            AffectsMeasure<CollapsePanel>(ProgressProperty);

            IsExpandedProperty.Changed.AddClassHandler<CollapsePanel>((panel, _) => panel.OnIsExpandedChanged());
            ProgressProperty.Changed.AddClassHandler<CollapsePanel>((panel, _) => panel.OnProgressChanged());
            ChildProperty.Changed.AddClassHandler<CollapsePanel>((panel, _) => panel.UpdateChildVisibility());
            DurationProperty.Changed.AddClassHandler<CollapsePanel>((panel, _) => panel.UpdateTransition());
        }

        public CollapsePanel()
        {
            Loaded += (_, _) => UpdateTransition();
        }

        private void OnIsExpandedChanged()
        {
            UpdateChildVisibility();
            Progress = IsExpanded ? 1 : 0;
        }

        // The content is clipped only while the panel moves. At rest it is open and free, as it was without the panel.
        private void OnProgressChanged()
        {
            ClipToBounds = Progress < 1;
            UpdateChildVisibility();
        }

        private void UpdateChildVisibility()
        {
            if (Child != null)
                Child.IsVisible = IsExpanded || Progress > 0;
        }

        private void UpdateTransition()
        {
            if (!IsLoaded)
                return;

            Transitions = new Transitions { new DoubleTransition { Property = ProgressProperty, Duration = Duration, Easing = Motion.Easing } };
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Child == null)
                return default;

            Child.Measure(new Size(availableSize.Width, double.PositiveInfinity));
            return new Size(Child.DesiredSize.Width, Child.DesiredSize.Height * Progress);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Child?.Arrange(new Rect(0, 0, finalSize.Width, Child.DesiredSize.Height));
            return finalSize;
        }
    }
}
