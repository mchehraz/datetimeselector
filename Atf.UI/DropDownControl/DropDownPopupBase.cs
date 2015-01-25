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
    using Atf.UI.Utility;

    using System;
    using System.Drawing;
    using System.Globalization;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;

    abstract class DropDownPopupBase : ToolStripDropDown, IDropDownPopup {
        #region Fields
        private ToolStripControlHost host;
        private bool initialized;
        private Control owner;
        #endregion

        #region Properties
        protected abstract Control Control {
            get;
        }  
        protected Control Owner {
            get { return this.owner; }
            private set {
                if (this.owner != value) {
                    this.owner = value;
                    this.OnOwnerChanged(EventArgs.Empty);
                }
            }
        }
        #endregion

        #region Methods
        private void Control_MouseUp(object sender, MouseEventArgs e) {
            if (this.Control.Bounds.Contains(e.Location)) { 
                if (this.Visible) {
                    this.BeginInvoke((MethodInvoker)delegate {
                        this.Control.Capture = false;
                        this.Control.Capture = true;
                    });
                }
            }
        }
        private void Initialize() {
            this.host = new ToolStripControlHost(this.Control);
            this.SuspendLayout();
            // control
            this.Control.LostFocus += new EventHandler(Control_LostFocus);
            this.Control.MouseUp += new MouseEventHandler(Control_MouseUp);
            this.Control.TabStop = false;
            // host
            this.host.AutoSize = true;
            this.host.Margin = new Padding(0);
            this.host.Padding = new Padding(1);
            // DateTimePickerPopup
            this.AutoSize = true;
            this.AutoClose = false;
            base.Items.Add(this.host);
            this.Padding = Padding.Empty;
            this.RenderMode = ToolStripRenderMode.System;
            this.TabStop = false;
            this.ResumeLayout();
            this.initialized = true;
        }
        private void Control_LostFocus(object sender, EventArgs e) {
            if (!this.ContainsFocus) {
                if (this.LostFocus != null) {
                    this.LostFocus(this, e);
                }
            }
        }
        protected override void OnLostFocus(EventArgs e) {
            base.OnLostFocus(e);
            if (!this.Control.CanFocus) {
                if (this.LostFocus != null) {
                    this.LostFocus(this, e);
                }
            }
        }
        protected virtual void OnOwnerChanged(EventArgs e) {
            if (this.Owner != null) {
                this.Font = this.Owner.Font;
                this.RightToLeft = this.Owner.RightToLeft;
            }
            else {
                this.Font = null;
                this.RightToLeft = RightToLeft.No;
            }
        }
        protected virtual void SetSize() {
        }
        private void ShowInternal() {
            if (this.owner != null) {
                base.Show(((IDropDownControl)this.owner).GetDropDownLocation());
            }
        }
        #endregion

        #region IDropDownControlPopup Members
        public new event EventHandler LostFocus;

        public new void Close() {      
            /********* CHANGED **********/
            if (this.Control.Capture) {
                this.Control.Capture = false;
            }
            base.Close();
            this.Owner = null;
        }
        public virtual new bool ContainsFocus {
            get {
                return base.ContainsFocus;
            }
        }
        public bool ContainsPoint(Point point) {
            return this.Visible && this.Bounds.Contains(point);
        }
        public bool ContainsWindow(IntPtr handle) {
            return handle == this.Handle || NativeMethods.IsChild(this.Handle, handle);
        }
        public virtual void Show(IDropDownControl owner) {
            if (!this.initialized) {
                this.Initialize();
            }
            if (owner != null) {
                this.Owner = (Control)owner;
                if (!this.Control.Capture) {
                    this.Control.Capture = true;
                }
                this.SetSize();
                this.ShowInternal();
            }
        }
        #endregion
    }
}
