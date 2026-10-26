using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls;
using CMiX.UI.Controls.Collection;
using CMiX.UI.Controls.String;
using Xunit;

namespace CMiX.UI.Tests
{
    // ListItemIcon is a row with an icon, a name and a toggle. It reads only its own properties.
    public class ListItemIconTests
    {
        private static ListItemIcon Show(ListItemIcon item)
        {
            new Window { Content = item, Width = 300, Height = 60 }.Show();
            Dispatcher.UIThread.RunJobs();
            return item;
        }

        [AvaloniaFact]
        public void ShowsItsIcon()
        {
            var item = Show(new ListItemIcon { Icon = "Folder" });

            Assert.Equal("Folder", item.GetVisualDescendants().OfType<AppIcon>().First().IconKey);
        }

        [AvaloniaFact]
        public void TheTitle_IsShownInTheEditableName_AndWrittenBack()
        {
            var item = Show(new ListItemIcon { Title = "Beach" });
            var name = item.GetVisualDescendants().OfType<TextBoxEditable>().Single();
            Assert.Equal("Beach", name.Text);

            name.Text = "Forest";
            Dispatcher.UIThread.RunJobs();

            Assert.Equal("Forest", item.Title);
        }

        [AvaloniaFact]
        public void TheToggle_FollowsIsChecked_AndWritesIt()
        {
            var item = Show(new ListItemIcon());
            var toggle = item.GetVisualDescendants().OfType<ToggleButton>().First();

            item.IsChecked = true;
            Dispatcher.UIThread.RunJobs();
            Assert.True(toggle.IsChecked);

            toggle.IsChecked = false;
            Dispatcher.UIThread.RunJobs();
            Assert.False(item.IsChecked);
        }

        [AvaloniaFact]
        public void TheItem_HasOnlyTheVisibilityToggle()
        {
            var item = Show(new ListItemIcon());

            Assert.Single(item.GetVisualDescendants().OfType<ToggleButton>());
        }
    }
}
