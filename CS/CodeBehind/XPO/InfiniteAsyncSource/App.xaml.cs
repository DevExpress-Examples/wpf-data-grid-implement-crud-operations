using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using XPOIssues.Issues;

namespace XPOIssues {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        public App() {
            ConnectionHelper.Connect();
            DemoDataHelper.Seed();
        }
    }
}
