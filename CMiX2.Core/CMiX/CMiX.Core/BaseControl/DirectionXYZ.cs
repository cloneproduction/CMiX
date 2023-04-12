// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControl;
using CMiX.Core.Presentations.Service;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentations.ViewModels
{
    public class DirectionXYZ : ObservableRecipient, IControl
    {
        public DirectionXYZ(DirectionXYZModel directionXYZModel, CompositionService compositionService)
        {
            ID = directionXYZModel.ID;
            DirectionX = new BooleanValue(directionXYZModel.DirectionX, compositionService);
            DirectionY = new BooleanValue(directionXYZModel.DirectionY, compositionService);
            DirectionZ = new BooleanValue(directionXYZModel.DirectionZ, compositionService);
        }

        public Guid ID { get; set; }
        public BooleanValue DirectionX { get; set; }
        public BooleanValue DirectionY { get; set; }
        public BooleanValue DirectionZ { get; set; }
    }
}
