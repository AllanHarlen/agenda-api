using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infraestructure.Converters
{
    /// <summary>
    /// Converter para DateTime - normaliza qualquer Kind para UTC ao salvar e marca como UTC ao ler
    /// </summary>
    public class MultiDbDateTimeConverter : ValueConverter<DateTime, DateTime>
    {
        public MultiDbDateTimeConverter() : base(
            v => NormalizeToUtc(v),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
        {
        }

        private static DateTime NormalizeToUtc(DateTime dateTime)
        {
            return dateTime.Kind switch
            {
                DateTimeKind.Unspecified => DateTime.SpecifyKind(dateTime, DateTimeKind.Utc),
                DateTimeKind.Local => dateTime.ToUniversalTime(),
                DateTimeKind.Utc => dateTime,
                _ => dateTime
            };
        }
    }

    /// <summary>
    /// Converter para DateTime? - normaliza qualquer Kind para UTC ao salvar e marca como UTC ao ler
    /// </summary>
    public class MultiDbNullableDateTimeConverter : ValueConverter<DateTime?, DateTime?>
    {
        public MultiDbNullableDateTimeConverter() : base(
            v => v.HasValue ? NormalizeToUtc(v.Value) : v,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v)
        {
        }

        private static DateTime NormalizeToUtc(DateTime dateTime)
        {
            return dateTime.Kind switch
            {
                DateTimeKind.Unspecified => DateTime.SpecifyKind(dateTime, DateTimeKind.Utc),
                DateTimeKind.Local => dateTime.ToUniversalTime(),
                DateTimeKind.Utc => dateTime,
                _ => dateTime
            };
        }
    }
}
