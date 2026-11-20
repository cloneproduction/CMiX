// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Linq;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Views.Modulation;
using CMiX.UI.Controls.Number;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    public class UserControlLimitedModulatableTests
    {
        private static TView Show<TView>() where TView : Control, new()
        {
            var view = new TView();
            var window = new Window { Content = view, Width = 400, Height = 100 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return view;
        }

        private static TInner Inner<TInner>(Control view) where TInner : Control =>
            view.GetVisualDescendants().OfType<TInner>().First();

        [AvaloniaFact]
        public void Limits_NotSet_KeepTheDefaultsOfEachEditor()
        {
            var dragValue = Show<DragValueModulatable>();
            var integerValue = Show<DragValueIntegerModulatable>();
            var slider = Show<SliderModulatable>();

            Assert.Equal(-10000.0, dragValue.Minimum);
            Assert.Equal(10000.0, dragValue.Maximum);
            Assert.Equal(-10000.0, Inner<DragValue>(dragValue).Minimum);
            Assert.Equal(10000.0, Inner<DragValue>(dragValue).Maximum);
            Assert.Equal(1.0, integerValue.Minimum);
            Assert.Equal(10000.0, integerValue.Maximum);
            Assert.Equal(1.0, Inner<DragValue>(integerValue).Minimum);
            Assert.Equal(10000.0, Inner<DragValue>(integerValue).Maximum);
            Assert.Equal(0.0, slider.Minimum);
            Assert.Equal(1.0, slider.Maximum);
            Assert.Equal(0.0, Inner<SliderCMiX>(slider).Minimum);
            Assert.Equal(1.0, Inner<SliderCMiX>(slider).Maximum);
        }
    }
}
