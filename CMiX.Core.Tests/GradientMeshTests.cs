using System.Text.Json;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Persistence;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Sources;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class GradientMeshTests
    {
        private static GradientMesh Create() =>
            (GradientMesh)TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>().Create(typeof(GradientMesh));

        [Fact]
        public void ModulatorManager_RemoveItem_UnbindsTheTimeValue()
        {
            var mesh = Create();
            mesh.ModulatorManager.AddItem(typeof(LFOModulator));
            var modulator = (LFOModulator)mesh.ModulatorManager.ManagerData.Items[0];
            mesh.Time.SetModulatorCommand.Execute(new ModulatorOutputSelection(modulator, modulator.Outputs[0]));
            Assert.Equal(modulator.ID, mesh.Time.ModulatorID);

            mesh.ModulatorManager.RemoveItem(modulator);

            Assert.Null(mesh.Time.ModulatorID);
        }

        [Fact]
        public void Serialize_ThenDeserialize_KeepsTheModel()
        {
            var mesh = Create();
            mesh.Color4.Value = "#FF654321";
            mesh.GradientType.Value = GradientMeshType.Spectral;
            var model = mesh.ToModel();

            var json = JsonSerializer.Serialize((IControlModel)model, ProjectSerializer.Options);
            var loaded = JsonSerializer.Deserialize<IControlModel>(json, ProjectSerializer.Options);

            var meshModel = Assert.IsType<GradientMeshModel>(loaded);
            Assert.Equal("#FF654321", meshModel.Color4.Value);
            Assert.Equal(GradientMeshType.Spectral, meshModel.GradientType.Value);
        }
    }
}
