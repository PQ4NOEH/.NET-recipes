namespace Altea.Extensions
{
    using System;

    public static class UserHelper
    {
        public static string GetFullName(string propertyNames, string propertyValues)
        {
            if (string.IsNullOrWhiteSpace(propertyNames) || string.IsNullOrWhiteSpace(propertyValues))
            {
                return null;
            }

            string[] propertyNamesSplitted = propertyNames.Split(
                new char[] { ':' },
                StringSplitOptions.RemoveEmptyEntries);

            int firstNameStart = -1;
            int firstNameEnd = -1;
            int lastNameStart = -1;
            int lastNameEnd = -1;

            for (int i = 0, l = propertyNamesSplitted.Length; i < l; i++)
            {
                if (propertyNamesSplitted[i].Equals("FirstName", StringComparison.Ordinal))
                {
                    if (i + 3 == l)
                    {
                        return null;
                    }

                    i++;
                    if (!int.TryParse(propertyNamesSplitted[++i], out firstNameStart)
                        || !int.TryParse(propertyNamesSplitted[++i], out firstNameEnd))
                    {
                        return null;
                    }
                }
                else if (propertyNamesSplitted[i].Equals("LastName", StringComparison.Ordinal))
                {
                    if (i + 3 == l)
                    {
                        return null;
                    }

                    i++;
                    if (!int.TryParse(propertyNamesSplitted[++i], out lastNameStart)
                        || !int.TryParse(propertyNamesSplitted[++i], out lastNameEnd))
                    {
                        return null;
                    }
                }
            }

            string firstName = propertyValues.Substring(firstNameStart, firstNameEnd);
            string lastName = propertyValues.Substring(lastNameStart, lastNameEnd);

            return firstName + " " + lastName;
        }
    }
}
