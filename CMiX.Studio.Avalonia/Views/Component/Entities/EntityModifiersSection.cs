// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia;
using Avalonia.Controls;

namespace CMiX.Studio.Avalonia.Views
{
    // Shared "Modifiers" expander + AutoSelectionPanel wiring every entity type used to hand-roll.
    // An entity that needs an extra AutoSelectionPanel (e.g. TextEntity, which also shows the
    // generic Entity modifiers) can project it in through Content.
    public class EntityModifiersSection : ContentControl
    {
        public static readonly StyledProperty<Type> PanelOwnerProperty =
            AvaloniaProperty.Register<EntityModifiersSection, Type>(nameof(PanelOwner));

        public Type PanelOwner
        {
            get => GetValue(PanelOwnerProperty);
            set => SetValue(PanelOwnerProperty, value);
        }
    }
}
