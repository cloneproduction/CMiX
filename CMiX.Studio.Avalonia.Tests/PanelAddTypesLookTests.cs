// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Views.Managers;
using Xunit;
using Xunit.Abstractions;

namespace CMiX.Studio.Avalonia.Tests
{
    public interface IThreeAddables
    {
    }

    public interface IFiveAddables
    {
    }

    public interface ITenAddables
    {
    }

    public class ThreeAddableA : IThreeAddables
    {
    }

    public class ThreeAddableB : IThreeAddables
    {
    }

    public class ThreeAddableC : IThreeAddables
    {
    }

    public class FiveAddableA : IFiveAddables
    {
    }

    public class FiveAddableB : IFiveAddables
    {
    }

    public class FiveAddableC : IFiveAddables
    {
    }

    public class FiveAddableD : IFiveAddables
    {
    }

    public class FiveAddableE : IFiveAddables
    {
    }

    public class TenAddableA : ITenAddables
    {
    }

    public class TenAddableB : ITenAddables
    {
    }

    public class TenAddableC : ITenAddables
    {
    }

    public class TenAddableD : ITenAddables
    {
    }

    public class TenAddableE : ITenAddables
    {
    }

    public class TenAddableF : ITenAddables
    {
    }

    public class TenAddableG : ITenAddables
    {
    }

    public class TenAddableH : ITenAddables
    {
    }

    public class TenAddableI : ITenAddables
    {
    }

    public class TenAddableJ : ITenAddables
    {
    }

    // The add panel puts its buttons in one, two or three equal columns.
    public class PanelAddTypesLookTests
    {
        private readonly ITestOutputHelper output;

        public PanelAddTypesLookTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        private static PanelAddTypes ShowInWindow(Type interfaceType)
        {
            var panel = new PanelAddTypes { Interface = interfaceType };
            new Window { Content = panel, Width = 400, Height = 400 }.Show();
            Dispatcher.UIThread.RunJobs();
            return panel;
        }

        private static PanelAddTypes ShowInARow(Type interfaceType)
        {
            var panel = new PanelAddTypes { Interface = interfaceType };
            var row = new StackPanel { Orientation = Orientation.Horizontal, Children = { panel } };
            new Window { Content = row, Width = 400, Height = 400 }.Show();
            Dispatcher.UIThread.RunJobs();
            return panel;
        }

        private string BoundsOf(PanelAddTypes panel)
        {
            var bounds = panel.GetVisualDescendants().OfType<Button>()
                .Select(button => $"{button.Content}={new Rect(button.TranslatePoint(default, panel)!.Value, button.Bounds.Size)}");
            var text = $"{panel.Bounds.Size}|{string.Join("|", bounds)}";
            output.WriteLine(text);
            return text;
        }

        [AvaloniaFact]
        public void Show_ThreeTypesInAWindow_PutsTheButtonsInTheirColumnsAndRows()
        {
            var panel = ShowInWindow(typeof(IThreeAddables));

            Assert.Equal("400, 400|Three Addable A=4, 4, 392, 24|Three Addable B=4, 28, 392, 24|Three Addable C=4, 52, 392, 24", BoundsOf(panel));
        }

        [AvaloniaFact]
        public void Show_FiveTypesInAWindow_PutsTheButtonsInTheirColumnsAndRows()
        {
            var panel = ShowInWindow(typeof(IFiveAddables));

            Assert.Equal("400, 400|Five Addable A=4, 4, 196, 24|Five Addable B=4, 28, 196, 24|Five Addable C=4, 52, 196, 24|Five Addable D=200, 4, 196, 24|Five Addable E=200, 28, 196, 24", BoundsOf(panel));
        }

        [AvaloniaFact]
        public void Show_TenTypesInAWindow_PutsTheButtonsInTheirColumnsAndRows()
        {
            var panel = ShowInWindow(typeof(ITenAddables));

            Assert.Equal("400, 400|Ten Addable A=4, 4, 130, 24|Ten Addable B=4, 28, 130, 24|Ten Addable C=4, 52, 130, 24|Ten Addable D=4, 76, 130, 24|Ten Addable E=134, 4, 131, 24|Ten Addable F=134, 28, 131, 24|Ten Addable G=134, 52, 131, 24|Ten Addable H=134, 76, 131, 24|Ten Addable I=265, 4, 131, 24|Ten Addable J=265, 28, 131, 24", BoundsOf(panel));
        }

        [AvaloniaFact]
        public void Show_ThreeTypesInARow_PutsTheButtonsInTheirColumnsAndRows()
        {
            var panel = ShowInARow(typeof(IThreeAddables));

            Assert.Equal("106, 400|Three Addable A=4, 4, 98, 24|Three Addable B=4, 28, 98, 24|Three Addable C=4, 52, 98, 24", BoundsOf(panel));
        }

        [AvaloniaFact]
        public void Show_FiveTypesInARow_PutsTheButtonsInTheirColumnsAndRows()
        {
            var panel = ShowInARow(typeof(IFiveAddables));

            Assert.Equal("185, 400|Five Addable A=4, 4, 88, 24|Five Addable B=4, 28, 88, 24|Five Addable C=4, 52, 88, 24|Five Addable D=92, 4, 89, 24|Five Addable E=92, 28, 89, 24", BoundsOf(panel));
        }

        [AvaloniaFact]
        public void Show_TenTypesInARow_PutsTheButtonsInTheirColumnsAndRows()
        {
            var panel = ShowInARow(typeof(ITenAddables));

            Assert.Equal("263, 400|Ten Addable A=4, 4, 87, 24|Ten Addable B=4, 28, 87, 24|Ten Addable C=4, 52, 87, 24|Ten Addable D=4, 76, 87, 24|Ten Addable E=91, 4, 86, 24|Ten Addable F=91, 28, 86, 24|Ten Addable G=91, 52, 86, 24|Ten Addable H=91, 76, 86, 24|Ten Addable I=177, 4, 82, 24|Ten Addable J=177, 28, 82, 24", BoundsOf(panel));
        }
    }
}
