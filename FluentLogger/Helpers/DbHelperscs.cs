using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace FluentLogger.Helpers
{
    public static class DbHelperscs
    {

        public static void AddParameter(this IDbCommand cmd, string parameter, object value)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = parameter;
            p.Value = value;

        }
    }
}
