// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Models.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels.BaseControl
{
    public class Vector2 : ObservableObject, IControl
    {
        public Vector2(Vector2Model vector2Model)
        {
            ID = vector2Model.ID;

            X = new FloatValue(vector2Model.X);
            Y = new FloatValue(vector2Model.Y);
        }

        public Guid ID { get; set; }

        public FloatValue X { get; set; }
        public FloatValue Y { get; set; }
    }
}
