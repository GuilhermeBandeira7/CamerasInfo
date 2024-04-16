using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CamerasInfo
{
    public static class Disponibility
    {
        public static float CalcPercentageDisponibility(List<BsonDocument> mongoRecords, TimeSpan totalTime, int pingsToOffline)
        {
            List < Ping_MongoDB > l = new();
            foreach (var document in mongoRecords)
                l.Add(BsonSerializer.Deserialize<Ping_MongoDB>(document));

            List<List<Ping_MongoDB>> l2 = new();
            foreach (var ping in l)
            {
                if (l2.Count == 0)
                    l2.Add(new List<Ping_MongoDB>() { ping });
                else
                {
                    if (ping.Counter - l2.Last().Last().Counter == 1)
                        l2.Last().Add(ping);
                    else
                        l2.Add(new List<Ping_MongoDB>() { ping });
                }
            }

            TimeSpan configTimeSpanPinging = new(0);

            foreach (var lping in l2)
            {
                if (lping.Last().Counter - lping.First().Counter >= pingsToOffline)
                    configTimeSpanPinging = configTimeSpanPinging.Add(lping.Last().DateTime - lping.First().DateTime);

            }

            return ReturnDisponibility(configTimeSpanPinging, totalTime);
        }

        public static TimeSpan CalculateTotalTime(List<BsonDocument> bsonElements)
        {
            string initial = $"{bsonElements.First()["DateTime"]}";
            string final = $"{bsonElements.Last()["DateTime"]}";

            DateTime initialTime = DateTime.Parse(initial);
            DateTime finalTime = DateTime.Parse(final);
            TimeSpan timeOffline = finalTime - initialTime;
            return timeOffline;
        }

        private static float ReturnDisponibility(TimeSpan tOffline, TimeSpan totalTime)
        {
            float offlineFraction = (float)tOffline.TotalSeconds / (float)totalTime.TotalSeconds;
            float onlinePercentage = 100 - offlineFraction;

            return onlinePercentage;
        }
    }
}
