using SQLite.Net;

namespace Questonaut.DependencyServices
{
    /// <summary>
    /// Dependcy service for the sqlite database.
    /// </summary>
    public interface ISQLiteService
    {
        SQLiteConnection GetSQLiteConnection();
    }
}
