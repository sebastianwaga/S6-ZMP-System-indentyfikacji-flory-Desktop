using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualHerbarium.AdminPanel.Models;

namespace VirtualHerbarium.AdminPanel.Services.Offline
{
    public static class LocalHerbariaRepository
    {
        private static string Conn => LocalDatabaseService.Instance.ConnectionString;

        public static async Task<List<HerbariumDetailsResponse>> GetHerbariaAsync()
        {
            var list = new List<HerbariumDetailsResponse>();

            using var connection = new SqliteConnection(Conn);
            await connection.OpenAsync();

            var sql = @"
                SELECT Id, Name, Description, IsPublic, CreatedAt, UpdatedAt
                FROM Herbaria;
            ";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = sql;

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new HerbariumDetailsResponse
                {
                    id = reader.GetString(0),
                    name = reader.GetString(1),
                    description = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    @public = reader.GetInt32(3) == 1,
                    createdAt = DateTime.Parse(reader.GetString(4)),
                    updatedAt = DateTime.Parse(reader.GetString(5))
                });
            }

            return list;
        }

        public static async Task SaveHerbariaAsync(List<HerbariumDetailsResponse> herbaria)
        {
            using var connection = new SqliteConnection(Conn);
            await connection.OpenAsync();

            foreach (var h in herbaria)
            {
                var sql = @"
                    INSERT OR REPLACE INTO Herbaria
                    (Id, Name, Description, IsPublic, CreatedAt, UpdatedAt)
                    VALUES ($id, $name, $desc, $public, $createdAt, $updatedAt);
                ";

                using var cmd = connection.CreateCommand();
                cmd.CommandText = sql;

                cmd.Parameters.AddWithValue("$id", h.id);
                cmd.Parameters.AddWithValue("$name", h.name);
                cmd.Parameters.AddWithValue("$desc", h.description ?? "");
                cmd.Parameters.AddWithValue("$public", h.@public ? 1 : 0);
                cmd.Parameters.AddWithValue("$createdAt", h.createdAt.ToString("o"));
                cmd.Parameters.AddWithValue("$updatedAt", h.updatedAt.ToString("o"));

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public static async Task AddHerbariumAsync(HerbariumDetailsResponse herbarium)
        {
            using var connection = new SqliteConnection(Conn);
            await connection.OpenAsync();

            var sql = @"
                INSERT INTO Herbaria
                (Id, Name, Description, IsPublic, CreatedAt, UpdatedAt)
                VALUES ($id, $name, $desc, $public, $createdAt, $updatedAt);
            ";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = sql;

            cmd.Parameters.AddWithValue("$id", herbarium.id);
            cmd.Parameters.AddWithValue("$name", herbarium.name);
            cmd.Parameters.AddWithValue("$desc", herbarium.description ?? "");
            cmd.Parameters.AddWithValue("$public", herbarium.@public ? 1 : 0);
            cmd.Parameters.AddWithValue("$createdAt", herbarium.createdAt.ToString("o"));
            cmd.Parameters.AddWithValue("$updatedAt", herbarium.updatedAt.ToString("o"));

            await cmd.ExecuteNonQueryAsync();
        }

        public static async Task UpdateHerbariumAsync(string id, string name, string description, bool isPublic)
        {
            using var connection = new SqliteConnection(Conn);
            await connection.OpenAsync();

            var sql = @"
                UPDATE Herbaria
                SET Name = $name,
                    Description = $desc,
                    IsPublic = $public,
                    UpdatedAt = $updatedAt
                WHERE Id = $id;
            ";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = sql;

            cmd.Parameters.AddWithValue("$id", id);
            cmd.Parameters.AddWithValue("$name", name);
            cmd.Parameters.AddWithValue("$desc", description ?? "");
            cmd.Parameters.AddWithValue("$public", isPublic ? 1 : 0);
            cmd.Parameters.AddWithValue("$updatedAt", DateTime.UtcNow.ToString("o"));

            await cmd.ExecuteNonQueryAsync();
        }

        public static async Task DeleteHerbariumAsync(string id)
        {
            using var connection = new SqliteConnection(Conn);
            await connection.OpenAsync();

            var sqlPlants = "DELETE FROM Plants WHERE HerbariumId = $id";
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = sqlPlants;
                cmd.Parameters.AddWithValue("$id", id);
                await cmd.ExecuteNonQueryAsync();
            }

            var sql = "DELETE FROM Herbaria WHERE Id = $id";
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("$id", id);
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}
