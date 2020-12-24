using DevExpress.Data.Filtering;
using DevExpress.Xpf.Data;
using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Data;

namespace GridControlCRUDMVVMInfiniteAsyncSource {
    public class ExpressionFilterConverter : IValueConverter {
        public Type Type { get; set; }
        object IValueConverter.Convert(object filter, Type targetType, object parameter, CultureInfo culture) {
            var convertMethod = GetType()
                .GetMethod(nameof(ConvertCore), BindingFlags.Static | BindingFlags.NonPublic)
                .MakeGenericMethod(Type);
            return convertMethod.Invoke(null, new[] { filter });
        }
        static Expression<Func<T, bool>> ConvertCore<T>(CriteriaOperator filter) {
            var converter = new GridFilterCriteriaToExpressionConverter<T>();
            converter.RegisterFunctionExpressionFactory(
                operatorType: FunctionOperatorType.StartsWith,
                factory: (string value) => {
                    var toLowerValue = value.ToLower();
                    return x => x.ToLower().StartsWith(toLowerValue);
                });
            return converter.Convert(filter);
        }
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
