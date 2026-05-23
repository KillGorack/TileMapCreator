using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using SkiaSharp;
using System;
using System.IO;
using System.Threading.Tasks;
namespace TileMapCreator.Views;

public partial class MainWindow : Window
{
    public static readonly int[][] TileSetRecipe = new int[][]
    {
        new[] { 1, 2, 3, 6, 7, 8, 9 },
        new[] { 1, 2, 3, 4, 7 },
        new[] { 1, 2, 3, 4, 7, 9 },
        new[] { 1, 2, 3, 7, 9 },
        new[] { 1, 3, 4, 7 },
        new[] { 1, 4, 7 },
        new[] { 7, 9 },
        new[] { 9 },
        new[] { 1, 2, 3, 7, 8, 9 },
        new[] { 1, 3, 7 },
        Array.Empty<int>(),
        new[] { 1, 3, 4, 6, 7, 8, 9 },
        new[] { 1, 2, 3, 6, 9 },
        new[] { 1, 2, 3, 6, 7, 9 },
        new[] { 1, 3, 7, 8, 9 },
        new[] { 1, 3, 6, 9 },
        new[] { 3, 6, 9 },
        new[] { 1, 3 },
        new[] { 7 },
        new[] { 1, 3, 4, 6, 7, 9 },
        new[] { 3, 7, 9 },
        new[] { 1, 2, 3, 4, 6, 7, 8, 9 },
        new[] { 1, 2, 3, 4, 6, 7, 9 },
        new[] { 1, 4, 7, 8, 9 },
        new[] { 1, 3, 4, 7, 8, 9 },
        new[] { 1, 3, 4, 7, 9 },
        new[] { 1, 2, 3, 7 },
        new[] { 1, 2, 3 },
        new[] { 3, 9 },
        new[] { 3 },
        new[] { 3, 7 },
        new[] { 1, 3, 9 },
        new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
        new[] { 1, 2, 3, 4, 7, 8, 9 },
        new[] { 3, 6, 7, 8, 9 },
        new[] { 1, 3, 6, 7, 8, 9 },
        new[] { 1, 3, 6, 7, 9 },
        new[] { 1, 2, 3, 9 },
        new[] { 7, 8, 9 },
        new[] { 1, 7 },
        new[] { 1 },
        new[] { 1, 9 },
        new[] { 1, 3, 7 },
        new[] { 1, 3, 7, 9 },
    };

    public static readonly int[][] TileSetRecipeB = new int[][]
    {
        new[] { 2, 3, 4 },
        new[] { 1, 3, 4 },
        new[] { 1, 2, 4 },
        new[] { 1, 2, 3 },
        new[] { 1, 2 },
        new[] { 3, 4 },
        new[] { 1, 3 },
        new[] { 2, 4 },
        new[] { 1 },
        new[] { 2 },
        new[] { 3 },
        new[] { 4 },
        new[] { 2, 3 },
        new[] { 1, 4 },
        Array.Empty<int>(),
        new[] { 1, 2, 3, 4 },
    };


    private SKBitmap? _borderBitmap;
    private SKBitmap? _tileBitmap;

    public MainWindow()
    {
        InitializeComponent();
        this.SizeChanged += (_, _) =>
        {
            UpdateEditHostSize();
            Edit_Screen.LayoutUpdated += OnEditScreenLayoutUpdated;
        };
    }


    private static Avalonia.Media.Imaging.Bitmap ToAvaloniaBitmap(SKBitmap sk)
    {
        using var img = SKImage.FromBitmap(sk);
        using var data = img.Encode(SKEncodedImageFormat.Png, 100);
        using var ms = new MemoryStream(data.ToArray());
        return new Avalonia.Media.Imaging.Bitmap(ms);
    }

    private void UpdateEditHostSize()
    {
        var col2 = EditHost.Parent as Control;
        if (col2 == null) return;
        double size = Math.Min(col2.Bounds.Width, col2.Bounds.Height);
        EditHost.Width  = size;
        EditHost.Height = size;
    }

    private void SetBorderBitmap(SKBitmap bmp)
    {
        _borderBitmap?.Dispose();
        _borderBitmap = bmp;
        Border_Map.Source = ToAvaloniaBitmap(bmp);
        Edit_Screen.Source = ToAvaloniaBitmap(bmp);
        
        Edit_Screen.LayoutUpdated += OnEditScreenLayoutUpdated;
    }

    private void OnEditScreenLayoutUpdated(object? sender, EventArgs e)
    {
        Edit_Screen.LayoutUpdated -= OnEditScreenLayoutUpdated;
        RefreshLinesFromInput();
    }

    private async Task<IStorageFile?> PickOpenFile(string title)
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = false,
            FileTypeFilter = new[] { new FilePickerFileType("PNG") { Patterns = new[] { "*.png" } } }
        });
        return files.Count > 0 ? files[0] : null;
    }

    private async Task<IStorageFile?> PickSaveFile()
    {
        return await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save tileset as...",
            SuggestedFileName = $"TileSet_{DateTime.Now:yyyyMMddHHmmss}.png",
            DefaultExtension = "png",
            FileTypeChoices = new[] { new FilePickerFileType("PNG") { Patterns = new[] { "*.png" } } }
        });
    }

    private async void Add_Border_Click(object? sender, RoutedEventArgs e)
    {
        var file = await PickOpenFile("Select Border Image");
        if (file == null) return;
        await using var stream = await file.OpenReadAsync();
        SetBorderBitmap(SKBitmap.Decode(stream));
    }

    private async void Add_Tile_Click(object? sender, RoutedEventArgs e)
    {
        var file = await PickOpenFile("Select Tile Image");
        if (file == null) return;
        await using var stream = await file.OpenReadAsync();
        _tileBitmap?.Dispose();
        _tileBitmap = SKBitmap.Decode(stream);
        Tile_Map.Source = ToAvaloniaBitmap(_tileBitmap);
    }

    private void Remove_Click(object? sender, RoutedEventArgs e)
    {
        _borderBitmap?.Dispose(); _borderBitmap = null;
        _tileBitmap?.Dispose();   _tileBitmap   = null;
        Border_Map.Source  = null;
        Tile_Map.Source    = null;
        Edit_Screen.Source = null;
        GridOverlay.Children.Clear();
    }

    private void TextChanged(object? sender, TextChangedEventArgs e) =>
        RefreshLinesFromInput();

    private async void BuildTileset_Click(object? sender, RoutedEventArgs e)
    {
        if (_borderBitmap == null || _tileBitmap == null ||
            !int.TryParse(Border_Size.Text, out int borderSize))
        {
            await ShowMessage("Missing images or invalid border size.");
            return;
        }

        int w = _borderBitmap.Width, h = _borderBitmap.Height;
        if (borderSize > w / 2 || borderSize > h / 2)
        {
            await ShowMessage("Border size is too large for the texture.");
            return;
        }

        int left = borderSize, top = borderSize,
            right = w - borderSize, bottom = h - borderSize;

        bool isHalf = borderSize * 2 == w && borderSize * 2 == h;

        var outFile = await PickSaveFile();
        if (outFile == null) return;

        SKBitmap sheet = isHalf
            ? BuildSheetB(_borderBitmap, _tileBitmap, top, left, right, bottom)
            : BuildSheet (_borderBitmap, _tileBitmap, top, left, right, bottom);

        await using var ws = await outFile.OpenWriteAsync();
        sheet.Encode(ws, SKEncodedImageFormat.Png, 100);
        sheet.Dispose();

        await ShowMessage($"Tileset saved: {outFile.Name}");
    }


    private void RefreshLinesFromInput()
    {
        if (_borderBitmap == null || !int.TryParse(Border_Size.Text, out int bs)) return;
        int w = _borderBitmap.Width, h = _borderBitmap.Height;
        if (bs > w / 2 || bs > h / 2) return;
        UpdateGridOverlay(bs, bs, w - bs, h - bs);
    }

    private void UpdateGridOverlay(int top, int left, int right, int bottom)
    {
        GridOverlay.Children.Clear();
        if (Edit_Screen.Source == null) return;

        double dw = Edit_Screen.Bounds.Width;
        double dh = Edit_Screen.Bounds.Height;
        if (dw <= 0 || dh <= 0) return;

        int pw = _borderBitmap!.Width, ph = _borderBitmap.Height;
        double sx = dw / pw, sy = dh / ph;

        GridOverlay.Width  = dw;
        GridOverlay.Height = dh;

        double uL = left * sx, uR = right * sx, uT = top * sy, uB = bottom * sy;

        GridOverlay.Children.Add(MakeLine(uL, 0, uL, dh));
        GridOverlay.Children.Add(MakeLine(uR, 0, uR, dh));
        GridOverlay.Children.Add(MakeLine(0, uT, dw, uT));
        GridOverlay.Children.Add(MakeLine(0, uB, dw, uB));
    }

    private static Line MakeLine(double x1, double y1, double x2, double y2) => new()
    {
        StartPoint = new Point(x1, y1),
        EndPoint   = new Point(x2, y2),
        Stroke     = Brushes.Red,
        StrokeThickness = 1,
    };

    private static SKBitmap BuildSheet(SKBitmap border, SKBitmap tile,
        int top, int left, int right, int bottom)
    {
        const int cols = 11, rows = 4;
        int tw = tile.Width, th = tile.Height;
        var sheet = new SKBitmap(cols * tw, rows * th);

        int bw = border.Width, bh = border.Height;
        var regions = new SKRectI[]
        {
            new(0,     0,   left,  top),
            new(left,  0,   right, top),
            new(right, 0,   bw,    top),

            new(0,     top,    left,  bottom),
            new(left,  top,    right, bottom),
            new(right, top,    bw,    bottom),

            new(0,     bottom, left,  bh),
            new(left,  bottom, right, bh),
            new(right, bottom, bw,    bh),
        };

        using var canvas = new SKCanvas(sheet);
        canvas.Clear(SKColors.Transparent);

        for (int i = 0; i < TileSetRecipe.Length; i++)
        {
            int col = i % cols, row = i / cols;
            float bx = col * tw, by = row * th;

            canvas.DrawBitmap(tile, new SKRect(bx, by, bx + tw, by + th));

            foreach (int si in TileSetRecipe[i])
            {
                if (si < 1 || si > 9) continue;
                var r = regions[si - 1];
                canvas.DrawBitmap(border, r,
                    new SKRect(bx + r.Left, by + r.Top, bx + r.Right, by + r.Bottom));
            }
        }

        return sheet;
    }

    private static SKBitmap BuildSheetB(SKBitmap border, SKBitmap tile,
        int top, int left, int right, int bottom)
    {
        const int cols = 4, rows = 4;
        int tw = tile.Width, th = tile.Height;
        var sheet = new SKBitmap(cols * tw, rows * th);

        int bw = border.Width, bh = border.Height;
        var regions = new SKRectI[]
        {
            new(0,    0,   left, top),
            new(left, 0,   bw,   top),
            new(0,    top, left, bh),
            new(left, top, bw,   bh),
        };

        using var canvas = new SKCanvas(sheet);
        canvas.Clear(SKColors.Transparent);

        for (int i = 0; i < TileSetRecipeB.Length; i++)
        {
            int col = i % cols, row = i / cols;
            float bx = col * tw, by = row * th;

            canvas.DrawBitmap(tile, new SKRect(bx, by, bx + tw, by + th));

            foreach (int si in TileSetRecipeB[i])
            {
                if (si < 1 || si > 4) continue;
                var r = regions[si - 1];
                canvas.DrawBitmap(border, r,
                    new SKRect(bx + r.Left, by + r.Top, bx + r.Right, by + r.Bottom));
            }
        }

        return sheet;
    }

    private async Task ShowMessage(string msg)
    {
        var dlg = new Window
        {
            Width = 360, Height = 120,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = new StackPanel
            {
                Margin = new Thickness(16),
                Children =
                {
                    new TextBlock { Text = msg, TextWrapping = TextWrapping.Wrap },
                    new Button    { Content = "OK", Margin = new Thickness(0,12,0,0),
                        HorizontalAlignment = HorizontalAlignment.Right }
                }
            }
        };
        var sp = (StackPanel)dlg.Content!;
        ((Button)sp.Children[1]).Click += (_, _) => dlg.Close();
        await dlg.ShowDialog(this);
    }
}