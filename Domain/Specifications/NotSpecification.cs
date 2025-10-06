using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaces.Specifications;

namespace Domain.Specifications
{
    /// <summary>
    /// Inverte a spec recebida (NOT lógico).
    /// </summary>
    public class NotSpecification<T> : ISpecification<T>
    {
        private readonly ISpecification<T> _inner;

        public NotSpecification(ISpecification<T> inner)
        {
            _inner = inner;
        }
        public Expression<Func<T, bool>> ToExpression()
        {
            var expr = _inner.ToExpression();
            var param = expr.Parameters[0];
            var body = Expression.Not(expr.Body);
            return Expression.Lambda<Func<T, bool>>(body, param);
        }
    }
}
