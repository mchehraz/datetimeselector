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
   using System.Collections;
   using System.Collections.Generic;
   using System.ComponentModel;
   using System.Diagnostics;
   using System.Drawing;
   using System.Runtime.InteropServices;
   using System.Text;
   using System.Windows.Forms;
   using System.Windows.Forms.VisualStyles;
   [ToolboxItem(false)]
   public class DropDownControl : Control, IDropDownControl, IMessageFilter {
      #region Fields
      protected static readonly int ArrowSeparatorWidth = 1;
      private static readonly int DefaultWidth = 100;
      private bool droppedDown = false;
      private DropDownControlStyle style = DropDownControlStyle.Editable;
      private ComboBoxState arrowState = ComboBoxState.Normal;
      private Rectangle arrowBounds;
      private Rectangle childBounds;
      private Rectangle frameBounds;
      #endregion

      #region Constructors
      protected DropDownControl() {
         if (this.Child != null) {
            this.Controls.Add((Control)this.Child);
            this.AttachChildEvents();
         }
         this.AutoSize = true;
         this.BackColor = SystemColors.Window;
         this.ForeColor = SystemColors.WindowText;
         this.DoubleBuffered = true;
         this.MeasureBounds();
         if (this.Child != null) {
            this.SetChildBounds();
         }
         this.Width = DefaultWidth;
      }
      #endregion

      #region Properties
      [Browsable(false)]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      [EditorBrowsable(EditorBrowsableState.Advanced)]
      public Rectangle ArrowBounds {
         get {
            return this.arrowBounds;
         }
         private set {
            this.arrowBounds = value;
         }
      }
      protected virtual ComboBoxState ArrowState {
         get {
            return this.arrowState;
         }
         private set {
            if (this.arrowState != value) {
               this.arrowState = value;
               this.Invalidate(this.ArrowBounds);
            }
         }
      }
      protected virtual int ArrowWidth {
         get {
            return SystemInformation.HorizontalScrollBarArrowWidth;
         }
      }
      [DefaultValue(typeof(Color), "Window")]
      public override Color BackColor {
         get {
            return base.BackColor;
         }
         set {
            if (this.BackColor != value) {
               base.BackColor = value;
               if (this.Child != null) {
                  this.Child.BackColor = value;
               }
            }
         }
      }
      protected virtual Size BorderSize {
         get {
            return SystemInformation.Border3DSize;
         }
      }
      protected virtual IDropDownChild Child {
         get {
            return null;
         }
      }
      protected Rectangle ChildBounds {
         get {
            return this.childBounds;
         }
         private set {
            this.childBounds = value;
         }
      }
      private bool DroppedDownInternal {
         set {
            this.droppedDown = value;
         }
      }
      [DefaultValue(typeof(Color), "WindowText")]
      public override Color ForeColor {
         get {
            return base.ForeColor;
         }
         set {
            if (this.ForeColor != value) {
               base.ForeColor = value;
            }
         }
      }
      protected virtual Rectangle FrameBounds {
         get {
            return this.frameBounds;
         }
         private set {
            this.frameBounds = value;
         }
      }
      protected virtual int PreferredHeight {
         get {
            return SizeUtil.GetCtrlHeight(this.Font) + 1;
         }
      }
      protected virtual IDropDownPopup Popup {
         get {
            return null;
         }
      }
      #endregion

      #region Methods
      protected virtual void AttachControlEvents() {
         this.GotFocus += new EventHandler(control_GotFocus);
         this.LostFocus += new EventHandler(control_LostFocus);
      }
      protected virtual void AttachChildEvents() {
         this.Child.LostFocus += new EventHandler(control_LostFocus);
      }
      protected virtual void CloseDropDown() {
         if (this.DroppedDown) {
            this.DroppedDownInternal = false;
            this.Popup.Close();
            this.Popup.LostFocus -= popUp_LostFocus;
            this.OnDropDownClosed(EventArgs.Empty);

            if (this.DropDownClosed != null)
               this.DropDownClosed(this, EventArgs.Empty);
         }
      }
      private void control_GotFocus(object sender, EventArgs e) {
         this.Invalidate();
      }
      private static ButtonState ComboBoxStateToComboButtonState(ComboBoxState cbs) {
         if (cbs == ComboBoxState.Pressed) {
            return ButtonState.Flat;
         }
         if (cbs == ComboBoxState.Disabled) {
            return ButtonState.Inactive;
         }
         return ButtonState.Normal;
      }
      private void control_LostFocus(object sender, EventArgs e) {
         if (!this.Popup.ContainsFocus && this.DroppedDown) {
            this.CloseDropDown();
         }
         if (this.Style == DropDownControlStyle.List) {
            this.Invalidate();
         }
      }
      protected virtual void DetachControlEvents() {
         this.GotFocus -= control_GotFocus;
         this.LostFocus -= control_LostFocus;
      }
      protected virtual void DetachChildEvents() {
         this.Child.LostFocus -= control_LostFocus;
      }
      protected virtual void DrawArrow(PaintEventArgs pe) {
         ComboBoxState arrowState = this.Enabled ? this.ArrowState : ComboBoxState.Disabled;
         if (ComboBoxRenderer.IsSupported && Application.RenderWithVisualStyles) {
            ComboBoxRenderer.DrawDropDownButton(pe.Graphics, this.ArrowBounds, arrowState);
         }
         else {
            ControlPaint.DrawComboButton(pe.Graphics, this.ArrowBounds,
                                         ComboBoxStateToComboButtonState(arrowState));
         }
      }
      protected virtual void DrawFrame(PaintEventArgs pe) {
         if (ComboBoxRenderer.IsSupported && Application.RenderWithVisualStyles) {
            ComboBoxRenderer.DrawTextBox(pe.Graphics, frameBounds,
                                          this.Enabled ? ComboBoxState.Normal : ComboBoxState.Disabled);
         }
         else {
            ControlPaint.DrawButton(pe.Graphics, frameBounds, ButtonState.Pushed);
         }
         if (this.Enabled) {
            using (SolidBrush brush = new SolidBrush(this.BackColor)) {
               pe.Graphics.FillRectangle(brush, this.childBounds);
            }
         }
      }
      protected virtual void DrawText(PaintEventArgs pe) {
         TextFormatFlags format = TextFormatFlags.Default;
         Rectangle chldBounds = this.ChildBounds;
         format |= TextFormatFlags.EndEllipsis;
         format |= TextFormatFlags.SingleLine;
         format |= TextFormatFlags.VerticalCenter;
         if (this.RightToLeft == RightToLeft.Yes) {
            format |= TextFormatFlags.RightToLeft;
            format |= TextFormatFlags.Right;
         }
         if (this.Focused && !this.DroppedDown && this.Style == DropDownControlStyle.List) {
            pe.Graphics.FillRectangle(SystemBrushes.Highlight, chldBounds);
            ControlPaint.DrawFocusRectangle(pe.Graphics, chldBounds);
            TextRenderer.DrawText(pe.Graphics, this.Text, this.Font, chldBounds,
                                  SystemColors.HighlightText, Color.Transparent, format);
         }
         else {
            Color color;
            if (this.Enabled) {
               using (SolidBrush brush = new SolidBrush(this.BackColor)) {
                  pe.Graphics.FillRectangle(brush, chldBounds);
               }
               color = this.ForeColor;
            }
            else {
               color = SystemColors.GrayText;
            }
            TextRenderer.DrawText(pe.Graphics, this.Text, this.Font, chldBounds, color, Color.Transparent, format);
         }
      }
      protected virtual void DropDownInternal() {
         if (this.Popup != null) {
            bool raiseDropDownEvent = !this.DroppedDown;
            this.DroppedDownInternal = true;
            this.OnDropDown(EventArgs.Empty);
            this.Popup.LostFocus += new EventHandler(popUp_LostFocus);
            this.Popup.Show(this);
            if (raiseDropDownEvent && this.DropDown != null) {
               this.DropDown(this, EventArgs.Empty);
            }
         }
      }
      public override Size GetPreferredSize(Size proposedSize) {
         Size basePreferredSize = base.GetPreferredSize(proposedSize);
         return new Size(basePreferredSize.Width, this.PreferredHeight);
      }
      protected virtual void MeasureBounds() {
         int controlHeight = this.PreferredHeight;
         Size borderSize = this.BorderSize;
         Size arrowSize = new Size(this.ArrowWidth, this.Height - 2 * borderSize.Height);
         Rectangle arrBounds = new Rectangle(Point.Empty, arrowSize);
         arrBounds = new Rectangle(Point.Empty, arrowSize);
         Rectangle chldBounds;
         if (this.RightToLeft != RightToLeft.Yes) {
            arrBounds.Offset(this.Width - arrowSize.Width - borderSize.Width, borderSize.Height);
            chldBounds = new Rectangle(0, 0, this.Width - this.ArrowBounds.Width - ArrowSeparatorWidth, this.Height);
            chldBounds.Inflate(-borderSize.Width, -borderSize.Height);
         }
         else {
            arrBounds.Offset(borderSize.Width, borderSize.Height);
            chldBounds = new Rectangle(this.ArrowBounds.Width + ArrowSeparatorWidth, 0,
                                                this.Width - this.ArrowBounds.Width, this.Height);
            chldBounds.Inflate(-borderSize.Width, -borderSize.Height);
         }
         Rectangle frmBounds = new Rectangle(0, 0, this.Width, this.Height);

         this.ArrowBounds = arrBounds;
         this.ChildBounds = chldBounds;
         this.FrameBounds = frmBounds;
      }
      protected virtual void OnArrowClicked(EventArgs e) {
         if (this.Child != null && !this.ContainsFocus && !((Control)this.Popup).ContainsFocus) {
            if (this.Style == DropDownControlStyle.Editable) {
               this.Child.Focus();
            }
         }
         this.DroppedDown = !this.DroppedDown;
      }
      protected override void OnClick(EventArgs e) {
         base.OnClick(e);
         if (this.Style == DropDownControlStyle.List ||
             this.ArrowBounds.Contains(this.PointToClient(Cursor.Position))) {
            this.OnArrowClicked(EventArgs.Empty);
         }
      }
      private void OnDropDown(EventArgs e) {
         Application.AddMessageFilter(this);
         if (this.Style == DropDownControlStyle.List) {
            this.Invalidate(this.childBounds);
         }
      }
      private void OnDropDownClosed(EventArgs e) {
         Application.RemoveMessageFilter(this);
         if (this.Style == DropDownControlStyle.List) {
            this.Invalidate(this.childBounds);
         }
      }
      protected override void OnEnabledChanged(EventArgs e) {
         base.OnEnabledChanged(e);
         if (this.Enabled) {
            this.ArrowState = ComboBoxState.Normal;
         }
         else {
         }
         this.Invalidate();
      }
      protected override void OnEnter(EventArgs e) {
         base.OnEnter(e);
         if (this.Style == DropDownControlStyle.Editable && this.Child != null) {
            this.Child.Focus();
         }
      }
      protected override void OnGotFocus(EventArgs e) {
         base.OnGotFocus(e);
         if (this.Style == DropDownControlStyle.Editable && this.Parent is ContainerControl) {
            ContainerControl container = (ContainerControl)this.Parent;
            container.SelectNextControl(this, false, true, true, true);
         }
      }
      protected override void OnFontChanged(EventArgs e) {
         base.OnFontChanged(e);
         this.MeasureBounds();
         int controlHeight = this.PreferredHeight;
         this.Height = controlHeight;
         if (this.Child != null) {
            this.SetChildBounds();
         }
      }
      protected override void OnMouseDown(MouseEventArgs e) {
         base.OnMouseDown(e);
         if (this.ClientRectangle.Contains(e.Location) && this.Style == DropDownControlStyle.List
             && !this.Focused) {
            this.Focus();
         }
         if (this.ArrowBounds.Contains(e.Location)) {
            this.ArrowState = ComboBoxState.Pressed;
         }
      }
      protected override void OnMouseLeave(EventArgs e) {
         base.OnMouseLeave(e);
         if (!this.DroppedDown) {
            this.ArrowState = ComboBoxState.Normal;
         }
      }
      protected override void OnMouseMove(MouseEventArgs e) {
         base.OnMouseMove(e);
         if (this.ArrowBounds.Contains(e.Location)) {
            if (this.ArrowState != ComboBoxState.Pressed) {
               this.ArrowState = ComboBoxState.Hot;
            }
         }
      }
      protected override void OnMouseUp(MouseEventArgs e) {
         base.OnMouseUp(e);
         if (this.ArrowBounds.Contains(e.Location) || this.ArrowState == ComboBoxState.Pressed) {
            this.ArrowState = ComboBoxState.Normal;
         }
      }
      protected override void OnPaint(PaintEventArgs pe) {
         if (pe.ClipRectangle == this.ArrowBounds) {
            this.DrawArrow(pe);
            return;
         }
         if (pe.ClipRectangle.Equals(childBounds)) {
            this.DrawText(pe);
            return;
         }
         this.DrawFrame(pe);
         this.DrawArrow(pe);
         this.DrawText(pe);
      }
      protected override void OnRightToLeftChanged(EventArgs e) {
         base.OnRightToLeftChanged(e);
         if (this.Child != null) {
            this.Child.RightToLeft = this.RightToLeft;
         }
         this.MeasureBounds();
         if (this.Child != null) {
            this.SetChildBounds();
         }
         this.Invalidate();
      }
      protected override void OnSizeChanged(EventArgs e) {
         base.OnSizeChanged(e);
         this.MeasureBounds();
         if (this.Child != null) {
            this.SetChildBounds();
         }
         this.Invalidate();
      }
      protected new virtual void OnStyleChanged(EventArgs e) {
         this.SetChildBounds();
         IDropDownChild child = this.Child;
         if (this.style == DropDownControlStyle.List) {
            if (child != null) {
               this.DetachChildEvents();
               if (child.Focused) {
                  this.Focus();
               }
               this.Controls.Remove((Control)child);
            }
            this.AttachControlEvents();
         }
         else {
            if (child != null) {
               this.DetachControlEvents();
               this.Controls.Add((Control)child);
               if (this.Focused) {
                  child.Focus();
               }
               this.AttachChildEvents();
            }
         }
      }
      protected override void OnTextChanged(EventArgs e) {
         base.OnTextChanged(e);
         this.Invalidate(this.ChildBounds);
      }
      protected override void OnTabStopChanged(EventArgs e) {
         base.OnTabStopChanged(e);
         if (this.Child != null) {
            this.Child.TabStop = this.TabStop;
         }
      }
      private void popUp_LostFocus(object sender, EventArgs e) {
         if (!this.ContainsFocus && this.DroppedDown) {
            this.CloseDropDown();
         }
         if (this.Style == DropDownControlStyle.List) {
            this.Invalidate();
         }
      }
      protected override bool ProcessDialogKey(Keys keyData) {
         switch (keyData) {
            case (Keys.Alt | Keys.Down):
               this.DroppedDown = !this.DroppedDown;
               return true;

            case Keys.Escape:
               if (this.DroppedDown) {
                  this.CloseDropDown();
                  return true;
               }
               break;
         }
         return base.ProcessDialogKey(keyData);
      }
      protected virtual void SetChildBounds() {
         IDropDownChild child = this.Child;
         if (child != null) {
            if (!child.Bounds.Equals(childBounds)) {
               child.SetBounds(childBounds.X, childBounds.Y, childBounds.Width, childBounds.Height);
            }
         }
      }
      #endregion

      #region IMessageFilter Members

      public bool PreFilterMessage(ref Message m) {
         if (!this.DroppedDown) {
            Debug.Fail("Problem!! + " + this.Visible.ToString());
            Application.RemoveMessageFilter(this);
            return false;
         }
         if (!IsMouseMessage(m)) {
            return false;
         }
         bool popUpContainsPoint = this.Popup.ContainsPoint(Cursor.Position);
         bool popUpContainsWindow = this.Popup.ContainsWindow(m.HWnd);
         bool thisContainsPoint = this.ContainsPoint(Cursor.Position);

         switch (m.Msg) {
            case NativeMethods.WM_LBUTTONDOWN:
               if (!popUpContainsPoint) {
                  this.CloseDropDown();
                  return true;
               }
               break;
            case NativeMethods.WM_MOUSEMOVE:
            case NativeMethods.WM_NCMOUSEMOVE:
               if (!thisContainsPoint) {
                  return true;
               }
               break;
            case NativeMethods.WM_MOUSEWHEEL:
               if (!this.ContainsWindow(m.HWnd)) {
                  return true;
               }
               break;
            default:
               if (!popUpContainsPoint && !popUpContainsWindow) {
                  this.CloseDropDown();
                  return false;
               }
               break;
         }
         return false;
      }
      private static bool IsMouseMessage(Message m) {
         bool filterMessage = false;

         if (m.Msg >= NativeMethods.WM_MOUSEFIRST && m.Msg <= NativeMethods.WM_MOUSELAST) {
            filterMessage = true;
         }
         else if (m.Msg >= NativeMethods.WM_NCLBUTTONDOWN && m.Msg <= NativeMethods.WM_NCMBUTTONDBLCLK) {
            filterMessage = true;
         }
         return filterMessage;
      }
      #endregion

      #region IDropDownControl Members
      public event EventHandler DropDown;
      public event EventHandler DropDownClosed;
      [Browsable(false)]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      [EditorBrowsable(EditorBrowsableState.Always)]
      public virtual bool DroppedDown {
         get {
            return this.droppedDown;
         }
         set {
            if (value && !this.DroppedDown) {
               this.DropDownInternal();
            }
            else if (!value) {
               this.CloseDropDown();
            }
         }
      }
      [Browsable(true)]
      [Category("Appearance")]
      [DefaultValue(DropDownControlStyle.Editable)]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
      [EditorBrowsable(EditorBrowsableState.Always)]
      public DropDownControlStyle Style {
         get {
            return this.style;
         }
         set {
            if (this.style != value) {
               this.style = value;
               this.OnStyleChanged(EventArgs.Empty);
            }
         }
      }
      public bool ContainsPoint(Point point) {
         return this.Visible && (this.RectangleToScreen(this.Bounds).Contains(point) ||
                this.Popup.ContainsPoint(point));
      }
      public bool ContainsWindow(IntPtr handle) {
         return handle == this.Handle || NativeMethods.IsChild(this.Handle, handle) ||
                          this.Popup.ContainsWindow(handle);
      }
      public virtual Point GetDropDownLocation() {
         int y = this.PointToScreen(new Point(0, this.Height)).Y;
         if (y + this.Popup.Height > Screen.PrimaryScreen.WorkingArea.Height) {
            y = this.PointToScreen(new Point(0, 0)).Y - this.Popup.Height;
         }
         if (this.RightToLeft != RightToLeft.Yes) {
            return new Point(this.PointToScreen(new Point(0, 0)).X, y);
         }
         else {
            return new Point(this.PointToScreen(new Point(-(this.Popup.Width - this.Width), 0)).X, y);
         }
      }
      #endregion

   }
}