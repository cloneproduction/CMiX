// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.Models.Scheduling
{
    public class SchedulerManagerModel : IModel
    {
        public SchedulerManagerModel()
        {
            this.ID = new Guid("22223344-5566-7788-99AA-BBCCDDEEFF00");
            PlaylistEditorModel = new PlaylistEditorModel();
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }
        public PlaylistEditorModel PlaylistEditorModel { get; set; }
    }
}
