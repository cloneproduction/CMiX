# CMiX colour audit (2026-11-14)

Scope: `CMiX.UI/Themes/Tokens/Colors.axaml` (43 Color keys), `CMiX.UI/Themes/Tokens/Brushes.axaml` (46 Brush keys), `CMiX.Studio.Avalonia/Themes/CategoryPalette.cs`, the tree converters, `Themes/Styles/ProjectTree.axaml`, `Views/Tree/*`, `Views/Inspector/PanelInspector.axaml`, `Views/Modulation/*`, `PopupCard`/`ToolTipCard`, `Themes/Styles/StatusIndicators.axaml`, and a grep of every `*.axaml` and `*.cs`. All numbers are computed (WCAG 2 ratio, OKLab dE x100, Machado 2009 CVD simulation at full severity). The mix formula is the one of the code: `ColorMix.Over` = sRGB lerp over `TreeBackground` #0F0F12.

## Summary

The grey system is sound: one ramp, state brushes named by meaning, text passes AA on every surface it is used on. The hue system is the problem. Thirteen identity hues (7 kinds + 6 wheel steps) are built at one lightness (OKLab L 0.72 to 0.74) and one chroma, so they separate by hue alone: in grayscale they are one colour, and under deuteranopia 5 pairs fall under dE 2.5 (the same colour). The identity hues also reuse the three status hues (green, orange, red), so a depth-3 composition row is the same green as "connected" (dE 2.0) and a camera row is the same red as "disconnected" (dE 3.9). One accent blue carries four meanings (on, focus, grab mode, in sync). The brief says the chosen row is at 0.85; the code has 0.55 (`CategoryPalette.cs:56`), and 0.85 fails WCAG AA for white text on every kind.

## 1. Inventory: Color keys

Name test: does the key say the meaning (M), the colour (C), or the place (P)?

| Key | Hex | Meaning (what it tells the user) | Used by | Name | Issue |
|---|---|---|---|---|---|
| Grey100 | #0F0F12 | window surface, deepest | Grey100Brush (33), checkerboard, empty toggle slot | C | primitive, fine |
| Grey200 | #212124 | panel level 1, hint strip, inspector settings band (`PanelInspector.axaml:94,103`) | Level1Brush, HintStripBackgroundBrush | C | fine |
| Grey300 | #2B2B2E | panel level 2, tree toolbar (`ProjectTreeView.axaml:43`), empty group row (`ProjectTree.axaml:244`) | Level2Brush, Grey300Brush (20) | C | fine |
| Grey400 | #363639 | panel level 3, empty group hover (`:247`) | Level3Brush | C | fine |
| Grey500 | #414144 | pressed control, hovered row, empty group chosen (`:250`) | StatePressedBrush, RowHoverBrush | C | one grey, three meanings (pressed, row hover, chosen empty group) |
| Grey600 | #4C4C4F | idle control, key cap frame, "+n" badge (`BadgesKindCount.axaml:59`) | StateIdleBrush, KeyCapBorderBrush | C | fine |
| Grey700 | #606063 | hovered control, dead group text and icon (`KindIconBrushConverter.cs:34`) | StateHoverBrush | C | same hex as ModulatorUnassigned |
| Grey800 | #808083 | chevron (`ProjectTree.axaml:262`), cursor frame without focus, "pending" sync dot | CursorFrameInactiveBrush, StatusPendingBrush | C | a status meaning on a primitive |
| Grey900 | #B0B1B4 | shortcut text on a hovered row | ShortcutTextHoverBrush | C | fine |
| TreeBackground | #0F0F12 | tree surface, dim cover, mix base of the palette | TreeBackgroundBrush, `CategoryPalette.cs:85` | M | duplicate of Grey100 |
| AccentPrimary | #5680C2 | on + pointer (Button.axaml:125,128,233; Menu.axaml:71), focus line (TextBox.axaml:102,194; TextBoxSearchTree.axaml:15; PanelFocus.axaml:12), text selection (TextBox:20,118; NumericInput:22), slider track (Slider:118), grab frame (`ProjectTreeView.axaml:203`), in-sync dot (`StatusIndicators.axaml:11`) | AccentBrush (31) | C | collision: 4 meanings |
| AccentSecondary | #4d6490 | on, at rest (ToggleButton:13,177; RadioButton:42; Slider:7; ListBox:172; ContextMenu:107) | AccentInactiveBrush (18) | C | fine |
| AccentHover | #4B74B6 | on + pointer, rows and toggles (ComboBox:137,147; ListBox:169; RadioButton:46; ToggleButton:17,180) | StateOnHoverBrush (14) | C | synonym of AccentPrimary in Button.axaml:233 (two "on + pointer" blues) |
| Active | #5680C2 | none | 0 uses | M | dead, duplicate of AccentPrimary |
| AccentHighlight | #91ACD7 | none | AccentHighlightBrush, 0 uses | C | dead |
| ModulatorUnassigned | #606063 | no modulator on this control | assign dot via `ModulatorStateToBrushConverter.cs:25` | M | same hex as Grey700; 3.05:1 on Grey100, 2.56:1 on Grey200 |
| ModulatorInternal | #AD8DFD | an internal modulator drives this | assign dot | M | identical to CategoryModulator (by design, documented) |
| ModulatorInternalHighlight | #CEC0FF | the same, modulator hovered | assign dot | M | fine |
| ModulatorExternal | #DF8605 | an external (Redis) modulator drives this | assign dot | M | dE 7.2 to PopupCardWarning: reads as a warning |
| ModulatorExternalHighlight | #F1B77E | the same, hovered | assign dot | M | fine |
| CategoryComposable | #DC89D5 | "the composable hue by name" | 0 code uses (`CategoryPalette.ColorKey` always indexes) | M | dead, duplicate of CategoryComposable0 |
| CategoryComposable0..5 | see 4.2 | composable depth 0..5 | CategoryPalette | M | see 4.2 and 4.3 |
| CategoryEntity | #F275A0 | kind: entity | CategoryPalette | M | dE 8.3 to Composable0, 7.0 to PopupCardDanger |
| CategoryCamera | #F87B5C | kind: camera | CategoryPalette | M | dE 3.0 to Composable1, 3.9 to PopupCardDanger |
| CategoryLight | #E3CC36 | kind: light | CategoryPalette | M | the only hue at L 0.84; white on its badge 3.99:1 |
| CategoryTexture | #73BB43 | kind: texture | CategoryPalette | M | dE 5.4 to Composable3, 6.5 to StatusSuccess |
| CategoryFilter | #2DC5A6 | kind: filter | CategoryPalette | M | dE 7.7 to Composable4 |
| CategoryModifier | #22B5E1 | kind: modifier | CategoryPalette | M | dE 3.8 to Composable4 |
| CategoryModulator | #AD8DFD | kind: modulator | CategoryPalette | M | dE 7.4 to Composable5 |
| WarningMuted | #B4685C | a conflict mark (`SettingsShortcut.axaml:49`) | WarningMutedBrush, 1 use | M | comment says "refused drop frame": the tree uses a grey cover for that |
| PopupCardBackground | #18181B | popup surface (tooltip, menu, flyout, Add panel) | PopupBackgroundBrush (12) | P | fine; a surface step between Grey100 and Grey200 that is not in the ramp |
| PopupCardWarning | #E2A355 | needs attention: disabled reason, missing asset (`BadgesKindCount.axaml:88`), unsynced dot | PopupCardWarningBrush, StatusUnsyncedBrush | P | name says place, meaning is status |
| PopupCardDanger | #E97871 | bad: Delete row, disconnected dot | PopupCardDangerBrush, StatusDisconnectedBrush | P | name says place, meaning is status |
| StatusSuccess | #6FC082 | good: connected dot | StatusConnectedBrush | M | dE 2.0 to CategoryComposable3 |
| TreeTextRest | #C9C9CE | row text at rest | TreeTextRestBrush (8) | M | fine |
| Text | #E0E0E0 | text | ForegroundBrush (82) | M | fine |
| ActiveText | #FFFFFF | text and icon on a coloured fill; cursor frame with focus; drop marks | ActiveForegroundBrush (33), CursorFrameBrush | M | one white, three cues (text on colour, cursor, drop); the shapes differ, accepted |
| ShortcutText | #85858C | quiet text: shortcut, menu title, hint strip | ShortcutTextBrush, TextMutedBrush, HintStripTextBrush | P | 4.38:1 on Grey200 (hint strip) is under AA |

## 2. Inventory: Brush keys

| Key | Colour | Meaning | Used at | Issue |
|---|---|---|---|---|
| Shadow | Black 0.2 | a frame line | `ComposableListBox.axaml:9` only | name says shadow, use is a border; 1 use |
| AccentBrush | AccentPrimary | see AccentPrimary | 31 | collision |
| AccentInactiveBrush | AccentSecondary | on, at rest | 18 | fine |
| AccentHighlightBrush | AccentHighlight | none | 0 | dead |
| ModulatorUnassignedBrush, ModulatorInternalBrush, ModulatorInternalHighlightBrush, ModulatorExternalBrush, ModulatorExternalHighlightBrush | Modulator* | assign dot | by name from the converter | fine |
| PanelBackground | Black 0.3 | panel tint | 14 | translucent black, outside the ramp |
| PanelBorderBackground | Black 0.4 | expander header | `Expander.axaml:44` | same value as ListBoxBackground |
| ListBoxBackground | Black 0.4 | list surface | ListBox.axaml:30,44,84,121; TypeWriter.axaml:23 | duplicate of PanelBorderBackground |
| PanelBackgroundLevel1Brush | Grey200 | panel level 1, tree stripes (`ProjectTreeView.axaml:60`) | 8 | fine |
| PanelBackgroundLevel2Brush | Grey300 | panel level 2 | ExpanderIndentedDepth.axaml:13; TogglesComposable.axaml:19 | fine |
| PanelBackgroundLevel3Brush | Grey400 | panel level 3 | ExpanderIndentedDepth.axaml:16 | fine |
| PopupBackgroundBrush | PopupCardBackground | popup surface | 12 | fine |
| PopupCardWarningBrush | PopupCardWarning | attention | 5 | name |
| PopupCardDangerBrush | PopupCardDanger | bad, destructive | 6 | name |
| TreeBackgroundBrush | TreeBackground | tree surface, dim cover (`ProjectTree.axaml:129`) | 4 | alias of Grey100 |
| Grey100Brush .. Grey900Brush | Grey* | raw | 33, 11, 20, 9, 12, 11, 8, 2, 2 | 108 raw grey uses in views bypass the semantic layer |
| TreeTextRestBrush | TreeTextRest | row text at rest | 8 | fine |
| StateIdleBrush / StateHoverBrush / StatePressedBrush | Grey600 / Grey700 / Grey500 | control states | 21 / 19 / 15 | fine |
| RowHoverBrush | Grey500 | row under the pointer | 15 | same grey as pressed |
| StateOnHoverBrush | AccentHover | on + pointer | 14 | fine |
| WarningMutedBrush | WarningMuted | conflict | 1 | 1 use |
| CursorFrameBrush | ActiveText | cursor with focus; drop frame; drop line | CursorRow.axaml:11,15; ProjectTree.axaml:154,185; ProjectTreeView.axaml:106 | one brush, two cues (cursor, drop); the shapes differ, accepted |
| CursorFrameInactiveBrush | Grey800 | cursor without focus | 6 | fine |
| ForegroundBrush / ActiveForegroundBrush | Text / ActiveText | text / text on colour | 82 / 33 | fine |
| TextMutedBrush | ShortcutText | quiet text | ContextMenu.axaml:130; ToolTip.axaml:65 | synonym of ShortcutTextBrush and HintStripTextBrush: 3 brushes, 1 colour, 1 meaning |
| CheckerboardBrush | Grey300 / Grey100 | transparency | `Button.axaml:152` | fine |
| ShortcutTextBrush / ShortcutTextHoverBrush | ShortcutText / Grey900 | shortcut text | 3 / 1 | fine |
| KeyCapBorderBrush | Grey600 | key cap frame | 1 | fine |
| HintStripBackgroundBrush / HintStripTextBrush | Grey200 / ShortcutText | hint strip | 1 / 1 | text 4.38:1, under AA |

Hardcoded values outside the tokens: `ProjectTreeView.axaml:194` BoxShadow `#E6000000`; `Brushes.axaml:5,18,19,20` literal `Black`; `CMiX.UI/Converters/CurrentStepConverter.cs:16` fallback `Brushes.OrangeRed` (the current step of BeatSteps); `CMiX.UI/Behaviors/ListBoxReorder.cs:291` `Brushes.Gray`; `PanelInspector.axaml:76` placeholder at Opacity 0.5 (opacity used as a text colour). Core model defaults (`#FFFFFFFF` and so on) are content, not UI.

## 3. Meaning map

| Meaning | Colour(s) today | Problem |
|---|---|---|
| on / selected control | AccentSecondary at rest, AccentHover under the pointer, AccentPrimary under the pointer in Button.axaml:233 | synonym: two "on + pointer" blues |
| focus (text box, panel, search) | AccentPrimary | collision with "on", "grab mode", "in sync" |
| grab mode frame | AccentPrimary (`ProjectTreeView.axaml:203`) | collision |
| in sync (dot) | AccentPrimary | collision: a status in the interaction hue |
| pending (dot) | Grey800 | same grey as the chevron and the unfocused cursor; dE to AccentPrimary 10.8, to Danger under protanopia 5.7 |
| ok / connected | StatusSuccess | dE 2.0 to CategoryComposable3, 6.5 to CategoryTexture |
| attention / unsynced / missing asset / disabled reason | PopupCardWarning | dE 5.3 to CategoryComposable2, 7.2 to ModulatorExternal |
| bad / disconnected / Delete | PopupCardDanger | dE 3.9 to CategoryCamera, 4.1 to CategoryComposable1 |
| conflict | WarningMuted | dE 10.9 to Danger; a fourth status hue for one use |
| what I picked (chosen row) | kind hue at 0.55 + white text | in the tree; the grey lists use AccentSecondary: two "chosen" codes in one window |
| cursor | 1 px frame, kind hue (focus) or Grey800 (no focus); white on the chosen row | fine: a shape, not a colour |
| drop target / drop line | white frame / white line | fine: a shape |
| out of play (drag, search) | Grey100 cover 0.6 / 0.3 | fine: luminance |
| hovered row | Grey500 (lists), kind hue 0.32 (tree) | two codes for one meaning; accepted as list vs tree |
| modulator source | violet (= kind Modulator), orange (= warning hue), Grey700 (= hover grey) | external reads as a warning |
| row text at rest / strong | TreeTextRest / ActiveText | fine |
| quiet text | ShortcutText through 3 brushes | synonym brushes |

## 4. Numbers

### 4.1 Luminance ladder

| Token | Hex | Y | OKLab L |
|---|---|---|---|
| Grey100 / TreeBackground | #0F0F12 | 0.0049 | 0.170 |
| PopupCardBackground | #18181B | 0.0093 | 0.210 |
| Grey200 | #212124 | 0.0154 | 0.249 |
| Grey300 | #2B2B2E | 0.0244 | 0.290 |
| Grey400 | #363639 | 0.0372 | 0.334 |
| Grey500 | #414144 | 0.0532 | 0.376 |
| Grey600 | #4C4C4F | 0.0727 | 0.418 |
| Grey700 | #606063 | 0.1175 | 0.490 |
| Grey800 | #808083 | 0.2167 | 0.601 |
| ShortcutText | #85858C | 0.2366 | 0.619 |
| Grey900 | #B0B1B4 | 0.4397 | 0.760 |
| TreeTextRest | #C9C9CE | 0.5865 | 0.837 |
| Text | #E0E0E0 | 0.7454 | 0.907 |
| ActiveText | #FFFFFF | 1.0000 | 1.000 |

Ramp steps in OKLab L: 0.079, 0.041, 0.044, 0.042, 0.041, 0.072, 0.111, 0.160. Two scales in one ramp: surfaces (200 to 600) tight at 0.04, text greys (700 to 900) wide. Usable. Grey800 and ShortcutText are 0.018 apart: a near duplicate.

### 4.2 Text contrast (WCAG)

| Text | Grey100 | Popup | Grey200 | Grey300 | Grey400 | Grey500 | Grey600 | Grey700 |
|---|---|---|---|---|---|---|---|---|
| Text | 14.50 | 13.42 | 12.17 | 10.69 | 9.12 | 7.71 | 6.48 | 4.75 |
| TreeTextRest | 11.60 | 10.74 | 9.73 | 8.56 | 7.30 | 6.17 | 5.19 | 3.80 |
| ShortcutText | 5.22 | 4.83 | **4.38** | 3.85 | 3.29 | 2.78 | 2.34 | 1.71 |
| Grey900 | 8.92 | 8.26 | 7.49 | 6.58 | 5.62 | 4.74 | 3.99 | 2.92 |
| Grey800 | 4.86 | 4.50 | 4.08 | 3.58 | 3.06 | 2.58 | 2.17 | 1.59 |

White on accent: AccentPrimary 3.99 (fails AA for text; Button.axaml:233 puts text on it), AccentHover 4.70, AccentSecondary 5.93. The AccentPrimary focus line on an idle control (Grey600) is 2.14, under the 3:1 target for a focus ring; on Grey100 it is 4.80.

Tree fills: TreeTextRest on the rest and hover fills, white on the chosen row, white icon on the kind badge:

| Kind | OKLCH | TreeTextRest on Fill .18 | on Fill .09 | on Hover .32 | white on chosen .55 (code) | white on chosen .85 (brief) | white on Kind .60 | Kind .60 vs bg |
|---|---|---|---|---|---|---|---|---|
| Composable0 | L.741 C.140 h330 | 8.71 | 10.28 | 6.39 | 6.14 | **3.23** | 5.49 | 3.49 |
| Composable1 | L.739 C.140 h30 | 8.77 | 10.28 | 6.43 | 6.19 | **3.25** | 5.51 | 3.47 |
| Composable2 | L.740 C.140 h90 | 8.64 | 10.21 | 6.26 | 5.89 | **3.09** | 5.23 | 3.66 |
| Composable3 | L.740 C.140 h150 | 8.55 | 10.22 | 6.06 | 5.67 | **2.91** | 5.03 | 3.80 |
| Composable4 | L.740 C.128 h210 | 8.64 | 10.26 | 6.18 | 5.79 | **2.96** | 5.12 | 3.73 |
| Composable5 | L.740 C.132 h270 | 8.63 | 10.19 | 6.27 | 5.93 | **3.11** | 5.26 | 3.64 |
| Entity | L.720 C.159 h0 | 8.95 | 10.42 | 6.67 | 6.59 | **3.53** | 5.92 | 3.23 |
| Camera | L.721 C.160 h35 | 8.95 | 10.37 | 6.61 | 6.54 | **3.47** | 5.82 | 3.29 |
| Light | L.840 C.160 h100 | 7.94 | 9.90 | 5.32 | 4.52 | **2.19** | 3.99 | 4.80 |
| Texture | L.720 C.170 h135 | 8.65 | 10.30 | 6.30 | 5.95 | **3.13** | 5.34 | 3.58 |
| Filter | L.740 C.130 h175 | 8.54 | 10.25 | 6.15 | 5.69 | **2.90** | 5.05 | 3.79 |
| Modifier | L.721 C.130 h225 | 8.74 | 10.30 | 6.41 | 6.11 | **3.19** | 5.40 | 3.54 |
| Modulator | L.719 C.161 h295 | 8.81 | 10.36 | 6.57 | 6.45 | **3.46** | 5.74 | 3.34 |

Rest and hover fills pass. Chosen at 0.55 passes AA for every kind (Light 4.52 is the floor). Chosen at 0.85 fails AA for every kind and fails 3:1 for Light. The kind badge at 0.60 with a white icon passes 3:1 everywhere (Light 3.99 is the floor). Fill 0.18 against the background is 1.30 to 1.46:1 and the floor 0.09 is 1.11 to 1.17:1: the depth dimming is below the step people notice (dE between the depth 0 and depth 1 fills = 2.2; depth 0 to the floor = 5.9).

### 4.3 Distinguishability (OKLab dE x100; under 12 is the same colour at a glance)

Kind hues against each other, at full colour (badge, icon, frame):

| Pair | normal | protan | deutan | tritan |
|---|---|---|---|---|
| Entity vs Filter | 29.0 | 14.2 | **1.5** | 31.2 |
| Camera vs Texture | 25.3 | 11.5 | **1.4** | 25.8 |
| Entity vs Texture | 30.4 | 18.9 | **9.8** | 25.1 |
| Camera vs Filter | 27.3 | 14.3 | **10.0** | 32.0 |
| Entity vs Camera | **9.6** | 10.2 | 8.9 | 2.2 |
| Texture vs Filter | **11.1** | 11.6 | 11.0 | 6.7 |
| Filter vs Modifier | **11.1** | 10.5 | 10.6 | 2.0 |
| Modifier vs Modulator | 17.0 | 9.0 | **4.6** | 12.5 |
| Light vs Texture | 15.6 | **8.5** | 13.2 | 18.2 |
| Entity vs Modifier | 26.8 | 12.1 | 11.5 | 30.7 |

Wheel steps against each other:

| Pair | normal | protan | deutan | tritan |
|---|---|---|---|---|
| 0 vs 4 | 23.2 | 9.0 | **1.5** | 23.3 |
| 1 vs 3 | 24.2 | 10.1 | **2.1** | 27.3 |
| 1 vs 2 | 14.0 | 10.4 | **6.0** | 8.7 |
| 0 vs 5 | 13.7 | **6.3** | 7.7 | 18.7 |
| 2 vs 3 | 13.9 | 8.3 | 7.9 | 18.9 |
| 4 vs 5 | 12.9 | 8.3 | 6.4 | 5.7 |
| 0 vs 1 | 14.0 | 14.3 | 14.1 | 7.4 |
| 3 vs 4 | 13.4 | 13.3 | 13.5 | 3.7 |

Wheel against fixed kinds (normal vision): 1 vs Camera 3.0, 4 vs Modifier 3.8, 3 vs Texture 5.4, 5 vs Modulator 7.4, 4 vs Filter 7.7, 0 vs Entity 8.3. Six of the seven kinds have a wheel twin.

Identity against status (normal vision): Composable3 vs StatusSuccess 2.0, Camera vs Danger 3.9, Composable1 vs Danger 4.1, Composable2 vs Warning 5.3, Texture vs Success 6.5, Entity vs Danger 7.0, ModulatorExternal vs Warning 7.2.

Status dots against each other (one shape, four states, colour only):

| Pair | normal | protan | deutan | tritan |
|---|---|---|---|---|
| Success vs Danger | 23.5 | 13.7 | **3.5** | 27.4 |
| Success vs Warning | 15.6 | **5.8** | **7.1** | 20.7 |
| Warning vs Danger | **11.9** | 11.6 | 7.5 | 9.0 |
| Danger vs Grey800 (pending) | 17.3 | **5.7** | 13.3 | 21.3 |
| Grey800 vs AccentPrimary (pending vs synced) | **10.8** | 10.1 | 11.1 | 9.8 |

Modulator dots: Internal vs External 28.7 / 28.8 / 28.8 / 17.6, Internal vs Unassigned 27.7 / 26.6 / 25.8 / 22.8. The pair is safe; its problem is the orange, not the separation.

Row fills at rest (0.18 over Grey100): every pair of the 13 is under dE 8, 44 pairs are under 5, and 20 pairs are under 2.5 for deuteranopes. At rest the row tint says "some kind", not "which kind".

### 4.4 Grayscale

OKLab L of the 13 hues: 0.719 to 0.741, Light 0.840. At Kind 0.60: 0.517 to 0.534, Light 0.596. At Fill 0.18: 0.280 to 0.289, Light 0.311. In grayscale twelve kinds are one grey and Light is a second grey. The system passes the grayscale test for state (rest L 0.29, chosen 0.50, badge 0.53) and fails it for kind. That is acceptable only because every row also carries the icon of its kind: the icon is the primary cue, the hue the secondary. Keep it that way in every view the system reaches.

## 5. Findings

```
[MISREAD] Connected and disconnected are one colour for deuteranopes
Where:     CMiX.Studio.Avalonia/Themes/Styles/StatusIndicators.axaml:7-11, 20-31
Problem:   Four states, one ellipse, colour only.
Numbers:   Success vs Danger dE 3.5 (deutan); Success vs Warning 5.8 (protan); Danger vs Grey800 5.7 (protan);
           Pending vs Synced 10.8 (everyone).
Who's hit: venue manager (healthy vs broken is the one thing they must see).
Fix:       Give each state a shape too: filled dot = connected, ring = pending, dot with a bar = unsynced, hollow square =
           disconnected. Keep the hues. Move the green out of the identity set (next finding).
```
```
[MISREAD] Identity hues reuse the status hues
Where:     Colors.axaml:46-47, 50-51, 53 against :65, :67, :70; StatusIndicators.axaml
Problem:   A chosen camera row (0.55 fill) and the red "disconnected" dot share one hue. When the Inspector header takes
           the kind hue, a band of red or green sits next to the sync dots.
Numbers:   Composable3 vs StatusSuccess dE 2.0; Camera vs Danger 3.9; Composable1 vs Danger 4.1; Composable2 vs Warning
           5.3; ModulatorExternal vs Warning 7.2.
Who's hit: both.
Fix:       Status owns pure green (h150), orange (h70) and red (h25); nothing else does. Keep every identity hue at least
           15 degrees from those three: Composable3 to h165, Camera to h45 at lower chroma or a new hue, Composable1 to
           h15 is still red, so take the hue budget fix below. Status also gets a shape, so a close hue can not misread.
```
```
[MAJOR] Thirteen identity hues at one lightness collapse under CVD and in grayscale
Where:     Colors.axaml:43-56; CategoryPalette.cs:54-65
Problem:   Thirteen hues on one circle at one L and one C can not be told apart by 1 in 12 men.
Numbers:   Deutan: Entity/Filter 1.5, Camera/Texture 1.4, wheel 0/4 1.5, wheel 1/3 2.1, Modifier/Modulator 4.6. Normal
           vision: Entity/Camera 9.6, Texture/Filter 11.1, Filter/Modifier 11.1; six wheel steps twin six kinds (3.0 to 8.3).
Who's hit: operator (preattentive targeting by kind fails for CVD users).
Fix:       Hue budget. Keep the wheel at 6 (nesting is a count; 6 steps is already the limit). Cut the fixed kinds from
           7 hues to 4 families by meaning: Scene (Entity, Camera, Light: things in the room), Texture (sources and
           filters: both make pixels), Modifier, Modulator. Place 10 hues, not 13, and vary L by family: Scene L .72,
           Texture L .78, Modifier L .68, Modulator L .74, so grayscale and deutan views still separate the families.
           The icon stays the primary cue in every view. If the owner keeps 7 kinds: lift Texture to L .78 and drop
           Filter to L .66, so the two deutan pairs that collapse (Entity/Filter, Camera/Texture) separate by lightness.
```
```
[MAJOR] The chosen-row strength of the brief (0.85) fails WCAG AA for white text on every kind
Where:     CategoryPalette.cs:56 (code: 0.55); the brief and the memory say 0.85
Numbers:   At 0.85: white on chosen = 2.19 (Light) to 3.53 (Entity), all under 4.5, Light under 3.0.
           At 0.55: 4.52 to 6.59, all pass.
Who's hit: both.
Fix:       Keep 0.55 for every fill that carries text (chosen row, Inspector header). Fills above 0.60 carry only a white
           icon or nothing; prefer 0.60 for badges (icons 4.0 to 5.9). Make the kit text and the constant agree.
```
```
[MAJOR] One blue, four meanings
Where:     Brushes.axaml:7; Button.axaml:125,128,233; TextBox.axaml:102,194; PanelFocus.axaml:12; TextBoxSearchTree.axaml:15;
           ProjectTreeView.axaml:203; StatusIndicators.axaml:11
Problem:   AccentPrimary is "on + pointer", the focus line, the grab-mode frame and the "in sync" dot. In the tree the
           cursor and the focus are a frame in the kind hue, so a blue focus elsewhere is a second focus code.
Numbers:   white on AccentPrimary 3.99 (text on an "on" button fails AA); focus line on Grey600 2.14 (under 3:1).
Who's hit: both.
Fix:       Split by meaning: Interaction.On (AccentSecondary rest, AccentHover pointer; stop using AccentBrush as an "on"
           fill in Button.axaml:233), Interaction.Focus (one FocusLineBrush, blue), Mode.Grab (a white frame, the tree's
           own mode colour), and Status.Synced becomes the filled dot of the ok state in StatusSuccess, not a blue.
```
```
[MAJOR] Status tokens named by place
Where:     Colors.axaml:65, 67; Brushes.axaml:30-31; StatusIndicators.axaml:7, 9
Problem:   PopupCardWarning and PopupCardDanger are the warning and danger of the whole application (sync dots, missing
           asset sign, Delete row). The name sends a developer to look for a popup colour. StatusSuccess is named right.
Fix:       Rename to StatusWarning / StatusDanger (+Brush).
```
```
[MINOR] Dead and duplicate colour keys
Where:     Colors.axaml:16 (TreeBackground = Grey100), :23 (Active = AccentPrimary, 0 uses), :25 (AccentHighlight, 0 uses),
           :43 (CategoryComposable = CategoryComposable0, 0 code uses), :32 (ModulatorUnassigned = Grey700);
           Brushes.axaml:9 (AccentHighlightBrush), :19-20 (PanelBorderBackground = ListBoxBackground = Black 0.4)
Fix:       Retire Active, AccentHighlight(+Brush), CategoryComposable. Keep TreeBackground only as a brush alias of Grey100
           and make CategoryPalette read Grey100. Merge PanelBorderBackground into ListBoxBackground.
```
```
[MINOR] Three brushes, one colour, one meaning (quiet text)
Where:     Brushes.axaml:74 (TextMutedBrush), :97 (ShortcutTextBrush), :103 (HintStripTextBrush), all ShortcutText
Fix:       One TextMutedBrush. A shortcut and a hint strip are places, not meanings.
```
```
[MINOR] Hint strip text under AA
Where:     Brushes.axaml:102-103 (Grey200 + ShortcutText)
Numbers:   4.38:1 (AA is 4.5). ShortcutTextLookTests checks PopupCardBackground only (4.83).
Fix:       Hint strip on PopupCardBackground (4.83) or text Grey900 (7.49).
```
```
[MINOR] WarningMuted: a fourth status hue for one use, with a stale comment
Where:     Colors.axaml:58-60; the only use is SettingsShortcut.axaml:49
Numbers:   dE to Danger 10.9 (same family at a glance); on Grey400 2.91 (under 3:1 for a mark).
Fix:       Use StatusWarning for the conflict mark and retire WarningMuted. The tree says "refused" with a grey cover.
```
```
[MINOR] Hardcoded values
Where:     ProjectTreeView.axaml:194 (#E6000000); Brushes.axaml:5,18-20 (Black); CurrentStepConverter.cs:16
           (Brushes.OrangeRed, the current step of BeatSteps); ListBoxReorder.cs:291 (Brushes.Gray);
           PanelInspector.axaml:76 (Opacity 0.5 as a text colour)
Fix:       Tokens: ShadowColor, OverlayBlack30 / OverlayBlack40, TextMutedBrush for the placeholder. The current step of
           BeatSteps is "live" feedback and needs its own token (chart group Feedback), not a fallback.
```
```
[MINOR] Depth dimming below perception
Where:     CategoryPalette.cs:57-58 (LevelFactor 0.8, FillFloor 0.09)
Numbers:   dE depth 0 to depth 1 = 2.2; depth 0 to the floor = 5.9; fill against bg 1.13:1 at the floor.
Fix:       Drop the dimming (the wheel already tells depth), or use two steps only (0.18 top kind, 0.11 nested) so the
           step is one the eye reads (dE about 4).
```
```
[POLISH] Raw grey brushes in views
Where:     Grey100Brush .. Grey900Brush, 108 uses outside the tokens
Fix:       Views read Surface.* and State.* brushes: Surface.Window (Grey100), Surface.Panel (Grey200), Surface.Raised and
           Surface.Toolbar (Grey300). Migrate per view touched.
```

## 6. Extension to the Inspector and other views

The tree code is: kind hue = who you are; tone strength = state; shapes (frame, line, cover) = cursor, drop, out of play. The Inspector shows one node, so it needs no state tones. It needs two facts: which kind, and inside which composable.

### 6.1 Cues

| Fact | Cue | Token / strength | Why this cue |
|---|---|---|---|
| Kind of the inspected node | Header band fill (already there, `PanelInspector.axaml:39`, FillSelected) with a white icon and a white name | kind hue at 0.55 over Grey100 (white text 4.5 to 6.6:1) | The same look as the chosen row: "the thing you picked is the thing you edit". One meaning, one look. |
| Kind, when the panel scrolls | A 2 px left bar down the whole editor | Kind 0.60 (3.2 to 4.8:1 on Grey100; 3:1 met) | A thin bar survives scrolling and does not compete with the walls. Not 0.85: a long saturated bar dazzles in a dark room. |
| Composable context (the composition, layer or scene that holds the node) | A breadcrumb strip under the header: one chip per ancestor composable, each in its wheel hue at Kind 0.60 with a white icon, the name in TreeTextRest on Grey200 | wheel step by depth, as in the tree | The wheel is a depth code: the user already learned "my composition is magenta, its layers are coral". The chips repeat exactly those tones. |
| Section of a child kind (the Modifiers group inside an entity) | Section header with a Kind 0.60 badge of that kind, text in TreeTextRest, no fill | badge only | A fill of a second kind inside a panel of a first kind reads as two selected things. Badges, like the count squares of a collapsed row, say "what is inside". |
| Modulated control | The assign dot stays violet (CategoryModulator) | unchanged | It is already the kind hue of a modulator. Only External changes (6.3). |

Do not tint the whole Inspector background with the kind: a 400 by 600 px panel at 0.09 over Grey100 is a large coloured surface in a dark room, and at 0.09 nobody reads it (1.13:1). Band plus bar is enough.

### 6.2 Strength ladder (one ladder for every view)

| Step | Strength | Meaning | Carries text? |
|---|---|---|---|
| Fill | 0.18 | this row is of this kind, at rest | yes, TreeTextRest (7.9 to 9.0:1) |
| Hover | 0.32 | the pointer is here | yes, TreeTextRest (5.3 to 6.7:1) |
| Chosen | 0.55 | what I picked; the Inspector header | yes, white (4.5 to 6.6:1) |
| Kind | 0.60 | a badge, a chip, the left bar | a white icon only (4.0 to 5.9:1) |
| Frame | 1.00 | the cursor frame, 1 px | no |

0.85 has no slot. The code constant is 0.55: keep it and correct the kit text.

### 6.3 Rules that follow

1. Status (green, orange, red) is never an identity colour. Move the identity hues that sit on them (Composable3, Camera, Composable1, Composable2) by at least 15 degrees, and give every status dot a shape.
2. The accent blue means "on" and "focus" only. The in-sync dot becomes a filled green dot; pending a ring; the grab frame white (the tree's mode colour).
3. ModulatorExternal leaves orange (the warning hue). External means "comes from outside": use the Modulator violet with a ring (hollow dot) for external and a filled dot for internal. One hue, two shapes. Unassigned stays a Grey700 dot.
4. A coloured fill that carries text is at most 0.55 and the text is white. A fill above 0.60 carries only a white icon or nothing.
5. A view never references a Category* key directly. It asks TreeToneConverter (tree, Inspector header, chips) or a new KindBrushConverter for badges and bars, so the ladder stays in one file.
6. Every kind cue sits beside the icon of the kind. Colour alone is never the message.
