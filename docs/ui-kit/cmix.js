/* The small behaviours of the kit, and the templates of the controls.
   A class of the kit is the key of a ControlTheme or the name of a control class of CMiX, for example ToggleButtonBox.
   A template gives a control its parts, as a ControlTemplate does in CMiX. So the markup of a mockup stays short:
       <button class="ButtonIcon" data-icon-key="Add"></button>
       <button class="ToggleButtonCaptioned" data-caption="Enabled"></button>
       <div class="DragValue" data-caption="Speed" data-value="0.5"></div>
   The states have the names of the pseudo classes of Avalonia: pointerover, pressed, checked, disabled, editing.
   A real hover, press or disabled attribute gives the same look as the class. */
(function () {
    'use strict';

    var icons = {};
    ((window.CMIX_ICONS || {}).icons || []).forEach(function (i) { icons[i.key] = i.path; });

    function escapeHtml(text) {
        return String(text).replace(/[&<>"]/g, function (c) { return { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;' }[c]; });
    }

    function icon(key) { return '<i class="AppIcon" data-icon-key="' + key + '"></i>'; }

    function attribute(element, name) { return escapeHtml(element.getAttribute(name) || ''); }

    function number(element, name, fallback) {
        var value = parseFloat(element.getAttribute(name));
        return isNaN(value) ? fallback : value;
    }

    function isInteger(element) {
        return element.classList.contains('IntegerValue') || element.hasAttribute('data-is-integer');
    }

    /* N0 for a whole number and N3 for the others, as the templates of CMiX. */
    function format(element, value) {
        var digits = isInteger(element) ? 0 : 3;
        return value.toLocaleString('en-US', { minimumFractionDigits: digits, maximumFractionDigits: digits });
    }

    /* data-value, data-caption, data-minimum, data-maximum, data-small-change, data-is-integer. */
    function dragValue(element) {
        var inline = element.classList.contains('DragValueInlineCaption');
        var caption = attribute(element, 'data-caption');
        var text = format(element, number(element, 'data-value', 0));
        var display = inline
            ? '<span class="captionDisplay">' + caption + '</span><span class="valueDisplay">' + text + '</span>'
            : '<span class="valueDisplay">' + text + '</span>';
        var body = '<span class="mainBorder"><span class="borderValueDisplay">' + display + '</span>'
            + '<button class="SubButton">' + icon('Sub') + '</button><button class="AddButton">' + icon('Add') + '</button>'
            + '<span class="NumericInput">' + text + '</span></span>';
        if (inline) return body;
        /* A child with data-slot="leading" or data-slot="trailing" is the LeadingContent or the TrailingContent. */
        var leading = slot(element, 'leading'), trailing = slot(element, 'trailing');
        return '<span class="caption">' + caption + '</span>' + (leading || trailing ? '<span class="value">' + leading + body + trailing + '</span>' : body);
    }

    /* data-left-caption, data-right-caption, data-left-value, data-right-value, data-is-integer. */
    function rangeValue(element) {
        function side(name, position) {
            return '<div class="DragValueInlineCaption" data-position="' + position + '" data-caption="' + attribute(element, 'data-' + name + '-caption')
                + '" data-value="' + number(element, 'data-' + name + '-value', 0) + '"' + (isInteger(element) ? ' data-is-integer' : '') + '></div>';
        }
        return side('left', 'Left') + side('right', 'Right');
    }

    function fraction(element, value) {
        var min = number(element, 'data-minimum', 0), max = number(element, 'data-maximum', 1);
        return max > min ? Math.min(1, Math.max(0, (value - min) / (max - min))) : 0;
    }

    /* data-value, data-caption, data-minimum (0), data-maximum (1). */
    function slider(element) {
        var value = number(element, 'data-value', 0);
        element.style.setProperty('--Fraction', String(fraction(element, value)));
        return '<span class="sliderName">' + attribute(element, 'data-caption') + '</span>'
            + '<span class="textDisplay">' + value.toFixed(3) + '</span><span class="NumericInput">' + value.toFixed(3) + '</span>';
    }

    function captioned(element, body) {
        return '<span class="caption">' + attribute(element, 'data-caption') + '</span>' + body;
    }

    /* data-caption, and data-x, data-y, data-z for the values. The first row has the Caption and the axis. */
    function vector(axes, kind) {
        return function (element) {
            var caption = element.getAttribute('data-caption') || '';
            return '<div class="StackPanelPositioned">' + axes.map(function (axis, i) {
                return '<div class="' + kind + '" data-caption="' + escapeHtml(i === 0 ? caption + ' ' + axis : axis)
                    + '" data-value="' + number(element, 'data-' + axis.toLowerCase(), 0) + '"></div>';
            }).join('') + '</div>';
        };
    }

    /* data-caption, and data-checked with the letters of the axes that are on, for example "XZ". */
    function axisSelector(axes) {
        var themes = ['ToggleButtonInlineLeft', 'ToggleButtonInlineCenter', 'ToggleButtonInlineRight'];
        return function (element) {
            var checked = (element.getAttribute('data-checked') || '').toUpperCase();
            return captioned(element, '<span class="outerBorder">' + axes.map(function (axis, i) {
                return '<button class="' + themes[i] + (checked.indexOf(axis) >= 0 ? ' checked' : '') + '">' + axis + '</button>';
            }).join('') + '</span>');
        };
    }

    /* The children with the class ComboBoxItem are the rows. data-selected-index (0), data-is-searchable. */
    function comboBox(element) {
        var items = Array.prototype.filter.call(element.children, function (c) { return c.classList.contains('ComboBoxItem'); });
        var index = number(element, 'data-selected-index', 0);
        items.forEach(function (item, i) { item.classList.toggle('selected', i === index); });
        var head = element.classList.contains('ComboBoxWithButton')
            ? '<button class="ToggleButtonRoundedLeft toggle"><i class="Chevron"></i></button>'
            : '<span class="toggle"><i class="Chevron"></i></span><span class="contentPresenter">' + (items[index] ? items[index].innerHTML : '') + '</span>';
        return head + '<div class="Popup"><div class="DropdownFrame">'
            + (element.hasAttribute('data-is-searchable') ? '<div class="SearchBox"></div>' : '')
            + '<div class="items">' + items.map(function (i) { return i.outerHTML; }).join('') + '</div></div></div>';
    }

    function innerComboBox(element) {
        return '<div class="ComboBox' + (element.classList.contains('open') ? ' open' : '') + '" data-selected-index="' + number(element, 'data-selected-index', 0) + '"'
            + (element.hasAttribute('data-is-searchable') ? ' data-is-searchable' : '') + '>' + element.innerHTML + '</div>';
    }

    function comboBoxCaptioned(element) { return captioned(element, innerComboBox(element)); }

    function pathSelector(element) {
        return captioned(element, '<div class="DockPanelPositioned">' + innerComboBox(element)
            + '<button class="Button">' + icon('Folder') + '</button></div>');
    }

    /* data-watermark, data-text. The class showsclear gives the clear button. */
    function searchBox(element) {
        var text = attribute(element, 'data-text');
        if (text) element.classList.add('hastext');
        return '<input class="TextBox" type="text" spellcheck="false" placeholder="' + attribute(element, 'data-watermark') + '" value="' + text + '">'
            + icon('Search') + '<button class="ButtonIconClose"></button>';
    }

    /* data-text, data-suffix. A double click on the text starts the edit. */
    function textBoxEditable(element) {
        var text = attribute(element, 'data-text');
        var suffix = element.getAttribute('data-suffix');
        return '<span class="TextDisplay">' + text + '</span>' + (suffix ? '<span class="Suffix">' + escapeHtml(suffix) + '</span>' : '')
            + '<input class="TextBox" type="text" spellcheck="false" value="' + text + '">';
    }

    /* data-caption, data-ip, for example "192.168.1.10". */
    function ipBox(element) {
        var parts = (element.getAttribute('data-ip') || '...').split('.');
        return captioned(element, '<span class="parts">' + [0, 1, 2, 3].map(function (i) {
            return '<input class="TextBoxNumbersOnly" type="text" maxlength="3" value="' + escapeHtml(parts[i] || '') + '">';
        }).join('<span class="Label">.</span>') + '</span>');
    }

    /* data-color is #RRGGBB or #RRGGBBAA. Gives r, g, b, a and h, s, v, each from 0 to 1 (h from 0 to 360). */
    function colorOf(element) {
        var hex = (element.getAttribute('data-color') || '#FFFFFF').replace('#', '');
        var r = parseInt(hex.substr(0, 2), 16) / 255, g = parseInt(hex.substr(2, 2), 16) / 255, b = parseInt(hex.substr(4, 2), 16) / 255;
        var a = hex.length >= 8 ? parseInt(hex.substr(6, 2), 16) / 255 : 1;
        var max = Math.max(r, g, b), min = Math.min(r, g, b), d = max - min, h = 0;
        if (d > 0) {
            if (max === r) h = ((g - b) / d + 6) % 6;
            else if (max === g) h = (b - r) / d + 2;
            else h = (r - g) / d + 4;
            h *= 60;
        }
        return { hex: '#' + hex.toUpperCase(), r: r, g: g, b: b, a: a, h: h, s: max === 0 ? 0 : d / max, v: max };
    }

    function colorWheel(element) {
        var c = colorOf(element);
        var radius = (parseFloat(getComputedStyle(element).width) || 200) / 2 - 5;
        var angle = c.h * Math.PI / 180;
        var x = 5 + radius + Math.cos(angle) * c.s * radius - 5;
        var y = 5 + radius - Math.sin(angle) * c.s * radius - 4;
        element.style.setProperty('--V', String(c.v));
        return '<span class="wheel"></span><span class="PART_CursorEllipse" style="left:' + x.toFixed(1) + 'px;top:' + y.toFixed(1)
            + 'px;background:' + c.hex.substr(0, 7) + '"></span>';
    }

    function colorSliders(element) {
        var c = colorOf(element);
        function slider(caption, value, position) {
            return '<div class="SliderCMiX" data-caption="' + caption + '" data-value="' + value + '"' + (position ? ' data-position="' + position + '"' : '') + '></div>';
        }
        function radio(mode, position) {
            return '<button class="RadioButton' + (mode === 'HSV' ? ' checked' : '') + '" data-position="' + position + '" data-mode="' + mode + '">' + mode + '</button>';
        }
        element.setAttribute('data-mode', 'HSV');
        return '<div class="modes">' + radio('HSV', 'Left') + radio('RGB', 'None') + radio('HEX', 'Right') + '</div>'
            + '<div class="panel">'
            + '<div class="mode" data-mode="HEX"><input class="TextBox" type="text" spellcheck="false" value="' + c.hex + '"></div>'
            + '<div class="mode" data-mode="RGB">' + slider('R', c.r, 'Top') + slider('G', c.g, 'None') + slider('B', c.b, 'Bottom') + '</div>'
            + '<div class="mode" data-mode="HSV">' + slider('H', c.h / 360, 'Top') + slider('S', c.s, 'None') + slider('V', c.v, 'Bottom') + '</div>'
            + '</div>' + slider('Alpha', c.a, '');
    }

    function colorPickerFull(element) {
        var color = attribute(element, 'data-color');
        return '<div class="top"><div class="ColorPickerWheel" data-color="' + color + '"></div>'
            + '<div class="SliderCMiXBrightness" style="--V:' + colorOf(element).v + '"><span class="Part_Thumb"></span></div></div>'
            + '<div class="ColorPickerSliders" data-color="' + color + '"></div>';
    }

    function colorSelector(element) {
        var color = attribute(element, 'data-color');
        return captioned(element, '<button class="ToggleButtonColor" style="--Color:' + color + '"></button>'
            + '<div class="Popup"><div class="PopupFrame"><div class="ColorPickerFull" data-color="' + color + '"></div></div></div>');
    }

    /* Takes the child with data-slot="name" out of the element and gives its markup, or an empty text. */
    function slot(element, name) {
        var child = element.querySelector(':scope > [data-slot="' + name + '"]');
        if (!child) return '';
        child.remove();
        child.removeAttribute('data-slot');
        return child.outerHTML;
    }

    /* The rows are the children. ListBox, and ListBoxCMiX with the handle under the rows. */
    function listBox(element) { return '<div class="items">' + element.innerHTML + '</div>'; }
    function listBoxCMiX(element) {
        return '<div class="Bd"><div class="items">' + element.innerHTML + '</div></div><div class="resizeBorder">' + icon('DragHandle') + '</div>';
    }

    /* The rows are the children with the class ListBoxItemBasic. data-reorder gives each row a drag handle. */
    function listBoxEditable(element) {
        if (element.hasAttribute('data-reorder')) {
            Array.prototype.forEach.call(element.children, function (row) {
                row.insertAdjacentHTML('beforeend', '<button class="ButtonDragHandle">' + icon('DragHandle') + '</button>');
            });
        }
        return '<div class="ListBoxEditableList">' + element.innerHTML + '</div>'
            + '<div class="buttons"><div class="StackPanelPositioned"><button class="Button">+</button><button class="Button">-</button></div>'
            + '<div class="StackPanelPositioned"><button class="Button">' + icon('ArrowUp') + '</button><button class="Button">' + icon('ArrowDown') + '</button></div></div>';
    }

    /* The rows are the children. The child with data-slot="add" is the AddContent, at the left of the remove button. */
    function listBoxRepository(element) {
        var add = slot(element, 'add');
        return '<div class="ListBoxCMiX">' + element.innerHTML + '</div>'
            + '<div class="DockPanelPositioned">' + (add || '<span></span>') + '<button class="Button">' + icon('Remove') + '</button></div>';
    }

    /* data-icon-key, data-title. The class checked is the eye that is off. */
    function listItemIcon(element) {
        return icon(attribute(element, 'data-icon-key')) + '<div class="TextBoxEditable" data-text="' + attribute(element, 'data-title') + '"></div>'
            + '<button class="ToggleButtonVisibility' + (element.classList.contains('checked') ? ' checked' : '') + '"></button>';
    }

    /* data-header, or a child with data-slot="header". The rest is the content. The class collapsed closes it. */
    function expander(element) {
        var header = slot(element, 'header') || attribute(element, 'data-header');
        return '<div class="PART_toggle"><i class="Chevron"></i><span class="header">' + header + '</span></div><div class="content">' + element.innerHTML + '</div>';
    }

    /* data-title. The content is the body. The class checked is the eye that is off. */
    function panelItem(element) {
        element.classList.add('ExpanderItem');
        return '<div class="PART_toggle"><i class="Chevron"></i><span class="header"><span class="title">' + attribute(element, 'data-title') + '</span>'
            + '<button class="ToggleButtonVisibility' + (element.classList.contains('checked') ? ' checked' : '') + '"></button>'
            + '<button class="ButtonIconClose"></button><button class="ButtonDragHandle">' + icon('DragHandle') + '</button></span></div>'
            + '<div class="content">' + element.innerHTML + '</div>';
    }

    /* The children are the items. data-add-caption. The child with data-slot="add" is the content of the popup. */
    function panelItemList(element) {
        var add = slot(element, 'add');
        return element.innerHTML + '<button class="ToggleButtonPopup' + (element.classList.contains('open') ? ' checked' : '') + '">' + attribute(element, 'data-add-caption') + '</button>'
            + '<div class="Popup"><div class="DropdownFrame">' + add + '</div></div>';
    }

    /* data-caption. The content is the content of the popup. */
    function popupButton(element) {
        return '<button class="ToggleButton' + (element.classList.contains('open') ? ' checked' : '') + '">' + attribute(element, 'data-caption') + '</button>'
            + '<div class="Popup"><div class="DropdownFrame">' + element.innerHTML + '</div></div>';
    }

    /* The children with the class TabItem are the pages. A page has data-header, data-icon-key,
       or a child with data-slot="header". The class selected is the page that shows. */
    function tabs(itemTheme) {
        return function (element) {
            var strip = Array.prototype.map.call(element.querySelectorAll(':scope > .TabItem'), function (page, index) {
                var header = slot(page, 'header') || (page.hasAttribute('data-icon-key') ? icon(attribute(page, 'data-icon-key')) : attribute(page, 'data-header'));
                return '<button class="' + itemTheme + (page.classList.contains('selected') ? ' selected' : '') + '" data-index="' + index + '">' + header + '</button>';
            }).join('');
            return '<div class="strip">' + strip + '</div><div class="contentPanel">' + element.innerHTML + '</div>';
        };
    }

    /* The children are the rows: buttons with the class ButtonItem. */
    function searchablePicker(element) {
        return '<div class="SearchBox"></div><div class="ItemPickerList">' + element.innerHTML + '</div><span class="PART_NoMatch">No match</span>';
    }

    /* The children are the rows: elements with the class row, and row header for the name of a group. */
    function groupedSearchList(element) {
        return '<div class="SearchBox"></div><div class="PART_Rows ScrollViewerOverlay">' + element.innerHTML + '</div><span class="PART_NoMatch">No match</span>';
    }

    function contentCaptioned(element) { return captioned(element, element.innerHTML); }

    /* A row of a menu. The content is the header. data-icon-key, data-input-gesture.
       A child with the class items holds the rows of a submenu. The class separator is a line. */
    function menuItem(frame) {
        return function (element) {
            if (element.classList.contains('separator')) return '';
            var items = element.querySelector(':scope > .items');
            var sub = '';
            if (items) { items.remove(); sub = '<div class="Popup"><div class="' + frame + '">' + items.innerHTML + '</div></div>'; element.classList.add('opens'); }
            var key = element.getAttribute('data-icon-key');
            var header = '<span class="PART_HeaderPresenter">' + element.innerHTML.trim() + '</span>';
            var gesture = '<span class="shortcut PART_InputGestureText">' + attribute(element, 'data-input-gesture') + '</span>';
            if (element.classList.contains('MenuItem')) return (key ? icon(key) : '') + header + gesture;
            return '<span class="head">' + (key ? icon(key) : '') + header + '</span>'
                + (element.classList.contains('MenuItemTop') ? '' : gesture + '<i class="AppIcon PART_Arrow" data-icon-key="ChevronRight"></i>') + sub;
        };
    }

    /* data-tip is a plain text. Or data-title, data-shortcut, data-description, data-reason, data-footer: a TipContent.
       data-edge is the edge of the card with the tip: Top (the default), Bottom, Left or Right. */
    function toolTip(element) {
        var tip = '<i class="PopupTip" data-edge="' + (attribute(element, 'data-edge') || 'Top') + '"></i>';
        if (element.hasAttribute('data-tip')) return '<span class="title">' + attribute(element, 'data-tip') + '</span>' + tip;
        function line(name) { return element.hasAttribute('data-' + name) ? '<span class="' + name + '">' + attribute(element, 'data-' + name) + '</span>' : ''; }
        return '<span class="TipContent"><span class="line"><span class="title">' + attribute(element, 'data-title') + '</span>'
            + (element.hasAttribute('data-shortcut') ? '<span class="shortcut">' + attribute(element, 'data-shortcut') + '</span>' : '') + '</span>'
            + line('description') + line('reason') + line('footer') + '</span>' + tip;
    }

    /* ---------------------------------------------------------------- CMiX.Studio.Avalonia */

    /* data-mask is "on" or "off": the item is a composable. No data-mask: the mask square is empty.
       data-hidden: the eye is off. data-empty: no item, the two squares are empty. */
    function composableToggles(element) {
        var hasMask = element.hasAttribute('data-mask'), hasItem = !element.hasAttribute('data-empty');
        return '<span class="slot mask' + (hasMask && hasItem ? '' : ' empty') + '">'
            + (hasMask && hasItem ? '<button class="ToggleButtonMask' + (element.getAttribute('data-mask') === 'on' ? ' checked' : '') + '"></button>' : '') + '</span>'
            + '<span class="slot visibility' + (hasItem ? '' : ' empty') + '">'
            + (hasItem ? '<button class="ToggleButtonVisibility' + (element.hasAttribute('data-hidden') ? ' checked' : '') + '"></button>' : '') + '</span>';
    }

    /* data-counts is a list such as "Entity:3,Camera:1": a square with the icon of the kind for each; the count is the tooltip.
       data-more is the number of the other kinds. data-missing is the number of missing assets: the amber sign. */
    var KIND_ICONS = { Composable: 'Composition', Light: 'MeshLight' };
    function kindCountBadges(element) {
        var html = (element.getAttribute('data-counts') || '').split(',').filter(Boolean).map(function (pair) {
            var parts = pair.split(':');
            var kind = parts[0].trim();
            var base = kind.replace(/\d+$/, '');
            return '<span class="kindcount" data-kind="' + escapeHtml(kind) + '" data-tooltip="' + escapeHtml((parts[1] || '').trim()) + ' ' + escapeHtml(kind) + '">' + icon(KIND_ICONS[base] || base) + '</span>';
        }).join('');
        if (element.hasAttribute('data-more')) html += '<span class="MoreKinds">+' + attribute(element, 'data-more') + '</span>';
        if (element.hasAttribute('data-missing')) html += '<span class="MissingSign" data-tooltip="' + attribute(element, 'data-missing') + ' missing asset(s)"><span>!</span></span>';
        return html;
    }

    /* A row of the project tree. The children with the class TreeViewItem are the rows under it.
       data-kind is the kind (Composable, Entity, Camera, Light, Texture, Filter, Modifier, Modulator), data-kind-depth its depth.
       data-icon-key is the icon. data-name is the name of an item. The class group and data-header make a group row.
       data-counts, data-more and data-missing are the squares of a collapsed row. data-mask and data-hidden are the toggles.
       The classes: expanded, selected, cursor, dimmed, drophint, droptarget, dropgap, droptop. */
    function treeViewItem(element) {
        var level = 0;
        for (var p = element.parentElement; p; p = p.parentElement) { if (p.classList && p.classList.contains('TreeViewItem')) level++; }
        element.style.setProperty('--TreeLevel', String(level));
        if (element.hasAttribute('data-kind-depth')) element.style.setProperty('--KindDepth', element.getAttribute('data-kind-depth'));
        if (!element.querySelector(':scope > .TreeViewItem')) element.classList.add('empty');

        var group = element.classList.contains('group');
        var badges = element.hasAttribute('data-counts') || element.hasAttribute('data-missing')
            ? '<span class="KindCountBadges" data-counts="' + attribute(element, 'data-counts') + '"'
                + (element.hasAttribute('data-more') ? ' data-more="' + attribute(element, 'data-more') + '"' : '')
                + (element.hasAttribute('data-missing') ? ' data-missing="' + attribute(element, 'data-missing') + '"' : '') + '></span>'
            : '';
        var hidden = element.hasAttribute('data-hidden');
        var header = group
            ? '<span class="TextBlock">' + attribute(element, 'data-header') + '</span>' + badges
            : '<div class="TextBoxEditable" data-text="' + attribute(element, 'data-name') + '"></div>' + badges;
        var toggles = group
            ? '<span class="slot"><button class="ToggleButtonVisibility' + (hidden ? ' checked' : '') + '"></button></span>'
            : '<span class="ComposableToggles row"' + (element.hasAttribute('data-mask') ? ' data-mask="' + attribute(element, 'data-mask') + '"' : '') + (hidden ? ' data-hidden' : '') + '></span>';
        return '<div class="RowGrid"><button class="TreeChevronToggle"><svg viewBox="-1 -1 6 10"><path d="M 0 0 L 4 4 L 0 8"/></svg></button>'
            + '<div class="RowBorder">' + icon(attribute(element, 'data-icon-key')) + '<span class="header">' + header + '</span>'
            + '<span class="RowToggles">' + toggles + '</span></div></div>'
            + '<div class="GapUnderTheRow"></div><div class="DropLine"></div><div class="items">' + element.innerHTML + '</div>';
    }

    /* data-modulator is Internal or External: the kind of the modulator of the value. No data-modulator: none. */
    function assignButton(element) {
        var kind = element.getAttribute('data-modulator');
        return '<button class="ButtonModulatorAssign"' + (kind ? ' data-modulator="' + escapeHtml(kind) + '"' : '') + ' data-slot="trailing"></button>';
    }

    /* A DragValue or an IntegerValue with a ButtonModulatorAssign as its TrailingContent. */
    function valueModulatable(kind) {
        return function (element) {
            element.classList.add(kind);
            element.insertAdjacentHTML('beforeend', assignButton(element));
            return dragValue(element);
        };
    }

    function sliderModulatable(element) {
        return '<div class="SliderCMiX" data-caption="' + attribute(element, 'data-caption') + '" data-value="' + number(element, 'data-value', 0) + '"></div>'
            + assignButton(element).replace(' data-slot="trailing"', '');
    }

    /* data-caption is the text of the button. The class checked is the toggle that is on. */
    function buttonModulatable(theme) {
        return function (element) {
            return '<span class="value"><button class="' + theme + (element.classList.contains('checked') ? ' checked' : '') + '">' + attribute(element, 'data-caption') + '</button>'
                + assignButton(element).replace(' data-slot="trailing"', '') + '</span>';
        };
    }

    function floatRangeEditor(element) {
        return captioned(element, '<div class="RangeValue" data-left-caption="Min" data-right-caption="Max" data-left-value="' + number(element, 'data-minimum', 0)
            + '" data-right-value="' + number(element, 'data-maximum', 1) + '"></div>');
    }

    /* data-state is connected, unsynced or connecting. No data-state: disconnected. The content is the text of the status. */
    function syncStatusIndicator(element) {
        var state = element.getAttribute('data-state') || '';
        var classes = state === 'unsynced' ? 'connected unsynced' : state;
        return '<i class="EllipseSyncConnection ' + escapeHtml(classes) + '"></i><span>' + element.innerHTML + '</span>';
    }

    /* A text with {content}: the markup inside the element, {caption}: data-caption, {icon}: data-icon-key.
       Or a function that makes the markup from the element. */
    var templates = {
        ButtonCaptioned: '{caption}',
        ButtonIcon: icon('{icon}'),
        ButtonWindowIcon: icon('{icon}'),
        ButtonIconClose: icon('Close'),
        ToggleButtonColor: '<span class="solid"></span><span class="alpha"></span>',
        ToggleButtonCheckBox: icon('IsChecked'),
        ToggleButtonCheck: icon('IsChecked'),
        ToggleButtonIcon: icon('{icon}'),
        ToggleButtonVisibility: icon('Visible') + icon('NotVisible'),
        ToggleButtonPower: icon('Power'),
        ToggleButtonUpdate: icon('Update'),
        ToggleButtonPlayPause: icon('Play') + icon('Stop'),
        ToggleButtonPopup: '<span class="plus">+</span><span class="content">{content}</span>',
        ToggleButtonBox: icon('IsChecked'),
        ToggleButtonCaptioned: '<span class="box">' + icon('IsChecked') + '</span><span class="caption">{caption}</span>',
        DragValue: dragValue,
        IntegerValue: dragValue,
        DragValueInlineCaption: dragValue,
        RangeValue: rangeValue,
        SliderCMiX: slider,
        Vector2: vector(['X', 'Y'], 'DragValue'),
        Vector3: vector(['X', 'Y', 'Z'], 'DragValue'),
        Integer2: vector(['X', 'Y'], 'IntegerValue'),
        Integer3: vector(['X', 'Y', 'Z'], 'IntegerValue'),
        AxisSelector2: axisSelector(['X', 'Y']),
        AxisSelector3: axisSelector(['X', 'Y', 'Z']),
        Chevron: icon('ChevronDown'),
        ComboBox: comboBox,
        ComboBoxWithButton: comboBox,
        ComboBoxCaptioned: comboBoxCaptioned,
        PathSelector: pathSelector,
        SearchBox: searchBox,
        TextBoxEditable: textBoxEditable,
        EditableValue: textBoxEditable,
        IPBox: ipBox,
        ColorPickerWheel: colorWheel,
        ColorPickerSliders: colorSliders,
        ColorPickerFull: colorPickerFull,
        ColorSelector: colorSelector,
        ListBox: listBox,
        ListBoxCMiX: listBoxCMiX,
        ListBoxEditable: listBoxEditable,
        ListBoxRepository: listBoxRepository,
        ListItemIcon: listItemIcon,
        PanelItem: panelItem,
        PanelItemList: panelItemList,
        PopupButton: popupButton,
        SearchableItemPickerList: searchablePicker,
        GroupedSearchList: groupedSearchList,
        Expander: expander,
        ExpanderItem: expander,
        ExpanderIndented: expander,
        TabControl: tabs('TabItemCMiX'),
        TabControlMain: tabs('TabItemHorizontal'),
        TabControlVertical: tabs('TabItemHorizontal'),
        ContentCaptioned: contentCaptioned,
        MenuItemContext: menuItem('PopupFrame'),
        MenuItemTop: menuItem('SubMenuBorder'),
        MenuItem: menuItem('SubMenuBorder'),
        ToolTip: toolTip,
        ToggleButtonMask: icon('MaskOff') + icon('Mask'),
        ComposableToggles: composableToggles,
        KindCountBadges: kindCountBadges,
        TreeViewItem: treeViewItem,
        CategoryBadge: icon('{icon}'),
        DragValueModulatable: valueModulatable('DragValue'),
        IntegerValueModulatable: valueModulatable('IntegerValue'),
        Vector2Modulatable: vector(['X', 'Y'], 'DragValueModulatable'),
        Vector3Modulatable: vector(['X', 'Y', 'Z'], 'DragValueModulatable'),
        SliderModulatable: sliderModulatable,
        ToggleButtonModulatable: buttonModulatable('ToggleButton'),
        ButtonMomentaryModulatable: buttonModulatable('ButtonMomentary'),
        FloatRangeEditor: floatRangeEditor,
        SyncStatusIndicator: syncStatusIndicator,
        MeshShapeFrame: '<div class="ScaleOffset"><div class="Vector3" data-caption="Scale" data-x="1" data-y="1" data-z="1"></div><div class="Vector3" data-caption="Offset" data-x="0" data-y="0" data-z="0"></div></div>{content}<div class="IntegerValue" data-caption="Tessellation" data-value="32"></div>'
    };

    function selector() {
        return Object.keys(templates).map(function (key) { return '.' + key; }).join(',');
    }

    function expand(element) {
        if (element.hasAttribute('data-cmix')) return;
        for (var i = 0; i < element.classList.length; i++) {
            var template = templates[element.classList[i]];
            if (template === undefined) continue;
            element.innerHTML = typeof template === 'function'
                ? template(element)
                : template
                    .replace('{content}', element.innerHTML)
                    .replace('{caption}', attribute(element, 'data-caption'))
                    .replace('{icon}', attribute(element, 'data-icon-key'));
            element.setAttribute('data-cmix', '');
            Array.prototype.forEach.call(element.querySelectorAll(selector()), expand);
            return;
        }
    }

    function drawIcon(element) {
        if (element.firstChild) return;
        var key = element.getAttribute('data-icon-key');
        var size = element.getAttribute('data-size');
        if (size) { element.style.width = size + 'px'; element.style.height = size + 'px'; }
        if (!icons[key]) { element.title = 'Unknown icon key: ' + key; return; }
        element.innerHTML = '<svg viewBox="0 0 24 24" aria-hidden="true"><path d="' + icons[key] + '"/></svg>';
    }

    /* Gives every control inside root its parts and its icons. Call it again after you add markup. */
    function apply(root) {
        Array.prototype.forEach.call(root.querySelectorAll(selector()), expand);
        Array.prototype.forEach.call(root.querySelectorAll('.AppIcon'), drawIcon);
    }

    function isKind(element, prefix) {
        for (var i = 0; i < element.classList.length; i++) { if (element.classList[i].indexOf(prefix) === 0) return true; }
        return false;
    }

    function closestKind(start, prefixes) {
        for (var e = start; e && e.classList; e = e.parentElement) {
            for (var i = 0; i < prefixes.length; i++) { if (isKind(e, prefixes[i])) return e; }
        }
        return null;
    }

    function isDisabled(element) { return element.disabled || element.classList.contains('disabled'); }

    /* The buttons of a DragValue change the value by SmallChange: 0.0001, or 1 for a whole number. */
    function stepDragValue(button) {
        var host = closestKind(button, ['DragValue', 'IntegerValue']);
        if (!host || isDisabled(host)) return;
        var change = number(host, 'data-small-change', isInteger(host) ? 1 : 0.0001);
        var value = number(host, 'data-value', 0) + (button.classList.contains('AddButton') ? change : -change);
        value = Math.min(number(host, 'data-maximum', 10000), Math.max(number(host, 'data-minimum', -10000), value));
        host.setAttribute('data-value', String(value));
        var text = format(host, value);
        host.querySelector('.valueDisplay').textContent = text;
        host.querySelector('.NumericInput').textContent = text;
    }

    document.addEventListener('click', function (e) {
        if (!e.target.closest || !e.target.closest('.cmix')) return;

        var step = e.target.closest('.SubButton, .AddButton');
        if (step) { stepDragValue(step); return; }

        /* A drop down opens under its toggle. A click on a row picks it. A click outside closes it. */
        var combo = e.target.closest('.ComboBox, .ComboBoxWithButton');
        var inPopup = e.target.closest('.Popup');
        Array.prototype.forEach.call(document.querySelectorAll('.cmix .ComboBox.open, .cmix .ComboBoxWithButton.open'), function (open) {
            if (open !== combo && !open.hasAttribute('data-keep-open')) open.classList.remove('open');
        });
        if (combo) {
            var item = e.target.closest('.ComboBoxItem');
            if (item && inPopup) {
                Array.prototype.forEach.call(item.parentElement.children, function (i) { i.classList.remove('selected'); });
                item.classList.add('selected');
                var presenter = combo.querySelector(':scope > .contentPresenter');
                if (presenter) presenter.innerHTML = item.innerHTML;
                if (!combo.hasAttribute('data-keep-open')) combo.classList.remove('open');
            }
            else if (e.target.closest('.toggle') && !isDisabled(combo) && !combo.hasAttribute('data-keep-open')) {
                combo.classList.toggle('open');
            }
            return;
        }

        var clear = e.target.closest('.SearchBox > .ButtonIconClose');
        if (clear) {
            var field = clear.parentElement.querySelector('.TextBox');
            field.value = '';
            field.dispatchEvent(new Event('input', { bubbles: true }));
            field.focus();
            return;
        }

        /* The chevron of a row of the tree opens and closes the row. A click on the row selects it. */
        var chevron = e.target.closest('.TreeChevronToggle');
        if (chevron) { chevron.closest('.TreeViewItem').classList.toggle('expanded'); return; }
        var treeRow = e.target.closest('.RowBorder');
        if (treeRow && !e.target.closest('.RowBorder button')) {
            var item = treeRow.closest('.TreeViewItem');
            var scope = item.closest('.TreeView') || item.closest('.cmix');
            Array.prototype.forEach.call(scope.querySelectorAll('.TreeViewItem.selected'), function (r) { if (!r.hasAttribute('data-keep-state')) r.classList.remove('selected'); });
            if (!item.hasAttribute('data-keep-state')) item.classList.add('selected');
        }

        /* A row of the menu bar opens its drop down. A click on another place closes it. */
        var top = e.target.closest('.MenuItemTop');
        Array.prototype.forEach.call(document.querySelectorAll('.cmix .MenuItemTop.selected'), function (open) {
            if (open !== top && !open.hasAttribute('data-keep-open')) open.classList.remove('selected');
        });
        if (top && !isDisabled(top) && !top.hasAttribute('data-keep-open')) {
            if (e.target.closest('.MenuItem')) top.classList.remove('selected');
            else top.classList.toggle('selected');
            return;
        }

        /* A tab shows its page. */
        var tab = e.target.closest('.strip > .TabItemCMiX, .strip > .TabItemHorizontal');
        if (tab && !isDisabled(tab)) {
            var pages = tab.parentElement.parentElement.querySelectorAll(':scope > .contentPanel > .TabItem');
            Array.prototype.forEach.call(tab.parentElement.children, function (t) { t.classList.toggle('selected', t === tab); });
            Array.prototype.forEach.call(pages, function (p, i) { p.classList.toggle('selected', String(i) === tab.getAttribute('data-index')); });
            return;
        }

        var target = closestKind(e.target, ['ToggleButton', 'RadioButton']);

        /* The header of an expander opens and closes it. A button in the header keeps its own click. */
        var header = e.target.closest('.PART_toggle');
        if (header && !target && !e.target.closest('.PART_toggle button') && !isDisabled(header.parentElement)) {
            header.parentElement.classList.toggle('collapsed');
            return;
        }

        /* A click on a row of a list selects it. */
        var row = e.target.closest('.ListBoxItem, .ListBoxItemCMiX, .ListBoxItemBasic');
        if (row && !target && !isDisabled(row)) {
            Array.prototype.forEach.call(row.parentElement.children, function (r) { r.classList.toggle('selected', r === row); });
        }

        if (!target || isDisabled(target)) return;

        /* The toggle of a ColorSelector, a PopupButton or a PanelItemList opens its popup. A RadioButton of a ColorPickerSliders picks the mode. */
        var owner = target.parentElement;
        if (/\b(ColorSelector|PopupButton|PanelItemList)\b/.test(owner.className)) {
            if (owner.hasAttribute('data-keep-open')) return;
            owner.classList.toggle('open');
        }
        if (target.hasAttribute('data-mode')) {
            var sliders = target.closest('.ColorPickerSliders');
            if (sliders) sliders.setAttribute('data-mode', target.getAttribute('data-mode'));
        }
        if (isKind(target, 'RadioButton')) {
            var group = target.getAttribute('data-group-name');
            Array.prototype.forEach.call(target.parentElement.children, function (sibling) {
                if (isKind(sibling, 'RadioButton') && sibling.getAttribute('data-group-name') === group) sibling.classList.remove('checked');
            });
            target.classList.add('checked');
        }
        else {
            target.classList.toggle('checked');
        }
    });

    /* A press or a drag on a slider sets its value from the place of the pointer. */
    function setSlider(host, clientX) {
        var box = host.getBoundingClientRect();
        var f = Math.min(1, Math.max(0, (clientX - box.left) / box.width));
        var min = number(host, 'data-minimum', 0), max = number(host, 'data-maximum', 1);
        var value = min + f * (max - min);
        host.setAttribute('data-value', String(value));
        host.style.setProperty('--Fraction', String(f));
        host.querySelector('.textDisplay').textContent = value.toFixed(3);
        host.querySelector('.NumericInput').textContent = value.toFixed(3);
    }
    document.addEventListener('pointerdown', function (e) {
        var host = e.target.closest ? e.target.closest('.cmix .SliderCMiX') : null;
        if (!host || e.button !== 0 || isDisabled(host) || host.classList.contains('editing')) return;
        host.setPointerCapture(e.pointerId);
        setSlider(host, e.clientX);
        function move(m) { setSlider(host, m.clientX); }
        function up() { host.removeEventListener('pointermove', move); host.removeEventListener('pointerup', up); host.removeEventListener('pointercancel', up); }
        host.addEventListener('pointermove', move);
        host.addEventListener('pointerup', up);
        host.addEventListener('pointercancel', up);
    });

    /* A search field knows that it holds text, and filters the rows of its drop down. */
    document.addEventListener('input', function (e) {
        var box = e.target.closest ? e.target.closest('.cmix .SearchBox') : null;
        if (!box) return;
        box.classList.toggle('hastext', e.target.value !== '');
        var frame = box.closest('.DropdownFrame');
        if (!frame) return;
        var text = e.target.value.toLowerCase();
        Array.prototype.forEach.call(frame.querySelectorAll('.ComboBoxItem'), function (item) {
            item.style.display = item.textContent.toLowerCase().indexOf(text) >= 0 ? '' : 'none';
        });
    });

    /* A double click on the text of a TextBoxEditable or an EditableValue starts the edit.
       Enter or a click outside keeps the text. Escape keeps the old text. An empty text is not a name. */
    function endEdit(host, keep) {
        var input = host.querySelector(':scope > .TextBox');
        var display = host.querySelector(':scope > .TextDisplay');
        if (keep && input.value !== '') display.textContent = input.value;
        host.classList.remove('editing');
    }
    document.addEventListener('dblclick', function (e) {
        var display = e.target.closest ? e.target.closest('.cmix .TextBoxEditable > .TextDisplay, .cmix .EditableValue > .TextDisplay') : null;
        if (!display) return;
        var host = display.parentElement;
        var input = host.querySelector(':scope > .TextBox');
        input.value = display.textContent;
        host.classList.add('editing');
        input.focus();
        input.select();
    });
    document.addEventListener('keydown', function (e) {
        var host = e.target.closest ? e.target.closest('.cmix .TextBoxEditable.editing, .cmix .EditableValue.editing') : null;
        if (!host) return;
        if (e.key === 'Enter') endEdit(host, true);
        else if (e.key === 'Escape') endEdit(host, false);
    });
    document.addEventListener('focusout', function (e) {
        var host = e.target.closest ? e.target.closest('.cmix .TextBoxEditable.editing, .cmix .EditableValue.editing') : null;
        if (host && !host.hasAttribute('data-keep-editing')) endEdit(host, true);
    });

    /* An element with data-tooltip shows a ToolTip under it, 8 away, with the tip at the pointer.
       data-tooltip-shortcut and data-tooltip-description add lines. */
    var shown = null, timer = 0;
    function hideToolTip() {
        clearTimeout(timer);
        if (shown) { shown.remove(); shown = null; }
    }
    document.addEventListener('mouseover', function (e) {
        var owner = e.target.closest ? e.target.closest('.cmix [data-tooltip]') : null;
        if (!owner) { hideToolTip(); return; }
        if (shown && shown.owner === owner) return;
        hideToolTip();
        var x = e.clientX;
        timer = setTimeout(function () {
            var root = owner.closest('.cmix');
            var rootBox = root.getBoundingClientRect(), ownerBox = owner.getBoundingClientRect();
            var scale = rootBox.width / root.offsetWidth || 1;
            var tip = document.createElement('div');
            tip.className = 'ToolTip';
            if (owner.hasAttribute('data-tooltip-shortcut') || owner.hasAttribute('data-tooltip-description')) {
                tip.setAttribute('data-title', owner.getAttribute('data-tooltip'));
                if (owner.hasAttribute('data-tooltip-shortcut')) tip.setAttribute('data-shortcut', owner.getAttribute('data-tooltip-shortcut'));
                if (owner.hasAttribute('data-tooltip-description')) tip.setAttribute('data-description', owner.getAttribute('data-tooltip-description'));
            }
            else {
                tip.setAttribute('data-tip', owner.getAttribute('data-tooltip'));
            }
            tip.style.position = 'absolute';
            tip.style.zIndex = '20';
            tip.style.left = ((x - rootBox.left) / scale - 20) + 'px';
            tip.style.top = ((ownerBox.bottom - rootBox.top) / scale + 8) + 'px';
            root.appendChild(tip);
            expand(tip);
            tip.owner = owner;
            shown = tip;
        }, 400);
    });
    document.addEventListener('mousedown', hideToolTip);

    window.CMiX = { apply: apply, templates: templates, icons: icons };

    document.addEventListener('DOMContentLoaded', function () { apply(document); });
})();
