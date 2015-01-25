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
   using System.ComponentModel;
   using System.Diagnostics;
   using System.Drawing;
   using System.Globalization;
   using System.Text;
   using System.Threading;
   using System.Windows.Forms;

   class CalendarControl : UserControl {
      #region fields
      private static readonly Padding BodyPaddingDLU = new Padding(1);
      private static readonly Padding DayTextPaddingDLU = new Padding(5);
      private static readonly int DaysInWeek = 7;
      private static readonly Size GlyphSize = new Size(13, 13);
      private static readonly Padding FooterPaddingDLU = new Padding(1);
      private static readonly Padding MonthPaddingDLU = new Padding(0, 5, 0, 0);
      private static readonly int SeparatorSize = 1;
      private static readonly int RowCount = 6;
      private static readonly Padding TitlePaddingDLU = new Padding(1, 5, 1, 5);
      private static readonly Padding TitleTextMarginDLU = new Padding(1, 0, 1, 0);
      private static readonly Padding WeekDayPaddingDLU = new Padding(1, 1, 1, 10);

      public event EventHandler DateSelected;
      public event EventHandler ValueChanged;

      private Rectangle bodyBounds;
      private Padding bodyPadding;
      private int columnWidth;
      private DateTimeFormatInfo dateTimeFormat;
      private Rectangle[] dayBounds;
      private Padding dayTextPadding;
      private DayPosition focusedDayPosition;
      private Rectangle footerBounds;
      private Padding footerPadding;
      private Rectangle glyph1Bounds;
      private Rectangle glyph2Bounds;
      private DateTime maxDate = DateTime.MaxValue;
      private DateTime minDate = DateTime.MinValue;
      private Padding monthPadding;
      private int rowHeight;
      private Point[] separatorLocation;
      private Size size;
      private Color titleBackColor = SystemColors.ActiveCaption;
      private Color titleForeColor = SystemColors.ActiveCaptionText;
      private Padding titlePadding;
      private Padding titleTextMargin;
      private Rectangle todaySignBounds;
      private Color trailingForeColor = SystemColors.GrayText;
      private Rectangle titleBounds;
      private Rectangle titleTextBounds;
      private DateTime value = DateTime.Now;
      private Rectangle[] weekDayBounds;
      private Padding weekDayPadding;
      private int widestShortDateWidth;
      #endregion

      #region Constructors
      public CalendarControl() {
         // CalendarControl
         this.SuspendLayout();
         this.AutoSize = true;
         this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
         this.DoubleBuffered = true;
         this.ForeColor = SystemColors.WindowText;
         this.MeasureFontRelatedSizes();
         this.MeasureBounds();
         this.ResumeLayout(true);
         this.minDate = this.Calendar.MinSupportedDateTime;
         this.maxDate = this.Calendar.MaxSupportedDateTime;
         if (this.value.Date < this.minDate) {
            this.value = this.minDate;
         }
         else if (this.value.Date > this.maxDate) {
            this.value = this.maxDate;
         }
      }
      #endregion

      #region Properties
      [Browsable(false)]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      public override bool AutoSize {
         get {
            return base.AutoSize;
         }
         set {
            base.AutoSize = value;
         }
      }
      [Browsable(false)]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      public new AutoSizeMode AutoSizeMode {
         get {
            return base.AutoSizeMode;
         }
         set {
            base.AutoSizeMode = value;
         }
      }
      private Calendar Calendar {
         get {
            return this.DateTimeFormat.Calendar;
         }
      }
      [Browsable(false)]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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
               Calendar calendar = this.Calendar;
               if (this.minDate < calendar.MinSupportedDateTime ||
                   this.minDate > calendar.MaxSupportedDateTime) {
                  this.minDate = calendar.MinSupportedDateTime;
                  if (this.value < this.minDate) {
                     this.value = this.minDate;
                  }
               }
               if (this.maxDate < calendar.MinSupportedDateTime ||
                   this.maxDate > calendar.MaxSupportedDateTime) {
                  this.maxDate = calendar.MaxSupportedDateTime;
                  if (this.value > this.maxDate) {
                     this.value = this.maxDate;
                  }
               }
               this.OnDateTimeFormatChanged();
            }
         }
      }
      private DayPosition FocusedDayPosition {
         get {
            return this.focusedDayPosition;
         }
         set {
            if (!this.focusedDayPosition.Equals(value)) {
               if (!this.focusedDayPosition.Equals(DayPosition.Empty)) {
                  this.InvalidateDayBounds(this.focusedDayPosition);
               }
               this.focusedDayPosition = value;
               if (!value.Equals(DayPosition.Empty)) {
                  this.InvalidateDayBounds(value);
               }
            }
         }
      }
      [Browsable(true)]
      [DefaultValue(typeof(Color), "WindowText")]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
      [EditorBrowsable(EditorBrowsableState.Always)]
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
      [Browsable(false)]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      public DateTime MaxDate {
         get {
            return this.maxDate;
         }
         set {
            if (this.maxDate != value) {
               if (value.Date < this.Calendar.MinSupportedDateTime ||
                   value.Date > this.Calendar.MaxSupportedDateTime) {
                  this.ThrowCalendarDateTimeIsNotSupportedException();
               }
               if (value.Date < this.minDate) {
                  throw new ArgumentOutOfRangeException("MaxDate must be greater than or equal to MinDate");
               }
               this.maxDate = value.Date;
               if (this.value.Date > this.maxDate) {
                  this.Value = this.maxDate;
               }
            }
         }
      }
      [Browsable(false)]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      public DateTime MinDate {
         get {
            return this.minDate;
         }
         set {
            if (this.minDate != value) {
               if (value.Date < this.Calendar.MinSupportedDateTime ||
                  value.Date > this.Calendar.MaxSupportedDateTime) {
                  this.ThrowCalendarDateTimeIsNotSupportedException();
               }
               if (value.Date > this.maxDate) {
                  throw new ArgumentOutOfRangeException("MinDate must be less than or equal to MaxDate");
               }
               this.minDate = value.Date;
               if (this.value.Date < this.minDate) {
                  this.Value = this.minDate;
               }
            }
         }
      }
      [Browsable(true)]
      [DefaultValue(typeof(Color), "ActiveCaption")]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
      [EditorBrowsable(EditorBrowsableState.Always)]
      public Color TitleBackColor {
         get {
            return this.titleBackColor;
         }
         set {
            if (this.titleBackColor != value) {
               this.titleBackColor = value;
               this.Invalidate();
            }
         }
      }
      [Browsable(true)]
      [DefaultValue(typeof(Color), "ActiveCaptionText")]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
      [EditorBrowsable(EditorBrowsableState.Always)]
      public Color TitleForeColor {
         get {
            return this.titleForeColor;
         }
         set {
            if (this.titleForeColor != value) {
               this.titleForeColor = value;
               this.Invalidate();
            }
         }
      }
      [Browsable(true)]
      [DefaultValue(typeof(Color), "GrayText")]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
      [EditorBrowsable(EditorBrowsableState.Always)]
      public Color TrailingForeColor {
         get {
            return this.trailingForeColor;
         }
         set {
            if (this.trailingForeColor != value) {
               this.trailingForeColor = value;
               this.Invalidate();
            }
         }
      }
      internal DateTime ValueInternal {
         get {
            return this.value;
         }
         set {
            if (this.value != value) {
               if (this.CanAssignValue(value)) {
                  this.Value = value;
               }
            }
         }
      }
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      public DateTime Value {
         get {
            return this.value;
         }
         set {
            if (this.value != value) {
               if (value.Date > this.maxDate ||
                   value.Date < this.minDate) {
                  throw new ArgumentOutOfRangeException("value");
               }
               int currentYear = this.Calendar.GetYear(this.value);
               int currentMonth = this.Calendar.GetMonth(this.value);
               DateTime currentDate = this.value;
               this.value = value;
               if (this.Visible) {
                  if (this.Calendar.GetYear(value) != currentYear ||
                      this.Calendar.GetMonth(value) != currentMonth) {
                     this.Invalidate();
                  }
                  else {
                     this.InvalidateDayBounds(currentDate);
                     this.Update();
                     this.InvalidateDayBounds(value);
                     this.Update();
                  }
               }
               this.OnValueChanged();
            }
         }
      }
      #endregion

      #region Methods
      private bool CanAssignValue(DateTime value) {
         if (value.Date > MaxDate ||
             value.Date < MinDate) {
            return false;
         }
         return true;
      }
      private void DrawBody(Graphics graphics) {
         this.DrawWeekDays(graphics);
         this.DrawSeparator(graphics);
         this.DrawMonth(graphics);
      }
      private void DrawDay(Graphics graphics, DateTime dayDate, DayPosition dayPosition) {
         bool isCurrentDay = dayDate.Date.Equals(value.Date);
         bool isCurrentMonthDay = this.IsCurrentMonthsDate(dayDate);
         bool isToday = dayDate.Date.Equals(DateTime.Now.Date);
         string dayText = this.Calendar.GetDayOfMonth(dayDate).ToString();
         bool isFocused = focusedDayPosition.Equals(dayPosition);
         Rectangle bounds = this.GetDayBounds(dayPosition);
         Rectangle textBounds = new Rectangle(bounds.Left + dayTextPadding.Left,
                                                 bounds.Top + dayTextPadding.Top,
                                                 bounds.Width - dayTextPadding.Horizontal,
                                                 bounds.Height - dayTextPadding.Vertical);

         TextFormatFlags format = TextFormatFlags.Right | TextFormatFlags.VerticalCenter;
         if (this.RightToLeft == RightToLeft.Yes) {
            format |= TextFormatFlags.RightToLeft;
         }
         Color foreColor;
         Color backColor;
         if (isCurrentDay) {
            foreColor = this.titleForeColor;
            backColor = this.titleBackColor;
         }
         else if (!isCurrentMonthDay) {
            foreColor = this.trailingForeColor;
            backColor = this.BackColor;
         }
         else {
            foreColor = this.ForeColor;
            backColor = this.BackColor;
         }
         using (var brush = new SolidBrush(backColor)) {
            Rectangle rect = bounds;
            rect.Inflate(-1, -1);
            graphics.FillRectangle(brush, rect);
         }
         TextRenderer.DrawText(graphics, dayText, this.Font, textBounds, foreColor, backColor, format);
         if (isToday) {
            Rectangle rect = bounds;
            rect.Width--;
            rect.Height--;
            using (var pen = new Pen(this.titleBackColor)) {
               graphics.DrawRectangle(pen, rect);
            }
         }
         if (isFocused || (this.Focused && isCurrentDay)) {
            Rectangle rect = bounds;
            rect.Inflate(-1, -1);
            ControlPaint.DrawFocusRectangle(graphics, rect, isToday ? this.BackColor : this.ForeColor,
                                        Color.Transparent);
         }
      }
      private void DrawFooter(Graphics graphics) {
         TextFormatFlags format = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;
         if (this.RightToLeft == RightToLeft.Yes) {
            format |= TextFormatFlags.RightToLeft;
         }
         string text = DateTime.Now.ToString(this.DateTimeFormat.ShortDatePattern, this.DateTimeFormat);
         Size bounds = TextRenderer.MeasureText(graphics, text, this.Font, Size.Empty, format);
         TextRenderer.DrawText(graphics, text, this.Font, this.footerBounds, this.ForeColor, this.BackColor, format);
         using (var pen = new Pen(this.titleBackColor)) {
            graphics.DrawRectangle(pen, todaySignBounds);
         }
      }
      private void DrawGlypth1(Graphics graphics) {
         Color foreColor;
         if ((this.RightToLeft != RightToLeft.Yes && this.IsPrevMonthInRange()) ||
             (this.RightToLeft == RightToLeft.Yes && this.IsNextMonthInRange())) {
            foreColor = this.titleForeColor;
         }
         else {
            // foreColor = SystemColors.GrayText;
            return;
         }
         using (Bitmap image = new Bitmap(GlyphSize.Width, GlyphSize.Height)) {
            using (Graphics grphcs = Graphics.FromImage(image)) {
               ControlPaint.DrawMenuGlyph(grphcs, 0, 0, GlyphSize.Width, GlyphSize.Height,
                                MenuGlyph.Arrow, this.titleForeColor, this.titleBackColor);

            }
            image.RotateFlip(RotateFlipType.RotateNoneFlipX);
            graphics.DrawImage(image, this.glyph1Bounds);
         }
      }
      private void DrawGlypth2(Graphics graphics) {
         Color foreColor;
         if ((this.RightToLeft != RightToLeft.Yes && this.IsNextMonthInRange()) ||
             (this.RightToLeft == RightToLeft.Yes && this.IsPrevMonthInRange())) {
            foreColor = this.titleForeColor;
         }
         else {
            // foreColor = SystemColors.GrayText;
            return;
         }
         ControlPaint.DrawMenuGlyph(graphics, this.glyph2Bounds, MenuGlyph.Arrow, foreColor,
                                     this.titleBackColor);
      }
      private void DrawMonth(Graphics graphics) {
         int calYear = this.Calendar.GetYear(this.value);
         int calMonth = this.Calendar.GetMonth(this.value);
         int calDay = 1;
         DateTime firstDayOfMonth = new DateTime(calYear, calMonth, calDay, this.Calendar);
         DateTime dayDate = firstDayOfMonth.AddDays(-GetPrevMonthDayCount());
         for (int i = 0; i < RowCount; i++) {
            DayOfWeek dayOfWeek = (DayOfWeek)i;
            for (int j = 0; j < DaysInWeek; j++) {
               if (dayDate >= this.Calendar.MinSupportedDateTime &&
                   dayDate <= this.Calendar.MaxSupportedDateTime) {
                  this.DrawDay(graphics, dayDate, new DayPosition(j, i));
               }
               dayDate = dayDate.AddDays(1);
            }
         }
      }
      private void DrawSeparator(Graphics graphics) {
         using (Pen pen = new Pen(this.titleBackColor)) {
            graphics.DrawLine(pen, separatorLocation[0], separatorLocation[1]);
         }
      }
      private void DrawTitle(Graphics graphics) {
         using (Brush brush = new SolidBrush(this.titleBackColor)) {
            graphics.FillRectangle(brush, this.titleBounds);
         }
         this.DrawGlypth1(graphics);
         this.DrawGlypth2(graphics);
         this.DrawTitleText(graphics);
      }
      private void DrawTitleText(Graphics graphics) {
         TextFormatFlags format = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;
         if (this.RightToLeft == RightToLeft.Yes) {
            format |= TextFormatFlags.RightToLeft;
         }
         string titleText = this.value.ToString(this.DateTimeFormat.YearMonthPattern, this.DateTimeFormat);
         TextRenderer.DrawText(graphics, titleText, this.Font, titleTextBounds, this.titleForeColor,
             this.titleBackColor, format);
      }
      private void DrawWeekDay(Graphics graphics, string weekDayText, Rectangle bounds) {
         TextFormatFlags format = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;
         if (this.RightToLeft == RightToLeft.Yes) {
            format |= TextFormatFlags.RightToLeft;
         }
         TextRenderer.DrawText(graphics, weekDayText, this.Font, bounds, this.titleBackColor,
                                 this.BackColor, format);
      }
      private void DrawWeekDays(Graphics graphics) {
         int firstDayOfWeek = (int)this.DateTimeFormat.FirstDayOfWeek;
         int weekDayPos = 0;
         for (int i = firstDayOfWeek; i < DaysInWeek; i++) {
            string weekDayName = this.DateTimeFormat.GetAbbreviatedDayName((DayOfWeek)i);
            this.DrawWeekDay(graphics, weekDayName, this.weekDayBounds[weekDayPos++]);
         }
         for (int i = 0; i < firstDayOfWeek; i++) {
            string weekDayName = this.DateTimeFormat.GetAbbreviatedDayName((DayOfWeek)i);
            this.DrawWeekDay(graphics, weekDayName, this.weekDayBounds[weekDayPos++]);
         }
      }
      private DayPosition GetDayPosition(DateTime dayDate) {
         int prevMonthDayCount = GetPrevMonthDayCount();
         int calYear = this.Calendar.GetYear(this.value);
         int calMonth = this.Calendar.GetMonth(this.value);
         DateTime firstDayOfMonth = new DateTime(calYear, calMonth, 1, this.Calendar);
         int index = prevMonthDayCount + dayDate.Date.Subtract(firstDayOfMonth).Days;
         return new DayPosition(index % DaysInWeek, index / DaysInWeek);
      }
      private int GetPrevMonthDayCount() {
         int calYear = this.Calendar.GetYear(this.value);
         int calMonth = this.Calendar.GetMonth(this.value);
         int calDay = 1;
         DateTime firstDayOfMonth = new DateTime(calYear, calMonth, calDay, this.Calendar);
         DayOfWeek firtDayWeekOfMonth = Calendar.GetDayOfWeek(firstDayOfMonth);
         DayOfWeek firstDayOfWeek = this.DateTimeFormat.FirstDayOfWeek;
         int diff = (int)firtDayWeekOfMonth - (int)firstDayOfWeek;
         if (diff < 0) {
            diff = DaysInWeek + diff;
         }
         else if (diff == 0) {
            diff = DaysInWeek;
         }
         return diff;
      }
      private DateTime GetDayDateFromPosition(int columnIndex, int rowIndex) {
         Debug.Assert(columnIndex >= 0 && columnIndex <= DaysInWeek);
         Debug.Assert(rowIndex >= 0 && rowIndex <= RowCount);
         int calYear = this.Calendar.GetYear(this.value);
         int calMonth = this.Calendar.GetMonth(this.value);
         int calDay = 1;
         DateTime firstDayOfMonth = new DateTime(calYear, calMonth, calDay, this.Calendar);
         int index = rowIndex * DaysInWeek + columnIndex;
         return firstDayOfMonth.AddDays(index - GetPrevMonthDayCount());
      }
      private Rectangle GetDayBounds(DayPosition dayPosition) {
         Debug.Assert(dayPosition.Column >= 0 && dayPosition.Column < DaysInWeek);
         Debug.Assert(dayPosition.Row >= 0 && dayPosition.Row < RowCount);
         return this.dayBounds[dayPosition.Row * DaysInWeek + dayPosition.Column];
      }
      private bool GetDayDateFromBounds(Rectangle bounds, out DateTime dayDate) {
         if (!bounds.Size.Equals(dayBounds[0].Size)) {
            dayDate = DateTime.MinValue;
            return false;
         }
         int index = -1;
         for (int i = 0; i < dayBounds.Length; i++) {
            if (dayBounds[i].Equals(bounds)) {
               index = i;
               break;
            }
         }
         if (index == -1) {
            dayDate = DateTime.MinValue;
            return false;
         }
         dayDate = GetDayDateFromPosition(index % DaysInWeek, index / DaysInWeek);
         return true;
      }
      public override Size GetPreferredSize(Size proposedSize) {
         return this.size;
         // return base.GetPreferredSize(proposedSize);
      }
      private HitTestInfo HitTest(Point location) {
         if (glyph1Bounds.Contains(location)) {
            return new HitTestInfo(HitTestType.Glyph1);
         }
         if (glyph2Bounds.Contains(location)) {
            return new HitTestInfo(HitTestType.Glyph2);
         }
         if (footerBounds.Contains(location)) {
            return new HitTestInfo(HitTestType.Footer);
         }
         for (int i = 0; i < RowCount; i++) {
            for (int j = 0; j < DaysInWeek; j++) {
               if (this.dayBounds[i * DaysInWeek + j].Contains(location)) {
                  return new HitTestInfo(HitTestType.Day, j, i);
               }
            }
         }
         return new HitTestInfo(HitTestType.None, -1, -1);
      }
      private void InvalidateDayBounds(DayPosition dayPosition) {
         Rectangle bounds = this.GetDayBounds(dayPosition);
         this.Invalidate(bounds);
      }
      private void InvalidateDayBounds(DateTime dayDate) {
         this.InvalidateDayBounds(GetDayPosition(dayDate));
      }
      private bool IsCurrentMonthsDate(DateTime date) {
         if (!this.CanAssignValue(date)) {
            return false;
         }
         return this.Calendar.GetMonth(date).Equals(Calendar.GetMonth(this.value));
      }
      private bool IsPrevMonthInRange() {
         int calYear = this.Calendar.GetYear(this.value);
         int calMonth = this.Calendar.GetMonth(this.value);
         int calDay = 1;
         DateTime firstDayOfCurrentMonth = new DateTime(calYear, calMonth, calDay, this.Calendar);
         if (this.MinDate.AddDays(1) > firstDayOfCurrentMonth) {
            return false;
         }
         DateTime lastDayOfPrevMonth = this.Calendar.AddDays(firstDayOfCurrentMonth, -1);
         return this.CanAssignValue(lastDayOfPrevMonth);
      }
      private bool IsNextMonthInRange() {
         int calYear = this.Calendar.GetYear(this.value);
         int calMonth = this.Calendar.GetMonth(this.value);
         int calDay = 1;
         DateTime firstDayOfCurrentMonth = new DateTime(calYear, calMonth, calDay, this.Calendar);
         int daysInCurrentMonth = this.Calendar.GetDaysInMonth(calYear, calMonth);
         if (this.MaxDate.AddDays(-daysInCurrentMonth) < firstDayOfCurrentMonth) {
            return false;
         }
         DateTime firstDayOfNextMonth = firstDayOfCurrentMonth.AddDays(daysInCurrentMonth);
         return this.CanAssignValue(firstDayOfNextMonth);
      }
      private void MeasureFontRelatedSizes() {
         // Find largest digit
         int widestDigitWidth = int.MinValue;
         char widestDigitChar = new char();
         for (int i = 0; i <= 9; i++) {
            char digitChar = (char)('0' + i);
            Size digitSize = TextRenderer.MeasureText(new string(digitChar, 1), this.Font);
            int digitWidth = digitSize.Width;
            if (widestDigitWidth < digitWidth) {
               widestDigitWidth = digitWidth;
               widestDigitChar = digitChar;
            }
         }
         // Find widest year/month width
         string sampleYearMonthText = DateTime.Now.ToString(this.DateTimeFormat.YearMonthPattern);
         var sbYearMonthText = new StringBuilder(sampleYearMonthText.Length);
         foreach (var ch in sampleYearMonthText) { // Replace digits with largest digit
            if (char.IsDigit(ch)) {
               sbYearMonthText.Append(widestDigitChar);
            }
            else {
               sbYearMonthText.Append(ch);
            }
         }
         sampleYearMonthText = sbYearMonthText.ToString();
         int widestYearMonthNameWidth = TextRenderer.MeasureText(sampleYearMonthText, this.Font).Width;

         // Find widest day in month (MM) format
         string sampleDayInMonthText = new string(widestDigitChar, 2);
         int widestDayInMonthWidth = TextRenderer.MeasureText(sampleDayInMonthText, this.Font).Width;

         // Find widest short date width
         string sampleShortDateText = DateTime.Now.ToString(this.DateTimeFormat.ShortDatePattern);
         var sbShortDateText = new StringBuilder(sampleShortDateText.Length);
         foreach (var ch in sampleShortDateText) { // Replace digits with largest digit
            if (char.IsDigit(ch)) {
               sbShortDateText.Append(widestDigitChar);
            }
            else {
               sbShortDateText.Append(ch);
            }
         }
         sampleShortDateText = sbShortDateText.ToString();
         this.widestShortDateWidth = TextRenderer.MeasureText(sampleShortDateText, this.Font).Width;

         // Find widest week day name
         int widestWeekDayNameWidth = int.MinValue;
         for (int i = 0; i < DaysInWeek; i++) {
            string weekDayName = this.DateTimeFormat.GetAbbreviatedDayName((DayOfWeek)i);
            Size weekDayNameSize = TextRenderer.MeasureText(weekDayName, this.Font);
            if (weekDayNameSize.Width > widestWeekDayNameWidth) {
               widestWeekDayNameWidth = weekDayNameSize.Width;
            }
         }
         // Measure body padding in pixels
         this.bodyPadding = SizeUtil.DLUToPixels(BodyPaddingDLU);
         // Measure title padding in pixels
         this.titlePadding = SizeUtil.DLUToPixels(TitlePaddingDLU);
         // Measure title text margin in pixels
         this.titleTextMargin = SizeUtil.DLUToPixels(TitleTextMarginDLU);
         // Measure week day padding
         this.weekDayPadding = SizeUtil.DLUToPixels(WeekDayPaddingDLU);
         // Measure column width 
         this.dayTextPadding = SizeUtil.DLUToPixels(DayTextPaddingDLU);
         this.columnWidth = Math.Max(widestDayInMonthWidth + this.dayTextPadding.Horizontal,
                                     widestWeekDayNameWidth + weekDayPadding.Horizontal);
         // Measure days part padding
         this.monthPadding = SizeUtil.DLUToPixels(MonthPaddingDLU);
         // Measure row hegiht
         this.rowHeight = this.Font.Height + this.dayTextPadding.Vertical;
         // Measure footer padding
         this.footerPadding = SizeUtil.DLUToPixels(FooterPaddingDLU);
      }
      private void MeasureBounds() {
         // Measure title preferred width
         int titlePrefWidth = titlePadding.Horizontal + titleTextMargin.Horizontal +
                             GlyphSize.Width * 2;
         // Measure title height
         int titleHeight = Math.Max(this.Font.Height + titleTextMargin.Vertical, GlyphSize.Height)
                             + titlePadding.Vertical;

         // Measure body preferred width
         int bodyPrefWidth = this.bodyPadding.Horizontal + this.columnWidth * DaysInWeek
                                     + this.monthPadding.Horizontal;

         // Measure body height
         int bodyHeight = bodyPadding.Vertical + SeparatorSize + Font.Height +
                         weekDayPadding.Vertical + rowHeight * RowCount +
                         this.monthPadding.Vertical;

         // Measure footer preferred width
         int footerPrefWidth = footerPadding.Horizontal + this.widestShortDateWidth + 2;

         // Measure footer height
         int footerHeight = footerPadding.Vertical + this.Font.Height + 2;

         // Measure size
         int height = titleHeight + bodyHeight + footerHeight;
         int width = Math.Max(titlePrefWidth, Math.Max(bodyPrefWidth, footerPrefWidth));
         this.size = new Size(width, height);
         if (this.BorderStyle == BorderStyle.FixedSingle) {
            this.size += SystemInformation.BorderSize;
            this.size += SystemInformation.BorderSize;
         }
         else if (this.BorderStyle == BorderStyle.Fixed3D) {
            this.size += SystemInformation.Border3DSize;
            this.size += SystemInformation.Border3DSize;
         }
         // Measure title bounds
         this.titleBounds = new Rectangle(0, 0, width, titleHeight);
         // Measure glyph1 bounds
         Rectangle glypg1Rect = new Rectangle(Point.Empty, GlyphSize);
         glypg1Rect.Offset(this.titleBounds.Left, titleBounds.Top +
                         (titleBounds.Height - titlePadding.Vertical - GlyphSize.Height) / 2);
         glypg1Rect.Offset(new Point(this.titlePadding.Left, this.titlePadding.Top));
         this.glyph1Bounds = glypg1Rect;
         // Measure glyph2 bounds
         Rectangle glypg2Rect = new Rectangle(Point.Empty, GlyphSize);
         glypg2Rect.Offset(new Point(this.titleBounds.Right - GlyphSize.Width,
             titleBounds.Top + (titleBounds.Height - titlePadding.Vertical - GlyphSize.Height) / 2));
         glypg2Rect.Offset(new Point(-this.titlePadding.Right, this.titlePadding.Top));
         this.glyph2Bounds = glypg2Rect;
         // Measure title text bounds
         titleTextBounds = new Rectangle(titleBounds.Left + titlePadding.Left + GlyphSize.Width +
                                          titleTextMargin.Left, titleBounds.Top + titlePadding.Top +
                                          titleTextMargin.Top,
                                          titleBounds.Width - 2 * GlyphSize.Width - titlePadding.Horizontal
                                          - titleTextMargin.Horizontal,
                                          titleBounds.Height - titlePadding.Vertical - titleTextMargin.Vertical);
         // Measure body bounds
         bodyBounds = new Rectangle(0, titleBounds.Height, width, bodyHeight);
         // Measure week day bounds
         this.weekDayBounds = new Rectangle[DaysInWeek];
         int weekDayHeight = this.Font.Height + this.weekDayPadding.Vertical;
         for (int i = 0; i < DaysInWeek; i++) {
            Rectangle rect = new Rectangle(i * this.columnWidth, 0, this.columnWidth, weekDayHeight);
            rect.Offset(this.bodyBounds.Left + this.bodyPadding.Left, this.bodyBounds.Top + this.bodyPadding.Top);
            if (this.RightToLeft != RightToLeft.Yes) {
               this.weekDayBounds.SetValue(rect, i);
            }
            else {
               this.weekDayBounds.SetValue(rect, DaysInWeek - i - 1);
            }
         }
         // Measure separator location
         separatorLocation = new Point[] {
                new Point(bodyBounds.Left + bodyPadding.Left, weekDayBounds[0].Bottom + 1),
                new Point(bodyBounds.Right - bodyPadding.Right - 1, weekDayBounds[0].Bottom + 1),
            };
         // Measure days bounds
         this.dayBounds = new Rectangle[DaysInWeek * RowCount];
         for (int i = 0; i < RowCount; i++) {
            for (int j = 0; j < DaysInWeek; j++) {
               Rectangle rect = new Rectangle(j * this.columnWidth, i * this.rowHeight, this.columnWidth,
                                               this.rowHeight);
               rect.Offset(this.separatorLocation[0].X + monthPadding.Left
                           , this.separatorLocation[0].Y + SeparatorSize + monthPadding.Top);
               if (this.RightToLeft != RightToLeft.Yes) {
                  this.dayBounds.SetValue(rect, i * DaysInWeek + j);
               }
               else {
                  this.dayBounds.SetValue(rect, i * DaysInWeek + DaysInWeek - j - 1);

               }
            }
         }
         // Measure footer bounds
         this.footerBounds = new Rectangle(0, height - footerHeight, width, footerHeight);
         Size todaySignSize = new Size(this.columnWidth / 2, this.rowHeight / 2);
         this.todaySignBounds = new Rectangle((footerBounds.Width - widestShortDateWidth) / 2 - todaySignSize.Width,
                                               (footerBounds.Height - todaySignSize.Height) / 2, todaySignSize.Width,
                                               todaySignSize.Height);
         this.todaySignBounds.Offset(this.footerBounds.Location);


      }
      private void NextMonth() {
         DateTime nextMonthDate = this.Calendar.AddMonths(this.value, +1);
         if (this.CanAssignValue(nextMonthDate)) {
            this.Value = nextMonthDate;
         }
         else {
            if (this.IsNextMonthInRange()) {
               this.ValueInternal = this.MaxDate;
            }
         }
      }
      private void PrevMonth() {
         DateTime prevMonthDate = this.Calendar.AddMonths(this.value, -1);
         if (this.CanAssignValue(prevMonthDate)) {
            this.Value = prevMonthDate;
         }
         else {
            if (this.IsPrevMonthInRange()) {
               this.ValueInternal = this.MinDate;
            }
         }
      }
      private void OnDateSelected() {
         if (this.DateSelected != null) {
            this.DateSelected(this, EventArgs.Empty);
         }
      }
      private void OnDateTimeFormatChanged() {
         this.MeasureFontRelatedSizes();
         this.MeasureBounds();
         this.Invalidate();
      }
      protected override void OnGotFocus(EventArgs e) {
         this.InvalidateDayBounds(this.value);
         base.OnGotFocus(e);
      }
      protected override void OnFontChanged(EventArgs e) {
         this.MeasureFontRelatedSizes();
         this.MeasureBounds();
         this.Invalidate();
         base.OnFontChanged(e);
      }
      protected override void OnLostFocus(EventArgs e) {
         this.InvalidateDayBounds(this.value);
         base.OnLostFocus(e);
      }
      protected override void OnMouseDown(MouseEventArgs e) {
         if ((e.Button & MouseButtons.Left) == MouseButtons.Left) {
            this.UpdateFocusedDayPosition(e.Location);
            HitTestInfo hti = this.HitTest(e.Location);
            if (hti.Type == HitTestType.Glyph1) {
               if (this.RightToLeft != RightToLeft.Yes) {
                  this.PrevMonth();
               }
               else {
                  this.NextMonth();
               }
            }
            else if (hti.Type == HitTestType.Glyph2) {
               if (this.RightToLeft != RightToLeft.Yes) {
                  this.NextMonth();
               }
               else {
                  this.PrevMonth();
               }
            }
            else if (hti.Type == HitTestType.Footer) {
               var selectedDate = DateTime.Now;
               this.ValueInternal = selectedDate;
               if (this.ValueInternal.Date.Equals(selectedDate.Date)) {
                  this.OnDateSelected();
               }
            }
         }
         base.OnMouseDown(e);
      }
      protected override void OnMouseLeave(EventArgs e) {
         this.FocusedDayPosition = DayPosition.Empty;
         base.OnMouseLeave(e);
      }
      protected override void OnMouseMove(MouseEventArgs e) {
         if ((e.Button & MouseButtons.Left) == MouseButtons.Left) {
            this.UpdateFocusedDayPosition(e.Location);
         }
         else {
            this.FocusedDayPosition = DayPosition.Empty;
         }
         base.OnMouseMove(e);
      }
      protected override void OnMouseUp(MouseEventArgs e) {
         if (!this.FocusedDayPosition.Equals(DayPosition.Empty)) {
            DateTime selectedDate = GetDayDateFromPosition(this.FocusedDayPosition.Column,
                                                        FocusedDayPosition.Row);
            this.ValueInternal = selectedDate;
            if (this.ValueInternal.Date.Equals(selectedDate.Date)) {
               this.OnDateSelected();
            }
         }
         base.OnMouseUp(e);
      }

      protected override void OnPaint(PaintEventArgs e) {
         if (e.ClipRectangle.Equals(glyph1Bounds)) {
            this.DrawGlypth1(e.Graphics);
         }
         else if (e.ClipRectangle.Equals(glyph2Bounds)) {
            this.DrawGlypth2(e.Graphics);
         }
         else if (e.ClipRectangle.Equals(titleTextBounds)) {
            this.DrawTitleText(e.Graphics);
         }
         else if (e.ClipRectangle.Equals(titleBounds)) {
            this.DrawTitle(e.Graphics);
         }
         else {
            DateTime dayDate;
            if (this.GetDayDateFromBounds(e.ClipRectangle, out dayDate)) {
               this.DrawDay(e.Graphics, dayDate, GetDayPosition(dayDate));
            }
            else {
               this.DrawTitle(e.Graphics);
               this.DrawBody(e.Graphics);
               this.DrawFooter(e.Graphics);
            }
         }
      }

      protected override void OnRightToLeftChanged(EventArgs e) {
         this.MeasureFontRelatedSizes();
         this.MeasureBounds();
         this.Invalidate();
         base.OnRightToLeftChanged(e);
      }
      private void OnValueChanged() {
         if (this.ValueChanged != null) {
            this.ValueChanged(this, EventArgs.Empty);
         }
      }
      protected override bool ProcessDialogKey(Keys keyData) {
         // Keys keyCode = keyData & Keys.KeyCode;
         switch (keyData) {
            case Keys.Down:
               this.ProcessDownKey();
               return true;
            //case Keys.Enter:
            //    this.ProcessEnterKey();
            //    return true;
            case Keys.Left:
               this.ProcessLeftKey();
               return true;
            case Keys.Right:
               this.ProcessRightKey();
               return true;
            case Keys.Up:
               this.ProcessUpKey();
               return true;
         }
         return base.ProcessDialogKey(keyData);
      }
      public void ProcessDownKey() {
         this.ValueInternal = this.Value.AddDays(DaysInWeek);
      }
      //private void ProcessEnterKey() {
      //    this.OnDateSelected();
      //}
      public void ProcessLeftKey() {
         if (this.RightToLeft != RightToLeft.Yes) {
            this.ValueInternal = this.Value.AddDays(-1);
         }
         else {
            this.ValueInternal = this.Value.AddDays(+1);
         }
      }
      public void ProcessRightKey() {
         if (this.RightToLeft != RightToLeft.Yes) {
            this.ValueInternal = this.Value.AddDays(+1);
         }
         else {
            this.ValueInternal = this.Value.AddDays(-1);
         }
      }
      public void ProcessUpKey() {
         this.ValueInternal = this.Value.AddDays(-DaysInWeek);
      }
      private void ThrowCalendarDateTimeIsNotSupportedException() {
         throw new ArgumentOutOfRangeException(
             string.Format("Calendar's supported date range is {0} to {1}",
             this.Calendar.MinSupportedDateTime.ToString(this.DateTimeFormat.ShortDatePattern,
                                                         this.DateTimeFormat),
             this.Calendar.MaxSupportedDateTime.ToString(this.DateTimeFormat.ShortDatePattern,
                                                         this.DateTimeFormat)));
      }
      private void UpdateFocusedDayPosition(Point location) {
         HitTestInfo hti = HitTest(location);
         if (hti.Type == HitTestType.Day) {
            DateTime hoveredDate = GetDayDateFromPosition(hti.ColumnIndex, hti.RowIndex);
            if (this.CanAssignValue(hoveredDate)/* && this.IsCurrentMonthsDate(hoveredDate)*/) {
               this.FocusedDayPosition = new DayPosition(hti.ColumnIndex, hti.RowIndex);
               return;
            }
         }
         this.FocusedDayPosition = DayPosition.Empty;
      }
      #endregion

      #region HitTest
      private struct DayPosition {
         private static readonly DayPosition empty = new DayPosition();
         private int column;
         private int row;
         public DayPosition(int column, int row) {
            this.column = column + 1;
            this.row = row + 1;
         }
         public int Column {
            get {
               return this.column - 1;
            }
            set {
               this.column = value + 1;
            }
         }
         public static DayPosition Empty {
            get {
               return empty;
            }
         }
         public int Row {
            get {
               return this.row - 1;
            }
            set {
               this.row = value + 1;
            }
         }
         public override bool Equals(object obj) {
            if (!(obj is DayPosition)) {
               return false;
            }
            DayPosition dayPosition = (DayPosition)obj;
            return dayPosition.column.Equals(this.column) && dayPosition.row.Equals(this.row);
         }
         public override int GetHashCode() {
            return column ^ row;
         }
      }
      private enum HitTestType {
         None,
         Footer,
         Glyph1,
         Glyph2,
         Day,
      }
      private class HitTestInfo {
         private HitTestType type;
         private int columnIndex;
         private int rowIndex;

         public HitTestInfo(HitTestType type) {
            this.type = type;
         }
         public HitTestInfo(HitTestType type, int columnIndex, int rowIndex) {
            this.columnIndex = columnIndex;
            this.rowIndex = rowIndex;
            this.type = type;
         }
         public int ColumnIndex {
            get {
               return this.columnIndex;
            }
         }
         public int RowIndex {
            get {
               return this.rowIndex;
            }
         }
         public HitTestType Type {
            get {
               return this.type;
            }
         }
      }
      #endregion
   }
}
