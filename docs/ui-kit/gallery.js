/* Builds the canvas of gallery.html from catalogue.js, tokens.js and the templates of the entries.
   A row of the canvas holds boards. A board is a category, or a group of tokens. A card is an entry.
   A line in a card is a variant with a fixed letter. A cell is one state of a variant.
   Addresses: gallery.html#1.6 (an entry), #1.6.a (a variant), #1.6.a/hover (a state), #Grey500 (a token), #readme. */
(function () {
    'use strict';

    var catalogue = window.CMIX_CATALOGUE || [];
    var tokens = window.CMIX_TOKENS || { groups: [], skipped: [] };
    var nav = document.getElementById('nav');
    var world = document.getElementById('world');
    var filter = document.getElementById('filter');
    var status = document.getElementById('status');
    var crumb = document.getElementById('crumb');
    var copyButton = document.getElementById('copy');
    var targets = {};
    var items = [];
    var navLinks = {};

    function el(tag, className, text) {
        var e = document.createElement(tag);
        if (className) e.className = className;
        if (text !== undefined) e.textContent = text;
        return e;
    }

    /* kind: board, entry, variant, state, sample, token, icon. path: the names from the part down to the item. */
    function register(key, element, options) {
        var target = {
            key: key, element: element, kind: options.kind, zoom: options.zoom || 1, path: options.path || [key],
            board: options.board || null, card: options.card || null, rect: options.rect || null,
            search: options.search ? options.search.toLowerCase() : ''
        };
        targets[key.toLowerCase()] = target;
        element.setAttribute('data-ref', key);
        if (target.search) items.push(target);
        return target;
    }

    function demoOf(entry) { return document.getElementById('entry-' + entry.id); }

    /* ------------------------------------------------------------------ rows and boards */

    function row(title, subtitle) {
        var r = el('div', 'g-row');
        var l = el('div', 'g-rowlabel', title);
        if (subtitle) l.appendChild(el('small', '', subtitle));
        r.appendChild(l);
        world.appendChild(r);
        return r;
    }

    function board(parent, id, name, source, width, partLabel) {
        var b = el('section', 'g-board' + (width ? ' ' + width : ''));
        var l = el('h2', 'g-boardlabel');
        l.appendChild(el('span', 'g-id', id));
        l.appendChild(el('span', 'g-boardname', name));
        if (source) l.appendChild(el('span', 'g-source', source));
        l.addEventListener('click', function () { go(id, true, true); });
        b.appendChild(l);
        parent.appendChild(b);
        register(id, b, { kind: 'board', zoom: 1, path: [partLabel, id + ' ' + name], board: id, search: id + ' ' + name });
        return b;
    }

    function navLink(id, name, count, countTitle, sub) {
        var a = el('a', sub ? 'g-sub' : '');
        a.href = '#' + id;
        a.appendChild(el('span', 'g-id', id));
        a.appendChild(el('span', 'g-navname', name));
        if (count) {
            var c = el('span', 'g-count', count);
            c.title = countTitle || '';
            a.appendChild(c);
        }
        nav.appendChild(a);
        navLinks[id] = a;
        return a;
    }

    /* ------------------------------------------------------------------ the read me board */

    function readMe(parent) {
        var b = el('section', 'g-board g-readme');
        var l = el('h2', 'g-boardlabel');
        l.appendChild(el('span', 'g-boardname', 'Read me first'));
        b.appendChild(l);

        function block(title, lines) {
            var d = el('div', 'g-readblock');
            d.appendChild(el('h3', '', title));
            lines.forEach(function (line) {
                var p = el('p');
                line.split('`').forEach(function (part, i) { p.appendChild(i % 2 ? el('code', '', part) : document.createTextNode(part)); });
                d.appendChild(p);
            });
            b.appendChild(d);
        }

        b.appendChild(el('h1', 'g-readtitle', 'CMiX UI kit'));
        b.appendChild(el('p', 'g-readlead', 'A copy in HTML of the controls of the application CMiX. Use it to look at the user interface and to discuss a design. It is not the application: it looks the same, but it does no real work.'));

        block('How it is organised', [
            'Part A, CMiX.UI: the general controls. Board 0 holds the tokens (the colors, the sizes, the fonts and the timing that all controls use). Boards 1 to 10 hold the controls.',
            'Part B, CMiX.Studio.Avalonia: the blocks of the application, made from the controls of Part A. Boards 20 to 27. The numbers 11 to 19 are kept free for Part A.',
            'A board is a category: `1 Boolean`. A card on a board is an entry: `1.1 Button`. A line in a card is a variant with a letter: `1.1.b ButtonStraight`.'
        ]);
        block('How to refer to something', [
            'Give the number. Write "look at `1.1.b`". For one state, add the name of the column: "`1.1.b`, Hover". For a color or a size, give the name of the token: `Grey500`.',
            'Example of a request: "In `2.1.a`, Hover: make the fill `Grey700`."',
            'A number and a letter never change, and a number is never used again. The address of an item is `gallery.html#1.1.b`. A click on a number copies its reference.'
        ]);
        block('How to read a card', [
            'The name in fixed-width type is the exact name in the code of CMiX (the key of a theme, or the name of a control class). Text after a comma is not a name of the code: it says which case the line shows.',
            'The grey line "Source" is the file of CMiX that defines the item, from the root of the repository.',
            'A column is a state that the page forces, so that all states show side by side. `–` means that the variant has no such state.',
            'A block with the mark `LIVE` forces nothing: use the mouse on it. The cell in the column Idle also reacts to the pointer.'
        ]);
        block('Words', [
            'Token: a named value of the theme. A Color is a plain color. A Brush is the use of a color for one purpose. The value `#FF111113` has the opacity first (FF is opaque).',
            'On: a toggle that is checked. Selected: the row that is chosen in a list. Highlighted: the row that the arrow keys or the search point at. Keyboard cursor: the frame of the row that the arrow keys move.',
            'Kind: the type of an item of the project tree (Composable, Entity, Camera, Light, Texture, Filter, Modifier, Modulator). Each kind has one color, see `21.10`.',
            'Removed: the entry left the code. Its number stays. Not built: the entry is in the code and is not copied here yet.'
        ]);
        block('How to move', [
            'Wheel: scroll. Ctrl + wheel: zoom at the pointer. Drag the background, or hold Space and drag, or use the middle button: move. Shift + 1: show all.',
            'The list at the left goes to a board or an entry. The field Find takes a number or a part of a name.'
        ]);
        block('How to use it in a mockup', [
            'A mockup is an HTML file in `docs/ui-kit/mockups/` that links `tokens.css`, `cmix.css`, `icons.js` and `cmix.js`, inside an element with the class `cmix`.',
            'The class of a control is its name in the code: `<button class="ToggleButtonBox"></button>`, `<div class="DragValue" data-caption="Speed" data-value="0.5"></div>`.'
        ]);

        parent.appendChild(b);
        register('readme', b, { kind: 'board', zoom: 1, path: ['Read me first'], board: 'readme', search: 'readme read me help legend' });
        return b;
    }

    /* ------------------------------------------------------------------ tokens */

    function hexOf(element) {
        var c = getComputedStyle(element).backgroundColor;
        var m = /color\(srgb ([\d.]+) ([\d.]+) ([\d.]+)/.exec(c);
        var rgb = m ? [m[1] * 255, m[2] * 255, m[3] * 255] : (/rgba?\((\d+), (\d+), (\d+)/.exec(c) || []).slice(1);
        if (rgb.length < 3) return '';
        return '#' + rgb.map(function (v) { return ('0' + Math.round(v).toString(16)).slice(-2); }).join('').toUpperCase();
    }

    function swatches(group, partLabel) {
        var grid = el('div', 'g-swatches');
        group.tokens.forEach(function (t) {
            var card = el('div', 'g-swatch');
            card.title = t.note || '';
            var chip = el('div', 'g-chip');
            var fill = el('i');
            fill.style.background = 'var(--' + t.name + ')';
            chip.appendChild(fill);
            var text = el('div', 'g-text');
            text.appendChild(el('div', 'g-tokenname', t.name));
            text.appendChild(el('div', 'g-tokenvalue', t.raw.replace(/\{DynamicResource (\w+)\}/, '$1')));
            card.appendChild(chip);
            card.appendChild(text);
            grid.appendChild(card);
            register(t.name, card, { kind: 'token', zoom: 2, path: [partLabel, group.id + ' ' + group.name, t.name], board: group.id, search: t.name });
        });
        return grid;
    }

    function sample(t) {
        if (t.type === 'CornerRadius') {
            var box = el('span', 'g-radius');
            box.style.borderRadius = 'var(--' + t.name + ')';
            return box;
        }
        if (t.type === 'Double' && /px$/.test(t.css)) {
            var bar = el('span', 'g-bar');
            bar.style.width = 'var(--' + t.name + ')';
            return bar;
        }
        if (t.type === 'FontFamily') {
            var font = el('span', 'g-fontsample', 'Ctrl+Shift+E 0123');
            font.style.fontFamily = 'var(--' + t.name + ')';
            return font;
        }
        return document.createTextNode('');
    }

    function table(group, partLabel) {
        var tbl = el('table', 'g-table');
        var head = el('tr');
        ['Token', 'Value', 'Sample', 'Note'].forEach(function (h) { head.appendChild(el('th', 'g-statehead', h)); });
        tbl.appendChild(head);
        group.tokens.forEach(function (t) {
            var tr = el('tr');
            var name = el('td');
            name.appendChild(el('span', 'g-tokenname', t.name));
            var value = el('td');
            value.appendChild(el('span', 'g-tokenvalue', t.raw));
            var visual = el('td');
            visual.appendChild(sample(t));
            tr.appendChild(name);
            tr.appendChild(value);
            tr.appendChild(visual);
            tr.appendChild(el('td', 'g-note', t.note || ''));
            tbl.appendChild(tr);
            register(t.name, tr, { kind: 'token', zoom: 1.5, path: [partLabel, group.id + ' ' + group.name, t.name], board: group.id, search: t.name });
        });
        return tbl;
    }

    /* ------------------------------------------------------------------ the colour system (board 0.0, colour-chart.js) */

    /* A chip of one token: its colour, then its name. A brush alias shows the colour of its colour. */
    function tokenChip(name) {
        var chip = el('span', 'g-cchip');
        var fill = el('i');
        fill.style.background = 'var(--' + name + ')';
        chip.appendChild(fill);
        chip.appendChild(el('small', '', name));
        chip.addEventListener('click', function (e) { e.stopPropagation(); go(name, true, true); });
        return chip;
    }

    /* The chart of the colour system: the groups by meaning, the ladder of the tones, then the legend that a user reads. */
    function colourChart(parent, chart, partLabel) {
        var b = board(parent, '0.0', 'Colour system', 'docs/ui-kit/colour-chart.js', 'g-tokens', partLabel);
        if (chart.intro) b.appendChild(el('p', 'g-cintro', chart.intro));
        var path = [partLabel, '0.0 Colour system'];

        (chart.groups || []).forEach(function (group) {
            var sec = el('div', 'g-cgroup' + (group.id === 'retire' ? ' g-cretire' : ''));
            var head = el('h3', 'g-cgrouptitle', group.title);
            sec.appendChild(head);
            if (group.meaning) sec.appendChild(el('p', 'g-cmeaning', group.meaning));
            var grid = el('div', 'g-cgrid');
            ['Role', 'Tokens', 'What it tells the user', 'Rule'].forEach(function (h) { grid.appendChild(el('span', 'g-statehead', h)); });
            (group.rows || []).forEach(function (r) {
                var role = el('span', 'g-crole', r.role);
                var chips = el('span', 'g-cchips');
                (r.tokens || []).forEach(function (t) { chips.appendChild(tokenChip(t)); });
                var meaning = el('span', 'g-cnote', r.meaning || '');
                var rule = el('span', 'g-cnote');
                rule.appendChild(document.createTextNode(r.rule || ''));
                (r.flags || []).forEach(function (f) { rule.appendChild(el('em', 'g-cflag', f)); });
                [role, chips, meaning, rule].forEach(function (c) { grid.appendChild(c); });
                register('0.0/' + r.role, role, { kind: 'sample', zoom: 1.5, path: path.concat([group.title, r.role]), board: '0.0', search: r.role + ' ' + (r.tokens || []).join(' ') });
            });
            sec.appendChild(grid);
            b.appendChild(sec);
        });

        if (chart.tones && chart.tones.length) {
            var sec = el('div', 'g-cgroup');
            sec.appendChild(el('h3', 'g-cgrouptitle', 'Tones of a kind'));
            sec.appendChild(el('p', 'g-cmeaning', 'One hue, mixed over TreeBackground at a strength that tells the state. The same ladder for every kind (CategoryPalette).'));
            ['Entity', 'Composable2'].forEach(function (kind) {
                var ladder = el('div', 'g-cladder');
                ladder.setAttribute('data-kind', kind);
                ladder.appendChild(el('span', 'g-crole', kind));
                chart.tones.forEach(function (t) {
                    var step = el('span', 'g-cstep');
                    var fill = el('i');
                    fill.style.background = 'color-mix(in srgb, var(--Kind) ' + Math.round(t.strength * 100) + '%, var(--TreeBackground))';
                    step.appendChild(fill);
                    step.appendChild(el('small', '', t.id + ' ' + Math.round(t.strength * 100) + '%'));
                    step.title = t.meaning || '';
                    ladder.appendChild(step);
                });
                sec.appendChild(ladder);
            });
            var notes = el('ul', 'g-clist');
            chart.tones.forEach(function (t) { var li = el('li'); li.appendChild(el('b', '', t.id + ': ')); li.appendChild(document.createTextNode(t.meaning || '')); notes.appendChild(li); });
            sec.appendChild(notes);
            b.appendChild(sec);
        }

        if (chart.legend && chart.legend.length) {
            var leg = el('div', 'g-cgroup');
            leg.appendChild(el('h3', 'g-cgrouptitle', 'Legend'));
            leg.appendChild(el('p', 'g-cmeaning', 'The sentences a user can keep in mind.'));
            var list = el('ul', 'g-clist');
            chart.legend.forEach(function (l) { var li = el('li'); li.appendChild(el('b', '', l.cue + ': ')); li.appendChild(document.createTextNode(l.meaning)); list.appendChild(li); });
            leg.appendChild(list);
            b.appendChild(leg);
        }
        return b;
    }

    /* ------------------------------------------------------------------ the demo of an entry */

    /* The states of a demo: the key in data-states, the heading, the state classes of cmix.css, and a line that explains the heading.
       A demo shows the columns that its lines use, in this order. The first four are the same in every card. */
    var STATES = [
        ['idle', 'Idle', [], 'At rest. This cell also reacts to the pointer.'],
        ['hover', 'Hover', ['pointerover'], 'The pointer is over the control.'],
        ['pressed', 'Pressed', ['pointerover', 'pressed'], 'The mouse button is down on the control.'],
        ['disabled', 'Disabled', ['disabled'], 'The control takes no input.'],
        ['on', 'On', ['checked'], 'A toggle that is checked.'],
        ['onhover', 'On + hover', ['checked', 'pointerover'], 'A toggle that is checked, with the pointer over it.'],
        ['selected', 'Selected', ['selected'], 'The row that is chosen.'],
        ['selectedhover', 'Selected + hover', ['selected', 'pointerover'], 'The row that is chosen, with the pointer over it.'],
        ['highlighted', 'Highlighted', ['highlighted'], 'The row that the arrow keys or the search point at. The code sets it.'],
        ['cursor', 'Keyboard cursor', ['cursor', 'focus-within'], 'The row of the arrow keys, while its list has the keyboard focus.'],
        ['cursorselected', 'Cursor + selected', ['cursor', 'selected', 'focus-within'], 'The row of the arrow keys that is also chosen.'],
        ['cursorunfocused', 'Cursor, no focus', ['cursor'], 'The row of the arrow keys, while its list does not have the keyboard focus.'],
        ['focus', 'Focus', ['focus'], 'The control has the keyboard focus.'],
        ['editing', 'Editing', ['editing'], 'The user types a value.'],
        ['recording', 'Recording', ['recording'], 'The box waits for a key combination.'],
        ['open', 'Open', ['open'], 'The control is open.'],
        ['dimmed', 'Dimmed', ['dimmed'], 'A row out of play: it can not take the dragged item (the dragged row too), or it does not match the search.'],
        ['drophint', 'Drop hint', ['drophint'], 'A collapsed row with a possible target deeper: half covered, the chevron left clear.'],
        ['droptarget', 'Drop target', ['droptarget'], 'The row that takes the drop inside it.'],
        ['dropgap', 'Drop gap', ['dropgap'], 'The drop goes after this row: the line is under it.'],
        ['droptop', 'Drop top', ['droptop'], 'The drop goes before the first row: the line is at its top.'],
        ['hovered', 'Row hovered', ['hovered'], 'The pointer is over the row that holds the squares.'],
        ['connecting', 'Connecting', ['connecting'], 'A connection is in progress.'],
        ['connected', 'Connected', ['connected'], 'Connected and in sync.'],
        ['unsynced', 'Not in sync', ['connected', 'unsynced'], 'Connected, but the project is different from the shared state.'],
        ['insync', 'In sync', ['insync'], 'In sync.'],
        ['highlight', 'Modulator hovered', ['highlight'], 'The pointer is over the panel of the modulator that drives this value.']
    ];
    var COLUMN_WIDTHS = [128, 192, 256, 320, 400];
    var MAX_COLUMNS = 8;

    function stateOf(key) {
        for (var i = 0; i < STATES.length; i++) { if (STATES[i][0] === key) return STATES[i]; }
        return null;
    }

    /* The name of a line: the part before the comma is the name in the code, the rest says which case it is. */
    function nameParts(holder, name) {
        var comma = name.indexOf(',');
        holder.appendChild(el('span', 'g-codename', comma < 0 ? name : name.slice(0, comma)));
        if (comma >= 0) holder.appendChild(el('span', 'g-casename', name.slice(comma + 1).trim()));
    }

    /* A child of the template with data-sub is a variant: a line with a fixed letter and one cell for each state.
       data-states names its states. The default is idle, hover, pressed and disabled. data-on adds the two On columns.
       The state goes on the elements with data-state, or on the first element.
       Lines that share states are one grid. Lines with other states start a new grid with its own headings.
       A child with data-sample is a live block under the grids, with a fixed letter too. data-note is its note. */
    function demo(entry, content, context) {
        var holder = document.createDocumentFragment();
        var runs = [];

        Array.prototype.forEach.call(content.children, function (child) {
            if (!child.hasAttribute('data-sub')) return;
            var wanted = (child.getAttribute('data-states') || 'idle hover pressed disabled').split(/\s+/);
            if (child.hasAttribute('data-on')) wanted = wanted.concat(['on', 'onhover']);
            var last = runs[runs.length - 1];
            var shared = last ? wanted.filter(function (w) { return w !== 'idle' && last.used[w]; }).length : 0;
            var union = last ? Object.keys(last.used).concat(wanted.filter(function (w) { return !last.used[w]; })).length : 0;
            if (!last || (shared === 0 && wanted.length > 1) || union > MAX_COLUMNS) {
                last = { used: {}, rows: [] };
                runs.push(last);
            }
            wanted.forEach(function (w) { last.used[w] = true; });
            last.rows.push({ child: child, wanted: wanted });
        });

        function label(child, letter, kind) {
            var id = entry.id + '.' + letter;
            var name = child.getAttribute('data-name');
            var l = el('div', 'g-sublabel');
            l.id = id;
            l.appendChild(el('span', 'g-id', id));
            nameParts(l, name);
            return { id: id, name: name, element: l };
        }

        runs.forEach(function (run) {
            var columns = STATES.filter(function (s) { return run.used[s[0]]; });
            var grid = el('div', 'g-states');
            grid.setAttribute('data-columns', String(columns.length));
            grid.appendChild(el('div', 'g-statehead g-forced', 'Forced states'));
            columns.forEach(function (s) {
                var h = el('div', 'g-statehead', s[1]);
                h.title = s[3];
                grid.appendChild(h);
            });

            run.rows.forEach(function (r) {
                var l = label(r.child, r.child.getAttribute('data-sub'));
                grid.appendChild(l.element);
                var cells = [];
                columns.forEach(function (s) {
                    var cell = el('div', 'g-cell');
                    if (r.wanted.indexOf(s[0]) >= 0) {
                        cell.setAttribute('data-canvas-live', '');
                        cell.setAttribute('data-state-name', s[0]);
                        cell.setAttribute('aria-label', l.id + ' ' + l.name + ', ' + s[1] + (s[0] === 'idle' ? '' : ' (forced)'));
                        Array.prototype.forEach.call(r.child.children, function (c) { cell.appendChild(c.cloneNode(true)); });
                        var marked = cell.querySelectorAll('[data-state]');
                        Array.prototype.forEach.call(marked.length ? marked : cell.children, function (t) {
                            s[2].forEach(function (c) { t.classList.add(c); });
                        });
                        cells.push(cell);
                        register(l.id + '/' + s[0], cell, {
                            kind: 'state', zoom: 2, path: context.path.concat([l.id + ' ' + l.name, s[1]]),
                            board: context.board, card: context.card, rect: function () { return rowRect(l.element, cells); }
                        });
                    }
                    else {
                        cell.classList.add('g-nostate');
                        cell.textContent = '–';
                        cell.title = 'This variant has no such state.';
                    }
                    grid.appendChild(cell);
                });
                register(l.id, l.element, {
                    kind: 'variant', zoom: 2, path: context.path.concat([l.id + ' ' + l.name]), board: context.board, card: context.card,
                    search: l.id + ' ' + l.name, rect: function () { return rowRect(l.element, cells); }
                });
            });
            holder.appendChild(grid);
        });

        Array.prototype.forEach.call(content.children, function (child) {
            if (!child.hasAttribute('data-sample')) return;
            var block = el('div', 'g-sample');
            var l = label(child, child.getAttribute('data-sample'));
            l.element.appendChild(el('span', 'g-live', 'Live'));
            if (child.getAttribute('data-note')) l.element.appendChild(el('span', 'g-samplenote', child.getAttribute('data-note')));
            block.appendChild(l.element);
            var body = el('div', 'g-samplebody');
            body.setAttribute('data-canvas-live', '');
            if (child.getAttribute('style')) body.setAttribute('style', child.getAttribute('style'));
            Array.prototype.forEach.call(child.children, function (c) { body.appendChild(c.cloneNode(true)); });
            block.appendChild(body);
            holder.appendChild(block);
            register(l.id, block, {
                kind: 'sample', zoom: 1.5, path: context.path.concat([l.id + ' ' + l.name]), board: context.board, card: context.card,
                search: l.id + ' ' + l.name + ' ' + (child.getAttribute('data-note') || '')
            });
        });

        return holder;
    }

    /* The place of a whole line: from its label to its last cell. */
    function rowRect(labelElement, cells) {
        var a = canvas.rectOf(labelElement);
        var b = cells.length ? canvas.rectOf(cells[cells.length - 1]) : a;
        var top = Math.min(a.y, b.y), bottom = Math.max(a.y + a.h, b.y + b.h);
        return { x: a.x, y: top, w: b.x + b.w - a.x, h: bottom - top };
    }

    function entryCard(entry, context) {
        var card = el('article', 'g-entry');
        card.id = entry.id;
        var head = el('header');
        head.appendChild(el('span', 'g-id', entry.id));
        head.appendChild(el('h3', 'g-name', entry.name));
        var template = demoOf(entry);
        if (entry.removed) head.appendChild(el('span', 'g-tag g-tag-removed', 'Removed'));
        else if (!template) head.appendChild(el('span', 'g-tag g-tag-todo', 'Not built'));
        var source = el('span', 'g-source', entry.source);
        source.setAttribute('data-label', 'Source');
        head.appendChild(source);
        card.appendChild(head);

        var own = { path: context.path.concat([entry.id + ' ' + entry.name]), board: context.board, card: card };
        if (template) {
            var body = el('div', 'g-demo cmix');
            body.appendChild(demo(entry, template.content, own));
            card.appendChild(body);
        }
        else {
            card.classList.add(entry.removed ? 'g-is-removed' : 'g-is-todo');
            card.appendChild(el('div', 'g-absent', entry.removed ? entry.removed : 'This entry is in the code of CMiX. It is not copied here yet.'));
        }
        register(entry.id, card, { kind: 'entry', zoom: 1, path: own.path, board: context.board, card: card, search: entry.id + ' ' + entry.name });
        return card;
    }

    /* ------------------------------------------------------------------ build */

    /* The sandbox: the proposals of sandbox.js that wait for a decision. It is the first row, and it is not there when no proposal waits. */
    var proposals = window.CMIX_SANDBOX || [];
    if (proposals.length) {
        var sandboxLabel = 'Sandbox';
        nav.appendChild(el('div', 'g-part g-part-sandbox', 'Sandbox: waiting for your decision'));
        var sandboxRow = row('Sandbox', 'proposals that wait for your decision. Nothing here is in CMiX. A proposal goes away when you decide.');
        sandboxRow.classList.add('g-sandboxrow');
        proposals.forEach(function (p) {
            var b = el('section', 'g-board g-proposal');
            b.id = p.id;
            var l = el('h2', 'g-boardlabel');
            l.appendChild(el('span', 'g-id', p.id));
            l.appendChild(el('span', 'g-boardname', p.title));
            l.addEventListener('click', function () { go(p.id, true, true); });
            b.appendChild(l);

            var head = el('div', 'g-proposalhead');
            head.appendChild(el('span', 'g-tag g-tag-wait', 'Waiting for your decision'));
            head.appendChild(el('h3', 'g-question', p.question));
            head.appendChild(el('p', 'g-why', p.why));
            b.appendChild(head);

            var sides = el('div', 'g-sides');
            [['Before', 'as it is in CMiX now', p.before, ''], ['After', 'the proposal', p.after, 'sb-' + p.id]].forEach(function (side) {
                var column = el('div', 'g-side' + (side[3] ? ' g-side-after' : ''));
                var title = el('div', 'g-sidetitle', side[0]);
                title.appendChild(el('small', '', side[1]));
                column.appendChild(title);
                var body = el('div', 'g-demo cmix');
                body.setAttribute('data-canvas-live', '');
                var holder = el('div', side[3]);
                holder.innerHTML = p.sample;
                body.appendChild(holder);
                column.appendChild(body);
                column.appendChild(el('p', 'g-sidenote', side[2]));
                sides.appendChild(column);
            });
            b.appendChild(sides);

            if (p.facts && p.facts.length) {
                var facts = el('table', 'g-facts');
                var top = el('tr');
                ['', 'Before', 'After'].forEach(function (h) { top.appendChild(el('th', 'g-statehead', h)); });
                facts.appendChild(top);
                p.facts.forEach(function (f) {
                    var tr = el('tr');
                    f.forEach(function (cell, i) { tr.appendChild(el('td', i ? 'g-fact' : '', cell)); });
                    facts.appendChild(tr);
                });
                b.appendChild(facts);
            }
            var effect = el('p', 'g-effect');
            effect.appendChild(el('b', '', 'If you say yes: '));
            effect.appendChild(document.createTextNode(p.effect));
            b.appendChild(effect);
            var answer = el('p', 'g-answer');
            answer.appendChild(document.createTextNode('To decide, write '));
            answer.appendChild(el('code', '', p.id + ' yes'));
            answer.appendChild(document.createTextNode(' or '));
            answer.appendChild(el('code', '', p.id + ' no'));
            answer.appendChild(document.createTextNode(', or say what to change.'));
            b.appendChild(answer);

            sandboxRow.appendChild(b);
            register(p.id, b, { kind: 'board', zoom: 1, path: [sandboxLabel, p.id + ' ' + p.title], board: p.id, search: p.id + ' ' + p.title + ' sandbox proposal' });
            navLink(p.id, p.title, '', '', false);
        });
    }

    var tokenRow = null;
    catalogue.forEach(function (part) {
        var partLabel = 'Part ' + part.part + ' · ' + part.title;
        nav.appendChild(el('div', 'g-part', 'Part ' + part.part + ': ' + part.title));

        part.categories.forEach(function (cat) {
            if (cat.id !== '0') return;
            tokenRow = row(partLabel + ' · 0 Tokens', 'the colors, the sizes, the fonts and the timing that all controls use');
            readMe(tokenRow);
            colourChart(tokenRow, window.CMIX_COLOUR_CHART || {}, partLabel);
            navLink('0.0', 'Colour system', '', 'the tokens by meaning', true);
            navLink(cat.id, cat.name, '', '', false).addEventListener('click', function (e) { e.preventDefault(); e.stopPropagation(); go('0.1', true, true); });
            tokens.groups.forEach(function (group) {
                var colors = group.tokens.every(function (t) { return t.type === 'Color' || t.type === 'SolidColorBrush'; });
                var b = board(tokenRow, group.id, group.name, group.source, 'g-tokens', partLabel);
                b.appendChild(colors ? swatches(group, partLabel) : table(group, partLabel));
                if (group.id === '0.2' && tokens.skipped.length) {
                    var names = tokens.skipped.map(function (s) { return s.name; }).join(', ');
                    b.appendChild(el('div', 'g-absent', 'Not read from the theme of CMiX by the tool that makes this page. Written by hand in cmix.css: ' + names + '.'));
                }
                navLink(group.id, group.name, String(group.tokens.length), group.tokens.length + ' tokens', true);
            });
        });

        var first = part.categories.filter(function (c) { return c.id !== '0'; });
        var r = row(partLabel + ' · ' + first[0].id + ' to ' + first[first.length - 1].id,
            part.part === 'A' ? 'the general controls' : 'the blocks of the application, made from the controls of Part A');
        first.forEach(function (cat) {
            var b = board(r, cat.id, cat.name, '', '', partLabel);
            var context = { path: [partLabel, cat.id + ' ' + cat.name], board: cat.id };
            var live = cat.entries.filter(function (e) { return !e.removed; });
            var shown = live.filter(demoOf).length;
            navLink(cat.id, cat.name, shown + '/' + live.length, shown + ' of ' + live.length + ' entries are shown. A removed entry does not count.', false);
            var list = el('div', 'g-entries');
            list.setAttribute('data-board', cat.id);
            cat.entries.forEach(function (entry) {
                b.appendChild(entryCard(entry, context));
                var a = el('a', 'g-entrylink' + (entry.removed ? ' g-gone' : ''));
                a.href = '#' + entry.id;
                a.appendChild(el('span', 'g-id', entry.id));
                a.appendChild(el('span', 'g-navname', entry.name));
                list.appendChild(a);
                navLinks[entry.id] = a;
            });
            nav.appendChild(list);
        });
    });

    /* The demo of CategoryPalette shows the six tones of every kind, at the depth 0 and at the depth 1 (only the fill at rest gets weaker). */
    Array.prototype.forEach.call(world.querySelectorAll('[data-palette]'), function (holder) {
        var roles = ['Fill', 'FillHover', 'FillSelected', 'KindTone', 'KindHover', 'KindSelected'];
        holder.appendChild(el('b'));
        ['Fill', 'FillHover', 'FillSelected', 'Kind', 'KindHover', 'KindSelected'].forEach(function (r) { holder.appendChild(el('b', '', r)); });
        ['Composable0', 'Composable1', 'Composable2', 'Composable3', 'Composable4', 'Composable5', 'Entity', 'Camera', 'Light', 'Texture', 'Filter', 'Modifier', 'Modulator'].forEach(function (kind) {
            [0, 1].forEach(function (depth) {
                holder.appendChild(el('span', '', depth ? 'one level deeper' : kind));
                roles.forEach(function (role) {
                    var chip = el('i');
                    chip.setAttribute('data-kind', kind);
                    chip.style.setProperty('--KindDepth', String(depth));
                    chip.style.background = 'var(--' + role + ')';
                    holder.appendChild(chip);
                });
            });
        });
    });

    /* The demo of AppIcon shows every icon key of icons.js. */
    Array.prototype.forEach.call(world.querySelectorAll('[data-all-icons]'), function (holder) {
        ((window.CMIX_ICONS || {}).icons || []).forEach(function (i) {
            var tile = el('div', 'g-icon');
            tile.title = i.kind + ' (' + i.source + ')';
            var glyph = el('i', 'AppIcon');
            glyph.setAttribute('data-icon-key', i.key);
            tile.appendChild(glyph);
            tile.appendChild(el('span', '', i.key));
            holder.appendChild(tile);
            register(i.key, tile, { kind: 'icon', zoom: 2, path: ['Part A · CMiX.UI', '10 Layout and containers', '10.13 AppIcon', i.key], board: '10', search: i.key + ' ' + i.kind });
        });
    });

    window.CMiX.apply(world);

    /* The columns of one grid have one width: the smallest of the fixed widths that holds the widest cell. */
    Array.prototype.forEach.call(world.querySelectorAll('.g-states'), function (grid) {
        var widest = 0;
        grid.style.gridTemplateColumns = '280px repeat(' + grid.getAttribute('data-columns') + ', max-content)';
        Array.prototype.forEach.call(grid.querySelectorAll('.g-cell:not(.g-nostate)'), function (cell) {
            Array.prototype.forEach.call(cell.children, function (child) { widest = Math.max(widest, child.offsetWidth); });
        });
        var width = COLUMN_WIDTHS[COLUMN_WIDTHS.length - 1];
        for (var i = 0; i < COLUMN_WIDTHS.length; i++) { if (COLUMN_WIDTHS[i] >= widest) { width = COLUMN_WIDTHS[i]; break; } }
        grid.style.gridTemplateColumns = '280px repeat(' + grid.getAttribute('data-columns') + ', ' + width + 'px)';
    });

    /* A color of the palette shows its value. */
    Array.prototype.forEach.call(world.querySelectorAll('.g-palette > i'), function (chip) {
        var hex = hexOf(chip);
        chip.textContent = hex;
        chip.title = hex;
    });

    /* ------------------------------------------------------------------ the canvas, the address and the reference bar */

    var canvas = window.CMiXCanvas.attach(document.querySelector('[data-canvas]'));
    var current = null;
    var hovered = null;
    var flying = false;

    function reference(target) {
        if (!target) return '';
        if (target.kind === 'state') {
            var parts = target.key.split('/');
            return parts[0] + ', ' + stateOf(parts[1])[1];
        }
        return target.key;
    }

    function showCrumb() {
        var target = hovered || current;
        crumb.textContent = '';
        if (!target) { crumb.appendChild(el('span', 'g-crumbhint', 'Point at an item to see its reference.')); copyButton.hidden = true; return; }
        target.path.forEach(function (part, i) {
            if (i) crumb.appendChild(el('span', 'g-crumbsep', '›'));
            crumb.appendChild(el('span', i === target.path.length - 1 ? 'g-crumblast' : '', part));
        });
        copyButton.hidden = target.key === 'readme';
        copyButton.textContent = 'Copy "' + reference(target) + '"';
        copyButton.setAttribute('data-copy', reference(target));
    }

    function copy(text) {
        function done() { status.textContent = 'Copied: ' + text; }
        if (navigator.clipboard && window.isSecureContext) { navigator.clipboard.writeText(text).then(done, function () {}); return; }
        var area = el('textarea');
        area.value = text;
        document.body.appendChild(area);
        area.select();
        try { document.execCommand('copy'); done(); } catch (e) { /* no clipboard */ }
        area.remove();
    }

    function markNav(target) {
        Object.keys(navLinks).forEach(function (id) { navLinks[id].classList.remove('g-current'); });
        Array.prototype.forEach.call(nav.querySelectorAll('.g-entries'), function (list) { list.classList.toggle('g-open', !!target && list.getAttribute('data-board') === target.board); });
        if (!target) return;
        if (navLinks[target.board]) navLinks[target.board].classList.add('g-current');
        var entryId = target.card ? target.card.id : (target.kind === 'entry' ? target.key : '');
        if (navLinks[entryId]) navLinks[entryId].classList.add('g-current');
    }

    function go(key, animate, push) {
        var target = targets[String(key).toLowerCase()];
        if (!target) return false;
        Array.prototype.forEach.call(world.querySelectorAll('.g-hit'), function (e) { e.classList.remove('g-hit'); });
        if (target.kind !== 'board') target.element.classList.add('g-hit');
        current = target;
        flying = true;
        if (target.rect) canvas.flyToRect(target.rect(), target.zoom, animate);
        else canvas.flyTo(target.element, target.zoom, animate);
        setTimeout(function () { flying = false; }, 350);
        var address = '#' + target.key;
        if (push && location.hash !== address) history.pushState(null, '', address);
        else history.replaceState(null, '', address);
        markNav(target);
        showCrumb();
        return true;
    }

    function overview(message) {
        current = null;
        canvas.fitAll(false);
        markNav(null);
        showCrumb();
        if (message) status.textContent = message;
    }

    /* When the user moves the view away, the address no longer names what is on the screen. */
    canvas.onChange(function (view, byUser) {
        world.classList.toggle('g-far', view.s < 0.5);
        world.classList.toggle('g-veryfar', view.s < 0.2);
        if (byUser && !flying && location.hash) history.replaceState(null, '', location.pathname + location.search);
    });

    world.addEventListener('mouseover', function (e) {
        var holder = e.target.closest('[data-ref]');
        var target = holder ? targets[holder.getAttribute('data-ref').toLowerCase()] : null;
        if (target === hovered) return;
        hovered = target && target.kind !== 'board' ? target : null;
        showCrumb();
    });
    world.addEventListener('mouseleave', function () { hovered = null; showCrumb(); });

    /* A click on a number copies its reference and makes it the address. */
    world.addEventListener('click', function (e) {
        var id = e.target.closest('.g-id');
        if (!id) return;
        var holder = id.closest('[data-ref]');
        if (!holder) return;
        var target = targets[holder.getAttribute('data-ref').toLowerCase()];
        copy(reference(target) + (target.kind === 'entry' || target.kind === 'variant' || target.kind === 'sample' ? ' ' + target.path[target.path.length - 1].replace(/^\S+\s/, '') : ''));
        history.replaceState(null, '', '#' + target.key);
    });
    copyButton.addEventListener('click', function () { copy(copyButton.getAttribute('data-copy')); });

    /* ------------------------------------------------------------------ find */

    var found = [];
    var foundIndex = -1;

    function matches(query) {
        var q = query.trim().toLowerCase();
        if (!q) return [];
        if (/^[\d.]+[a-z]?$/.test(q)) {
            var exact = items.filter(function (i) { return i.key.toLowerCase() === q; });
            if (exact.length) return exact;
        }
        return items.filter(function (i) { return i.search.indexOf(q) >= 0; });
    }

    function dim(on) {
        var keep = [];
        found.forEach(function (t) { keep.push(t.element); if (t.card) keep.push(t.card); });
        items.forEach(function (i) {
            if (i.kind === 'board') return;
            i.element.classList.toggle('g-dim', on && keep.indexOf(i.element) < 0 && !found.some(function (t) { return t.card === i.element || i.card && t.element === i.card; }));
        });
    }

    filter.addEventListener('input', function () {
        found = matches(filter.value);
        foundIndex = -1;
        var active = filter.value.trim() !== '';
        dim(active && found.length > 0);
        status.textContent = !active ? '' : found.length === 0 ? 'No match.' : found.length === 1 ? '1 match. Enter: go to it.' : found.length + ' matches. Enter: go to the next one.';
    });
    filter.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') { filter.value = ''; filter.dispatchEvent(new Event('input')); return; }
        if (e.key !== 'Enter' || !found.length) return;
        foundIndex = (foundIndex + (e.shiftKey ? found.length - 1 : 1)) % found.length;
        dim(false);
        go(found[foundIndex].key, true, true);
        status.textContent = (foundIndex + 1) + ' of ' + found.length + ': ' + reference(found[foundIndex]) + '. Enter: next.';
    });

    /* ------------------------------------------------------------------ the list, the legend and the address */

    nav.addEventListener('click', function (e) {
        var a = e.target.closest('a');
        if (!a) return;
        e.preventDefault();
        go(decodeURIComponent(a.getAttribute('href').slice(1)), true, true);
    });
    document.getElementById('help').addEventListener('click', function (e) { e.preventDefault(); go('readme', true, true); });
    document.getElementById('home').addEventListener('click', function () { history.replaceState(null, '', location.pathname + location.search); overview(''); });

    var legend = document.getElementById('legend');
    document.getElementById('legendButton').addEventListener('click', function () { legend.hidden = !legend.hidden; });
    document.getElementById('legendClose').addEventListener('click', function () { legend.hidden = true; });

    function fromAddress(animate) {
        var key = decodeURIComponent(location.hash.slice(1));
        if (!key) { overview(''); return; }
        if (!go(key, animate, false)) overview('No item "' + key + '". Showing all.');
    }
    window.addEventListener('hashchange', function () { fromAddress(true); });
    window.addEventListener('popstate', function () { fromAddress(true); });

    fromAddress(false);
})();
