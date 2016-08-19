namespace WPF
{
    using System.Collections.Generic;
    using System.Linq;
    using Model;

    public static class Extentions
    {
        public static bool IsEqualTo<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            if (first.Except(second).Count() == 0 && second.Except(first).Count() == 0)
                return true;

            return false;
        }

        public static bool IsEqualTo(this PromotionDate first, PromotionDate second)
        {
            if ((first.StartDate.Date != second.StartDate.Date) || (first.EndDate.Date != second.EndDate.Date))
                return false;

            return true;
        }
    }
}