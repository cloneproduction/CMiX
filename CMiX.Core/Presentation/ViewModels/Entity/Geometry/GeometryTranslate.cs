// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using Memento;

namespace CMiX.Core.Presentation.ViewModels
{
    public class GeometryTranslate : ObservableObject
    {
        #region CONSTRUCTORS
        public GeometryTranslate(string messageAddress, Mementor mementor)
        {

        }
        #endregion

        #region PROPERTIES
        private GeometryTranslateMode _Mode;
        public GeometryTranslateMode Mode
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
        #endregion

        #region COPY/PASTE/RESET
        public void Reset()
        {
            Mode = default;
        }

        public void CopyModel(GeometryTranslateModel geometryTranslateModel)
        {
            geometryTranslateModel.Mode = Mode;
        }

        public void SetViewModel(GeometryTranslateModel geometryTranslateModel)
        {
            Mode = geometryTranslateModel.Mode;
        }
        #endregion
    }
}
