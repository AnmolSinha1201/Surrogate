using System;

namespace Surrogate.Helpers
{
    public static class Extensions
    {
        public static T With<T>(this T Item, Action<T> Predicate) where T: IClonable<T>
        {
            var clone = Item.Clone();
            Predicate(clone);
            return clone;
        }
    }

    public interface IClonable<T>
    {
        T Clone();
    }
}