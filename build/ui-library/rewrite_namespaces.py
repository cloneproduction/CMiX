#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
# SPDX-License-Identifier: LGPL-3.0-or-later
"""Moves the generic UI types to the CMiX.UI namespaces, in place.

Run from the repo root:  python build/ui-library/rewrite_namespaces.py [--dry-run]

The script changes only namespaces and the names that point to them. It does not move files.
It rewrites, in CMiX.Studio.Avalonia and CMiX.Studio.Avalonia.Tests:
  - the namespace line of every moving C# file,
  - the using lines of every C# file, from the types the file uses,
  - x:Class and the xmlns aliases of every AXAML file.
An alias that points to an old namespace gets the CMiX.UI URI when the file uses only moving
types through it. A file that uses both kinds keeps the old alias for the types that stay and
gets a Studio<Name> alias for them. The script is safe to run again: it finds nothing to do
after the first run. Delete it with the rest of the UI library work when the branch is merged.
"""
import os
import re
import sys

ROOT = os.getcwd()
STUDIO = 'CMiX.Studio.Avalonia'
TESTS = 'CMiX.Studio.Avalonia.Tests'
OLD_ROOT = 'CMiX.Studio.Avalonia'
UI_URI = 'https://github.com/cloneproduction/CMiX.UI'

C = 'Views/Controls/'

# (path relative to the Studio folder or a folder ending with /, new namespace)
CONTROLS = ('DragValue CMiXSlider CMiXToggleButton CMiXListBox CMiXPopup PopupFrame NumericInput '
            'NumericText LabeledContent DefaultResetMenu ContextMenuGuard DragEditHelper ControlPosition '
            'CaptionedUserControl').split()
CONTROLS_XAML = ('CaptionedButton CaptionedComboBox CaptionedToggleButton IntegerValue Integer2Value '
                 'Integer3Value Vector2 VectorXYZ RangeValue AxisSelector2D AxisSelector3D IPBox '
                 'ColorSelector AppIcon').split()
CONVERTERS = ('ColorToBrushConverter ColorToHexConverter ColorToNoAlphaConverter HexToBrushConverter '
              'CountToVisibility CurrentStepConverter DoubleToStringConverter EnumMatchConverter '
              'FontFamilyToStringConverter NullToVisibilityConverter ControlPositionToCornerRadiusConverter '
              'RemapDoubleConverter').split()


def moving_files():
    """Returns {path: new namespace} for every moving file, paths relative to the Studio folder."""
    m = {}
    for n in CONTROLS:
        m[C + n + '.cs'] = 'CMiX.UI.Controls'
    for n in CONTROLS_XAML:
        m[C + n + '.axaml'] = 'CMiX.UI.Controls'
        m[C + n + '.axaml.cs'] = 'CMiX.UI.Controls'
    for sub, ns in (('ColorPicker', 'CMiX.UI.Controls.ColorPicker'), ('TextBox', 'CMiX.UI.Controls.TextInput')):
        folder = os.path.join(STUDIO, C + sub)
        for f in os.listdir(folder):
            m[C + sub + '/' + f] = ns
    for n in ('IndentedExpander.cs', 'PositionedStackPanel.cs', 'PositionedUniformGrid.cs'):
        m[C + 'Panels/' + n] = 'CMiX.UI.Panels'
    for f in os.listdir(os.path.join(STUDIO, 'AttachedProperties')):
        m['AttachedProperties/' + f] = 'CMiX.UI.AttachedProperties'
    for n in ('ClosePopupBehavior', 'ScrollViewerBehavior', 'ColorSliderBehavior'):
        m['Behaviors/' + n + '.cs'] = 'CMiX.UI.Behaviors'
    for n in CONVERTERS:
        m['Converters/' + n + '.cs'] = 'CMiX.UI.Converters'
    m['Mathematics/MathUtils.cs'] = 'CMiX.UI.Mathematics'
    m['Services/ListBoxReorderDragDrop.cs'] = 'CMiX.UI.Behaviors'
    m['Services/IReorderService.cs'] = 'CMiX.UI.Behaviors'
    m['Views/LogicalPanelContent.cs'] = 'CMiX.UI.AttachedProperties'
    m['Interaction/InteractionHost.cs'] = 'CMiX.UI.Interaction'
    m['Themes/Basic/Icons.cs'] = 'CMiX.UI.Themes'
    return m


# ---------------------------------------------------------------- file helpers

class Doc:
    def __init__(self, path):
        self.path = path
        raw = open(path, 'rb').read()
        self.bom = raw.startswith(b'\xef\xbb\xbf')
        text = raw.decode('utf-8-sig')
        self.crlf = '\r\n' in text
        self.text = text.replace('\r\n', '\n')
        self.original = self.text

    def save(self):
        if self.text == self.original:
            return False
        out = self.text.replace('\n', '\r\n') if self.crlf else self.text
        open(self.path, 'wb').write((b'\xef\xbb\xbf' if self.bom else b'') + out.encode('utf-8'))
        return True


def files(folder, ext):
    for dirpath, dirs, names in os.walk(os.path.join(ROOT, folder)):
        dirs[:] = [d for d in dirs if d not in ('obj', 'bin')]
        for n in names:
            if n.endswith(ext):
                yield os.path.join(dirpath, n)


def rel_studio(path):
    p = os.path.relpath(path, os.path.join(ROOT, STUDIO)).replace(os.sep, '/')
    return p


NS_RE = re.compile(r'^namespace\s+([\w.]+)', re.M)
TYPE_RE = re.compile(
    r'^    (?:\[[^\]]*\]\s*)?(?:(?:public|internal|static|sealed|abstract|partial|readonly|unsafe)\s+)*'
    r'(?:class|enum|interface|struct|record|delegate\s+[\w<>?\[\],\s]+?)\s+([A-Z]\w*)', re.M)


def code_only(s):
    s = re.sub(r'/\*.*?\*/', '', s, flags=re.S)
    s = re.sub(r'//[^\n]*', '', s)
    s = re.sub(r'@"(?:[^"]|"")*"', '""', s)
    s = re.sub(r'\$?"(?:\\.|[^"\\\n])*"', '""', s)
    return s


def declared(doc):
    """Returns (namespace, [top level type names]) of a C# document."""
    m = NS_RE.search(doc.text)
    ns = m.group(1) if m else ''
    return ns, TYPE_RE.findall(code_only(doc.text))


def main():
    dry = '--dry-run' in sys.argv
    moving = moving_files()

    # 1. The type tables.
    studio_cs = {p: Doc(p) for p in files(STUDIO, '.cs')}
    moved_types = {}   # type name -> (old namespace, new namespace)
    staying = {}       # old namespace -> set of type names that stay
    for path, doc in studio_cs.items():
        rel = rel_studio(path)
        ns, types = declared(doc)
        if rel in moving:
            for t in types:
                moved_types[t] = (ns, moving[rel])
        else:
            staying.setdefault(ns, set()).update(types)
    # A type in an AXAML file without a code file is rare; the x:Class gives its name.
    for rel, new_ns in moving.items():
        if rel.endswith('.axaml'):
            doc = Doc(os.path.join(ROOT, STUDIO, rel))
            m = re.search(r'x:Class="([\w.]+)"', doc.text)
            if m:
                old_ns, _, name = m.group(1).rpartition('.')
                moved_types.setdefault(name, (old_ns, new_ns))

    clashes = [t for t in moved_types if any(t in s for s in staying.values())]
    if clashes:
        print('STOP: a moving type has the name of a type that stays:', clashes)
        return 1
    same_uri = {}
    for t, (_, new_ns) in moved_types.items():
        if new_ns in XAML_NAMESPACES:
            same_uri.setdefault(t, []).append(new_ns)
    dup = {t: v for t, v in same_uri.items() if len(v) > 1}
    if dup:
        print('STOP: two types with one name under the one URI:', dup)
        return 1

    old_namespaces = {old for old, _ in moved_types.values()}
    fully_moved = {ns for ns in old_namespaces if not staying.get(ns)}

    changed = []
    # 2. The C# files.
    all_cs = dict(studio_cs)
    for p in files(TESTS, '.cs'):
        all_cs[p] = Doc(p)
    for path, doc in all_cs.items():
        rel = rel_studio(path) if path.startswith(os.path.join(ROOT, STUDIO)) else None
        rewrite_cs(doc, rel, moving, moved_types, staying, old_namespaces)

    # 3. The AXAML files.
    all_xaml = {}
    for p in list(files(STUDIO, '.axaml')):
        all_xaml[p] = Doc(p)
    for path, doc in all_xaml.items():
        rel = rel_studio(path)
        rewrite_xaml(doc, rel, moving, moved_types, staying, fully_moved)

    for path, doc in list(all_cs.items()) + list(all_xaml.items()):
        if doc.text != doc.original:
            changed.append(os.path.relpath(path, ROOT))
            if not dry:
                doc.save()
    print(('Would change' if dry else 'Changed'), len(changed), 'files')
    return 0


XAML_NAMESPACES = {
    'CMiX.UI.Controls', 'CMiX.UI.Controls.ColorPicker', 'CMiX.UI.Controls.TextInput', 'CMiX.UI.Panels',
    'CMiX.UI.AttachedProperties', 'CMiX.UI.Behaviors', 'CMiX.UI.Converters',
}


# ---------------------------------------------------------------- C#

USING_RE = re.compile(r'^using ([\w.]+);[ \t]*\n', re.M)


def rewrite_cs(doc, rel, moving, moved_types, staying, old_namespaces):
    text = doc.text
    ns_match = NS_RE.search(text)
    own_ns = ns_match.group(1) if ns_match else ''
    if rel in moving:
        own_ns_new = moving[rel]
        text = NS_RE.sub('namespace ' + own_ns_new, text, count=1)
    else:
        own_ns_new = own_ns

    code = code_only(text)
    idents = set(re.findall(r'\b[A-Z]\w*\b', code))

    # Qualified names of moving types lose the qualifier. The using line brings the type back.
    qualifier = r'(?:(?:CMiX\.Studio\.Avalonia\.)?(?:Views\.Controls\.Panels|Views\.Controls|Views|Themes|Services|' \
                r'Converters|Behaviors|AttachedProperties|Mathematics|Interaction)\.)'
    for t in sorted(moved_types, key=len, reverse=True):
        if t in idents:
            text = re.sub(r'(?<![\w.])' + qualifier + r'(' + re.escape(t) + r')\b', r'\1', text)

    code = code_only(text)
    idents = set(re.findall(r'\b[A-Z]\w*\b', code))

    needed_new = set()
    for t, (old_ns, new_ns) in moved_types.items():
        if t in idents and not enclosing(own_ns_new, new_ns):
            needed_new.add(new_ns)
    needed_old = set()
    for old_ns in old_namespaces:
        used_staying = staying.get(old_ns, set()) & idents
        if used_staying:
            needed_old.add(old_ns)

    usings = USING_RE.findall(text)
    # Remove the old usings that nothing needs.
    for u in usings:
        if u in old_namespaces and u not in needed_old:
            text = re.sub(r'^using ' + re.escape(u) + r';[ \t]*\n', '', text, count=1, flags=re.M)
    # Add the new usings that are missing.
    present = set(USING_RE.findall(text))
    for new_ns in sorted(needed_new):
        if new_ns not in present:
            text = insert_using(text, new_ns)
    doc.text = text


def enclosing(own, other):
    """True when the namespace `other` is `own` or one of its parents."""
    return own == other or own.startswith(other + '.')


def insert_using(text, ns):
    line = 'using ' + ns + ';\n'
    lines = text.split('\n')
    cmix = [i for i, l in enumerate(lines) if l.startswith('using CMiX.')]
    anyu = [i for i, l in enumerate(lines) if l.startswith('using ') and l.rstrip().endswith(';')]
    if cmix:
        for i in cmix:
            if lines[i].rstrip()[len('using '):-1] > ns:
                lines.insert(i, line.rstrip('\n'))
                return '\n'.join(lines)
        lines.insert(cmix[-1] + 1, line.rstrip('\n'))
        return '\n'.join(lines)
    if anyu:
        lines.insert(anyu[-1] + 1, line.rstrip('\n'))
        return '\n'.join(lines)
    # No usings: put one before the namespace line, with a blank line after.
    for i, l in enumerate(lines):
        if l.startswith('namespace '):
            lines[i:i] = [line.rstrip('\n'), '']
            return '\n'.join(lines)
    return text


# ---------------------------------------------------------------- AXAML

XMLNS_RE = re.compile(r'xmlns:(\w+)="(?:clr-namespace|using):([\w.]+)(;assembly=[\w.]+)?"')


def rewrite_xaml(doc, rel, moving, moved_types, staying, fully_moved):
    text = doc.text

    # x:Class of a moving file.
    if rel in moving:
        def fix_class(m):
            old_ns, _, name = m.group(1).rpartition('.')
            return 'x:Class="' + moving[rel] + '.' + name + '"'
        text = re.sub(r'x:Class="([\w.]+)"', fix_class, text, count=1)

    decls = {}
    for m in XMLNS_RE.finditer(text):
        alias, ns = m.group(1), m.group(2)
        if m.group(3) is None and (ns == OLD_ROOT or ns.startswith(OLD_ROOT + '.')):
            decls[alias] = ns

    taken = set(re.findall(r'xmlns:(\w+)=', text))
    for alias, ns in decls.items():
        usage = re.compile(r'(?<![\w:])' + re.escape(alias) + r'([:|])([A-Za-z_]\w*)')
        uses = [(m.group(1), m.group(2)) for m in usage.finditer(text)]
        moving_use = [u for u in uses if u[1] in moved_types and moved_types[u[1]][0] == ns]
        staying_use = [u for u in uses if u not in moving_use]
        decl_re = re.compile(r'xmlns:' + re.escape(alias) + r'="(?:clr-namespace|using):' + re.escape(ns) + r'"')

        if not uses:
            if ns in fully_moved:
                text = remove_decl(text, decl_re)
            continue
        if not moving_use:
            continue

        decl_text = decl_re.search(text).group(0)
        new_decl = 'xmlns:' + alias + '="' + UI_URI + '"'
        if staying_use:
            last = ns.rsplit('.', 1)[-1]
            new_alias = 'Studio' + last
            n = 2
            while new_alias in taken:
                new_alias = 'Studio' + last + str(n)
                n += 1
            taken.add(new_alias)
            # The staying uses move to the new alias.
            staying_names = {u[1] for u in staying_use}
            def move_use(m):
                if m.group(2) in staying_names:
                    return new_alias + m.group(1) + m.group(2)
                return m.group(0)
            text = usage.sub(move_use, text)
            text = place_decl(text, decl_re, new_decl, 'xmlns:' + new_alias + '="' + decl_text.split('="', 1)[1],
                              decl_text)
        else:
            text = decl_re.sub(new_decl, text, count=1)
    doc.text = normalize_aliases(text)


def normalize_aliases(text):
    """Gives every alias of the CMiX.UI URI the one name `ui`."""
    uri_decl = re.compile(r'xmlns:(\w+)="' + re.escape(UI_URI) + '"')
    aliases = [m.group(1) for m in uri_decl.finditer(text)]
    if not aliases or aliases == ['ui']:
        return text
    other = set(re.findall(r'xmlns:(\w+)=', text)) - set(aliases)
    if 'ui' in other:
        return text
    for alias in aliases:
        if alias == 'ui':
            continue
        usage = re.compile(r'(?<![\w:])' + re.escape(alias) + r'([:|])(?=[A-Za-z_])')
        text = usage.sub(lambda m: 'ui' + m.group(1), text)
    # Keep the first declaration, as `ui`. The others go away with their line.
    mark = '@@REMOVE@@'
    state = {'first': True}

    def keep_first(m):
        if state['first']:
            state['first'] = False
            return 'xmlns:ui="' + UI_URI + '"'
        return mark

    text = uri_decl.sub(keep_first, text)
    text = re.sub(r'\n[ \t]*' + mark + r'(?=\n|>|/>)', '', text)
    text = re.sub(r'[ \t]*' + mark, '', text)
    return text


def remove_decl(text, decl_re):
    # The attribute on its own line goes away with the line. Otherwise only the attribute goes.
    own_line = re.compile(r'\n[ \t]*' + decl_re.pattern + r'(?=\n|>|/>)')
    if own_line.search(text):
        return own_line.sub('', text, count=1)
    return re.sub(r'[ \t]*' + decl_re.pattern, '', text, count=1)


def place_decl(text, decl_re, replace_with, extra, decl_text):
    """Replaces the old declaration and adds `extra` after it, on a new line if the old one had its own."""
    m = decl_re.search(text)
    start = m.start()
    line_start = text.rfind('\n', 0, start) + 1
    prefix = text[line_start:start]
    if prefix.strip() == '':
        glue = '\n' + prefix
    else:
        glue = ' '
    return text[:start] + replace_with + glue + extra + text[m.end():]


if __name__ == '__main__':
    sys.exit(main())
