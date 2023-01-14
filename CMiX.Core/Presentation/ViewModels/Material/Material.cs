// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Network;
using CMiX.Core.Presentation.ViewModels.Prefab;
using CMiX.Core.Presentation.ViewModels.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Material : ObservableRecipient, IRecipient<MessageRequestControl>, IPrefab, IDisposable
    {
        public Material(MaterialModel materialModel, CompositionService compositionService)
        {
            this.ID = materialModel.ID;
            Name = this.GetType().Name;
            CompositionService = compositionService;

            Texture = new Texture(materialModel.Texture, compositionService);

            Mask = new Mask(materialModel.Mask, compositionService);
            MaskChannelSelector = new ComboBox<MaskChannel>(materialModel.MaskChannelSelector);

            Pipeline = new ComboBox<PipelineType>(materialModel.Pipeline);
            CullMode = new ComboBox<CullModeType>(materialModel.CullMode);
            Transparency = new ComboBox<TransparencyType>(materialModel.Transparency);

            Metalness = new Slider(nameof(Metalness), materialModel.Metalness);
            Specularity = new Slider(nameof(Specularity), materialModel.Specularity);
            Glossiness = new Slider(nameof(Glossiness), materialModel.Glossiness);
            Alpha = new Slider(nameof(Alpha), materialModel.Alpha);
            IsShadowCaster = new ToggleButton(materialModel.IsShadowCaster);

            IsActive = true;

            Color = new ColorSelector(materialModel.ColorModel);
            OpenColorSelectorCommand = new RelayCommand(OpenColorSelector);
        }


        public Guid ID { get; set; }
        public CompositionService CompositionService { get; set; }

        private Texture _texture;
        public Texture Texture
        {
            get => _texture;
            set => SetProperty(ref _texture, value);
        }


        private Transform _transform;
        public Transform Transform
        {
            get => _transform;
            set
            {
                SetProperty(ref _transform, value);
                SendMessage(new MessageChangePrefab(this.ID, value, nameof(Transform)));
            }
        }


        bool CanSend = true;
        public void SendMessage(IMessage message)
        {
            if (CanSend)
                WeakReferenceMessenger.Default.Send<IMessage, int>(message, MessageType.Out);
        }

        public ColorSelector Color { get; set; }
        public ICommand OpenColorSelectorCommand { get; set; }
        public void OpenColorSelector()
        {
            //CompositionService.DialogService.Show<ColorSelectorWindow>(this, this.Color);
        }



        public Mask Mask { get; set; }
        public ComboBox<MaskChannel> MaskChannelSelector { get; set; }
        public ComboBox<PipelineType> Pipeline { get; set; }
        public ComboBox<TransparencyType> Transparency { get; set; }
        public ComboBox<CullModeType> CullMode { get; set; }

        public Slider Metalness { get; set; }
        public Slider Specularity { get; set; }
        public Slider Glossiness { get; set; }
        public Slider Alpha { get; set; }
        public ToggleButton IsShadowCaster { get; set; }


        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private bool _isRenaming;
        public bool IsRenaming
        {
            get => _isRenaming;
            set => SetProperty(ref _isRenaming, value);
        }

        private string _isExpanded;
        public string IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }


        public IModel GetModel()
        {
            MaterialModel model = new MaterialModel();
            model.ID = this.ID;

            model.Texture = (TextureModel)Texture.GetModel();
            model.Mask = (MaskModel)Mask.GetModel();
            model.MaskChannelSelector = (ComboBoxModel<MaskChannel>)MaskChannelSelector.GetModel();

            model.Pipeline = (ComboBoxModel<PipelineType>)Pipeline.GetModel();
            model.CullMode = (ComboBoxModel<CullModeType>)CullMode.GetModel();
            model.Transparency = (ComboBoxModel<TransparencyType>)Transparency.GetModel();

            model.Metalness = (SliderModel)Metalness.GetModel();
            model.Specularity = (SliderModel)Specularity.GetModel();
            model.Glossiness = (SliderModel)Glossiness.GetModel();
            model.Alpha = (SliderModel)Alpha.GetModel();

            model.IsShadowCaster = (ToggleButtonModel)IsShadowCaster.GetModel();

            model.ColorModel = (ColorSelectorModel)Color.GetModel();
            
            return model;
        }

        public void SetViewModel(IModel model)
        {
            MaterialModel materialModel = model as MaterialModel;
            this.ID = materialModel.ID;

            this.Texture.SetViewModel(materialModel.Texture);
            this.Mask.SetViewModel(materialModel.Mask);
            this.MaskChannelSelector.SetViewModel(materialModel.MaskChannelSelector);

            this.Pipeline.SetViewModel(materialModel.Pipeline);
            this.CullMode.SetViewModel(materialModel.CullMode);
            this.Transparency.SetViewModel(materialModel.Transparency);

            this.Metalness.SetViewModel(materialModel.Metalness);
            this.Specularity.SetViewModel(materialModel.Specularity);
            this.Glossiness.SetViewModel(materialModel.Glossiness);
            this.Alpha.SetViewModel(materialModel.Alpha);

            this.IsShadowCaster.SetViewModel(materialModel.IsShadowCaster);

            this.Color.SetViewModel(materialModel.ColorModel);
        }

        public void Dispose()
        {

        }

        public void Receive(MessageRequestControl message)
        {
            if (message.ID == this.ID && !message.HasReceivedResponse)
                message.Reply(this);
        }
    }
}
