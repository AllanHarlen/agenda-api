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
    /// Combina duas specs via OR lógico.
    /// </summary>
    public class OrSpecification<T> : ISpecification<T>
    {
        private readonly ISpecification<T> _left;
        private readonly ISpecification<T> _right;

        public OrSpecification(ISpecification<T> left, ISpecification<T> right)
        {
            _left = left;
            _right = right;
        }

        public Expression<Func<T, bool>> ToExpression()
        {
            var leftExpr = _left.ToExpression();
            var rightExpr = _right.ToExpression();

            var param = leftExpr.Parameters.Single();
            var invokedRight = Expression.Invoke(rightExpr, param);
            var body = Expression.OrElse(leftExpr.Body, invokedRight);

            return Expression.Lambda<Func<T, bool>>(body, param);
        }
    }
}
