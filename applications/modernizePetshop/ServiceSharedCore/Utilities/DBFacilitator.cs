using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServiceSharedCore.Utilities
{
    public static class DBFacilitator
    {
        public static List<T> GetList<T>(string connString, string npgSqlCmd, List<Tuple<string, string, NpgsqlDbType>> parameters = null)
        {
            var list = new List<T>();

            Func<NpgsqlDataReader, T> rowReader = ReflectionPopulator.GetReader<T>();

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(npgSqlCmd, conn))
                {
                    if (parameters != null && parameters.Any())
                        foreach (var parameter in parameters)
                            cmd.Parameters.AddWithValue(parameter.Item1, parameter.Item3, parameter.Item2);

                    using (var rdr = cmd.ExecuteReader())
                        while (rdr.Read())
                            list.Add(rowReader(rdr));
                }
            }
            return list;
        }
        public static T GetOne<T>(string connString, string npgSqlCmd, List<Tuple<string, string, NpgsqlDbType>> parameters = null) where T : class
        {
            return GetList<T>(connString, npgSqlCmd, parameters).FirstOrDefault();
        }
        public static int? GetInteger(string connString, string npgSqlCmd, List<Tuple<string, string, NpgsqlDbType>> parameters = null)
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(npgSqlCmd, conn))
                {
                    if (parameters != null && parameters.Any())
                        foreach (var parameter in parameters)
                            cmd.Parameters.AddWithValue(parameter.Item1, parameter.Item3, parameter.Item2);

                    using (var rdr = cmd.ExecuteReader())
                        while (rdr.Read())
                            return rdr.GetInt32(0);
                }
            }
            return null;
        }

        public static void ExecuteCommand(string connString, string npgSqlCmd, List<Tuple<string, string, NpgsqlDbType>> parameters = null) //maybe change this to return # of updated rows later
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(npgSqlCmd, conn))
                {
                    if (parameters != null && parameters.Any())
                        foreach (var parameter in parameters)
                            cmd.Parameters.AddWithValue(parameter.Item1, parameter.Item3, parameter.Item2);

                    var rdr = cmd.ExecuteNonQuery();
                }
            }
        }
    }
}