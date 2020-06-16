Imports DevExpress.Xpf.Core
Imports System.Windows

Namespace GridControlCRUDSimple
	Partial Public Class App
		Inherits Application

		Public Sub New()
			ApplicationThemeHelper.UpdateApplicationThemeName()
			DevExpress.Internal.DbEngineDetector.PatchConnectionStringsAndConfigureEntityFrameworkDefaultConnectionFactory()
		End Sub
	End Class
End Namespace
