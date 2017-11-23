using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;

namespace JrzAsp.Mvc.Captcha.Models {
    public class CaptchaDetail {
        //property
        public const int DEFAULT_WIDTH = 600;

        public const int DEFAULT_HEIGHT = 200;

        protected static readonly Random RANDOMIZER = new Random();

        protected static readonly FontFamily[] FONT_FAMILIES = {
            FontFamily.Families.First(f => f.Name == "Times New Roman"),
            FontFamily.Families.First(f => f.Name == "Consolas")
        };

        protected static readonly StringFormat FONT_FORMAT = new StringFormat {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };

        protected static readonly Color[] GRAYSCALE_COLORS = {
            Color.White,
            Color.FromArgb(255, 50, 50, 50)
        };

        protected static readonly Color[] COLORFUL_COLORS = {
            Color.White,
            Color.FromArgb(255, 50, 50, 50),
            Color.Red,
            Color.Green,
            Color.Blue,
            Color.Magenta,
            Color.Yellow,
            Color.Cyan
        };

        protected static readonly Color BG_COLOR = Color.Black;

        private Image _generatedImage;

        private int _height;
        private bool _mustRegenImage = true;
        private Color[] _randColors;

        private string _text;

        private bool _useGrayscale;

        private int _width;

        //Methods declaration
        public CaptchaDetail(string s) {
            CreatedUtc = DateTime.UtcNow;
            Text = s;
            Width = DEFAULT_WIDTH;
            Height = DEFAULT_HEIGHT;
            UseGrayscale = false;
        }

        public string Text {
            get => _text;
            set {
                _text = value;
                _mustRegenImage = true;
            }
        }

        public int Width {
            get => _width;
            set {
                _width = value > 0 ? value : 1;
                _mustRegenImage = true;
            }
        }

        public int Height {
            get => _height;
            set {
                _height = value > 0 ? value : 1;
                _mustRegenImage = true;
            }
        }

        public bool UseGrayscale {
            get => _useGrayscale;
            set {
                _useGrayscale = value;
                _randColors = value ? GRAYSCALE_COLORS : COLORFUL_COLORS;
                _mustRegenImage = true;
            }
        }

        public DateTime CreatedUtc { get; set; }

        public Image ImageData {
            get {
                if (_generatedImage == null || _mustRegenImage) return ReGenerateImage();
                return _generatedImage;
            }
        }

        public virtual Image ReGenerateImage() {
            if (_generatedImage != null) {
                _generatedImage.Dispose();
                _generatedImage = null;
            }
            var selectedFontFamily = FONT_FAMILIES[RANDOMIZER.Next() % FONT_FAMILIES.Length];

            var bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            var g = Graphics.FromImage(bitmap);
            g.TextRenderingHint = TextRenderingHint.AntiAlias;
            g.PageUnit = GraphicsUnit.Pixel;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new Rectangle(0, 0, Width, Height);

            g.FillRectangle(new SolidBrush(BG_COLOR), rect);

            var fontSize = Height + 1;
            Font font;
            SizeF fontSizeF;
            do {
                fontSize--;
                font = new Font(selectedFontFamily, fontSize, FontStyle.Bold);
                fontSizeF = g.MeasureString(Text, font, new PointF(0, 0), StringFormat.GenericTypographic);
            } while (fontSizeF.Width > Width || fontSizeF.Height > Height);

            var fontPath = new GraphicsPath();
            fontPath.AddString(Text, font.FontFamily, (int) font.Style, font.Size, rect, FONT_FORMAT);
            var v = 4F;
            PointF[] points = {
                new PointF(RANDOMIZER.Next(rect.Width) / v,
                    RANDOMIZER.Next(rect.Height) / v),
                new PointF(rect.Width - RANDOMIZER.Next(rect.Width) / v,
                    RANDOMIZER.Next(rect.Height) / v),
                new PointF(RANDOMIZER.Next(rect.Width) / v,
                    rect.Height - RANDOMIZER.Next(rect.Height) / v),
                new PointF(rect.Width - RANDOMIZER.Next(rect.Width) / v,
                    rect.Height - RANDOMIZER.Next(rect.Height) / v)
            };
            var matrix = new Matrix();
            matrix.Translate(0F, 0F);
            fontPath.Warp(points, rect, matrix, WarpMode.Perspective, 0F);
            var fc1Idx = RANDOMIZER.Next(_randColors.Length);
            var fc2Idx = fc1Idx;
            while (fc2Idx == fc1Idx) fc2Idx = RANDOMIZER.Next(_randColors.Length);
            g.FillPath(new LinearGradientBrush(rect,
                Color.FromArgb(RANDOMIZER.Next(128, 256), _randColors[fc1Idx]),
                Color.FromArgb(RANDOMIZER.Next(128, 256), _randColors[fc2Idx]),
                LinearGradientMode.Horizontal), fontPath);

            for (var i = 0; i < 50; i++) {
                var c1Idx = RANDOMIZER.Next(_randColors.Length);
                var c2Idx = c1Idx;
                while (c2Idx == c1Idx) c2Idx = RANDOMIZER.Next(_randColors.Length);
                var lineBrush = new LinearGradientBrush(rect,
                    Color.FromArgb(RANDOMIZER.Next(256), _randColors[c1Idx]),
                    Color.FromArgb(RANDOMIZER.Next(256), _randColors[c2Idx]),
                    RANDOMIZER.Next(1, 2) % 2 == 0
                        ? LinearGradientMode.BackwardDiagonal
                        : LinearGradientMode.ForwardDiagonal);
                var pen = new Pen(lineBrush, 2);
                var linePoints = new List<Point>();
                var maxLinePoints = RANDOMIZER.Next(2, 5);
                for (var j = 0; j < maxLinePoints; j++) {
                    linePoints.Add(new Point(RANDOMIZER.Next(rect.Width + 1), RANDOMIZER.Next(rect.Height + 1)));
                }
                g.DrawLines(pen, linePoints.ToArray());
            }

            font.Dispose();
            g.Dispose();
            _generatedImage = bitmap;
            _mustRegenImage = false;
            return _generatedImage;
        }
    }
}