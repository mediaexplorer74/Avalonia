using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering;
using SkiaSharp;

namespace Avalonia.Skia
{
    class BitmapImpl : IRenderTargetBitmapImpl, IWritableBitmapImpl
    {
        public SKBitmap Bitmap { get; private set; }

        public BitmapImpl(SKBitmap bm)
        {
            Bitmap = bm;
            PixelHeight = bm.Height;
            PixelWidth = bm.Width;
        }

        public BitmapImpl(int width, int height, PixelFormat? fmt = null)
        {
            PixelHeight = height;
            PixelWidth = width;
            var colorType = fmt?.ToSkColorType() ?? SKImageInfo.PlatformColorType;
            var runtime = AvaloniaLocator.Current?.GetService<IRuntimePlatform>()?.GetRuntimeInfo();
            if (runtime?.IsDesktop == true && runtime?.OperatingSystem == OperatingSystemType.Linux)
                colorType = SKColorType.Bgra8888;
            Bitmap = new SKBitmap(width, height, colorType, SKAlphaType.Premul);
            Bitmap.Erase(SKColor.Empty);
        }

        public void Dispose()
        {
            Bitmap.Dispose();
        }

        public void Save(string fileName)
        {
            
#if DESKTOP
            if(Bitmap.ColorType != SKColorType.Bgra8888)
            {
                using (var tmp = new BitmapImpl(Bitmap.Copy(SKColorType.Bgra8888)))
                    tmp.Save(fileName);
                return;
            }

            IntPtr length;
            using (var sdb = new System.Drawing.Bitmap(PixelWidth, PixelHeight, Bitmap.RowBytes,
                
                System.Drawing.Imaging.PixelFormat.Format32bppArgb,
                
                Bitmap.GetPixels(out length)))
                sdb.Save(fileName);
#else
            //SkiaSharp doesn't expose image encoders yet
#endif
        }

        public int PixelWidth { get; private set; }
        public int PixelHeight { get; private set; }

        class BitmapDrawingContext : DrawingContextImpl
        {
            private readonly SKSurface _surface;

            public BitmapDrawingContext(SKBitmap bitmap, IVisualBrushRenderer visualBrushRenderer)
                : this(CreateSurface(bitmap), visualBrushRenderer)
            {

            }

            private static SKSurface CreateSurface(SKBitmap bitmap)
            {
                IntPtr length;
                var rv =  SKSurface.Create(bitmap.Info, bitmap.GetPixels(out length), bitmap.RowBytes);
                if (rv == null)
                    throw new Exception("Unable to create Skia surface");
                return rv;
            }

            public BitmapDrawingContext(SKSurface surface, IVisualBrushRenderer visualBrushRenderer)
                : base(surface.Canvas, visualBrushRenderer)
            {
                _surface = surface;
            }

            public override void Dispose()
            {
                base.Dispose();
                _surface.Dispose();
            }
        }

        public IDrawingContextImpl CreateDrawingContext(IVisualBrushRenderer visualBrushRenderer)
        {
            return new BitmapDrawingContext(Bitmap, visualBrushRenderer);
        }

        public void Save(Stream stream)
        {
            IntPtr length;
            using (var image = SKImage.FromPixels(Bitmap.Info, Bitmap.GetPixels(out length), Bitmap.RowBytes))
            using (var data = image.Encode())
            {
                data.SaveTo(stream);
            }
        }

        class BitmapFramebuffer : ILockedFramebuffer
        {
            private SKBitmap _bmp;

            public BitmapFramebuffer(SKBitmap bmp)
            {
                _bmp = bmp;
                _bmp.LockPixels();
            }

            public void Dispose()
            {
                _bmp.UnlockPixels();
                _bmp = null;
            }

            public IntPtr Address => _bmp.GetPixels();
            public int Width => _bmp.Width;
            public int Height => _bmp.Height;
            public int RowBytes => _bmp.RowBytes;
            public Size Dpi { get; } = new Size(96, 96);
            public PixelFormat Format => _bmp.ColorType.ToPixelFormat();
        }

        public ILockedFramebuffer Lock() => new BitmapFramebuffer(Bitmap);
    }
}
