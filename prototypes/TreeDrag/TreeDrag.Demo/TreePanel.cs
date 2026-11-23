using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using TreeDrag.Core;
using CoreRect = TreeDrag.Core.Rect;

namespace TreeDrag.Demo;

/// <summary>Shows the tree as a flat list of rows and moves a row by drag. Prototype: the panel makes its own rows.</summary>
public sealed class TreePanel : Panel
{
    public const double RowHeight = 28;
    public const double Indent = 20;
    public const double DragThreshold = 4;
    private const string ArrowName = "arrow";

    public static readonly StyledProperty<string?> SelectedIdProperty =
        AvaloniaProperty.Register<TreePanel, string?>(nameof(SelectedId));

    private static readonly IBrush RowBrush = Brushes.WhiteSmoke;
    private static readonly IBrush SelectedBrush = Brushes.LightSteelBlue;

    public static readonly StyledProperty<Node> RootProperty =
        AvaloniaProperty.Register<TreePanel, Node>(nameof(Root), Node.Branch("root"));

    private readonly Dictionary<string, Border> _rows = new();
    private readonly Dictionary<string, TextBlock> _arrows = new();
    private readonly Border _indicator = new()
    {
        BorderBrush = Brushes.DodgerBlue,
        BorderThickness = new Thickness(1),
        IsVisible = false,
        IsHitTestVisible = false,
    };

    private Dictionary<string, CoreRect> _home = new();
    private string? _dragId;
    private HashSet<string> _dragSubtree = new();
    private List<Candidate> _candidates = new();
    private Candidate? _best;
    private Point _grab;
    private string? _pressId;
    private Point _pressPoint;
    private IPointer? _pointer;

    static TreePanel() => AffectsMeasure<TreePanel>(RootProperty);

    public TreePanel()
    {
        Focusable = true;
        Background = Brushes.Transparent;
        Children.Add(_indicator);
        SyncRows();
    }

    public Node Root
    {
        get => GetValue(RootProperty);
        set => SetValue(RootProperty, value);
    }

    /// <summary>The rule canDrop(node, parent) of the author. Null allows every parent.</summary>
    public Func<Node, Node, bool>? CanDrop { get; set; }

    /// <summary>The id of the selected node, or null. It stays when the node moves or is hidden.</summary>
    public string? SelectedId
    {
        get => GetValue(SelectedIdProperty);
        set => SetValue(SelectedIdProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == RootProperty)
            SyncRows();
        else if (change.Property == SelectedIdProperty)
            UpdateSelection();
    }

    /// <summary>Makes one row for each visible node, removes the rows of hidden nodes and sets the arrows.</summary>
    private void SyncRows()
    {
        var visible = TreeOps.Layout(Root).Keys.ToHashSet();
        foreach (var id in _rows.Keys.Where(id => !visible.Contains(id)).ToList())
        {
            Children.Remove(_rows[id]);
            _rows.Remove(id);
            _arrows.Remove(id);
        }
        foreach (var id in visible.Where(id => !_rows.ContainsKey(id)))
        {
            var arrow = new TextBlock
            {
                Name = ArrowName,
                Width = 16,
                Background = Brushes.Transparent,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            var row = new Border
            {
                Tag = id,
                Background = RowBrush,
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(0, 0, 0, 1),
                Child = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(2, 0),
                    Children = { arrow, new TextBlock { Text = id, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(4, 0) } },
                },
                RenderTransform = new TranslateTransform(),
            };
            SetAnimated(row, true);
            _rows[id] = row;
            _arrows[id] = arrow;
            Children.Add(row);
        }
        foreach (var (id, arrow) in _arrows)
        {
            var node = TreeOps.Find(Root, id)!;
            arrow.Text = node.Children.Count == 0 ? "" : node.IsExpanded ? "▾" : "▸";
        }
        UpdateSelection();
    }

    private void UpdateSelection()
    {
        foreach (var (id, row) in _rows)
            row.Background = id == SelectedId ? SelectedBrush : RowBrush;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var width = double.IsInfinity(availableSize.Width) ? 240 : availableSize.Width;
        foreach (var child in Children)
            child.Measure(new Size(width, RowHeight));
        return new Size(width, _rows.Count * RowHeight);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        _home = TreeOps.Layout(Root, RowHeight, Indent, finalSize.Width);
        foreach (var (id, row) in _rows)
            row.Arrange(ToAvalonia(_home[id]));
        _indicator.Arrange(_best is null ? default : ToAvalonia(_best.Spot));
        return finalSize;
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (_dragId is not null || !e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            return;
        var (id, onArrow) = RowIdOf(e.Source);
        if (id is null)
        {
            SelectedId = null;
            return;
        }
        if (onArrow)
        {
            if (TreeOps.Find(Root, id)!.Children.Count > 0)
                Root = TreeOps.Toggle(Root, id);
            e.Handled = true;
            return;
        }

        _pressId = id;
        _pressPoint = e.GetPosition(this);
        e.Pointer.Capture(this);
        e.Handled = true;
    }

    /// <summary>Starts the drag of the row. The grab point is the press point, so the row does not jump.</summary>
    private void StartDrag(string id, Point press, IPointer pointer)
    {
        _pressId = null;
        _dragId = id;
        SelectedId = id;
        _grab = new Point(press.X - _home[id].X, press.Y - _home[id].Y);
        _dragSubtree = SubtreeIds(TreeOps.Find(Root, id)!).Where(_rows.ContainsKey).ToHashSet();
        _candidates = TreeOps.Candidates(Root, id, RowHeight, Indent, Bounds.Width, CanDrop);
        _best = null;
        foreach (var subId in _dragSubtree)
        {
            SetAnimated(_rows[subId], false);
            _rows[subId].ZIndex = 1;
        }
        _indicator.IsVisible = true;
        _pointer = pointer;
        Focus();
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        var p = e.GetPosition(this);
        if (_dragId is null && _pressId is not null
            && DragMath.PassedThreshold(_pressPoint.X, _pressPoint.Y, p.X, p.Y, DragThreshold))
            StartDrag(_pressId, _pressPoint, e.Pointer);
        if (_dragId is not null)
            UpdateDrag(p);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (_dragId is null)
        {
            SelectedId = _pressId;
            _pressId = null;
            return;
        }
        e.Handled = true;
        var best = _best;
        if (best is null)
        {
            EndDrag(animate: true);
            return;
        }

        // The new arrange puts each dragged row in its slot. The offset keeps it where it is now, then animates to 0.
        var newHome = TreeOps.Layout(best.Tree, RowHeight, Indent, Bounds.Width);
        var settle = _dragSubtree.ToDictionary(id => id, id =>
        {
            var t = (TranslateTransform)_rows[id].RenderTransform!;
            return new Point(_home[id].X + t.X - newHome[id].X, _home[id].Y + t.Y - newHome[id].Y);
        });
        EndDrag(animate: false);
        Root = best.Tree;
        foreach (var (id, offset) in settle)
        {
            var row = _rows[id];
            row.ZIndex = 1;
            SetAnimated(row, false);
            SetOffset(row, offset.X, offset.Y);
            SetAnimated(row, true);
            SetOffset(row, 0, 0);
        }
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        base.OnPointerCaptureLost(e);
        _pressId = null;
        if (_dragId is not null)
            EndDrag(animate: true);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Key == Key.Escape && _dragId is not null)
        {
            _pointer?.Capture(null);
            e.Handled = true;
        }
    }

    /// <summary>Moves the dragged rows with the pointer and the other rows to their place in the nearest candidate tree.</summary>
    private void UpdateDrag(Point p)
    {
        var x = p.X - _grab.X;
        var y = p.Y - _grab.Y;
        var home = _home[_dragId!];
        foreach (var subId in _dragSubtree)
            SetOffset(_rows[subId], x - home.X, y - home.Y);

        var best = TreeOps.Nearest(_candidates, x, y);
        if (ReferenceEquals(best, _best))
            return;
        _best = best;
        var target = TreeOps.Layout(best.Tree, RowHeight, Indent, Bounds.Width);
        foreach (var (id, row) in _rows)
            if (!_dragSubtree.Contains(id))
                SetOffset(row, target[id].X - _home[id].X, target[id].Y - _home[id].Y);
        InvalidateArrange();
    }

    private void EndDrag(bool animate)
    {
        foreach (var row in _rows.Values)
        {
            SetAnimated(row, animate);
            SetOffset(row, 0, 0);
            row.ZIndex = 0;
        }
        if (!animate)
            foreach (var row in _rows.Values)
                SetAnimated(row, true);
        _dragId = null;
        _best = null;
        _candidates = new();
        _indicator.IsVisible = false;
        _pointer = null;
    }

    /// <summary>Gives the id of the row under the source, and if the source is on its arrow.</summary>
    private (string? Id, bool OnArrow) RowIdOf(object? source)
    {
        var onArrow = false;
        for (var c = source as Control; c is not null; c = c.Parent as Control)
        {
            onArrow |= c.Name == ArrowName;
            if (c.Parent == this && c.Tag is string id)
                return (id, onArrow);
        }
        return (null, false);
    }

    private static IEnumerable<string> SubtreeIds(Node node) => node.Children.SelectMany(SubtreeIds).Prepend(node.Id);

    private static void SetOffset(Control row, double x, double y)
    {
        var t = (TranslateTransform)row.RenderTransform!;
        t.X = x;
        t.Y = y;
    }

    private static void SetAnimated(Control row, bool animated)
    {
        var t = (TranslateTransform)row.RenderTransform!;
        t.Transitions = animated
            ? new Transitions
            {
                new DoubleTransition { Property = TranslateTransform.XProperty, Duration = TimeSpan.FromMilliseconds(120), Easing = new CubicEaseOut() },
                new DoubleTransition { Property = TranslateTransform.YProperty, Duration = TimeSpan.FromMilliseconds(120), Easing = new CubicEaseOut() },
            }
            : null;
    }

    private static Avalonia.Rect ToAvalonia(CoreRect r) => new(r.X, r.Y, r.Width, r.Height);
}
