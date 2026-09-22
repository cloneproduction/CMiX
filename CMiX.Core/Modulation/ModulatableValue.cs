// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using CMiX.Core.BaseControls;
using CMiX.Core.Modulation.Modulators;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Modulation
{
    public partial class ModulatableValue<T> : ObservableObject, IControl, IModulatorBindable
    {
        public ModulatableValue(GenericValue<T> value,
                                GenericValue<Guid?> modulatorID,
                                GenericValue<string> boundOutputName)
        {
            ValueSource = value;
            ModulatorIDSource = modulatorID;
            BoundOutputNameSource = boundOutputName;

            ValueSource.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(GenericValue<T>.Value)) OnPropertyChanged(nameof(Value)); };
            ModulatorIDSource.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName != nameof(GenericValue<Guid?>.Value)) return;
                OnPropertyChanged(nameof(ModulatorID));

                // BoundModulator is computed from ModulatorID through ModulatorResolver, so it
                // can never go stale on its own. Notify here so a bound view refreshes on every
                // path that changes ModulatorID: SetModulator, undo, and redo alike.
                OnPropertyChanged(nameof(BoundModulator));
            };
            BoundOutputNameSource.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(GenericValue<string>.Value)) OnPropertyChanged(nameof(BoundOutputName)); };
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public string Label { get; set; } = string.Empty;
        public GenericValue<T> ValueSource { get; set; }
        public GenericValue<Guid?> ModulatorIDSource { get; set; }

        public GenericValue<string> BoundOutputNameSource { get; set; }

        public T Value
        {
            get => ValueSource.Value;
            set => ValueSource.Value = value;
        }

        public Guid? ModulatorID
        {
            get => ModulatorIDSource.Value;
            set => ModulatorIDSource.Value = value;
        }

        public string BoundOutputName
        {
            get => BoundOutputNameSource.Value;
            set => BoundOutputNameSource.Value = value;
        }

        public IModulator BoundModulator =>
            ModulatorID is { } id ? ModulatorResolver?.Invoke(id) : null;

        public Func<Guid, IModulator> ModulatorResolver { get; set; }

        bool IModulatorBindable.CanBind(IModulatorOutput output) => output is ModulatorOutput<T>;

        [RelayCommand]
        private void SetModulator(ModulatorOutputSelection selection)
        {
            var undoManager = ModulatorIDSource.UndoManager;
            undoManager?.BeginCapture();
            try
            {
                ModulatorID = selection?.Modulator?.ID;
                BoundOutputName = selection?.Output?.Name;
            }
            finally
            {
                undoManager?.EndCapture();
            }
        }

        ICommand IModulatorBindable.SetModulatorCommand => SetModulatorCommand;

        public IControlModel ToModel() => new ModulatableValueModel<T>
        {
            ID = ID,
            Label = Label,
            Value = (GenericValueModel<T>)ValueSource.ToModel(),
            ModulatorID = (GenericValueModel<Guid?>)ModulatorIDSource.ToModel(),
            BoundOutputName = (GenericValueModel<string>)BoundOutputNameSource.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (ModulatableValueModel<T>)model;
            ID = m.ID;
            Label = m.Label;
            ValueSource.FromModel(m.Value);
            ModulatorIDSource.FromModel(m.ModulatorID);
            BoundOutputNameSource.FromModel(m.BoundOutputName);
        }
    }
}
