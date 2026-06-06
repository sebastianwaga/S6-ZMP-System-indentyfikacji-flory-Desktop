using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualHerbarium.AdminPanel.Models;

namespace VirtualHerbarium.AdminPanel.Services.Offline
{
    public static class LocalPlantsRepository
    {
        private static string Conn => LocalDatabaseService.Instance.ConnectionString;

        public static async Task<List<PlantResponse>> GetPlantsAsync()
        {
            var list = new List<PlantResponse>();

            using var connection = new SqliteConnection(Conn);
            await connection.OpenAsync();

            var sql = "SELECT Id, HerbariumId, Name, CreatedAt, UpdatedAt FROM Plants";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = sql;

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new PlantResponse
                {
                    id = reader.GetString(0),
                    herbariumId = reader.GetString(1),
                    name = reader.GetString(2),
                    createdAt = DateTime.Parse(reader.GetString(3)),
                    updatedAt = DateTime.Parse(reader.GetString(4))
                });
            }

            return list;
        }

        public static async Task SavePlantsAsync(List<PlantResponse> plants)
        {
            using var connection = new SqliteConnection(Conn);
            await connection.OpenAsync();

            foreach (var p in plants)
            {
                var sql = @"
                    INSERT OR REPLACE INTO Plants
                    (Id, HerbariumId, Name, CreatedAt, UpdatedAt)
                    VALUES ($id, $herbariumId, $name, $createdAt, $updatedAt);
                ";

                using var cmd = connection.CreateCommand();
                cmd.CommandText = sql;

                cmd.Parameters.AddWithValue("$id", p.id);
                cmd.Parameters.AddWithValue("$herbariumId", p.herbariumId);
                cmd.Parameters.AddWithValue("$name", p.name);
                cmd.Parameters.AddWithValue("$createdAt", p.createdAt.ToString("o"));
                cmd.Parameters.AddWithValue("$updatedAt", p.updatedAt.ToString("o"));

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public static async Task AddPlantAsync(PlantResponse plant)
        {
            using var connection = new SqliteConnection(Conn);
            await connection.OpenAsync();

            var sql = @"
                INSERT INTO Plants
                (Id, HerbariumId, Name, CreatedAt, UpdatedAt)
                VALUES ($id, $herbariumId, $name, $createdAt, $updatedAt);
            ";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = sql;

            cmd.Parameters.AddWithValue("$id", plant.id);
            cmd.Parameters.AddWithValue("$herbariumId", plant.herbariumId);
            cmd.Parameters.AddWithValue("$name", plant.name);
            cmd.Parameters.AddWithValue("$createdAt", plant.createdAt);
            cmd.Parameters.AddWithValue("$updatedAt", plant.updatedAt);

            await cmd.ExecuteNonQueryAsync();
        }

        public static async Task UpdatePlantAsync(string id, string newName)
        {
            using var connection = new SqliteConnection(Conn);
            await connection.OpenAsync();

            var sql = @"
                UPDATE Plants
                SET Name = $name,
                    UpdatedAt = $updatedAt
                WHERE Id = $id;
            ";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = sql;

            cmd.Parameters.AddWithValue("$id", id);
            cmd.Parameters.AddWithValue("$name", newName);
            cmd.Parameters.AddWithValue("$updatedAt", DateTime.UtcNow.ToString("o"));

            await cmd.ExecuteNonQueryAsync();
        }

        public static async Task DeletePlantAsync(string id)
        {
            using var connection = new SqliteConnection(Conn);
            await connection.OpenAsync();

            var sqlPhotos = "DELETE FROM Photos WHERE PlantId = $id";
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = sqlPhotos;
                cmd.Parameters.AddWithValue("$id", id);
                await cmd.ExecuteNonQueryAsync();
            }

            var sql = "DELETE FROM Plants WHERE Id = $id";
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("$id", id);
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}
