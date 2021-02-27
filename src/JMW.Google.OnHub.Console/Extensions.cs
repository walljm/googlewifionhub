using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JMW.Extensions.Text
{
    public static class Extensions
    {
        private class Column
        {
            public PropertyInfo Info { get; set; }
            public string Name { get; set; }
            public int Width { get; set; }
            public bool IsList { get; set; }
        }

        /// <summary>
        /// Changes a "CamelCasedString" to "Camel Cased String"
        /// </summary>
        /// <param name="camelCasedString"></param>
        /// <returns></returns>
        public static string SpaceByCamelCase(this string camelCasedString)
        {
            return System.Text.RegularExpressions.Regex.Replace(camelCasedString,
                "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");
        }

        public static bool IsEnumerable(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        }

        public static void WriteAsTable<T>(this IEnumerable<T> objs, Action<string> write, char columnDivider = '-', string indent = "", string propDescription = "")
        {
            if (!objs.Any())
            {
                return;
            }

            var groups = objs.GroupBy(k => k.GetType());

            foreach (var group in groups)
            {
                var t = group.Key;
                write($"{indent}  Record Type: {t.Name} {propDescription}{Environment.NewLine}");
                var fArray = t
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Select(propInfo =>
                    {
                        return new Column
                        {
                            Info = propInfo,
                            Name = propInfo.Name.SpaceByCamelCase(),
                            Width = 0,
                            IsList = propInfo.PropertyType.IsEnumerable()
                        };
                    })
                    .ToList();

                // calculate column widths
                foreach (var prop in fArray)
                {
                    prop.Width = prop.Width < prop.Name.Length ? prop.Name.Length : prop.Width;
                    foreach (var d in group)
                    {
                        var v = prop.Info.GetValue(d)?.ToString() ?? "";
                        prop.Width = prop.Width < v.Length ? v.Length : prop.Width;
                    }
                }

                // column headers.
                write(indent);
                foreach (var prop in fArray.Where(o => !o.IsList))
                {
                    write($"  {prop.Name.PadRight(prop.Width + 2)}");
                }
                write(Environment.NewLine);

                // column divider
                write(indent);
                foreach (var prop in fArray.Where(o => !o.IsList))
                {
                    write($"  {string.Empty.PadRight(prop.Width, columnDivider)}  ");
                }
                write(Environment.NewLine);

                // data
                foreach (var d in group)
                {
                    write(indent);
                    foreach (var prop in fArray.Where(o => !o.IsList))
                    {
                        var v = prop.Info.GetValue(d)?.ToString() ?? "";
                        write($"  {v.PadRight(prop.Width + 2)}");
                    }
                    write(Environment.NewLine);
                    foreach (var prop in fArray.Where(o => o.IsList))
                    {
                        var v = prop.Info.GetValue(d);
                        MethodInfo method = typeof(Extensions).GetMethod(nameof(Extensions.WriteAsTable));
                        MethodInfo generic = method.MakeGenericMethod(prop.Info.PropertyType.GetGenericArguments()[0]);
                        generic.Invoke(null, new[] { v, write, columnDivider, $"  {indent}", prop.Name });
                    }
                }
                write(Environment.NewLine);
            }
        }
    }
}