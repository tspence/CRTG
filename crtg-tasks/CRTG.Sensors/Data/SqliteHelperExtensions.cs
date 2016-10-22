using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace CRTG.Sensors.Data
{
    public static class SqliteHelperExtensions
    {
        /// <summary>
        /// Detect whether a particular type of thing exists
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="schema_type"></param>
        /// <param name="schema_name"></param>
        /// <returns></returns>
        private static bool SchemaExists(SQLiteConnection conn, string schema_type, string schema_name)
        {
            int count = conn.Query<int>(@"SELECT COUNT(1) FROM sqlite_master WHERE type=@schematype AND name=@schemaname;",
                new { schematype = schema_type, schemaname = schema_name }).FirstOrDefault();
            return (count == 1);
        }

        /// <summary>
        /// Test if a schema object exists and create it if it does not
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="schematype"></param>
        /// <param name="schemaname"></param>
        /// <param name="schema_script"></param>
        public static void CreateIfNotExists(this SQLiteConnection conn, string schematype, string schemaname, string schema_script)
        {
            if (!SchemaExists(conn, schematype, schemaname)) {
                conn.Execute(schema_script);
            }
        }
    }
}
