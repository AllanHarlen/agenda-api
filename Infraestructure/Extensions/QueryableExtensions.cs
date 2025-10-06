using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaces.Specifications;

namespace Infraestructure.Extensions
{
    public static class QueryableExtensions
    {
        /// <summary>
        /// Aplica a Specification<T> como um filtro WHERE.
        /// </summary>
        public static IQueryable<T> ApplySpecification<T>(
            this IQueryable<T> source,
            ISpecification<T> spec)
        {
            return source.Where(spec.ToExpression());
        }
    }
}
