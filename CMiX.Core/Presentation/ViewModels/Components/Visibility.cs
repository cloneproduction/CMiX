// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Models.Component;
using CMiX.Core.Network.Communicators;
using CMiX.Core.Network.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public class Visibility : ObservableObject, IControl
    {
        public Visibility(VisibilityModel visibilityModel)
        {
            IsVisible = true;
            ParentIsVisible = true;
            SetVisibilityCommand = new RelayCommand<Component>(SetVisibility);
        }

        public Visibility(Visibility parentVisibility, VisibilityModel visibilityModel)
        {
            IsVisible = true;
            ParentIsVisible = parentVisibility.ParentIsVisible && parentVisibility.IsVisible;
            SetVisibilityCommand = new RelayCommand<Component>(SetVisibility);
        }


        public Guid ID { get; set; }
        public Communicator Communicator { get; set; }
        public ICommand SetVisibilityCommand { get; set; }

        private bool _isVisible;
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                SetProperty(ref _isVisible, value);
                //Communicator?.SendMessage(new MessageUpdateViewModel(this));
                Console.WriteLine("Visibility Is " + IsVisible);
            }
        }

        private bool _parentIsVisible;
        public bool ParentIsVisible
        {
            get => _parentIsVisible;
            set => SetProperty(ref _parentIsVisible, value);
        }


        public void SetVisibility(Component component)
        {
            if (!IsVisible)
                IsVisible = true;
            else
                IsVisible = false;

            if (ParentIsVisible)
                this.SetChildVisibility(component, this.IsVisible);
        }

        public void SetChildVisibility(Component component, bool parentVisibility)
        {
            foreach (var childComponent in component.Components)
            {
                if (childComponent.Visibility.IsVisible)
                {
                    childComponent.Visibility.SetChildVisibility(childComponent, parentVisibility);
                }
                childComponent.Visibility.ParentIsVisible = parentVisibility;
            }
        }


        public void SetCommunicator(Communicator communicator)
        {
            Communicator = new Communicator(this);
            Communicator.SetCommunicator(communicator);
        }

        public void UnsetCommunicator(Communicator communicator)
        {
            Communicator.UnsetCommunicator(communicator);
        }


        public IModel GetModel()
        {
            VisibilityModel visibilityModel = new VisibilityModel();
            visibilityModel.ID = this.ID;
            visibilityModel.IsVisible = this.IsVisible;
            return visibilityModel;
        }

        public void SetViewModel(IModel model)
        {
            var visibilityModel = model as VisibilityModel;
            this.ID = visibilityModel.ID;
            this.IsVisible = visibilityModel.IsVisible;
        }
    }
}
