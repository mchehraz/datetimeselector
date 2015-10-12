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
namespace Atf.Core.Text {
   using System;
   using System.Collections.Generic;
   using System.Globalization;
   using System.Text;

   internal class DateTimeFormatter {
      #region Specifier info
      public enum ValueType {
         None = 0,
         Numeral,
         Items,
         Static,
         StaticItems,
         StringLiteral,
      }
      public enum SpecifierType {
         s,
         ss,
         m,
         mm,
         h,
         hh,
         H,
         HH,
         t,
         tt,
         d,
         dd,
         ddd,
         dddd,
         M,
         MM,
         MMM,
         MMMM,
         y,
         yy,
         yyy,
         yyyy,
         g,
         DateSeparator,
         TimeSeparator,
         StringLiteral,
      }
      public struct SpecifierInfo {
         public SpecifierInfo(SpecifierType type, ValueType valueType, string symbol,
                              int max, bool matchesExtraSymbols)
            : this() {
            MatchesExtraSymbols = matchesExtraSymbols;
            MaxLength = max;
            Symbol = symbol;
            Type = type;
            ValueType = valueType;
         }
         public bool MatchesExtraSymbols {
            get;
            set;
         }
         public int MaxLength {
            get;
            set;
         }
         public string Symbol {
            get;
            set;
         }
         public SpecifierType Type {
            get;
            set;
         }
         public ValueType ValueType {
            get;
            set;
         }
         public static SpecifierInfo CreateStringLiteralSpecifier(string text) {
            return new SpecifierInfo(SpecifierType.StringLiteral, ValueType.StringLiteral, text, -1, false);

         }
         public override string ToString() {
            if (this.Symbol != null) {
               return this.Symbol.ToString();
            }
            return base.ToString();
         }
      }
      #endregion

      #region Fields
      private static readonly char escapeCharacter = '\\';
      private static Dictionary<char, IList<SpecifierInfo>> specSymbolCache;
      private static readonly SpecifierInfo s_s = new SpecifierInfo(SpecifierType.s, ValueType.Numeral, "s", 2, false);
      private static readonly SpecifierInfo s_ss = new SpecifierInfo(SpecifierType.ss, ValueType.Numeral, "ss", 2, true);
      private static readonly SpecifierInfo s_m = new SpecifierInfo(SpecifierType.m, ValueType.Numeral, "m", 2, false);
      private static readonly SpecifierInfo s_mm = new SpecifierInfo(SpecifierType.mm, ValueType.Numeral, "mm", 2, true);
      private static readonly SpecifierInfo s_h = new SpecifierInfo(SpecifierType.h, ValueType.Numeral, "h", 2, false);
      private static readonly SpecifierInfo s_hh = new SpecifierInfo(SpecifierType.hh, ValueType.Numeral, "hh", 2, true);
      private static readonly SpecifierInfo s_H = new SpecifierInfo(SpecifierType.H, ValueType.Numeral, "H", 2, false);
      private static readonly SpecifierInfo s_HH = new SpecifierInfo(SpecifierType.HH, ValueType.Numeral, "HH", 2, true);
      private static readonly SpecifierInfo s_t = new SpecifierInfo(SpecifierType.t, ValueType.Items, "t", 1, false);
      private static readonly SpecifierInfo s_tt = new SpecifierInfo(SpecifierType.tt, ValueType.Items, "tt", 2, true);

      private static readonly SpecifierInfo s_d = new SpecifierInfo(SpecifierType.d, ValueType.Numeral, "d", 2, false);
      private static readonly SpecifierInfo s_dd = new SpecifierInfo(SpecifierType.dd, ValueType.Numeral, "dd", 2, false);
      private static readonly SpecifierInfo s_ddd = new SpecifierInfo(SpecifierType.ddd, ValueType.StaticItems, "ddd", 0, false);
      private static readonly SpecifierInfo s_dddd = new SpecifierInfo(SpecifierType.dddd, ValueType.StaticItems, "dddd", 0, true);
      private static readonly SpecifierInfo s_M = new SpecifierInfo(SpecifierType.M, ValueType.Numeral, "M", 2, false);
      private static readonly SpecifierInfo s_MM = new SpecifierInfo(SpecifierType.MM, ValueType.Numeral, "MM", 2, false);
      private static readonly SpecifierInfo s_MMM = new SpecifierInfo(SpecifierType.MMM, ValueType.Items, "MMM", 0, false);
      private static readonly SpecifierInfo s_MMMM = new SpecifierInfo(SpecifierType.MMMM, ValueType.Items, "MMMM", 0, true);
      private static readonly SpecifierInfo s_y = new SpecifierInfo(SpecifierType.y, ValueType.Numeral, "y", 2, false);
      private static readonly SpecifierInfo s_yy = new SpecifierInfo(SpecifierType.yy, ValueType.Numeral, "yy", 2, false);
      private static readonly SpecifierInfo s_yyy = new SpecifierInfo(SpecifierType.yyy, ValueType.Numeral, "yyy", 4, false);
      private static readonly SpecifierInfo s_yyyy = new SpecifierInfo(SpecifierType.yyyy, ValueType.Numeral, "yyyy", 4, true);
      private static readonly SpecifierInfo s_g = new SpecifierInfo(SpecifierType.g, ValueType.Static, "g", -1, true);

      private static readonly SpecifierInfo s_DateSeparator = new SpecifierInfo(SpecifierType.DateSeparator, ValueType.Static,
                                                                                "/", -1, false);
      private static readonly SpecifierInfo s_TimeSeparator = new SpecifierInfo(SpecifierType.TimeSeparator, ValueType.Static,
                                                                                ":", -1, false);
      private static readonly SpecifierInfo[] Specifiers = new SpecifierInfo[] {  
            s_s,
            s_ss,
            s_m,
            s_mm,
            s_h,
            s_hh,
            s_H,
            s_HH,
            s_t,
            s_tt,
            s_d,
            s_dd,
            s_ddd,
            s_dddd,
            s_M,
            s_MM,
            s_MMM,
            s_MMMM,
            s_y,
            s_yy,
            s_yyy,
            s_yyyy,
            s_g,
            s_DateSeparator,
            s_TimeSeparator,
        };
      public event EventHandler ValueChanged;
      private DateTimeFormatInfo dateTimeFormat;
      private bool am = true;
      private int? day;
      private int? hour;
      private int? minute;
      private int? month;
      private int? second;
      private int? year;
      private DateTime? value;


      #endregion

      #region Constructor
      public DateTimeFormatter(DateTimeFormatInfo dateTimeFormat) {
         if (dateTimeFormat == null) {
            throw new ArgumentNullException("dateTimeFormat");
         }
         this.dateTimeFormat = dateTimeFormat;
      }
      public DateTimeFormatter(DateTimeFormatInfo dateTimeFormat, DateTime? value) {
         if (dateTimeFormat == null) {
            throw new ArgumentNullException("dateTimeFormat");
         }
         this.dateTimeFormat = dateTimeFormat;
         this.Value = value;
      }
      #endregion

      #region Properties
      public DateTime? Time {
         get {
            if (this.hour.HasValue && this.minute.HasValue && this.second.HasValue) {
               DateTime minDateTime = this.Calendar.MinSupportedDateTime;

               return new DateTime(minDateTime.Year, minDateTime.Month, minDateTime.Day,
                                            this.hour.Value, this.minute.Value, this.second.Value);
            }
            return null;
         }
      }
      public DateTime? Value {
         get {
            return this.value;
         }
         set {
            if (this.value != value) {
               this.value = value;
               this.SetDateTimeComponents();
            }
         }
      }
      #endregion

      #region Methods      
      public void ClearComponentValue(SpecifierInfo specifier) {
         switch (specifier.Type) {
            case SpecifierType.s:
            case SpecifierType.ss:
               if (this.second != null) {
                  this.second = null;
                  this.UpdateDateTimeComponets();
               }
               break;
            case SpecifierType.m:
            case SpecifierType.mm:
               if (this.minute != null) {
                  this.minute = null;
                  this.UpdateDateTimeComponets();
               }
               break;
            case SpecifierType.h:
            case SpecifierType.hh:
               if (this.hour != null) {
                  this.hour = null;
                  this.UpdateDateTimeComponets();
               }
               break;
            case SpecifierType.d:
            case SpecifierType.dd:
               if (this.day != null) {
                  this.day = null;
                  this.UpdateDateTimeComponets();
               }
               break;
            case SpecifierType.M:
            case SpecifierType.MM:
               if (this.month != null) {
                  this.month = null;
                  this.UpdateDateTimeComponets();
               }
               break;
            case SpecifierType.y:
            case SpecifierType.yy:
            case SpecifierType.yyy:
            case SpecifierType.yyyy:
               if (this.year != null) {
                  this.year = null;
                  this.UpdateDateTimeComponets();
               }
               break;
         }
      }
      public void ClearInvalids() {
         if (this.year == null || this.month == null || this.day == null) {
            this.year = this.month = this.day = null;
         }
      }
      public bool DecreaseComponentValue(SpecifierInfo specifier) {
         switch (specifier.Type) {
            case SpecifierType.s:
            case SpecifierType.ss:
               if (this.second.HasValue && this.second.Value > 0) {
                  this.second--;
               }
               else {
                  this.second = 59;
               }
               return true;
            case SpecifierType.m:
            case SpecifierType.mm:
               if (this.minute.HasValue && this.minute.Value > 0) {
                  this.minute--;
               }
               else {
                  this.minute = 59;
               }
               return true;
            case SpecifierType.h:
            case SpecifierType.hh:
            case SpecifierType.H:
            case SpecifierType.HH:
               if (this.hour.HasValue && this.hour.Value > 0) {
                  this.hour--;
               }
               else {
                  this.hour = 23;
               }
               this.am = hour < 12;
               return true;
            case SpecifierType.d:
            case SpecifierType.dd:
               if (this.day.HasValue && this.day.Value > 1) {
                  this.day--;
               }
               else {
                  if (this.year.HasValue && this.month.HasValue) {
                     int daysInMonth = this.Calendar.GetDaysInMonth(this.year.Value, this.month.Value);
                     this.day = daysInMonth;
                     return true;
                  }
                  this.day = 31; /********/
               }
               return true;

            case SpecifierType.M:
            case SpecifierType.MM:
            case SpecifierType.MMM:
            case SpecifierType.MMMM:
               if (this.month.HasValue && this.month.Value > 1) {
                  this.month--;
               }
               else {
                  if (this.year.HasValue) {
                     int monthsInYear = this.Calendar.GetMonthsInYear(this.year.Value);
                     this.month = monthsInYear;
                     return true;
                  }
                  this.month = 12; /**********/
               }
               return true;

            case SpecifierType.y:
            case SpecifierType.yy:
            case SpecifierType.yyy:
            case SpecifierType.yyyy:
               int minYear = this.Calendar.GetYear(this.Calendar.MinSupportedDateTime);
               if (this.year.HasValue && this.year.Value > minYear) {
                  this.year--;
               }
               else {
                  this.year = this.Calendar.GetYear(DateTime.Now);
               }
               return true;
            case SpecifierType.t:
            case SpecifierType.tt:
               if (this.hour.HasValue) {
                  if (this.hour < 12) {
                     this.hour += 12;
                     this.am = false;
                  }
                  else {
                     this.hour -= 12;
                     this.am = true;
                  }
                  return true;
               }
               return false;

         }
         return false;
      }
      public bool EditComponentValue(SpecifierInfo specifier, int componentValue, bool commit, int length) {
         switch (specifier.Type) {
            case SpecifierType.s:
            case SpecifierType.ss:
               if (componentValue < 0) {
                  return false;
               }
               if (componentValue < 60) {
                  if (commit) {
                     this.second = componentValue;
                     this.UpdateDateTimeComponets();
                  }
                  return true;
               }
               return false;

            case SpecifierType.m:
            case SpecifierType.mm:
               if (componentValue < 0) {
                  return false;
               }
               if (componentValue < 60) {
                  if (commit) {
                     this.minute = componentValue;
                     this.UpdateDateTimeComponets();
                  }
                  return true;
               }
               return false;
            case SpecifierType.h:
            case SpecifierType.hh:
               if (componentValue < 0) {
                  return false;
               }
               if (!commit) {
                  if (componentValue < 24) {
                     return true;
                  }
                  return false;
               }
               if (componentValue == 0) {
                  this.hour = 0;
               }
               else if (componentValue < 12) {
                  this.hour = this.am ? componentValue : componentValue + 12;
               }
               else if (componentValue < 24) {
                  this.hour = componentValue;
               }
               else {
                  return false;
               }
               this.UpdateDateTimeComponets();
               return true;
            case SpecifierType.H:
            case SpecifierType.HH:
               if (componentValue < 0) {
                  return false;
               }
               if (componentValue < 24) {
                  if (commit) {
                     this.hour = componentValue;
                     this.UpdateDateTimeComponets();
                  }
                  return true;
               }
               return false;
            case SpecifierType.d:
            case SpecifierType.dd:
               if (componentValue <= 0) {
                  return false;
               }
               if (this.year.HasValue && this.month.HasValue) {
                  int daysInMonth = this.Calendar.GetDaysInMonth(this.year.Value, this.month.Value);
                  if (componentValue > daysInMonth) {
                     return false;
                  }
               }
               if (commit) {
                  this.day = componentValue;
                  this.UpdateDateTimeComponets();
               }
               return true;
            case SpecifierType.M:
            case SpecifierType.MM:
               if (componentValue <= 0) {
                  return false;
               }
               if (this.year.HasValue) {
                  int monthsInYear = this.Calendar.GetMonthsInYear(this.year.Value);
                  if (componentValue > monthsInYear) {
                     return false;
                  }
               }
               if (commit) {
                  this.month = componentValue;
                  this.UpdateDateTimeComponets();
               }
               return true;
            case SpecifierType.y:
            case SpecifierType.yy:
               if (componentValue <= 0) {
                  return false;
               }
               if (componentValue <= 99) {
                  if (commit) {
                     this.year = this.Calendar.ToFourDigitYear(componentValue);
                     this.UpdateDateTimeComponets();
                  }
                  return true;
               }
               return false;
            case SpecifierType.yyy:
            case SpecifierType.yyyy:
               if (componentValue <= 0) {
                  return false;
               }
               if (length <= 2 && componentValue <= 99) {
                  if (commit) {
                     this.year = this.Calendar.ToFourDigitYear(componentValue);
                     this.UpdateDateTimeComponets();
                  }
                  return true;
               }
               else {
                  int maxYear = this.Calendar.GetYear(this.Calendar.MaxSupportedDateTime);
                  int minYear = this.Calendar.GetYear(this.Calendar.MinSupportedDateTime);
                  if (componentValue >= minYear && componentValue <= maxYear) {
                     if (commit) {
                        this.year = componentValue;
                        this.UpdateDateTimeComponets();
                     }
                     return true;
                  }
                  else if (componentValue > maxYear) {
                     if (commit) {
                        this.UpdateDateTimeComponets();
                        return true;
                     }
                     return false;
                  }
                  else if (componentValue < minYear) {
                     if (commit) {
                        this.UpdateDateTimeComponets();
                     }
                     return true;
                  }
               }
               return false;

         }
         return false;
      }
      public static string Format(string format, DateTimeFormatInfo dateTimeFormat,
                                                Calendar calendar, DateTime? value) {
         SpecifierInfo[] specifiers = GetSpecifiers(format);
         DateTimeFormatter formatter = new DateTimeFormatter(dateTimeFormat, value);
         StringBuilder sb = new StringBuilder();
         foreach (var specifier in specifiers) {
            sb.Append(formatter.GetDisplayText(specifier));
         }
         return sb.ToString();
      }
      public string GetDisplayText(SpecifierInfo specifier) {
         if (specifier.ValueType == ValueType.StringLiteral) {
            return specifier.Symbol;
         }
         string displayText = null;
         switch (specifier.Type) {
            case SpecifierType.s:
               if (this.second.HasValue) {
                  displayText = this.second.Value.ToString();
               }
               break;
            case SpecifierType.ss:
               if (this.second.HasValue) {
                  displayText = this.second.Value.ToString("00");
               }
               break;
            case SpecifierType.m:
               if (this.minute.HasValue) {
                  displayText = this.minute.Value.ToString();
               }
               break;
            case SpecifierType.mm:
               if (this.minute.HasValue) {
                  displayText = this.minute.Value.ToString("00");
               }
               break;
            case SpecifierType.h:
               if (this.hour.HasValue) {
                  int hour = this.hour.Value;
                  if (hour > 12) {
                     hour -= 12;
                  }
                  displayText = hour.ToString();
               }
               break;
            case SpecifierType.hh:
               if (this.hour.HasValue) {
                  int hour = this.hour.Value;
                  if (hour > 12) {
                     hour -= 12;
                  }
                  displayText = hour.ToString("00");
               }
               break;
            case SpecifierType.H:
               if (this.hour.HasValue) {
                  displayText = this.hour.ToString();
               }
               break;
            case SpecifierType.HH:
               if (this.hour.HasValue) {
                  displayText = this.hour.Value.ToString("00");
               }
               break;
            case SpecifierType.t:
               if (this.hour.HasValue) {
                  int hour = this.hour.Value;
                  string designator;
                  if (hour < 12) {
                     designator = this.DateTimeFormat.AMDesignator;
                  }
                  else {
                     designator = this.DateTimeFormat.PMDesignator;
                  }
                  if (!string.IsNullOrEmpty(designator)) {
                     displayText = designator.Substring(0, 1);
                  }
                  else {
                     displayText = string.Empty;
                  }
               }
               break;
            case SpecifierType.tt:
               if (this.hour.HasValue) {
                  int hour = this.hour.Value;
                  if (hour < 12) {
                     displayText = this.DateTimeFormat.AMDesignator;
                  }
                  else {
                     displayText = this.DateTimeFormat.PMDesignator;
                  }
               }
               break;
            case SpecifierType.d:
               if (this.day.HasValue) {
                  displayText = this.day.ToString();
               }
               break;
            case SpecifierType.dd:
               if (this.day.HasValue) {
                  displayText = this.day.Value.ToString("00");
               }
               break;
            case SpecifierType.ddd:
               if (this.value.HasValue) {
                  var dayOfWeek = this.Calendar.GetDayOfWeek(this.value.Value);
                  displayText = this.DateTimeFormat.GetAbbreviatedDayName(dayOfWeek);
               }
               break;
            case SpecifierType.dddd:
               if (this.value.HasValue) {
                  var dayOfWeek = this.Calendar.GetDayOfWeek(this.value.Value);
                  displayText = this.DateTimeFormat.GetDayName(dayOfWeek);
               }
               break;
            case SpecifierType.M:
               if (this.month.HasValue) {
                  displayText = this.month.ToString();
               }
               break;
            case SpecifierType.MM:
               if (this.month.HasValue) {
                  displayText = this.month.Value.ToString("00");
               }
               break;
            case SpecifierType.MMM:
               if (this.month.HasValue) {
                  displayText = this.DateTimeFormat.GetAbbreviatedMonthName(this.month.Value);
               }
               break;
            case SpecifierType.MMMM:
               if (this.month.HasValue) {
                  displayText = this.DateTimeFormat.GetMonthName(this.month.Value);
               }
               break;
            case SpecifierType.y:
               if (this.year.HasValue) {
                  displayText = (this.year.Value % 100).ToString();
               }
               break;
            case SpecifierType.yy:
               if (this.year.HasValue) {
                  displayText = (this.year.Value % 100).ToString("00");
               }
               break;
            case SpecifierType.yyy:
               if (this.year.HasValue) {
                  displayText = this.year.Value.ToString("000");
               }
               break;
            case SpecifierType.yyyy:
               if (this.year.HasValue) {
                  displayText = this.year.Value.ToString("0000");
               }
               break;
            case SpecifierType.g:
            case SpecifierType.DateSeparator:
            case SpecifierType.TimeSeparator:
               return GetStaticDisplayText(specifier, this.DateTimeFormat, this.Calendar, this.Value);
            default:
               throw new ArgumentOutOfRangeException("specifier.Type");
         }
         return displayText;
      }
      public static string GetStaticDisplayText(SpecifierInfo specifier, DateTimeFormatInfo dateTimeFormat,
                                                Calendar calendar, DateTime? value) {
         if (specifier.ValueType != ValueType.Static) {
            throw new ArgumentException("specifier");
         }
         switch (specifier.Type) {
            case SpecifierType.g:
               if (value.HasValue) {
                  int era = calendar.GetEra(value.Value);
                  return dateTimeFormat.GetEraName(era);
               }
               return string.Empty;
            case SpecifierType.DateSeparator:
               return dateTimeFormat.DateSeparator;
            case SpecifierType.TimeSeparator:
               return dateTimeFormat.TimeSeparator;
            default:
               throw new ArgumentOutOfRangeException("specifier.Type");
         }
      }
      public string[] GetItems(SpecifierType specifierType) {
         return GetItems(specifierType, this.DateTimeFormat);
      }
      public static string[] GetItems(SpecifierType specifierType, DateTimeFormatInfo dateTimeFormat) {
         switch (specifierType) {
            case SpecifierType.ddd:
               return dateTimeFormat.AbbreviatedDayNames;
            case SpecifierType.dddd:
               return dateTimeFormat.DayNames;
            case SpecifierType.MMM:
               return dateTimeFormat.AbbreviatedMonthNames;
            case SpecifierType.MMMM:
               return dateTimeFormat.MonthNames;
            case SpecifierType.t:
               string amDesignator = dateTimeFormat.AMDesignator;
               string pmDesignator = dateTimeFormat.PMDesignator;
               string[] items = new string[] {
                        !string.IsNullOrEmpty(amDesignator) ? amDesignator.Substring(0, 1) : string.Empty,
                        !string.IsNullOrEmpty(pmDesignator) ? pmDesignator.Substring(0, 1) : string.Empty,

                    };
               return items;
            case SpecifierType.tt:
               return new string[] {
                        dateTimeFormat.AMDesignator,
                        dateTimeFormat.PMDesignator,
                    };
            default:
               throw new ArgumentOutOfRangeException("specifierType");
         }
      }
      public static SpecifierInfo[] GetSpecifiers(string format) {
         if (format == null) {
            throw new ArgumentNullException("format");
         }
         StringBuilder stringLiteral = new StringBuilder();
         var matchedSpecifiers = new List<SpecifierInfo>();
         int length = format.Length;
         for (int index = 0; index < length; ) {
            char symbol = format[index];
            if (symbol.Equals(escapeCharacter)) {
               if (index < length - 1) {
                  index++; // Move to the next character
                  stringLiteral.Append(format[index]);
                  index++;
               }
               continue;
            }
            var specsOfSymbol = GetSpecsOfSymbol(symbol);
            int lastMatchedRepeatCount = 1;
            SpecifierInfo matchedSpecifier = new SpecifierInfo();
            bool specifierFound = false;
            int repeatCount = 0;
            if (specsOfSymbol != null) {
               do {
                  repeatCount++;
                  foreach (var specifier in specsOfSymbol) {
                     int specSymbolCount = specifier.Symbol.Length;
                     if (specSymbolCount == repeatCount || (specSymbolCount < repeatCount &&
                         specifier.MatchesExtraSymbols)) {
                        matchedSpecifier = specifier;
                        lastMatchedRepeatCount = repeatCount;
                        specifierFound = true;

                     }
                  }

               } while ((index + repeatCount) < length && format[index + repeatCount].Equals(symbol));
            }
            else {
               repeatCount = 1;
            }
            if (specifierFound) {
               if (stringLiteral.Length > 0) {
                  var slSpecifier = SpecifierInfo.CreateStringLiteralSpecifier(stringLiteral.ToString());
                  matchedSpecifiers.Add(slSpecifier);
                  stringLiteral = new StringBuilder();
               }
               matchedSpecifiers.Add(matchedSpecifier);
            }
            else {
               stringLiteral.Append(new string(symbol, repeatCount));
            }
            index += lastMatchedRepeatCount;
         }
         if (stringLiteral.Length > 0) {
            var slSpecifier = SpecifierInfo.CreateStringLiteralSpecifier(stringLiteral.ToString());
            matchedSpecifiers.Add(slSpecifier);
         }
         return matchedSpecifiers.ToArray();
      }
      public bool IncreaseComponentValue(SpecifierInfo specifier) {
         switch (specifier.Type) {
            case SpecifierType.s:
            case SpecifierType.ss:
               if (this.second.HasValue && this.second.Value < 59) {
                  this.second++;
               }
               else {
                  this.second = 1;
               }
               return true;
            case SpecifierType.m:
            case SpecifierType.mm:
               if (this.minute.HasValue && this.minute.Value < 59) {
                  this.minute++;
               }
               else {
                  this.minute = 1;
               }
               return true;
            case SpecifierType.h:
            case SpecifierType.hh:
            case SpecifierType.H:
            case SpecifierType.HH:
               if (this.hour.HasValue && this.hour.Value < 23) {
                  this.hour++;
               }
               else {
                  this.hour = 0;

               }
               this.am = hour < 12;
               return true;
            case SpecifierType.d:
            case SpecifierType.dd:
               if (this.day.HasValue) {
                  if (this.year.HasValue && this.month.HasValue) {
                     int daysInMonth = this.Calendar.GetDaysInMonth(this.year.Value, this.month.Value);
                     if (this.day.Value < daysInMonth) {
                        this.day++;
                        return true;
                     }
                  }
               }
               this.day = 1;
               return true;

            case SpecifierType.M:
            case SpecifierType.MM:
            case SpecifierType.MMM:
            case SpecifierType.MMMM:
               if (this.month.HasValue) {
                  if (this.year.HasValue) {
                     int monthsInYear = this.Calendar.GetMonthsInYear(this.year.Value);
                     if (this.month.Value < monthsInYear) {
                        this.month++;
                        return true;
                     }
                  }
                  else {
                     if (this.month.Value < 12) { /**********/
                        this.month++;
                        return true;
                     }
                  }
               }
               this.month = 1;
               return true;

            case SpecifierType.y:
            case SpecifierType.yy:
            case SpecifierType.yyy:
            case SpecifierType.yyyy:
               if (this.year.HasValue) {
                  int maxYear = this.Calendar.GetYear(this.Calendar.MaxSupportedDateTime);
                  if (this.year.Value < maxYear) {
                     this.year++;
                     return true;
                  }
                  else {
                     return false;
                  }
               }
               this.year = this.Calendar.GetYear(DateTime.Now);
               return true;
            case SpecifierType.t:
            case SpecifierType.tt:
               if (this.hour.HasValue) {
                  if (this.hour < 12) {
                     this.hour += 12;
                     this.am = false;
                  }
                  else {
                     this.hour -= 12;
                     this.am = true;
                  }
                  return true;
               }
               return false;
         }
         return false;
      }
      protected static IList<SpecifierInfo> GetSpecsOfSymbol(char symbol) {
         IList<SpecifierInfo> specifiers;
         if (specSymbolCache == null) {
            specSymbolCache = new Dictionary<char, IList<SpecifierInfo>>();
         }
         else {
            if (specSymbolCache.TryGetValue(symbol, out specifiers)) {
               return specifiers;
            }
         }
         specifiers = new List<SpecifierInfo>();
         foreach (var specifier in Specifiers) {
            if (specifier.ValueType != ValueType.StringLiteral &&
                specifier.Symbol[0].Equals(symbol)) {
               specifiers.Add(specifier);
            }
         }
         if (specifiers.Count > 0) {
            specSymbolCache.Add(symbol, specifiers);
            return specifiers;
         }
         return null;
      }
      private void SetDateTimeComponents() {
         if (this.value.HasValue) {
            DateTime _value = this.value.Value;
            Calendar calendar = this.Calendar;
            this.day = calendar.GetDayOfMonth(_value);
            this.hour = calendar.GetHour(_value);
            this.minute = calendar.GetMinute(_value);
            this.month = calendar.GetMonth(_value);
            this.second = calendar.GetSecond(_value);
            this.year = calendar.GetYear(_value);
            this.am = this.hour < 12;
         }
         else {
            this.day =
            this.hour =
            this.minute =
            this.month =
            this.second =
            this.year = null;
            this.am = true;
         }
      }
      private static bool ShouldCommit(int? dateTimeValue, int? value) {
         if (dateTimeValue.HasValue) {
            return !value.HasValue || !dateTimeValue.Value.Equals(value.Value);
         }
         return value.HasValue;
      }
      public bool ShouldCommit(SpecifierInfo specifier) {
         if (specifier.ValueType == ValueType.StringLiteral) {
            return false;
         }
         int? dateTimeValue = null;
         switch (specifier.Type) {
            case SpecifierType.s:
               return ShouldCommit(this.value.HasValue ? (int?)this.value.Value.Second : null, this.second);
            case SpecifierType.m:
            case SpecifierType.mm:
               return ShouldCommit(this.value.HasValue ? (int?)this.value.Value.Minute : null, this.minute);               
            case SpecifierType.h:
            case SpecifierType.hh:              
            case SpecifierType.H:              
            case SpecifierType.HH:
            case SpecifierType.t: // Correct??
            case SpecifierType.tt: // Correct??
               return ShouldCommit(this.value.HasValue ? (int?)this.value.Value.Hour : null, this.hour); 
            case SpecifierType.d:
            case SpecifierType.dd:
            case SpecifierType.ddd: // Correct??
            case SpecifierType.dddd: // Correct??
               if (this.value.HasValue) {
                  dateTimeValue = this.Calendar.GetDayOfMonth(this.value.Value);
               }
               return ShouldCommit(dateTimeValue, this.day);   
            case SpecifierType.M:
            case SpecifierType.MM:
            case SpecifierType.MMM:
            case SpecifierType.MMMM:
               if (this.value.HasValue) {
                  dateTimeValue = this.Calendar.GetMonth(this.value.Value);
               }
               return ShouldCommit(dateTimeValue, this.month);   
            case SpecifierType.y:               
            case SpecifierType.yy:
            case SpecifierType.yyy:
            case SpecifierType.yyyy:
               if (this.value.HasValue) {
                  dateTimeValue = this.Calendar.GetYear(this.value.Value);
               }
               return ShouldCommit(dateTimeValue, this.year);
            case SpecifierType.g:
            case SpecifierType.DateSeparator:
            case SpecifierType.TimeSeparator:
               return false;
            default:
               throw new ArgumentOutOfRangeException("specifier.Type");
         }
      }
      private void UpdateDateTimeComponets() {
         int hour = this.hour.HasValue ? this.hour.Value : 0;
         int minute = this.minute.HasValue ? this.minute.Value : 0;
         int second = this.second.HasValue ? this.second.Value : 0;
         DateTime? newValue = null;
         if (this.year.HasValue) {
            int year = this.year.Value;
            int maxYear = this.Calendar.GetYear(this.Calendar.MaxSupportedDateTime);
            int minYear = this.Calendar.GetYear(this.Calendar.MinSupportedDateTime);
            if (year > maxYear) {
               this.year = year = maxYear;
            }
            else if (year < minYear) {
               this.year = year = minYear;
            }
            if (this.month.HasValue) {
               int month = this.month.Value;
               int monthsInYear = this.Calendar.GetMonthsInYear(this.year.Value);
               if (monthsInYear < month) {
                  this.month = month = monthsInYear;
               }
               if (this.day.HasValue) {
                  int day = this.day.Value;
                  int daysInMonth = this.Calendar.GetDaysInMonth(this.year.Value, this.month.Value);
                  if (daysInMonth < day) {
                     this.day = day = daysInMonth;
                  }
                  DateTimeKind kind = DateTimeKind.Unspecified;
                  if (this.value.HasValue) {
                     kind = this.value.Value.Kind;
                  }
                  newValue = new DateTime(year, month, day, hour, minute, second, 0, this.Calendar, kind);
               }
            }
         }
         this.am = this.hour < 12;
         if (!newValue.HasValue) {
            // If one of the time componets are set, set null ones to zero
            // So 11:null:null becomes 11:00:00 and so one
            if (this.hour.HasValue || this.minute.HasValue || this.second.HasValue) {
               this.hour = hour;
               this.minute = minute;
               this.second = second;
            }
         }
         if (newValue != this.value) {
            this.value = newValue;
            if (this.ValueChanged != null) {
               this.ValueChanged(this, EventArgs.Empty);
            }
         }
      }
      #endregion

      #region Properties
      protected Calendar Calendar {
         get {
            return this.dateTimeFormat.Calendar; ;
         }
      }
      public DateTimeFormatInfo DateTimeFormat {
         get {
            return this.dateTimeFormat;
         }
      }
      #endregion
   }
}
