using System.IO;
using Questonaut.DependencyServices;
using Questonaut.Droid.DependencyServices;
using SQLite.Net;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLiteService))]
namespace Questonaut.Droid.DependencyServices
{
    public class SQLiteService : ISQLiteService
    {
        public SQLiteService()
        {
        }
        public SQLiteConnection GetSQLiteConnection()
        {
            var fileName = "Questonaut.db3";
            var documentPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var path = Path.Combine(documentPath, fileName);
            var platform = new SQLite.Net.Platform.XamarinAndroid.SQLitePlatformAndroid();
            var connection = new SQLiteConnection(platform, path);
            return connection;
        }
    }
}
