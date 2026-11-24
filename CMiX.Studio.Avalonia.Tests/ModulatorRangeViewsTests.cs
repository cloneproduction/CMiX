// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using CMiX.Studio.Avalonia.Views.Managers;
using CMiX.UI.Controls.Collection;
using CMiX.UI.Controls.Number;
using CMiX.UI.Controls.Parts;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using ViewFFTModulator = CMiX.Studio.Avalonia.Views.FFTModulator;
using ViewLISAModulator = CMiX.Studio.Avalonia.Views.LISAModulator;
using ViewTrackingModulator = CMiX.Studio.Avalonia.Views.TrackingModulator;
using ViewVisitorDataModulator = CMiX.Studio.Avalonia.Views.VisitorDataModulator;

namespace CMiX.Studio.Avalonia.Tests
{
    // The views of the modulators that show one range value for each axis, band or value.
    public class ModulatorRangeViewsTests : IDisposable
    {
        public enum Kind { LISA, Tracking, VisitorData, FFT }

        public static TheoryData<Kind> Kinds => new() { Kind.LISA, Kind.Tracking, Kind.VisitorData, Kind.FFT };

        // The view of a kind: its number of ranges and captions, the defaults of the first and the last range, a value for the
        // first range and the range of the model that it sets.
        private sealed record Case(Type Modulator, Func<Control> NewView, int Ranges, int Captions,
            (double Left, double Right) First, (double Left, double Right) Last, (double Left, double Right) Set,
            Func<IModulator, (float Minimum, float Maximum)> FirstRangeOf);

        private static Case CaseOf(Kind kind) => kind switch
        {
            Kind.LISA => new(typeof(LISAModulator), () => new ViewLISAModulator(), 2, 3, (-1, 1), (1, -1), (-2, 2),
                modulator => (((LISAModulator)modulator).XRange.Minimum.Value, ((LISAModulator)modulator).XRange.Maximum.Value)),
            Kind.Tracking => new(typeof(TrackingModulator), () => new ViewTrackingModulator(), 2, 3, (-1, 1), (1, -1), (-2, 2),
                modulator => (((TrackingModulator)modulator).XRange.Minimum.Value, ((TrackingModulator)modulator).XRange.Maximum.Value)),
            Kind.VisitorData => new(typeof(VisitorDataModulator), () => new ViewVisitorDataModulator(), 6, 7, (0, 1), (1, -1), (40, 200),
                modulator => (((VisitorDataModulator)modulator).HeartRateRange.Minimum.Value, ((VisitorDataModulator)modulator).HeartRateRange.Maximum.Value)),
            // Four band captions and the caption of the track index.
            _ => new(typeof(FFTModulator), () => new ViewFFTModulator(), 4, 5, (0, 1), (0, 1), (-2, 2),
                modulator => (((FFTModulator)modulator).BassRange.Minimum.Value, ((FFTModulator)modulator).BassRange.Maximum.Value)),
        };

        private readonly IServiceProvider provider = TestServiceProviderFactory.Create();
        private readonly List<Window> windows = new();

        public void Dispose()
        {
            foreach (var window in windows)
                window.Close();
        }

        private IModulator NewModulator(Type type)
        {
            var blur = (Blur)provider.GetRequiredService<ControlFactory>().Create(typeof(Blur));
            blur.ModulatorManager.AddItem(type);
            return (IModulator)blur.ModulatorManager.SelectedItem;
        }

        private Control Show(Case shown, IModulator modulator)
        {
            var view = shown.NewView();
            view.DataContext = modulator;
            var window = new Window { Content = view, Width = 400, Height = 400 };
            windows.Add(window);
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return view;
        }

        private static List<RangeValue> RangeValues(Control view) =>
            view.GetVisualDescendants().OfType<RangeValue>().ToList();

        [AvaloniaTheory]
        [MemberData(nameof(Kinds))]
        public void Show_AModulator_HasOneRangeValuePerAxisBandOrValue(Kind kind)
        {
            var shown = CaseOf(kind);
            var view = Show(shown, NewModulator(shown.Modulator));

            Assert.Equal(shown.Ranges, RangeValues(view).Count);
            Assert.Equal(shown.Captions, view.GetVisualDescendants().OfType<ContentCaptioned>().Count());
        }

        [AvaloniaTheory]
        [MemberData(nameof(Kinds))]
        public void Show_AFreshModulator_ShowsTheDefaults(Kind kind)
        {
            var shown = CaseOf(kind);
            var view = Show(shown, NewModulator(shown.Modulator));

            var ranges = RangeValues(view);
            Assert.Equal(shown.First, (ranges.First().LeftValue, ranges.First().RightValue));
            Assert.Equal(shown.Last, (ranges.Last().LeftValue, ranges.Last().RightValue));
        }

        [AvaloniaTheory]
        [MemberData(nameof(Kinds))]
        public void SetValue_OnTheFirstRangeValue_ChangesTheFirstRange(Kind kind)
        {
            var shown = CaseOf(kind);
            var modulator = NewModulator(shown.Modulator);
            var view = Show(shown, modulator);

            var first = RangeValues(view).First();
            first.LeftValue = shown.Set.Left;
            first.RightValue = shown.Set.Right;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(((float)shown.Set.Left, (float)shown.Set.Right), shown.FirstRangeOf(modulator));
        }

        [AvaloniaFact]
        public void Show_AFreshFFTModulator_ShowsZeroToOneOnEveryBand()
        {
            var shown = CaseOf(Kind.FFT);
            var view = Show(shown, NewModulator(shown.Modulator));

            Assert.All(RangeValues(view), range =>
            {
                Assert.Equal(0.0, range.LeftValue);
                Assert.Equal(1.0, range.RightValue);
            });
        }

        [AvaloniaFact]
        public void SetValue_OnTheLastRangeValueOfAnFFTModulator_ChangesTheHighRange()
        {
            var shown = CaseOf(Kind.FFT);
            var modulator = (FFTModulator)NewModulator(shown.Modulator);
            var view = Show(shown, modulator);

            var highRange = RangeValues(view).Last();
            highRange.LeftValue = 3.0;
            highRange.RightValue = 4.0;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(3f, modulator.HighRange.Minimum.Value);
            Assert.Equal(4f, modulator.HighRange.Maximum.Value);
        }

        [AvaloniaFact]
        public void Build_TheListItemOfALISAModulator_IsAListItemIcon()
        {
            var templates = new PrefabListItemTemplates();
            var modulator = NewModulator(typeof(LISAModulator));

            Assert.True(templates.Match(modulator));
            Assert.IsType<ListItemIcon>(templates.Build(modulator));
        }
    }
}
