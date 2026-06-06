using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using VirtualHerbarium.AdminPanel.Models;

namespace VirtualHerbarium.AdminPanel.Services
{
    public class LocalDatabaseService
    {
        public static LocalDatabaseService Instance { get; } = new LocalDatabaseService();

        private readonly string _connectionString;
        public string ConnectionString => _connectionString;

        private LocalDatabaseService()
        {
            var dbPath = LocalDatabasePath.GetDatabasePath();
            _connectionString = $"Data Source={dbPath};";
            EnsureDatabase();
        }

        private void EnsureDatabase()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            CreateTables(connection);
        }

        private void CreateTables(SqliteConnection connection)
        {
            var createHerbaria = @"
                CREATE TABLE IF NOT EXISTS Herbaria (
                    Id TEXT PRIMARY KEY,
                    Name TEXT NOT NULL,
                    Description TEXT,
                    IsPublic INTEGER NOT NULL,
                    CreatedAt TEXT NOT NULL,
                    UpdatedAt TEXT NOT NULL
                );
            ";

            var createPlants = @"
                CREATE TABLE IF NOT EXISTS Plants (
                    Id TEXT PRIMARY KEY,
                    HerbariumId TEXT NOT NULL,
                    Name TEXT NOT NULL,
                    CreatedAt TEXT NOT NULL,
                    UpdatedAt TEXT NOT NULL
                );
            ";

            var createPhotos = @"
                CREATE TABLE IF NOT EXISTS Photos (
                    Id TEXT PRIMARY KEY,
                    PlantId TEXT NOT NULL,
                    Description TEXT,
                    Url TEXT NOT NULL,
                    CreatedAt TEXT NOT NULL,
                    UpdatedAt TEXT NOT NULL
                );
            ";

            var createSyncQueue = @"
                CREATE TABLE IF NOT EXISTS SyncQueue (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ActionType TEXT NOT NULL,
                    EntityType TEXT NOT NULL,
                    EntityId TEXT NOT NULL,
                    Payload TEXT NOT NULL,
                    BaseUpdatedAt TEXT,
                    RetryCount INTEGER NOT NULL DEFAULT 0,
                    Sent INTEGER NOT NULL DEFAULT 0,
                    CreatedAt TEXT NOT NULL
                );
            ";

            var createMetadata = @"
                CREATE TABLE IF NOT EXISTS Metadata (
                    Key TEXT PRIMARY KEY,
                    Value TEXT
                );
            ";

            ExecuteNonQuery(connection, createHerbaria);
            ExecuteNonQuery(connection, createPlants);
            ExecuteNonQuery(connection, createPhotos);
            ExecuteNonQuery(connection, createSyncQueue);
            ExecuteNonQuery(connection, createMetadata);

            var createIndexes = @"
                CREATE INDEX IF NOT EXISTS idx_plants_herbarium ON Plants(HerbariumId);
                CREATE INDEX IF NOT EXISTS idx_photos_plant ON Photos(PlantId);
            ";

            ExecuteNonQuery(connection, createIndexes);
        }

        private void ExecuteNonQuery(SqliteConnection connection, string sql)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        public void AddToSyncQueue(string actionType, string entityType, string entityId, string payload, string? baseUpdatedAt)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var sql = @"
                INSERT INTO SyncQueue (
                    ActionType, EntityType, EntityId,
                    Payload, BaseUpdatedAt, CreatedAt,
                    Sent, RetryCount
                )
                VALUES (
                    $actionType, $entityType, $entityId,
                    $payload, $baseUpdatedAt, $createdAt,
                    0, 0
                );
            ";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = sql;

            cmd.Parameters.AddWithValue("$actionType", actionType);
            cmd.Parameters.AddWithValue("$entityType", entityType);
            cmd.Parameters.AddWithValue("$entityId", entityId);
            cmd.Parameters.AddWithValue("$payload", payload);

            if (baseUpdatedAt == null)
                cmd.Parameters.AddWithValue("$baseUpdatedAt", DBNull.Value);
            else
                cmd.Parameters.AddWithValue("$baseUpdatedAt", baseUpdatedAt);

            cmd.Parameters.AddWithValue("$createdAt", DateTime.UtcNow.ToString("o"));

            cmd.ExecuteNonQuery();
        }

        public List<SyncQueueItem> GetPendingQueueItems()
        {
            var list = new List<SyncQueueItem>();

            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var sql = @"
                SELECT Id, ActionType, EntityType, EntityId, Payload, BaseUpdatedAt, RetryCount, Sent, CreatedAt
                FROM SyncQueue
                WHERE Sent = 0
                ORDER BY CreatedAt ASC;
            ";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = sql;

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new SyncQueueItem
                {
                    Id = reader.GetInt32(0),
                    ActionType = reader.GetString(1),
                    EntityType = reader.GetString(2),
                    EntityId = reader.GetString(3),
                    Payload = reader.GetString(4),
                    BaseUpdatedAt = reader.IsDBNull(5) ? null : reader.GetString(5),
                    RetryCount = reader.GetInt32(6),
                    Sent = reader.GetInt32(7) == 1,
                    CreatedAt = DateTime.Parse(reader.GetString(8))
                });
            }

            return list;
        }

        public void MarkQueueItemAsSent(int id)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var sql = "UPDATE SyncQueue SET Sent = 1 WHERE Id = $id";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();
        }

        public void IncrementRetry(int id)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var sql = "UPDATE SyncQueue SET RetryCount = RetryCount + 1 WHERE Id = $id";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();
        }

        public DateTime GetLastSyncTime()
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var sql = "SELECT Value FROM Metadata WHERE Key = 'LastSyncTime'";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = sql;

            var result = cmd.ExecuteScalar() as string;

            if (string.IsNullOrEmpty(result))
                return DateTime.MinValue;

            return DateTime.Parse(result);
        }

        public void UpdateLastSyncTime(DateTime time)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var sql = @"
                INSERT OR REPLACE INTO Metadata (Key, Value)
                VALUES ('LastSyncTime', $value);
            ";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("$value", time.ToString("o"));
            cmd.ExecuteNonQuery();
        }
    }
}
