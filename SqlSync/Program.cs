using DAL;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SqlSync.DataModel;

namespace SqlSync
{
    class Program
    {
        static void Main(string[] args)
        {
            #region config
            var conf = new RedisEndpoint() { Host = "******************.redis.cache.windows.net", Password = "*******************", Ssl = true, Port = 6380 };
            #endregion
            using (IRedisClient client = new RedisClient(conf))
            {
                IRedisSubscription sub = null;
                using (sub = client.CreateSubscription())
                {
                    sub.OnMessage += (channel, message) =>
                    {
                        try
                        {
                            List<NotifyData> dataList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<NotifyData>>(message);
                            foreach (NotifyData data in dataList)
                            {
                                //Update Sql
                                UpdateData(data);
                                Console.WriteLine(data.ID + ":" + data.Score);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    };
                }
                sub.SubscribeToChannels(new string[] { "SqlSync" });
            }
            Console.ReadLine();
        }
        protected static void UpdateData(NotifyData data)
        {
            using (LiveScoreContext dbContext = new LiveScoreContext())
            {
                LiveScore score = dbContext.LiveScore.Where(ls => ls.Css == data.ID.Replace("txt","")).FirstOrDefault();
                //LiveScore score = dbContext.LiveScore.Where(ls => ls.ID == data.No).FirstOrDefault();
                score.Score = data.Score;
                dbContext.SaveChanges();
            }
        }
    }
}
