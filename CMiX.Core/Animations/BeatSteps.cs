// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

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
                         int stepCount = 8)
        {
            for (int i = 0; i < stepCount; i++)
            {
                var step = new GenericValue<bool>(controlMessenger, messageFactory, activationService, undoManager);
                step.SetDefault(true);
                Steps.Add(step);
            }
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
