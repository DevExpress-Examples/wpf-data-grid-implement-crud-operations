Imports DevExpress.Data.Filtering
Imports DevExpress.Xpf.Data

Namespace GridControlCRUDMVVMInfiniteAsyncSource
	Public Class IssueDataFilterConverter
		Inherits ExpressionFilterConverter(Of IssueData)

		Protected Overrides Sub SetUpConverter(ByVal converter As GridFilterCriteriaToExpressionConverter(Of IssueData))
			converter.RegisterFunctionExpressionFactory(operatorType:= FunctionOperatorType.StartsWith, factory:= Function(value As String)
				Dim toLowerValue = value.ToLower()
				Return x
				If True Then
					Get
						Return x.ToLower().StartsWith(toLowerValue)
					End Get
				End If
			End Function)
		End Sub
	End Class
End Namespace
