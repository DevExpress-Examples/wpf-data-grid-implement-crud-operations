Imports DevExpress.Mvvm
Imports DevExpress.Xpf.Core
Imports System.Windows

Namespace GridControlCRUDMVVMAsync
	Partial Public Class App
		Inherits Application

		Public Sub New()
			ApplicationThemeHelper.UpdateApplicationThemeName()
			DevExpress.Internal.DbEngineDetector.PatchConnectionStringsAndConfigureEntityFrameworkDefaultConnectionFactory()
		End Sub
	End Class
End Namespace
