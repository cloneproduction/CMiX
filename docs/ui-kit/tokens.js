/* Made by tools/Update-Tokens.ps1 from CMiX.UI/Themes. Do not edit. */
window.CMIX_TOKENS = {
    "groups":  [
                   {
                       "id":  "0.1",
                       "name":  "Colors",
                       "source":  "CMiX.UI/Themes/Tokens/Colors.axaml",
                       "tokens":  [
                                      {
                                          "name":  "Grey100",
                                          "type":  "Color",
                                          "raw":  "#FF0F0F12",
                                          "css":  "#0F0F12",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "Grey200",
                                          "type":  "Color",
                                          "raw":  "#FF212124",
                                          "css":  "#212124",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "Grey300",
                                          "type":  "Color",
                                          "raw":  "#FF2B2B2E",
                                          "css":  "#2B2B2E",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "Grey400",
                                          "type":  "Color",
                                          "raw":  "#FF363639",
                                          "css":  "#363639",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "Grey500",
                                          "type":  "Color",
                                          "raw":  "#FF414144",
                                          "css":  "#414144",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "Grey600",
                                          "type":  "Color",
                                          "raw":  "#FF4C4C4F",
                                          "css":  "#4C4C4F",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "Grey700",
                                          "type":  "Color",
                                          "raw":  "#FF606063",
                                          "css":  "#606063",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "Grey800",
                                          "type":  "Color",
                                          "raw":  "#FF808083",
                                          "css":  "#808083",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "Grey900",
                                          "type":  "Color",
                                          "raw":  "#FFB0B1B4",
                                          "css":  "#B0B1B4",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "TreeBackground",
                                          "type":  "Color",
                                          "raw":  "#FF0F0F12",
                                          "css":  "#0F0F12",
                                          "note":  "The color under the rows of the project tree: the window surface (Grey100). The palette of the rows mixes against it."
                                      },
                                      {
                                          "name":  "AccentPrimary",
                                          "type":  "Color",
                                          "raw":  "#FF5680C2",
                                          "css":  "#5680C2",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "AccentSecondary",
                                          "type":  "Color",
                                          "raw":  "#FF4d6490",
                                          "css":  "#4D6490",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "AccentHover",
                                          "type":  "Color",
                                          "raw":  "#FF4B74B6",
                                          "css":  "#4B74B6",
                                          "note":  "The fill of a control that is on, or of a row that is selected, under the pointer. It is between AccentSecondary and AccentPrimary: white text has a contrast of 4.70:1 on it. AccentPrimary is for lines, such as the focus line and the drop line."
                                      },
                                      {
                                          "name":  "Active",
                                          "type":  "Color",
                                          "raw":  "#FF5680C2",
                                          "css":  "#5680C2",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "AccentHighlight",
                                          "type":  "Color",
                                          "raw":  "#FF91ACD7",
                                          "css":  "#91ACD7",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "ModulatorUnassigned",
                                          "type":  "Color",
                                          "raw":  "#FF606063",
                                          "css":  "#606063",
                                          "note":  "The colors of the modulator assign button, one pair for each ModulatorCategory. The name is Modulator + category + (Highlight when the modulator is hovered). A new category needs two colors here and two brushes in Brushes.axaml. An internal modulator has the color of the modulator rows of the tree (CategoryModulator), not the accent: the accent means on or selected."
                                      },
                                      {
                                          "name":  "ModulatorInternal",
                                          "type":  "Color",
                                          "raw":  "#FFAD8DFD",
                                          "css":  "#AD8DFD",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "ModulatorInternalHighlight",
                                          "type":  "Color",
                                          "raw":  "#FFCEC0FF",
                                          "css":  "#CEC0FF",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "ModulatorExternal",
                                          "type":  "Color",
                                          "raw":  "#FFDF8605",
                                          "css":  "#DF8605",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "ModulatorExternalHighlight",
                                          "type":  "Color",
                                          "raw":  "#FFF1B77E",
                                          "css":  "#F1B77E",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "CategoryComposable",
                                          "type":  "Color",
                                          "raw":  "#FFDC89D5",
                                          "css":  "#DC89D5",
                                          "note":  "The colors of the kinds of row in the project tree, one for each TreeCategory. A composable takes a hue by its depth of nesting from a wheel of six (CategoryComposable0 to 5, 60 degrees apart), so a container and the containers in it never share a hue. The other kinds keep their own colour inside any container. CategoryComposable is the first hue of the wheel, for the places that ask for the kind by name. The tree reads them by the name Category + kind, and shows them as tones over the panel background."
                                      },
                                      {
                                          "name":  "CategoryComposable0",
                                          "type":  "Color",
                                          "raw":  "#FFDC89D5",
                                          "css":  "#DC89D5",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "CategoryComposable1",
                                          "type":  "Color",
                                          "raw":  "#FFF68675",
                                          "css":  "#F68675",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "CategoryComposable2",
                                          "type":  "Color",
                                          "raw":  "#FFCDA629",
                                          "css":  "#CDA629",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "CategoryComposable3",
                                          "type":  "Color",
                                          "raw":  "#FF62C37A",
                                          "css":  "#62C37A",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "CategoryComposable4",
                                          "type":  "Color",
                                          "raw":  "#FF00C0D7",
                                          "css":  "#00C0D7",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "CategoryComposable5",
                                          "type":  "Color",
                                          "raw":  "#FF8AA6FE",
                                          "css":  "#8AA6FE",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "CategoryEntity",
                                          "type":  "Color",
                                          "raw":  "#FFF275A0",
                                          "css":  "#F275A0",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "CategoryCamera",
                                          "type":  "Color",
                                          "raw":  "#FFF87B5C",
                                          "css":  "#F87B5C",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "CategoryLight",
                                          "type":  "Color",
                                          "raw":  "#FFE3CC36",
                                          "css":  "#E3CC36",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "CategoryTexture",
                                          "type":  "Color",
                                          "raw":  "#FF73BB43",
                                          "css":  "#73BB43",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "CategoryFilter",
                                          "type":  "Color",
                                          "raw":  "#FF2DC5A6",
                                          "css":  "#2DC5A6",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "CategoryModifier",
                                          "type":  "Color",
                                          "raw":  "#FF22B5E1",
                                          "css":  "#22B5E1",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "CategoryModulator",
                                          "type":  "Color",
                                          "raw":  "#FFAD8DFD",
                                          "css":  "#AD8DFD",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "WarningMuted",
                                          "type":  "Color",
                                          "raw":  "#FFB4685C",
                                          "css":  "#B4685C",
                                          "note":  "A muted warning tone, not a full red: the frame of a target that refuses a drop, the mark of a conflict. It is not PopupCardWarning, the line of a tooltip."
                                      },
                                      {
                                          "name":  "PopupCardBackground",
                                          "type":  "Color",
                                          "raw":  "#FF18181B",
                                          "css":  "#18181B",
                                          "note":  "The background of every popup: the card of a tooltip, the Add panel, a menu, a flyout and a drop down. It has no outline."
                                      },
                                      {
                                          "name":  "PopupCardWarning",
                                          "type":  "Color",
                                          "raw":  "#FFE2A355",
                                          "css":  "#E2A355",
                                          "note":  "The line of a tooltip that says why a control is disabled."
                                      },
                                      {
                                          "name":  "PopupCardDanger",
                                          "type":  "Color",
                                          "raw":  "#FFE97871",
                                          "css":  "#E97871",
                                          "note":  "The text of a row of a menu that takes something away, such as Delete."
                                      },
                                      {
                                          "name":  "StatusSuccess",
                                          "type":  "Color",
                                          "raw":  "#FF6FC082",
                                          "css":  "#6FC082",
                                          "note":  "A good status, such as a connection that is up. With PopupCardDanger (a bad status) and PopupCardWarning (a status that needs attention), it is one of the three status colors. It has a contrast of 3:1 or more on every panel."
                                      },
                                      {
                                          "name":  "TreeTextRest",
                                          "type":  "Color",
                                          "raw":  "#FFC9C9CE",
                                          "css":  "#C9C9CE",
                                          "note":  "The text of a row of the tree at rest: a step under Text, so the white of the cursor row and of the selected row stands out."
                                      },
                                      {
                                          "name":  "Text",
                                          "type":  "Color",
                                          "raw":  "#FFE0E0E0",
                                          "css":  "#E0E0E0",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "ActiveText",
                                          "type":  "Color",
                                          "raw":  "#FFFFFFFF",
                                          "css":  "#FFFFFF",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "ShortcutText",
                                          "type":  "Color",
                                          "raw":  "#FF85858C",
                                          "css":  "#85858C",
                                          "note":  "The text of a shortcut in a tooltip, a menu and a key cap. It has a contrast of 4.74:1 on PopupCardBackground (WCAG AA)."
                                      }
                                  ]
                   },
                   {
                       "id":  "0.2",
                       "name":  "Brushes",
                       "source":  "CMiX.UI/Themes/Tokens/Brushes.axaml",
                       "tokens":  [
                                      {
                                          "name":  "Shadow",
                                          "type":  "SolidColorBrush",
                                          "raw":  "Black, Opacity 0.2",
                                          "css":  "color-mix(in srgb, #000000 20%, transparent)",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "AccentBrush",
                                          "type":  "SolidColorBrush",
                                          "raw":  "{DynamicResource AccentPrimary}",
                                          "css":  "var(--AccentPrimary)",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "AccentInactiveBrush",
                                          "type":  "SolidColorBrush",
                                          "raw":  "{DynamicResource AccentSecondary}",
                                          "css":  "var(--AccentSecondary)",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "AccentHighlightBrush",
                                          "type":  "SolidColorBrush",
                                          "raw":  "{DynamicResource AccentHighlight}",
                                          "css":  "var(--AccentHighlight)",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "ModulatorUnassignedBrush",
                                          "type":  "SolidColorBrush",
                                          "raw":  "{DynamicResource ModulatorUnassigned}",
                                          "css":  "var(--ModulatorUnassigned)",
                                          "note":  "The modulator assign button. The studio finds these by name: Modulator + category + (Highlight) + Brush."
                                      },
                                      {
                                          "name":  "ModulatorInternalBrush",
                                          "type":  "SolidColorBrush",
                                          "raw":  "{DynamicResource ModulatorInternal}",
                                          "css":  "var(--ModulatorInternal)",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "ModulatorInternalHighlightBrush",
                                          "type":  "SolidColorBrush",
                                          "raw":  "{DynamicResource ModulatorInternalHighlight}",
                                          "css":  "var(--ModulatorInternalHighlight)",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "ModulatorExternalBrush",
                                          "type":  "SolidColorBrush",
                                          "raw":  "{DynamicResource ModulatorExternal}",
                                          "css":  "var(--ModulatorExternal)",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "ModulatorExternalHighlightBrush",
                                          "type":  "SolidColorBrush",
                                          "raw":  "{DynamicResource ModulatorExternalHighlight}",
                                          "css":  "var(--ModulatorExternalHighlight)",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "PanelBackground",
                                          "type":  "SolidColorBrush",
                                          "raw":  "Black, Opacity 0.3",
                                          "css":  "color-mix(in srgb, #000000 30%, transparent)",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "PanelBorderBackground",
                                          "type":  "SolidColorBrush",
                                          "raw":  "Black, Opacity 0.4",
                                          "css":  "color-mix(in srgb, #000000 40%, transparent)",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "ListBoxBackground",
                                          "type":  "SolidColorBrush",
                                          "raw":  "Black, Opacity 0.4",
                                          "css":  "color-mix(in srgb, #000000 40%, transparent)",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "PanelBackgroundLevel1Brush",
                                          "type":  "SolidColorBrush",
                                          "raw":  "{DynamicResource Grey200}",
                                          "css":  "var(--Grey200)",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "PanelBackgroundLevel2Brush",
                                          "type":  "SolidColorBrush",
                                          "raw":  "{DynamicResource Grey300}",
                                          "css":  "var(--Grey300)",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "PanelBackgroundLevel3Brush",
                                          "type":  "SolidColorBrush",
                                          "raw":  "{DynamicResource Grey400}",
                                          "css":  "var(--Grey400)",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "PopupBackgroundBrush",
                                          "type":  "SolidColorBrush",
                                          "raw":  "{DynamicResource PopupCardBackground}",
                                          "css":  "var(--PopupCardBackground)",
                                          "note":  "The background of every popup, flyout, drop down and context menu. PopupFrame and DropdownFrame read it."
                                      },
                                      {
                                          "name":  "PopupCardWarningBrush",
                                          "type":  "SolidColorBrush",
                                          "raw":  "{DynamicResource PopupCardWarning}",
                                          "css":  "var(--PopupCardWarning)",
                                          "note":  "The line of a tooltip that says why a control is disabled, and the colors of a menu row that takes something away."
                                      },
                                      {
                                          "name":  "PopupCardDangerBrush",
                                          "type":  "SolidColorBrush",
                                          "raw":  "{DynamicResource PopupCardDanger}",
                                          "css":  "var(--PopupCardDanger)",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "TreeBackgroundBrush",
                                          "type":  "SolidColorBrush",
                                          "raw":  "{DynamicResource TreeBackground}",
                                          "css":  "var(--TreeBackground)",
                                          "note":  "The background of the project tree and the layer that dims a row."
                                      },
                                      {
                                          "name":  "Grey100Brush",
                                          "type":  "SolidColorBrush",
                                          "raw":  "{DynamicResource Grey100}",
                                          "css":  "var(--Grey100)",
                                          "note":  "The indent guides of the project tree: a quiet line at each level, and a stronger line on the branch of the cursor row."
                                      },
                                      {
                                          "name":  "Grey200Brush",
                                          "type":  "SolidColorBrush",
                                          "raw":  "{DynamicResource Grey200}",
                                          "css":  "var(--Grey200)",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "Grey300Brush",
                                          "type":  "SolidColorBrush",
                                          "raw":  "{DynamicResource Grey300}",
                                          "css":  "var(--Grey300)",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "Grey400Brush",
                                          "type":  "SolidColorBrush",
                                          "raw":  "{DynamicResource Grey400}",
                                          "css":  "var(--Grey400)",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "Grey500Brush",
                                          "type":  "SolidColorBrush",
                                          "raw":  "{DynamicResource Grey500}",
                                          "css":  "var(--Grey500)",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "Grey600Brush",
                                          "type":  "SolidColorBrush",
                                          "raw":  "{DynamicResource Grey600}",
                                          "css":  "var(--Grey600)",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "Grey700Brush",
                                          "type":  "SolidColorBrush",
                                          "raw":  "{DynamicResource Grey700}",
                                          "css":  "var(--Grey700)",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "Grey800Brush",
                                          "type":  "SolidColorBrush",
                                          "raw":  "{DynamicResource Grey800}",
                                          "css":  "var(--Grey800)",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "Grey900Brush",
                                          "type":  "SolidColorBrush",
                                          "raw":  "{DynamicResource Grey900}",
                                          "css":  "var(--Grey900)",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "TreeTextRestBrush",
                                          "type":  "SolidColorBrush",
                                          "raw":  "{DynamicResource TreeTextRest}",
                                          "css":  "var(--TreeTextRest)",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "StateIdleBrush",
                                          "type":  "SolidColorBrush",
                                          "raw":  "{DynamicResource Grey600}",
                                          "css":  "var(--Grey600)",
                                          "note":  "The states of the controls. Every theme reads these brushes, so a grey changes in one place. Surface controls (buttons, toggles, combo boxes, values, sliders): idle, then one step up under the pointer, then one step under idle while pressed (a press makes the control darker). The pressed grey is not the grey of a surface (Grey100 to Grey400), so a pressed control shows on the window and on every panel. The on state is accent-inactive, and accent under the pointer. Rows (list items, menu items, tabs): transparent, then RowHover under the pointer."
                                      },
                                      {
                                          "name":  "StateHoverBrush",
                                          "type":  "SolidColorBrush",
                                          "raw":  "{DynamicResource Grey700}",
                                          "css":  "var(--Grey700)",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "StatePressedBrush",
                                          "type":  "SolidColorBrush",
                                          "raw":  "{DynamicResource Grey500}",
                                          "css":  "var(--Grey500)",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "RowHoverBrush",
                                          "type":  "SolidColorBrush",
                                          "raw":  "{DynamicResource Grey500}",
                                          "css":  "var(--Grey500)",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "StateOnHoverBrush",
                                          "type":  "SolidColorBrush",
                                          "raw":  "{DynamicResource AccentHover}",
                                          "css":  "var(--AccentHover)",
                                          "note":  "A control that is on, or a row that is selected, under the pointer. At rest it has AccentInactiveBrush. The text on the two fills is ActiveForegroundBrush."
                                      },
                                      {
                                          "name":  "WarningMutedBrush",
                                          "type":  "SolidColorBrush",
                                          "raw":  "{DynamicResource WarningMuted}",
                                          "css":  "var(--WarningMuted)",
                                          "note":  "A muted warning: the frame of a target that refuses a drop, the mark of a conflict."
                                      },
                                      {
                                          "name":  "CursorFrameBrush",
                                          "type":  "SolidColorBrush",
                                          "raw":  "{DynamicResource ActiveText}",
                                          "css":  "var(--ActiveText)",
                                          "note":  "The frame of the cursor row of a list: ActiveText while the list has the focus, Grey800 when it does not."
                                      },
                                      {
                                          "name":  "CursorFrameInactiveBrush",
                                          "type":  "SolidColorBrush",
                                          "raw":  "{DynamicResource Grey800}",
                                          "css":  "var(--Grey800)",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "ForegroundBrush",
                                          "type":  "SolidColorBrush",
                                          "raw":  "{DynamicResource Text}",
                                          "css":  "var(--Text)",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "ActiveForegroundBrush",
                                          "type":  "SolidColorBrush",
                                          "raw":  "{DynamicResource ActiveText}",
                                          "css":  "var(--ActiveText)",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "TextMutedBrush",
                                          "type":  "SolidColorBrush",
                                          "raw":  "{DynamicResource ShortcutText}",
                                          "css":  "var(--ShortcutText)",
                                          "note":  "Quiet text on a popup: the title of a menu, the last line of a tooltip. It is the color of a shortcut, the darkest text that has a contrast of 4.5:1 on PopupCardBackground."
                                      },
                                      {
                                          "name":  "ShortcutTextBrush",
                                          "type":  "SolidColorBrush",
                                          "raw":  "{DynamicResource ShortcutText}",
                                          "css":  "var(--ShortcutText)",
                                          "note":  "The text of a shortcut (the class shortcut of a TextBlock), and the frame of a key cap (ContentControlKeyCap)."
                                      },
                                      {
                                          "name":  "ShortcutTextHoverBrush",
                                          "type":  "SolidColorBrush",
                                          "raw":  "{DynamicResource Grey900}",
                                          "css":  "var(--Grey900)",
                                          "note":  "The text of a shortcut on a hovered or selected menu row (RowHoverBrush)."
                                      },
                                      {
                                          "name":  "KeyCapBorderBrush",
                                          "type":  "SolidColorBrush",
                                          "raw":  "{DynamicResource Grey600}",
                                          "css":  "var(--Grey600)",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "HintStripBackgroundBrush",
                                          "type":  "SolidColorBrush",
                                          "raw":  "{DynamicResource Grey200}",
                                          "css":  "var(--Grey200)",
                                          "note":  "A hint strip (KeyHintStrip): a quiet fill, and the words in the grey of a shortcut."
                                      },
                                      {
                                          "name":  "HintStripTextBrush",
                                          "type":  "SolidColorBrush",
                                          "raw":  "{DynamicResource ShortcutText}",
                                          "css":  "var(--ShortcutText)",
                                          "note":  ""
                                      }
                                  ]
                   },
                   {
                       "id":  "0.3",
                       "name":  "Geometry",
                       "source":  "CMiX.UI/Themes/Tokens/Geometry.axaml",
                       "tokens":  [
                                      {
                                          "name":  "CornerRadiusNone",
                                          "type":  "CornerRadius",
                                          "raw":  "0",
                                          "css":  "0",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "CornerRadiusAll",
                                          "type":  "CornerRadius",
                                          "raw":  "2",
                                          "css":  "2px",
                                          "note":  "The one radius: 2. Every corner that is round has it."
                                      },
                                      {
                                          "name":  "CornerRadiusTop",
                                          "type":  "CornerRadius",
                                          "raw":  "2,2,0,0",
                                          "css":  "2px 2px 0 0",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "CornerRadiusBottom",
                                          "type":  "CornerRadius",
                                          "raw":  "0,0,2,2",
                                          "css":  "0 0 2px 2px",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "CornerRadiusLeft",
                                          "type":  "CornerRadius",
                                          "raw":  "2,0,0,2",
                                          "css":  "2px 0 0 2px",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "CornerRadiusRight",
                                          "type":  "CornerRadius",
                                          "raw":  "0,2,2,0",
                                          "css":  "0 2px 2px 0",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "MarginCard",
                                          "type":  "Thickness",
                                          "raw":  "2,1",
                                          "css":  "1px 2px",
                                          "note":  "The space around a card, such as ExpanderIndented: SpacingXS at the sides, SpacingXXS above and below."
                                      },
                                      {
                                          "name":  "ControlHeightXS",
                                          "type":  "Double",
                                          "raw":  "16",
                                          "css":  "16px",
                                          "note":  "Control heights"
                                      },
                                      {
                                          "name":  "ControlHeightMD",
                                          "type":  "Double",
                                          "raw":  "24",
                                          "css":  "24px",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "ControlHeightXL",
                                          "type":  "Double",
                                          "raw":  "30",
                                          "css":  "30px",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "ControlHeightXXL",
                                          "type":  "Double",
                                          "raw":  "34",
                                          "css":  "34px",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "IndicatorSize",
                                          "type":  "Double",
                                          "raw":  "8",
                                          "css":  "8px",
                                          "note":  "Sizes of small parts. They are not control heights, but they sit in the same scale."
                                      },
                                      {
                                          "name":  "SplitterSize",
                                          "type":  "Double",
                                          "raw":  "8",
                                          "css":  "8px",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "SplitterIndicatorSize",
                                          "type":  "Double",
                                          "raw":  "2",
                                          "css":  "2px",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "SplitterRowOverlap",
                                          "type":  "Thickness",
                                          "raw":  "0,-4",
                                          "css":  "-4px 0",
                                          "note":  "A horizontal splitter lies over the edge between two rows: half of its height in each row."
                                      },
                                      {
                                          "name":  "SplitterColumnOverlap",
                                          "type":  "Thickness",
                                          "raw":  "0,0,-6,0",
                                          "css":  "0 -6px 0 0",
                                          "note":  "A vertical splitter lies over the edge between two columns: SplitterSize less the scroll bar margin is in the right column."
                                      },
                                      {
                                          "name":  "ScrollBarSize",
                                          "type":  "Double",
                                          "raw":  "8",
                                          "css":  "8px",
                                          "note":  "The room of a scroll bar: ScrollBarThickness plus the two sides of ScrollBarMargin. A test pins the sum."
                                      },
                                      {
                                          "name":  "ScrollBarThickness",
                                          "type":  "Double",
                                          "raw":  "4",
                                          "css":  "4px",
                                          "note":  "The width of the vertical bar itself."
                                      },
                                      {
                                          "name":  "ScrollBarMargin",
                                          "type":  "Thickness",
                                          "raw":  "2,0,2,0",
                                          "css":  "0 2px 0 2px",
                                          "note":  "The space at the two sides of a bar."
                                      },
                                      {
                                          "name":  "ScrollBarGutter",
                                          "type":  "Thickness",
                                          "raw":  "0,0,8,0",
                                          "css":  "0 8px 0 0",
                                          "note":  "The room that the content of an overlay scroll viewer leaves on the right for the bar: ScrollBarSize. The bar lies in this room, so the content has one width with a bar and without one."
                                      },
                                      {
                                          "name":  "SeparatorSize",
                                          "type":  "Double",
                                          "raw":  "0.5",
                                          "css":  "0.5px",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "PopupMinWidth",
                                          "type":  "Double",
                                          "raw":  "200",
                                          "css":  "200px",
                                          "note":  "The least width of the visible panel of every menu and popup, without the margin of the shadow. More content makes it wider. A popup can follow the width of its toggle instead: TogglePopup.MatchesToggleWidth."
                                      },
                                      {
                                          "name":  "ColorWheelSize",
                                          "type":  "Double",
                                          "raw":  "200",
                                          "css":  "200px",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "TextAreaHeight2",
                                          "type":  "Double",
                                          "raw":  "44",
                                          "css":  "44px",
                                          "note":  "Multi line text boxes: two lines and four lines."
                                      },
                                      {
                                          "name":  "TextAreaHeight4",
                                          "type":  "Double",
                                          "raw":  "88",
                                          "css":  "88px",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "IconSizeXS",
                                          "type":  "Double",
                                          "raw":  "16",
                                          "css":  "16px",
                                          "note":  "Icon glyph sizes - deliberately separate from ControlHeight*, which sizes control bounding boxes, not glyph size. See Views/Controls/AppIcon."
                                      },
                                      {
                                          "name":  "IconSizeSM",
                                          "type":  "Double",
                                          "raw":  "12",
                                          "css":  "12px",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "IconSizeMD",
                                          "type":  "Double",
                                          "raw":  "16",
                                          "css":  "16px",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "IconSizeLG",
                                          "type":  "Double",
                                          "raw":  "24",
                                          "css":  "24px",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "ListVisibleItems",
                                          "type":  "Int32",
                                          "raw":  "5",
                                          "css":  "5",
                                          "note":  "Lists and drop downs. The height of each is a number of rows of ControlHeightMD, plus the margins and the spaces around the rows. No other file holds one of these numbers. ListBoxCMiX: ListVisibleItems rows + the top and bottom of ListScrollMargin. ListBoxEditable: ListEditableVisibleItems rows + the spaces of ListSpacing + ListEditablePadding. ComboBox: ComboBoxVisibleItems rows."
                                      },
                                      {
                                          "name":  "ListScrollMargin",
                                          "type":  "Thickness",
                                          "raw":  "2",
                                          "css":  "2px",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "ListResizeHandleHeight",
                                          "type":  "Double",
                                          "raw":  "12",
                                          "css":  "12px",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "ListEditableVisibleItems",
                                          "type":  "Int32",
                                          "raw":  "4",
                                          "css":  "4",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "ListSpacing",
                                          "type":  "Double",
                                          "raw":  "2",
                                          "css":  "2px",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "ListEditablePadding",
                                          "type":  "Thickness",
                                          "raw":  "4",
                                          "css":  "4px",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "ComboBoxVisibleItems",
                                          "type":  "Int32",
                                          "raw":  "7",
                                          "css":  "7",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "FontSizeMD",
                                          "type":  "Double",
                                          "raw":  "12",
                                          "css":  "12px",
                                          "note":  "The size of the text: about half a row of ControlHeightMD, rounded."
                                      },
                                      {
                                          "name":  "SpacingXXS",
                                          "type":  "Double",
                                          "raw":  "1",
                                          "css":  "1px",
                                          "note":  "Spacing scale for Margin/Padding"
                                      },
                                      {
                                          "name":  "SpacingXS",
                                          "type":  "Double",
                                          "raw":  "2",
                                          "css":  "2px",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "SpacingSM",
                                          "type":  "Double",
                                          "raw":  "4",
                                          "css":  "4px",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "SpacingMD",
                                          "type":  "Double",
                                          "raw":  "8",
                                          "css":  "8px",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "PopupCardPadding",
                                          "type":  "Thickness",
                                          "raw":  "10,8",
                                          "css":  "8px 10px",
                                          "note":  "The space inside the card of a tooltip and of the Add panel: SpacingMD above and below, 10 at the sides."
                                      },
                                      {
                                          "name":  "TipMaxWidth",
                                          "type":  "Double",
                                          "raw":  "260",
                                          "css":  "260px",
                                          "note":  "The widest line of a tooltip. A longer description wraps."
                                      },
                                      {
                                          "name":  "AddPanelWidth",
                                          "type":  "Double",
                                          "raw":  "200",
                                          "css":  "200px",
                                          "note":  "The width of the list inside the Add panel, without the card around it."
                                      },
                                      {
                                          "name":  "TreeIndent",
                                          "type":  "Double",
                                          "raw":  "16",
                                          "css":  "16px",
                                          "note":  "The indent of one level, and the width of the chevron."
                                      },
                                      {
                                          "name":  "TreeIndentMargin",
                                          "type":  "Thickness",
                                          "raw":  "16,0,0,0",
                                          "css":  "0 0 0 16px",
                                          "note":  "TreeIndent at the left: the drop line starts after the chevron."
                                      },
                                      {
                                          "name":  "TreeRowGap",
                                          "type":  "Double",
                                          "raw":  "3",
                                          "css":  "3px",
                                          "note":  "The space between two rows at a screen of 100%. The view sets it again in whole pixels."
                                      },
                                      {
                                          "name":  "TreeMargin",
                                          "type":  "Thickness",
                                          "raw":  "8,8,0,8",
                                          "css":  "8px 0 8px 8px",
                                          "note":  "The space around the tree: SpacingMD at the left, top and bottom. The scroll bar gutter is at the right."
                                      },
                                      {
                                          "name":  "TreeRowContentMargin",
                                          "type":  "Thickness",
                                          "raw":  "2,0,0,0",
                                          "css":  "0 0 0 2px",
                                          "note":  "The space before the content of a row: SpacingXS."
                                      },
                                      {
                                          "name":  "TreeKindSquareSize",
                                          "type":  "Double",
                                          "raw":  "16",
                                          "css":  "16px",
                                          "note":  "The square of a kind after the name of a collapsed row, and the sign of a missing asset: the icon (IconSizeXS) sits in it with the same margin on every side, so every kind shows the same way."
                                      },
                                      {
                                          "name":  "TreeHeaderMargin",
                                          "type":  "Thickness",
                                          "raw":  "4,0",
                                          "css":  "0 4px",
                                          "note":  "The space at the two sides of the icon and the name of a row: SpacingSM."
                                      },
                                      {
                                          "name":  "TreeToolbarPadding",
                                          "type":  "Thickness",
                                          "raw":  "4",
                                          "css":  "4px",
                                          "note":  "The space inside the toolbar of the tree: SpacingSM."
                                      },
                                      {
                                          "name":  "TreeSearchWidth",
                                          "type":  "Double",
                                          "raw":  "160",
                                          "css":  "160px",
                                          "note":  "The width of the search field of the tree, in the bar of the panel, at the right."
                                      },
                                      {
                                          "name":  "TreeRowHeight",
                                          "type":  "Double",
                                          "raw":  "24",
                                          "css":  "24px",
                                          "note":  "The row of the tree at a screen of 100%. TreeRowMetrics sets these again per view, in whole pixels of the screen."
                                      },
                                      {
                                          "name":  "TreeRowSpacing",
                                          "type":  "Double",
                                          "raw":  "1",
                                          "css":  "1px",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "TreeDropLineThickness",
                                          "type":  "Double",
                                          "raw":  "1",
                                          "css":  "1px",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "TreeDropLineGapMargin",
                                          "type":  "Thickness",
                                          "raw":  "0,25,0,0",
                                          "css":  "25px 0 0 0",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "TreeDropLineTopMargin",
                                          "type":  "Thickness",
                                          "raw":  "0,-2,0,0",
                                          "css":  "-2px 0 0 0",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "TreeSelectionThickness",
                                          "type":  "Thickness",
                                          "raw":  "1",
                                          "css":  "1px",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "TreeToggleWidth",
                                          "type":  "Double",
                                          "raw":  "50",
                                          "css":  "50px",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "TreeDimOpacity",
                                          "type":  "Double",
                                          "raw":  "0.6",
                                          "css":  "0.6",
                                          "note":  "The cover of a row that can not take a drag, and the half cover of a collapsed row with a target deeper."
                                      },
                                      {
                                          "name":  "TreeHintCoverOpacity",
                                          "type":  "Double",
                                          "raw":  "0.3",
                                          "css":  "0.3",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "DragScrollEdge",
                                          "type":  "Double",
                                          "raw":  "28",
                                          "css":  "28px",
                                          "note":  "The height of the zone at the top and the bottom edge that scrolls the view."
                                      },
                                      {
                                          "name":  "DragScrollStep",
                                          "type":  "Double",
                                          "raw":  "18",
                                          "css":  "18px",
                                          "note":  "The largest scroll of one step of the timer."
                                      },
                                      {
                                          "name":  "DragThreshold",
                                          "type":  "Double",
                                          "raw":  "4",
                                          "css":  "4px",
                                          "note":  "The distance that a press moves before a drag starts."
                                      },
                                      {
                                          "name":  "CursorFrameThickness",
                                          "type":  "Thickness",
                                          "raw":  "2",
                                          "css":  "2px",
                                          "note":  "The frame of the cursor row: CursorFrameThickness while the list has the focus, CursorFrameInactiveThickness when it does not."
                                      },
                                      {
                                          "name":  "CursorFrameInactiveThickness",
                                          "type":  "Thickness",
                                          "raw":  "1",
                                          "css":  "1px",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "OpacityDisabled",
                                          "type":  "Double",
                                          "raw":  "0.5",
                                          "css":  "0.5",
                                          "note":  "A disabled control."
                                      },
                                      {
                                          "name":  "OpacityDragSource",
                                          "type":  "Double",
                                          "raw":  "0.4",
                                          "css":  "0.4",
                                          "note":  "The item that a drag moves, at its old place."
                                      },
                                      {
                                          "name":  "OpacityDragGhost",
                                          "type":  "Double",
                                          "raw":  "0.8",
                                          "css":  "0.8",
                                          "note":  "The copy of the item that follows the pointer during a drag."
                                      },
                                      {
                                          "name":  "OpacityIconIdle",
                                          "type":  "Double",
                                          "raw":  "0.6",
                                          "css":  "0.6",
                                          "note":  "An icon has one of two opacities. OpacityIconIdle: a quiet icon. An icon only control that is not under the pointer and not on, a chevron, the arrow of a menu row, the icon before a name. OpacityIconAction: the icon of a button that does an action, in a filled button or in a toolbar."
                                      },
                                      {
                                          "name":  "OpacityIconAction",
                                          "type":  "Double",
                                          "raw":  "0.9",
                                          "css":  "0.9",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "KeyCapBorderThickness",
                                          "type":  "Thickness",
                                          "raw":  "1",
                                          "css":  "1px",
                                          "note":  "A key cap (ContentControlKeyCap): the line around the key text, and the space between the line and the text."
                                      },
                                      {
                                          "name":  "KeyCapPadding",
                                          "type":  "Thickness",
                                          "raw":  "4,0",
                                          "css":  "0 4px",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "NotificationMargin",
                                          "type":  "Thickness",
                                          "raw":  "0,0,0,16",
                                          "css":  "0 0 16px 0",
                                          "note":  "The space between a notification (PanelNotification) and the bottom edge of its area."
                                      },
                                      {
                                          "name":  "HintStripHeight",
                                          "type":  "Double",
                                          "raw":  "22",
                                          "css":  "22px",
                                          "note":  "A hint strip (KeyHintStrip): its height, the space at its two sides (SpacingMD), and the space between two hints."
                                      },
                                      {
                                          "name":  "HintStripPadding",
                                          "type":  "Thickness",
                                          "raw":  "8,0",
                                          "css":  "0 8px",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "HintStripSpacing",
                                          "type":  "Double",
                                          "raw":  "12",
                                          "css":  "12px",
                                          "note":  ""
                                      }
                                  ]
                   },
                   {
                       "id":  "0.4",
                       "name":  "Typography",
                       "source":  "CMiX.UI/Themes/Tokens/Typography.axaml",
                       "tokens":  [
                                      {
                                          "name":  "DefaultFontSize",
                                          "type":  "Double",
                                          "raw":  "10",
                                          "css":  "10px",
                                          "note":  ""
                                      },
                                      {
                                          "name":  "FontFamilyMono",
                                          "type":  "FontFamily",
                                          "raw":  "Cascadia Mono,Consolas,Menlo,monospace",
                                          "css":  "Cascadia Mono,Consolas,Menlo,monospace",
                                          "note":  "The font of a shortcut and of a command name in a tooltip."
                                      }
                                  ]
                   },
                   {
                       "id":  "0.5",
                       "name":  "Motion",
                       "source":  "CMiX.UI/Themes/Motion.cs",
                       "tokens":  [
                                      {
                                          "name":  "MotionDuration",
                                          "type":  "TimeSpan",
                                          "raw":  "160 ms",
                                          "css":  "160ms",
                                          "note":  "Motion.Duration"
                                      },
                                      {
                                          "name":  "MotionFadeDuration",
                                          "type":  "TimeSpan",
                                          "raw":  "120 ms",
                                          "css":  "120ms",
                                          "note":  "Motion.FadeDuration"
                                      },
                                      {
                                          "name":  "MotionEasing",
                                          "type":  "Easing",
                                          "raw":  "CubicEaseOut",
                                          "css":  "cubic-bezier(0.215, 0.61, 0.355, 1)",
                                          "note":  "Motion.Easing"
                                      }
                                  ]
                   }
               ],
    "skipped":  [
                    {
                        "name":  "CheckerboardBrush",
                        "type":  "DrawingBrush",
                        "source":  "Tokens\\Brushes.axaml"
                    }
                ]
};
