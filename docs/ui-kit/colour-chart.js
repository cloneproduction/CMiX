/* The chart of the colour system, board 0.0 of gallery.html. Written after the colour audit of 2026-11-14 (colour-audit-2026-11-14.md).
   groups: the tokens by meaning. A token name is the exact x:Key of Colors.axaml or Brushes.axaml; every key appears once.
   tones: the ladder of CategoryPalette, from weak to strong. legend: the sentences a user keeps in mind. A flag is a finding of the audit, not a decision. */
window.CMIX_COLOUR_CHART = {
    intro: 'CMiX is not a grey UI with one accent. Colour tells the user the context: the kind of a node (a hue), its place in the nesting (the composable wheel), its state (a tone of that hue), and the state of the system (status). This board reads the tokens by meaning; boards 0.1 and 0.2 list them by name. The flags are the findings of the colour audit of 2026-11-14 and wait for a decision of the owner.',
    "groups":  [
                       {
                           "id":  "surfaces",
                           "title":  "Surfaces",
                           "meaning":  "Where you are: the window, a panel, a popup. Surfaces step up in grey; they never carry a hue.",
                           "rows":  [
                                        {
                                            "role":  "Window",
                                            "meaning":  "The deepest surface: the window and the project tree. Every tone of the tree is mixed over it.",
                                            "tokens":  [
                                                           "TreeBackground",
                                                           "TreeBackgroundBrush"
                                                       ],
                                            "rule":  "Use for the tree background and the dim cover of a row; the window itself uses the ramp key Grey100.",
                                            "flags":  [
                                                          "TreeBackground duplicates Grey100 (#0F0F12): keep the brush as an alias of Grey100, retire the Color"
                                                      ]
                                        },
                                        {
                                            "role":  "Popup",
                                            "meaning":  "A card that floats over the window: tooltip, menu, flyout, Add panel.",
                                            "tokens":  [
                                                           "PopupCardBackground",
                                                           "PopupBackgroundBrush"
                                                       ],
                                            "rule":  "Every popup, no outline. A tree tooltip replaces it with the hover tone of its row.",
                                            "flags":  [
                                                          "a surface step (L 0.21) that is not in the grey ramp"
                                                      ]
                                        },
                                        {
                                            "role":  "Panel level 1",
                                            "meaning":  "A panel on the window, and the stripes behind the tree rows.",
                                            "tokens":  [
                                                           "PanelBackgroundLevel1Brush",
                                                           "HintStripBackgroundBrush"
                                                       ],
                                            "rule":  "First panel inside the window (Grey200).",
                                            "flags":  [
                                                          "HintStripBackgroundBrush is a place alias of the same grey"
                                                      ]
                                        },
                                        {
                                            "role":  "Panel level 2",
                                            "meaning":  "A panel inside a panel; the tree toolbar.",
                                            "tokens":  [
                                                           "PanelBackgroundLevel2Brush"
                                                       ],
                                            "rule":  "Second nesting (Grey300). The toolbar of the tree reads Grey300Brush directly today (ProjectTreeView.axaml:43).",
                                            "flags":  [
    
                                                      ]
                                        },
                                        {
                                            "role":  "Panel level 3",
                                            "meaning":  "The deepest nesting of panels.",
                                            "tokens":  [
                                                           "PanelBackgroundLevel3Brush"
                                                       ],
                                            "rule":  "Third nesting (Grey400). Do not nest further.",
                                            "flags":  [
    
                                                      ]
                                        },
                                        {
                                            "role":  "Translucent overlay",
                                            "meaning":  "A black tint over whatever is below: list surfaces and expander headers.",
                                            "tokens":  [
                                                           "PanelBackground",
                                                           "ListBoxBackground",
                                                           "PanelBorderBackground"
                                                       ],
                                            "rule":  "Only where the surface below must show through. Prefer a ramp grey when it does not.",
                                            "flags":  [
                                                          "literal Black with Opacity 0.3 / 0.4 / 0.4",
                                                          "PanelBorderBackground duplicates ListBoxBackground"
                                                      ]
                                        },
                                        {
                                            "role":  "Divider",
                                            "meaning":  "A faint line between two surfaces.",
                                            "tokens":  [
                                                           "Shadow"
                                                       ],
                                            "rule":  "One use, ComposableListBox.axaml:9. Name says shadow, the use is a border.",
                                            "flags":  [
                                                          "rename to DividerBrush or merge into a ramp grey"
                                                      ]
                                        },
                                        {
                                            "role":  "Transparency",
                                            "meaning":  "The checker that says \u0027nothing here\u0027 behind a colour swatch.",
                                            "tokens":  [
                                                           "CheckerboardBrush"
                                                       ],
                                            "rule":  "Colour swatches only.",
                                            "flags":  [
    
                                                      ]
                                        }
                                    ]
                       },
                       {
                           "id":  "text",
                           "title":  "Text and icons",
                           "meaning":  "How loud a word is: rest, standard, strong (on a colour), muted.",
                           "rows":  [
                                        {
                                            "role":  "Standard",
                                            "meaning":  "Text and icons on any grey surface.",
                                            "tokens":  [
                                                           "Text",
                                                           "ForegroundBrush"
                                                       ],
                                            "rule":  "Default. 14.5:1 on Grey100, 4.75:1 on Grey700.",
                                            "flags":  [
    
                                                      ]
                                        },
                                        {
                                            "role":  "Rest (tree row)",
                                            "meaning":  "A row that is not the cursor and not chosen: one step under Text so the white of the chosen row stands out.",
                                            "tokens":  [
                                                           "TreeTextRest",
                                                           "TreeTextRestBrush"
                                                       ],
                                            "rule":  "Row text and icons of the tree at rest; also the toggles at rest. 7.9:1 or more on every rest fill.",
                                            "flags":  [
    
                                                      ]
                                        },
                                        {
                                            "role":  "Strong, on a colour",
                                            "meaning":  "White: text and icon on a chosen row, on a badge, in the Inspector header.",
                                            "tokens":  [
                                                           "ActiveText",
                                                           "ActiveForegroundBrush"
                                                       ],
                                            "rule":  "Only on a coloured fill of strength 0.55 or less (text) or on a badge (icon). Never as body text on a grey.",
                                            "flags":  [
                                                          "also the colour of CursorFrameBrush: one white, several shapes"
                                                      ]
                                        },
                                        {
                                            "role":  "Muted",
                                            "meaning":  "Quiet text: a shortcut, a menu title, the last line of a tooltip, the key hints.",
                                            "tokens":  [
                                                           "ShortcutText",
                                                           "TextMutedBrush",
                                                           "ShortcutTextBrush",
                                                           "HintStripTextBrush"
                                                       ],
                                            "rule":  "Only on Grey100 or PopupCardBackground (5.2 / 4.8:1). On Grey200 it is 4.38:1, under AA.",
                                            "flags":  [
                                                          "three brushes for one colour and one meaning: keep TextMutedBrush",
                                                          "hint strip (Grey200) fails AA"
                                                      ]
                                        },
                                        {
                                            "role":  "Muted on a hovered row",
                                            "meaning":  "The shortcut of a menu row under the pointer.",
                                            "tokens":  [
                                                           "ShortcutTextHoverBrush"
                                                       ],
                                            "rule":  "Grey900 on RowHoverBrush (4.74:1).",
                                            "flags":  [
    
                                                      ]
                                        },
                                        {
                                            "role":  "Key cap frame",
                                            "meaning":  "The outline of a key in a tooltip.",
                                            "tokens":  [
                                                           "KeyCapBorderBrush"
                                                       ],
                                            "rule":  "Grey600; the text inside is Muted.",
                                            "flags":  [
    
                                                      ]
                                        }
                                    ]
                       },
                       {
                           "id":  "identity",
                           "title":  "Identity hues (kinds)",
                           "meaning":  "Which kind of thing this is. The hue is the same at every depth; the icon of the kind always sits next to it.",
                           "rows":  [
                                        {
                                            "role":  "Entity",
                                            "meaning":  "A thing in the scene: model, quad, text.",
                                            "tokens":  [
                                                           "CategoryEntity"
                                                       ],
                                            "rule":  "Rows, badges, Inspector header of an entity. OKLCH L.72 C.16 h0.",
                                            "flags":  [
                                                          "dE 8.3 to CategoryComposable0",
                                                          "dE 7.0 to PopupCardDanger",
                                                          "deutan: same as CategoryFilter (dE 1.5)"
                                                      ]
                                        },
                                        {
                                            "role":  "Camera",
                                            "meaning":  "A camera.",
                                            "tokens":  [
                                                           "CategoryCamera"
                                                       ],
                                            "rule":  "OKLCH L.72 C.16 h35.",
                                            "flags":  [
                                                          "dE 3.0 to CategoryComposable1",
                                                          "dE 3.9 to PopupCardDanger",
                                                          "deutan: same as CategoryTexture (dE 1.4)"
                                                      ]
                                        },
                                        {
                                            "role":  "Light",
                                            "meaning":  "A light.",
                                            "tokens":  [
                                                           "CategoryLight"
                                                       ],
                                            "rule":  "OKLCH L.84 C.16 h100. The only kind at a higher L; a white icon on its badge is 3.99:1.",
                                            "flags":  [
                                                          "protan: dE 8.5 to CategoryTexture"
                                                      ]
                                        },
                                        {
                                            "role":  "Texture",
                                            "meaning":  "A texture source.",
                                            "tokens":  [
                                                           "CategoryTexture"
                                                       ],
                                            "rule":  "OKLCH L.72 C.17 h135.",
                                            "flags":  [
                                                          "dE 5.4 to CategoryComposable3",
                                                          "dE 6.5 to StatusSuccess"
                                                      ]
                                        },
                                        {
                                            "role":  "Filter",
                                            "meaning":  "A texture filter.",
                                            "tokens":  [
                                                           "CategoryFilter"
                                                       ],
                                            "rule":  "OKLCH L.74 C.13 h175.",
                                            "flags":  [
                                                          "dE 11.1 to CategoryTexture and to CategoryModifier",
                                                          "dE 7.7 to CategoryComposable4"
                                                      ]
                                        },
                                        {
                                            "role":  "Modifier",
                                            "meaning":  "A modifier.",
                                            "tokens":  [
                                                           "CategoryModifier"
                                                       ],
                                            "rule":  "OKLCH L.72 C.13 h225.",
                                            "flags":  [
                                                          "dE 3.8 to CategoryComposable4",
                                                          "deutan: dE 4.6 to CategoryModulator"
                                                      ]
                                        },
                                        {
                                            "role":  "Modulator",
                                            "meaning":  "A modulator; also the assign dot of an internal modulator.",
                                            "tokens":  [
                                                           "CategoryModulator"
                                                       ],
                                            "rule":  "OKLCH L.72 C.16 h295. ModulatorInternal is the same value by design.",
                                            "flags":  [
                                                          "dE 7.4 to CategoryComposable5"
                                                      ]
                                        }
                                    ]
                       },
                       {
                           "id":  "wheel",
                           "title":  "Composable wheel (depth 0 to 5)",
                           "meaning":  "How deep a composable is nested. A container and the containers inside it never share a hue; the hue repeats after six levels.",
                           "rows":  [
                                        {
                                            "role":  "Depth 0",
                                            "meaning":  "A composable at the root.",
                                            "tokens":  [
                                                           "CategoryComposable0"
                                                       ],
                                            "rule":  "OKLCH L.74 C.14 h330.",
                                            "flags":  [
                                                          "dE 8.3 to CategoryEntity",
                                                          "deutan: same as depth 4 (dE 1.5)"
                                                      ]
                                        },
                                        {
                                            "role":  "Depth 1",
                                            "meaning":  "A composable inside a depth-0 composable.",
                                            "tokens":  [
                                                           "CategoryComposable1"
                                                       ],
                                            "rule":  "OKLCH L.74 C.14 h30.",
                                            "flags":  [
                                                          "dE 3.0 to CategoryCamera",
                                                          "dE 4.1 to PopupCardDanger",
                                                          "deutan: same as depth 3 (dE 2.1)"
                                                      ]
                                        },
                                        {
                                            "role":  "Depth 2",
                                            "meaning":  "Second nesting.",
                                            "tokens":  [
                                                           "CategoryComposable2"
                                                       ],
                                            "rule":  "OKLCH L.74 C.14 h90.",
                                            "flags":  [
                                                          "dE 5.3 to PopupCardWarning"
                                                      ]
                                        },
                                        {
                                            "role":  "Depth 3",
                                            "meaning":  "Third nesting.",
                                            "tokens":  [
                                                           "CategoryComposable3"
                                                       ],
                                            "rule":  "OKLCH L.74 C.14 h150.",
                                            "flags":  [
                                                          "dE 2.0 to StatusSuccess: a depth-3 row is the \u0027connected\u0027 green",
                                                          "dE 5.4 to CategoryTexture"
                                                      ]
                                        },
                                        {
                                            "role":  "Depth 4",
                                            "meaning":  "Fourth nesting.",
                                            "tokens":  [
                                                           "CategoryComposable4"
                                                       ],
                                            "rule":  "OKLCH L.74 C.13 h210.",
                                            "flags":  [
                                                          "dE 3.8 to CategoryModifier",
                                                          "dE 7.7 to CategoryFilter"
                                                      ]
                                        },
                                        {
                                            "role":  "Depth 5",
                                            "meaning":  "Fifth nesting; depth 6 wraps to depth 0.",
                                            "tokens":  [
                                                           "CategoryComposable5"
                                                       ],
                                            "rule":  "OKLCH L.74 C.13 h270.",
                                            "flags":  [
                                                          "dE 7.4 to CategoryModulator"
                                                      ]
                                        }
                                    ]
                       },
                       {
                           "id":  "interaction",
                           "title":  "Interaction",
                           "meaning":  "What my hands and keys are doing: a control that is on, the pointer, the focus, the cursor, a drop.",
                           "rows":  [
                                        {
                                            "role":  "On, at rest",
                                            "meaning":  "A control that is on, or a row of a grey list that is selected.",
                                            "tokens":  [
                                                           "AccentSecondary",
                                                           "AccentInactiveBrush"
                                                       ],
                                            "rule":  "Toggles, radios, slider tracks, list rows. White text on it is 5.93:1.",
                                            "flags":  [
    
                                                      ]
                                        },
                                        {
                                            "role":  "On, under the pointer",
                                            "meaning":  "The same control with the pointer over it.",
                                            "tokens":  [
                                                           "AccentHover",
                                                           "StateOnHoverBrush"
                                                       ],
                                            "rule":  "One step up from On at rest. White text 4.70:1.",
                                            "flags":  [
                                                          "Button.axaml:233 uses AccentBrush for this instead: a synonym"
                                                      ]
                                        },
                                        {
                                            "role":  "Focus and selection line",
                                            "meaning":  "Where the keyboard goes: the frame of a focused text box or panel, the text selection, the grab frame.",
                                            "tokens":  [
                                                           "AccentPrimary",
                                                           "AccentBrush"
                                                       ],
                                            "rule":  "Lines and selections only, never a fill with text (white on it is 3.99:1). On Grey600 the line is 2.14:1: use it on surfaces, not on controls.",
                                            "flags":  [
                                                          "collision: also \u0027on + pointer\u0027 in Button.axaml and the \u0027in sync\u0027 status dot",
                                                          "the tree uses the kind hue for focus and cursor: two focus codes"
                                                      ]
                                        },
                                        {
                                            "role":  "Control idle",
                                            "meaning":  "A button, toggle, combo or value at rest.",
                                            "tokens":  [
                                                           "StateIdleBrush"
                                                       ],
                                            "rule":  "Grey600. Text on it 6.48:1.",
                                            "flags":  [
    
                                                      ]
                                        },
                                        {
                                            "role":  "Control under the pointer",
                                            "meaning":  "The pointer is over a control.",
                                            "tokens":  [
                                                           "StateHoverBrush"
                                                       ],
                                            "rule":  "Grey700, one step up. Text on it 4.75:1.",
                                            "flags":  [
    
                                                      ]
                                        },
                                        {
                                            "role":  "Control pressed",
                                            "meaning":  "The button is down.",
                                            "tokens":  [
                                                           "StatePressedBrush"
                                                       ],
                                            "rule":  "Grey500, one step under idle: a press goes darker.",
                                            "flags":  [
    
                                                      ]
                                        },
                                        {
                                            "role":  "Row under the pointer",
                                            "meaning":  "The pointer is over a list, menu or tab row.",
                                            "tokens":  [
                                                           "RowHoverBrush"
                                                       ],
                                            "rule":  "Grey500 over a transparent row. The tree uses the hover tone of its kind instead.",
                                            "flags":  [
                                                          "same grey as pressed"
                                                      ]
                                        },
                                        {
                                            "role":  "Cursor with focus; drop marks",
                                            "meaning":  "The row the arrow keys are on while the list has the focus; the row or gap that takes a drop.",
                                            "tokens":  [
                                                           "CursorFrameBrush"
                                                       ],
                                            "rule":  "A 1 px white frame or line. In the tree the cursor frame is the kind hue, and white only on the chosen row.",
                                            "flags":  [
                                                          "one white, two cues: cursor (frame) and drop (frame or line); the context tells them apart"
                                                      ]
                                        },
                                        {
                                            "role":  "Cursor without focus",
                                            "meaning":  "The cursor row of a list that does not have the focus.",
                                            "tokens":  [
                                                           "CursorFrameInactiveBrush"
                                                       ],
                                            "rule":  "Grey800, 1 px.",
                                            "flags":  [
    
                                                      ]
                                        }
                                    ]
                       },
                       {
                           "id":  "status",
                           "title":  "Status",
                           "meaning":  "Healthy or broken: the state of the sync, a missing asset, a destructive action. Always with a shape or a word.",
                           "rows":  [
                                        {
                                            "role":  "Ok",
                                            "meaning":  "Connected, in sync, fine.",
                                            "tokens":  [
                                                           "StatusSuccess"
                                                       ],
                                            "rule":  "The connected dot. Also take over the \u0027in sync\u0027 dot from AccentPrimary.",
                                            "flags":  [
                                                          "dE 2.0 to CategoryComposable3",
                                                          "deutan: dE 3.5 to PopupCardDanger"
                                                      ]
                                        },
                                        {
                                            "role":  "Warning",
                                            "meaning":  "Needs attention: unsynced, a missing asset, why a control is disabled.",
                                            "tokens":  [
                                                           "PopupCardWarning",
                                                           "PopupCardWarningBrush"
                                                       ],
                                            "rule":  "Sync dot, missing-asset sign, the warning line of a tooltip.",
                                            "flags":  [
                                                          "rename to StatusWarning: the name says a place",
                                                          "dE 7.2 to ModulatorExternal"
                                                      ]
                                        },
                                        {
                                            "role":  "Danger",
                                            "meaning":  "Bad: disconnected, or an action that takes something away.",
                                            "tokens":  [
                                                           "PopupCardDanger",
                                                           "PopupCardDangerBrush"
                                                       ],
                                            "rule":  "Sync dot, Delete row text.",
                                            "flags":  [
                                                          "rename to StatusDanger",
                                                          "dE 3.9 to CategoryCamera",
                                                          "protan: dE 5.7 to Grey800 (pending dot)"
                                                      ]
                                        },
                                        {
                                            "role":  "Conflict, muted",
                                            "meaning":  "A mark that two things clash (shortcut conflict).",
                                            "tokens":  [
                                                           "WarningMuted",
                                                           "WarningMutedBrush"
                                                       ],
                                            "rule":  "One use, ShortcutSettingsView.axaml:49. Use Warning instead.",
                                            "flags":  [
                                                          "candidate to retire: a fourth status hue for one mark",
                                                          "comment says \u0027refused drop frame\u0027 but the tree uses a grey cover"
                                                      ]
                                        }
                                    ]
                       },
                       {
                           "id":  "modulator",
                           "title":  "Modulator source",
                           "meaning":  "Who drives this control: nobody, a modulator inside CMiX, or a value from outside.",
                           "rows":  [
                                        {
                                            "role":  "Unassigned",
                                            "meaning":  "No modulator on this control.",
                                            "tokens":  [
                                                           "ModulatorUnassigned",
                                                           "ModulatorUnassignedBrush"
                                                       ],
                                            "rule":  "A Grey700 dot; 3.05:1 on Grey100, 2.56:1 on Grey200.",
                                            "flags":  [
                                                          "same hex as Grey700"
                                                      ]
                                        },
                                        {
                                            "role":  "Internal",
                                            "meaning":  "An internal modulator drives this control.",
                                            "tokens":  [
                                                           "ModulatorInternal",
                                                           "ModulatorInternalBrush"
                                                       ],
                                            "rule":  "The kind hue of a modulator, so the dot and the tree row match.",
                                            "flags":  [
                                                          "same value as CategoryModulator by design"
                                                      ]
                                        },
                                        {
                                            "role":  "Internal, hovered",
                                            "meaning":  "The same, while the modulator row is under the pointer.",
                                            "tokens":  [
                                                           "ModulatorInternalHighlight",
                                                           "ModulatorInternalHighlightBrush"
                                                       ],
                                            "rule":  "A lighter step of the same hue.",
                                            "flags":  [
    
                                                      ]
                                        },
                                        {
                                            "role":  "External",
                                            "meaning":  "A value from outside (Redis) drives this control.",
                                            "tokens":  [
                                                           "ModulatorExternal",
                                                           "ModulatorExternalBrush"
                                                       ],
                                            "rule":  "Today orange. Proposed: the Modulator violet as a ring, so one hue and two shapes say internal or external.",
                                            "flags":  [
                                                          "dE 7.2 to PopupCardWarning: reads as a warning"
                                                      ]
                                        },
                                        {
                                            "role":  "External, hovered",
                                            "meaning":  "The same, hovered.",
                                            "tokens":  [
                                                           "ModulatorExternalHighlight",
                                                           "ModulatorExternalHighlightBrush"
                                                       ],
                                            "rule":  "A lighter step of the same hue.",
                                            "flags":  [
    
                                                      ]
                                        }
                                    ]
                       },
                       {
                           "id":  "greys",
                           "title":  "Grey ramp",
                           "meaning":  "The primitive steps every surface and state is built from. Views should read a surface or state brush, not a step.",
                           "rows":  [
                                        {
                                            "role":  "Grey100",
                                            "meaning":  "Window. OKLab L 0.170.",
                                            "tokens":  [
                                                           "Grey100",
                                                           "Grey100Brush"
                                                       ],
                                            "rule":  "Window surface; mix base of the tree.",
                                            "flags":  [
                                                          "33 raw uses in views"
                                                      ]
                                        },
                                        {
                                            "role":  "Grey200",
                                            "meaning":  "Panel level 1. L 0.249.",
                                            "tokens":  [
                                                           "Grey200",
                                                           "Grey200Brush"
                                                       ],
                                            "rule":  "Through PanelBackgroundLevel1Brush.",
                                            "flags":  [
    
                                                      ]
                                        },
                                        {
                                            "role":  "Grey300",
                                            "meaning":  "Panel level 2, toolbar, empty group row. L 0.290.",
                                            "tokens":  [
                                                           "Grey300",
                                                           "Grey300Brush"
                                                       ],
                                            "rule":  "Through PanelBackgroundLevel2Brush.",
                                            "flags":  [
                                                          "20 raw uses"
                                                      ]
                                        },
                                        {
                                            "role":  "Grey400",
                                            "meaning":  "Panel level 3, empty group hovered. L 0.334.",
                                            "tokens":  [
                                                           "Grey400",
                                                           "Grey400Brush"
                                                       ],
                                            "rule":  "Through PanelBackgroundLevel3Brush.",
                                            "flags":  [
    
                                                      ]
                                        },
                                        {
                                            "role":  "Grey500",
                                            "meaning":  "Pressed control, hovered row, chosen empty group. L 0.376.",
                                            "tokens":  [
                                                           "Grey500",
                                                           "Grey500Brush"
                                                       ],
                                            "rule":  "Through StatePressedBrush or RowHoverBrush.",
                                            "flags":  [
                                                          "one grey, three meanings"
                                                      ]
                                        },
                                        {
                                            "role":  "Grey600",
                                            "meaning":  "Idle control, key cap frame, the +n badge. L 0.418.",
                                            "tokens":  [
                                                           "Grey600",
                                                           "Grey600Brush"
                                                       ],
                                            "rule":  "Through StateIdleBrush.",
                                            "flags":  [
    
                                                      ]
                                        },
                                        {
                                            "role":  "Grey700",
                                            "meaning":  "Hovered control, dead group text. L 0.490.",
                                            "tokens":  [
                                                           "Grey700",
                                                           "Grey700Brush"
                                                       ],
                                            "rule":  "Through StateHoverBrush. Text on it: Text 4.75:1, TreeTextRest 3.80:1.",
                                            "flags":  [
    
                                                      ]
                                        },
                                        {
                                            "role":  "Grey800",
                                            "meaning":  "Chevron, cursor without focus, pending dot. L 0.601.",
                                            "tokens":  [
                                                           "Grey800",
                                                           "Grey800Brush"
                                                       ],
                                            "rule":  "Lines and small marks: 4.86:1 on Grey100, 3.06:1 on Grey400.",
                                            "flags":  [
                                                          "0.018 L from ShortcutText: a near duplicate",
                                                          "carries a status meaning (pending)"
                                                      ]
                                        },
                                        {
                                            "role":  "Grey900",
                                            "meaning":  "Muted text on a hovered row. L 0.760.",
                                            "tokens":  [
                                                           "Grey900",
                                                           "Grey900Brush"
                                                       ],
                                            "rule":  "Text on Grey500 or darker.",
                                            "flags":  [
    
                                                      ]
                                        }
                                    ]
                       },
                       {
                           "id":  "retire",
                           "title":  "Retire",
                           "meaning":  "Keys with no use or no meaning of their own.",
                           "rows":  [
                                        {
                                            "role":  "Active",
                                            "meaning":  "none",
                                            "tokens":  [
                                                           "Active"
                                                       ],
                                            "rule":  "Remove.",
                                            "flags":  [
                                                          "0 uses",
                                                          "duplicate of AccentPrimary"
                                                      ]
                                        },
                                        {
                                            "role":  "AccentHighlight",
                                            "meaning":  "none",
                                            "tokens":  [
                                                           "AccentHighlight",
                                                           "AccentHighlightBrush"
                                                       ],
                                            "rule":  "Remove.",
                                            "flags":  [
                                                          "0 uses of the brush"
                                                      ]
                                        },
                                        {
                                            "role":  "CategoryComposable",
                                            "meaning":  "none",
                                            "tokens":  [
                                                           "CategoryComposable"
                                                       ],
                                            "rule":  "Remove. CategoryPalette.ColorKey always writes an indexed key.",
                                            "flags":  [
                                                          "0 code uses",
                                                          "duplicate of CategoryComposable0"
                                                      ]
                                        }
                                    ]
                       }
                   ],
        "tones":  [
                      {
                          "id":  "Floor",
                          "strength":  0.09,
                          "meaning":  "The weakest rest fill, for a row deep under changes of kind (FillFloor). Below the step the eye notices: 1.13:1 on the background."
                      },
                      {
                          "id":  "Fill",
                          "strength":  0.18,
                          "meaning":  "A row of this kind, at rest. The strength falls by 0.8 per change of kind down to the floor. Text is TreeTextRest."
                      },
                      {
                          "id":  "Hover",
                          "strength":  0.32,
                          "meaning":  "The pointer is over this row; also the fill of the tooltips of the row. Text is TreeTextRest."
                      },
                      {
                          "id":  "Chosen",
                          "strength":  0.55,
                          "meaning":  "The row I picked, and the Inspector header. Text and icon white (4.5:1 or more on every kind). The code constant is 0.55; 0.85 fails AA."
                      },
                      {
                          "id":  "Kind",
                          "strength":  0.6,
                          "meaning":  "A badge, a chip, a bar: the colour of the kind with a white icon and no text (3.99:1 or more)."
                      },
                      {
                          "id":  "KindHover",
                          "strength":  0.85,
                          "meaning":  "A badge under the pointer. Icon only: white on it is 2.2 to 3.5:1, never text."
                      },
                      {
                          "id":  "Frame",
                          "strength":  1.0,
                          "meaning":  "The kind at full colour: the 1 px cursor frame, the icon of a row at rest, a badge on a chosen row."
                      }
                  ],
        "legend":  [
                       {
                           "cue":  "Coloured row",
                           "meaning":  "What I picked. The strong fill in the colour of its kind, white text and white icon."
                       },
                       {
                           "cue":  "Faint tint of a row",
                           "meaning":  "What kind of thing this row is. Read the icon first; the tint confirms it."
                       },
                       {
                           "cue":  "Hue of a composition, layer or scene",
                           "meaning":  "How deep it is nested. A container never has the hue of the container around it."
                       },
                       {
                           "cue":  "A row one step brighter",
                           "meaning":  "The pointer is here."
                       },
                       {
                           "cue":  "Thin frame in the kind colour",
                           "meaning":  "The cursor: where the arrow keys are. White when it sits on the chosen row."
                       },
                       {
                           "cue":  "Thin grey frame",
                           "meaning":  "The cursor of a list that does not have the focus."
                       },
                       {
                           "cue":  "White frame around a row, or a white line between rows",
                           "meaning":  "Where the drop goes: inside this row, or between these rows."
                       },
                       {
                           "cue":  "Greyed-out row",
                           "meaning":  "Out of play: it can not take the drop, or it does not match the search. Half grey: not here, but look inside."
                       },
                       {
                           "cue":  "Small coloured square with a white icon",
                           "meaning":  "What a collapsed row holds; the tooltip gives how many."
                       },
                       {
                           "cue":  "Grey square +n",
                           "meaning":  "More kinds inside than the row can show."
                       },
                       {
                           "cue":  "Amber triangle",
                           "meaning":  "A missing asset on this row or under it."
                       },
                       {
                           "cue":  "Grey row in a tree of coloured rows",
                           "meaning":  "An empty group: nothing of that kind here yet."
                       },
                       {
                           "cue":  "Blue control",
                           "meaning":  "This control is on. Lighter blue: on and under the pointer."
                       },
                       {
                           "cue":  "Blue line around a box or panel",
                           "meaning":  "The keyboard goes here."
                       },
                       {
                           "cue":  "Green dot",
                           "meaning":  "Connected and healthy."
                       },
                       {
                           "cue":  "Orange dot or orange line",
                           "meaning":  "Needs attention: out of sync, an asset missing, why a control is off."
                       },
                       {
                           "cue":  "Red dot or red word",
                           "meaning":  "Broken, or an action that takes something away."
                       },
                       {
                           "cue":  "Grey dot",
                           "meaning":  "Still connecting."
                       },
                       {
                           "cue":  "Violet dot next to a value",
                           "meaning":  "A modulator drives this value."
                       },
                       {
                           "cue":  "Orange dot next to a value",
                           "meaning":  "A value from outside drives this value."
                       },
                       {
                           "cue":  "Dark grey dot next to a value",
                           "meaning":  "Nothing drives this value."
                       },
                       {
                           "cue":  "Coloured header band in the Inspector",
                           "meaning":  "You are editing the thing of this kind that you picked in the tree."
                       }
                   ]
};
