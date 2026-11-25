/* The sandbox of the UI kit: proposals that wait for a decision of the owner.
   A proposal shows one thing as it is now (Before) and as it could be (After), side by side.
   It changes nothing in CMiX and nothing in cmix.css: the After look is in sandbox.css, under the class of the proposal.
   When the owner decides, remove the proposal from this file and its block from sandbox.css.
   The number of a proposal (S1, S2 ...) is only for the discussion. It is not a fixed number of the catalogue.
   Do not use the number of an earlier proposal again. The last number used is S3.

   id: the number. title: what it is about. question: the decision to make, as one question.
   why: the problem of today. effect: what changes in CMiX when the answer is yes.
   before, after: one line under each side. facts: measured values, as [label, before, after].
   sample: the markup of the controls, the same for the two sides. The After side has the class "sb-" + id around it. */
window.CMIX_SANDBOX = [
{
    id: 'S2',
    title: 'Status with a shape and a word',
    question: 'Does every status show a shape and a word, and not a colour only?',
    why: 'A status dot is a colour only. The depth 3 row is the same green as Connected, and the camera row is almost the red of Down. A colour blind user can not tell Connected from Down.',
    effect: 'StatusIndicators gets a shape per state and a short word. The row colours of the tree do not change. A later proposal moves the row hues out of green, orange and red.',
    before: 'Four dots. Only the colour tells the state.',
    after: 'Filled circle: Connected. Ring: Connecting. Circle with a bar: Out of sync. Hollow square: Down. The word is next to it.',
    facts: [
        ['Connected vs depth 3 row (dE)', '2.0, one colour', 'shape and word differ'],
        ['Down vs camera row (dE)', '3.9', 'shape and word differ'],
        ['Connected vs Down, deuteranopia (dE)', '3.5', 'shape and word differ']
    ],
    sample: "<div style=\"display:flex;gap:16px;align-items:flex-start;background:var(--TreeBackground);padding:8px\"><div style=\"width:190px;display:flex;flex-direction:column;gap:2px\"><div style=\"display:flex;align-items:center;gap:6px;height:22px;padding-left:8px;background:color-mix(in srgb, var(--CategoryComposable0) 18%, var(--TreeBackground));color:#E6E6E8;font:12px/22px sans-serif\"><i style=\"width:8px;height:8px;border-radius:2px;background:var(--CategoryComposable0)\"></i>Show</div><div style=\"display:flex;align-items:center;gap:6px;height:22px;padding-left:20px;background:color-mix(in srgb, var(--CategoryComposable1) 18%, var(--TreeBackground));color:#E6E6E8;font:12px/22px sans-serif\"><i style=\"width:8px;height:8px;border-radius:2px;background:var(--CategoryComposable1)\"></i>Layer A</div><div style=\"display:flex;align-items:center;gap:6px;height:22px;padding-left:32px;background:color-mix(in srgb, var(--CategoryComposable2) 18%, var(--TreeBackground));color:#E6E6E8;font:12px/22px sans-serif\"><i style=\"width:8px;height:8px;border-radius:2px;background:var(--CategoryComposable2)\"></i>Layer B</div><div style=\"display:flex;align-items:center;gap:6px;height:22px;padding-left:44px;background:color-mix(in srgb, var(--CategoryComposable3) 55%, var(--TreeBackground));color:#E6E6E8;font:12px/22px sans-serif\"><i style=\"width:8px;height:8px;border-radius:2px;background:var(--CategoryComposable3)\"></i>Layer C (depth 3)</div><div style=\"display:flex;align-items:center;gap:6px;height:22px;padding-left:20px;background:color-mix(in srgb, var(--CategoryCamera) 18%, var(--TreeBackground));color:#E6E6E8;font:12px/22px sans-serif\"><i style=\"width:8px;height:8px;border-radius:2px;background:var(--CategoryCamera)\"></i>Camera</div><div style=\"display:flex;align-items:center;gap:6px;height:22px;padding-left:20px;background:color-mix(in srgb, var(--CategoryLight) 18%, var(--TreeBackground));color:#E6E6E8;font:12px/22px sans-serif\"><i style=\"width:8px;height:8px;border-radius:2px;background:var(--CategoryLight)\"></i>Light</div></div><div style=\"display:flex;flex-direction:column;gap:6px;padding:6px 8px;background:var(--Grey200);border-radius:3px\"><div class=\"s2-peer s2-ok\" style=\"display:flex;align-items:center;gap:6px;font:12px/20px sans-serif;color:#C8C8CC\"><span class=\"s2-dot\" style=\"width:8px;height:8px;border-radius:50%;background:var(--StatusSuccess);color:var(--StatusSuccess)\"></span><span>Engine 1</span><span class=\"s2-word\" style=\"display:none;color:var(--StatusSuccess);font-weight:700\">Connected</span></div><div class=\"s2-peer s2-down\" style=\"display:flex;align-items:center;gap:6px;font:12px/20px sans-serif;color:#C8C8CC\"><span class=\"s2-dot\" style=\"width:8px;height:8px;border-radius:50%;background:var(--PopupCardDanger);color:var(--PopupCardDanger)\"></span><span>Engine 2</span><span class=\"s2-word\" style=\"display:none;color:var(--PopupCardDanger);font-weight:700\">Down</span></div><div class=\"s2-peer s2-warn\" style=\"display:flex;align-items:center;gap:6px;font:12px/20px sans-serif;color:#C8C8CC\"><span class=\"s2-dot\" style=\"width:8px;height:8px;border-radius:50%;background:var(--PopupCardWarning);color:var(--PopupCardWarning)\"></span><span>Engine 3</span><span class=\"s2-word\" style=\"display:none;color:var(--PopupCardWarning);font-weight:700\">Out of sync</span></div><div class=\"s2-peer s2-wait\" style=\"display:flex;align-items:center;gap:6px;font:12px/20px sans-serif;color:#C8C8CC\"><span class=\"s2-dot\" style=\"width:8px;height:8px;border-radius:50%;background:var(--Grey800);color:var(--Grey800)\"></span><span>Engine 4</span><span class=\"s2-word\" style=\"display:none;color:var(--Grey800);font-weight:700\">Connecting</span></div></div></div>"
},
{
    id: 'S3',
    title: 'Search match mark of the tree',
    question: 'During a search of the tree, does a match row show the accent brush on its mark at the left?',
    why: '',
    effect: 'RowListTree gets the pseudo class :searchmatch. Its Border#mark shows the AccentBrush. The name keeps the rest text colour. The cursor frame and the selection do not change.',
    before: '',
    after: '',
    sample: (function () {
        function row(name, depth, kind, state) {
            var selected = state.indexOf('selected') >= 0;
            var cursor = state.indexOf('cursor') >= 0;
            var match = state.indexOf('match') >= 0;
            var rowStyle = 'position:relative;height:var(--TreeRowHeight);margin-bottom:var(--TreeRowGap)';
            if (selected) { rowStyle += ';background:var(--Grey200)'; }
            var band = 'position:absolute;top:0;bottom:0;right:0;left:calc(var(--TreeIndent) * ' + depth + ');display:flex;align-items:center;padding-left:var(--TreeSelectionThickness);border-radius:var(--CornerRadiusAll)';
            if (selected) { band += ';background:transparent'; }
            else { band += ';background:color-mix(in srgb, var(--ActiveText) 5%, transparent)'; }
            if (cursor) { band += ';box-shadow:inset 0 0 0 var(--TreeSelectionThickness) ' + kind; }
            var text = 'font-size:var(--FontSizeMD);font-family:sans-serif;white-space:nowrap;color:var(--TreeTextRest)';
            if (selected) { text = text.replace('var(--TreeTextRest)', 'var(--ActiveText)'); }
            var mark = 'position:absolute;top:0;bottom:0;left:0;width:var(--TreeRowGap);display:none;background:' + kind;
            if (selected) { mark = mark.replace('display:none', 'display:block'); }
            return '<div class="s3-row' + (match ? ' s3-match' : '') + '" style="' + rowStyle + '">'
                + '<div style="' + band + '">'
                + '<i style="flex:none;width:calc(var(--IconSizeXS) / 2);height:calc(var(--IconSizeXS) / 2);margin:0 calc(var(--IconSizeXS) / 4);border-radius:var(--CornerRadiusAll);background:' + kind + '"></i>'
                + '<span style="' + text + '">' + name + '</span></div>'
                + '<div class="s3-mark" style="' + mark + '"></div></div>';
        }
        return '<div style="min-width:calc(var(--TreeToggleWidth) * 4);padding:var(--TreeToolbarPadding);background:var(--TreeBackground)">'
            + row('Show', 0, 'var(--CategoryComposable0)', '')
            + row('Layer A', 1, 'var(--CategoryComposable1)', 'selected')
            + row('Effects', 2, 'var(--Grey800)', '')
            + row('Blur', 3, 'var(--CategoryFilter)', 'match')
            + row('Blur Mask', 3, 'var(--CategoryFilter)', 'match')
            + row('Blur Glow', 3, 'var(--CategoryFilter)', 'match cursor')
            + '</div>';
    })()
}
];
