// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Windows.Input;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Undo;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Prefabs.Managers
{
    public abstract partial class PrefabManagerBase : ReceivableControl, IControl, IRecipient<IMessage>
    {
        protected PrefabManagerBase(ControlRepository controlRepository,
                                     ControlRegistry registry,
                                     UndoManager undoManager)
        {
            ControlRepository = controlRepository;
            UndoManager = undoManager;
            IsActive = false;
            registry.Register(this);
        }

        public ControlRepository ControlRepository { get; }
        public UndoManager UndoManager { get; }

        public abstract ICommand AddItemCommand { get; }
        public abstract ICommand ClearSelectionCommand { get; }
        public abstract IControl SelectedItem { get; set; }
        public Guid ID { get; set; }

        public abstract void Receive(IMessage message);
        public abstract IControlModel ToModel();
        public abstract void FromModel(IControlModel model);
    }
}

