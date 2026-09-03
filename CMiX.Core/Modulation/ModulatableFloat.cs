// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using CMiX.Core.BaseControls;
using CMiX.Core.Modulation.Modulators;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Modulation
{
    public partial class ModulatableFloat : ObservableObject, IControl, IModulatorBindable
    {
        public ModulatableFloat(GenericValue<float> value,
                           GenericValue<Guid?> modulatorID,
                           GenericValue<string> boundOutputName)
        {
            Value = value;
            ModulatorID = modulatorID;
            BoundOutputName = boundOutputName;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public string Label { get; set; } = string.Empty;
        public GenericValue<float> Value { get; set; }
        public GenericValue<Guid?> ModulatorID { get; set; }

        public GenericValue<string> BoundOutputName { get; set; }

        [ObservableProperty]
        private IModulator boundModulator;

        Type IModulatorBindable.RequiredValueType => typeof(float);

        [RelayCommand]
        private void SetModulator(ModulatorOutputSelection selection)
        {
            BoundModulator = selection?.Modulator;
            ModulatorID.Value = selection?.Modulator?.ID;
            BoundOutputName.Value = selection?.Output?.Name;
        }

        ICommand IModulatorBindable.SetModulatorCommand => SetModulatorCommand;

        Guid? IModulatorBindable.ModulatorID => ModulatorID.Value;
        string IModulatorBindable.BoundOutputName => BoundOutputName.Value;

        public IControlModel ToModel() => new ModulatableFloatModel
        {
            ID = ID,
            Label = Label,
            Value = (GenericValueModel<float>)Value.ToModel(),
            ModulatorID = (GenericValueModel<Guid?>)ModulatorID.ToModel(),
            BoundOutputName = (GenericValueModel<string>)BoundOutputName.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (ModulatableFloatModel)model;
            ID = m.ID;
            Label = m.Label;
            Value.FromModel(m.Value);
            ModulatorID.FromModel(m.ModulatorID);
            BoundOutputName.FromModel(m.BoundOutputName);
        }
    }
}
