# CMiXShaders

This folder contains tile aware TextureFX shaders for CMiX. They are based on shaders copied from VL.Addons. They are for a multi machine projection. Every machine renders one tile of a shared composition, and overlapping projectors show identical texels. The design is in `tiled-rendering-documentation.md`.

## Requirements

* vvvv gamma with VL.Stride.
* VL.Addons.Stride 0.5.5 or newer.

The shaders inherit `AddonShaderUtils`, `BlurUtils`, `NoiseFunctions` and `Spectral` from VL.Addons.Stride, and `HappyNoise` from VL.Stride. This repository does not contain copies of these shaders.

## Naming

* A tile aware shader carries `CMiX` as the vvvv version segment. The file name is `<Name>_CMiX_TextureFX.sdsl`, for example `Edge_CMiX_TextureFX.sdsl`. vvvv reads this as the node "<Name> (CMiX)", for example "Edge (CMiX)". The old prefix form gave the node "CMiX (Edge)". A hyphen is not allowed in an SDSL identifier, so `CMiX-Edge` is not possible.
* The blur base is `BlurPassBase_CMiX.sdsl`. The blur passes are `BlurPass1_CMiX_TextureFX.sdsl`, `BlurPass2_CMiX_TextureFX.sdsl` and `BlurPass3_CMiX_TextureFX.sdsl`.
* The shader name equals the file name. `TileParams` and `ScreenTile` keep their names.
* The `_Internal` suffix is gone, so every node is visible in vvvv.
* The wrapper patches for the three blur passes and the two HeightShadows passes must use the new node names. These patches are in the CMiX project, outside this repository.

## Shared mixins

`Common/TileParams.sdsl` holds the tile pins and the conversions:

* Pins: `TileOffset`, `TileSize` (the tile rectangle in composition UV) and `CompositionResolution` (the composition size in texels).
* `TileToCompositionUV(tileUV)`: tile UV to composition UV.
* `CompositionToTileUV(compositionUV)`: composition UV to tile UV.
* `TileToCompositionTexel(tileUV)`: tile UV to a position in composition texels.
* `CompositionTexelIndex(tileUV, viewSize)`: the integer composition texel of a tile pixel. The integer math gives the same bits on every machine.

`Common/ScreenTile.sdsl` is the vertex stage remap for sources. It must be the last entry of the inheritance list.

A source adds `ScreenTile` as the last entry of its inheritance list. `ScreenTile` changes `streams.TexCoord` to composition UV in the vertex stage, so the body of the source stays the same. A filter adds `TileParams`, keeps `streams.TexCoord` in tile UV to sample its input, and uses the methods for its position math and its offsets.

## Decisions

* **Pin meaning.** A pin that is a fraction of the texture stays a fraction of the composition. At the full tile, the output is identical to the original. A preview at a lower resolution with the same aspect keeps the same pin values.
* **Mip maps.** The tile computation aligns the tiles to multiples of 2^L (64 or 128). The shaders take their LOD from `CompositionResolution`. The alignment step is patch work in the tile computation, outside this repository. Section 2.5 of the documentation specifies it.
* **Integer texel helper.** `TileParams` has `CompositionTexelIndex`. Hashes and grid snapping use it. The HeightShadows jitter patterns are different from the original files.
* **Unbounded gathers.** `Kaleidoscope` and `Transform` are not compliant. They are removed, because VL.Addons.Stride has them. `Texturize_CMiX` is a CMiX original. It is kept and renamed, with no code change.
* **Removal.** A shader that needs no change and exists in VL.Addons.Stride or VL.Stride is removed from this repository. The project uses the package originals. The external includes `BlurUtils`, `ThresholdOperations` and `TextureUtils` need no change, so this repository has no copies.
* **Rename.** Every shader that stays has a new name: no `_Internal`, and `CMiX` as the vvvv version segment. The blur files also drop `Addon`.

## Shaders

Categories: G1 source, G2 per pixel, G3 neighbourhood, G4 position dependent, G5 gather, G7 global statistic, G8 mixer, G9 random or time. "Mip" means that the shader samples a mip level above 0. The reach is in composition texels. `R` is `CompositionResolution`. `max R` is the larger of its two components. The pad of the tile must be at least the sum of the reaches of the chain.

| File | Category | Change | Pad reach (composition texels, R = CompositionResolution) | Compliant |
|---|---|---|---|---|
| `Common/TileParams.sdsl` | Shared mixin | New. Pins and conversions. | n/a | n/a |
| `Common/ScreenTile.sdsl` | Shared mixin | New. Vertex stage remap for sources. | n/a | n/a |
| `Sources/BubbleNoise_CMiX_TextureFX.sdsl` | G1 | Adds `ScreenTile`. The neighbour step is one composition texel, `1 / CompositionResolution`. | 0 | Yes |
| `Sources/Checkerboard_CMiX_TextureFX.sdsl` | G1 | Adds `ScreenTile`. | 0 | Yes |
| `Sources/Gradient_CMiX_TextureFX.sdsl` | G1 | Adds `ScreenTile`. | 0 | Yes |
| `Sources/Gradient_Mesh_CMiX_TextureFX.sdsl` | G1 | Adds `ScreenTile`. The aspect comes from `CompositionResolution`. | 0 | Yes |
| `Sources/MyNoise_CMiX_TextureFX.sdsl` | G1 | Moved from `Filters/`. Adds `ScreenTile`. | 0 | Yes |
| `Sources/Noise_CMiX_TextureFX.sdsl` | G1 | Adds `ScreenTile`. | 0 | Yes |
| `Sources/Voronoi_Border_CMiX_TextureFX.sdsl` | G1 | Adds `ScreenTile`. | 0 | Yes. A pixel on a cell edge can differ. |
| `Sources/Voronoi_Dots_CMiX_TextureFX.sdsl` | G1 | Adds `ScreenTile`. | 0 | Yes. A pixel on a cell edge can differ. |
| `Filters/BlurPassBase_CMiX.sdsl` | G3, Mip, G7 at high strength | Adds `TileParams`. The LOD and the offset come from `CompositionResolution`. The offset is divided by `TileSize`. | `0.93 * 2^lod`, plus the mip footprint `2^lod`, with `lod = Strength * log2(max R)` | Yes, exact up to `Strength = Lmax / log2(max R)` |
| `Filters/BlurPass1_CMiX_TextureFX.sdsl` | G3, Mip | Inherits `BlurPassBase_CMiX`. The body is unchanged. | As `BlurPassBase_CMiX` | As `BlurPassBase_CMiX` |
| `Filters/BlurPass2_CMiX_TextureFX.sdsl` | G3, Mip | Inherits `BlurPassBase_CMiX`. | As `BlurPassBase_CMiX` | As `BlurPassBase_CMiX` |
| `Filters/BlurPass3_CMiX_TextureFX.sdsl` | G3, Mip | Inherits `BlurPassBase_CMiX`. The body is unchanged. | As `BlurPassBase_CMiX`, with `lod = 0.75 * Strength * log2(max R)` | As `BlurPassBase_CMiX` |
| `Filters/Edge_CMiX_TextureFX.sdsl` | G3, Mip | Adds `TileParams`. The LOD and the texel size come from `CompositionResolution`. The kernel offsets are divided by `TileSize`. `Charcoal` keeps a step of one texel. | Per axis `rad * R.xy / min(R)`, plus `0.001 * R` (MotorSaw), plus the mip footprint `2^v` | Yes, exact up to Lmax. The AieKick mode (`fwidth`) needs 2^L aligned tiles. |
| `Filters/Dither_CMiX_TextureFX.sdsl` | G4 | Adds `TileParams`. The Bayer and PS1 patterns come from `CompositionTexelIndex`. | 0 | Yes |
| `Filters/Displace_CMiX_TextureFX.sdsl` | G5 bounded, G8 | Adds `TileParams`. The offset is divided by `TileSize`. `LinearSampler` replaces `LinearRepeatSampler`. | `OffsetScale * max(abs(map - Offset)) * R` (819 at the defaults, R = 16384) | Yes. `Texture1` must be the same tile as `Texture0`. |
| `Filters/ShiftRGB_CMiX_TextureFX.sdsl` | G5 bounded | Adds `TileParams`. The offset is divided by `TileSize`. | `0.1 * Shift * R` (164 at the defaults, R = 16384) | Yes |
| `Filters/Tiles_CMiX_TextureFX.sdsl` | G4, G5 | Adds `TileParams`. The grid is in composition space from `CompositionTexelIndex`, centred on the composition centre. The sample goes through `CompositionToTileUV`. | `0.5 * FactorXY` | Yes |
| `Filters/LEDPanel_CMiX_TextureFX.sdsl` | G4, G5 | Adds `TileParams`. The cell grid is in composition space from `CompositionTexelIndex`. The sample goes through `CompositionToTileUV`. | `PixelSize * (1 + 3 * MaskStagger)` | Yes |
| `Filters/ASCII_CMiX_TextureFX.sdsl` | G4, G5, `Texture1` static | Adds `TileParams`. The half resolution grid is in composition space from `CompositionTexelIndex`. The cell sample goes through `CompositionToTileUV`. The glyph lookup in `Texture1` is unchanged. | `2 * gridSize` | Yes. `Texture1` is a shared static glyph atlas. |
| `Filters/Halftone_CMiX_TextureFX.sdsl` | G4, G5, `Texture1` static | Adds `TileParams`. The dot grid is in composition UV from `CompositionTexelIndex`. The `Texture0` samples go through `CompositionToTileUV` with `LinearSampler` instead of `LinearRepeatSampler`. The `Texture1` pattern keeps `LinearRepeatSampler`. | `1.5 * R / NumberOfTiles` | Yes. `Texture1` is a shared static pattern. |
| `Filters/HeightShadows_Pass0_CMiX_TextureFX.sdsl` | G5, G9, no mip (level 0 only) | Adds `TileParams`. The ray marches in composition UV. The height samples go through `CompositionToTileUV`. The jitter seed is the composition texel index. | About `(RayLength + 4.8 * RayJitter) * R` | Yes. Only small `RayLength` and `RayJitter` values fit a pad. `Texture1` must be the same tile. |
| `Filters/HeightShadows_Pass1_CMiX_TextureFX.sdsl` | G5, G9, Mip | Adds `TileParams`. The tap offsets are divided by `TileSize`. The seeds are the composition texel index. The LOD comes from `CompositionResolution`. | `1.25 * ShadowBlurAmount * R`, plus the mip footprint `2^lod`, with `lod` up to `0.25 * log2(max R)` | Yes, exact up to Lmax |
| `Filters/TexArrayBlend_CMiX_TextureFX.sdsl` | G8, per pixel | Rename only. No code change. | 0 | Yes. All array slices must be the same tile. |
| `Filters/Texturize_CMiX_TextureFX.sdsl` | G4, grid at the tile origin | Rename only. No code change. | n/a | No, unless its input `Texture0` is a shared static texture. |

`Texturize_CMiX` samples `Texture0` near the tile origin for every pixel. A later conversion is possible: a grid in composition space and the aspect from `CompositionResolution`.

## Removed shaders

These files were in the repository before this work. The project uses the package originals.

| File | Reason | Pad reach | Original in |
|---|---|---|---|
| `Common/AddonColorUtility.sdsl` | Pure include | n/a | VL.Addons.Stride, `shaders/Common` |
| `Common/AddonShaderUtils.sdsl` | Pure include | n/a | VL.Addons.Stride, `shaders/Common` |
| `Common/InvertOperations.sdsl` | Pure include | n/a | VL.Addons.Stride, `shaders/Common` |
| `Common/NoiseFunctions.sdsl` | Pure include | n/a | VL.Addons.Stride, `shaders/Common` |
| `Common/Spectral.sdsl` | Pure include | n/a | VL.Addons.Stride, `shaders/Common` |
| `HappyLibs/HappyCalc.sdsl` | Pure include | n/a | VL.Stride, `Effects/HappyLibs` |
| `HappyLibs/HappyNoise.sdsl` | Pure include | n/a | VL.Stride, `Effects/HappyLibs` |
| `HappyLibs/HappyTransform.sdsl` | Pure include | n/a | VL.Stride, `Effects/HappyLibs` |
| `HappyLibs/VectorUtils.sdsl` | Pure include | n/a | VL.Stride, `Effects/HappyLibs` |
| `Filters/TriColorOperations.sdsl` | Pure include | n/a | VL.Addons.Stride, `shaders/TextureFX/Filters` |
| `Filters/HSCB_Internal_TextureFX.sdsl` | Per pixel | 0 | VL.Addons.Stride, `shaders/TextureFX/Filters` |
| `Filters/Invert_Internal_TextureFX.sdsl` | Per pixel | 0 | VL.Addons.Stride, `shaders/TextureFX/Filters` |
| `Filters/Threshold_Internal_TextureFX.sdsl` | Per pixel. `fwidth` needs 2^L aligned tiles. | 0 | VL.Addons.Stride, `shaders/TextureFX/Filters` |
| `Filters/TriColor_Internal_TextureFX.sdsl` | Per pixel | 0 | VL.Addons.Stride, `shaders/TextureFX/Filters` |
| `Utils/SetAlpha_Internal_TextureFX.sdsl` | Per pixel. A graph on the `AlphaMask` pin must be tile safe. | 0 | VL.Addons.Stride, `shaders/TextureFX/Utils` |
| `Filters/Kuwahara_Internal_TextureFX.sdsl` | Texel unit neighbourhood | `1.42 * KernelSize` | VL.Addons.Stride, `shaders/TextureFX/Filters` |
| `Filters/Kuwahara_Papari_Internal_TextureFX.sdsl` | Texel unit neighbourhood | `Radius` | VL.Addons.Stride, `shaders/TextureFX/Filters` |
| `Filters/Kuwahara_PolynomalWeighting_Internal_TextureFX.sdsl` | Texel unit neighbourhood | `Radius` | VL.Addons.Stride, `shaders/TextureFX/Filters` |
| `Filters/Kuwahara_Anisotropic_Internal_TextureFX.sdsl` | Texel unit neighbourhood. `Texture1` must be the same tile. | `1.04 * Radius` | VL.Addons.Stride, `shaders/TextureFX/Filters` |
| `Filters/Kaleidoscope_Internal_TextureFX.sdsl` | Not compliant, unbounded gather. It reaches the whole texture. | n/a | VL.Addons.Stride, `shaders/TextureFX/Filters` |
| `Filters/Transform_Internal_TextureFX.sdsl` | Not compliant, unbounded gather. A rotation or a scale reaches across the composition. | n/a | VL.Addons.Stride, `shaders/TextureFX/Filters` |

The folders `HappyLibs/` and `Utils/` are gone. `Common/` now contains only the two shared mixins.

## Verification

Verification runs in vvvv. Follow section 4 of `tiled-rendering-documentation.md`.

1. **Compilation.** vvvv gamma compiles every shader. Check the node browser and the shader error window. The project must reference VL.Addons.Stride 0.5.5 or newer.
2. **Identity test.** Set `TileOffset = (0, 0)`, `TileSize = (1, 1)` and `CompositionResolution` to the output size. The output must be identical to the original shader. There are two exceptions:
   * HeightShadows: the jitter seed is now the texel index, so the noise pattern is different.
   * Displace and Halftone: the sampler changed from Wrap to Clamp, so a band along the composition border is different.
3. **Compliance test.** Render two instances with overlapping tiles. Align the tiles to multiples of 2^L. Crop the overlap from each and subtract. The difference must be zero for every changed shader when the pad is larger than the reach.
4. **Pad test.** For each G3 and G5 shader, set the pad to 0. A seam must appear at the tile edge. Set the production pad. The seam must disappear.
5. **Mip alignment test.** For `BlurPass1_CMiX`, `BlurPass2_CMiX`, `BlurPass3_CMiX`, `Edge_CMiX` and `HeightShadows_Pass1_CMiX`, repeat test 3 with tile origins that are not multiples of 2^L. A mismatch must appear. Align the tiles. The mismatch must disappear.

`LinearSampler` and `Sampler0` declare no address mode. The shaders use the Stride default, which is expected to be Clamp. Verify this default once.

