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
    using System.Diagnostics;

    class DateTimeSelectorPopup : DropDownPopupBase {

        #region fields       
        private CalendarControl calendarControl;
        public event EventHandler Cancel;
        public event EventHandler DateSelected;    
        #endregion

        #region Constructors
        public DateTimeSelectorPopup() {
            this.calendarControl = new CalendarControl();
           
            this.SuspendLayout();
            // calendarControl
            this.calendarControl.DateSelected += new EventHandler(calendarControl_DateSelected);
            // this.calendarControl.TabStop = false;
            // DateTimePickerPopup
            this.BackColor = SystemColors.Window;
            this.ResumeLayout();
        }             
        #endregion

        #region Properties 
        public event EventHandler ValueChanged {
            add {
                this.calendarControl.ValueChanged += value;
            }
            remove {
                this.calendarControl.ValueChanged -= value;
            }
        }
        public CalendarControl CalendarControl {
            get {
                return this.calendarControl;
            }
        }
        protected override Control Control {
            get {
                return this.calendarControl;
            }
        }
        public DateTimeFormatInfo DateTimeFormat {
            get {
                 return this.calendarControl.DateTimeFormat;
            }
            set {
                 this.calendarControl.DateTimeFormat = value;
            }
        }        
        public DateTime MaxDate {
            get {
                return this.calendarControl.MaxDate;
            }
            set {
                this.calendarControl.MaxDate = value;
            }
        }
        public DateTime MinDate {
            get {
                return this.calendarControl.MinDate;
            }
            set {
                this.calendarControl.MinDate = value;
            }
        }        
        public DateTime Value {
            get {
                return this.calendarControl.Value;
            }
            set {
                this.calendarControl.ValueInternal = value;
            }
        }
        #endregion

        #region Methods
        private void calendarControl_DateSelected(object sender, EventArgs e) {
            this.OnDateSelected();
        }
        private void OnCancel() {
            if (this.Cancel != null) {
                this.Cancel(this, EventArgs.Empty);
            }
        }
        private void OnDateSelected() {
            if (this.DateSelected != null) {
                this.DateSelected(this, EventArgs.Empty);
            }
        } 
        protected override void OnOwnerChanged(EventArgs e) {
            if (this.Owner != null) {
                this.Font = this.Owner.Font;
            }
            else {
                this.Font = null;
            }
        }
        protected override bool ProcessDialogKey(Keys keyData) {
            // Keys keyCode = keyData & Keys.KeyCode;
            switch (keyData) {
                case Keys.Enter:
                    this.OnDateSelected();
                    return true;
                case (Keys.Alt|Keys.Down):
                case Keys.Escape:
                    this.OnCancel();
                    return true;

            }
            return base.ProcessDialogKey(keyData);
        }        
        protected override void SetSize() {
            this.Size = this.Control.Size;
            // base.SetSize();
        }
        public override void Show(IDropDownControl owner) {
            base.Show(owner);
            this.Control.Focus();
        }
        #endregion

    }
}
