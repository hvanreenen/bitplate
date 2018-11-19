using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HJORM
{
    
    public static class DataConverter
    {
        public static Guid ToGuid(Object value)
        {
            return value == DBNull.Value ? Guid.Empty : new Guid(value.ToString());
        }

        public static bool ToBoolean(Object value)
        {
            return value == DBNull.Value ? false : Convert.ToBoolean(value);
        }

        public static int ToInt32(Object value)
        {
            return value == DBNull.Value ? 0 : Convert.ToInt32(value);
        }

        public static int? ToNullableInt(Object value)
        {
            return value == DBNull.Value ? null : (int?)Convert.ToInt32(value);
        }

        public static double ToDouble(Object value)
        {
            return value == DBNull.Value ? 0 : Convert.ToDouble(value);
        }

        public static double? ToNullableDouble(Object value)
        {
            return value == DBNull.Value ? null : (double?)Convert.ToDouble(value);
        }

        public static DateTime ToDateTime(Object value)
        {
            return value == DBNull.Value ? new DateTime() : Convert.ToDateTime(value);
        }

        public static DateTime? ToNullableDateTime(Object value)
        {
            return value == DBNull.Value ? null : (DateTime?) Convert.ToDateTime(value);
        }
    }
}
