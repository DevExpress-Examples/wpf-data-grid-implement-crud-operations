Imports Microsoft.EntityFrameworkCore

Namespace Issues
    Public Class IssuesContext
        Inherits DbContext

        Private Shared ReadOnly options As DbContextOptions(Of IssuesContext) = New DbContextOptionsBuilder(Of IssuesContext)().UseInMemoryDatabase(databaseName:="Test").Options

        Public Sub New()
            MyBase.New(options)
        End Sub

        Public Property Issues As DbSet(Of Issue)
        Public Property Users As DbSet(Of User)
    End Class
End Namespace
