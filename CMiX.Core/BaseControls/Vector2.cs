// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.BaseControls
{
    public class Vector2 : ObservableObject, IControl
    {
        public Vector2()
        {
            X = new FloatValue(0.0f);
            Y = new FloatValue(0.0f);
        }

        public Vector2(float x, float y) : this()
        {
            X.Value = x;
            Y.Value = y;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public FloatValue X { get; set; }
        public FloatValue Y { get; set; }
    }
}
