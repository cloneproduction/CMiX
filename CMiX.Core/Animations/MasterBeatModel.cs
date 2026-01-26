// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Animations
{
    public record MasterBeatModel : IControlModel
    {
        public MasterBeatModel()
        {
            // initialize Periods array using LINQ for simplicity
            float multiplier = 1f / 128f;
            Periods = Enumerable.Range(0, 15)
                .Select(i => (multiplier *= i == 0 ? 1 : 2) * Period.Value)
                .ToArray();
        }
        public Guid ID { get; init; } = Guid.NewGuid();
        public ButtonModel Resync { get; init; } = new();
        public GenericValueModel<bool> Pause { get; init; } = new(false);
        public GenericValueModel<int> Index { get; init; } = new(3);
        public GenericValueModel<int> BeatIndex { get; init; } = new(0);
        public GenericValueModel<float> Period { get; init; } = new(1000);
        public float[] Periods { get; init; }
    }
}
