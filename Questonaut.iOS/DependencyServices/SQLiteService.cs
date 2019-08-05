using System;
using Xamarin.Forms;
using System.IO;
using Questonaut.iOS.DependencyServices;
using SQLite.Net;
using Questonaut.DependencyServices;

[assembly: Dependency(typeof(SQLiteService))]
namespace Questonaut.iOS.DependencyServices
{
    public class SQLiteService : ISQLiteService
    {
        public SQLiteConnection GetSQLiteConnection()
        {
            var fileName = "Questonaut.db3";
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var libraryPath = Path.Combine(documentsPath, "..", "Library");
            var path = Path.Combine(libraryPath, fileName);
            var platform = new SQLite.Net.Platform.XamarinIOS.SQLitePlatformIOS();
            var connection = new SQLite.Net.SQLiteConnection(platform, path);
            return connection;
        }
    }
}
