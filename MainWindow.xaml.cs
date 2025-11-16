using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.Generic;



namespace TileMapCreator
{
    public partial class MainWindow : Window
    {

        public static readonly int[][] TileSetRecipe = new int[][]
        {
            new int[] { },
            new int[] { 1, 2, 3, 4, 6, 7, 8, 9 },
            new int[] { 1, 2, 3, 6, 7, 8, 9 },
            new int[] { 1, 3, 4, 6, 7, 8, 9 },
            new int[] { 1, 2, 3, 4, 6, 7, 9 },
            new int[] { 1, 2, 3, 4, 7, 8, 9 },
            new int[] { 1, 2, 3, 4, 7, 9 },
            new int[] { 1, 2, 3, 6, 7, 9 },
            new int[] { 1, 2, 3, 7, 8, 9 },
            new int[] { 1, 3, 4, 7, 8, 9 },
            new int[] { 1, 3, 6, 7, 8, 9 },
            new int[] { 1, 3, 4, 6, 7, 9 },
            new int[] { 1, 2, 3, 7, 9 },
            new int[] { 1, 3, 7, 8, 9 },
            new int[] { 1, 2, 3, 4, 7 },
            new int[] { 1, 2, 3, 6, 9 },
            new int[] { 1, 3, 4, 7, 9 },
            new int[] { 1, 3, 6, 7, 9 },
            new int[] { 1, 4, 7, 8, 9 },
            new int[] { 3, 6, 7, 8, 9 },
            new int[] { 1, 3, 7, 9 },
            new int[] { 1, 3, 4, 7 },
            new int[] { 1, 3, 6, 9 },
            new int[] { 1, 2, 3, 7 },
            new int[] { 1, 2, 3, 9 },
            new int[] { 1, 4, 7 },
            new int[] { 3, 6, 9 },
            new int[] { 1, 2, 3 },
            new int[] { 7, 8, 9 },
            new int[] { 1, 3, 7 },
            new int[] { 1, 3, 9 },
            new int[] { 7, 9 },
            new int[] { 1, 3 },
            new int[] { 3, 9 },
            new int[] { 1, 7 },
            new int[] { 3, 7 },
            new int[] { 9 },
            new int[] { 7 },
            new int[] { 3 },
            new int[] { 1 },
        };

        public static readonly int[][] TileSetRecipeB = new int[][]
        {
            new int[] { 2, 3, 4 },
            new int[] { 1, 3, 4 },
            new int[] { 1, 2, 4 },
            new int[] { 1, 2, 3 },
            new int[] { 1, 2 },
            new int[] { 3, 4 },
            new int[] { 1, 3 },
            new int[] { 2, 4 },
            new int[] { 1 },
            new int[] { 2 },
            new int[] { 3 },
            new int[] { 4 },
            new int[] { 2, 3 },
            new int[] { 1, 4 },
            new int[] {  },
            new int[] { 1, 2, 3, 4 }
        };

        public MainWindow()
        {
            InitializeComponent();
        }



        private string SelectSavePath()
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Title = "Save tileset as...",
                Filter = "PNG Image (*.png)|*.png",
                FileName = $"TileSet_{DateTime.Now:yyyyMMddHHmmss}.png",
                DefaultExt = ".png",
                AddExtension = true
            };

            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        private void BuildTilesetSpriteSheet(BitmapSource borderSource, BitmapSource tileBase, int top, int left, int right, int bottom)
        {
            if (TileSetRecipe.Length != 40)
            {
                MessageBox.Show($"Expected 40 recipes, found {TileSetRecipe.Length}.");
                return;
            }

            int tileW = tileBase.PixelWidth;
            int tileH = tileBase.PixelHeight;

            int cols = 10;
            int rows = 4;

            int sheetW = cols * tileW;
            int sheetH = rows * tileH;

            // Slice border into 9 pieces
            var slices = new List<BitmapSource>();
            Int32Rect[] regions = new Int32Rect[]
            {
                new Int32Rect(0, 0, left, top),
                new Int32Rect(left, 0, right - left, top),
                new Int32Rect(right, 0, borderSource.PixelWidth - right, top),

                new Int32Rect(0, top, left, bottom - top),
                new Int32Rect(left, top, right - left, bottom - top),
                new Int32Rect(right, top, borderSource.PixelWidth - right, bottom - top),

                new Int32Rect(0, bottom, left, borderSource.PixelHeight - bottom),
                new Int32Rect(left, bottom, right - left, borderSource.PixelHeight - bottom),
                new Int32Rect(right, bottom, borderSource.PixelWidth - right, borderSource.PixelHeight - bottom),
            };

            foreach (var r in regions)
                slices.Add(new CroppedBitmap(borderSource, r));

            // Ask user for save path
            string outPath = SelectSavePath();
            if (outPath == null) return;

            // Build the spritesheet
            DrawingVisual sheetVisual = new DrawingVisual();
            using (DrawingContext sheetDC = sheetVisual.RenderOpen())
            {
                sheetDC.DrawRectangle(Brushes.Transparent, null, new Rect(0, 0, sheetW, sheetH));

                for (int i = 0; i < TileSetRecipe.Length; i++)
                {
                    int col = i % cols;
                    int row = i / cols;

                    double baseX = col * tileW;
                    double baseY = row * tileH;

                    // Draw base tile
                    sheetDC.DrawImage(tileBase, new Rect(baseX, baseY, tileW, tileH));

                    // Overlay slices per recipe
                    foreach (int sliceIndex in TileSetRecipe[i])
                    {
                        if (sliceIndex < 1 || sliceIndex > 9) continue;
                        int idx = sliceIndex - 1;
                        Int32Rect r = regions[idx];
                        var destRect = new Rect(baseX + r.X, baseY + r.Y, r.Width, r.Height);
                        sheetDC.DrawImage(slices[idx], destRect);
                    }
                }
            }

            // Render and save
            RenderTargetBitmap rtb = new RenderTargetBitmap(sheetW, sheetH, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(sheetVisual);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));

            using (var fs = new System.IO.FileStream(outPath, System.IO.FileMode.Create))
            {
                encoder.Save(fs);
            }

            MessageBox.Show($"Tileset saved: {outPath}");
        }




        private void BuildTilesetSpriteSheetB(BitmapSource borderSource, BitmapSource tileBase, int top, int left, int right, int bottom)
        {
            if (TileSetRecipeB.Length != 16)
            {
                MessageBox.Show($"Expected 16 recipes, found {TileSetRecipeB.Length}.");
                return;
            }

            int tileW = tileBase.PixelWidth;
            int tileH = tileBase.PixelHeight;

            int cols = 4;
            int rows = 4;

            int sheetW = cols * tileW;
            int sheetH = rows * tileH;

            // Slice border into 4 quadrants
            var slices = new List<BitmapSource>();
            Int32Rect[] regions = new Int32Rect[]
            {
                // Top-left
                new Int32Rect(0, 0, left, top),
                // Top-right
                new Int32Rect(left, 0, borderSource.PixelWidth - left, top),
                // Bottom-left
                new Int32Rect(0, top, left, borderSource.PixelHeight - top),
                // Bottom-right
                new Int32Rect(left, top, borderSource.PixelWidth - left, borderSource.PixelHeight - top),
            };

            foreach (var r in regions)
            {
                if (r.Width > 0 && r.Height > 0)
                    slices.Add(new CroppedBitmap(borderSource, r));
                else
                    slices.Add(null); // handle zero-sized gracefully
            }

            // Ask user for save path
            string outPath = SelectSavePath();
            if (outPath == null) return;

            // Build the spritesheet
            DrawingVisual sheetVisual = new DrawingVisual();
            using (DrawingContext sheetDC = sheetVisual.RenderOpen())
            {
                sheetDC.DrawRectangle(Brushes.Transparent, null, new Rect(0, 0, sheetW, sheetH));

                for (int i = 0; i < TileSetRecipeB.Length; i++)
                {
                    int col = i % cols;
                    int row = i / cols;

                    double baseX = col * tileW;
                    double baseY = row * tileH;

                    // Draw base tile
                    sheetDC.DrawImage(tileBase, new Rect(baseX, baseY, tileW, tileH));

                    // Overlay slices per recipe
                    foreach (int sliceIndex in TileSetRecipeB[i])
                    {
                        if (sliceIndex < 1 || sliceIndex > 4) continue;
                        int idx = sliceIndex - 1;
                        BitmapSource slice = slices[idx];
                        if (slice == null) continue;

                        Int32Rect r = regions[idx];
                        var destRect = new Rect(baseX + r.X, baseY + r.Y, r.Width, r.Height);
                        sheetDC.DrawImage(slice, destRect);
                    }
                }
            }

            // Render and save
            RenderTargetBitmap rtb = new RenderTargetBitmap(sheetW, sheetH, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(sheetVisual);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));

            using (var fs = new System.IO.FileStream(outPath, System.IO.FileMode.Create))
            {
                encoder.Save(fs);
            }

            MessageBox.Show($"4×4 tileset saved: {outPath}");
        }




        private void Add_Border_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "PNG files (*.png)|*.png",
                Title = "Select Border Image"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;

                // Load the image into an Image control or process it
                BitmapImage borderImage = new BitmapImage(new Uri(selectedFilePath));
                Border_Map.Source = borderImage;
                Edit_Screen.Source = borderImage;
            }
        }


        private void RefreshLinesFromInput()
        {
            if (Border_Map.Source is BitmapSource border &&
                int.TryParse(Border_Size.Text, out int borderSize))
            {
                int textureW = border.PixelWidth;
                int textureH = border.PixelHeight;

                if (borderSize > textureW / 2 || borderSize > textureH / 2)
                {
                    MessageBox.Show("Border size is too large for the texture.");
                    return;
                }

                int left = borderSize;
                int top = borderSize;
                int right = textureW - borderSize;
                int bottom = textureH - borderSize;

                UpdateGridOverlay(top, left, right, bottom);
            }
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshLinesFromInput();
        }



        private void UpdateGridOverlay(int top, int left, int right, int bottom)
        {
            GridOverlay.Children.Clear();

            // Ensure we have a bitmap source
            if (Edit_Screen.Source is not BitmapSource bmp)
                return;

            // Native pixel dimensions of the bitmap
            int pixelWidth = bmp.PixelWidth;
            int pixelHeight = bmp.PixelHeight;

            // Displayed size of the image in the UI
            double displayWidth = Edit_Screen.ActualWidth;
            double displayHeight = Edit_Screen.ActualHeight;

            // Scale factors (how much each pixel is blown up)
            double scaleX = displayWidth / pixelWidth;
            double scaleY = displayHeight / pixelHeight;

            // Match overlay canvas to displayed image size
            GridOverlay.Width = displayWidth;
            GridOverlay.Height = displayHeight;

            // Convert pixel positions to UI coordinates
            double uiLeft = left * scaleX;
            double uiRight = right * scaleX;
            double uiTop = top * scaleY;
            double uiBottom = bottom * scaleY;

            double offset = 0.5; // optional: draw between pixels

            // Vertical lines
            GridOverlay.Children.Add(CreateLine(uiLeft + offset, 0, uiLeft + offset, displayHeight));
            GridOverlay.Children.Add(CreateLine(uiRight + offset, 0, uiRight + offset, displayHeight));

            // Horizontal lines
            GridOverlay.Children.Add(CreateLine(0, uiTop + offset, displayWidth, uiTop + offset));
            GridOverlay.Children.Add(CreateLine(0, uiBottom + offset, displayWidth, uiBottom + offset));
        }




        private Line CreateLine(double x1, double y1, double x2, double y2)
        {
            return new Line
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                Stroke = Brushes.DarkGoldenrod,
                StrokeThickness = 1,
                SnapsToDevicePixels = true
            };
        }




        private void Add_Tile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "PNG files (*.png)|*.png",
                Title = "Select Border Image"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;

                // Load the image into an Image control or process it
                BitmapImage borderImage = new BitmapImage(new Uri(selectedFilePath));
                Tile_Map.Source = borderImage;
            }
        }



        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            Tile_Map.Source = null;
            Border_Map.Source = null;
            Edit_Screen.Source = null;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Your logic here
            RefreshLinesFromInput(); // for example
        }

        private void BuildTileset_Click(object sender, RoutedEventArgs e)
        {
            if (Border_Map.Source is BitmapSource border &&
                Tile_Map.Source is BitmapSource tile &&
                int.TryParse(Border_Size.Text, out int borderSize)) // one input field now
            {
                int textureW = border.PixelWidth;
                int textureH = border.PixelHeight;

                // Validate border size
                if (borderSize > textureW / 2 || borderSize > textureH / 2)
                {
                    MessageBox.Show("Border size is too large for the texture.");
                    return;
                }

                int left = borderSize;
                int top = borderSize;
                int right = textureW - borderSize;
                int bottom = textureH - borderSize;

                bool isHalf = (borderSize * 2 == textureW) && (borderSize * 2 == textureH);

                if (isHalf)
                {
                    BuildTilesetSpriteSheetB(border, tile, top, left, right, bottom);
                }
                else
                {
                    BuildTilesetSpriteSheet(border, tile, top, left, right, bottom);
                }
            }
            else
            {
                MessageBox.Show("Missing images or invalid border size.");
            }
        }
    }
}