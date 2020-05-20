using DevExpress.Xpf.Core;
using System.Windows;

namespace GridControlCRUDSimple {
    public partial class App : Application {
        public App() {
            ApplicationThemeHelper.UpdateApplicationThemeName();
            DevExpress.Internal.DbEngineDetector.PatchConnectionStringsAndConfigureEntityFrameworkDefaultConnectionFactory();
        }
    }
}
