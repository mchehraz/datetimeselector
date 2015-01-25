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
   using System.Runtime.InteropServices;

   static class NativeMethods {
      public const int MK_LBUTTON = 0x0001;
      public const int WM_MOUSEFIRST = 0x0200;
      public const int WM_MOUSEMOVE = 0x0200;
      public const int WM_LBUTTONDOWN = 0x0201;
      public const int WM_LBUTTONUP = 0x0202;
      public const int WM_RBUTTONUP = 0x0205;
      public const int WM_MBUTTONUP = 0x0208;
      public const int WM_MOUSEWHEEL = 0x020A;
      public const int WM_MOUSELAST = 0x020D;
      public const int WM_NCLBUTTONDOWN = 0x00A1;
      public const int WM_NCMBUTTONDBLCLK = 0x00A9;
      public const int WM_NCMOUSEMOVE = 0x00A0;
      public const int WM_XBUTTONUP = 0x020C;
      [DllImport("user32.dll")]
      public static extern bool IsChild(IntPtr hWndParent, IntPtr hWnd);
   }
}
