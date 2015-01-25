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
   using System.Globalization;
   using System.Reflection;
   class PersianDateTimeFormat {
      private static readonly string amDesignator = "ق.ظ";
      private static readonly string[] abbrevDayNames = new string[] { "ی", "د", "س", "چ", "پ", "ج", "ش" };
      private static readonly string[] abbrevMonthNames = new string[] { "فرو", "ارد", "خرد", "تیر", "مرد", "شهر", "مهر",
                                                                         "آبان", "آذر", "دی", "بهم", "اسف", ""};
      private static readonly string[] dayNames = new string[] { "یکشنبه", "دوشنبه", "سه‌شنبه",
                                                                 "چهارشنبه", "پنجشنبه", "جمعه", "شنبه"};
      private static readonly int defaultTwoDigitMaxYear = 1410;
      private static readonly DayOfWeek firstDayOfWeek = DayOfWeek.Saturday;
      private static readonly string longDatePattern = "dddd، dd MMMM yyyy";
      private static readonly string[] monthNames = new string[] { "فرودین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور",
                                                                   "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند",""};
      private static readonly string pmDesignator = "ب.ظ";
      private static readonly string shortDatePattern = "yyyy/MM/dd";
      private static readonly string yearMonthPattern = "MMMM yyyy";
      public static DateTimeFormatInfo GetPersianDateTimeFormat() {
         DateTimeFormatInfo dateTimeFormat;
         dateTimeFormat = (DateTimeFormatInfo)CultureInfo.CurrentCulture.DateTimeFormat.Clone();
         dateTimeFormat.AbbreviatedDayNames = abbrevDayNames;
         dateTimeFormat.AbbreviatedMonthNames = abbrevMonthNames;
         dateTimeFormat.AMDesignator = amDesignator;
         dateTimeFormat.DayNames = dayNames;
         dateTimeFormat.FirstDayOfWeek = firstDayOfWeek;
         dateTimeFormat.LongDatePattern = longDatePattern;
         dateTimeFormat.MonthNames = monthNames;
         dateTimeFormat.PMDesignator = pmDesignator;
         dateTimeFormat.ShortDatePattern = shortDatePattern;
         dateTimeFormat.YearMonthPattern = yearMonthPattern;
         Calendar calendar = new PersianCalendar();
         calendar.TwoDigitYearMax = defaultTwoDigitMaxYear;
         SetDateTimeFormatCalendar(dateTimeFormat, calendar);
         return dateTimeFormat;
      }
      private static void SetDateTimeFormatCalendar(DateTimeFormatInfo dateTimeFormat, Calendar calendar) {
         try {
            dateTimeFormat.Calendar = calendar;
         }
         catch (ArgumentException) {
            FieldInfo fi = typeof(DateTimeFormatInfo).GetField("calendar", BindingFlags.NonPublic |
                                                                           BindingFlags.Instance);
            fi.SetValue(dateTimeFormat, calendar);
         }
      }
   }
}
