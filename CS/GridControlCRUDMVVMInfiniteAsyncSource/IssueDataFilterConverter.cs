using DevExpress.Data.Filtering;
using DevExpress.Xpf.Data;

namespace GridControlCRUDMVVMInfiniteAsyncSource {
    public class IssueDataFilterConverter : ExpressionFilterConverter<IssueData> {
        protected override void SetUpConverter(GridFilterCriteriaToExpressionConverter<IssueData> converter) {
            converter.RegisterFunctionExpressionFactory(
                operatorType: FunctionOperatorType.StartsWith,
                factory: (string value) => {
                    var toLowerValue = value.ToLower();
                    return x => x.ToLower().StartsWith(toLowerValue);
                });
        }
    }
}
