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
    /// Combina duas specs via AND l�gico.
    /// </summary>
    public class AndSpecification<T> : ISpecification<T>
    {
        private readonly ISpecification<T> _left;
        private readonly ISpecification<T> _right;

        public AndSpecification(ISpecification<T> left, ISpecification<T> right)
        {
            _left = left;
            _right = right;
        }

        public Expression<Func<T, bool>> ToExpression()
        {
            var leftExpr = _left.ToExpression();
            var rightExpr = _right.ToExpression();

            // substitui o par�metro para o mesmo em ambas as express�es
            var param = leftExpr.Parameters.Single();

            var invokedRight = Expression.Invoke(rightExpr, param);
            var body = Expression.AndAlso(leftExpr.Body, invokedRight);

            return Expression.Lambda<Func<T, bool>>(body, param);
        }
    }
}
