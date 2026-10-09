// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using CMiX.Core.Modulation;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    // One row of the modulator assign menu.
    public sealed class ModulatorOutputChoice
    {
        private const string ModulatorSuffix = " Modulator";

        private ModulatorOutputChoice(string label, bool isAssigned, ICommand? command, ModulatorOutputSelection? selection)
        {
            Label = label;
            IsAssigned = isAssigned;
            Command = command;
            Selection = selection;
        }

        public string Label { get; }
        public bool IsAssigned { get; }
        public ICommand? Command { get; }
        public ModulatorOutputSelection? Selection { get; }

        public static ModulatorOutputChoice For(ModulatorOutputSelection selection, IModulatorBindable? bindable)
        {
            var isAssigned = bindable != null
                && Equals(selection.Modulator, bindable.BoundModulator)
                && selection.Output?.Name == bindable.BoundOutputName;
            var command = bindable == null ? null : new RelayCommand(() => bindable.SetModulatorCommand.Execute(selection));
            return new ModulatorOutputChoice(LabelOf(selection), isAssigned, command, selection);
        }

        public static ModulatorOutputChoice Unassign(IModulatorBindable? bindable) =>
            new("Unassign", false,
                new RelayCommand(() => bindable?.SetModulatorCommand.Execute(null), () => bindable?.BoundModulator != null),
                null);

        // A row that cannot execute shows as a muted title.
        public static ModulatorOutputChoice Title(string text) =>
            new(text, false, new RelayCommand(() => { }, () => false), null);

        // A MenuItem with the header "-" shows as a separator.
        public static ModulatorOutputChoice Separator() => new("-", false, null, null);

        public static IReadOnlyList<ModulatorOutputChoice> Build(IEnumerable<ModulatorOutputSelection> selections, IModulatorBindable? bindable)
        {
            var choices = new List<ModulatorOutputChoice> { Title("Assign Modulator"), Separator() };
            choices.AddRange(selections.Select(selection => For(selection, bindable)));
            choices.Add(Unassign(bindable));
            return choices;
        }

        // "Random Modulator" with the output "Value" gives "Random Value".
        private static string LabelOf(ModulatorOutputSelection selection)
        {
            var modulatorName = selection.Modulator?.PrefabService?.Name?.Value;
            if (modulatorName != null && modulatorName.EndsWith(ModulatorSuffix, StringComparison.Ordinal))
                modulatorName = modulatorName[..^ModulatorSuffix.Length];

            return $"{modulatorName} {selection.Output?.Name}";
        }
    }
}
