// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.Scheduling
{
    public class CompositionSchedulerModel : IModel
    {
        public CompositionSchedulerModel()
        {
            ID = Guid.NewGuid();

            JobSchedulerModel = new JobSchedulerModel();
            JobEditorModel = new JobEditorModel();
        }

        public Guid ID { get; set; }
        public string Name { get; set; }
        public JobSchedulerModel JobSchedulerModel { get; set; }
        public JobEditorModel JobEditorModel { get; set; }
    }
}
