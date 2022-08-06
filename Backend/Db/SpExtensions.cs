using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Db
{
    public static class SpExtensions
    {
        public static int ExcecuteScalar(this Db.BgDbContext db, string commandText, (string name, string value) para)
        {
            object result;
            using var connection = (SqlConnection)db.Database.GetDbConnection();
            var command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = commandText;
            command.Parameters.AddWithValue(para.name, para.value);
            connection.Open();
            result = command.ExecuteScalar();
            connection.Close(); // should it be closed?
            command.Dispose();
            return (int)((long)result);// cast directly to int crashes.
        }
    }
}
