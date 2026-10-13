#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
# SPDX-License-Identifier: LGPL-3.0-or-later
"""Groups the controls of CMiX.UI in folders by the type of data they edit.

Run from the repo root:  python build/ui-library/group_controls.py [--dry-run]

For every control in the table the script:
  - moves its files with git mv to CMiX.UI/Controls/<Folder>,
  - sets the namespace to CMiX.UI.Controls.<Folder> (the namespace follows the folder),
  - sets x:Class in its AXAML file,
  - fixes the using lines of every C# file in CMiX.UI, CMiX.Studio.Avalonia and the two test projects.
The AXAML of the views does not change: they reach all the controls through the one ui: name.
"""
import glob
import os
import re
import subprocess
import sys

UI = 'CMiX.UI'
CONTROLS = UI + '/Controls'
CODE_FOLDERS = [UI, 'CMiX.UI.Tests', 'CMiX.Studio.Avalonia', 'CMiX.Studio.Avalonia.Tests']

# Folder -> controls. A control that is not in the table stays where it is.
GROUPS = {
    'Number': 'DragValue CMiXSlider NumericInput NumericText IntegerValue RangeValue InputNumbersOnly',
    'Vector': 'Vector2 VectorXYZ Integer2Value Integer3Value AxisSelector2D AxisSelector3D',
    'Boolean': 'CaptionedToggleButton CMiXToggleButton CaptionedButton',
    'Enum': 'CaptionedComboBox',
    'String': 'EditableTextBox EditableValue IPBox SelectTextOnFocus',
    'ColorPicker': 'ColorSelector',
    'Parts': 'LabeledContent CaptionedUserControl DefaultResetMenu ContextMenuGuard DragEditHelper PopupFrame CMiXPopup',
}
# ControlPosition is a layout idea. It goes next to the positioned panels.
PANELS = ['ControlPosition']


def norm(p):
    return p.replace(os.sep, '/')


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


NS_RE = re.compile(r'^namespace\s+([\w.]+)', re.M)
TYPE_RE = re.compile(
    r'^    (?:\[[^\]]*\]\s*)?(?:(?:public|internal|static|sealed|abstract|partial|readonly|unsafe)\s+)*'
    r'(?:class|enum|interface|struct|record|delegate\s+[\w<>?\[\],\s]+?)\s+([A-Z]\w*)', re.M)
USING_RE = re.compile(r'^using ([\w.]+);[ \t]*\n', re.M)


def code_only(s):
    s = re.sub(r'/\*.*?\*/', '', s, flags=re.S)
    s = re.sub(r'//[^\n]*', '', s)
    s = re.sub(r'@"(?:[^"]|"")*"', '""', s)
    s = re.sub(r'\$?"(?:\\.|[^"\\\n])*"', '""', s)
    return s


def enclosing(own, other):
    return own == other or own.startswith(other + '.')


def target_of(base):
    """Returns (folder path, namespace) for a control name, or None when it stays."""
    for folder, names in GROUPS.items():
        if base in names.split():
            return CONTROLS + '/' + folder, 'CMiX.UI.Controls.' + folder
    if base in PANELS:
        return UI + '/Panels', 'CMiX.UI.Panels'
    return None


def base_name(path):
    n = os.path.basename(path)
    for suffix in ('.axaml.cs', '.axaml', '.cs'):
        if n.endswith(suffix):
            return n[:-len(suffix)]
    return n


def main():
    dry = '--dry-run' in sys.argv
    # 1. The files of the controls, and where each one goes.
    files = [norm(f) for f in glob.glob(CONTROLS + '/**/*', recursive=True)
             if os.path.isfile(f) and f.endswith(('.cs', '.axaml'))]
    moves = {}   # old path -> (new path, new namespace)
    for f in files:
        target = target_of(base_name(f))
        if target:
            folder, ns = target
            new = folder + '/' + os.path.basename(f)
            if new != f:
                moves[f] = (new, ns)
            else:
                moves[f] = (f, ns)
    unknown = [n for names in GROUPS.values() for n in names.split()
               if not any(base_name(f) == n for f in files)]
    if unknown:
        print('STOP: no file for', unknown)
        return 1

    # 2. The final namespace of every type declared in a control file.
    docs = {f: Doc(f) for f in files if f.endswith('.cs')}
    ns_of = {}
    for f, doc in docs.items():
        own = NS_RE.search(doc.text).group(1)
        final = moves[f][1] if f in moves else own
        for t in TYPE_RE.findall(code_only(doc.text)):
            ns_of[t] = final
    duplicates = [t for t in ns_of if sum(1 for d in docs.values() if t in TYPE_RE.findall(code_only(d.text))) > 1]
    if duplicates:
        print('STOP: a type name is declared twice:', duplicates)
        return 1

    # 3. Namespaces of the moving files.
    for f, (new, ns) in moves.items():
        if f in docs:
            docs[f].text = NS_RE.sub('namespace ' + ns, docs[f].text, count=1)
        elif f.endswith('.axaml'):
            pass

    # 4. The using lines of all the C# files.
    all_cs = dict(docs)
    for folder in CODE_FOLDERS:
        for p in glob.glob(folder + '/**/*.cs', recursive=True):
            p = norm(p)
            if '/obj/' in p or '/bin/' in p or p in all_cs:
                continue
            all_cs[p] = Doc(p)
    for path, doc in all_cs.items():
        fix_usings(doc, ns_of)

    # 5. x:Class of the AXAML files that move.
    xaml = {}
    for f, (new, ns) in moves.items():
        if f.endswith('.axaml'):
            d = Doc(f)
            d.text = re.sub(r'x:Class="[\w.]+\.(\w+)"', lambda m: 'x:Class="' + ns + '.' + m.group(1) + '"', d.text, count=1)
            xaml[f] = d

    changed = [p for p, d in list(all_cs.items()) + list(xaml.items()) if d.text != d.original]
    print(('Would move' if dry else 'Moving'), sum(1 for f, (n, _) in moves.items() if n != f), 'files;',
          ('would change' if dry else 'changing'), len(changed), 'files')
    if dry:
        return 0

    # Write the text first, then move: git mv keeps the new text.
    for path, doc in list(all_cs.items()) + list(xaml.items()):
        doc.save()
    for f, (new, _) in sorted(moves.items()):
        if new != f:
            os.makedirs(os.path.dirname(new), exist_ok=True)
            subprocess.run(['git', 'mv', f, new], check=True)
    return 0


def fix_usings(doc, ns_of):
    text = doc.text
    m = NS_RE.search(text)
    own = m.group(1) if m else ''
    idents = set(re.findall(r'\b[A-Z]\w*\b', code_only(text)))
    needed = set()
    for t, ns in ns_of.items():
        if t in idents and not enclosing(own, ns):
            needed.add(ns)
    managed_needed = {n for n in needed if n.startswith('CMiX.UI.Controls')}
    # Remove the control usings that nothing needs now.
    for u in USING_RE.findall(text):
        if u.startswith('CMiX.UI.Controls') and u not in managed_needed:
            text = re.sub(r'^using ' + re.escape(u) + r';[ \t]*\n', '', text, count=1, flags=re.M)
    present = set(USING_RE.findall(text))
    # Add the missing ones. The Panels using is only added, never removed.
    for ns in sorted(needed):
        if ns not in present and (ns.startswith('CMiX.UI.Controls') or ns == 'CMiX.UI.Panels'):
            text = insert_using(text, ns)
    doc.text = text


def insert_using(text, ns):
    lines = text.split('\n')
    cmix = [i for i, l in enumerate(lines) if l.startswith('using CMiX.')]
    anyu = [i for i, l in enumerate(lines) if l.startswith('using ') and l.rstrip().endswith(';')]
    new = 'using ' + ns + ';'
    if cmix:
        for i in cmix:
            if lines[i].rstrip()[len('using '):-1] > ns:
                lines.insert(i, new)
                return '\n'.join(lines)
        lines.insert(cmix[-1] + 1, new)
        return '\n'.join(lines)
    if anyu:
        lines.insert(anyu[-1] + 1, new)
        return '\n'.join(lines)
    for i, l in enumerate(lines):
        if l.startswith('namespace '):
            lines[i:i] = [new, '']
            return '\n'.join(lines)
    return text


if __name__ == '__main__':
    sys.exit(main())
