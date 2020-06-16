Imports System
Imports System.Collections.Generic
Imports System.Linq

Namespace DevExpress.CRUD.DataModel
	Public Interface IDataProvider(Of T As Class)
		Function Read() As IList(Of T)
	End Interface
End Namespace
