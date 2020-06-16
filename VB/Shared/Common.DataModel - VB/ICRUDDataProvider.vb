Imports System
Imports System.Linq

Namespace DevExpress.CRUD.DataModel
	Public Interface ICRUDDataProvider(Of T As Class)
		Inherits IDataProvider(Of T)

		Sub Create(ByVal obj As T)
		Sub Update(ByVal obj As T)
		Sub Delete(ByVal obj As T)
	End Interface
End Namespace
