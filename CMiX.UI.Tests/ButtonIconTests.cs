using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls;
using CMiX.UI.Controls.Boolean;
using CMiX.UI.Popups;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // ButtonIcon is a Button with one icon. Its Tip is its tooltip.
    public class ButtonIconTests
    {
        [AvaloniaFact]
        public void IconKey_Set_ShowsTheIcon()
        {
            var button = new ButtonIcon { IconKey = "Add" };
            TestWindow.Show(button, 100, 100);

            var icon = button.GetVisualDescendants().OfType<AppIcon>().Single();
            Assert.Equal("Add", icon.IconKey);

            button.IconKey = "Remove";
            Assert.Equal("Remove", icon.IconKey);
        }

        [AvaloniaFact]
        public void Tip_WhenSet_GivesATooltip()
        {
            var button = new ButtonIcon { IconKey = "Add", Tip = "Add" };

            Assert.Equal("Add", ToolTip.GetTip(button));
        }

        [AvaloniaFact]
        public void Tip_WhenEmpty_GivesNoTooltip()
        {
            var button = new ButtonIcon { IconKey = "Add", Tip = "Add" };

            button.Tip = string.Empty;

            Assert.Null(ToolTip.GetTip(button));
        }

        [AvaloniaFact]
        public void Tip_WithAShortcut_GivesATipContentWithThatShortcut()
        {
            var button = new ButtonIcon { IconKey = "Add", Tip = "Add", TipShortcut = "Shift+A" };

            var tip = Assert.IsType<TipContent>(ToolTip.GetTip(button));
            Assert.Equal(new TipContent("Add", "Shift+A"), tip);
        }

        [AvaloniaFact]
        public void TipShortcut_Changed_ChangesTheTooltip()
        {
            var button = new ButtonIcon { IconKey = "Add", Tip = "Add", TipShortcut = "Shift+A" };

            button.TipShortcut = "Ctrl+N";
            Assert.Equal(new TipContent("Add", "Ctrl+N"), ToolTip.GetTip(button));

            button.TipShortcut = null;
            Assert.Equal("Add", ToolTip.GetTip(button));
        }

        [AvaloniaFact]
        public void DisabledReason_WhileTheButtonIsDisabled_ShowsInTheTooltip()
        {
            var button = new ButtonIcon { IconKey = "Remove", Tip = "Remove", DisabledReason = "Select a row to remove." };
            Assert.Equal("Remove", ToolTip.GetTip(button));

            button.IsEnabled = false;

            Assert.Equal(new TipContent("Remove", Reason: "Select a row to remove."), ToolTip.GetTip(button));
            Assert.True(ToolTip.GetShowOnDisabled(button));

            button.IsEnabled = true;
            Assert.Equal("Remove", ToolTip.GetTip(button));
        }

        [AvaloniaFact]
        public void DisabledReason_WhenACommandCannotRun_ShowsInTheTooltip()
        {
            var command = new CommunityToolkit.Mvvm.Input.RelayCommand(() => { }, () => false);
            var button = new ButtonIcon { IconKey = "Remove", Tip = "Remove", DisabledReason = "Nothing to remove.", Command = command };
            TestWindow.Show(button, 100, 100);

            Assert.False(button.IsEffectivelyEnabled);
            Assert.Equal(new TipContent("Remove", Reason: "Nothing to remove."), ToolTip.GetTip(button));
        }

        [AvaloniaFact]
        public void Tip_WithoutAReason_DoesNotShowOnADisabledButton()
        {
            var button = new ButtonIcon { IconKey = "Add", Tip = "Add" };

            Assert.False(ToolTip.GetShowOnDisabled(button));
        }
    }
}
