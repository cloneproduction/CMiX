// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Modifiers;
using CMiX.Core.Presentation.ViewModels.Network;
using CMiX.Core.Presentation.ViewModels.Service;
using CMiX.Core.Presentation.ViewModels.Services;
using CMiX.Core.Presentation.Views.Dialogs;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MvvmDialogs;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public class LayerScene : Component, IBeatable, ILayer, IRecipient<MessageKeyPressed>
    {
        public LayerScene(LayerSceneModel layerModel, CompositionService compositionService)
        {
            ID = layerModel.ID;
            CompositionService = compositionService;

            BackgroundColor = new ColorSelector(layerModel.ColorSelectorModel);

            Visibility = new ToggleButton(layerModel.VisibilityModel);

            ModifierManager = new ModifierManager(layerModel.ModifierManager, new TextureFilterFactory());
            Camera = new Camera(layerModel.Camera);
            AmbientOcclusion = new AmbientOcclusion(layerModel.AmbientOcclusion);
            OpenColorSelectorCommand = new RelayCommand(OpenColorSelector);

            SelectedTabIndex = 0;
        }


        public ICommand OpenColorSelectorCommand { get; set; }
        public ICommand ChangeEntityPanelCommand { get; set; }


        public void OpenColorSelector()
        {
            IDialogService dialogService = WeakReferenceMessenger.Default.Send(new MessageRequestDialogService(), MessageType.Internal).Response;
            dialogService.Show<ColorSelectorWindow>(this, this.BackgroundColor);
        }


        public CompositionService CompositionService { get; set; }

        public ToggleButton Visibility { get; set; }
        public ColorSelector BackgroundColor { get; set; }
        public ModifierManager ModifierManager { get; set; }


        public CameraManager CameraManager { get; set; }
        public Camera Camera { get; set; }
        public AmbientOcclusion AmbientOcclusion { get; set; }


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


        private int _selectedTabIndex;
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set => SetProperty(ref _selectedTabIndex, value);
        }


        public void SetMasterBeat(MasterBeat masterBeat)
        {
            this.MasterBeat = masterBeat;
        }


        public override IComponentModel GetModel()
        {
            LayerSceneModel model = new LayerSceneModel();

            model.ID = this.ID;
            model.Name = this.Name;

            model.ModifierManager = (ModifierManagerModel)this.ModifierManager.GetModel();
            model.ColorSelectorModel = (ColorSelectorModel)this.BackgroundColor.GetModel();
            model.Camera = (CameraModel)this.Camera.GetModel();
            model.VisibilityModel = (ToggleButtonModel)this.Visibility.GetModel();
            model.AmbientOcclusion = (AmbientOcclusionModel)this.AmbientOcclusion.GetModel();

            foreach (Component item in this.Components)
                model.ComponentModels.Add(item.GetModel());

            return model;
        }

        public override void SetViewModel(IComponentModel model)
        {
            LayerSceneModel layerModel = model as LayerSceneModel;

            this.ID = layerModel.ID;
            this.Name = layerModel.Name;

            this.ModifierManager.SetViewModel(layerModel.ModifierManager);
            this.BackgroundColor.SetViewModel(layerModel.ColorSelectorModel);
            this.Camera.SetViewModel(layerModel.Camera);
            this.Visibility.SetViewModel(layerModel.VisibilityModel);
            this.AmbientOcclusion.SetViewModel(layerModel.AmbientOcclusion);

            this.Components.Clear();
        }

        public void Receive(MessageKeyPressed message)
        {
            if (!IsSelected)
                return;

            switch (message.Key)
            {
                case Key.D1:
                    SelectedTabIndex = 0;
                    break;

                case Key.D2:
                    SelectedTabIndex = 1;
                    break;

                case Key.D3:
                    SelectedTabIndex = 2;
                    break;

                case Key.D4:
                    SelectedTabIndex = 3;
                    break;

                case Key.D5:
                    SelectedTabIndex = 4;
                    break;
            }
        }
    }
}
