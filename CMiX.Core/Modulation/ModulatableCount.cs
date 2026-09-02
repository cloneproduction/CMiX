// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using CMiX.Core.BaseControls;
using CMiX.Core.Modulation.Modulators;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Modulation
{
    // Int-valued counterpart to Modulatable, for a Count that should be bindable the same way a
    // float channel already is. Unlike Modulatable's Value, this is never blended/reinterpreted -
    // unbound means "type a number" and bound means "follow the modulator", a straight override,
    // not a depth. ModulatorID is what survives a save/load round trip; BoundModulator is a live,
    // non-serialized reference kept alongside it purely for runtime convenience. Both are set
    // together by SetModulator.
    public partial class ModulatableCount : ObservableObject, IControl, IModulatorBindable
    {
        public ModulatableCount(GenericValue<int> value,
                                GenericValue<Guid?> modulatorID,
                                GenericValue<string> boundOutputName)
        {
            Value = value;
            ModulatorID = modulatorID;
            BoundOutputName = boundOutputName;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public string Label { get; set; } = string.Empty;
        public GenericValue<int> Value { get; set; }
        public GenericValue<Guid?> ModulatorID { get; set; }

        // Which of BoundModulator's Outputs this channel picked - meaningless while unbound.
        public GenericValue<string> BoundOutputName { get; set; }

        [ObservableProperty]
        private IModulator boundModulator;

        // Always backed by GenericValue<int> - see the constructor.
        ModulatorValueType IModulatorBindable.RequiredValueType => ModulatorValueType.Integer;

        // Bound to by the channel-assign popup - each listed row's ModulatorOutputSelection, or
        // null for the popup's "Unassign" entry, is passed straight through as CommandParameter.
        [RelayCommand]
        private void SetModulator(ModulatorOutputSelection selection)
        {
            BoundModulator = selection?.Modulator;
            ModulatorID.Value = selection?.Modulator?.ID;
            BoundOutputName.Value = selection?.Output?.Name;
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

        public IControlModel ToModel() => new ModulatableCountModel
        {
            ID = ID,
            Label = Label,
            Value = (GenericValueModel<int>)Value.ToModel(),
            ModulatorID = (GenericValueModel<Guid?>)ModulatorID.ToModel(),
            BoundOutputName = (GenericValueModel<string>)BoundOutputName.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (ModulatableCountModel)model;
            ID = m.ID;
            Label = m.Label;
            Value.FromModel(m.Value);
            ModulatorID.FromModel(m.ModulatorID);
            BoundOutputName.FromModel(m.BoundOutputName);
        }
    }
}
