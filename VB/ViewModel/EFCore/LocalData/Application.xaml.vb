Imports EFCoreIssues.Issues

Class Application

    ' Application-level events, such as Startup, Exit, and DispatcherUnhandledException
    ' can be handled in this file.
    Public Sub New()
        IssuesContextInitializer.Seed()
    End Sub
End Class
