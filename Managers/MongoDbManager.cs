using CamerasInfo.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CamerasInfo.Managers
{
    public static class MongoDbManager
    {
        private static string ConnectionString { get; set; } = "mongodb://localhost:27017";
        private static string DatabaseName { get; set; } = "test";
        private static string CollectionName { get; set; } = "data";

        public static void SaveToMongo(string ip, string time, bool success)
        {
            var client = new MongoClient(ConnectionString);
            // Get a reference to the database
            var database = client.GetDatabase(DatabaseName);
            // Get a reference to the collection
            var collection = database.GetCollection<BsonDocument>(CollectionName);

            // Create a document to be inserted
            var document = new BsonDocument
            {
                {"ip", ip},
                {"StarUpTime", time},
                {"isSuccess", success }
            };

            // Insert the document into the collection
            collection.InsertOne(document);

            //Console.WriteLine("Document inserted successfully.");

            // Close the connection
            client = null;
        }
    }
}
