// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.ObjectModel;

using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Network;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels
{
    public class TransformModifier : ObservableRecipient, IRecipient<IMessage>, IControl
    {
        public TransformModifier(MasterBeat beat, TransformModifierModel transformModifierModel)
        {
            this.ID = transformModifierModel.ID;
            Modifiers = new ObservableCollection<ITransformModifier>();
            Factory = new TransformModifierFactory(beat);
            WeakReferenceMessenger.Default.Register(this, MessageType.In);

            CreateTransformModifierCommand = new RelayCommand<TransformModifierNames>(CreateTransformModifier);
            RemoveTransformModifierCommand = new RelayCommand<ITransformModifier>(RemoveTransformModifier);
        }

        public Guid ID { get; set; }
        public ICommand CreateTransformModifierCommand { get; set; }
        public ICommand AddTransformModifierCommand { get; set; }
        public ICommand RemoveTransformModifierCommand { get; set; }
        public TransformModifierFactory Factory { get; set; }


        private ObservableCollection<ITransformModifier> _modifiers;
        public ObservableCollection<ITransformModifier> Modifiers
        {
            get => _modifiers;
            set => SetProperty(ref _modifiers, value);
        }



        public void CreateTransformModifier(TransformModifierNames transformModifierNames)
        {
            ITransformModifier transformModifier = Factory.CreateTransformModifier(transformModifierNames);
            AddTransformModifier(transformModifier);
        }

        public void CreateTransformModifier(ITransformModifierModel transformModifierModel)
        {
            ITransformModifier transformModifier = Factory.CreateTransformModifier(transformModifierModel);
            AddTransformModifier(transformModifier);
        }


        public void AddTransformModifier(ITransformModifier transformModifier)
        {
            Modifiers.Add(transformModifier);
            WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageAddTransformModifier(this.ID, transformModifier), MessageType.Out);
        }

        public void RemoveTransformModifier(ITransformModifier transformModifier)
        {
            this.Modifiers.Remove(transformModifier);
        }


        public void SetViewModel(IModel model)
        {
            TransformModifierModel transformModifierModel = model as TransformModifierModel;
            this.ID = transformModifierModel.ID;
        }

        public IModel GetModel()
        {
            TransformModifierModel model = new TransformModifierModel();
            model.ID = this.ID;
            return model;
        }

        public void Receive(IMessage message)
        {
            if (message is MessageAddTransformModifier msg && message.ID == this.ID)
            {
                this.CreateTransformModifier(msg.TransformModifierModel);
                return;
            }
        }
    }
}
