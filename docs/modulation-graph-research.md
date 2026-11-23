# Modulation graph: research

Status: research, not built. Started 2026-11-22. Paused 2026-11-23.
The mockups are in `docs/ui-kit/mockups/`. Open them through the kit server and the presentation menu `docs/ui-kit/index.html`.

## 1. The goal

- One modulator can drive values in many objects: a TFX, a modifier, another modulator.
- A user builds modulators into a named chain, and uses the outputs of the chain as sources.
- The chain window is a node graph. The tree stays a clean stack.

## 2. The model

### 2.1 Sources, targets and links

- A source gives values with a name: a modulator output, a chain output, a MIDI fader, an OSC address, an audio input.
- A target receives a value: every modulatable value, and every input of a chain.
- A link joins one source output to one target. It belongs to the project pool, not to the target.
- A link has an amount (result = base + amount x signal) or a range (min to max). The inspector shows the form that suits the source.
- A chain is both: its inputs are targets, its outputs are sources.

### 2.2 Chains

- A chain has a definition (inner nodes, inner links, exposed inputs and outputs) and instances (input values, runtime state).
- Store the definition and the instance from the start. The first version makes only copies.
- Copy: a new definition with one instance. It appears in the pool with a new name, for example "Beat FX 2".
- Instance: one more instance of the same definition. A change to the inside reaches every instance. Each instance keeps its own input values.
- "Make unique" turns an instance into a copy. TFX chains on several layers use the same model.
- An input of a chain has a default in the definition. An instance has its own value. Reset goes back to the default.
- An exposed input or output has a stable ID. The name is for display only. Two outputs of one chain never share a name. A duplicate gets the next number ("Pulse 2"). A rename to a name in use is refused.

### 2.3 Where things show

| What | Where |
|---|---|
| The inside of a chain (the definition) | The chain window, a node graph |
| The values of one instance and what drives them | The inspector of the instance |
| Every link of the project | The matrix |
| A target and what drives it | The inspector of the target, a link line under the value |

The chain window shows only the definition. MIDI, OSC and the targets outside the chain never show in it.

## 3. The vvvv engine

Read from `CMiX.Engine/CMiX_ENGINE.Definitions.vl` and the patches of the parent repository.

- The lookup of a modulator is already project wide: `GetModulatorByName` finds it by ID in the engine `Repository`.
- The engine creates an engine object for each control of the repository. It never reads the modulator list of an owner. A project pool needs no change there.
- The resolve logic is in two node definitions, `ModulatableValue` and `ModulatableValueSpread`. They have 116 call sites. If their pins stay the same, the call sites stay the same.
- `ModulationResolver` has no caller.
- A modulator output writes a channel each frame. A loop between two modulators is probably a delay of one frame. Verify live.
- The update order is `UpdateOrderProcessor` in C#. All modulators share one row.
- A modulator parameter is a `GenericValue<T>`. An input needs `ModulatableValue<T>`. This change brings hand edits in each engine modulator patch, and in `CMiX.Extensions.vl` (VisitorData, LISA, tracking).
- An engine object keeps state for one use. So an instance is a linked copy in Core. The engine sees plain copies.

Recommended order of work: links in the pool with amount and range, then modulator inputs, then chains and the copy of TFX chains, then instances.

## 4. The node graph: decisions

The lead mockup is `modulation-chain-depth.html`.

### 4.1 Layout

- Grid of 24 px. Nodes snap to it. A node is a whole number of cells.
- A node has one margin of 12 px all around. The header is 24 px. A control is 20 px high. Two controls are 4 px apart.
- Every control in a node has the same height, including a ComboBox.
- Corners are 2 px (`CornerRadiusAll`).
- The pins of one side spread evenly from the top to the bottom of the node, each on the centre of a cell.
- A node has no port names. A pin shows its name and its range in a tooltip.

### 4.2 Ports

- Every port is a float: one number in, one number out. Every pin is the same round shape.
- A chain input and a chain output are small separate nodes, one for each port, placed where they are used.
- A port node is a tree item: a coloured square of 24 px with a white arrow at the outer end, then the name. Inputs and outputs are mirrored.
- A port node shows no value.
- A list beside the graph sets the order of the ports outside the chain.

### 4.3 Colour

- Colour follows depth, as the composables of the tree: the wheel `CategoryComposable0` to `5`, in the same order.
- The chain inputs are stage 0. A node is one stage after the deepest node that feeds it. Two connected stages never share a hue.
- A cable has the colour of the stage it leaves. A pin has the exact colour of its cable at the same strength.
- At rest a node has no colour: one grey surface and a grey icon. The header has no tint.
- Hover and selection follow the tree: a full outline 2 px outside the node, 70 % of the stage colour on hover and full on selection. The surface never changes.
- Cable strength: 65 % at rest, 82 % on hover, full and thicker on selection.

### 4.4 Cables

- A cable is a bezier curve. It leaves and reaches a pin flat.
- A cable has a gradient: full colour at both pins, 12 % opacity in the middle (30 % when selected).
- Cables are above the nodes. Pins are above the cables.

## 5. Options tried and rejected

| Option | Why it went out |
|---|---|
| Line styles for data types (dotted, dashed, double) | Merge at a far zoom, nobody reads them. The owner rejected them. |
| Colour for each data type | Clash with the category colours of the tree. |
| Colour for each node category, after Bitwig Grid (`modulation-chain-nodes.html`) | Did not say much. Option A. |
| Colour for each output line, as a transit map (`modulation-chain-lines.html`) | Little difference on screen. Option B. |
| Purple on the cables of a selection | Gave the modulator colour a second meaning. |
| Value badges on cables, after Patina | Removed by the owner. |
| Compact tiles with knobs, after Bitwig Grid | Too far from the CMiX controls. |
| Square routes, 45 degree routes, late turn, after TiXL | The owner chose the bezier curve with a gradient. |
| Port names on the nodes | Replaced by tooltips. |
| A surface that lights on hover and selection | Only the outline changes. |

## 6. Reviews

- Colour system agent: hue is identity, strength is state, status is a shape and an anchored hue. A graph has no indent, so the cables carry the structure. Blue stays for interaction.
- Venue manager agent: show that a chain is alive, use plain words, keep the port order the same on the canvas and in the list, lock the canvas during a show.

## 7. Open questions

1. Does the canvas open locked during a show, with an Edit switch?
2. Does the canvas keep a legend of the stages?
3. Proposed tokens: `GraphCanvas`, `GraphGrid`, `GraphPin`. Not in CMiX yet.
4. The colour audit proposes to move "external modulator" from orange to a ring and another hue. This affects the whole of CMiX.
5. What does a chain show when its audio source has no sound?
6. A dragged node rises above the others, and its own pins go under the cables until the drop.
7. How does the user see where an output goes outside the chain? The port list shows a count of uses.

## 8. Files

| File | What |
|---|---|
| `docs/ui-kit/mockups/modulation-pool.html` | The project pool and the links with an amount |
| `docs/ui-kit/mockups/modulation-links.html` | Where a chain is used, what drives a value |
| `docs/ui-kit/mockups/modulation-chain.html` | A chain with inputs and outputs: pool, inspector, assign menu |
| `docs/ui-kit/mockups/modulation-chain-nodes.html` | Graph, colour option A: node categories |
| `docs/ui-kit/mockups/modulation-chain-lines.html` | Graph, colour option B: output lines |
| `docs/ui-kit/mockups/modulation-chain-depth.html` | Graph, colour option C, the lead: depth |
| `docs/ui-kit/index.html` | The presentation menu of all mockups |
