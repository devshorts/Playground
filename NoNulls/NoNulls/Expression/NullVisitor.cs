using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NoNulls
{
    public class NullVisitor<T> : ExpressionVisitor
    {
        private Stack<Expression> expressions = new Stack<Expression>();

        private Expression finalExpression;

        private Boolean IsMethod { get; set; }
        
        private void CaptureFinal(Expression node, bool isMethod)
        {
            if (finalExpression == null)
            {
                finalExpression = node;

                IsMethod = isMethod;
            }
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            var result = base.Visit(node.Body);

            return Expression.Lambda(result);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            CaptureFinal(node, true);

            expressions.Push(node);

            if (IsMethod)
            {
                return BuildFinal(node);
            }

            return Visit(node.Object);
        }

        protected override Expression VisitMember(MemberExpression node)
        {            
            expressions.Push(node);

            CaptureFinal(node, false);

            var exp = Visit(node.Expression);

            if (IsMethod)
            {
                return node;
            }
            
            return BuildFinal(exp);
        }

        private Expression BuildFinal(Expression exp)
        {
            if (expressions.Count == 0)
            {
                return exp;
            }

            var condition = BuildIfs(expressions.Pop());

            return Expression.Block(new[] { condition });
        }

        private Expression BuildIfs(Expression top)
        {
            var stringRepresentation = Expression.Constant(top.ToString(), typeof(string));

            var trueVal = Expression.Constant(true);

            var falseVal = Expression.Constant(false);

            var nullValue = Expression.Constant(default(T), finalExpression.Type);

            var constructorInfo = typeof(MethodValue<T>).GetConstructor(new[] { typeof(T), typeof(string), typeof(bool) });

            var returnNull = Expression.New(constructorInfo, new [] { nullValue, stringRepresentation, falseVal });

            var ifNull = Expression.ReferenceEqual(top, Expression.Constant(null));
            
            var finalReturn = Expression.New(constructorInfo, new []{ finalExpression, stringRepresentation, trueVal });
           
            if (expressions.Count == 1)
            {
                expressions.Clear();
            }
            
            var condition = Expression.Condition(ifNull, returnNull, 
                    expressions.Count == 0 ? 
                        finalReturn 
                        : BuildIfs(expressions.Pop()));

            
            return condition;            
        }
    }
}