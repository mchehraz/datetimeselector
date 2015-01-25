/*
 * Copyright (c) 2015 Mehrzad Chehraz (mehrzady@gmail.com)
 * Released under the MIT License
 * http://chehraz.ir/mit_license
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:

 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.

 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
namespace Atf.UI.Utility {
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;
    static class SizeUtil {
        #region Fields
        private static Dictionary<DLUParams, SizeF> cache = new Dictionary<DLUParams, SizeF>();
        private static string defaultDialogChars;
        private static readonly int defaultDialogCharsEnd = 0x7E;
        private static readonly int defaultDialogCharsStart = 0x21;
        #endregion

        #region Properties
        public static string DefaultDialogChars {
            get {
                if (defaultDialogChars == null) {
                    StringBuilder sb = new StringBuilder(defaultDialogCharsEnd - defaultDialogCharsStart + 1);
                    for (int i = defaultDialogCharsStart; i <= defaultDialogCharsEnd; i++) {
                        sb.Append((char)i);
                    }
                    defaultDialogChars = sb.ToString();
                }
                return defaultDialogChars;
            }
        }
        #endregion

        #region Methods
        public static void ClearCache() {
            cache = new Dictionary<DLUParams, SizeF>();
        }
        // Height Conversion
        public static int DLUVerToPixels(int height) {
            return DLUVerToPixels(height, SystemFonts.DefaultFont, DefaultDialogChars);
        }
        public static int DLUVerToPixels(int height, Font font) {
            return DLUVerToPixels(height, font, DefaultDialogChars);
        }
        public static int DLUVerToPixels(int height, Font font, string characters) {
            SizeF dluSizeF = GetDLUSizeF(font, characters);
            return (int)Math.Round(height * dluSizeF.Height);
        }
        // Padding Conversion
        public static Padding DLUToPixels(Padding padding) {
            return DLUToPixels(padding, SystemFonts.DefaultFont, DefaultDialogChars);
        }
        public static Padding DLUToPixels(Padding padding, Font font) {
            return DLUToPixels(padding, font, DefaultDialogChars);
        }
        public static Padding DLUToPixels(Padding padding, Font font, string characters) {
            SizeF dluSizeF = GetDLUSizeF(font, characters);
            return new Padding((int)Math.Round(padding.Left * dluSizeF.Width),
                            (int)Math.Round(padding.Top * dluSizeF.Height),
                            (int)Math.Round(padding.Right * dluSizeF.Width),
                            (int)Math.Round(padding.Bottom * dluSizeF.Height));
        }
        // Size conversion
        public static Size DLUToPixels(Size size) {
            return DLUToPixels(size, SystemFonts.DefaultFont, DefaultDialogChars);
        }
        public static Size DLUToPixels(Size size, Font font) {
            return DLUToPixels(size, font, DefaultDialogChars);
        }
        public static Size DLUToPixels(Size size, Font font, string characters) {
            SizeF dluSizeF = GetDLUSizeF(font, characters);
            return new Size((int)Math.Round(size.Width * dluSizeF.Width),
                            (int)Math.Round(size.Height * dluSizeF.Height));
        }
        // Width Conversion
        public static int DLUHorToPixels(int width) {
            return DLUHorToPixels(width, SystemFonts.DefaultFont, DefaultDialogChars);
        }
        public static int DLUHorToPixels(int width, Font font) {
            return DLUHorToPixels(width, font, DefaultDialogChars);
        }
        public static int DLUHorToPixels(int width, Font font, string characters) {
            SizeF dluSizeF = GetDLUSizeF(font, characters);
            return (int)Math.Round(width * dluSizeF.Width);
        }
        public static int GetCtrlHeight(Font font) {
            if (font == null) {
                throw new ArgumentNullException("font");
            }
            // Formula is based on textbox preferred height (TextBoxBase.PreferredHeight)  
            return font.Height + SystemInformation.BorderSize.Height * 4 + 3;
        }

        public static SizeF GetDLUSizeF() {
            return GetDLUSizeF(SystemFonts.DefaultFont, DefaultDialogChars);
        }
        public static SizeF GetDLUSizeF(Font font) {
            return GetDLUSizeF(font, DefaultDialogChars);
        }
        public static SizeF GetDLUSizeF(Font font, string characters) {
            SizeF averageSize;
            var dluParams = new DLUParams(font, characters);
            if (!cache.TryGetValue(dluParams, out averageSize)) {
                Size total = TextRenderer.MeasureText(characters, font);
                averageSize = new SizeF(total.Width / characters.Length, total.Height);
                cache.Add(dluParams, averageSize);
            }
            return new SizeF(averageSize.Width / 4.0f, averageSize.Width / 8.0f);
        }

        #endregion

        #region DLUParams
        private class DLUParams {
            private string fontFamily;
            private int fontHashCode;
            private string characters;

            public DLUParams(Font font, string characters) {
                if (string.IsNullOrEmpty(characters)) {
                    throw new ArgumentNullException("characters");
                }
                if (font == null) {
                    throw new ArgumentNullException("font");
                }
                this.characters = characters;
                this.fontFamily = font.FontFamily.Name;
                this.fontHashCode = font.GetHashCode();
            }
            public override bool Equals(object obj) {
                if (obj == null) {
                    return false;
                }
                DLUParams objAsDLUParams = obj as DLUParams;
                if (objAsDLUParams == null) {
                    return false;
                }
                return this.characters.Equals(objAsDLUParams.characters, StringComparison.Ordinal) &&
                        this.fontHashCode.Equals(objAsDLUParams.fontHashCode) &&
                        this.fontFamily.Equals(objAsDLUParams.fontFamily);

            }
            public override int GetHashCode() {
                return this.fontFamily.GetHashCode() ^ this.fontHashCode ^ this.characters.GetHashCode();
            }
        }
        #endregion
    }
}
