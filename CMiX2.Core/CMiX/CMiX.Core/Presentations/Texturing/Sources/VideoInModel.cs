// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControl;

namespace CMiX.Core.Presentations.Texturing.Sources
{
    public class VideoInModel : IModel
    {
        public VideoInModel()
        {
            ID = Guid.NewGuid();
            SizeX = new IntegerValueModel(1920);
            SizeY = new IntegerValueModel(1080);
        }

        public Guid ID { get; set; }

        public IntegerValueModel SizeX { get; set; }
        public IntegerValueModel SizeY { get; set; }
    }
}
