using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaces.Specifications;
using Entities.Models;

namespace Domain.Specifications
{
    /// <summary>
    /// Gera um filtro dinámico a partir de PesquisaAvancada (Campo, Valor, Condicional).
    /// </summary>
    public class FilterSpecification<T> : ISpecification<T> where T : class
    {
        private readonly PesquisaAvancada _filtro;

        public FilterSpecification(PesquisaAvancada filtro)
        {
            _filtro = filtro;
        }

        public Expression<Func<T, bool>> ToExpression()
        {
            var tipo = typeof(T);
            var propInfo = tipo.GetProperty(_filtro.Campo)
                           ?? throw new ArgumentException($"Campo '{_filtro.Campo}' não existe em {tipo.Name}");

            var param = Expression.Parameter(tipo, "e");
            var member = Expression.Property(param, propInfo);

            object? valor;
            if (string.IsNullOrWhiteSpace(_filtro.Valor) || _filtro.Valor.Equals("null", StringComparison.OrdinalIgnoreCase))
                valor = null;            
            else
                if (propInfo.PropertyType.IsEnum)
                {
                    valor = Enum.Parse(propInfo.PropertyType, _filtro.Valor, ignoreCase: true);
                }
                else
                {
                    var targetType = Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType;
                    valor = Convert.ChangeType(_filtro.Valor, targetType);
                }                           

            var constant = Expression.Constant(valor, propInfo.PropertyType);
            Expression body = _filtro.Condicional switch
            {
                "==" => Expression.Equal(member, constant),
                "!=" => Expression.NotEqual(member, constant),
                ">" => Expression.GreaterThan(member, constant),
                "<" => Expression.LessThan(member, constant),
                "in" => BuildInExpression(propInfo, member, _filtro.Valor),
                _ => throw new NotSupportedException($"Operador '{_filtro.Condicional}' não suportado")
            };

            return Expression.Lambda<Func<T, bool>>(body, param);
        }

        private static Expression BuildInExpression(
            PropertyInfo propInfo,
            MemberExpression member,
            string valor)
        {
            var elementos = valor
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(v => Convert.ChangeType(v, propInfo.PropertyType))
                .ToList();

            var listaConst = Expression.Constant(elementos);
            var containsMi = typeof(List<>)
                .MakeGenericType(propInfo.PropertyType)
                .GetMethod("Contains", new[] { propInfo.PropertyType })!;

            return Expression.Call(listaConst, containsMi, member);
        }
    }
}
