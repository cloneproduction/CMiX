// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Modifiers;
using CMiX.Core.Presentation.ViewModels.Prefab;
using CMiX.Core.Presentation.ViewModels.Service;
using CMiX.Core.Presentation.Views.Dialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Coloration : ObservableObject, IControl, IDisposable, IPrefab
    {
        public Coloration(ColorationModel colorationModel, CompositionService compositionService)
        {
            ID = colorationModel.ID;
            Name = this.GetType().Name;
            CompositionService = compositionService;
            OpenColorSelectorCommand = new RelayCommand(OpenColorSelector);

            ModifierManager = new ModifierManager(colorationModel.ModifierManager, new ColorModifierFactory(compositionService));
            ColorSelector = new ColorSelector(colorationModel.ColorSelector);
        }


        public ICommand OpenColorSelectorCommand { get; set; }
        public Guid ID { get; set; }
        public ModifierManager ModifierManager { get; set; }
        public ColorSelector ColorSelector { get; set; }
        public CompositionService CompositionService { get; set; }


        private bool _isRenaming;
        public bool IsRenaming
        {
            get => _isRenaming;
            set => SetProperty(ref _isRenaming, value);
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }


        public void OpenColorSelector()
        {
            CompositionService.DialogService.Show<ColorSelectorWindow>(this, this.ColorSelector);
        }


        public IModel GetModel()
        {
            ColorationModel colorationModel = new ColorationModel();
            colorationModel.ID = ID;
            colorationModel.ColorSelector = (ColorSelectorModel)ColorSelector.GetModel();
            colorationModel.ModifierManager = (ModifierManagerModel)ModifierManager.GetModel();
            return colorationModel;
        }

        public void SetViewModel(IModel model)
        {
            ColorationModel colorationModel = model as ColorationModel;
            ID = colorationModel.ID;
            ColorSelector.SetViewModel(colorationModel.ColorSelector);
            ModifierManager.SetViewModel(colorationModel.ModifierManager);
        }

        public void Dispose()
        {
            
        }
    }
}
