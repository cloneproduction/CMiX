// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using CMiX.Core.Models;

namespace CMiX.Core.Presentation.ViewModels
{
    public interface ITransformModifier
    {
        TransformModifierNames Name { get; set; }
        ObservableCollection<Transform> Transforms { get; set; }
        ModifierType SelectedModifierType { get; set; }


        void UpdateOnBeatTick(double period);
        void UpdateOnGameLoop(double period);


        IModel GetModel();
        void SetViewModel(IModel model);
    }
}
