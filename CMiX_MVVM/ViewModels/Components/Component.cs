﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CMiX.MVVM.Interfaces;
using CMiX.MVVM.Services;

namespace CMiX.MVVM.ViewModels
{
    public abstract class Component : Sendable, IHandler
    {
        public Component(int id)
        {
            ID = id;
            Name = $"{GetType().Name}{ID}";

            IsExpanded = false;
            Components = new ObservableCollection<Component>();
        }


        public void HandleMessage(Message.Message message, string parentMessageAddress)
        {
            string messageAddress = parentMessageAddress + this.GetMessageAddress();

            if (message.MessageAddress == messageAddress)
            {
                Console.WriteLine("MessageHandledBy : " + messageAddress);
                //this.SetViewModel(message.Payload as IComponentModel);
            }
            else if (message.MessageAddress.Contains(messageAddress))
            {
                foreach (var handler in GetHandlers())
                {
                    handler.HandleMessage(message, this.GetMessageAddress());
                }
            }
        }


        public virtual List<IHandler> GetHandlers()
        {
            return this.Components.ToList<IHandler>();
        }


        public override void OnParentReceiveChange(object sender, ModelEventArgs e)
        {
            if (e.ParentMessageAddress + this.GetMessageAddress() == e.MessageAddress)
                this.SetViewModel(e.Model as IComponentModel);
            else if (e.MessageAddress.Contains(e.ParentMessageAddress + this.GetMessageAddress()))
                OnReceiveChange(e.Model, e.MessageAddress, e.ParentMessageAddress + this.GetMessageAddress());
            //else
            //    OnReceiveChange(e.Model, e.MessageAddress, e.ParentMessageAddress + this.GetMessageAddress());
        }

        public override string GetMessageAddress()
        {
            return $"{this.GetType().Name}/{ID}/";
        }


        private int _id;
        public int ID
        {
            get => _id;
            set => SetAndNotify(ref _id, value);
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetAndNotify(ref _name, value);
        }

        private bool _isRenaming;
        public bool IsRenaming
        {
            get => _isRenaming;
            set => SetAndNotify(ref _isRenaming, value);
        }

        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            set => SetAndNotify(ref _isEditing, value);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetAndNotify(ref _isSelected, value);
        }

        private bool _isVisible = false;
        public bool IsVisible
        {
            get
            {
                if (!ParentIsVisible)
                    return _isVisible;
                else
                    return true;
            }
            set
            {
                if (!ParentIsVisible)
                    SetAndNotify(ref _isVisible, value);
                SetVisibility();
            }
        }

        private bool _parentIsVisible = false;
        public bool ParentIsVisible
        {
            get => _parentIsVisible;
            set
            {
                SetAndNotify(ref _parentIsVisible, value);
                Notify(nameof(IsVisible));
            }
        }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetAndNotify(ref _isExpanded, value);
        }

        private ObservableCollection<Component> _components;
        public ObservableCollection<Component> Components
        {
            get => _components;
            set => SetAndNotify(ref _components, value);
        }

        private Component _selectedComponent;
        public Component SelectedComponent
        {
            get => _selectedComponent;
            set => SetAndNotify(ref _selectedComponent, value);
        }

        public void AddComponent(Component component)
        {
            Components.Add(component);
            component.SubscribeToEvent(this);
            IsExpanded = true;
            OnSendChange(this.GetModel(), this.GetMessageAddress());
        }

        public void RemoveComponent(Component component)
        {
            Components.Remove(component);
            component.UnSubscribeToEvent(this);
            OnSendChange(this.GetModel(), this.GetMessageAddress());
        }

        public void InsertComponent(int index, Component component)
        {
            Components.Insert(index, component);
            component.SubscribeToEvent(this);
            OnSendChange(this.GetModel(), this.GetMessageAddress());
        }

        public void MoveComponent(int oldIndex, int newIndex)
        {
            Components.Move(oldIndex, newIndex);
            OnSendChange(this.GetModel(), this.GetMessageAddress());
        }

        private void SetVisibility()
        {
            foreach (var item in Components)
            {
                item.ParentIsVisible = IsVisible;
                item.SetVisibility();
            } 
        }
    }
}