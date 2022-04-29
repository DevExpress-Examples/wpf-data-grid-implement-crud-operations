Imports DevExpress.Xpo
Imports DevExpress.Xpo.DB
Imports DevExpress.Xpo.Metadata
Imports System
Imports System.Configuration

Namespace Issues
    Public Module ConnectionHelper
        Private ReadOnly PersistentTypes As Type() = New Type() {GetType(Issue), GetType(User)}

        Public Sub Connect()
            XpoDefault.DataLayer = CreateDataLayer(True)
        End Sub

        Private Function CreateDataLayer(ByVal threadSafe As Boolean) As IDataLayer
            Dim connStr As String = If(ConfigurationManager.ConnectionStrings("XpoTutorial")?.ConnectionString, "XpoProvider=InMemoryDataStore")
            ' Uncomment this line if you use a database server like SQL Server, Oracle, PostgreSql etc.
            'connStr = XpoDefault.GetConnectionPoolString(connStr);
            Dim dictionary As ReflectionDictionary = New ReflectionDictionary()
            ' Pass all of your persistent object types to this method.
            dictionary.GetDataStoreSchema(PersistentTypes)
            ' Use AutoCreateOption.DatabaseAndSchema if the database or tables do not exist.
            ' Use AutoCreateOption.SchemaAlreadyExists if the database already exists.
            Dim autoCreateOption As AutoCreateOption = AutoCreateOption.DatabaseAndSchema
            Dim provider As IDataStore = XpoDefault.GetConnectionProvider(connStr, autoCreateOption)
            Return If(threadSafe, CType(New ThreadSafeDataLayer(dictionary, provider), IDataLayer), New SimpleDataLayer(dictionary, provider))
        End Function
    End Module
End Namespace
