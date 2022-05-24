// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Modifiers;
using CMiX.Core.Presentation.ViewModels.Network;
using CMiX.Core.Presentation.Views.Dialogs;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CMiX.Core.Models.Component;
using MvvmDialogs;
using CMiX.Core.Presentation.ViewModels.Services;
using CMiX.Core.Presentation.ViewModels.Service;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public class LayerMask : Component, ILayer, IBeatable
    {
        public LayerMask(LayerMaskModel maskModel, CompositionService compositionService)
        {
            ID = maskModel.ID;
            Opacity = new Slider(nameof(Opacity), maskModel.Opacity);

            Entities = new ObservableCollection<IEntity>();
            BackgroundColor = new ColorSelector(maskModel.ColorSelectorModel);

            //MeshEntities = new ObservableCollection<MeshEntity>();
            //MeshEntityManager = new MeshEntityManager();

            //LightEntities = new ObservableCollection<LightEntity>();
            //LightEntityManager = new LightEntityManager();

            Visibility = new ToggleButton(maskModel.VisibilityModel);
            Invert = new ToggleButton(maskModel.Invert);

            ModifierManager = new ModifierManager(maskModel.ModifierManager, new TextureFilterFactory());
            Camera = new Camera(maskModel.Camera);
            AmbientOcclusion = new AmbientOcclusion(maskModel.AmbientOcclusion);
            OpenColorSelectorCommand = new RelayCommand(OpenColorSelector);

            EntityPanelIsSelected = true;
            MaskChannelSelector = new ComboBox<MaskChannel>(maskModel.MaskChannelSelector);
        }


        public ICommand OpenColorSelectorCommand { get; set; }
        public ICommand CreateEntityCommand { get; set; }
        public ICommand RemoveSelectedEntityCommand { get; set; }


        public void OpenColorSelector()
        {
            IDialogService dialogService = WeakReferenceMessenger.Default.Send(new MessageRequestDialogService(), MessageType.Internal).Response;
            dialogService.Show<ColorSelectorWindow>(this, this.BackgroundColor);
        }


        public MeshManager MeshEntityManager { get; set; }

        private ObservableCollection<Mesh> _meshEntities;
        public ObservableCollection<Mesh> MeshEntities
        {
            get => _meshEntities;
            set => SetProperty(ref _meshEntities, value);
        }


        public LightEntityManager LightEntityManager { get; set; }

        private ObservableCollection<LightEntity> _lightEntities;
        public ObservableCollection<LightEntity> LightEntities
        {
            get => _lightEntities;
            set => SetProperty(ref _lightEntities, value);
        }


        private bool _entityPanelIsSelected;
        public bool EntityPanelIsSelected
        {
            get => _entityPanelIsSelected;
            set => SetProperty(ref _entityPanelIsSelected, value);
        }

        private ObservableCollection<IEntity> _entities;
        public ObservableCollection<IEntity> Entities
        {
            get => _entities;
            set => SetProperty(ref _entities, value);
        }


        //public PrefabManager Services { get; set; }
        public ToggleButton Visibility { get; set; }
        public ColorSelector BackgroundColor { get; set; }
        public ModifierManager ModifierManager { get; set; }
        public Slider Opacity { get; set; }
        public CameraManager CameraManager { get; set; }
        public Camera Camera { get; set; }
        public AmbientOcclusion AmbientOcclusion { get; set; }
        public ComboBox<MaskChannel> MaskChannelSelector { get; set; }
        public ToggleButton Invert { get; set; }

        public void AddEntity(IEntity entity)
        {
            Entities.Add(entity);
            entity.SetMasterBeat(this.MasterBeat);
        }

        public void RemoveEntity(IEntity entity)
        {
            Entities.Remove(entity);
        }

        public void SetMasterBeat(MasterBeat masterBeat)
        {
            this.MasterBeat = masterBeat;
        }


        public override IComponentModel GetModel()
        {
            LayerMaskModel model = new LayerMaskModel();

            model.ID = this.ID;
            model.Name = this.Name;

            model.Opacity = (SliderModel)this.Opacity.GetModel();
            model.ModifierManager = (ModifierManagerModel)this.ModifierManager.GetModel();
            model.ColorSelectorModel = (ColorSelectorModel)this.BackgroundColor.GetModel();
            model.Camera = (CameraModel)this.Camera.GetModel();
            model.VisibilityModel = (ToggleButtonModel)this.Visibility.GetModel();
            model.AmbientOcclusion = (AmbientOcclusionModel)this.AmbientOcclusion.GetModel();
            model.Invert = (ToggleButtonModel)this.Invert.GetModel();

            model.MaskChannelSelector = (ComboBoxModel<MaskChannel>)this.MaskChannelSelector.GetModel();

            foreach (Component item in this.Components)
                model.ComponentModels.Add(item.GetModel());

            return model;
        }

        public override void SetViewModel(IComponentModel model)
        {
            LayerMaskModel layerModel = model as LayerMaskModel;

            this.ID = layerModel.ID;
            this.Name = layerModel.Name;

            this.Opacity.SetViewModel(layerModel.Opacity);
            this.ModifierManager.SetViewModel(layerModel.ModifierManager);
            this.BackgroundColor.SetViewModel(layerModel.ColorSelectorModel);
            this.Camera.SetViewModel(layerModel.Camera);
            this.Visibility.SetViewModel(layerModel.VisibilityModel);
            this.AmbientOcclusion.SetViewModel(layerModel.AmbientOcclusion);

            this.MaskChannelSelector.SetViewModel(layerModel.MaskChannelSelector);
            this.Invert.SetViewModel(layerModel.Invert);

            this.Components.Clear();

            //foreach (var componentModel in layerModel.ComponentModels)
            //{
            //    //var newComponent = this.ComponentFactory.CreateComponent(componentModel);
            //    //this.AddComponent(newComponent);
            //}
        }
    }
}
