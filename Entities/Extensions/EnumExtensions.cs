using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Entities.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum value)
        {
            var member = value.GetType()
                              .GetMember(value.ToString())
                              .FirstOrDefault();
            var display = member?.GetCustomAttribute<DisplayAttribute>();
            return display?.GetName() ?? value.ToString();
        }
    }
}
