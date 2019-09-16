﻿using Philo.Search.Filter;
using Philo.Search.Helper;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Philo.Search.Mapping
{
  public class FilterMapping<TEntityType, TPropType>
    : IMapAFilter<TEntityType>
    where TEntityType : class
  {
    public FilterMapping(string field, Expression<Func<TEntityType, TPropType>> mapping)
    {
      Field = field;
      Mapping = mapping;

      var memberExpression = mapping.Body as MemberExpression;
      var binaryExpression = mapping.Body as BinaryExpression;
      if ((binaryExpression == null && memberExpression == null)
        || (memberExpression != null && memberExpression.Member.MemberType != MemberTypes.Property))
      {
        throw new NotSupportedException("Expression must be member expression and refer to a property");
      }
    }

    public string Field { get; set; }

    public bool IsDefaultSortFilter { get; set; } = false;
    private Expression<Func<TEntityType, TPropType>> Mapping { get; set; }

    public IQueryable<TEntityType> ApplySort(IQueryable<TEntityType> query, bool descending)
    {
      return query.OrderByWithDirection(Mapping, descending);
    }

    public Expression<Func<TEntityType, bool>> GetFilterLambda(string value, Comparator comparator)
    {
      return SearchHelper.GetLambdaExpression<TEntityType, TPropType>(Mapping, value, comparator);
    }
  }


  public class BinaryFilter<TEntityType, TPropType> : IMapAFilter<TEntityType> where TEntityType : class
  {
    public BinaryFilter(string field, Expression<Func<TEntityType, TPropType>> mapping)
    {
      Field = field;
      Mapping = mapping;

      var binaryExpression = mapping.Body as BinaryExpression;
      if (binaryExpression == null)
      {
        throw new NotSupportedException("Expression must be binary expression");
      }
    }

    public string Field { get; set; }

    public bool IsDefaultSortFilter { get; set; } = false;
    private Expression<Func<TEntityType, TPropType>> Mapping { get; set; }

    public IQueryable<TEntityType> ApplySort(IQueryable<TEntityType> query, bool descending)
    {
      throw new NotImplementedException();
    }

    public Expression<Func<TEntityType, bool>> GetFilterLambda(string value, Comparator comparator)
    {
      return SearchHelper.GetLambdaExpression<TEntityType, TPropType>(Mapping, value, comparator);
    }
  }
}