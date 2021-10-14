Imports DevExpress.Xpo

Namespace Issues
    Public Class Issue
        Inherits XPObject

        Public Sub New(ByVal session As Session)
            MyBase.New(session)
            Created = Date.Now
        End Sub

        Private _Subject As String

        <Size(200)>
        Public Property Subject As String
            Get
                Return _Subject
            End Get
            Set(ByVal value As String)
                SetPropertyValue(NameOf(Issue.Subject), _Subject, value)
            End Set
        End Property

        Private _UserId As Integer

        Public Property UserId As Integer
            Get
                Return _UserId
            End Get
            Set(ByVal value As Integer)
                SetPropertyValue(NameOf(Issue.UserId), _UserId, value)
            End Set
        End Property

        Private _User As User

        <Association("UserIssues")>
        Public Property User As User
            Get
                Return _User
            End Get
            Set(ByVal value As User)
                SetPropertyValue(NameOf(Issue.User), _User, value)
            End Set
        End Property

        Private _Created As Date

        Public Property Created As Date
            Get
                Return _Created
            End Get
            Set(ByVal value As Date)
                SetPropertyValue(NameOf(Issue.Created), _Created, value)
            End Set
        End Property

        Private _Votes As Integer

        Public Property Votes As Integer
            Get
                Return _Votes
            End Get
            Set(ByVal value As Integer)
                SetPropertyValue(NameOf(Issue.Votes), _Votes, value)
            End Set
        End Property

        Private _Priority As Priority

        Public Property Priority As Priority
            Get
                Return _Priority
            End Get
            Set(ByVal value As Priority)
                SetPropertyValue(NameOf(Issue.Priority), _Priority, value)
            End Set
        End Property
    End Class

    Public Enum Priority
        Low
        BelowNormal
        Normal
        AboveNormal
        High
    End Enum
End Namespace