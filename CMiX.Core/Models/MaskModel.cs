// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Models
{
    public class MaskModel : IModel
    {
        public MaskModel()
        {
            KeepOriginal = true;
            MaskType = ((MaskType)2).ToString();
            MaskControlType = ((MaskControlType)1).ToString();
            Enabled = false;
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }
        public bool IsMask { get; set; }
        public bool KeepOriginal { get; set; }
        public string MaskType { get; set; }
        public string MaskControlType { get; set; }
    }
}
