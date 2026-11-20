using System.Globalization;
using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Modulation.Modulators;
using CMiX.Studio.Avalonia.Converters;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using Xunit;
using Image = CMiX.Core.Texturing.Sources.Image;

namespace CMiX.Studio.Avalonia.Tests
{
    // AddableTypes.LabelOf gives the label of a type in the menus of the tree and in the add panels.
    public class AddableTypesTests
    {
        [Fact]
        public void LabelOf_AModifierType_DropsTheSuffix()
        {
            Assert.Equal("Translate", AddableTypes.LabelOf(typeof(TranslateModifier)));
        }

        [Fact]
        public void LabelOf_AModulatorType_DropsTheSuffix()
        {
            Assert.Equal("LFO", AddableTypes.LabelOf(typeof(LFOModulator)));
        }

        [Fact]
        public void LabelOf_TheLISAModulator_IsLISA()
        {
            Assert.Equal("LISA", AddableTypes.LabelOf(typeof(LISAModulator)));
        }

        [Fact]
        public void LabelOf_ATextureType_KeepsTheName()
        {
            Assert.Equal("Image", AddableTypes.LabelOf(typeof(Image)));
        }

        [Fact]
        public void LabelOf_ATypeWithAModifierLabel_UsesTheLabel()
        {
            Assert.Equal("Visibility", AddableTypes.LabelOf(typeof(ComposableVisibilityModifier)));
        }

        [Fact]
        public void LabelOf_ATypeThatIsNoModifier_KeepsTheSuffix()
        {
            Assert.Equal("Plain Modifier", AddableTypes.LabelOf(typeof(PlainModifier)));
        }

        [Fact]
        public void TypeNameToDisplayLabelConverter_AnyType_GivesTheLabelOfAddableTypes()
        {
            var converter = TypeNameToDisplayLabelConverter.Instance;

            foreach (var type in new[] { typeof(TranslateModifier), typeof(LFOModulator), typeof(Image), typeof(PlainModifier) })
                Assert.Equal(AddableTypes.LabelOf(type), converter.Convert(type, typeof(string), null, CultureInfo.InvariantCulture));
        }

        [Fact]
        public void Columns_FourItems_GivesOneColumnOfFour()
        {
            var columns = AddableTypes.Columns(Enumerable.Range(0, 4).ToList());

            Assert.Equal(new[] { new[] { 0, 1, 2, 3 } }, columns.Select(column => column.ToArray()));
        }

        [Fact]
        public void Columns_NineItems_GivesTwoColumnsFilledTopDown()
        {
            var columns = AddableTypes.Columns(Enumerable.Range(0, 9).ToList());

            Assert.Equal(new[] { new[] { 0, 1, 2, 3, 4 }, new[] { 5, 6, 7, 8 } }, columns.Select(column => column.ToArray()));
        }

        [Fact]
        public void Columns_TenItems_GivesThreeColumnsFilledTopDown()
        {
            var columns = AddableTypes.Columns(Enumerable.Range(0, 10).ToList());

            Assert.Equal(new[] { new[] { 0, 1, 2, 3 }, new[] { 4, 5, 6, 7 }, new[] { 8, 9 } }, columns.Select(column => column.ToArray()));
        }

        private sealed class PlainModifier
        {
        }
    }
}
