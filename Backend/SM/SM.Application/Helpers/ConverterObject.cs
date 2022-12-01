
using System;
using System.Collections.Generic;
using CMC.Common.Extensions;
using System.Collections;
using System.Linq.Expressions;
using System.Linq;
using SM.Domain.Enum.Positioning;

namespace SM.Application.Helpers
{
    public static class ConverterObject
    {
        public static object TryConvertToInt(string value)
        {
            if (int.TryParse(value, out int valueInt))
                return valueInt;

            return value.ToString();
        }

        public static Expression<Func<T, bool>> GetCondition<T>(string propertyName, object category)
        {
            var param = Expression.Parameter(typeof(T));

            var isNumber = category.ToString().All(char.IsNumber);

            var value = Convert.ChangeType(category, isNumber && propertyName.Equals("LevelId") ? typeof(int) : isNumber ? typeof(long) : typeof(string));

            if (value.ToString().Equals(((int)FilterAllEnum.All).ToString()) ||
                value.ToString().Equals(FilterAllEnum.All.ToString()))
                return Expression.Lambda<Func<T, bool>>(Expression.Constant(true), param);


            return Expression.Lambda<Func<T, bool>>(
                    Expression.Equal(
                        Expression.Property(param, propertyName),
                        Expression.Constant(value, value.GetType())
                    ),
                    param
                );
        }


        public static IEnumerable<T> GetInHeaderProperty<T>(
            object list,
            Type type,
            List<string> exp) where T : new()
        {
            var resultList = new List<T>();

            int count = 0;
            foreach (var item in list as IEnumerable)
            {
                if (!exp.Contains(item.ToString()))
                {
                    var result = new T();

                    var newItem = Convert.ChangeType(item, type);

                    foreach (var propertyInfo in result.GetType().GetProperties())
                    {
                        if (propertyInfo.Name.Equals("Type"))
                        {
                            propertyInfo.SetValue(result, typeof(string).Name);
                            continue;
                        }
                        if (propertyInfo.Name.Equals("ColPos"))
                        {
                            propertyInfo.SetValue(result, count);
                            continue;
                        }
                        if (propertyInfo.PropertyType.Name.Equals(typeof(string).Name))
                        {
                            propertyInfo.SetValue(result, (newItem as Enum).GetDescription());
                        }




                    }
                    count++;
                    resultList.Add(result);
                }
            }
            return resultList;
        }
    }
}
