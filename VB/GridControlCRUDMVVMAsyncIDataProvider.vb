Imports System.Collections.Generic

Namespace DevExpress.CRUD.DataModel

    Public Interface IDataProvider(Of T As Class)

        Function Read() As IList(Of T)

    End Interface
End Namespace
