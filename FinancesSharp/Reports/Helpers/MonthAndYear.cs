using System;

namespace FinancesSharp.Reports.Helpers
{
    public struct MonthAndYear : IComparable
    {
        public readonly int Month;
        public readonly int Year;

        public MonthAndYear(int year, int month)
        {
            Year = year;
            Month = month;
        }
        public MonthAndYear(DateTime datetime)
        {
            Year = datetime.Year;
            Month = datetime.Month;
        }

        public MonthAndYear AddMonths(int monthsToAdd)
        {
            return new MonthAndYear(ToDateTime().AddMonths(monthsToAdd));
        }
        public MonthAndYear AddYears(int yearsToAdd)
        {
            return new MonthAndYear(ToDateTime().AddYears(yearsToAdd));
        }

        #region Equality and comparison
        public static bool operator ==(MonthAndYear a, MonthAndYear b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(MonthAndYear a, MonthAndYear b)
        {
            return !a.Equals(b);
        }
        public static bool operator >(MonthAndYear a, MonthAndYear b)
        {
            return a.ToDateTime() > b.ToDateTime();
        }
        public static bool operator <(MonthAndYear a, MonthAndYear b)
        {
            return a.ToDateTime() < b.ToDateTime();
        }
        public static bool operator >=(MonthAndYear a, MonthAndYear b)
        {
            return a.ToDateTime() >= b.ToDateTime();
        }
        public static bool operator <=(MonthAndYear a, MonthAndYear b)
        {
            return a.ToDateTime() <= b.ToDateTime();
        }

        public override bool Equals(object obj)
        {
            var other = obj as MonthAndYear?;
            return other != null
                && Year == other.Value.Year
                && Month == other.Value.Month;
        }
        public override int GetHashCode()
        {
            return string.Format("{0:D4}{1:D2}", Year, Month).GetHashCode();
        }

        public int CompareTo(object obj)
        {
            var other = (MonthAndYear)obj;
            return ToDateTime().CompareTo(other.ToDateTime());
        }
        #endregion

        public override string ToString()
        {
            return new DateTime(Year, Month, 1).ToString("MMMM yyyy");
        }
        public DateTime ToDateTime(PositionInMonth position = PositionInMonth.Start)
        {
            var datetime = new DateTime(Year, Month, 1);
            if (position == PositionInMonth.End)
            {
                datetime = datetime.AddMonths(1).AddDays(-1);
            }
            return datetime;
        }

        public enum PositionInMonth
        {
            Start,
            End
        }

        public static MonthAndYear Current
        {
            get { return new MonthAndYear(DateTime.Now); }
        }
    }
}