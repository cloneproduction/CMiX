using System;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using CMiX.Core.Prefabs;
using CMiX.Studio.Avalonia.Tests.LocatorFixtures.Models;
using CMiX.Studio.Avalonia.Views;
using StudioViews = CMiX.Studio.Avalonia.Views;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The locator finds a view by the type name of the model, in the root namespace and in
    // its child namespaces, except the child namespaces that hold support controls.
    public class ViewModelToViewTemplateTests
    {
        private const string Root = "CMiX.Studio.Avalonia.Tests.LocatorFixtures.Screens";

        private static ViewModelToViewTemplate Fixture() =>
            new(typeof(ViewModelToViewTemplateTests).Assembly, Root, "Controls");

        [Fact]
        public void ViewInTheRootNamespace_IsFound()
        {
            Assert.True(Fixture().Match(new Alpha()));
        }

        [Fact]
        public void ViewInAChildNamespace_IsFound()
        {
            Assert.True(Fixture().Match(new Beta()));
        }

        [Fact]
        public void ViewInAnExcludedNamespace_IsIgnored()
        {
            Assert.False(Fixture().Match(new Gamma()));
        }

        [Fact]
        public void TypesThatAreNotConcreteControls_AreIgnored()
        {
            var template = Fixture();

            Assert.False(template.Match(new Delta()));
            Assert.False(template.Match(new Epsilon()));
        }

        [Fact]
        public void ModelWithoutAView_AndNull_DoNotMatch()
        {
            var template = Fixture();

            Assert.False(template.Match(new Zeta()));
            Assert.False(template.Match(null));
        }

        [AvaloniaFact]
        public void Build_CreatesTheViewOfTheModel()
        {
            var template = Fixture();

            Assert.IsType<LocatorFixtures.Screens.Alpha>(template.Build(new Alpha()));
            Assert.IsType<LocatorFixtures.Screens.Nested.Beta>(template.Build(new Beta()));
        }

        [Fact]
        public void TwoViewsWithTheSameName_AreReported()
        {
            var template = new ViewModelToViewTemplate(
                typeof(ViewModelToViewTemplateTests).Assembly,
                "CMiX.Studio.Avalonia.Tests.LocatorFixtures.Duplicates");

            var error = Assert.Throws<InvalidOperationException>(() => template.Match(new Same()));

            Assert.Contains("Same", error.Message);
        }

        [AvaloniaFact]
        public void StudioViews_AreFoundByTheirModel()
        {
            var provider = TestServiceProviderFactory.Create();
            var blur = provider.GetRequiredService<ControlFactory>().Create(typeof(CMiX.Core.Texturing.Filters.Blur));
            var template = new ViewModelToViewTemplate();

            Assert.True(template.Match(blur));
            Assert.IsType<StudioViews.Blur>(template.Build(blur));
        }
    }
}

namespace CMiX.Studio.Avalonia.Tests.LocatorFixtures.Models
{
    public class Alpha { }
    public class Beta { }
    public class Gamma { }
    public class Delta { }
    public class Epsilon { }
    public class Zeta { }
    public class Same { }
}

namespace CMiX.Studio.Avalonia.Tests.LocatorFixtures.Screens
{
    public class Alpha : Control { }
    public class Delta { }
    public abstract class Epsilon : Control { }
}

namespace CMiX.Studio.Avalonia.Tests.LocatorFixtures.Screens.Nested
{
    public class Beta : Control { }
}

namespace CMiX.Studio.Avalonia.Tests.LocatorFixtures.Screens.Controls
{
    public class Gamma : Control { }
}

namespace CMiX.Studio.Avalonia.Tests.LocatorFixtures.Duplicates.First
{
    public class Same : Control { }
}

namespace CMiX.Studio.Avalonia.Tests.LocatorFixtures.Duplicates.Second
{
    public class Same : Control { }
}
