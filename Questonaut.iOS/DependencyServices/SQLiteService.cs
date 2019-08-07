using System;
using Xamarin.Forms;
using System.IO;
using Questonaut.iOS.DependencyServices;
using SQLite;
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
            var connection = new SQLiteConnection(path);
            return connection;
        }
    }
}
