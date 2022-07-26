Imports DevExpress.Xpf.Core
Imports System.Windows

Namespace GridControlCRUDMVVMAsync

    Public Partial Class App
        Inherits Application

        Public Sub New()
            Call ApplicationThemeHelper.UpdateApplicationThemeName()
            DevExpress.Internal.DbEngineDetector.PatchConnectionStringsAndConfigureEntityFrameworkDefaultConnectionFactory()
        End Sub
    End Class
End Namespace
