// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Animations
{
    public class MasterBeatModel : IControlModel
    {
        public MasterBeatModel()
        {
            Resync = new ButtonModel();
            Pause = new GenericValueModel<bool>(false);
            Index = new GenericValueModel<int>(3);
            BeatIndex = new GenericValueModel<int>(0);
            Period = new GenericValueModel<float>(1000);
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public ButtonModel Resync { get; set; }
        public GenericValueModel<bool> Pause { get; set; }
        public GenericValueModel<int> Index { get; set; }
        public GenericValueModel<int> BeatIndex { get; set; }
        public GenericValueModel<float> Period { get; set; }
    }
}
