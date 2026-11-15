# CMiX colour: candidate principles (2026-11-14)

Discovery notes, not a decision. Numbers: OKLCH, WCAG 2 ratio, OKLab dE x100 (under 8 is hard to tell apart at a glance, under 2.5 is one colour), Machado 2009 simulation at full severity. Fills are `ColorMix.Over` (sRGB lerp) over Grey100 #0F0F12, as in `CategoryPalette.cs`. Reference: `docs/ui-kit/colour-audit-2026-11-14.md`.

## 0. Three facts that every candidate must live with

1. **Seven hues at one lightness are never safe for colour-blind users.** 7 kinds at L .74 C .13: min pair deutan dE 1.1 (Camera/Texture), protan 2.4. Quieter (L .84 C .06): deutan 0.5. No placement of 7 hues on one ring fixes this. So in every candidate the icon is the message and the kind colour is a confirmation. Or the kinds drop to 4 families and get two lightness levels.
2. **Depth by chroma does not work.** A chroma ladder C .14 / .10 / .06 at one hue: dE 4.0 per step at full colour, 0.9 at the rest fill. Nobody sees it. A lightness ladder .82 / .72 / .62: dE 10 per step at full colour, 6.3 on the chosen row, 2.4 on the rest fill. A hue wheel of 6 at 60 degrees: 17.6 per step, 4.0 to 4.7 on the rest fill. So the rest fill never tells depth or kind; the chosen row, the badge, the frame and the icon do.
3. **A free hue slider and anchored status hues can not be made safe by colour alone.** Worst case over all rotations, identity versus the three status hues, with CVD: A 0.3, B 0.6, C 1.5 to 6.4. Only one setup survives for normal vision at every rotation: identity pale and quiet (L .82 C .07) against status at mid lightness and full chroma (L .74 C .16): worst 11.9, and 15.4 at L .86 C .06. So the status code must also be a shape and a place (a dot or a word in the status strip, never a row fill), and status hues must stay anchored. Status that rotates is not an option in a signage system: at 60 degrees "broken" becomes yellow (#D6A20A) and "ok" becomes cyan (#00C0D7). A user who learned red = broken reads the room wrong.

Status hues also collapse among themselves at one lightness: ok vs danger deutan dE 1.2. A status lightness ladder (danger L .60, ok L .72, warn L .86) gives deutan 11.2 and protan 7.3, but costs rotation safety against pale identity (warn at L .86 meets pale rows). Pick one: ladder plus shapes, or flat plus shapes. Shapes are mandatory in both.

## 1. Candidate A: "Hue says what it is, lightness says how deep it sits"

**Rule a user learns:** Every kind of node has one colour. A composable gets lighter and darker as it nests; a child is never as light as its parent.

**Axes:** hue = identity (kind). lightness = depth (a ladder of 3, cycling). chroma = fixed. fill strength = state (rest .18, hover .32, chosen .55, badge .60, frame 1.0).

**Fixes from the audit:** 13 identity hues become 8 (7 kinds + one composable hue). No wheel twin of a kind (today 1 vs Camera dE 3.0, 4 vs Modifier 3.8). The kind hues leave the status sectors (Camera 35 to 45, Texture 135 to 120, composable 3 and 1 no longer green and red). Depth dimming by LevelFactor goes away: depth is a step the eye sees (dE 10 at full colour, 6.3 on the chosen row).

**Costs:** The composable wheel of 6 becomes a ladder of 3: depth 0 and depth 3 are the same colour (dE 0.0). "A container never shares a colour with the container around it" still holds; "six depths, six colours" does not. 7 kinds on one ring stay colour-blind unsafe among themselves (deutan 1.1); the icon carries the kind. Lightness is used twice (depth and state), so a chosen depth-2 row (#B167AB at .55 = L .43) is darker than a chosen depth-0 row (L .56): the strength ladder must be read per hue.

**Generation (inputs: base hue H0, hue list of 7 offsets, C, L_kind, L ladder):**
```
kind k:        OKLCH(L_kind=.74, C=.13, H0 + offset_k)
composable d:  OKLCH(L_depth[d mod 3] = (.82, .72, .62), C=.13, H_composable)
tone t:        Over(colour, Grey100, t)   t in {.09 .18 .32 .55 .60 1.0}
```
Offsets today: Composable 330, Entity 355, Camera 45, Light 100, Texture 120, Filter 185, Modifier 225, Modulator 290 (all 20 degrees or more from status 25 / 70 / 150).

**Worked example (status flat L .74 C .16: ok #51C672, warn #EA9602, danger #FF7E77):**

| Token | OKLCH | Hex | White on .55 | Rest text on .18 | Badge .60 vs bg |
|---|---|---|---|---|---|
| Entity | .74 .13 355 | #EA86AF | 6.22 | 8.79 | 3.45 |
| Camera | .74 .13 45 | #EE8F63 | 6.08 | 8.71 | 3.54 |
| Light | .74 .13 100 | #BEAC3D | 5.88 | 8.62 | 3.67 |
| Texture | .74 .13 120 | #A0B751 | 5.77 | 8.56 | 3.75 |
| Filter | .74 .13 185 | #04C5B5 | 5.71 | 8.60 | 3.78 |
| Modifier | .74 .13 225 | #2EBBE8 | 5.76 | 8.59 | 3.71 |
| Modulator | .74 .13 290 | #AA9CF7 | 5.98 | 8.66 | 3.58 |
| Depth 0, 3 | .82 .13 330 | #F4A5EC | 4.95 | 8.17 | 4.36 |
| Depth 1, 4 | .72 .13 330 | #D285CB | 6.47 | 8.87 | 3.31 |
| Depth 2, 5 | .62 .13 330 | #B167AB | 8.43 | 9.48 | 2.50 |

Tone ladder of hue 330 L .74: .09 #211A23, .18 #332635, .32 #50374F, .55 #7E547C, .60 #885A85, 1.0 #D98CD2 (OKLab L .23 .29 .37 .51 .53 .74). All white-on-chosen pass AA. Badge .60 of depth 2 is 2.50:1 against the background: a dark step needs a .70 badge or a white icon only.

Separation: kinds among themselves normal 4.6 (Light/Texture), protan 2.4, deutan 1.1. Identity vs status normal 5.8 (Camera/danger), protan 3.6, deutan 2.0. Adjacent depths 10.2.

**Slider:** rotation of H0 is unsafe at every angle (worst vs status 0.3 with CVD; only 22 of 360 degrees keep every kind hue 15 degrees from a status hue). Safe to expose: the lightness ladder (dark/light), C down to .09. Must stay fixed: the hue offsets, the status hues. Hue rotation only as a snap to the 22 safe degrees, or not at all.

**Inspector and other views:** the header band is the kind colour at .55 (same as the chosen row). Breadcrumb chips of ancestors are the composable hue at the depth lightness, .60. A 2 px left bar of the kind at .60. Modulator dot = Modulator hue. Lists and tabs: the chosen row of a grey list stays blue (AccentSecondary) because grey lists have no kind; the two "chosen" codes of the audit remain.

## 2. Candidate B: "Only containers have colour; a thing is an icon"

**Rule a user learns:** Colour tells you which box you are in. The six colours of the wheel are the six depths of nesting. Everything inside a box is grey, and its icon says what it is.

**Axes:** hue = container depth (the wheel of 6). lightness = alternates .84 / .72 / .60 along the wheel so neighbours differ in two ways. chroma = fixed. fill strength = state. Kinds: no hue; icon in TreeTextRest, badge Grey600 with a white icon.

**Fixes from the audit:** 13 hues become 6. All kind-vs-wheel twins gone. All kind-vs-status collisions gone (Camera/danger 3.9, Texture/ok 6.5). The grayscale test for kind becomes moot: kind is an icon. Modulator assign dot keeps a violet only if the violet is a status-like token "driven", not a kind.

**Costs:** The operator loses preattentive kind targeting by colour ("all the lights are yellow"). The venue manager loses nothing. The Inspector header has the colour of the parent composable, not of the thing: "you are inside the magenta layer" instead of "you are editing a light". With the lightness cycle, depth 0 and 3 share lightness but differ by 180 degrees of hue; under deutan that pair still falls to 4.6 and 0/4 under protan to 5.0.

**Generation (inputs: base hue H0, step 60, C, L cycle):**
```
depth d:  OKLCH(L_cycle[d mod 3] = (.84, .72, .60), C=.13, H0 + 60 d)
kinds:    Grey600 badge, TreeTextRest icon, white icon on chosen
```

**Worked example (H0 = 345):**

| Token | OKLCH | Hex | White on .55 | Rest text on .18 | Badge .60 vs bg |
|---|---|---|---|---|---|
| Depth 0 | .84 .11 345 | #FEADDB | 4.73 | 8.06 | 4.61 |
| Depth 1 | .72 .13 45 | #E7885D | 6.40 | 8.83 | 3.35 |
| Depth 2 | .60 .13 105 | #8C8305 | 8.40 | 9.48 | 2.50 |
| Depth 3 | .84 .13 165 | #6CE5B5 | 4.37 | 7.81 | 4.97 |
| Depth 4 | .72 .13 225 | #22B5E1 | 6.11 | 8.74 | 3.54 |
| Depth 5 | .60 .13 285 | #7973CA | 8.66 | 9.55 | 2.42 |
| Kinds (all 7) | grey | Grey600 #4C4C4F badge, icon TreeTextRest | n/a | 11.6 | n/a |

Depth 3 white on .55 is 4.37, under AA by a hair: use L .82 for the top step. Separation: adjacent depths 17.0 normal, protan 10.4 or more, deutan 11.9 or more; non-adjacent worst deutan 4.6 (0/3), protan 5.0 (0/4). Depth vs status: normal 11.1 (Depth 3/ok), protan 9.4, deutan 3.1 (Depth 1/ok). With a two-level cycle (.80 / .68) and H0 345: Depth 0 #F99BD1, 1 #D97C50, 2 #CAC256, 3 #2EB184, 4 #49CFFC, 5 #918BE5; adjacent normal 17.6 or more, deutan 11.9 or more.

**Slider:** rotation of H0: for normal vision, 84 of 180 tested angles keep every depth 8 dE or more from the status ladder (24 of 180 against flat status). With CVD, no angle is safe (worst 0.6). So: rotation as a snap to safe angles, status anchored, status always a shape. Safe to expose: L cycle (dark/light), C. Fixed: the 60 degree step, the status hues.

**Inspector and other views:** header band = the parent composable colour at .55 with the kind icon in white; "Layer 2 > Light 1" reads as magenta band, light icon. Breadcrumb chips = wheel colours. Panels inside the Inspector are grey levels, as today. Tree group rows (Entities, Modifiers) are grey. This is the closest to the transit map: the line is a colour, the station is a dot.

## 3. Candidate C: "Identity whispers, status shouts"

**Rule a user learns:** Pale colours tell you what and where. Strong colours tell you if something is wrong. A strong colour always comes with a shape or a word.

**Axes:** hue = name (kind or depth, from A or B). lightness and chroma together = voice: identity lives in the pale band (L .82 to .86, C .06 to .08), status in the loud band (L .68 to .74, C .16 to .17). fill strength = state. This is the only candidate where each perceptual axis has one job: hue = identity, L and C = who is speaking, strength = state.

**Fixes from the audit:** every identity-vs-status collision, at every rotation, for normal vision: worst 11.9 (L .82 C .07) or 15.4 (L .86 C .06) against flat status L .74 C .16. The accent blue and the kind hues can never be mistaken for a status. The chosen row stays readable (white on .55: 4.65 to 7.08).

**Costs:** Quiet identity separates less: 7 kinds at C .07 are 2.1 to 5 dE apart (dE per hue step is about 200 x C x sin(step/2): 45 degrees at C .07 gives 5.4, at C .13 gives 10). So C works with 4 kind families at 90 degrees (9.9 at C .07) or with B (6 depths at 60 degrees, 7.3 at C .08; 12.8 measured with the L cycle). It does not work with 7 kinds. Pale identity also lifts the rest fills a little (OKLab L .30 against .29), no cost. A status lightness ladder (.60 / .72 / .86) breaks the rotation guarantee (worst 4.8 to 5.5): status stays flat and gets its shapes.

**Generation (inputs: base hue H0, hue scheme from A or B, two bands):**
```
identity:  OKLCH(L_pale = .82 (depth cycle .88/.82/.76 or .84/.72 for B), C_pale = .07, hue from scheme)
status:    OKLCH(L_loud = .74, C_loud = .16, h fixed: ok 150, warn 70, danger 25)
interaction (on, focus): OKLCH(.60, .10, 255)   a third band, mid L, mid C, one hue
tone t:    Over(colour, Grey100, t)
```

**Worked example (A hues in the pale band, status flat loud):**

| Token | OKLCH | Hex | White on .55 | Rest text on .18 | Badge .60 vs bg |
|---|---|---|---|---|---|
| Entity | .78 .08 355 | #E1A2BA | 5.46 | 8.42 | 3.95 |
| Camera | .78 .08 46 | #E3A88D | 5.38 | 8.33 | 4.02 |
| Light | .78 .08 100 | #C3B97D | 5.24 | 8.27 | 4.11 |
| Texture | .78 .08 120 | #B0BF85 | 5.22 | 8.25 | 4.16 |
| Filter | .78 .08 185 | #79C8BE | 5.16 | 8.29 | 4.19 |
| Modifier | .78 .08 224 | #7DC3DE | 5.20 | 8.30 | 4.16 |
| Modulator | .78 .08 290 | #B6AFE8 | 5.36 | 8.31 | 4.03 |
| Depth 0, 3 | .84 .08 330 | #E9B8E4 | 4.65 | 8.01 | 4.67 |
| Depth 1, 4 | .76 .08 331 | #CF9FC9 | 5.71 | 8.48 | 3.76 |
| Depth 2, 5 | .68 .08 330 | #B586B0 | 7.08 | 9.07 | 3.01 |
| ok | .68 .17 150 | #2EB45C | 6.59 | on bg 7.11 | dot |
| warn | .68 .15 70 | #D08506 | 7.08 | on bg 6.41 | dot |
| danger | .68 .17 25 | #EF6661 | 7.35 | on bg 6.14 | dot |

Same with the B wheel in the pale band (L .82 / .72, C .08, H0 345): Depth 0 #EBB0D0, 1 #CF957B, 2 #CBC88B, 3 #73B598, 4 #8AD0EB, 5 #9F9ED5; white on .55 4.70 to 6.28; adjacent normal 12.8 or more; vs loud status normal 9.1, protan 6.1, deutan 4.1.

Tone ladder of pale 330: .09 #211C23, .18 #332A34, .32 #4E3F4F, .55 #7C627B, .60 #866984, 1.0 #D5A5D0. Pale .55 against loud .55 of the same hue: dE 3.9, so a chosen row can never look like a status fill only because status is never a fill.

**Slider:** H0 rotation of the identity band is safe for normal vision at every angle (status anchored). CVD safety of status comes from shapes, not from the hue gap (worst with CVD 1.5 to 6.4). Safe to expose: H0 (free), L_pale within .78 to .88, dark/light. Fixed: C_pale under .09, the loud band, the status hues, the shapes.

**Inspector and other views:** identical to A or B for the header, chips and bar, all in the pale band. Every status cue in every view is a loud dot, triangle, ring or word: sync dots, missing-asset triangle, Delete text, the BeatSteps current step (today Brushes.OrangeRed, a hardcode) becomes a loud "live" token with its own shape. The interaction blue is a third, mid band: on and focus only.

## 4. Candidate D: "Colour is for status only"

**Rule a user learns:** The screen is grey. Green, orange and red mean ok, attention and broken. Blue means on or focused. Everything else is a shape and a name.

**Axes:** hue = status and interaction only (4 hues). lightness = surface and depth (the Grey ramp; a nested composable is one Grey step lighter, as the panels are). chroma = 0 for identity. fill strength = state (Grey500 hover, AccentSecondary chosen).

**Fixes from the audit:** every finding about identity hues disappears. Hue budget 4. Nothing competes with the walls. Perfect grayscale and CVD behaviour by construction.

**Costs:** Nearly all of what the owner has built: kind rows, wheel, chips, Inspector band. The operator targets by icon shape and indent only. The gamer feel (team colours) is gone. Depth by Grey steps gives dE about 4 per step (Grey200 to Grey300: OKLab L .249 to .290) and runs out after 4 steps.

**Generation:** none beyond the existing ramp. The chosen row is white on AccentSecondary (5.93:1).

**Slider:** nothing to rotate; dark/light only.

## 5. Which axis carries what

| Candidate | hue | lightness | chroma | strength (mix over bg) |
|---|---|---|---|---|
| A | kind (8) | depth (3-cycle) | fixed | state |
| B | depth (6) | guard against CVD (cycle) | fixed | state |
| C | name (from A or B) | voice: pale identity, mid status | voice: quiet identity, loud status | state |
| D | status + on (4) | surface and depth | zero for identity | state |

The simplest hierarchy with one meaning per axis is C: hue = identity, lightness + chroma band = who speaks (identity, interaction, status), strength = state. A and B give lightness two jobs (depth and state). D gives hue one job and lightness two (surface and depth).

## 6. Comparison

| | A hue = kind, L = depth | B containers only | C whisper / shout | D grey + status |
|---|---|---|---|---|
| Hues that carry meaning | 8 + 3 status + 1 accent | 6 + 3 + 1 | same as A or B | 3 + 1 |
| One sentence to learn | yes | yes, the clearest | yes, plus one of A or B | yes |
| Kind-vs-wheel twins | gone | gone | gone | gone |
| Identity-vs-status, normal | 5.8 worst | 11.1 | 11.9 to 15.4 at any rotation | n/a |
| Identity-vs-status, CVD | 2.0 | 3.1 | 1.5 to 6.4 (shape needed) | n/a |
| Kind separation, CVD | 1.1 (icon is the cue) | n/a (icon is the cue) | worse than A | n/a |
| Depth separation | 10 per step, 3 steps | 17 per step, 6 steps | 7 to 13 | 4 per step |
| White on chosen .55 | 4.95 to 8.43 | 4.37 to 8.66 | 4.65 to 7.08 | 5.93 |
| Free hue slider | no (22 of 360 safe) | snap only (84 of 180 vs status ladder) | yes, for normal vision | n/a |
| Safe parameters | L ladder, C | L cycle, C | H0, L_pale, dark/light | dark/light |
| Must stay fixed | hue offsets, status | 60 step, status | C bands, status, shapes | all |
| What the owner gives up | wheel of 6 | kind colours | kind count (4 families) or kind by colour | the colour system |
| Serves the manager | ok | best | best | good |
| Serves the operator | best (kind colours) | icon only | good with 4 families | icon only |

## 7. My ranking, as the signage designer

1. **C on top of B.** The transit rule: a line is a colour (the container), a station is a dot (the icon), and an alarm is red with a bell (shape plus loud colour). It is the only pair where each axis has one job, the only one where a hue slider is safe for normal vision at every angle, and it drops the hue count to 6 + 3 + 1. Status gets shapes in every candidate anyway, so the CVD cost of C is not an extra cost. What it gives up is kind colours. The operator keeps kind by icon, and the audit already says the icon is the primary cue because 7 hues never pass CVD.
2. **C on top of A with 4 families.** Keeps kind colour for the operator (Scene, Texture, Modifier, Modulator at 90 degrees, dE 9.9 in the pale band) and keeps the slider safe. It costs the wheel of 6 (depth becomes a 3-step ladder) and asks the owner to merge 7 kinds into 4 colours.
3. **B alone.** Clear and few hues, but identity and status share the loud band, so the slider must snap and a chosen green-ish depth-3 row sits 11.1 from "ok". Good if the slider idea is dropped.
4. **A alone.** Closest to today and the operator likes it, but it keeps 8 identity hues in the status band: worst 5.8 normal, 2.0 deutan, and no slider.
5. **D.** The floor. Right for a hospital corridor, too quiet for a game-like control room. Keep it as the fallback if the hue system does not earn its place in the venue.

Two rules hold in every candidate and should be written first: status is anchored and always a shape; a coloured fill that carries text is at most .55 with white text.

Scripts: `C:\Users\ras\AppData\Local\Temp\claude\dd489d4371d66ae0\1df228b0-60aa-4b0a-95aa-0d88d3c5adce\scratchpad\calc\Program.cs`, `Program2.cs`, `Program3.cs` (dotnet, throwaway).
