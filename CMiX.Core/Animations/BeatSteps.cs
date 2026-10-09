// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.ObjectModel;
using CMiX.Core.BaseControls;
using CMiX.Core.Networking;
using CMiX.Core.Prefabs.Messages;
using CMiX.Core.Undo;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Animations
{
    public class BeatSteps : ObservableObject, IControl
    {
        public BeatSteps(ControlMessenger controlMessenger,
                         MessageFactory messageFactory,
                         ControlActivationService activationService,
                         UndoManager undoManager,
                         int stepCount = BeatStepsModel.DefaultStepCount)
        {
            for (int i = 0; i < stepCount; i++)
                Steps.Add(new GenericValue<bool>(controlMessenger, messageFactory, activationService, undoManager));
        }
        public Guid ID { get; set; } = Guid.NewGuid();

        public ObservableCollection<GenericValue<bool>> Steps { get; } = new();

        private int _currentStepIndex;
        public int CurrentStepIndex
        {
            get => _currentStepIndex;
            set
            {
                _currentStepIndex = Steps.Count == 0 ? 0 : value % Steps.Count;
                OnPropertyChanged();
            }
        }

        public IControlModel ToModel() => new BeatStepsModel
        {
            Steps = Steps.Select(s => (GenericValueModel<bool>)s.ToModel()).ToList()
        };

        public void FromModel(IControlModel model)
        {
            var m = (BeatStepsModel)model;
            for (int i = 0; i < Steps.Count && i < m.Steps.Count; i++)
                Steps[i].FromModel(m.Steps[i]);
        }
    }
}
