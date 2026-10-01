# Tiled Rendering for Multi Machine Projection

This document describes the tiled rendering setup of CMiX. Each machine computes one tile of a shared 2D composition and projects it onto the walls of a room.

## 1 Overview

### 1.1 Purpose

A 3D room model has walls and a floor. The walls carry one contiguous unrolled UV strip inside the unit square. The floor has UVs outside the unit square. A 2D composition is applied to the walls through the UV strip.

Each machine drives one projector. Each projector illuminates a part of the walls. By default, a TextureFX computes its UVs over 0..1 across its output. Every machine would then compute the entire composition. The setup makes each machine compute only the tile that its projector can see. The machine then renders the room from the projector frustum with the tile on the walls.

The composition is procedural. Any part of it is computable from shared parameters and a composition coordinate. The shared parameters come over the network or from a Redis database.

### 1.2 Pipeline

Each machine runs two stages.

The 2D stage produces the tile texture:

1. A source TextureFX renders the tile. It inherits `ScreenTile`, which remaps the UV of the source to the composition.
2. The effect chain processes the tile texture.

The 3D stage projects the tile:

3. The wall material converts the mesh UV from composition UV to tile UV and samples the tile texture.
4. The renderer draws the room from the projector frustum.
5. The result goes to the projector through the existing warp and edge blend stage.

### 1.3 Terminology

| Term | Meaning |
|------|---------|
| Composition | The whole 2D image that covers all walls. Its UV space is 0..1 across the unrolled strip. |
| Tile | The rectangle of the composition that one machine computes. |
| Tile texture | The texture that holds one tile. |
| `CompositionResolution` | The size of the whole composition in pixels, as a `(width, height)` pair. It is a design choice, identical on every machine. Example: `(16384, 16384)`. |
| `TileOffset`, `TileSize` | The tile rectangle in composition UV. |
| `OutputSize` | The size of the tile texture in texels. |
| Tile UV | 0..1 across the tile texture. Inside a filter, `streams.TexCoord`, `ViewSize`, and `Texture0TexelSize` describe the tile texture. |
| Composition UV | 0..1 across the composition. |
| Composition texel | Composition UV multiplied by `CompositionResolution`. |
| `CompositionTexelIndex` | The integer composition texel of a tile pixel. It is computed with integer math, so it is identical on every machine. |
| Pixel | A cell of a rendered image, for example the UV pass image. |
| Texel | A cell of the composition or of a tile texture. |


File names follow one convention. Tile aware shaders in the CMiX repository carry `CMiX` as the vvvv version segment, so the file name is `<Name>_CMiX_TextureFX.sdsl`, for example `Edge_CMiX_TextureFX.sdsl`. vvvv reads this as the node "<Name> (CMiX)", for example "Edge (CMiX)". The blur base is `BlurPassBase_CMiX.sdsl`. The shader name equals the file name, and the `_Internal` suffix is gone, so every node is visible. The shared shaders `TileParams` and `ScreenTile` keep their names. Shaders that need no change are not kept in the repository. The project uses the versions from VL.Addons.Stride and VL.Stride.

### 1.4 Rules that apply everywhere

**Rule 1: Tile UV covers only the tile.** Inside a TextureFX, 0..1 is the tile, not the composition. A shader that treats 0..1 as the whole picture treats one tile as the whole picture.

**Rule 2: Identical composition texels must give identical output on every machine.** Projectors overlap. The overlap is edge blended. Any difference between two machines for the same composition texel shows in the blend zone.

**Texel density is 1:1.** Every tile is rendered at the same texel density as the whole composition (section 2.6). One tile texel is exactly one composition texel. A distance in texels is the same physical size on every machine. A distance given as a fraction of the texture is a different physical size on every machine.

**All coordinates are texture UV.** Mesh UVs are texture coordinates. `ScreenTile` remaps texture coordinates. The wall material samples with texture coordinates. There is no world space conversion and no y flip.

**The unit square separates walls from floor.** Wall UVs are inside the unit square. Floor UVs are outside it. The test "is the UV inside the unit square" is the test "is this a wall". Section 2.1 uses it to exclude the floor from the tile computation. Section 4.1 uses it to give the floor a border colour.

## 2 Tile computation

The tile is the bounding box of every wall point that the projector illuminates, in composition UV, plus padding. Compute it once at setup. Compute it again when a projector moves, and when the effect chain needs more padding.

The tile is computed by rendering. This gives frustum clipping, occlusion, and back face culling from the renderer. It also gives exactly the UVs that the wall material will sample with.

### 2.1 UV pass material

Use one material for walls and floor. It writes the mesh UV into R and G. It writes an exclusion flag into B: 0 for walls, 1 for the floor.

```hlsl
// UV pass material, for walls and floor.
// R,G = mesh UV.  B = exclusion flag (0 = wall, 1 = floor).  A = 1.
float2 uv = streams.TexCoord;

// all() is true only if every component of the comparison is true.
bool isWall = all(uv >= 0.0) && all(uv <= 1.0);

return float4(uv, isWall ? 0.0 : 1.0, 1.0);
```

In ShaderFX: connect the texture coordinate node to R and G, the inverted comparison to B, and a constant 1 to A.

The material must be unlit. Connect the output to the emissive or unlit colour input. Then no lighting term changes the values. The name of that input in vvvv gamma is not confirmed.

### 2.2 UV pass render

Set the camera to the projector frustum from calibration. Render the whole room, floor included, into a linear floating point render target (`R32G32B32A32_Float`). Clear the target to `(0,0,1,0)`. The `1` in B gives the background the same exclusion flag as the floor. A target of 512x512 is sufficient for a bounding box.

The values must reach the target unmodified. These four things change the values and must be absent from this render:

* Lighting. The unlit material removes it.
* Colour space conversion. An sRGB target or an 8 bit target remaps the values. An 8 bit target quantises UV to 1/255, which is a 64 texel error on a 16384 texel composition.
* Post processing. Tone mapping, colour grading, and bloom change the values.
* Multisampling. An MSAA target is resolved by averaging. A pixel on the wall to floor edge then gets a B between 0 and 1 and a UV averaged across the edge. It passes the exclusion test and injects a wrong UV. The same applies to any resolve or filter on the target.

Keep the floor in the render. The pass uses the depth buffer to decide which wall points the projector reaches. A projector ray that hits the floor stops there. Without the floor, that ray continues to the geometry behind the floor, usually the base of the far wall. That wall part is then marked as covered although the projector does not light it. The floor writes depth and carries flag 1. This gives correct occlusion and correct exclusion at the same time.

### 2.3 Bounding box reduction

The reduction finds the minimum and maximum composition texel over all wall pixels. A compute shader does this on the GPU. Only four values come back to the patch.

HLSL atomic operations (`InterlockedMin`, `InterlockedMax`) exist only for integer types. The reduction therefore converts each UV to a texel index first: `floor(uv * CompositionResolution)` for the minimum, `ceil(uv * CompositionResolution)` for the maximum. Section 2.5 needs exactly this rounding, so nothing is lost.

Atomics are necessary because every thread that finds a wall pixel updates the same four result slots. Without atomics, two threads that write one slot at the same time lose an update. An atomic min or max makes the hardware serialise each update.

Four findings from compiling this shader in vvvv gamma:

* The base is plain `ComputeShaderBase`, without generic thread group parameters. The group size comes from the vvvv base. The dispatch count must be derived from that size.
* `GetDimensions` is not available. The target size comes in through the `InputSize` pin.
* A C style cast on a vector expression, `(uint2)floor(...)`, is rejected. Convert per component.
* The stored form must match the readback type. The buffer is declared `uint` so that the atomics compile. The readback in the patch decides how the 32 bits are interpreted. Two consistent pairs exist. If the readback is float, store the bit pattern of the float with `asuint(floor(...))`. A float readback reverses `asuint` exactly. If the readback is uint, store the numeric value with `uint(floor(...))`. The atomics are correct in both cases. For non negative floats, the bit pattern increases with the value, so `InterlockedMin` and `InterlockedMax` on the patterns select the true minimum and maximum. Mixed pairs fail without an error. Integers read back as float appear as `0` because they are denormals. Float patterns read back as uint appear as values near one billion. The confirmed vvvv setup reads back as float, so the shader uses `asuint`.

```hlsl
// ComputeBoundingBox_ComputeFX.sdsl
// Reduces the UV pass texture to the minimum and maximum composition texel
// of all wall pixels.
//
// A PIXEL is a cell of the UV pass image. A TEXEL is a cell of the composition.
//
// Bounds layout (RWStructuredBuffer<uint>, 4 entries), in composition texels:
//   [0] = min X   [1] = min Y   [2] = max X   [3] = max Y
// The patch must set [0],[1] to 0xFFFFFFFF and [2],[3] to 0 before each dispatch.
// The atomics only move the minimum down and the maximum up.
shader ComputeBoundingBox_ComputeFX : ComputeShaderBase
{
    // The UV pass render target. R,G = mesh UV, B = exclusion flag (1 = not wall).
    Texture2D InputTexture;

    // Size of InputTexture in pixels. Derive the dispatch count from the same value.
    int2 InputSize;

    // Size of the composition in texels.
    int2 CompositionResolution;

    // Four uints, layout above.
    RWStructuredBuffer<uint> Bounds;

    override void Compute()
    {
        // The pixel of this thread in the UV pass image.
        int2 pixel = (int2)streams.DispatchThreadId.xy;

        // Threads past the image edge do nothing.
        if (pixel.x >= InputSize.x || pixel.y >= InputSize.y)
            return;

        // The value at that pixel: R,G = mesh UV, B = exclusion flag.
        float3 uvPass = InputTexture.Load(int3(pixel, 0)).rgb;

        // Floor and background carry flag 1. Exclude them.
        // A threshold survives a filtered target. An equality test does not.
        if (uvPass.b > 0.5)
            return;

        // UV to a position in composition texels.
        float2 texelPos = uvPass.rg * (float2)CompositionResolution;

        // floor for the minimum, ceil for the maximum, then store the bit
        // pattern with asuint. A float readback in the patch reverses asuint.
        // If the readback is changed to uint, use uint(...) instead.
        uint lox = asuint(floor(texelPos.x));
        uint loy = asuint(floor(texelPos.y));
        uint hix = asuint(ceil (texelPos.x));
        uint hiy = asuint(ceil (texelPos.y));

        // Atomic min and max into the shared bounds.
        InterlockedMin(Bounds[0], lox);
        InterlockedMin(Bounds[1], loy);
        InterlockedMax(Bounds[2], hix);
        InterlockedMax(Bounds[3], hiy);
    }
};
```

Patch procedure:

1. Before each dispatch, upload `[0xFFFFFFFF, 0xFFFFFFFF, 0, 0]` into `Bounds` from a spread. Without this reset, a stale result survives.
2. Connect the dimensions of the UV pass target to `InputSize`. Derive the dispatch count from the same value: `ceil(InputSize / groupSize)` per axis. `groupSize` is the thread group size of the vvvv `ComputeShaderBase`.
3. Read `Bounds` back as float. The four values are `minTexelX, minTexelY, maxTexelX, maxTexelY`. They are floats that hold whole numbers. Convert them to integers before section 2.5. If no wall pixel was found, the minimum slots keep `0xFFFFFFFF`, which reads back as NaN. Use this as the "empty tile" signal.

Every wall pixel performs four atomics on the same four addresses. For a 512x512 target this takes less than a millisecond. If the reduction runs per frame or on a much larger target, use a two level reduction: each thread group reduces its pixels in `groupshared` memory, then issues one atomic per group.

The reduction gives only the bounds. The cluster check in section 2.4 is a connectivity question. It stays a visual inspection.

### 2.4 Cluster check

Inspect the B channel of the UV pass render. With a contiguous UV strip, the wall pixels (B = 0) form one connected area.

Two clusters at opposite ends of the strip mean that the projector straddles the unroll seam. The unroll seam is where the loop of walls was cut. Move the unroll seam in the UV layout so that no projector straddles it. A doorway or a boundary between two projectors is a suitable place.

A thin line of wall pixels along the floor edge means that a vertex on the wall to floor boundary is shared and has one UV. A floor triangle that uses it interpolates from a wall UV to the floor UVs and passes through the unit square. Modelling tools normally split vertices at a UV seam. If the line appears, split the vertices along the floor line.

### 2.5 Snap and pad

Snapping aligns the tile with the composition texel grid. Overlapping projectors then sample identical texels.

Padding has three purposes. The pad must be large enough for all of them:

1. Cover the quantisation of the low resolution UV render. This needs a few texels.
2. Give bilinear filtering its neighbours at the tile edge. This needs one or two texels.
3. Supply neighbours to neighbourhood filters and gathers. Such a filter reads texels up to its reach `R` away. In the outermost `R` texels of the tile, those neighbours are missing and the filter clamps. Its output there is wrong. A pad of at least `R` beyond the projector coverage moves that wrong band into texels that the projector never displays. No crop is necessary: the wall material samples only wall points inside the unpadded bounding box.

For purpose 3, the pad must be at least the sum of the reaches of every neighbourhood filter and gather in the chain. Each filter consumes its own reach of valid neighbours from its input. A blur with radius 10 followed by a sharpen with radius 8 needs 18. A pixelate with 32 texel blocks needs 32. A chain without such filters needs only a few texels. A constant such as 64 or 128 covers most chains. The cost is a larger tile texture (section 2.6).

The pad is clamped at the composition edge on each side independently. A tile that touches texel 0 does not extend to the left. A tile that touches `CompositionResolution` does not extend to the right. The pad has no purpose beyond the composition: no filter needs neighbours there, and every wall UV is inside the composition. Do not move the clamped amount to the opposite side. An edge tile is one pad narrower than an interior tile. This is correct. The alignment step below may still extend the tile beyond the edge. That serves a different purpose and is allowed.

Section 2.3 gives `minTexel` and `maxTexel` with the rounding applied. Convert them to integers, then:

```
// pad in texels: at least the total filter reach of the chain, and at least 4.
// Subtract as signed integers. On an unsigned value, minTexel - pad wraps around.
minTexel = max(minTexel - pad, 0)
maxTexel = min(maxTexel + pad, CompositionResolution)

// A = alignment in texels, a power of two: 64 or 128.
// Origin down, end up. The end may pass the composition edge (see below).
minTexel = floor(minTexel / A) * A
maxTexel = ceil(maxTexel / A) * A

TileOffset = minTexel / CompositionResolution
TileSize   = (maxTexel - minTexel) / CompositionResolution
OutputSize = maxTexel - minTexel          // integer texels
```

`TileOffset` and `TileSize` are in composition UV. No further conversion is necessary.

The alignment step follows the pad. `A` is the alignment in texels. It is a power of two, 64 or 128. The step moves `minTexel` down and `maxTexel` up to a multiple of `A`. `minTexel` never goes below 0, because the origin is not negative. `maxTexel` is not clamped. At the right and bottom composition edge it may lie beyond `CompositionResolution`. Those texels have composition UV above 1. A procedural source computes values there. The wall material never samples them, because every wall UV is inside the unit square. An edge tile is therefore up to `A - 1` texels larger per axis, and `TileOffset + TileSize` may exceed 1 for it. This is correct. `CompositionResolution` can be any size.

The alignment exists for two kinds of filters. The first kind reads a mip level (the blur passes, `Edge` at a higher radius, `HeightShadows` pass 1). A mip level L of the tile texture averages blocks of 2^L texels, and these blocks start at texel 0 of the tile. Two machines whose tile origins differ by a number that is not a multiple of 2^L group different composition texels into their blocks. The same composition texel is then averaged with different neighbours on each machine. The tile size matters too: a texture whose size is not a multiple of 2^L is halved with an uneven filter, so its whole mip level differs from a cleanly halved one. The second kind uses screen space derivatives (`fwidth` in `Threshold` with antialiasing, the AieKick mode of `Edge`). The GPU pairs pixels in 2x2 quads that also start at the tile origin. Two tiles whose origins differ by an odd number of texels pair different neighbours. With origins and sizes that are multiples of `A`, every machine builds the same blocks and the same quads up to level Lmax = log2(A), and both kinds of filter give identical results. Levels above Lmax still differ.

The pad does not help against either effect. It only supplies neighbours at the tile edge. The block grid and the quad grid stay anchored at the tile origin.

The alignment is optional. Without it, only these two kinds of filter differ between machines, and the difference is bounded. For a mip filter the error is at most the block size of the highest level read, 2^L texels. The blur at `Strength 0.25` on a 16384 texel composition reads level 3.5, so the blocks are 8 to 16 texels, and the mismatch is a soft offset of a few texels in the blurred content. On a sharp edge inside a blurred image it looks like a faint double edge in the blend zone. On smooth content it is invisible. At a higher strength the blocks grow, but the content is also smoother, so the difference stays subtle until the top levels, where each machine averages a different tile and the result is a brightness difference. For a derivative filter the difference is one pixel wide along antialiased edges. All other filters, the sources, and the per pixel chain stay identical with or without the alignment. The shader code is the same in both cases, so the alignment can be added later when the compliance test (section 3.4.5) shows a seam in a blurred area. Without the alignment, edge tiles end at the composition edge and the tile computation has no rounding step.

### 2.6 Tile texture resolution

`ScreenTile` remaps UV. It does not set the output resolution of the source node. Set the output resolution of the source node to `OutputSize`. Downstream filters and mixers take their size from their input. Leave them at default.

This keeps the texel density of the whole composition. Adjacent and overlapping projectors sample the composition at the same resolution. One texel of any tile is exactly one texel of the composition.

`CompositionResolution` sets the texel density on the wall. Choose it so that the texel density on the wall is equal to or higher than the pixel density of the projectors on the wall. Otherwise the projector magnifies texels. With a uniform scale unroll (constant texels per metre along the strip), one value gives uniform density everywhere.

Padding increases the cost. An interior tile grows from `w · h` to `(w + 2·pad)(h + 2·pad)`. The whole 2D chain runs on the larger texture. A 3000x2000 tile with a 512 pad becomes 4024x3024, about 2.0 times the texels. A 1500x1000 tile with the same pad becomes 2524x2024, about 3.4 times. Small tiles pay more for the same pad. Set the pad to the actual reach of the chain when GPU headroom is limited.

## 3 The 2D shader chain

### 3.1 Shared shaders

Two shaders are shared by all tile aware shaders. Both files are in the `Common/` folder of the CMiX repository. vvvv gamma compiles shaders in the project folders and in referenced folders and reloads them on save.

`TileParams` holds the three tile pins and the conversions. It overrides no method. It can be at any position in an inheritance list. It never changes the meaning of `TexCoord`.

```hlsl
// TileParams.sdsl
// Pins and conversions for all tile aware shaders.
// This shader overrides no method. It can be at any position in an inheritance list.
shader TileParams
{
    // The tile rectangle in composition UV (0..1 across the whole composition).
    float2 TileOffset = float2(0, 0);
    float2 TileSize   = float2(1, 1);

    // The size of the whole composition in pixels.
    // The default is a placeholder only. Always set the real value; texel unit
    // math in TileToCompositionTexel and in tile aware filters depends on it.
    float2 CompositionResolution = float2(1, 1);

    // Tile UV to composition UV.
    float2 TileToCompositionUV(float2 tileUV)
    {
        return TileOffset + tileUV * TileSize;
    }

    // Composition UV to tile UV.
    float2 CompositionToTileUV(float2 compositionUV)
    {
        return (compositionUV - TileOffset) / TileSize;
    }

    // Tile UV to a position in composition texels.
    float2 TileToCompositionTexel(float2 tileUV)
    {
        return TileToCompositionUV(tileUV) * CompositionResolution;
    }

    // Composition texel index of the tile pixel at tileUV. Integer math, the same bits on every machine.
    int2 CompositionTexelIndex(float2 tileUV, float2 viewSize)
    {
        int2 pixel = int2(floor(tileUV * viewSize));
        int2 tileOrigin = int2(round(TileOffset * CompositionResolution));
        return tileOrigin + pixel;
    }
};
```

`viewSize` is the vvvv `ViewSize` of the filter, the size of the tile texture. The tile origin is a whole texel because section 2.5 snaps it.

`ScreenTile` is the vertex stage remap for sources.

```hlsl
// ScreenTile.sdsl
// Remaps the fullscreen quad UV so that this machine renders one tile of the
// composition. Add it to TextureFX SOURCE shaders only. List it last.
shader ScreenTile : ImageEffectShader, TileParams
{
    stage override void VSMain()
    {
        // Sets streams.ShadingPosition (the fullscreen quad).
        // Does not change streams.TexCoord.
        base.VSMain();

        // TexCoord is a stream. The written value goes to the pixel stage.
        streams.TexCoord = TileToCompositionUV(streams.TexCoord);
    }
};
```

With `TileOffset=(0,0)` and `TileSize=(1,1)`, the two UV conversions are the identity, so the `ScreenTile` remap changes nothing. `CompositionResolution` has no such default. Its value `(1,1)` is a placeholder. `TileToCompositionTexel` multiplies by it, so every texel unit calculation, in a source or in a tile aware filter, is wrong until it is set to the real composition size. A tile aware shader reproduces the unmodified shader only when all three pins are set: the two UV pins at their identity values and `CompositionResolution` at the real size, with the output resolution equal to it (verification item 1).

`ScreenTile` inherits `ImageEffectShader` for three reasons. They follow from the Stride base shader chain:

```
ImageEffectShader : SpriteBase
SpriteBase        : ShaderBase, Texturing
ShaderBase        : ShaderBaseStream   // declares  stage void VSMain() {}
Texturing                              // declares  stage stream float2 TexCoord : TEXCOORD0;
```

* `override` needs a `VSMain` in the inheritance chain, and `base.VSMain()` needs something to call. `ShaderBase` declares the empty `VSMain`. Without a base that declares it, the compiler reports `E0214: There is no method [...VSMain...] to override`.
* `SpriteBase` overrides `VSMain` and sets `streams.ShadingPosition`, the fullscreen quad. `ImageEffectShader` is above `SpriteBase` in the chain, so `base.VSMain()` reaches that quad positioning. `SpriteBase.VSMain` does not change `streams.TexCoord`.
* `Texturing` declares `TexCoord` as a vertex input stream. A value written to a stream in the vertex stage goes to the pixel stage, interpolated. The source body reads it without change.

The mixin listed last is the entry point. `ScreenTile` must therefore be last in the inheritance list of a source. `stage override void VSMain()` is the form that `SpriteBase` uses.

`TileParams` is separate from `ScreenTile` for one reason: filters need the pins and the conversions, but their `TexCoord` must stay tile UV. The input of a filter is the tile texture and must be sampled in tile UV. `FilterBase` also samples `tex0col` at `TexCoord` before `Filter()` runs. A filter that inherits `ScreenTile` gets a wrong `tex0col` and samples its input at the wrong place.

### 3.2 Sources

Add `, ScreenTile` at the end of the inheritance list. A source that reads `ViewSize` for content replaces it with `CompositionResolution` (rule G1). The source body reads `streams.TexCoord`, which is now composition UV. `TileParams` is included, so `CompositionResolution` and the conversions are available for texel unit math.

```hlsl
// Before:
shader MySomething_TextureFX : TextureFX { ... };

// After:
shader MySomething_TextureFX : TextureFX, ScreenTile { ... };
```

A gather effect (kaleidoscope, swirl, warp, radial blur) maps a destination coordinate to a source coordinate. With a procedural source, the shader evaluates the source function at the gathered coordinate. No texture and no network are necessary. The source coordinate can be anywhere on the composition.

```hlsl
// MyWarp_TextureFX.sdsl
// Gather on a procedural source. Fully tiled, no texture, no network.
shader MyWarp_TextureFX : TextureFX, ScreenTile
{
    stage override float4 Shading()
    {
        // ScreenTile has set streams.TexCoord to the composition UV of this pixel.
        float2 dstUV = streams.TexCoord;

        // The gather maps the destination to a source coordinate on the composition.
        float2 srcUV = SomeGatherFunction(dstUV);

        // Evaluate the procedural source at that coordinate.
        return EvaluateProceduralSource(srcUV);
    }
};
```

This requires that the source is a pure function of a composition coordinate.

### 3.3 Filters and mixers

Per pixel filters (colour grade, LUT, tone map, blend) need no change. They sample their input at tile UV, which maps 1:1 onto the tile. Do not add `ScreenTile` to a filter.

Neighbourhood filters (blur, sharpen, edge detect) need no code change if their kernel is in texel units. They need a sufficient pad (section 2.5).

Position dependent filters and gathers (pixelate, halftone, vignette, tiling, warps) inherit `TileParams` and do their position math in composition space.

Mixers need no change. Every input must be the same tile: identical `TileOffset` and `TileSize`, so identical size. A mixer samples all inputs at the same tile UV.

Pins follow one policy. A pin that gives a position or a size as a fraction of the texture is defined as a fraction of the composition. The shader converts it with `TileSize` when it samples. At the identity tile, the output equals the original. A preview of the whole composition at a lower resolution with the same aspect keeps the same pin values. Pins that are already texel counts stay texel counts. README.md lists the reach values of each shader.

The rules for each category are in section 3.4.

### 3.4 Shader compliance guideline

This section is self contained. Use it to check a TextureFX shader for this setup and to modify it if necessary. It assumes that `TileParams` and `ScreenTile` from section 3.1 exist.

#### 3.4.1 Basis

All rules follow from rule 1 and rule 2 in section 1.4:

* Tile UV covers only the tile. `streams.TexCoord`, `ViewSize`, and `Texture0TexelSize` describe the tile texture.
* Identical composition texels must give identical output on every machine. A shader can look correct on every machine alone and still fail in the blend zone.

Coordinate spaces in a shader:

| Space | Range | Meaning | Access |
|-------|-------|---------|--------|
| Tile UV | 0..1 | Position in the tile texture | `streams.TexCoord` in a filter |
| Composition UV | 0..1 | Position on the composition | `streams.TexCoord` in a source with `ScreenTile`; `TileToCompositionUV(uv)` in a filter |
| Composition texel | 0..`CompositionResolution` | Position on the composition in texels | `TileToCompositionTexel(uv)` |
| Tile texel | 0..`ViewSize` | Position in the tile texture in texels | `uv * ViewSize` |

One tile texel is one composition texel (section 1.4). A distance `k * Texture0TexelSize` is `k` composition texels in every tile. A distance given as a fraction of the tile texture is a different physical size in every tile.

A pin that gives a position or a size as a fraction of the texture is defined as a fraction of the composition. The shader converts it with `TileSize` when it samples. A pin in tile UV cannot have one consistent value across machines. At the identity tile, the output equals the original. A preview of the whole composition at a lower resolution with the same aspect keeps the same pin values. Pins that are already texel counts stay texel counts. README.md lists the reach values of each shader.

#### 3.4.2 Classification

Answer these questions. A shader can match several. Apply every matching rule.

| Question | Category | Rule |
|----------|----------|------|
| Does it generate content without an input texture? | Source | G1 |
| Does the output at a pixel depend only on the input at that pixel? | Per pixel | G2 |
| Does it read neighbouring texels at fixed offsets (blur, sharpen, edge detect, dilate)? | Neighbourhood | G3 |
| Does it compute anything from the pixel position: a grid, tiling, a centre, a distance, a gradient, noise seeded by position, an aspect correction? | Position dependent | G4 |
| Does it sample the input at a location that it computes (warp, displacement, kaleidoscope, radial blur, zoom blur, chromatic offset)? | Gather | G5 |
| Does it read a previous frame or accumulate over time? | Temporal | G6 |
| Does it compute a statistic over the whole texture (average colour, histogram, minimum, maximum, auto exposure)? | Global statistic | G7 |
| Does it take two or more input textures? | Mixer | G8 |
| Does it use random numbers or time? | Any | G9 |

#### 3.4.3 Patterns to search for

Each pattern below is a place where rule 1 or rule 2 is likely broken. A match is not automatically a failure. The rule for the category says what to do.

| Pattern | Reason | Rule |
|---------|--------|------|
| `ViewSize`, `ViewportSize`, `Texture0Size`, an output resolution pin | This is the tile size. The only correct uses are a texel step, `1 / ViewSize`, which equals `Texture0TexelSize`, and the `viewSize` argument of `CompositionTexelIndex`. Any other use bakes the tile size into the result. | G3, G4 |
| `ViewSize.x / ViewSize.y` or any aspect ratio | This is the tile aspect. Use the composition aspect, `CompositionResolution.x / CompositionResolution.y`. | G4 |
| `Texture0TexelSize` multiplied by a fraction; a UV fraction used as a distance (`0.01`, `Radius * 0.5`) | A fraction of the tile texture is a different distance on every machine. Use `k * Texture0TexelSize` with `k` in texels, or define the fraction as a fraction of the composition and divide the offset by `TileSize`. | G3, G5 |
| `0.5`, `float2(0.5, 0.5)`, or a "Center" pin used as a position | This is the tile centre. Use a position in composition UV. | G4 |
| `floor`, `ceil`, `round`, `frac`, `fmod`, `step`, `%` applied to `TexCoord` or to `TexCoord * n` | This snaps to a grid at the tile origin. Anchor grids at the composition origin. | G4 |
| `length`, `distance`, `atan2` applied to `TexCoord - something` | This is a distance or angle from a tile relative point. Compute it in composition space. | G4 |
| A hash, noise, or random function with `TexCoord`, a pixel position, or `ViewSize` as input | The seed is tile relative. Seed from the composition texel position. | G4, G9 |
| `Sample` or `Load` at a coordinate other than `TexCoord` | This is a gather. Its reach sets the pad. Convert the coordinate to tile UV before sampling. | G5 |
| `frac(TexCoord * n)`, or `Wrap` or `Repeat` address mode, used to tile the input | This tiles the tile texture. Every machine shows a different tiling. Tile in composition space. | G4 |
| A previous frame texture, a feedback input, an accumulation buffer | Temporal. Each machine has only its own history. | G6 |
| The top mip level sampled for an average, a histogram buffer, a min or max pass | A statistic over the tile. It differs per machine. | G7 |
| A frame counter, a local clock, `Time` from a source other than the shared parameter | Unsynchronised time differs per machine. | G9 |
| A pin that means "a position in the texture" or "a size relative to the texture" | Ambiguous between tile UV and composition UV. Define it as composition UV and convert. | G4 |
| `Lod(ViewSize)`, or `log2(ViewSize)` used as a mip level | The mip level depends on the tile size. Use `CompositionResolution`. | G3 |
| `Wrap`, `Repeat`, or `Mirror` address mode on a sampler for a tile input texture | The sampler wraps at the tile edge. Use Clamp or Border. | G5 |
| `fwidth`, `ddx`, `ddy` | The derivative quads start at the tile origin. The alignment of section 2.5 is necessary. | G3 |
| A `compose` pin (ShaderFX graph) that reads `TexCoord` or samples a texture | The same rules apply to that graph. | All |
| `Texture1` or another second input texture | If it is a tile, it must be the same tile as `Texture0`. If it is a shared static texture (glyph atlas, pattern), sample it at any coordinate. | G8 |

#### 3.4.4 Rules and recipes

**G1 Sources.** Inherit `ScreenTile`, listed last. Compute the colour from `streams.TexCoord` (composition UV after the remap), shared parameters, and shared time. Do not use `ViewSize` for content. For a distance in texels, use `CompositionResolution`.

Check: with `TileOffset=(0,0)` and `TileSize=(1,1)`, the output must be identical to the original. With a sub rectangle, the output must be that part of the original at 1:1 scale.

**G2 Per pixel filters.** No change, if section 3.4.3 finds no pattern. The only failure is a hidden position dependence, for example a vignette inside a colour grade or a dither pattern. If section 3.4.3 finds one, the shader is also G4.

**G3 Neighbourhood filters.** Express the kernel reach in texels: `k * Texture0TexelSize` with `k` a texel count. The pad (section 2.5) must be at least the reach. For several neighbourhood filters or gathers in sequence, the pad must be at least the sum of their reaches. A kernel in texels needs no inheritance change.

Recipe: if a radius pin is a fraction of the texture, keep the pin and define it as a fraction of the composition (section 3.4.1). Inherit `TileParams`. The offset in tile UV is `offset / TileSize`. The reach is `fraction * CompositionResolution` texels.

Mip sampling. Take the LOD from `CompositionResolution`, never from `ViewSize`. With the alignment of section 2.5, the result is identical on every machine up to level Lmax. Above Lmax it differs per machine. At the highest levels the value is the tile average, which is a global statistic (G7). A blur with `lod = Strength * log2(max(CompositionResolution))` is exact up to `Strength = Lmax / log2(max(CompositionResolution))`.

Check: run the compliance test (section 3.4.5). Then set the pad to zero. A seam must appear at the tile edge. Restore the pad. The seam must disappear. If it does not, the pad is smaller than the total reach.

**G4 Position dependent filters.** Inherit `TileParams`. Do every position calculation in composition space. Start from the integer composition texel of the pixel, `CompositionTexelIndex(streams.TexCoord, ViewSize)`. Take centres and positions from pins in composition UV. Take the aspect from `CompositionResolution`. Keep `streams.TexCoord` in tile UV for sampling the input. If the calculation produces a sample location, convert it back with `CompositionToTileUV` before sampling. The shader is then also G5. Do not inherit `ScreenTile` in a filter.

Recipe, common:

```hlsl
// Composition texel centre of this pixel. Integer origin plus integer pixel index, then 0.5.
float2 ctex = float2(CompositionTexelIndex(streams.TexCoord, ViewSize)) + 0.5;
float2 cuv  = ctex / CompositionResolution;
// Grid or hash math from ctex or cuv.
// Sample location back to tile UV.
Texture0.SampleLevel(Sampler0, CompositionToTileUV(sampleCuv), 0);
```

Every grid, centre, distance and hash input starts from `ctex` or `cuv`. The integer path gives the same bits on every machine. At the identity tile, `ctex` equals `TexCoord * ViewSize` and `cuv` equals `TexCoord`, so the output equals the original.

Recipe, pixelate. The original has two errors: the block size is `FactorXY * 0.5 * ViewSize`, a fraction of the tile, and `floor(vp / sz)` snaps to a grid at the tile origin.

```hlsl
// Before
shader Pixelate_TextureFX : FilterBase
{
    float2 FactorXY = 1.0;
    float4 Filter(float4 tex0col)
    {
        float2 uv = streams.TexCoord;
        float2 R  = ViewSize;
        float2 vp = uv * R;
        float2 sz = min(max(0.5/R, (FactorXY * 0.5) * R), R);
        return Texture0.Sample(Sampler0, floor(vp/sz) * sz/R + .5/R);
    }
};

// After
shader Pixelate_TextureFX : FilterBase, TileParams
{
    // A fraction of the COMPOSITION. The same pin values as the original.
    float2 FactorXY = 1.0;

    float4 Filter(float4 tex0col)
    {
        // TexCoord is tile UV. TileParams does not override VSMain.
        // Composition texel centre of this pixel, from the integer path.
        float2 R    = CompositionResolution;
        float2 ctex = float2(CompositionTexelIndex(streams.TexCoord, ViewSize)) + 0.5;

        // Block size in composition texels.
        float2 sz = min(max(0.5/R, (FactorXY * 0.5) * R), R);

        // Snap to a grid at the COMPOSITION origin. Sample the centre of the
        // first texel of the block.
        float2 sampleCuv = floor(ctex/sz) * sz/R + .5/R;

        // Back to tile UV before sampling the tile texture.
        return Texture0.SampleLevel(Sampler0, CompositionToTileUV(sampleCuv), 0);
    }
};
```

The pin keeps its meaning and its values. Only the reference changes from the tile to the composition. This shader also gathers. It samples up to one block away. The pad must be at least the block size, `FactorXY * 0.5 * CompositionResolution` texels.

Recipe, vignette. The original has three errors: the centre is the tile centre, the aspect is the tile aspect, and the radius is a fraction of the tile. The shader samples only at `TexCoord` through `tex0col`. It needs `TileParams` but no extra pad.

```hlsl
// Before
float2 uv     = streams.TexCoord;
float2 aspect = float2(ViewSize.x / ViewSize.y, 1);
float  d      = length((uv - 0.5) * aspect);
return tex0col * smoothstep(Radius, Radius - Softness, d);

// After (the shader inherits TileParams; Center, Radius, Softness are in composition UV)
float2 ctex   = float2(CompositionTexelIndex(streams.TexCoord, ViewSize)) + 0.5;
float2 cuv    = ctex / CompositionResolution;
float2 aspect = float2(CompositionResolution.x / CompositionResolution.y, 1);
float  d      = length((cuv - Center) * aspect);
return tex0col * smoothstep(Radius, Radius - Softness, d);
```

Check: with two overlapping tiles, the pattern (grid lines, vignette falloff, tiles) must continue across the seam without offset and without a change of scale.

An alternative for one fixed grid size exists: align tile origins to multiples of the grid size in section 2.5 instead of rewriting the shader. This holds only for one fixed grid size. It couples the tile layout to one effect parameter. It is not the general solution.

**G5 Gathers.** Inherit `TileParams`. Compute the source location in composition space. Convert it to tile UV with `CompositionToTileUV`. Then sample. The pad must be at least the reach, which is the maximum distance between a pixel and the location it samples. Sum the reaches of all neighbourhood filters and gathers in the chain.

If the reach is bounded (a displacement map with a known maximum, a radial blur with a known length), the maximum is the pad. If the reach is unbounded (a full composition kaleidoscope or swirl), the gather cannot run on the tile texture. The data is on other machines. Such a gather is possible only on a procedural source (section 3.2).

Recipe: `cuv = TileToCompositionUV(TexCoord)`. Compute `src` from `cuv` in composition UV or composition texels as before. Then `Sample(Sampler0, CompositionToTileUV(src))`. Offsets inside the gather math follow G3 (texel units) and G4 (composition space centres). Fraction pins stay fractions of the composition. The offset in tile UV is `offset / TileSize`. The reach is `fraction * CompositionResolution` texels.

Check: as G4, plus the pad test from G3.

**G6 Temporal.** Not supported. Each machine has only the history of its own tile. Content that crosses a tile edge over time does not match. Support requires an exchange of a border strip of the previous frame between neighbouring machines each frame. That is outside this document. Report the shader as non compliant. Do not modify it.

**G7 Global statistics.** Not supported. A statistic over the tile differs per machine. Auto levels, auto exposure, average colour keying, and histogram effects give different results in the blend zone. Two possible solutions: compute the statistic over a low resolution render of the whole composition on every machine (any machine can render any part at low resolution because the content is procedural), or compute it once and distribute it as a shared parameter. Report the shader as non compliant.

**G8 Mixers.** Every input must be the same tile: identical `TileOffset` and `TileSize`. A mixer samples every input at the same tile UV. Inputs from different tiles misalign. This holds automatically when all inputs come from the chain of the same machine. If the mixer has per pixel or position dependent logic of its own, classify that logic separately.

The same rule applies to any filter with a second tile input, for example `Texture1` as a displacement map, a mask, or a normal map. That input must be the same tile as `Texture0`. A shared static texture is a different case. It is not a tile. It is the same texture on every machine, for example a glyph atlas or a pattern. The shader can sample it at any coordinate. No rule applies to it.

**G9 Randomness and time.** Every random or noise value must be a function of the composition texel position (or composition UV) and a shared seed. It must not depend on the tile pixel position, `ViewSize`, the machine identity, or an unshared seed. Time must come from the shared parameter. It must not come from a local frame counter or a local clock.

Recipe: in a filter, replace `hash(TexCoord)` or `hash(pixel)` with `hash(CompositionTexelIndex(streams.TexCoord, ViewSize))`. Do not seed a hash with a float composition position such as `TileToCompositionTexel(TexCoord)`. A float composition position differs in its last bits between machines. A hash turns that small difference into a different number. In a source, `hash(TexCoord)` is already correct, with one limit. A source receives `TexCoord` interpolated across a per machine quad. A hash of a cell index in a source can then differ for a pixel that lies on a cell edge. This is rare and one pixel wide. Replace a local time input with the shared time pin.

#### 3.4.5 Compliance test

Two machines, or two instances on one machine with different `TileOffset` and `TileSize`, compute tiles that overlap. Convert both tiles to composition coordinates. Crop the overlap rectangle from each. Subtract the two crops. A compliant shader gives zero difference across the whole overlap.

Non zero results have recognisable signatures:

* A band at the edges of the overlap: insufficient pad (G3, G5).
* A uniform offset or a change of scale: a tile relative position (G4).
* A noise like difference: per machine randomness or time (G9).

Run the test with the production pad. Repeat with the pad at zero. This confirms that the pad makes the neighbourhood filters and gathers correct.

#### 3.4.6 Quick reference

| Category | Inherit | Positions in | Pad | Supported |
|----------|---------|--------------|-----|-----------|
| Source | `ScreenTile`, last | Composition, through `TexCoord` | No | Yes |
| Per pixel | Nothing | Tile | No | Yes |
| Neighbourhood | Nothing; `TileParams` for a fraction pin | Tile, offsets in texels | At least the reach, summed along the chain | Yes |
| Mip sampling | `TileParams` | LOD from `CompositionResolution` | The alignment of section 2.5, plus the reach | Yes, exact up to Lmax |
| Position dependent | `TileParams` | Composition, through conversions | No, unless it also gathers | Yes |
| Gather, bounded reach | `TileParams` | Composition, converted to tile UV to sample | At least the reach, summed along the chain | Yes |
| Gather, unbounded reach | `ScreenTile`, as a source | Composition | No | Only as a procedural source |
| Temporal | None | None | None | No, needs border exchange |
| Global statistic | None | None | None | No, needs a low resolution global computation or a shared value |
| Mixer | Nothing | Tile | No | Yes, all inputs the same tile |
| Shared static texture | Nothing | Any coordinate | No | Yes |


## 4 Verification

1. **Defaults.** Set `CompositionResolution` to the composition size, `TileOffset=(0,0)`, `TileSize=(1,1)`, and the output resolution of the node to `CompositionResolution`. The tile is then the whole composition at 1:1. The output must be identical to the unmodified shader rendered at that resolution. This applies to sources with `ScreenTile` and to filters that inherit `TileParams`. A reduced `CompositionResolution` is acceptable for this test if the real size does not fit in one texture; the identity holds for any value as long as the output resolution equals it. With `CompositionResolution` left at its placeholder `(1,1)`, every texel unit calculation is wrong and the comparison fails for the wrong reason. Two exceptions exist. A shader whose hash seed changed from a float position to the texel index (the HeightShadows passes) gives a different noise pattern. A shader that changed a Wrap sampler to Clamp (Displace, Halftone) differs in a band along the composition border.
2. **UV pass.** Render the UV pass for one projector. Inspect the B channel. Walls are dark (B = 0). The floor and the background are blue (B = 1). The wall pixels form one connected cluster inside the strip. Two clusters at opposite ends mean that the projector straddles the unroll seam. A thin dark line along the floor edge means a shared vertex on the wall to floor boundary (section 2.4).
3. **Reduction bounds.** Check the four values from section 2.3 against two references. A projector whose frustum covers the entire strip must return bounds that span the whole strip in texels. For any projector, convert the bounds back to UV, draw that rectangle over the UV pass render, and confirm that it encloses the dark cluster tightly. Wrong results have signatures: all zeros means integers read back as float; values near one billion mean float patterns read back as uint; bounds equal to their initial values mean the reset or the dispatch did not run; a rectangle smaller than the cluster means the dispatch count or the thread group size left pixels unvisited.
4. **Tile placement.** Use an asymmetric test composition with readable coordinates or labelled features. Render the projector view with the computed tile. The labels on the wall must match the composition positions of those wall points.
5. **Inverse remap.** Render the same projector view twice: once with a reduced resolution test composition applied as a full texture, once through the tile path. The two must match on the wall.
6. **Border colour.** With a loud border colour, none appears on any wall inside the frustum. The floor shows the border colour. Border colour on a wall means that the tile is too small. Increase the pad or recompute the tile.
7. **Overlap zone, per pixel chain.** Two adjacent projectors each compute their own tile. In the blend zone the content from both must be identical. A mismatch means per machine randomness, unsynchronised time, or a non deterministic node (G9).
8. **Overlap zone, filters active.** Repeat item 7 with every neighbourhood, position dependent, and gather filter enabled. The two tiles must be aligned as section 2.5 specifies. Run the compliance test (section 3.4.5). Then set the pad to zero. A seam must appear at the tile edges. Restore the pad. The seam must disappear.
9. **TileParams compile.** `ScreenTile` compiles with `TileParams` inherited. A filter that inherits `TileParams` can call the conversions from `Filter()`. This confirms that plain methods in a mixin are callable from both stages.
10. **Mip alignment.** Repeat item 8 for the mip sampling shaders with tile origins that are not multiples of `A`. A mismatch must appear. Align the tile origins as section 2.5 specifies. The mismatch must disappear.

## 5 Limits

The method covers procedural sources, procedural gathers, per pixel filters, neighbourhood filters and bounded gathers with a sufficient pad, position dependent filters built on `TileParams`, and mixers. These cases are outside it:

* **Unroll seam.** A projector that straddles the corner where the two ends of the strip meet needs two tiles. Place the unroll seam where no projector straddles it (section 2.4). The pad is clamped at the composition edge, so a neighbourhood filter cannot reach across that corner either. This matters only if the procedural content is continuous across the unroll seam.
* **Unbounded gathers on textures** (full composition kaleidoscope, swirl) need data from other machines. They are supported only as procedural sources (G5).
* **Temporal effects** (previous frame sampling, accumulation) read real pixel data, which is not procedural. Continuity across machines needs an exchange of a border strip between neighbouring machines each frame (G6). The 10G network makes that practical. The design is outside this document.
* **Global statistics** (auto levels, auto exposure, histogram effects) differ per tile (G7). They are supported only through a low resolution computation over the whole composition or a shared value.
* **Mip levels above Lmax** differ per machine. The alignment of section 2.5 makes only the levels up to Lmax = log2(A) identical.
* **`Kaleidoscope` and `Transform`** are unbounded gathers on a texture. They are not compliant. They are not part of the CMiX repository.
* **`TransformTexture_CMiX_ShaderFX`** applies a matrix to the coordinate and samples a tile. Only a translation smaller than the pad is compliant; its reach is the translation in composition UV times `CompositionResolution`. A rotation or a scale reads positions that other machines render, so it is not compliant. For a rotation or a scale of the content, use the `Transform` pin of a source: a source computes any composition position on demand. The node is kept in the repository without a change.
* **`Texturize_CMiX`** samples its input near the texture origin for every pixel. It is compliant only when that input is a shared static texture.
* **Hashed content in a source** can differ by one pixel on a cell edge (G9).

## 6 Implementation status

Confirmed by test:

* The `ScreenTile` mechanism: a mixin that inherits `ImageEffectShader`, overrides `VSMain`, and remaps `streams.TexCoord`. Confirmed under the earlier name `ScreenRegion`.
* The mixin order. The mixin listed last is the entry point. `base.VSMain()` reaches the quad positioning.

Implemented, not yet tested in vvvv:

* `TileParams` with `CompositionTexelIndex`, and `ScreenTile`, in `Common/`. Verification item 9 is still open: `ScreenTile` compiles with `TileParams` inherited, and the plain methods in `TileParams` are callable from the vertex stage and from the pixel stage.
* The eight tile aware sources: `BubbleNoise_CMiX`, `Checkerboard_CMiX`, `Gradient_CMiX`, `Gradient_Mesh_CMiX`, `MyNoise_CMiX`, `Noise_CMiX`, `Voronoi_Border_CMiX`, `Voronoi_Dots_CMiX`.
* The tile aware filters: `BlurPassBase_CMiX`, `BlurPass1_CMiX`, `BlurPass2_CMiX`, `BlurPass3_CMiX`, `Edge_CMiX`, `Dither_CMiX`, `Displace_CMiX`, `ShiftRGB_CMiX`, `Tiles_CMiX`, `LEDPanel_CMiX`, `ASCII_CMiX`, `Halftone_CMiX`, `HeightShadows_Pass0_CMiX`, `HeightShadows_Pass1_CMiX`. `Tiles_CMiX` is the repository equivalent of the pixelate example in G4.
* The 2^L alignment rule of section 2.5, with the tile end not clamped at the composition edge. It is documented. The patch side in the tile computation is not yet implemented.

Not yet tested:

* The reduction bounds against a known reference (verification item 3).
* The inverse remap and the border handling in the wall material (verification items 5 and 6).
* The overlap consistency between adjacent projectors (verification items 7 and 8).

