using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data;
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
                {"DateTime", DateTime.SpecifyKind(mongoDoc.DateTime, DateTimeKind.Local) },
                {"Status", mongoDoc.Status }
            };

            // Insert the document into the collection
            mongoCollection.InsertOne(document);
        }

        public static async Task<List<BsonDocument>> GetDataHourlyAsync(long avConfigId, DateTime start, DateTime end, int segmentSize)
        {
            var filterBuilder = Builders<BsonDocument>.Filter;
            var builder = Builders<BsonDocument>.Filter;

            // Create a list to hold all the results
            List<BsonDocument> allResults = new();

            // Create a list of tasks to query each hourly data
            List<Task<List<BsonDocument>>> tasks = new();

            double steps = (end-start).TotalSeconds/ segmentSize;


            // Loop through each hour in the month
            for (int index = 0; index < steps; index++)
            {
                // Calculate the range for the hour
                var startOfHour = start.AddSeconds(index * segmentSize);  // Verification time + number of seconds per hour
                var endOfHour = startOfHour.AddSeconds(segmentSize);  // One hour later

                // Build the filter for the current hour range
                var filters = builder.And(new FilterDefinition<BsonDocument>[]
                {
                    builder.Eq("AvailabilityConfig", avConfigId),
                    builder.Lte("Status", "offline"),
                    builder.Gte("DateTime", startOfHour),
                    builder.Lt("DateTime", endOfHour)  // Less than the next hour
                });

                // Add the task to the list
                tasks.Add(Task.Run(async () =>
                {
                    MongoClient readClient = new(ConnectionString);
                    IMongoDatabase database = readClient.GetDatabase(DatabaseName);
                    IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>(CollectionName);

                    var queryResult = await collection.Find(filters).ToListAsync();
                    return queryResult;
                }));
            }

            // Wait for all tasks to complete
            var results = await Task.WhenAll(tasks);

            // Combine all results into a single list
            foreach (var result in results)
            {
                allResults.AddRange(result);
            }

            return allResults;
        }

        public static async Task<float> GetDisponibilityAsync(int avConfigId)
        {
            try
            {
                DateTime varificationTime;
                //TimeSpan totalTime = new();
                //Get the config
                Config? config = CamManager.Configs.Where(c => c.Id == avConfigId).FirstOrDefault();
                if (config != null)
                {
                    DateTime end = DateTime.Now;
                    DateTime start = end.AddSeconds(-config.VerificationTime);
                    List<BsonDocument> listOffline = await GetDataHourlyAsync(avConfigId, start, end, 3600);


                    float calcDisponibility = Disponibility.CalcPercentageDisponibility(listOffline, config.VerificationTime, config.PingsToOffline);
                    return calcDisponibility;
                }
                else
                    throw new Exception("Configuration not found.");

                /*MongoClient readClient = new MongoClient(ConnectionString);
                IMongoDatabase database = Client.GetDatabase(DatabaseName);
                IMongoCollection<BsonDocument> collection = mongoDatabase.GetCollection<BsonDocument>(CollectionName);


                var filterBuilder = Builders<BsonDocument>.Filter;
                var builder = Builders<BsonDocument>.Filter;
                var filters = builder.And(new FilterDefinition<BsonDocument>[]
                {
                    builder.Eq("AvailabilityConfig", avConfigId),
                    builder.Lte("Status", "offline"),
                    builder.Gte("DateTime", varificationTime)
                });



                var queryOfflineRec = collection.Find(filters);
                List<BsonDocument> listOffline = queryOfflineRec.ToList();*/
           

                /*var allFilter = builder.And(new FilterDefinition<BsonDocument>[]
                {
                    builder.Gte("DateTime", varificationTime)
                });*/


                // Retrieve all documents from the collection
                //List<BsonDocument> allDocuments = mongoCollection.Find(allFilter).ToList();
                //totalTime = Disponibility.CalculateTotalTime(allDocuments);

                //calculate offline time 
                //TimeSpan offlineTime = Disponibility.CalculateOfflineTime(listOffline);
            

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }

        public static long DocumentLastCount(long avConfigId)
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }
    }
}
