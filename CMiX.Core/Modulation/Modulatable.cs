// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using CMiX.Core.BaseControls;
using CMiX.Core.Modulation.Modulators;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Modulation
{
    // ModulatorID is what survives a save/load round trip; BoundModulator is a live, non-serialized
    // reference kept alongside it purely for runtime convenience (e.g. the UI reading which instance
    // is bound without a lookup). Both are set together by SetModulator. Null means unbound - the
    // channel's own Value is just its plain edited value, same as any ordinary property. Bound, that
    // same Value is reinterpreted (by the engine, not here) as the modulation depth - there is no
    // separate depth field, the same number just means something different depending on binding state.
    public partial class Modulatable : ObservableObject, IControl, IModulatorBindable
    {
        public Modulatable(GenericValue<float> value,
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

        // Which of BoundModulator's OutputNames this channel picked - meaningless while unbound.
        // Orthogonal to Value's own depth-reinterpretation (see the type comment above): this is
        // purely "which of the source's outputs", not "how much of it".
        public GenericValue<string> BoundOutputName { get; set; }

        [ObservableProperty]
        private IModulator boundModulator;

        // Bound to by the channel-assign popup - each listed row's ModulatorOutputSelection, or
        // null for the popup's "Unassign" entry, is passed straight through as CommandParameter.
        [RelayCommand]
        private void SetModulator(ModulatorOutputSelection selection)
        {
            BoundModulator = selection?.Modulator;
            ModulatorID.Value = selection?.Modulator?.ID;
            BoundOutputName.Value = selection?.OutputName;
        }

        // The source-generated SetModulatorCommand is IRelayCommand<ModulatorOutputSelection> -
        // not, by itself, a match for IModulatorBindable's plain ICommand (interface implementation
        // requires an exact return type, not just an assignable one), so this bridges the two.
        ICommand IModulatorBindable.SetModulatorCommand => SetModulatorCommand;

        // Same bridging reason as SetModulatorCommand above: the public members are the
        // GenericValue<T> wrappers themselves (needed for ToModel/FromModel), not the plain values
        // IModulatorBindable exposes.
        Guid? IModulatorBindable.ModulatorID => ModulatorID.Value;
        string IModulatorBindable.BoundOutputName => BoundOutputName.Value;

        public IControlModel ToModel() => new ModulatableModel
        {
            ID = ID,
            Label = Label,
            Value = (GenericValueModel<float>)Value.ToModel(),
            ModulatorID = (GenericValueModel<Guid?>)ModulatorID.ToModel(),
            BoundOutputName = (GenericValueModel<string>)BoundOutputName.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (ModulatableModel)model;
            ID = m.ID;
            Label = m.Label;
            Value.FromModel(m.Value);
            ModulatorID.FromModel(m.ModulatorID);
            BoundOutputName.FromModel(m.BoundOutputName);
        }
    }
}
