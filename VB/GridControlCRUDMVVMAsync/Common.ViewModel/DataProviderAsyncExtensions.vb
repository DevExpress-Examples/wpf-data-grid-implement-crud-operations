Imports DevExpress.CRUD.DataModel
Imports System.Collections.Generic
Imports System.Threading.Tasks
Imports System.Runtime.CompilerServices

Namespace DevExpress.CRUD.ViewModel

    Public Module DataProviderAsyncExtensions

        <Extension()>
        Public Async Function ReadAsync(Of T As Class)(ByVal dataProvider As IDataProvider(Of T)) As Task(Of IList(Of T))
#If DEBUG
            Await Task.Delay(500)
#End If
            Return Await Task.Run(New System.Func(Of IList(Of T))(AddressOf dataProvider.Read))
        End Function

        <Extension()>
        Public Async Function UpdateAsync(Of T As Class)(ByVal dataProvider As ICRUDDataProvider(Of T), ByVal entity As T) As Task
#If DEBUG
            Await Task.Delay(500)
#End If
            Await Task.Run(Sub() dataProvider.Update(entity))
        End Function

        <Extension()>
        Public Async Function CreateAsync(Of T As Class)(ByVal dataProvider As ICRUDDataProvider(Of T), ByVal entity As T) As Task
#If DEBUG
            Await Task.Delay(500)
#End If
            Await Task.Run(Sub() dataProvider.Create(entity))
        End Function
    End Module
End Namespace
