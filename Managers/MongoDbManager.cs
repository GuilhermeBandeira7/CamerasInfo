using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
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
        private static string DatabaseName { get; set; } = "camerasInfoDb";
        private static string CollectionName { get; set; } = "pingData";

        //Mongo DB instance
        private static MongoClient Client { get; set; } = new MongoClient(ConnectionString);
        private static IMongoDatabase mongoDatabase { get; set; } = Client.GetDatabase(DatabaseName);
        private static IMongoCollection<BsonDocument> mongoCollection { get; set; } = 
            mongoDatabase.GetCollection<BsonDocument>(CollectionName);


        public static void SaveToMongo(Ping_MongoDB mongoDoc)
        {
            // Create a document to be inserted
            var document = new BsonDocument
            {
                {"AvailabilityConfig", mongoDoc.AvailabilityConfig},
                {"Counter", mongoDoc.Counter},
                {"DateTime", DateTime.SpecifyKind(mongoDoc.DateTime, DateTimeKind.Utc) },
                {"Status", mongoDoc.Status }
            };

            // Insert the document into the collection
            mongoCollection.InsertOne(document);
        }

        public static float GetDisponibility(int avConfigId)
        {
            try
            {
                DateTime varificationTime;
                TimeSpan totalTime = new();
                //Get the config
                Config? config = CamManager.configs.Where(c => c.Id == avConfigId).FirstOrDefault();
                if (config != null)
                {
                    DateTime dateTime = DateTime.UtcNow.AddSeconds(-config.VerificationTime);
                    varificationTime = dateTime;

                }
                else
                    throw new Exception("Configuration not found.");

                var filterBuilder = Builders<BsonDocument>.Filter;
                var builder = Builders<BsonDocument>.Filter;
                var filters = builder.And(new FilterDefinition<BsonDocument>[]
                {
                    builder.Eq("AvailabilityConfig", avConfigId),
                    builder.Lte("Status", "offline"),
                    builder.Gte("DateTime", varificationTime)
                });



                var queryOfflineRec = mongoCollection.Find(filters);
                List<BsonDocument> listOffline = queryOfflineRec.ToList();

                var allFilter = builder.And(new FilterDefinition<BsonDocument>[]
                {
                    builder.Gte("DateTime", varificationTime)
                });
                // Retrieve all documents from the collection
                List<BsonDocument> allDocuments = mongoCollection.Find(allFilter).ToList();
                totalTime = Disponibility.CalculateTotalTime(allDocuments);

                //calculate offline time 
                //TimeSpan offlineTime = Disponibility.CalculateOfflineTime(listOffline);
                float calcDisponibility = Disponibility.CalcPercentageDisponibility(listOffline, totalTime, config.PingsToOffline);

                return calcDisponibility;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }

        public static long DocumentLastCount(long avConfigId)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("AvailabilityConfig", avConfigId);
            var sort = Builders<BsonDocument>.Sort.Descending("_id");
            BsonDocument document = mongoCollection.Find(filter).Sort(sort).FirstOrDefault();

            // Deserialize BsonDocument to Person object
            Ping_MongoDB? doc = document != null ? BsonSerializer.Deserialize<Ping_MongoDB>(document) : null;
            if (doc != null)
                return doc.Counter;
            return -1;
        }
    }
}
