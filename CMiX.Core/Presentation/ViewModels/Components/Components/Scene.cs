// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Network;
using CMiX.Core.Presentation.Views.Dialogs;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MvvmDialogs;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public class Scene : Component, IBeatable, IDisposable
    {
        public Scene(SceneModel sceneModel)
        {
            ID = sceneModel.ID;
            // Visibility = new Visibility(layer.Visibility, sceneModel.VisibilityModel);
            BeatModifier = new BeatModifier(sceneModel.BeatModifierModel);
            PostFX = new PostFX(sceneModel.PostFXModel);
            Mask = new Mask(sceneModel.MaskModel);
            Transform = new Transform(sceneModel.TransformModel);
            ColorSelector = new ColorSelector(sceneModel.BackgroundColorSelectorModel);
            Camera = new Camera(sceneModel.CameraModel);

            OpenColorSelectorCommand = new RelayCommand(OpenColorSelector);
        }

        public ICommand OpenColorSelectorCommand { get; set; }
        public ColorSelector ColorSelector { get; set; }
        public Transform Transform { get; set; }
        public Mask Mask { get; set; }
        public PostFX PostFX { get; set; }
        public BeatModifier BeatModifier { get; set; }
        public Camera Camera { get; set; }

        public void OpenColorSelector()
        {
            IDialogService dialogService = WeakReferenceMessenger.Default.Send(new MessageRequestDialogService(), MessageType.Internal).Response;
            dialogService.Show<ColorSelectorWindow>(this, ColorSelector);
        }

        public override void AddComponent(IComponent component)
        {
            base.AddComponent(component);
            component.MasterBeat = this.MasterBeat;
        }

        public void SetMasterBeat(MasterBeat masterBeat)
        {
            this.MasterBeat = masterBeat;
            BeatModifier.MasterBeat = masterBeat;
        }

        public override IComponentModel GetModel()
        {
            SceneModel model = new SceneModel(this.ID);

            model.ID = this.ID;
            model.Name = this.Name;

            model.BeatModifierModel = (BeatModifierModel)this.BeatModifier.GetModel();
            model.PostFXModel = (PostFXModel)this.PostFX.GetModel();
            model.MaskModel = (MaskModel)this.Mask.GetModel();
            model.TransformModel = (TransformModel)this.Transform.GetModel();
            model.BackgroundColorSelectorModel = (ColorSelectorModel)this.ColorSelector.GetModel();
            model.CameraModel = (CameraModel)this.Camera.GetModel();

            foreach (Component item in this.Components)
                model.ComponentModels.Add(item.GetModel());

            return model;
        }

        public override void SetViewModel(IComponentModel model)
        {
            SceneModel sceneModel = model as SceneModel;

            this.ID = sceneModel.ID;
            this.BeatModifier.SetViewModel(sceneModel.BeatModifierModel);
            this.PostFX.SetViewModel(sceneModel.PostFXModel);
            this.Mask.SetViewModel(sceneModel.MaskModel);
            this.Transform.SetViewModel(sceneModel.TransformModel);
            this.ColorSelector.SetViewModel(sceneModel.BackgroundColorSelectorModel);
            this.Camera.SetViewModel(sceneModel.CameraModel);

            this.Components.Clear();
            foreach (var componentModel in sceneModel.ComponentModels)
            {
                //var newComponent = this.ComponentFactory.CreateComponent(componentModel);
                //this.AddComponent(newComponent);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            BeatModifier.Dispose();
        }
    }
}
