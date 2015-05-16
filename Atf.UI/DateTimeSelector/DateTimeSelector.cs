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
   using Atf.Core.Text;
   using Atf.UI.Utility;

   using System;
   using System.Collections.Generic;
   using System.ComponentModel;
   using System.Diagnostics;
   using System.Drawing;
   using System.Globalization;
   using System.Threading;
   using System.Windows.Forms;
   using System.Windows.Forms.Design;
   using System.Text;
   [Designer(typeof(DateTimeSelectorDesigner))]
   [DesignTimeVisible(true)]
   [ToolboxItem(true)]
   [ToolboxBitmap(typeof(DateTimeSelector), "DateTimeSelector.bmp")]
   public class DateTimeSelector : DropDownControl {
      #region Events
      [Browsable(false)]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      public new event EventHandler TextChanged {
         add {}
         remove {}
      }
      [Category("Property Changed")]
      public event EventHandler ValueChanged;
      #endregion

      #region Fields
      private DateTimeSelectorChild child;
      private Color calendarBackColor = SystemColors.Window;
      private Color calendarForeColor = SystemColors.WindowText;
      private Color calendarTitleBackColor = SystemColors.ActiveCaption;
      private Color calendarTitleForeColor = SystemColors.ActiveCaptionText;
      private Color calendarTrailingForeColor = SystemColors.GrayText;
      private string customFormat = string.Empty;
      private DateTimeSelectorFormat format = DateTimeSelectorFormat.Custom;
      private static readonly DateTimeSelectorPopup datePopup = new DateTimeSelectorPopup();
      private RightToLeft calendarRightToLeft = RightToLeft.Inherit;
      private static DateTimeFormatInfo persianFormat;
      private bool usePersianFormat = false;
      #endregion

      #region Constructors
      public DateTimeSelector() {
         this.Text = null;
         this.Format = DateTimeSelectorFormat.Short;
      }
      #endregion

      #region Properties
      [Browsable(false)]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      public override Image BackgroundImage {
         get {
            return base.BackgroundImage;
         }
         set {
            base.BackgroundImage = value;
         }
      }
      [Browsable(false)]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      public override ImageLayout BackgroundImageLayout {
         get {
            return base.BackgroundImageLayout;
         }
         set {
            base.BackgroundImageLayout = value;
         }
      }
      [Browsable(true)]
      [Category("Appearance")]
      [DefaultValue(typeof(Color), "Window")]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
      [EditorBrowsable(EditorBrowsableState.Always)]
      public Color CalendarBackColor {
         get {
            return this.calendarBackColor;
         }
         set {
           this.calendarBackColor = value;
         }
      }
      [Browsable(true)]
      [Category("Appearance")]
      [DefaultValue(typeof(Color), "WindowText")]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
      [EditorBrowsable(EditorBrowsableState.Always)]
      public Color CalendarForeColor {
         get {
            return this.calendarForeColor;
         }
         set {
            this.calendarForeColor = value;
         }
      }
      [Browsable(true)]
      [Category("Appearance")]
      [DefaultValue(typeof(Color), "ActiveCaption")]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
      [EditorBrowsable(EditorBrowsableState.Always)]
      public Color CalendarTitleBackColor {
         get {
            return this.calendarTitleBackColor;
         }
         set {
            this.calendarTitleBackColor = value;
         }
      }
      [Browsable(true)]
      [Category("Appearance")]
      [DefaultValue(typeof(Color), "ActiveCaptionText")]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
      [EditorBrowsable(EditorBrowsableState.Always)]
      public Color CalendarTitleForeColor {
         get {
            return this.calendarTitleForeColor;
         }
         set {
            this.calendarTitleForeColor = value;
         }
      }
      [Browsable(true)]
      [Category("Appearance")]
      [DefaultValue(typeof(Color), "GrayText")]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
      [EditorBrowsable(EditorBrowsableState.Always)]
      public Color CalendarTrailingForeColor {
         get {
            return this.calendarTrailingForeColor;
         }
         set {
            this.calendarTrailingForeColor = value;
         }
      }
      [Browsable(true)]
      [Category("Appearance")]
      [DefaultValue(RightToLeft.Inherit)]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
      [EditorBrowsable(EditorBrowsableState.Always)]
      public RightToLeft CalendarRightToLeft {
         get {
            return this.calendarRightToLeft;
         }
         set {
            this.calendarRightToLeft = value;
         }
      }
      protected override IDropDownChild Child {
         get {
            return this.ChildInternal;
         }
      }
      private DateTimeSelectorChild ChildInternal {
         get {
            if (this.child == null) {
               this.child = new DateTimeSelectorChild(this);

               this.child.LostFocus += new EventHandler(child_LostFocus);
            }
            return this.child;
         }
      }
      [Browsable(true)]
      [Category("Behaivor")]
      [DefaultValue("")]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
      [EditorBrowsable(EditorBrowsableState.Always)]
      public string CustomFormat {
         get {
            return this.customFormat;
         }
         set {
            if (!string.Equals(this.customFormat, value)) {
               this.customFormat = value;
               this.OnCustomFormatChanged();
            }
         }
      }
      private DateTimeSelectorPopup DatePopup {
         get {
            return datePopup;
         }
      }
      [Browsable(false)]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      [EditorBrowsable(EditorBrowsableState.Always)]
      public DateTimeFormatInfo DateTimeFormat {
         get {
            return this.child.DateTimeFormat;
         }
         set {
            if (!this.child.DateTimeFormat.Equals(value)) {
               this.child.DateTimeFormat = value;
               this.OnDateTimeFormatChanged();
            }
         }
      }
      [Browsable(true)]
      [Category("Behaivor")]
      [DefaultValue(DateTimeSelectorFormat.Short)]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
      [EditorBrowsable(EditorBrowsableState.Always)]
      public DateTimeSelectorFormat Format {
         get {
            return this.format;
         }
         set {
            if (this.format != value) {
               this.format = value;
               this.OnFormatChanged();
            }
         }
      }
      protected override IDropDownPopup Popup {
         get {
            return this.DatePopup;
         }
      }
      [Browsable(false)]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      [DefaultValue(null)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      public override string Text {
         get {
            DateTime? value = this.Value;
            if (value.HasValue) {
               string formatString = this.GetFormatString();
               return DateTimeFormatter.Format(formatString, this.DateTimeFormat, this.DateTimeFormat.Calendar,
                                               this.Value);
               // return value.Value.ToString(formatString, this.DateTimeFormat);
            }
            return string.Empty;
         }
         set {
            if (string.IsNullOrEmpty(value)) {
               this.Value = null;
            }
            else {
               DateTime dateTime;
               if (DateTime.TryParse(value, this.DateTimeFormat, DateTimeStyles.None, out dateTime)) {
                  this.Value = dateTime;
               }
               else {
                  this.Value = null;
               }
               // else {
               //   throw new ArgumentOutOfRangeException("value", value + " is not a valid date/time string.");
               // }
            }        
         }
      }
      [Browsable(false)]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      [EditorBrowsable(EditorBrowsableState.Always)]
      public DateTime? Time {
         get {
            return this.child.Time;
         }
      }
      [Browsable(true)]
      [Category("Behaivor")]
      [DefaultValue(false)]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
      [EditorBrowsable(EditorBrowsableState.Always)]
      public bool UsePersianFormat {
         get {
            return this.usePersianFormat;
         }
         set {
            if (this.usePersianFormat != value) {
               this.usePersianFormat = value;
               this.OnUsePersianFormatChanged();
            }
         }
      }
      [Browsable(true)]
      [Category("Behaivor")]
      [DefaultValue(null)]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
      [EditorBrowsable(EditorBrowsableState.Always)]
      public DateTime? Value {
         get {
            return this.ValueInternal;
         }
         set {
            this.ValueInternal = value;
         }
      }
      internal DateTime? ValueInternal {
         get {
            return this.child.Value;
         }
         set {
            DateTime? currentValue = this.ValueInternal;
            if ((currentValue.HasValue ^ value.HasValue) ||
                    (currentValue.HasValue && !currentValue.Value.Equals(value.Value))) {
               this.child.Value = value;
               this.OnValueChangedInternal();
            }
         }
      }
      #endregion

      #region Methods
      private void child_LostFocus(object sender, EventArgs e) {
         if (!this.ContainsFocus && !this.Popup.ContainsFocus) {
            this.OnLostFocus(e);
         }
      }
      protected override void CloseDropDown() {
         this.DatePopup.Cancel -= DatePopup_Cancel;
         this.DatePopup.DateSelected -= DatePopup_DateSelected;
         base.CloseDropDown();
      }
      public void CommitChanges() {
         this.child.CommitChanges();
      }
      private void DatePopup_Cancel(object sender, EventArgs e) {
         this.CloseDropDown();
      }
      private void DatePopup_DateSelected(object sender, EventArgs e) {
         this.CloseDropDown();
         DateTime selectedValue = datePopup.Value;
         if (this.ValueInternal.HasValue) {
            DateTime value = this.ValueInternal.Value;
            this.ValueInternal = new DateTime(selectedValue.Year, selectedValue.Month, selectedValue.Day,
                                              value.Hour, value.Minute, value.Second, value.Millisecond, value.Kind);
         }
         else {
            if (this.child.Time.HasValue) {
               DateTime time = this.child.Time.Value;
               this.ValueInternal = new DateTime(selectedValue.Year, selectedValue.Month, selectedValue.Day,
                                                 time.Hour, time.Minute, time.Second, time.Millisecond);
            }
            else {
               this.ValueInternal = datePopup.Value;
            }
         }
      }
      protected override void DropDownInternal() {
         this.child.CommitChanges();
         this.DatePopup.Cancel += new EventHandler(DatePopup_Cancel);
         this.DatePopup.DateSelected += new EventHandler(DatePopup_DateSelected);
         this.DatePopup.Font = this.Font;
         this.DatePopup.CalendarControl.ForeColor = this.calendarForeColor;
         this.DatePopup.RightToLeft = this.calendarRightToLeft;
         this.DatePopup.Value = this.ValueInternal.HasValue ? this.ValueInternal.Value : DateTime.Now;
         // Calendar
         CalendarControl calendar = this.DatePopup.CalendarControl;
         calendar.BackColor = this.calendarBackColor;
         calendar.ForeColor = this.calendarForeColor;
         calendar.TitleBackColor = this.calendarTitleBackColor;
         calendar.TitleForeColor = this.calendarTitleForeColor;
         calendar.TrailingForeColor = this.calendarTrailingForeColor;
         base.DropDownInternal();
      }
      private string GetFormatString() {
         switch (this.format) {
            case DateTimeSelectorFormat.Custom:
               return this.customFormat;
            case DateTimeSelectorFormat.Long:
               return this.DateTimeFormat.LongDatePattern;
             case DateTimeSelectorFormat.Short:
               return this.DateTimeFormat.ShortDatePattern;
            case DateTimeSelectorFormat.Time:
               return this.DateTimeFormat.LongTimePattern;
            default:
               throw new InvalidOperationException("Invalid format.");
         }
      }
      public override Point GetDropDownLocation() {
         Point location = base.GetDropDownLocation();
         int x;
         if (this.RightToLeft != RightToLeft.Yes) {
            x = this.ArrowBounds.Left - this.Popup.Width;
         }
         else {
            x = this.ArrowBounds.Right;
         }
         x = this.PointToScreen(new Point(x, 0)).X;
         return new Point(x, location.Y);
      }
      public override Size GetPreferredSize(Size proposedSize) {
         Size basePreferredSize = base.GetPreferredSize(proposedSize);
         Size childProposedSize = new Size(Math.Max(0, proposedSize.Width - this.ArrowWidth - ArrowSeparatorWidth - 
                                           this.BorderSize.Width * 2), proposedSize.Height);
         Size childPreferredSize = this.child.GetPreferredSize(childProposedSize);
         int preferredWidth =
               childPreferredSize.Width + 
               this.ArrowWidth + 
               ArrowSeparatorWidth + 
               this.BorderSize.Width * 2;
         return new Size(Math.Max(preferredWidth, basePreferredSize.Width), basePreferredSize.Height);
      }
      public string GetText(string format) {
         if (string.IsNullOrEmpty(format)) {
            return this.Text;
         }
         DateTime? value = this.Value;
         if (value.HasValue) {
            if (!string.IsNullOrEmpty(format)) {
               // return value.Value.ToString(format, this.DateTimeFormat);
               return DateTimeFormatter.Format(format, this.DateTimeFormat, this.DateTimeFormat.Calendar,
                                               this.Value);
            }
         }
         return string.Empty;
      }
      //protected override void OnAutoSizeChanged(EventArgs e) {
      //   base.OnAutoSizeChanged(e);
      //   if (this.AutoSize) {
      //      Size propSize = this.Size;
      //      Size prefSize = this.GetPreferredSize(propSize);
      //      this.SetBounds(0, 0, prefSize.Width, prefSize.Height, BoundsSpecified.Width | BoundsSpecified.Height);
      //   }
      //}
      private void OnCustomFormatChanged() {
         if (this.format == DateTimeSelectorFormat.Custom) {
            this.child.Format = this.CustomFormat;
            Control parent = this.Parent;
            if (parent != null) {
               parent.PerformLayout(this, "CustomFormat");
            }
         }
      }
      protected override void OnFontChanged(EventArgs e) {
         this.DatePopup.Font = this.Font;
         base.OnFontChanged(e);
      }
      private void OnFormatChanged() {
         switch (this.format) {
            case DateTimeSelectorFormat.Custom:
               this.child.Format = this.CustomFormat;
               break;
            case DateTimeSelectorFormat.Short:
               this.child.Format = this.DateTimeFormat.ShortDatePattern;
               break;
            case DateTimeSelectorFormat.Long:
               this.child.Format = this.DateTimeFormat.LongDatePattern;
               break;
            case DateTimeSelectorFormat.Time:
               this.child.Format = this.DateTimeFormat.LongTimePattern;
               break;
         }
         Control parent = this.Parent;
         if (parent != null) {
            parent.PerformLayout(this, "Format");
         }
      }
      protected override void OnEnter(EventArgs e) {
         base.OnEnter(e);
         this.child.Focus();
      }
      private void OnUsePersianFormatChanged() {
         if (this.usePersianFormat) {
            if (persianFormat == null) {
               persianFormat = PersianDateTimeFormat.GetPersianDateTimeFormat();
            }
            this.DateTimeFormat = persianFormat;
         }
         else {
            this.DateTimeFormat = null;
         }
      }
      protected virtual void OnValueChanged() {
         if (this.ValueChanged != null) {
            this.ValueChanged(this, EventArgs.Empty);
         }
      }
      private void OnValueChangedInternal() {
         if (this.Value == this.ValueInternal) {
            this.OnValueChanged();
         }
      }
      private void OnDateTimeFormatChanged() {
         this.OnFormatChanged();
         this.DatePopup.DateTimeFormat = this.DateTimeFormat;
      }
      public bool ShouldCommit() {
         return this.child.ShouldCommit();
      }
      protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
         height = base.PreferredHeight;
         base.SetBoundsCore(x, y, width, height, specified);
      }
      #endregion

      #region DateTiemPickerChild
      [ToolboxItem(false)]
      class DateTimeSelectorChild : Control, IDropDownChild {
         private int activeComponent;
         private Rectangle[] componentBounds;
         private Padding componentPadding = new Padding(1);
         private DateTimeFormatInfo dateTimeFormat;
         private DateTimeSelector dateTimeSelector;
         private TextBox editorTextBox;
         private string format = string.Empty;
         private DateTimeFormatter dateTimeFormatter;
         private IList<DateTimeFormatter.SpecifierInfo> formatSpecifiersInfo;
         private bool init;
         private bool isInStaticMode = true;
         private int lastActiveComponent = -1;
         private char widestDigitChar;
         public DateTimeSelectorChild(DateTimeSelector dateTimeSelector) {
            if (dateTimeSelector == null) {
               throw new ArgumentNullException("dateTimeSelector");
            }
            this.dateTimeSelector = dateTimeSelector;
            this.editorTextBox = new TextBox();
            this.SuspendLayout();
            // dateComponentTextBox
            this.editorTextBox.BackColor = this.BackColor;
            this.editorTextBox.BorderStyle = BorderStyle.None;
            this.editorTextBox.GotFocus += new EventHandler(editorTextBox_GotFocus);
            this.editorTextBox.LostFocus += new EventHandler(editorTextBox_LostFocus);
            this.editorTextBox.KeyPress += new KeyPressEventHandler(editorTextBox_KeyPress);
            this.editorTextBox.TabStop = false;
            //DateTimeSelectorChild
            this.ActiveComponent = -1;
            this.Controls.Add(this.editorTextBox);
            this.init = true;
            this.ResumeLayout();
         }
         #region Properties
         private int ActiveComponent {
            get {
               return this.activeComponent;
            }
            set {
               if (this.activeComponent != value) {
                  if (init) {
                     this.OnActiveComponentChanging();
                  }
                  this.activeComponent = value;
                  this.OnActiveComponentChanged();
               }
            }
         }
         [Browsable(false)]
         [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
         [EditorBrowsable(EditorBrowsableState.Always)]
         public DateTimeFormatInfo DateTimeFormat {
            get {
               if (this.dateTimeFormat == null) {
                  return CultureInfo.CurrentCulture.DateTimeFormat;
               }
               return this.dateTimeFormat;
            }
            set {
               if (!this.DateTimeFormat.Equals(value)) {
                  this.dateTimeFormat = value;
                  this.OnDateTimeFormatChanged();
               }
            }
         }
         [Browsable(true)]
         [DefaultValue("")]
         [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
         [EditorBrowsable(EditorBrowsableState.Always)]
         public string Format {
            get {
               return this.format;
            }
            set {
               if (this.format != value) {
                  this.format = value;
                  this.OnFormatChanged();
               }
            }
         }
         private DateTimeFormatter Formatter {
            get {
               if (this.dateTimeFormatter == null) {
                  this.dateTimeFormatter = new DateTimeFormatter(this.DateTimeFormat);
                  this.dateTimeFormatter.ValueChanged += new EventHandler(dateTimeFormatter_ValueChanged);
               }
               return this.dateTimeFormatter;
            }
            set {
               this.dateTimeFormatter = value;
            }
         }
         private bool IsInStaticMode {
            get {
               return this.isInStaticMode;
            }
            set {
               if (this.isInStaticMode != value) {
                  this.isInStaticMode = value;
               }
            }
         }
         [Browsable(false)]
         [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
         public DateTime? Time {
            get {
               return this.Formatter.Time;
            }
         }
         [Browsable(true)]
         [DefaultValue(null)]
         [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
         [EditorBrowsable(EditorBrowsableState.Always)]
         public DateTime? Value {
            get {
               return this.Formatter.Value;
            }
            set {
               if ((this.Value.HasValue ^ value.HasValue) ||
                   (this.Value.HasValue && !this.Value.Value.Equals(value.Value))) {
                  this.Formatter.Value = value;
                  this.OnValueChanged();
               }
            }
         }
         #endregion

         #region Methods
         protected override void OnBackColorChanged(EventArgs e) {
            this.editorTextBox.BackColor = this.BackColor;
            base.OnBackColorChanged(e);
         }
         internal void CommitChanges() {
            this.EditCurrentComponentValue();
         }
         private void dateTimeFormatter_ValueChanged(object sender, EventArgs e) {
            this.OnValueChanged();
         }
         private bool DecreaseCurrentComponent(int amount) {
            if (!this.IsInStaticMode) {

               var spicifier = this.formatSpecifiersInfo[this.ActiveComponent];
               bool decreased = false;
               for (int i = 0; i < amount; i++) {
                  if (!this.Formatter.DecreaseComponentValue(spicifier)) {
                     break;
                  }
                  decreased = true;
               }
               if (decreased) {
                  this.editorTextBox.Text = this.Formatter.GetDisplayText(spicifier);
                  this.Invalidate();
               }
               this.editorTextBox.SelectAll();
               return true;
            }
            return false;
         }
         private void editorTextBox_LostFocus(object sender, EventArgs e) {
            if (this.ActiveComponent != -1 && !this.ContainsFocus) {
               // this.lastActiveComponent = this.ActiveComponent;
               this.ActiveComponent = -1;
            }
            this.OnLostFocus(e);
         }
         private void EditCurrentComponentValue() {
            if (this.ActiveComponent != -1) {
               var specifier = this.formatSpecifiersInfo[this.ActiveComponent];
               if (specifier.ValueType == DateTimeFormatter.ValueType.Numeral) {
                  string text = this.editorTextBox.Text.Trim();
                  if (!string.IsNullOrEmpty(text)) {
                     int value;
                     if (int.TryParse(this.editorTextBox.Text, out value)) {
                        if (this.Formatter.EditComponentValue(specifier, value, true, text.Length)) {
                           this.Invalidate();
                        }
                     }
                  }
                  else {
                     this.Formatter.ClearComponentValue(specifier);
                  }
               }
            }
         }
         private void editorTextBox_GotFocus(object sender, EventArgs e) {
            if (!this.IsInStaticMode) {
               if (this.ActiveComponent == -1) {
                  if (this.lastActiveComponent >= 0 &&
                      this.lastActiveComponent < this.formatSpecifiersInfo.Count) {
                     this.ActiveComponent = this.lastActiveComponent;
                  }
                  else {
                     this.SelectNextEditableComponent();
                  }
               }
               this.TabStop = false;
            }
            editorTextBox.SelectAll();
         }
         private void editorTextBox_KeyPress(object sender, KeyPressEventArgs e) {
            // Calculate new text if we don't handle the event
            StringBuilder sb = new StringBuilder(editorTextBox.Text);
            sb.Remove(editorTextBox.SelectionStart, editorTextBox.SelectionLength);
            if (e.KeyChar >= 0x20) {
               sb.Insert(editorTextBox.SelectionStart, e.KeyChar);
            }
            var text = sb.ToString().Trim();
            int value;
            bool isBackspace = e.KeyChar == '\b';
            if (text.Length > 0 && int.TryParse(text, out value)) {
               var specifier = this.formatSpecifiersInfo[this.ActiveComponent];
               if (this.Formatter.EditComponentValue(specifier, value, false, text.Length)) {
                  return; // e.Handled = false
               }
            }
            else if (isBackspace) {
               return; // Do not handle backspace
            }
            e.Handled = true;
         }
         public override Size GetPreferredSize(Size proposedSize) {
            Size basePreferredSize = base.GetPreferredSize(proposedSize);
            int totalWidth = 0;
            foreach (var bound in this.componentBounds) {
               totalWidth += bound.Width;
            }
            return new Size(totalWidth, basePreferredSize.Height);
         }
         private bool IncreaseCurrentComponent(int amount) {
            if (!this.IsInStaticMode) {
               var spicifier = this.formatSpecifiersInfo[this.ActiveComponent];
               bool increased = false;
               for (int i = 0; i < amount; i++) {
                  if (!this.Formatter.IncreaseComponentValue(spicifier)) {
                     break;
                  }
                  increased = true;
               }
               if (increased) {
                  this.editorTextBox.Text = this.Formatter.GetDisplayText(spicifier);
                  this.Invalidate();
               }
               this.editorTextBox.SelectAll();
               return true;
            }
            return false;
         }
         private bool IsComponentEditable(int iComponent) {
            if (iComponent >= this.formatSpecifiersInfo.Count) {
               return false;
            }
            var specifier = this.formatSpecifiersInfo[iComponent];
            return specifier.ValueType == DateTimeFormatter.ValueType.Numeral ||
                specifier.ValueType == DateTimeFormatter.ValueType.Items;
         }
         private void MeasureBounds() {
            if (this.formatSpecifiersInfo == null) {
               this.componentBounds = null;
               return;
            }
            this.componentBounds = new Rectangle[this.formatSpecifiersInfo.Count];
            int x = 0;
            TextFormatFlags format = TextFormatFlags.NoPadding;
            using (var graphics = this.CreateGraphics()) {
               Rectangle bounds = Rectangle.Empty;
               for (int i = 0; i < this.formatSpecifiersInfo.Count; i++) {
                  var specifier = this.formatSpecifiersInfo[i];
                  switch (specifier.ValueType) {
                     case DateTimeFormatter.ValueType.Numeral:
                        string widestNum = new string(this.widestDigitChar, specifier.MaxLength);
                        Size numSize = TextRenderer.MeasureText(graphics, widestNum, this.Font, Size.Empty, format);
                        bounds = new Rectangle(x, (this.Height - numSize.Height) / 2, numSize.Width, numSize.Height);
                        x = bounds.Right;
                        break;
                     case DateTimeFormatter.ValueType.Items:
                     case DateTimeFormatter.ValueType.StaticItems:
                        string[] items = this.Formatter.GetItems(specifier.Type);
                        int widestItemWidth = 0;
                        Size itemSize = Size.Empty;
                        int tallestItemHeight = 0;
                        foreach (var item in items) {
                           itemSize = TextRenderer.MeasureText(graphics, item, this.Font, Size.Empty, format);
                           widestItemWidth = Math.Max(widestItemWidth, itemSize.Width);
                           tallestItemHeight = Math.Max(tallestItemHeight, itemSize.Height);
                        }
                        bounds = new Rectangle(x, (this.Height - tallestItemHeight) / 2, widestItemWidth, tallestItemHeight);
                        x = bounds.Right;
                        break;
                     case DateTimeFormatter.ValueType.StringLiteral:
                     case DateTimeFormatter.ValueType.Static:
                        string text = this.Formatter.GetDisplayText(specifier);
                        Size textSize = TextRenderer.MeasureText(graphics, text, this.Font, Size.Empty, format);
                        bounds = new Rectangle(x, (this.Height - textSize.Height) / 2, textSize.Width, textSize.Height);
                        
                        break;
                  }
                  // Add padding
                  bounds.Width += componentPadding.Horizontal;
                  // bounds.Height += componentPadding.Vertical;
                  x = bounds.Right;
                  this.componentBounds[i] = bounds;
               }
            }
            if (this.RightToLeft == RightToLeft.Yes) {
               int right = 0;
               int count = componentBounds.Length;
               if (count > 0) {
                  this.componentBounds[count - 1].X = this.Width - this.componentBounds[count - 1].Right;
                  right = this.componentBounds[count - 1].Right;
               }
               for (int i = count - 2; i >= 0; i--) {
                  var elementBound = this.componentBounds[i];
                  this.componentBounds[i].X = this.componentBounds[i + 1].Right;
                  right = this.componentBounds[i].Right;
               }
            }
         }
         private void MeasureFoundRelatedStuff() {
            // Find widest digit
            int widestDigitWidth = int.MinValue;
            using (var graphics = this.CreateGraphics()) {
               for (int i = 0; i <= 9; i++) {
                  char digitChar = (char)('0' + i);
                  Size digitSize = TextRenderer.MeasureText(graphics,
                                   new string(digitChar, 1), this.Font, Size.Empty,
                                   TextFormatFlags.NoPadding);
                  int digitWidth = digitSize.Width;
                  if (widestDigitWidth < digitWidth) {
                     widestDigitWidth = digitWidth;
                     widestDigitChar = digitChar;
                  }
               }
            }
         }
         private void OnActiveComponentChanged() {
            if (this.ActiveComponent != -1) {
               Debug.Assert(this.ActiveComponent < this.formatSpecifiersInfo.Count);
               var specifier = this.formatSpecifiersInfo[this.ActiveComponent];
               Rectangle bounds = this.componentBounds[this.ActiveComponent];
               Size hiddenRegionSize = SystemInformation.Border3DSize + new Size(1, 1);
               // WinForms TextBox Padding is Padding(1)
               // Dan.UI.TextBox has a hidden region around itself when border style is 
               // set to BorderStyle.None
               bounds.Inflate(hiddenRegionSize);
               // We hide editor without losing focus before chaning all the 
               // properties by assigning empty rectangle to its bounds.
               //if (specifier.ValueType != DateTimeFormatter.ValueType.Numeral) {
               //    NativeMethods.HideCaret(this.editorTextBox.Handle);
               //}
               //else {
               //    NativeMethods.ShowCaret(this.editorTextBox.Handle);
               //}
               this.editorTextBox.Bounds = Rectangle.Empty;
               if (specifier.ValueType == DateTimeFormatter.ValueType.Numeral) {
                  this.editorTextBox.Cursor = Cursors.IBeam;
                  this.editorTextBox.TextAlign = HorizontalAlignment.Right;
               }
               else {
                  this.editorTextBox.Cursor = Cursors.Default;
                  this.editorTextBox.TextAlign = HorizontalAlignment.Center;
               }
               this.editorTextBox.MaxLength = specifier.MaxLength;
               this.editorTextBox.Text = this.Formatter.GetDisplayText(specifier);
               this.editorTextBox.Bounds = bounds;
               //if (!this.ContainsFocus) {
               //   this.Focus();
               //}
               this.editorTextBox.Focus();
               this.editorTextBox.Select(0, this.editorTextBox.Text.Length);
            }
            else {
               this.editorTextBox.Width = 0;
            }
            this.Invalidate();
         }
         private void OnActiveComponentChanging() {
            this.EditCurrentComponentValue();
         }
         private void OnDateTimeFormatChanged() {
            if (this.dateTimeFormatter != null) {
               this.dateTimeFormatter.ValueChanged -= dateTimeFormatter_ValueChanged;
            }
            this.dateTimeFormatter = new DateTimeFormatter(this.DateTimeFormat, this.Value);
            this.dateTimeFormatter.ValueChanged += new EventHandler(dateTimeFormatter_ValueChanged);
            this.formatSpecifiersInfo = DateTimeFormatter.GetSpecifiers(this.format);
            this.UpdateMode();
            this.MeasureFoundRelatedStuff();
            this.MeasureBounds();
            this.Invalidate();
         }
         protected override void OnEnter(EventArgs e) {
            if (!this.IsInStaticMode) {
               if (this.ActiveComponent == -1) {
                  if (this.lastActiveComponent >= 0 &&
                      this.lastActiveComponent < this.formatSpecifiersInfo.Count) {
                     this.ActiveComponent = this.lastActiveComponent;
                  }
                  else {
                     this.SelectNextEditableComponent();
                  }
               }
               this.TabStop = false;
            }
            base.OnEnter(e);
         }
         protected override void OnFontChanged(EventArgs e) {
            this.MeasureFoundRelatedStuff();
            this.MeasureBounds();
            base.OnFontChanged(e);
         }
         private void OnFormatChanged() {
            this.formatSpecifiersInfo = DateTimeFormatter.GetSpecifiers(this.Format);
            this.UpdateMode();
            this.MeasureFoundRelatedStuff();
            this.MeasureBounds();
            this.Invalidate();
         }
         protected override void OnGotFocus(EventArgs e) {
            if (this.IsInStaticMode) {
               this.Invalidate();
            }
            else if (this.activeComponent != -1) {
               this.editorTextBox.Focus();
            }
            base.OnGotFocus(e);
         }
         protected override void OnLeave(EventArgs e) {
            Debug.WriteLine("DateTimeSelectorChild::OnLeave");
            if (this.IsInStaticMode) {
               this.Invalidate();
            }
            else if (this.ActiveComponent != -1) {
               Debug.WriteLine("this.ActiveComponent = -1;");

               // this.lastActiveComponent = this.ActiveComponent;
               this.ActiveComponent = -1;
            }
            this.Formatter.ClearInvalids();
            this.TabStop = this.dateTimeSelector.TabStop;
            base.OnLeave(e);
         }
         protected override void OnLostFocus(EventArgs e) {
            if (this.IsInStaticMode) {
               this.Invalidate();
            }
            base.OnLostFocus(e);
         }
         protected override void OnMouseDown(MouseEventArgs e) {
            int iSelectedComponent = -1;
            for (int i = 0; i < this.componentBounds.Length; i++) {
               if (this.componentBounds[i].Contains(e.Location)) {
                  iSelectedComponent = i;
                  break;
               }
            }
            if (iSelectedComponent != -1) {
               if (this.IsComponentEditable(iSelectedComponent)) {
                  this.ActiveComponent = iSelectedComponent;
               }
               else if (this.ActiveComponent == -1) {
                  if (!this.SelectNextEditableComponent()) {
                     this.Focus();
                  }
               }
            }
            else if (this.ActiveComponent == -1) {
               if (!this.SelectNextEditableComponent()) {
                  this.Focus();
               }
            }
            base.OnMouseDown(e);
         }
         protected override void OnMouseWheel(MouseEventArgs e) {
            int increaseCount = e.Delta / 120;
            if (increaseCount > 0) {
               this.IncreaseCurrentComponent(increaseCount);
            }
            else {
               this.DecreaseCurrentComponent(-increaseCount);
            }
            base.OnMouseWheel(e);
         }
         protected override void OnSizeChanged(EventArgs e) {
            this.MeasureFoundRelatedStuff();
            this.MeasureBounds();
            base.OnSizeChanged(e);
         }
         protected override void OnPaint(PaintEventArgs e) {
            // Random rand = new Random((int)DateTime.Now.Ticks);
            for (int i = 0; i < this.formatSpecifiersInfo.Count; i++) {
               Rectangle rectangle = this.componentBounds[i];
               //Color color = Color.FromArgb(255, rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255));
               //using (var brush = new SolidBrush(color)) {
               //   e.Graphics.FillRectangle(brush, rectangle);
               //}
               if (e.ClipRectangle.IntersectsWith(rectangle)) {
                  var fspInfo = this.formatSpecifiersInfo[i];
                  string text = this.Formatter.GetDisplayText(fspInfo);
                  TextFormatFlags format = TextFormatFlags.NoPadding | TextFormatFlags.VerticalCenter;
                  if (fspInfo.ValueType == DateTimeFormatter.ValueType.Numeral) {
                     if (this.RightToLeft != RightToLeft.Yes) {
                        format |= TextFormatFlags.Right;
                     }
                     else {
                        format |= TextFormatFlags.Left;
                     }
                  }
                  else {
                     format |= TextFormatFlags.HorizontalCenter;
                  }
                  if (this.RightToLeft == RightToLeft.Yes) {
                     format |= TextFormatFlags.RightToLeft;
                  }
                  TextRenderer.DrawText(e.Graphics, text, this.Font, rectangle, this.ForeColor,
                                       Color.Transparent, format);
               }
            }
            if (this.IsInStaticMode && this.Focused) {
               Rectangle rectangle = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
               ControlPaint.DrawFocusRectangle(e.Graphics, rectangle);
            }
         }
         protected override void OnRightToLeftChanged(EventArgs e) {
            this.MeasureBounds();
            base.OnRightToLeftChanged(e);
         }
         private void OnValueChanged() {
            if (this.ActiveComponent != -1) {
               var specifier = this.formatSpecifiersInfo[this.ActiveComponent];
               this.editorTextBox.Text = this.Formatter.GetDisplayText(specifier);
               this.editorTextBox.SelectAll();
            }
            this.Invalidate();
            this.dateTimeSelector.OnValueChanged();
         }
         protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            switch (keyData) {
               case Keys.Up:
                  if (this.IncreaseCurrentComponent(1)) {
                     return true;
                  }
                  break;
               case Keys.Down:
                  if (this.DecreaseCurrentComponent(1)) {
                     return true;
                  }
                  break;
               case Keys.Right:
                  if (editorTextBox.SelectionLength == editorTextBox.Text.Length ||
                      editorTextBox.SelectionStart == editorTextBox.Text.Length) {
                     if (this.RightToLeft != RightToLeft.Yes) {
                        if (this.SelectNextEditableComponent()) {
                           return true;
                        }
                     }
                     else {
                        if (this.SelectPrevEditableComponent()) {
                           return true;
                        }
                     }
                  }
                  break;
               case Keys.Left:
                  if (editorTextBox.SelectionLength == editorTextBox.Text.Length ||
                      editorTextBox.SelectionStart == 0) {
                     if (this.RightToLeft != RightToLeft.Yes) {
                        if (this.SelectPrevEditableComponent()) {
                           return true;
                        }
                     }
                     else {
                        if (this.SelectNextEditableComponent()) {
                           return true;
                        }
                     }
                  }
                  break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
         }
         protected override bool ProcessDialogKey(Keys keyData) {
            Keys keyCode = keyData & Keys.KeyCode;
            Keys modifiers = keyData & Keys.Modifiers;
            switch (keyCode) {
               case Keys.Tab:
                  if ((modifiers & Keys.Control) == Keys.Control) {
                     return base.ProcessDialogKey(keyData ^ Keys.Control);
                  }
                  if ((modifiers & Keys.Shift) != Keys.Shift) {
                     if (this.SelectNextEditableComponent()) {
                        return true;
                     }
                  }
                  else {
                     if (this.SelectPrevEditableComponent()) {
                        return true;
                     }
                  }
                  break;
            }
            return base.ProcessDialogKey(keyData);
         }
         private bool SelectNextEditableComponent() {
            int startIndex = this.ActiveComponent + 1;
            for (int i = startIndex; i < this.formatSpecifiersInfo.Count; i++) {
               if (this.IsComponentEditable(i)) {
                  this.ActiveComponent = i;
                  return true;
               }
            }
            return false;
         }
         private bool SelectPrevEditableComponent() {
            int startIndex = this.ActiveComponent - 1;
            for (int i = startIndex; i >= 0; i--) {
               if (this.IsComponentEditable(i)) {
                  this.ActiveComponent = i;
                  return true;
               }
            }
            return false;
         }
         public bool ShouldCommit() {
            if (this.ActiveComponent != -1) {
               var specifier = this.formatSpecifiersInfo[this.ActiveComponent];
               if (specifier.ValueType == DateTimeFormatter.ValueType.Numeral) {
                  DateTimeFormatter formatter = this.Formatter;
                  if (formatter.ShouldCommit(specifier)) {
                     return true;
                  }
                  string text = this.editorTextBox.Text.Trim();
                  string displayText = formatter.GetDisplayText(specifier);
                  return !string.Equals(text, displayText);
               }
            }
            return false;
         }
         private void UpdateMode() {
            if (this.formatSpecifiersInfo == null) {
               this.IsInStaticMode = true;
            }
            else {
               foreach (var specifier in this.formatSpecifiersInfo) {
                  if (specifier.ValueType == DateTimeFormatter.ValueType.Numeral ||
                      specifier.ValueType == DateTimeFormatter.ValueType.Items) {
                     this.IsInStaticMode = false;
                     return;
                  }
               }
               this.IsInStaticMode = true;
            }
         }
         #endregion
      }
      #endregion

      #region Designer
      internal class DateTimeSelectorDesigner : ControlDesigner {
         DateTimeSelectorDesigner() {
            base.AutoResizeHandles = true;
         }
         public override SelectionRules SelectionRules {
            get {
               return SelectionRules.LeftSizeable | SelectionRules.RightSizeable | SelectionRules.Moveable;
            }
         }
      }
      #endregion

   }
}
