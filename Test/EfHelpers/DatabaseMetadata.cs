﻿// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace test.EfHelpers
{
    public static class DatabaseMetadata
    {
        public static string GetTableName<TEntity>(this DbContext context)
        {
            var efType = context.Model.FindEntityType(typeof(TEntity).FullName);
            var relational = efType.Relational();
            return relational.TableName;
        }

        public static string GetColumnName<TEntity, TProperty>(this DbContext context, TEntity source, 
            Expression<Func<TEntity, TProperty>> model) where TEntity : class
        {
            var efType = context.Model.FindEntityType(typeof(TEntity).FullName);
            var propInfo = GetPropertyInfoFromLambda(model);
            return efType.FindProperty(propInfo.Name).Relational().ColumnName;
        }

        public static string GetColumnName<TEntity>(this DbContext context, string propertyName) where TEntity : class
        {
            var efType = context.Model.FindEntityType(typeof(TEntity).FullName);
            return efType.FindProperty(propertyName).Relational().ColumnName;
        }

        public static string GetColumnNameSqlite<TEntity, TProperty>(this DbContext context, TEntity source,
            Expression<Func<TEntity, TProperty>> model) where TEntity : class
        {
            var efType = context.Model.FindEntityType(typeof(TEntity).FullName);
            var propInfo = GetPropertyInfoFromLambda(model);
            return efType.FindProperty(propInfo.Name).Sqlite().ColumnName;
        }

        public static string GetColumnRelationalType<TEntity, TProperty>(this DbContext context, 
            TEntity source, Expression<Func<TEntity, TProperty>> model) where TEntity : class
        {
            var efType = context.Model.FindEntityType(typeof(TEntity).FullName);
            var propInfo = GetPropertyInfoFromLambda(model);
            var relational = efType.FindProperty(propInfo.Name).Relational();
            return relational.ColumnType;
        }

        public static string GetColumnSqliteType<TEntity, TProperty>(this DbContext context, TEntity source, 
            Expression<Func<TEntity, TProperty>> model) where TEntity : class
        {
            var efType = context.Model.FindEntityType(typeof(TEntity).FullName);
            var propInfo = GetPropertyInfoFromLambda(model);
            var relational = efType.FindProperty(propInfo.Name).Sqlite();
            return relational.ColumnType;
        }

        //---------------------------------------------------
        //private methods

        private static PropertyInfo GetPropertyInfoFromLambda<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> model) where TEntity : class
        {
            var memberEx = (MemberExpression)model.Body;
            if (memberEx == null)
                throw new ArgumentNullException("model", "You must supply a LINQ expression that is a property.");

            var propInfo = typeof(TEntity).GetProperty(memberEx.Member.Name);
            if (propInfo == null)
                throw new ArgumentNullException("model", "The member you gave is not a property.");
            return propInfo;
        }
    }
}