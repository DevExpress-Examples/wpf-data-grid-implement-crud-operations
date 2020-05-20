using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using System.Windows;

namespace GridControlCRUDMVVMAsync {
    public partial class App : Application {
        public App() {
            ApplicationThemeHelper.UpdateApplicationThemeName();
            DevExpress.Internal.DbEngineDetector.PatchConnectionStringsAndConfigureEntityFrameworkDefaultConnectionFactory();
        }
    }
}
