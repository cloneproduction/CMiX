# Makes tokens.css and tokens.js of the UI kit from the theme of CMiX.UI.
# Run it after a change of a token:  powershell -File docs/ui-kit/tools/Update-Tokens.ps1
# The names of the CSS variables are the keys of the theme, for example --Grey100.

$ErrorActionPreference = 'Stop'

$kit = Split-Path -Parent $PSScriptRoot
$repo = Split-Path -Parent (Split-Path -Parent $kit)
$themes = Join-Path $repo 'CMiX.UI\Themes'
$xamlNamespace = 'http://schemas.microsoft.com/winfx/2006/xaml'
$invariant = [Globalization.CultureInfo]::InvariantCulture

$namedColors = @{ Black = '#000000'; White = '#FFFFFF' }
$easings = @{ CubicEaseOut = 'cubic-bezier(0.215, 0.61, 0.355, 1)' }

$skipped = New-Object System.Collections.ArrayList

# '#AARRGGBB' of Avalonia to '#RRGGBB' or '#RRGGBBAA' of CSS.
function ConvertTo-CssColor([string]$value) {
    $v = $value.Trim()
    if ($namedColors.ContainsKey($v)) { return $namedColors[$v] }
    if ($v -match '^#([0-9a-fA-F]{2})([0-9a-fA-F]{6})$') {
        if ($Matches[1] -ieq 'FF') { return ('#' + $Matches[2]).ToUpperInvariant() }
        return ('#' + $Matches[2] + $Matches[1]).ToUpperInvariant()
    }
    if ($v -match '^#[0-9a-fA-F]{6}$') { return $v.ToUpperInvariant() }
    return $null
}

function ConvertTo-Px([string]$number) {
    $n = $number.Trim()
    if ([double]::Parse($n, $invariant) -eq 0) { return '0' }
    return $n + 'px'
}

# Thickness of Avalonia: 'all', 'horizontal,vertical' or 'left,top,right,bottom'. CSS: 'top right bottom left'.
function ConvertTo-CssThickness([string]$value) {
    $p = @($value.Split(',') | ForEach-Object { ConvertTo-Px $_ })
    if ($p.Count -eq 1) { return $p[0] }
    if ($p.Count -eq 2) { return $p[1] + ' ' + $p[0] }
    if ($p.Count -eq 4) { return $p[1] + ' ' + $p[2] + ' ' + $p[3] + ' ' + $p[0] }
    return $null
}

# CornerRadius of Avalonia and border-radius of CSS have the same order of corners.
function ConvertTo-CssRadius([string]$value) {
    return (@($value.Split(',') | ForEach-Object { ConvertTo-Px $_ }) -join ' ')
}

function ConvertTo-CssBrush($node) {
    $color = $node.GetAttribute('Color')
    $opacity = $node.GetAttribute('Opacity')
    $css = $null
    if ($color -match '^\{(?:DynamicResource|StaticResource)\s+(\w+)\}$') { $css = 'var(--' + $Matches[1] + ')' }
    else { $css = ConvertTo-CssColor $color }
    if (-not $css) { return $null }
    if ($opacity) {
        $percent = ([double]::Parse($opacity, $invariant) * 100).ToString('0.##', $invariant)
        $css = 'color-mix(in srgb, ' + $css + ' ' + $percent + '%, transparent)'
    }
    return $css
}

function Read-Dictionary([string]$file) {
    $path = Join-Path $themes $file
    $xml = New-Object System.Xml.XmlDocument
    $xml.Load($path)
    $tokens = New-Object System.Collections.ArrayList
    $note = ''
    foreach ($node in $xml.DocumentElement.ChildNodes) {
        if ($node.NodeType -eq [System.Xml.XmlNodeType]::Comment) {
            $note = ($node.Value -replace '\s+', ' ').Trim()
            continue
        }
        if ($node.NodeType -ne [System.Xml.XmlNodeType]::Element) { continue }
        $key = $node.GetAttribute('Key', $xamlNamespace)
        if (-not $key) { continue }
        $type = $node.LocalName
        $raw = $node.InnerText.Trim()
        $css = $null
        switch ($type) {
            'Color' { $css = ConvertTo-CssColor $raw }
            'SolidColorBrush' {
                $css = ConvertTo-CssBrush $node
                $raw = $node.GetAttribute('Color')
                if ($node.GetAttribute('Opacity')) { $raw = $raw + ', Opacity ' + $node.GetAttribute('Opacity') }
            }
            'CornerRadius' { $css = ConvertTo-CssRadius $raw }
            'Thickness' { $css = ConvertTo-CssThickness $raw }
            'Double' { if ($key -like '*Opacity*') { $css = $raw } else { $css = ConvertTo-Px $raw } }
            'Int32' { $css = $raw }
            'FontFamily' { $css = $raw }
        }
        if ($css) {
            [void]$tokens.Add([ordered]@{ name = $key; type = $type; raw = $raw; css = $css; note = $note })
        }
        else {
            [void]$skipped.Add([ordered]@{ name = $key; type = $type; source = $file })
        }
        $note = ''
    }
    return , $tokens
}

function Read-Motion {
    $text = [IO.File]::ReadAllText((Join-Path $themes 'Motion.cs'))
    $tokens = New-Object System.Collections.ArrayList
    foreach ($m in [regex]::Matches($text, '(\w+)\s*=\s*TimeSpan\.FromMilliseconds\((\d+)\)')) {
        [void]$tokens.Add([ordered]@{
                name = 'Motion' + $m.Groups[1].Value; type = 'TimeSpan'
                raw = $m.Groups[2].Value + ' ms'; css = $m.Groups[2].Value + 'ms'; note = 'Motion.' + $m.Groups[1].Value
            })
    }
    $e = [regex]::Match($text, 'Easing\s*=>\s*new\s+(\w+)\(')
    if ($e.Success -and $easings.ContainsKey($e.Groups[1].Value)) {
        [void]$tokens.Add([ordered]@{
                name = 'MotionEasing'; type = 'Easing'
                raw = $e.Groups[1].Value; css = $easings[$e.Groups[1].Value]; note = 'Motion.Easing'
            })
    }
    else {
        [void]$skipped.Add([ordered]@{ name = 'MotionEasing'; type = 'Easing'; source = 'Motion.cs' })
    }
    return , $tokens
}

$groups = @(
    [ordered]@{ id = '0.1'; name = 'Colors'; source = 'CMiX.UI/Themes/Tokens/Colors.axaml'; tokens = (Read-Dictionary 'Tokens\Colors.axaml') },
    [ordered]@{ id = '0.2'; name = 'Brushes'; source = 'CMiX.UI/Themes/Tokens/Brushes.axaml'; tokens = (Read-Dictionary 'Tokens\Brushes.axaml') },
    [ordered]@{ id = '0.3'; name = 'Geometry'; source = 'CMiX.UI/Themes/Tokens/Geometry.axaml'; tokens = (Read-Dictionary 'Tokens\Geometry.axaml') },
    [ordered]@{ id = '0.4'; name = 'Typography'; source = 'CMiX.UI/Themes/Tokens/Typography.axaml'; tokens = (Read-Dictionary 'Tokens\Typography.axaml') },
    [ordered]@{ id = '0.5'; name = 'Motion'; source = 'CMiX.UI/Themes/Motion.cs'; tokens = (Read-Motion) }
)

$css = New-Object System.Text.StringBuilder
[void]$css.AppendLine('/* Made by tools/Update-Tokens.ps1 from CMiX.UI/Themes. Do not edit. */')
[void]$css.AppendLine(':root {')
foreach ($g in $groups) {
    [void]$css.AppendLine('    /* ' + $g.id + ' ' + $g.name + ': ' + $g.source + ' */')
    foreach ($t in $g.tokens) { [void]$css.AppendLine('    --' + $t.name + ': ' + $t.css + ';') }
}
[void]$css.AppendLine('}')

$json = [ordered]@{ groups = $groups; skipped = $skipped } | ConvertTo-Json -Depth 6
$js = "/* Made by tools/Update-Tokens.ps1 from CMiX.UI/Themes. Do not edit. */`r`nwindow.CMIX_TOKENS = " + $json + ";`r`n"

$utf8 = New-Object System.Text.UTF8Encoding($false)
[IO.File]::WriteAllText((Join-Path $kit 'tokens.css'), $css.ToString(), $utf8)
[IO.File]::WriteAllText((Join-Path $kit 'tokens.js'), $js, $utf8)

$count = 0
foreach ($g in $groups) { $count += $g.tokens.Count }
Write-Output ("Tokens: " + $count)
foreach ($s in $skipped) { Write-Output ("Skipped: " + $s.name + " (" + $s.type + ", " + $s.source + ")") }
