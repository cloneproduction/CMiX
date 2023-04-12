// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentations.Service;
using CMiX.Core.Presentations.Transform;
using CMiX.Core.Presentations.ViewModels.BaseControl;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentations.ViewModels
{
    public partial class Translate : ObservableObject, IModifier
    {
        public Translate(TranslateModel translateModel, CompositionService compositionService) 
        {
            this.ID = translateModel.ID;
            this.Visible = new BooleanValue(translateModel.Visible, compositionService);
            XYZ = new Vector3(translateModel.XYZ, compositionService);
            isExpanded = true;
        }

        public Guid ID { get; set; }
        public Vector3 XYZ { get; set; }
        public BooleanValue Visible { get; set; }

        [ObservableProperty]
        private bool isExpanded;

        public void Dispose()
        {

        }
    }
}
