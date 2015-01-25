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
namespace Atf.UI {
   using System;
   using System.Drawing;
   using System.ComponentModel;

   [ToolboxItem(false)]
   public class TextBox : System.Windows.Forms.TextBox {
      private System.Windows.Forms.BorderStyle borderStyle;
      private Region lastSetRegion;

      public TextBox() {
         this.BorderStyle = base.BorderStyle;
      }
      protected override void OnSizeChanged(EventArgs e) {
         int selLengrh = this.SelectionLength;
         int textLength = this.Text.Length;
         if (selLengrh > 0 && selLengrh == textLength) {
            this.SelectionLength = 0;
            this.SelectionLength = textLength;
         }
         base.OnSizeChanged(e);
      }
      protected override void SetBoundsCore(int x, int y, int width, int height,
                                            System.Windows.Forms.BoundsSpecified specified) {
         base.SetBoundsCore(x, y, width, height, specified);
         if (this.borderStyle == System.Windows.Forms.BorderStyle.None &&
             base.BorderStyle == System.Windows.Forms.BorderStyle.Fixed3D)
            this.UpdateRegion(this.Width, this.Height);
      }

      private void UpdateRegion(int width, int height) {
         if (this.BorderStyle == System.Windows.Forms.BorderStyle.None &&
            base.BorderStyle == System.Windows.Forms.BorderStyle.Fixed3D) {
            Region oldRegion = lastSetRegion;
            Size border3DSize = System.Windows.Forms.SystemInformation.Border3DSize;
            this.Region = lastSetRegion = new Region(new Rectangle(border3DSize.Width, border3DSize.Height,
                                                                    width - 2 * border3DSize.Width,
                                                                    height - 2 * border3DSize.Height - 1));
            if (oldRegion != null)
               oldRegion.Dispose();
         }
         else {
            this.Region = null;
            if (lastSetRegion != null) {
               lastSetRegion.Dispose();
               lastSetRegion = null;
            }
         }
      }
      [Browsable(true)]
      [DefaultValue(System.Windows.Forms.BorderStyle.Fixed3D)]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
      public new System.Windows.Forms.BorderStyle BorderStyle {
         get { return this.borderStyle; }
         set {
            if (value != this.borderStyle) {
               this.borderStyle = value;
               if (this.BorderStyle == System.Windows.Forms.BorderStyle.None) {
                  if (base.BorderStyle != System.Windows.Forms.BorderStyle.Fixed3D)
                     base.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
                  else
                     this.OnBorderStyleChanged(EventArgs.Empty);
               }
               else if (base.BorderStyle != value) {
                  base.BorderStyle = value;
               }
               this.UpdateRegion(this.Width, this.Height);
            }
         }
      }
   }
}
