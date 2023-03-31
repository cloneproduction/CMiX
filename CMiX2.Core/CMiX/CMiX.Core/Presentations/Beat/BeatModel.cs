// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.Presentations.Beat
{
    public class BeatModel : IModel
    {
        //public BeatModel()
        //{
        //    this.ID = Guid.NewGuid();
        //}

        public Guid ID { get; set; }
        public float[] Periods { get; set; }
        public float Period { get; set; }
    }
}
