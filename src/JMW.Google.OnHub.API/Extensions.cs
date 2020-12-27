using Microsoft.Extensions.Configuration;

namespace JMW.Google.OnHub.API
{
    public static class Extensions
    {
        public static T GetSection<T>(this IConfiguration config)
        {
            var name = typeof(T).Name;
            if (name.EndsWith("Options"))
            {
                name = name.Substring(0, name.Length - 7);
            }
            return config.GetSection(name)
                .Get<T>();
        }

        public static IConfigurationSection Section<T>(this IConfiguration config)
        {
            var name = typeof(T).Name;
            if (name.EndsWith("Options"))
            {
                name = name.Substring(0, name.Length - 7);
            }
            return config.GetSection(name);
        }
    }
}