Namespace Issues
    Public Class Issue
        Public Property Id As Integer
        Public Property Subject As String
        Public Property UserId As Integer
        Public Overridable Property User As User
        Public Property Created As Date
        Public Property Votes As Integer
        Public Property Priority As Priority

        Public Sub New()
            Created = Date.Now
        End Sub
    End Class

    Public Enum Priority
        Low
        BelowNormal
        Normal
        AboveNormal
        High
    End Enum
End Namespace

