// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;

namespace CMiX.Core.Animations
{
    public record MasterBeatModel : IControlModel
    {
        public const int PeriodCount = 15;

        // The table entry that holds the period of Index 0.
        public const int CenterIndex = (PeriodCount - 1) / 2;

        public MasterBeatModel()
        {
            // initialize Periods array using LINQ for simplicity
            float multiplier = 1f / 128f;
            Periods = Enumerable.Range(0, PeriodCount)
                .Select(i => (multiplier *= i == 0 ? 1 : 2) * Period.Value)
                .ToArray();
        }
        public Guid ID { get; init; } = Guid.NewGuid();
        public ButtonModel Resync { get; init; } = new();
        public GenericValueModel<bool> Pause { get; init; } = new(false);
        public GenericValueModel<int> Index { get; init; } = new(0);
        public GenericValueModel<int> BeatIndex { get; init; } = new(CenterIndex);
        public GenericValueModel<float> Period { get; init; } = new(1000);
        public float[] Periods { get; init; }
    }
}
