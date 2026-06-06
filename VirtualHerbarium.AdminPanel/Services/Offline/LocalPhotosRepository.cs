using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualHerbarium.AdminPanel.Models;

namespace VirtualHerbarium.AdminPanel.Services.Offline
{
    public static class LocalPhotosRepository
    {
        private static string Conn => LocalDatabaseService.Instance.ConnectionString;

        public static async Task<List<PlantPhotoResponse>> GetPhotosAsync(string plantId)
        {
            var list = new List<PlantPhotoResponse>();

            using var connection = new SqliteConnection(Conn);
            await connection.OpenAsync();

            var sql = @"
                SELECT Id, PlantId, Description, Url, CreatedAt, UpdatedAt
                FROM Photos
                WHERE PlantId = $plantId;
            ";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("$plantId", plantId);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new PlantPhotoResponse
                {
                    id = reader.GetString(0),
                    plantId = reader.GetString(1),
                    description = reader.IsDBNull(2) ? null : reader.GetString(2),
                    url = reader.GetString(3),
                    createdAt = DateTime.Parse(reader.GetString(4)),
                    updatedAt = DateTime.Parse(reader.GetString(5))
                });
            }

            return list;
        }

        public static async Task SavePhotosAsync(List<PlantPhotoResponse> photos)
        {
            using var connection = new SqliteConnection(Conn);
            await connection.OpenAsync();

            foreach (var p in photos)
            {
                var sql = @"
                    INSERT OR REPLACE INTO Photos
                    (Id, PlantId, Description, Url, CreatedAt, UpdatedAt)
                    VALUES ($id, $plantId, $desc, $url, $createdAt, $updatedAt);
                ";

                using var cmd = connection.CreateCommand();
                cmd.CommandText = sql;

                cmd.Parameters.AddWithValue("$id", p.id);
                cmd.Parameters.AddWithValue("$plantId", p.plantId);
                cmd.Parameters.AddWithValue("$desc", p.description ?? "");
                cmd.Parameters.AddWithValue("$url", p.url);
                cmd.Parameters.AddWithValue("$createdAt", p.createdAt.ToString("o"));
                cmd.Parameters.AddWithValue("$updatedAt", p.updatedAt.ToString("o"));

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public static async Task AddPhotoAsync(PlantPhotoResponse photo)
        {
            using var connection = new SqliteConnection(Conn);
            await connection.OpenAsync();

            var sql = @"
                INSERT INTO Photos
                (Id, PlantId, Description, Url, CreatedAt, UpdatedAt)
                VALUES ($id, $plantId, $desc, $url, $createdAt, $updatedAt);
            ";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = sql;

            cmd.Parameters.AddWithValue("$id", photo.id);
            cmd.Parameters.AddWithValue("$plantId", photo.plantId);
            cmd.Parameters.AddWithValue("$desc", photo.description ?? "");
            cmd.Parameters.AddWithValue("$url", photo.url);
            cmd.Parameters.AddWithValue("$createdAt", photo.createdAt.ToString("o"));
            cmd.Parameters.AddWithValue("$updatedAt", photo.updatedAt.ToString("o"));

            await cmd.ExecuteNonQueryAsync();
        }

        public static async Task UpdatePhotoDescriptionAsync(string photoId, string newDescription)
        {
            using var connection = new SqliteConnection(Conn);
            await connection.OpenAsync();

            var sql = @"
                UPDATE Photos
                SET Description = $desc,
                    UpdatedAt = $updatedAt
                WHERE Id = $id;
            ";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = sql;

            cmd.Parameters.AddWithValue("$id", photoId);
            cmd.Parameters.AddWithValue("$desc", newDescription);
            cmd.Parameters.AddWithValue("$updatedAt", DateTime.UtcNow.ToString("o"));

            await cmd.ExecuteNonQueryAsync();
        }

        public static async Task MovePhotoAsync(string photoId, string newPlantId)
        {
            using var connection = new SqliteConnection(Conn);
            await connection.OpenAsync();

            var sql = @"
                UPDATE Photos
                SET PlantId = $plantId,
                    UpdatedAt = $updatedAt
                WHERE Id = $id;
            ";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = sql;

            cmd.Parameters.AddWithValue("$id", photoId);
            cmd.Parameters.AddWithValue("$plantId", newPlantId);
            cmd.Parameters.AddWithValue("$updatedAt", DateTime.UtcNow.ToString("o"));

            await cmd.ExecuteNonQueryAsync();
        }

        public static async Task DeletePhotoAsync(string photoId)
        {
            using var connection = new SqliteConnection(Conn);
            await connection.OpenAsync();

            var sql = "DELETE FROM Photos WHERE Id = $id";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("$id", photoId);

            await cmd.ExecuteNonQueryAsync();
        }
    }
}
