using System.Text.Json;
using System.Text.Json.Nodes;
using CMiX.Core.Animations;
using CMiX.Core.Compositing;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Persistence;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // A copy gets new ids for the controls inside it. An id that points to something outside the copy stays.
    public class ControlClonerTests
    {
        private static IEnumerable<string> IdsOfControls(IControlModel model)
        {
            var found = new List<string>();
            Collect(JsonNode.Parse(JsonSerializer.Serialize(model, model.GetType(), ProjectSerializer.Options)), found);
            return found;
        }

        private static void Collect(JsonNode? node, List<string> found)
        {
            switch (node)
            {
                case JsonObject obj:
                    foreach (var pair in obj)
                    {
                        if (pair.Key == "ID" && pair.Value is JsonValue value && value.TryGetValue<string>(out var id))
                            found.Add(id);
                        else
                            Collect(pair.Value, found);
                    }
                    break;
                case JsonArray array:
                    foreach (var item in array) Collect(item, found);
                    break;
            }
        }

        private static (IServiceProvider provider, PrefabManager compositions, Composition composition) NewComposition()
        {
            var provider = TestServiceProviderFactory.Create();
            var compositions = provider.GetRequiredService<PrefabManager>();
            compositions.AddItem(typeof(Composition));
            return (provider, compositions, (Composition)compositions.SelectedItem);
        }

        [Fact]
        public void ADuplicatedComposition_HasNoControlIdOfTheOriginal()
        {
            var (_, compositions, composition) = NewComposition();
            composition.LayerManager.AddItem(typeof(Layer));
            composition.ModifierManager.AddItem(typeof(AlphaModifier));

            var copy = compositions.DuplicateItem(composition);

            Assert.Empty(IdsOfControls(composition.ToModel()).Intersect(IdsOfControls(copy.ToModel())));
        }

        [Fact]
        public void ADuplicatedComposition_KeepsTheOutputMapping_ButHasItsOwnOutputControl()
        {
            var (_, compositions, composition) = NewComposition();

            var copy = (Composition)compositions.DuplicateItem(composition);

            Assert.Equal(composition.SelectedOutputMappingID.Value, copy.SelectedOutputMappingID.Value);
            Assert.NotEqual(composition.SelectedOutputMappingID.ID, copy.SelectedOutputMappingID.ID);
        }

        [Fact]
        public void ChangingTheOutputOfTheCopy_DoesNotReachTheOriginal()
        {
            var (_, compositions, composition) = NewComposition();
            var copy = (Composition)compositions.DuplicateItem(composition);
            var message = new Networking.Messages.MessageValueChanged { ID = copy.SelectedOutputMappingID.ID, Value = new BaseControls.GenericValueModel<Guid>(Guid.NewGuid()) };
            var before = composition.SelectedOutputMappingID.Value;

            composition.SelectedOutputMappingID.Receive(message);

            Assert.Equal(before, composition.SelectedOutputMappingID.Value);
        }

        [Fact]
        public void TwoClonesOfTheSameModel_HaveDifferentIds()
        {
            var (_, _, composition) = NewComposition();
            var model = composition.ToModel();

            var first = ControlCloner.CloneWithNewGuids(model);
            var second = ControlCloner.CloneWithNewGuids(model);

            Assert.Empty(IdsOfControls(first).Intersect(IdsOfControls(second)));
        }

        [Fact]
        public void Clone_ARedisAddressModulator_KeepsTheSelectionAndTheGain()
        {
            var (provider, compositions, composition) = NewComposition();
            var registry = provider.GetRequiredService<RedisAddressRegistry>();
            registry.AddressManager.AddItem(typeof(RedisAddress));
            var address = (RedisAddress)registry.AddressManager.SelectedItem;
            address.Value = "cmix:a";
            composition.ModifierManager.AddItem(typeof(AlphaModifier));
            var modifier = (Modifier)composition.ModifierManager.SelectedItem;
            modifier.ModulatorManager.AddItem(typeof(RedisAddressModulator));
            var modulator = (RedisAddressModulator)modifier.ModulatorManager.SelectedItem;
            modulator.SelectedAddress = address;
            modulator.Gain.Value = 0.5f;

            var copy = (Composition)compositions.DuplicateItem(composition);
            var copyModulator = (RedisAddressModulator)((Modifier)copy.ModifierManager.ManagerData.Items.First()).ModulatorManager.ManagerData.Items.First();

            Assert.Same(address, copyModulator.SelectedAddress);
            Assert.Equal(0.5f, copyModulator.Gain.Value);
            Assert.NotEqual(modulator.ID, copyModulator.ID);
            Assert.NotEqual(modulator.Gain.ID, copyModulator.Gain.ID);
        }
    }
}
