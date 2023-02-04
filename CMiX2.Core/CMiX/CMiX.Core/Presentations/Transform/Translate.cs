// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.BaseControl;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Translate : ObservableObject, ITransformModifier
    {
        public Translate(TranslateModel translateModel) 
        {
            this.ID = translateModel.ID;
            this.Visible = new BooleanValue(translateModel.Visible);
            XYZ = new Vector3(translateModel.XYZ);
            IsExpanded = true;
        }

        public Guid ID { get; set; }
        public Vector3 XYZ { get; set; }
        public BooleanValue Visible { get; set; }


        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        public void Dispose()
        {

        }
    }
}
