// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;
using Avalonia.Media;
using CMiX.UI.Popups;

namespace CMiX.UI.Controls.Parts
{
    // The card in the template of a tooltip. It is a PopupCard that moves its tip where the placement of the tooltip says.
    // The tooltip of a control is placed by ToolTipDefaults, which writes the result on the control (see ResultProperty). The card finds the
    // control through the popup of the tooltip, and follows that value for as long as the tooltip shows.
    public class ToolTipCard : PopupCard
    {
        // The last placement of the tooltip of a control. ToolTipDefaults writes it, and the card of the tooltip reads it.
        public static readonly AttachedProperty<PlacementResult?> ResultProperty =
            AvaloniaProperty.RegisterAttached<ToolTipCard, Control, PlacementResult?>("Result");

        public static void SetResult(Control control, PlacementResult? value) => control.SetValue(ResultProperty, value);
        public static PlacementResult? GetResult(Control control) => control.GetValue(ResultProperty);

        // The fill of the tooltips of a control and of everything under it: a row of a tree gives its tooltips its own colour. The value
        // is inherited, so it crosses the popup of the tooltip, which a style of the control would not (a popup takes the styles of the
        // application only). Null keeps the fill of the theme (PopupBackgroundBrush).
        public static readonly AttachedProperty<IBrush?> FillProperty =
            AvaloniaProperty.RegisterAttached<ToolTipCard, Control, IBrush?>("Fill", inherits: true);

        public static void SetFill(Control control, IBrush? value) => control.SetValue(FillProperty, value);

        public static IBrush? GetFill(Control control) => control.GetValue(FillProperty);

        static ToolTipCard()
        {
            FillProperty.Changed.AddClassHandler<ToolTipCard>((card, e) =>
            {
                if (e.NewValue is IBrush fill)
                    card.Background = fill;
                else
                    card.ClearValue(BackgroundProperty);
            });
        }

        private IDisposable? following;

        protected override Type StyleKeyOverride => typeof(PopupCard);

        protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
        {
            base.OnAttachedToLogicalTree(e);
            Follow();
        }

        protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
        {
            Stop();
            base.OnDetachedFromLogicalTree(e);
        }

        protected override void OnAttachedToVisualTree(Avalonia.VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            Follow();
        }

        protected override void OnDetachedFromVisualTree(Avalonia.VisualTreeAttachmentEventArgs e)
        {
            Stop();
            base.OnDetachedFromVisualTree(e);
        }

        private void Stop()
        {
            following?.Dispose();
            following = null;
        }

        // The control of the tooltip is the target of the popup that holds the tooltip.
        private void Follow()
        {
            Stop();
            if (this.FindLogicalAncestorOfType<Popup>()?.PlacementTarget is not { } target)
                return;

            following = target.GetObservable(ResultProperty).Subscribe(new Observer(this));
        }

        private sealed class Observer : IObserver<PlacementResult?>
        {
            private readonly ToolTipCard card;

            public Observer(ToolTipCard card) => this.card = card;

            public void OnNext(PlacementResult? value)
            {
                if (value is { } result)
                    card.Place(result);
            }

            public void OnCompleted() { }
            public void OnError(Exception error) { }
        }
    }
}
