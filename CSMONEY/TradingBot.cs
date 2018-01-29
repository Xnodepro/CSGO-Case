using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSMONEY
{
    class TradingBot
    {
        #region SteamTrade Objects
       
        #endregion

        #region Private fields
        private string apikey;
        private IEnumerable<Cookie> cookies;
        private List<System.Net.Cookie> cookiesAll;
        private bool isWorking;
        
        HttpClientHandler  handler = new HttpClientHandler();
        CookieContainer cookies1 = new CookieContainer();
        string Offerid = "";
        StringContent Content;
        #endregion
        public TradingBot(IEnumerable<Cookie> _cookies, List<System.Net.Cookie> _cookiesAll, string _apiKey)
        {
            cookies = _cookies;
            cookiesAll = _cookiesAll;
            apikey = _apiKey;
            StartCheck();
        }
        bool check = false;
        public bool Check {
            get { return check; }
            set {
                if (check == false)
                {
                    check = value;
                    AcceptOffer();
                }
            }
        }
        private void StartCheck()
        {
            foreach (System.Net.Cookie cookie in cookiesAll)
            {
                handler.CookieContainer.Add(cookie);
            }
            Program.Mess.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|Начал запускать запросники!");
            for (int i = 0; i < 20; i++)
            {
                new System.Threading.Thread(delegate () { GetOffer(); }).Start();
            }
            Program.Mess.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|ЗВАЕРШИЛ запускать запросники!");
        }
        private void GetOffer()
        {
            var firstFull = Convert.ToInt32(DateTime.Now.ToString("HHmmss"));
            HttpClient client = null;
            //         client.Timeout = TimeSpan.FromSeconds(90);
            client = new HttpClient(handler);
           
            while (true)
            {
                Random rand = new Random();
                Thread.Sleep(rand.Next(200, 500));
                if (Convert.ToInt32(DateTime.Now.ToString("HHmmss")) - firstFull > 100|| Check==true)
                {
                    break;
                }
                Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                try
                {
                    HttpResponseMessage response = client.GetAsync("https://api.steampowered.com/IEconService/GetTradeOffers/v1/?get_received_offers=1&active_only=1&time_historical_cutoff="+ unixTimestamp + "&key=" + apikey).Result;
                    var responseContent = response.Content;
                    string responseString = responseContent.ReadAsStringAsync().Result;
                    if (responseString != "{\n\t\"response\": {\n\n\t}\n}")
                    {
                        Program.Mess.Enqueue("" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|" + unixTimestamp.ToString() + "|Проверяю новые трейды!");
                        var da = JsonConvert.DeserializeObject<dynamic>(responseString);
                        var tradeofferid = da.response.trade_offers_received[0].tradeofferid.Value;
                        var accountid_other = da.response.trade_offers_received[0].accountid_other.Value;
                        var partner = "7656119" + (accountid_other + 7960265728).ToString();

                        StringContent content = new StringContent("sessionid" + "=" + cookies.ElementAt(0).Value
                         + "&" + "serverid" + "=" + "1"
                         + "&" + "tradeofferid" + "=" + tradeofferid
                         + "&" + "partner" + "=" + partner
                         + "&" + "captcha" + "=" + "", Encoding.UTF8, "application/x-www-form-urlencoded");
                        if (Check == false)
                        {
                            Program.Mess.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|Нашел Трейд!");
                            Content = content;
                            Offerid = tradeofferid;
                            Check = true;

                        }
                    }
                  //  else { Program.Mess.Enqueue("" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|" + unixTimestamp.ToString() + "|Проверяю новые трейды2!"); }
               //     new System.Threading.Thread(delegate () { AcceptOffer(content, tradeofferid); }).Start();


                }
                catch (Exception ex) {
                   // Program.Mess.Enqueue("1|"+ex.Message);
                }
            }
            
        }
        private void AcceptOffer()
        {
            HttpClient client = null;
            client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("User-Agent",
 "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36");
            client.DefaultRequestHeaders.Add("Accept", "*/*");
            client.DefaultRequestHeaders.Add("Accept-Language", "ru-RU,ru;q=0.8,en-US;q=0.6,en;q=0.4");
                  client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            client.DefaultRequestHeaders.Add("Origin", "https://steamcommunity.com");
            client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.Add("Referer", "https://steamcommunity.com/tradeoffer/"+ Offerid + "/");
            Program.Mess.Enqueue("" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|Отправил принятие трейда!");
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                try
                {
                Program.Mess.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|Отправил запрос на трейд!");
                HttpResponseMessage response = client.PostAsync("https://steamcommunity.com/tradeoffer/"+ Offerid + "/accept", Content).Result;
                    var responseContent = response.Content;
                    string responseString = responseContent.ReadAsStringAsync().Result;
                //        Program.Mess.Enqueue("" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|Отправил принятие трейда|"+ responseString);
                Program.Mess.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|Получил ответ от трейда!");
            }
                catch (Exception ex) {
                Program.Mess.Enqueue("2|" + ex.Message);
            }
            
        }
        //public void CheckOffers()
        //{
            
        //    for (int i = 0; i < 3; i++)
        //    {
        //        new System.Threading.Thread(delegate () {
        //            offe(i);
        //        }).Start();
        //    }
            
        //}
        //private void offe(int index)
        //{
        //    int first = api.GetAllTradeOffers().TradeOffersReceived.Count;
        //    var firstFull = Convert.ToInt32(DateTime.Now.ToString("HHmmss"));
        //    while (true)
        //    {
        //        try
        //        {
        //            if (Convert.ToInt32(DateTime.Now.ToString("HHmmss")) - firstFull > 70)
        //            {
        //                break;
        //            }
        //            var aa1 = api.GetAllTradeOffers();
        //            if(aa1.TradeOffersReceived.Count!= first)
        //            {
        //                Program.Mess.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|" + "{" + index.ToString() + "}" + "|" + aa1.TradeOffersReceived[0].TradeOfferState);
        //            }
        //            Program.Mess.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|" + "{" + index.ToString() + "}" + "|Количество Офферов:" + aa1.TradeOffersReceived.Count);
        //            foreach (var Offer in aa1.TradeOffersReceived)
        //            {
                    
        //                if (Offer.TradeOfferState == TradeOfferState.TradeOfferStateActive)
        //                {
        //                    var res = session.Accept(Offer.TradeOfferId);
        //                    Program.Mess.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|Подтвердил обмен|" + "{" + index.ToString() + "}" + "|" + res.Accepted + "|" + res.TradeError);
        //                    break;
        //                }
        //            }
        //        }
        //        catch (Exception ex) { Program.Mess.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|" + "{" + index.ToString() + "}" + "|" + ex.Message); }

        //    }
        //}
    }
}
