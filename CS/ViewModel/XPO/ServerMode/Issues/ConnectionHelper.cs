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
            // Uncomment this line if you use a database server like SQL Server, Oracle, PostgreSql etc.
            //connStr = XpoDefault.GetConnectionPoolString(connStr);
            ReflectionDictionary dictionary = new ReflectionDictionary();
            // Pass all of your persistent object types to this method.
            dictionary.GetDataStoreSchema(PersistentTypes);
            // Use AutoCreateOption.DatabaseAndSchema if the database or tables do not exist.
            // Use AutoCreateOption.SchemaAlreadyExists if the database already exists.
            AutoCreateOption autoCreateOption = AutoCreateOption.DatabaseAndSchema;
            IDataStore provider = XpoDefault.GetConnectionProvider(connStr, autoCreateOption);
            return threadSafe ? (IDataLayer)new ThreadSafeDataLayer(dictionary, provider) : new SimpleDataLayer(dictionary, provider);
        }
    }
}
