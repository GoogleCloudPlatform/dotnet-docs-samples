using Npgsql;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ServiceSharedCore.Utilities
{
    public static class ReflectionPopulator
    {
        // Method for creating the dynamic funtion for setting entity properties

        // Cache store for memorizing the delegate for later use
        private static ConcurrentDictionary<Type, Delegate> ExpressionCache = new ConcurrentDictionary<Type, Delegate>();
        public static Func<NpgsqlDataReader, T> GetReader<T>()
        {
            Delegate resDelegate;
            if (!ExpressionCache.TryGetValue(typeof(T), out resDelegate))
            {
                // Get the indexer property of NpgsqlDataReader 
                var indexerProperty = typeof(NpgsqlDataReader).GetProperty("Item", typeof(object), new[] { typeof(string) });
                // List of statements in our dynamic method 
                var statements = new List<Expression>();
                // Instance type of target entity class 
                ParameterExpression instanceParam = Expression.Variable(typeof(T));
                // Parameter for the NpgsqlDataReader object
                ParameterExpression readerParam =
                    Expression.Parameter(typeof(NpgsqlDataReader));

                // Create and assign new T to variable. Ex. var instance = new T(); 
                BinaryExpression createInstance = Expression.Assign(instanceParam, Expression.New(typeof(T)));
                statements.Add(createInstance);

                foreach (var property in typeof(T).GetProperties())
                {

                    // instance.Property 
                    MemberExpression getProperty = Expression.Property(instanceParam, property);
                    // row[property] The assumption is, column names are the 
                    // same as PropertyInfo names of T 
                    IndexExpression readValue =
                        Expression.MakeIndex(readerParam, indexerProperty,
                        new[] { Expression.Constant(property.Name) });
                    // instance.Property = row[property] 
                    UnaryExpression convertExpression = Expression.Convert(readValue, property.PropertyType);
                    BinaryExpression assignProperty = Expression.Assign(getProperty, convertExpression);
                    //needs a try block in case we don't fetch up all the fields on the model
                    TryExpression tryExpression =
                        Expression.TryCatch(
                                assignProperty,
                            Expression.Catch(
                                typeof(Exception),
                                Expression.Assign(getProperty, property.PropertyType == typeof(string) ?
                                Expression.Convert(Expression.Constant(""), property.PropertyType) :
                                Expression.Convert(Expression.Constant(0), property.PropertyType))));

                    statements.Add(tryExpression);
                    //statements.Add(assignProperty);
                }
                var returnStatement = instanceParam;
                statements.Add(returnStatement);

                var body = Expression.Block(instanceParam.Type, new[] { instanceParam }, statements.ToArray());

                var lambda = Expression.Lambda<Func<NpgsqlDataReader, T>>(body, readerParam);
                resDelegate = lambda.Compile();

                // Cache the dynamic method into ExpressionCache dictionary
                ExpressionCache[typeof(T)] = resDelegate;
            }
            return (Func<NpgsqlDataReader, T>)resDelegate;
        }
        // Method to be called for getting the list of entity class
        // filled with data from the data reader. 
    }
}
