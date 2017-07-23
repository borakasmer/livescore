using LiveScore.Models;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LiveScore.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            var model = new List<MatchData>();
            if (hasDataOnRedis() == false)
            {
                model = FillData();
                SaveRedis(model);
            }
            else
            {
                model = getDataFromRedis();
            }
            return View(model);
        }

        protected List<MatchData> FillData()
        {
            return new List<MatchData>()
                {
                    new MatchData(){ID=1,Team1Flag="spritesFB", Team1Name="FENERBAHÇE",Team2Flag="spritesGS",Team2Name="GALATASARAY",Team1Score=0,Team2Score=0 },
                    new MatchData(){ID=2,Team1Flag="spritesBJK", Team1Name="BEŞİKTAŞ",Team2Flag="spritesBRS",Team2Name="BURSA",Team1Score=0,Team2Score=0 },
                    new MatchData(){ID=3,Team1Flag="spritesGZT", Team1Name="GAZİANTEP",Team2Flag="spritesDNZ",Team2Name="DENİZLİ",Team1Score=0,Team2Score=0 },
                    new MatchData(){ID=4,Team1Flag="spritesELZ", Team1Name="ELAZIĞ",Team2Flag="spritesGRS",Team2Name="GİRESUN",Team1Score=0,Team2Score=0 }
                };
        }
        protected void SaveRedis(List<MatchData> datas)
        {
            #region conf
            var conf = new RedisEndpoint() { Host = "xxxxxxxxxxxxxx.redis.cache.windows.net", Password = "yyyyyyyyyyyyyyyyyyyyyyyyyy", Ssl = true, Port = 6380 };
            #endregion
            using (IRedisClient client = new RedisClient(conf))
            {
                if (client.SearchKeys("flag*").Count == 0)
                {
                    foreach (MatchData data in datas)
                    {
                        var matchClient = client.As<MatchData>();
                        matchClient.SetValue("flag" + data.ID, data);
                    }
                }
            }
        }
        protected List<MatchData> getDataFromRedis()
        {
            List<MatchData> dataList = new List<MatchData>();
            #region conf
            var conf = new RedisEndpoint() { Host = "****************.redis.cache.windows.net", Password = "******************", Ssl = true, Port = 6380 };
            #endregion
            using (IRedisClient client = new RedisClient(conf))
            {
                List<string> allKeys = client.SearchKeys("flag*");
                foreach (string key in allKeys)
                {
                    dataList.Add(client.Get<MatchData>(key));
                }
                return dataList.OrderBy(res => res.ID).ToList();
            }
        }
        protected bool hasDataOnRedis()
        {
            #region conf
            var conf = new RedisEndpoint() { Host = "*************************.redis.cache.windows.net", Password = "**********", Ssl = true, Port = 6380 };
            #endregion
            using (IRedisClient client = new RedisClient(conf))
            {
                if (client.SearchKeys("flag*").Count == 0)
                {
                    return false;
                }
                return true;
            }
        }
        public ActionResult Admin()
        {
            var model = new List<MatchData>();
            if (hasDataOnRedis() == false)
            {
                model = FillData();
                SaveRedis(model);
            }
            else
            {
                model = getDataFromRedis();
            }
            return View(model);
        }
    }
    public class Match : Hub
    {
        //static RedisEndpoint conf = new RedisEndpoint() { Host = "127.0.0.1", Port = 6379 };
        //public override async Task OnConnected()
        //{
        //    await Clients.Caller.notifyConnect(Context.ConnectionId + ":" + DateTime.Now);
        //}

        public async Task NotifyClients(List<NotifyData> data)
        {
             #region conf
            var conf = new RedisEndpoint() { Host = "**************.redis.cache.windows.net", Password = "********************", Ssl = true, Port = 6380 };
            #endregion
            using (IRedisClient client = new RedisClient(conf))
            {
                //Update Redis
                foreach (NotifyData dt in data)
                {
                    MatchData changeData = client.Get<MatchData>("flag" + dt.No);
                    if(changeData.Team1Flag.Contains(dt.ID.Replace("txt", "")))
                    {
                        changeData.Team1Score = dt.Score;
                        client.Set<MatchData>("flag" + dt.No,changeData);
                    }
                    else
                    {
                        changeData.Team2Score = dt.Score;
                        client.Set<MatchData>("flag" + dt.No, changeData);
                    }
                }
                //-------------------

                //Send MicroService
                string jsonData = JsonConvert.SerializeObject(data);
                client.PublishMessage("SqlSync", jsonData);
            }
            await Clients.Others.updateScore(data);
        }
    }

    public class NotifyData
    {
        public int Score { get; set; }
        public string ID { get; set; }

        public int No { get; set; }
    }
}