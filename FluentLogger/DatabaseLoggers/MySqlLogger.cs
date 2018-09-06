using System;
using System.Data;

namespace FluentLogger
{
    public class MySqlLogger : DbLogger
    {
        public MySqlLogger(IDbConnection connection, LogLevel minLevel) : base(connection, minLevel)
        {
        }

        public override void CreateTable()
        {
            var sql = @"CREATE TABLE `errors` (
                          `id` varchar(39) NOT NULL DEFAULT '',
                          `level` varchar(15) NOT NULL DEFAULT '',
                          `message` varchar(600) DEFAULT NULL,
                          `stacktrace` varchar(1000) DEFAULT NULL,
                          `objects` text,
                          `timestamp` datetime NOT NULL,
                          PRIMARY KEY(`Id`)
                        ) ENGINE = InnoDB DEFAULT CHARSET = latin1; ";
            var cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        public override bool TableExists()
        {
            try
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"SELECT 1 FROM testtable LIMIT 1";
                cmd.ExecuteScalar();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
