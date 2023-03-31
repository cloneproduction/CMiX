// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.Presentations.Scheduling
{
    public class SchedulerManagerModel : IModel
    {
        public SchedulerManagerModel()
        {
            ID = new Guid("22223344-5566-7788-99AA-BBCCDDEEFF00");
            PlaylistEditorModel = new PlaylistEditorModel();
        }

        public Guid ID { get; set; }
        public PlaylistEditorModel PlaylistEditorModel { get; set; }
    }
}
