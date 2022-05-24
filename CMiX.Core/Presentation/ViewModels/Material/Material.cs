// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Prefab;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Material : ObservableObject, IPrefab, IControl, IBeatable, IDisposable
    {
        public Material(MaterialModel materialModel)
        {
            this.ID = materialModel.ID;
            Name = this.GetType().Name;

            BeatModifier = new BeatModifier(materialModel.BeatModifierModel);

            Texture = new Texture(materialModel.Texture);
            Mask = new Mask(materialModel.Mask);

            MaskChannelSelector = new ComboBox<MaskChannel>(materialModel.MaskChannelSelector);

            Pipeline = new ComboBox<PipelineType>(materialModel.Pipeline);
            CullMode = new ComboBox<CullModeType>(materialModel.CullMode);
            Transparency = new ComboBox<TransparencyType>(materialModel.Transparency);

            Metalness = new Slider(nameof(Metalness), materialModel.Metalness);
            Specularity = new Slider(nameof(Specularity), materialModel.Specularity);
            Glossiness = new Slider(nameof(Glossiness), materialModel.Glossiness);
            Alpha = new Slider(nameof(Alpha), materialModel.Alpha);
            IsShadowCaster = new ToggleButton(materialModel.IsShadowCaster);
        }


        public Guid ID { get; set; }

        public BeatModifier BeatModifier { get; set; }
        public Texture Texture { get; set; }
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


        public void SetMasterBeat(MasterBeat masterBeat)
        {
            BeatModifier.SetMasterBeat(masterBeat);
            Texture.SetMasterBeat(masterBeat);
            Mask.SetMasterBeat(masterBeat);
        }

        public IModel GetModel()
        {
            MaterialModel model = new MaterialModel();
            model.ID = this.ID;

            model.Texture = (TextureModel)Texture.GetModel();
            model.Mask = (MaskModel)Mask.GetModel();
            model.MaskChannelSelector = (ComboBoxModel<MaskChannel>)MaskChannelSelector.GetModel();

            model.BeatModifierModel = (BeatModifierModel)BeatModifier.GetModel();

            model.Pipeline = (ComboBoxModel<PipelineType>)Pipeline.GetModel();
            model.CullMode = (ComboBoxModel<CullModeType>)CullMode.GetModel();
            model.Transparency = (ComboBoxModel<TransparencyType>)Transparency.GetModel();

            model.Metalness = (SliderModel)Metalness.GetModel();
            model.Specularity = (SliderModel)Specularity.GetModel();
            model.Glossiness = (SliderModel)Glossiness.GetModel();
            model.Alpha = (SliderModel)Alpha.GetModel();

            model.IsShadowCaster = (ToggleButtonModel)IsShadowCaster.GetModel();

            return model;
        }

        public void SetViewModel(IModel model)
        {
            MaterialModel materialModel = model as MaterialModel;
            this.ID = materialModel.ID;

            this.Texture.SetViewModel(materialModel.Texture);
            this.Mask.SetViewModel(materialModel.Mask);
            this.MaskChannelSelector.SetViewModel(materialModel.MaskChannelSelector);

            this.BeatModifier.SetViewModel(materialModel.BeatModifierModel);

            this.Pipeline.SetViewModel(materialModel.Pipeline);
            this.CullMode.SetViewModel(materialModel.CullMode);
            this.Transparency.SetViewModel(materialModel.Transparency);

            this.Metalness.SetViewModel(materialModel.Metalness);
            this.Specularity.SetViewModel(materialModel.Specularity);
            this.Glossiness.SetViewModel(materialModel.Glossiness);
            this.Alpha.SetViewModel(materialModel.Alpha);

            this.IsShadowCaster.SetViewModel(materialModel.IsShadowCaster);
        }

        public void Dispose()
        {
            BeatModifier.Dispose();
        }
    }
}
