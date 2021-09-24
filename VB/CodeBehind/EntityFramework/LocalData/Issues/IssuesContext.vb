Imports System.Data.Entity

Namespace Issues
    Public Class IssuesContext
        Inherits DbContext

        Shared Sub New()
            Database.SetInitializer(New IssuesContextInitializer())
        End Sub

        Public Sub New()
        End Sub

        Protected Overrides Sub OnModelCreating(ByVal modelBuilder As DbModelBuilder)
            MyBase.OnModelCreating(modelBuilder)
            modelBuilder.Entity(Of Issue)().HasIndex(Function(x) x.Created)
            modelBuilder.Entity(Of Issue)().HasIndex(Function(x) x.Votes)
        End Sub

        Public Property Issues As DbSet(Of Issue)
        Public Property Users As DbSet(Of User)
    End Class
End Namespace

