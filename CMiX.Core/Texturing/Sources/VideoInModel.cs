// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Texturing.Sources
{
    public class VideoInModel : IControlModel
    {
        public VideoInModel()
        {
            ID = Guid.NewGuid();
            SizeX = new GenericValueModel<int>(1920);
            SizeY = new GenericValueModel<int>(1080);
        }

        public Guid ID { get; set; }
        public GenericValueModel<int> SizeX { get; set; }
        public GenericValueModel<int> SizeY { get; set; }
    }
}
