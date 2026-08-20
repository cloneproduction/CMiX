// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core
{
    public static class ManagerIDs
    {
        public static readonly Guid Project = Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF00");
        public static readonly Guid CompositionManager = Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF01");
        public static readonly Guid TextureManager = Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF02");
        public static readonly Guid MaterialManager = Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF03");
        public static readonly Guid EntityManager = Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF04");
        public static readonly Guid CameraManager = Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF05");
        public static readonly Guid LightManager = Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF06");
        public static readonly Guid BeatManager = Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF07");
        public static readonly Guid ColorPaletteManager = Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF08");

        public static readonly Guid Index = Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF09");
        public static readonly Guid Period = Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF10");
        public static readonly Guid BeatIndex = Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF11");
        public static readonly Guid Pause = Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF12");
        public static readonly Guid Resync = Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF13");

        public static readonly Guid MasterBeat = Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF5B");

        // Project.PrefabService.Name/IsSelected/Visibility, fixed the same way PrefabService.ID is.
        public static readonly Guid ProjectPrefabServiceName = Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF5C");
        public static readonly Guid ProjectPrefabServiceIsSelected = Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF5D");
        public static readonly Guid ProjectPrefabServiceVisibility = Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF5E");

        // Fixed like everything above: OutputMappingManager and its 10 slots are created directly
        // at Project startup on both the .NET and vvvv sides, never announced through
        // MessageAddItem, so both sides must agree on these IDs up front rather than learn them
        // from a message.
        public static readonly Guid OutputMappingManager = Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF14");
        public static readonly Guid[] OutputMappingSlots =
        {
            Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF15"),
            Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF16"),
            Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF17"),
            Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF18"),
            Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF19"),
            Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF1A"),
            Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF1B"),
            Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF1C"),
            Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF1D"),
            Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF1E"),
        };

        // Same reasoning one level deeper: each OutputMapping's own GenericValue<T> fields
        // (Name, Resolution and its X/Y, TexcoordSemantic, Visibility) also default to a random
        // Guid.NewGuid() unless overridden, and since they never travel over MessageAddItem either,
        // both sides need to agree on these too - see MasterBeat.cs for the same pattern applied to
        // Index/Period/BeatIndex/Pause/Resync.
        public static readonly OutputMappingSlotFieldIds[] OutputMappingSlotFields =
        {
            new(Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF1F"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF20"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF21"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF22"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF23"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF24")),
            new(Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF25"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF26"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF27"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF28"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF29"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF2A")),
            new(Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF2B"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF2C"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF2D"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF2E"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF2F"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF30")),
            new(Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF31"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF32"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF33"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF34"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF35"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF36")),
            new(Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF37"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF38"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF39"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF3A"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF3B"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF3C")),
            new(Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF3D"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF3E"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF3F"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF40"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF41"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF42")),
            new(Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF43"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF44"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF45"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF46"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF47"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF48")),
            new(Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF49"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF4A"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF4B"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF4C"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF4D"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF4E")),
            new(Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF4F"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF50"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF51"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF52"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF53"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF54")),
            new(Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF55"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF56"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF57"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF58"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF59"), Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF5A")),
        };
    }

    public record struct OutputMappingSlotFieldIds(
        Guid Name,
        Guid Resolution,
        Guid ResolutionX,
        Guid ResolutionY,
        Guid TexcoordSemantic,
        Guid Visibility);
}
