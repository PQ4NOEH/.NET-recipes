using System;

namespace Davalor.MomProxy.Domain.Configuration
{
    public class Time
    {
        public Hour Hour { get; set; }
        public Minute Minute { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (Object.ReferenceEquals(this, obj)) return true;

            Time casted = obj as Time;
            if (casted == null) return false;
            return (this.Hour == casted.Hour && this.Minute == casted.Minute);
        }
        public static bool operator ==(Time a, Time b)
        {
            if (object.ReferenceEquals(a, null)) return object.ReferenceEquals(b, null);
            return a.Equals(b);
        }
        public static bool operator !=(Time a, Time b)
        {
            return !(a == b);
        }
        public static bool operator >(Time a, Time b)
        {
            if (a == null || b == null) return false;
           

            if (a.Hour.Value > b.Hour.Value) return true;
            if (a.Hour.Value == b.Hour.Value && a.Minute.Value > b.Minute.Value) return true;
            return false; 
        }
        public static bool operator <(Time a, Time b)
        {
            if (a == null || b == null) return false;
            return !(a > b);
        }
        public static bool operator >=(Time a, Time b)
        {
            return (a == b) ? true : a > b;
        }
        public static bool operator <=(Time a, Time b)
        {
            return (a == b) ? true : a < b;
        }
    }

    public struct Hour
    {
        public readonly int Value;
        public Hour(int hour)
        {
            if (hour < 0 || hour > 23) throw new ArgumentOutOfRangeException("The hour has to be expressed from 0 to 23");
            Value = hour;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Hour? casted = obj as Hour?;
            if (casted == null) return false;
            return this.Value == casted.Value.Value;
        }

        public static bool operator ==(Hour a, Hour b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(Hour a, Hour b)
        {
            return !(a == b);
        }
    }
    public struct Minute
    {
        public readonly int Value;
        public Minute(int minute)
        {
            if (minute < 0 || minute > 59) throw new ArgumentOutOfRangeException("The Minutes has to be expressed from 0 to 59");
            Value = minute;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Minute? casted = obj as Minute?;
            if (casted == null) return false;
            return this.Value == casted.Value.Value;
        }
        public static bool operator ==(Minute a, Minute b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(Minute a, Minute b)
        {
            return !(a == b);
        }
    }
}
