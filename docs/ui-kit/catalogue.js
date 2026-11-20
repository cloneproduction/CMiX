/* The catalogue of the UI kit: the parts, the categories and the entries, with their fixed numbers.
   Never change a number and never use a number again. A new entry gets the next free number of its category.
   A name is the exact name of the class or the style key in CMiX. `built` says that gallery.html shows the entry. */
window.CMIX_CATALOGUE = [
    {
        part: 'A', title: 'CMiX.UI', categories: [
            { id: '0', name: 'Tokens', entries: [] },
            {
                id: '1', name: 'Boolean', entries: [
                    { id: '1.1', name: 'Button', source: 'CMiX.UI/Themes/Styles/Button.axaml' },
                    { id: '1.2', name: 'ButtonCaptioned', source: 'CMiX.UI/Controls/Boolean/ButtonCaptioned.cs' },
                    { id: '1.3', name: 'ButtonIcon', source: 'CMiX.UI/Controls/Boolean/ButtonIcon.cs' },
                    { id: '1.4', name: 'ButtonMomentary', source: 'CMiX.UI/Controls/Boolean/ButtonMomentary.cs' },
                    { id: '1.5', name: 'IconButton', source: 'CMiX.UI/Themes/Styles/IconButton.axaml' },
                    { id: '1.6', name: 'ToggleButtonCMiX', source: 'CMiX.UI/Controls/Boolean/ToggleButtonCMiX.cs' },
                    { id: '1.7', name: 'ToggleButtonCaptioned', source: 'CMiX.UI/Controls/Boolean/ToggleButtonCaptioned.cs' },
                    { id: '1.8', name: 'RadioButton', source: 'CMiX.UI/Themes/Styles/RadioButton.axaml' }
                ]
            },
            {
                id: '2', name: 'Number', entries: [
                    { id: '2.1', name: 'DragValue', source: 'CMiX.UI/Controls/Number/DragValue.cs' },
                    { id: '2.2', name: 'NumericInput', source: 'CMiX.UI/Controls/Number/NumericInput.cs' },
                    { id: '2.3', name: 'DragValueInteger', source: 'CMiX.UI/Controls/Number/DragValueInteger.cs' },
                    { id: '2.4', name: 'RangeValue', source: 'CMiX.UI/Controls/Number/RangeValue.axaml' },
                    { id: '2.5', name: 'SliderCMiX', source: 'CMiX.UI/Controls/Number/SliderCMiX.cs' }
                ]
            },
            {
                id: '3', name: 'Vector', entries: [
                    { id: '3.1', name: 'Vector2', source: 'CMiX.UI/Controls/Vector/Vector2.axaml' },
                    { id: '3.2', name: 'Vector3', source: 'CMiX.UI/Controls/Vector/Vector3.axaml' },
                    { id: '3.3', name: 'Integer2', source: 'CMiX.UI/Controls/Vector/Integer2.axaml' },
                    { id: '3.4', name: 'Integer3', source: 'CMiX.UI/Controls/Vector/Integer3.axaml' },
                    { id: '3.5', name: 'AxisSelector2', source: 'CMiX.UI/Controls/Vector/AxisSelector2.axaml' },
                    { id: '3.6', name: 'AxisSelector3', source: 'CMiX.UI/Controls/Vector/AxisSelector3.axaml' }
                ]
            },
            {
                id: '4', name: 'Enum', entries: [
                    { id: '4.1', name: 'ComboBox', source: 'CMiX.UI/Themes/Styles/ComboBox.axaml' },
                    { id: '4.2', name: 'ComboBoxCaptioned', source: 'CMiX.UI/Controls/Enum/ComboBoxCaptioned.axaml' },
                    { id: '4.3', name: 'ComboBoxSearch', source: 'CMiX.UI/Controls/Parts/ComboBoxSearch.cs' }
                ]
            },
            {
                id: '5', name: 'String', entries: [
                    { id: '5.1', name: 'TextBlock', source: 'CMiX.UI/Themes/Styles/TextBlock.axaml' },
                    { id: '5.2', name: 'TextBlockTrimmed', source: 'CMiX.UI/Controls/String/TextBlockTrimmed.cs' },
                    { id: '5.3', name: 'Label', source: 'CMiX.UI/Themes/Styles/Label.axaml' },
                    { id: '5.4', name: 'TextBox', source: 'CMiX.UI/Themes/Styles/TextBox.axaml' },
                    { id: '5.5', name: 'TextBoxEditable', source: 'CMiX.UI/Controls/String/TextBoxEditable.axaml' },
                    { id: '5.6', name: 'EditableValue', source: 'CMiX.UI/Controls/String/EditableValue.axaml' },
                    { id: '5.7', name: 'TextBoxSearch', source: 'CMiX.UI/Controls/Parts/TextBoxSearch.axaml' },
                    { id: '5.8', name: 'TextBoxIP', source: 'CMiX.UI/Controls/String/TextBoxIP.axaml' },
                    { id: '5.9', name: 'TextBoxKeyGesture', source: 'CMiX.UI/Controls/String/TextBoxKeyGesture.axaml' },
                    { id: '5.10', name: 'ContentControlKeyCap', source: 'CMiX.UI/Themes/Styles/KeyCap.axaml' },
                    { id: '5.11', name: 'KeyHintStrip', source: 'CMiX.UI/Controls/Parts/KeyHintStrip.cs' }
                ]
            },
            {
                id: '6', name: 'Path', entries: [
                    { id: '6.1', name: 'ComboBoxPath', source: 'CMiX.UI/Controls/Path/ComboBoxPath.axaml' }
                ]
            },
            {
                id: '7', name: 'ColorPicker', entries: [
                    { id: '7.1', name: 'SelectorColor', source: 'CMiX.UI/Controls/ColorPicker/SelectorColor.axaml' },
                    { id: '7.2', name: 'ColorPickerFull', source: 'CMiX.UI/Controls/ColorPicker/ColorPickerFull.axaml' },
                    { id: '7.3', name: 'ColorPickerSliders', source: 'CMiX.UI/Controls/ColorPicker/ColorPickerSliders.axaml' },
                    { id: '7.4', name: 'ColorPickerWheel', source: 'CMiX.UI/Controls/ColorPicker/ColorPickerWheel.cs' }
                ]
            },
            {
                id: '8', name: 'Collection', entries: [
                    { id: '8.1', name: 'ListBoxCMiX', source: 'CMiX.UI/Controls/ListBoxCMiX.cs' },
                    { id: '8.2', name: 'ListBoxEditable', source: 'CMiX.UI/Controls/Collection/ListBoxEditable.axaml' },
                    { id: '8.3', name: 'ListBoxRepository', source: 'CMiX.UI/Controls/Collection/ListBoxRepository.axaml' },
                    { id: '8.4', name: 'ListItemIcon', source: 'CMiX.UI/Controls/Collection/ListItemIcon.axaml' },
                    { id: '8.5', name: 'PanelItem', source: 'CMiX.UI/Controls/Collection/PanelItem.cs' },
                    { id: '8.6', name: 'ListPanelItem', source: 'CMiX.UI/Controls/Collection/ListPanelItem.axaml' },
                    { id: '8.7', name: 'ListItemPicker', source: 'CMiX.UI/Controls/Collection/ListItemPicker.axaml' },
                    { id: '8.8', name: 'ListSearchableItemPicker', source: 'CMiX.UI/Controls/Collection/ListSearchableItemPicker.axaml' },
                    { id: '8.9', name: 'ListGroupedSearch', source: 'CMiX.UI/Controls/Collection/ListGroupedSearch.axaml' }
                ]
            },
            {
                id: '9', name: 'Popups and menus', entries: [
                    { id: '9.1', name: 'Menu', source: 'CMiX.UI/Themes/Styles/Menu/Menu.axaml' },
                    { id: '9.2', name: 'ContextMenu', source: 'CMiX.UI/Themes/Styles/Menu/ContextMenu.axaml' },
                    { id: '9.3', name: 'ContextMenuTitled', source: 'CMiX.UI/Controls/Parts/ContextMenuTitled.cs' },
                    { id: '9.4', name: 'ValueMenu', source: 'CMiX.UI/Controls/Parts/ValueMenu.cs' },
                    { id: '9.5', name: 'DropdownFrame', source: 'CMiX.UI/Controls/Parts/DropdownFrame.cs' },
                    { id: '9.6', name: 'PopupFrame', source: 'CMiX.UI/Controls/Parts/PopupFrame.cs' },
                    { id: '9.7', name: 'PopupCard', source: 'CMiX.UI/Controls/Parts/PopupCard.cs' },
                    { id: '9.8', name: 'PopupFlyout', source: 'CMiX.UI/Themes/Styles/Menu/PopupFlyout.axaml' },
                    { id: '9.9', name: 'ButtonPopup', source: 'CMiX.UI/Controls/ButtonPopup.axaml' },
                    { id: '9.10', name: 'ToolTipCard', source: 'CMiX.UI/Controls/Parts/ToolTipCard.cs' },
                    { id: '9.11', name: 'PopupTip', source: 'CMiX.UI/Controls/Parts/PopupTip.cs' }
                ]
            },
            {
                id: '10', name: 'Layout and containers', entries: [
                    { id: '10.1', name: 'Window', source: 'CMiX.UI/Themes/Styles/Windows/Window.axaml' },
                    { id: '10.2', name: 'Tab', source: 'CMiX.UI/Themes/Styles/Tab.axaml' },
                    { id: '10.3', name: 'Expander', source: 'CMiX.UI/Themes/Styles/Expander.axaml' },
                    { id: '10.4', name: 'ExpanderIndented', source: 'CMiX.UI/Panels/ExpanderIndented.cs' },
                    { id: '10.5', name: 'Chevron', source: 'CMiX.UI/Controls/Parts/Chevron.cs' },
                    { id: '10.6', name: 'GridSplitter', source: 'CMiX.UI/Themes/Styles/GridSplitter.axaml' },
                    { id: '10.7', name: 'ScrollViewer', source: 'CMiX.UI/Themes/Styles/ScrollViewer.axaml' },
                    { id: '10.8', name: 'Border', source: 'CMiX.UI/Themes/Styles/Border.axaml' },
                    { id: '10.9', name: 'ContentCaptioned', source: 'CMiX.UI/Controls/Parts/ContentCaptioned.cs' },
                    { id: '10.10', name: 'CursorRow', source: 'CMiX.UI/Themes/Styles/CursorRow.axaml' },
                    { id: '10.11', name: 'RowStripes', source: 'CMiX.UI/Controls/Parts/RowStripes.cs' },
                    { id: '10.12', name: 'PanelFocus', source: 'CMiX.UI/Themes/Styles/PanelFocus.axaml' },
                    { id: '10.13', name: 'AppIcon', source: 'CMiX.UI/Controls/AppIcon.axaml' }
                ]
            }
        ]
    },
    {
        part: 'B', title: 'CMiX.Studio.Avalonia', categories: [
            {
                id: '20', name: 'Shell', entries: [
                    { id: '20.1', name: 'MainMenu', source: 'CMiX.Studio.Avalonia/Views/Menus/MainMenu.axaml' },
                    { id: '20.2', name: 'PanelMainTabs', source: 'CMiX.Studio.Avalonia/Views/Panels/PanelMainTabs.axaml' },
                    { id: '20.3', name: 'SidePanelHost', source: 'CMiX.Studio.Avalonia/Views/Panels/SidePanelHost.axaml' },
                    { id: '20.4', name: 'Inspector', source: 'CMiX.Studio.Avalonia/Views/Inspector/Inspector.axaml' },
                    { id: '20.5', name: 'Outliner', source: 'CMiX.Studio.Avalonia/Views/Controls/Outliner.axaml' },
                    { id: '20.6', name: 'MessageDialog', source: 'CMiX.Studio.Avalonia/Views/Dialogs/MessageDialog.axaml' },
                    { id: '20.7', name: 'ShortcutSettingsView', source: 'CMiX.Studio.Avalonia/Views/ShortcutSettingsView.axaml' }
                ]
            },
            {
                id: '21', name: 'Tree', entries: [
                    { id: '21.1', name: 'ProjectTree', source: 'CMiX.Studio.Avalonia/Themes/Styles/ProjectTree.axaml' },
                    { id: '21.2', name: 'ProjectTreeToolbar', source: 'CMiX.Studio.Avalonia/Views/Tree/ProjectTreeToolbar.axaml' },
                    { id: '21.3', name: 'TreeSearchBox', source: 'CMiX.Studio.Avalonia/Views/Tree/TreeSearchBox.axaml' },
                    { id: '21.4', name: 'RowToggles', source: 'CMiX.Studio.Avalonia/Views/Tree/RowToggles.axaml' },
                    { id: '21.5', name: 'BadgeCategory', source: 'CMiX.Studio.Avalonia/Views/Tree/BadgeCategory.axaml' },
                    { id: '21.6', name: 'KindCountBadges', source: 'CMiX.Studio.Avalonia/Views/Tree/KindCountBadges.axaml' },
                    { id: '21.7', name: 'AddPanel', source: 'CMiX.Studio.Avalonia/Views/Tree/AddPanel.cs' },
                    { id: '21.8', name: 'TreeDragGhost', source: 'CMiX.Studio.Avalonia/Views/Tree/TreeDragGhost.cs' },
                    { id: '21.9', name: 'TreeDropMarker', source: 'CMiX.Studio.Avalonia/Views/Tree/TreeDropMarker.cs' },
                    { id: '21.10', name: 'CategoryPalette', source: 'CMiX.Studio.Avalonia/Themes/CategoryPalette.cs' }
                ]
            },
            {
                id: '22', name: 'Managers', entries: [
                    { id: '22.1', name: 'PrefabListItemTemplates', source: 'CMiX.Studio.Avalonia/Views/Managers/PrefabListItemTemplates.axaml' },
                    { id: '22.2', name: 'PrefabRowMenu', source: 'CMiX.Studio.Avalonia/Views/Managers/PrefabRowMenu.cs' },
                    { id: '22.3', name: 'PrefabSlotManager', source: 'CMiX.Studio.Avalonia/Views/Managers/PrefabSlotManager.axaml' },
                    { id: '22.4', name: 'RepositoryTab', source: 'CMiX.Studio.Avalonia/Views/Managers/RepositoryTab.axaml' },
                    { id: '22.5', name: 'ComboBoxAssetPath', source: 'CMiX.Studio.Avalonia/Views/Assets/ComboBoxAssetPath.cs' },
                    { id: '22.6', name: 'PrefabSlotEditor', source: 'CMiX.Studio.Avalonia/Views/Managers/PrefabSlotEditor.axaml' }
                ]
            },
            {
                id: '23', name: 'Messenger', entries: [
                    { id: '23.1', name: 'BadgeSyncStatus', source: 'CMiX.Studio.Avalonia/Views/Messenger/BadgeSyncStatus.axaml' },
                    { id: '23.2', name: 'StatusIndicators', source: 'CMiX.Studio.Avalonia/Themes/Styles/StatusIndicators.axaml' }
                ]
            },
            {
                id: '24', name: 'Beat', entries: [
                    { id: '24.1', name: 'PanelBeat', source: 'CMiX.Studio.Avalonia/Views/Beat/PanelBeat.axaml' },
                    { id: '24.2', name: 'PanelMasterBeat', source: 'CMiX.Studio.Avalonia/Views/Beat/PanelMasterBeat.axaml' },
                    { id: '24.3', name: 'Quantize', source: 'CMiX.Studio.Avalonia/Views/Beat/Quantize.axaml' },
                    { id: '24.4', name: 'Easing', source: 'CMiX.Studio.Avalonia/Views/Beat/Easing.axaml' }
                ]
            },
            {
                id: '25', name: 'Modulation', entries: [
                    { id: '25.1', name: 'DragValueModulatable', source: 'CMiX.Studio.Avalonia/Views/Modulation/DragValueModulatable.axaml' },
                    { id: '25.2', name: 'SliderModulatable', source: 'CMiX.Studio.Avalonia/Views/Modulation/SliderModulatable.axaml' },
                    { id: '25.3', name: 'DragValueIntegerModulatable', source: 'CMiX.Studio.Avalonia/Views/Modulation/DragValueIntegerModulatable.axaml' },
                    { id: '25.4', name: 'ToggleButtonModulatable', source: 'CMiX.Studio.Avalonia/Views/Modulation/ToggleButtonModulatable.axaml' },
                    { id: '25.5', name: 'ButtonMomentaryModulatable', source: 'CMiX.Studio.Avalonia/Views/Modulation/ButtonMomentaryModulatable.axaml' },
                    { id: '25.6', name: 'Vector2Modulatable', source: 'CMiX.Studio.Avalonia/Views/Modulation/Vector2Modulatable.axaml' },
                    { id: '25.7', name: 'Vector3Modulatable', source: 'CMiX.Studio.Avalonia/Views/Modulation/Vector3Modulatable.axaml' },
                    { id: '25.8', name: 'PanelModulatable', source: 'CMiX.Studio.Avalonia/Views/Modulation/PanelModulatable.axaml' },
                    { id: '25.9', name: 'ButtonModulatorAssign', source: 'CMiX.Studio.Avalonia/Views/Modulation/ButtonModulatorAssign.axaml' },
                    { id: '25.10', name: 'FloatRangeEditor', source: 'CMiX.Studio.Avalonia/Views/Modulation/FloatRangeEditor.axaml' },
                    { id: '25.11', name: 'SelectorRedisAddress', source: 'CMiX.Studio.Avalonia/Views/Modulation/SelectorRedisAddress.axaml' }
                ]
            },
            {
                id: '26', name: 'Composable', entries: [
                    { id: '26.1', name: 'ComposableManagerItem', source: 'CMiX.Studio.Avalonia/Views/Component/Composable/ComposableManagerItem.axaml' },
                    { id: '26.2', name: 'ToolbarComposable', source: 'CMiX.Studio.Avalonia/Views/Component/Composable/ToolbarComposable.axaml' },
                    { id: '26.3', name: 'ComposableToggles', source: 'CMiX.Studio.Avalonia/Views/Component/Composable/ComposableToggles.axaml' },
                    { id: '26.4', name: 'ComposableListBox', source: 'CMiX.Studio.Avalonia/Themes/Styles/ComposableListBox.axaml' }
                ]
            },
            {
                id: '27', name: 'Panels', entries: [
                    { id: '27.1', name: 'Parameter panel', source: 'CMiX.Studio.Avalonia/Views/Modifiers, Views/Texturing, Views/Component' },
                    { id: '27.2', name: 'ModifierEntry', source: 'CMiX.Studio.Avalonia/Views/Modifiers/ModifierEntry.cs', removed: 'It is a data class with no look of its own. See 27.8.' },
                    { id: '27.3', name: 'ModifierModeSelector', source: 'CMiX.Studio.Avalonia/Views/Modifiers/ModifierModeSelector.axaml' },
                    { id: '27.4', name: 'SectionEntityModifiers', source: 'CMiX.Studio.Avalonia/Themes/Styles/SectionEntityModifiers.axaml' },
                    { id: '27.5', name: 'TextureSlot', source: 'CMiX.Studio.Avalonia/Views/Texturing/TextureSlot.axaml' },
                    { id: '27.6', name: 'PlayerTransport', source: 'CMiX.Studio.Avalonia/Views/Texturing/Source/PlayerTransport.axaml' },
                    { id: '27.7', name: 'TypeWriter', source: 'CMiX.Studio.Avalonia/Themes/Styles/TypeWriter.axaml' },
                    { id: '27.8', name: 'ModifierManager', source: 'CMiX.Studio.Avalonia/Views/Modifiers/ModifierManager.axaml' },
                    { id: '27.9', name: 'MeshShapeFrame', source: 'CMiX.Studio.Avalonia/Themes/Styles/MeshShapeFrame.axaml' },
                    { id: '27.10', name: 'SectionEntityTexture', source: 'CMiX.Studio.Avalonia/Themes/Styles/SectionEntityTexture.axaml' }
                ]
            }
        ]
    }
];
