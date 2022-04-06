Imports XPOIssues.Issues

Class Application

    ' Application-level events, such as Startup, Exit, and DispatcherUnhandledException
    ' can be handled in this file.
    Public Sub New()
        ConnectionHelper.Connect()
        DemoDataHelper.Seed()
    End Sub
End Class
