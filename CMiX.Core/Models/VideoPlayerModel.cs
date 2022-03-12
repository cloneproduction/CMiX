// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.Models
{
    public class VideoPlayerModel :IModel
    {
        public VideoPlayerModel()
        {
            ID = Guid.NewGuid();
            SeekFrame = new CounterModel();
            DoSeek = new ButtonModel();
            PlayModel = new ToggleButtonModel();
            PlayModel.IsChecked = true;
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }

        public ButtonModel DoSeek { get; set; }
        public CounterModel SeekFrame { get; set; }
        public ToggleButtonModel PlayModel { get; set; }
    }
}
