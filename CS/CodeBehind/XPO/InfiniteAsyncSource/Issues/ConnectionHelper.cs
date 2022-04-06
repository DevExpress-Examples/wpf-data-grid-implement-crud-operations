using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using System;
using System.Configuration;

namespace XPOIssues.Issues {
    public static class ConnectionHelper {

        static readonly Type[] PersistentTypes = new Type[]{
            typeof(Issue),
            typeof(User)
        };

        public static void Connect() {
            XpoDefault.DataLayer = CreateDataLayer(true);
        }

        static IDataLayer CreateDataLayer(bool threadSafe) {
            string connStr = ConfigurationManager.ConnectionStrings["XpoTutorial"]?.ConnectionString ?? "XpoProvider=InMemoryDataStore";
            //connStr = XpoDefault.GetConnectionPoolString(connStr);  // Uncomment this line if you use a database server like SQL Server, Oracle, PostgreSql etc.
            ReflectionDictionary dictionary = new ReflectionDictionary();
            dictionary.GetDataStoreSchema(PersistentTypes);   // Pass all of your persistent object types to this method.
            AutoCreateOption autoCreateOption = AutoCreateOption.DatabaseAndSchema;  // Use AutoCreateOption.DatabaseAndSchema if the database or tables do not exist. Use AutoCreateOption.SchemaAlreadyExists if the database already exists.
            IDataStore provider = XpoDefault.GetConnectionProvider(connStr, autoCreateOption);
            return threadSafe ? (IDataLayer)new ThreadSafeDataLayer(dictionary, provider) : new SimpleDataLayer(dictionary, provider);
        }
    }
}
