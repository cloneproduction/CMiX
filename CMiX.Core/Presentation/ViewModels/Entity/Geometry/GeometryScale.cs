// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class GeometryScale : ObservableObject
    {
        public GeometryScale()
        {
            Mode = default;
        }

        private GeometryScaleMode _Mode;
        public GeometryScaleMode Mode
        {
            get => _Mode;
            set
            {
                //if(Mementor != null)
                //    Mementor.PropertyChange(this, nameof(Mode));
                SetProperty(ref _Mode, value);
                //SendMessages(MessageAddress + nameof(Mode), Mode);
            }
        }
    }
}
