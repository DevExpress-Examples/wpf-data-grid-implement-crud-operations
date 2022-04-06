Imports DevExpress.Xpo

Namespace Issues
    Public Class User
        Inherits XPObject

        Public Sub New(ByVal session As Session)
            MyBase.New(session)
        End Sub

        Private _FirstName As String

        Public Property FirstName As String
            Get
                Return _FirstName
            End Get
            Set(ByVal value As String)
                SetPropertyValue(NameOf(User.FirstName), _FirstName, value)
            End Set
        End Property

        Private _LastName As String

        Public Property LastName As String
            Get
                Return _LastName
            End Get
            Set(ByVal value As String)
                SetPropertyValue(NameOf(User.LastName), _LastName, value)
            End Set
        End Property

        <Association("UserIssues")>
        Public ReadOnly Property Issues As XPCollection(Of Issue)
            Get
                Return GetCollection(Of Issue)(NameOf(User.Issues))
            End Get
        End Property
    End Class
End Namespace
