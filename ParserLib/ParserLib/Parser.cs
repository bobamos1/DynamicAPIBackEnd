using System;
using System.Collections;
using System.Drawing;
using System.Globalization;
using System.Linq;
namespace ParserLib
{
    public static class Parser
    {
        public static string defaultImgPath { get; set; }
        #region validator
        public static bool IsNumeric(object obj)
        {
            return Double.TryParse(Convert.ToString(obj), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out _);
        }
        public static bool isValid(object item)
        {
            if (!IsNullOrEmpty(item))
                if (item is ICollection collection && collection.Count == 0)
                    return false;
                else if (IsEmptyValueType(item))
                    return false;
                else
                    return true;
            return false;
        }
        public static bool IsEmptyValueType(object item)
        {
            var type = item.GetType();
            if (IsNumeric(item) && Convert.ToDouble(item) == 0)
                return false;
            return type.IsValueType ? item.Equals(Activator.CreateInstance(type)) : false;
        }
        public static bool ValidString(string str)
        {
            return (str == null || string.IsNullOrWhiteSpace(str));
        }
        private static bool IsNullOrEmpty(object obj)
        {
            return obj == null || obj == DBNull.Value || string.IsNullOrWhiteSpace(obj.ToString());
        }
        #endregion
        #region converter
        public static string toSqlString(object item)
        {
            if (item == null)
                return "NULL";
            else if (item is string)
                return "'" + item.ToString().Replace("'", "''") + "'";
            else if (IsNumeric(item))
                return item.ToString();
            else if (item is bool)
                return ((bool)item) ? "1" : "0";
            else if (item is DateTime)
                return "'" + ((DateTime)item).ToString("yyyy-MM-dd HH:mm:ss.fff") + "'";
            else if (item is IEnumerable)
                return string.Join(",", ((IEnumerable)item).Cast<object>().Select(x => toSqlString(x)));
            else
                throw new NotSupportedException($"Type {item.GetType().Name} is not supported.");
        }


        public static string toString(object obj, string defVal = default(string))
        {
            if (!IsNullOrEmpty(obj))
                return obj.ToString();
            return defVal == default(string) ? "" : defVal;
        }

        public static bool toBool(object obj, bool defVal = default(bool))
        {
            if (obj is bool boolean)
                return boolean;
            if (!IsNullOrEmpty(obj))
                if (obj.GetType().FullName == "System.Boolean")
                    return Convert.ToBoolean(obj);
                else if (IsNumeric(obj))
                    return Convert.ToInt16(obj) > 0;
                else
                {
                    string[] strings = !defVal ? new string[] { "true", "yes", "1", "on" } : new string[] { "false", "no", "0", "off" };
                    string input = obj.ToString().ToLower();
                    foreach (string s in strings)
                        if (input == s)
                            return true;
                }
            return defVal == default(bool) ? false : defVal;
        }
        /*
        public static Image getImage(string path, Image defVal = default(Image), string defaultPath = "")
        {
            if (path == "")
                return defVal;
            try
            {
                if (defaultPath == null)
                    defaultPath = defaultImgPath;
                return Image.FromFile(defaultPath + path);
            }
            catch { }
            return defVal;
        }
        */
        public static T toNumber<T>(object obj, T defVal = default(T))
        {
            defVal = defVal.Equals(default(T)) ? (T)Convert.ChangeType(0, typeof(T)) : defVal;
            if (!IsNullOrEmpty(obj))
                if (obj is bool)
                    return (T)(object)((bool)obj ? 1 : 0);
                else if (obj.GetType().BaseType == typeof(Enum))
                    return (T)Convert.ChangeType(obj, Enum.GetUnderlyingType(obj.GetType()));
                else
                    try
                    {
                        return (T)Convert.ChangeType(obj, typeof(T), CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                        return defVal;
                    }
            return defVal;
        }

        public static DateTime? toDateTime(object value, DateTime? defVal = null)
        {
            if (value == null || value == DBNull.Value)
                return defVal ?? DateTime.MinValue;
            if (value is DateTime dateTime)
                return dateTime;
            if (DateTime.TryParse(value.ToString(), out dateTime))
                return dateTime;
            if (DateTimeOffset.TryParse(value.ToString(), out var offset))
                return offset.DateTime;
            if (long.TryParse(value.ToString(), out var ticks))
                return new DateTime(ticks);
            if (value is string stringValue)
            {
                var formats = new[] {
                "yyyy-MM-dd HH:mm:ss.fffffff",
                "yyyy-MM-dd HH:mm:ss.fff",
                "yyyy-MM-dd HH:mm:ss",
                "yyyy-MM-dd HH:mm",
                "yyyy-MM-dd",
                "yyyy-MM-ddTHH:mm:ss.fffffffK",
                "yyyy-MM-ddTHH:mm:ss.fffK",
                "yyyy-MM-ddTHH:mm:ssK",
                "yyyy-MM-ddTHH:mm:ss.fffffff",
                "yyyy-MM-ddTHH:mm:ss.fff",
                "yyyy-MM-ddTHH:mm:ss",
                "yyyy-MM-ddTHH:mmK",
                "yyyy-MM-ddTHH:mm",
                "MM/dd/yyyy HH:mm:ss",
                "MM/dd/yyyy HH:mm",
                "MM/dd/yyyy",
                "dd/MM/yyyy HH:mm:ss",
                "dd/MM/yyyy HH:mm",
                "dd/MM/yyyy",
                "MMMM dd, yyyy",
                "dddd, MMMM dd, yyyy"
            };

                foreach (var format in formats)
                    if (DateTime.TryParseExact(stringValue, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                        return dateTime;
            }
            return defVal ?? DateTime.MinValue;
        }

        public static T to<T>(object obj, T defVal = default(T))
        {
            if (obj is null)
                return defVal;
            if (obj is T objCasted)
                return objCasted;
            if (typeof(T) == typeof(bool))
                return (T)(object)toBool(obj, (bool)(object)defVal);
            if (typeof(T) == typeof(string))
                return (T)(object)toString(obj, (string)(object)defVal);
            if (typeof(T) == typeof(DateTime))
                return (T)(object)toDateTime(obj, (DateTime)(object)defVal);
            //if (typeof(T) == typeof(Image) || typeof(T) == typeof(Bitmap))
                //return (T)(object)getImage(to(obj, ""), (Image)(object)defVal, "");
            return toNumber(obj, defVal);
        }
        public static T To<T>(this object obj, T defaultValue = default(T))
        {
            return to<T>(obj, defaultValue);
        }
        public static object toNull<T>(object obj)
        {
            T item = to(obj, default(T));
            if (item.Equals(default(T)))
                return null;
            else
                return item;
        }
        public static T to<S, T>(object obj, T defVal = default(T))
        {
            S item = to(obj, default(S));
            if (item.Equals(default(S)))
                return defVal;
            return to(item, defVal);
        }

        public static string dateToString(object dtDate, string defVal = "")
        {
            return dateTimeToString(dtDate, defVal, "yyyy-MM-dd");
        }

        public static string dateTimeToString(object dtDate, string defVal = "", string format = "yyyy-MM-dd HH:mm:ss")
        {
            if (!(dtDate is DateTime))
                dtDate = to(dtDate, default(DateTime));
            if (dtDate.Equals(default(DateTime)))
                return defVal;
            return ((DateTime)dtDate).ToString(format);
        }
        #endregion
    }
}