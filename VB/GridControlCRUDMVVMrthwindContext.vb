Imports System.Data.Entity

Namespace DevExpress.CRUD.Northwind

    Public Class NorthwindContext
        Inherits DbContext

        Shared Sub New()
            Call Database.SetInitializer(New NorthwindContextInitializer())
        End Sub

        Protected Overrides Sub OnModelCreating(ByVal modelBuilder As DbModelBuilder)
            modelBuilder.Entity(Of Product)().Property(Function(x) x.Name).IsRequired()
        End Sub

        Public Property Categories As DbSet(Of Category)

        Public Property Products As DbSet(Of Product)
    End Class
End Namespace
