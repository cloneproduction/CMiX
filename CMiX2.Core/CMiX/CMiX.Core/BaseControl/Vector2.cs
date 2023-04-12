// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControl;
using CMiX.Core.Presentations.Service;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentations.ViewModels.BaseControl
{
    public class Vector2 : ObservableObject, IControl
    {
        public Vector2(Vector2Model vector2Model, CompositionService compositionService)
        {
            ID = vector2Model.ID;
            X = new FloatValue(vector2Model.X, compositionService);
            Y = new FloatValue(vector2Model.Y, compositionService);
        }

        public Guid ID { get; set; }

        public FloatValue X { get; set; }
        public FloatValue Y { get; set; }
    }
}
