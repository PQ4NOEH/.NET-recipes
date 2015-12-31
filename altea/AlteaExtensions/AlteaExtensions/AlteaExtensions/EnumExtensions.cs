namespace Altea.Extensions
{
    using System;
    using System.Collections.Concurrent;

    public static class EnumExtensions
    {
        private static readonly ConcurrentDictionary<Tuple<Enum, Type>, Attribute> Attributes =
            new ConcurrentDictionary<Tuple<Enum, Type>, Attribute>();

        public static T GetAttribute<T>(this Enum @this)
            where T : Attribute
        {
            if (@this == null)
            {
                throw new ArgumentNullException("this");
            }

            Type type = @this.GetType();
            Tuple<Enum, Type> tuple = Tuple.Create(@this, typeof(T));

            Attribute attribute;
            Attributes.TryGetValue(tuple, out attribute);

            if (attribute == null)
            {
                attribute = Attribute.GetCustomAttribute(
                    type.GetMember(Enum.GetName(type, @this))[0],
                    typeof(T)
                );

                Attributes.TryAdd(tuple, attribute);
            }

            return attribute as T;
        }
    }
}
