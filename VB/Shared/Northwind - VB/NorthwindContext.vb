Imports System
Imports System.Collections.Generic
Imports System.Data.Entity
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks

Namespace DevExpress.CRUD.Northwind
	Public Class NorthwindContext
		Inherits DbContext

		Shared Sub New()
			Database.SetInitializer(New NorthwindContextInitializer())
		End Sub
		Protected Overrides Sub OnModelCreating(ByVal modelBuilder As DbModelBuilder)
			modelBuilder.Entity(Of Product)().Property(Function(x) x.Name).IsRequired()
		End Sub
		Public Property Categories() As DbSet(Of Category)
		Public Property Products() As DbSet(Of Product)
	End Class
End Namespace
