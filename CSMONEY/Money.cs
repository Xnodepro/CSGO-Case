using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSMONEY
{
    class Money
    {
        IWebDriver driver;
        int ID = 0;

        List<System.Net.Cookie> cook;
        List<System.Net.Cookie> cookAll;
        string apiKey = Properties.Settings.Default.ApiKey;
        string ga = "";
        CookieContainer cookies = new CookieContainer();
        HttpClientHandler handler = new HttpClientHandler();
        public struct Dat
        {
            public string Event { get; set; }
            inData data { get; set; }
        }
        public struct inData
        {

            public string m { get; set; }
            public string e { get; set; }
            public double p { get; set; }
        }
        public Money()
        {

        }
       
        public void INI()
        {
            try
            {
                var driverService = ChromeDriverService.CreateDefaultService();  //скрытие 
                driverService.HideCommandPromptWindow = true;                    //консоли
                driver = new ChromeDriver(driverService);
                driver.Navigate().GoToUrl("https://cs.money/ru");


                
               
                new System.Threading.Thread(delegate () {
                    try
                    {
                        while (true)
                        {
                            try
                            {
                                Thread.Sleep(240000);
                                if (Program.AutoRefreshPage)
                                {
                                    driver.Navigate().Refresh();
                                    Program.Mess.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|Обновил страницу");
                                }

                            }
                            catch (Exception ex) { }
                        }

                    }
                    catch (Exception ex) { }
                }).Start();

                new System.Threading.Thread(delegate () { try { TimeToSaveFile(); } catch (Exception ex) { } }).Start();
                new System.Threading.Thread(delegate () {  try  {   Get();  }  catch (Exception ex) { }  }).Start();

                driver.Navigate().GoToUrl("https://cs.money/ru");
                driver.Manage().Window.Position = new Point(0, 0);

                var _cookies1 = driver.Manage().Cookies.AllCookies;
                List<KeyValuePair<string, string>> cook11 = new List<KeyValuePair<string, string>>();
                foreach (var item in _cookies1)
                {
                    cook11.Add(new KeyValuePair<string, string>(item.Name, item.Value));
                }

                WebSocket4Net.WebSocket websocket = new WebSocket4Net.WebSocket("wss://cs.money/ws", "", cook11, version: WebSocket4Net.WebSocketVersion.Rfc6455, userAgent: Properties.Settings.Default.UsetAgent);
                websocket.MessageReceived += new EventHandler<WebSocket4Net.MessageReceivedEventArgs>(websocket_Opened);

                websocket.Open();
                new System.Threading.Thread(delegate ()
                {
                    try
                    {
                        while (true)
                        {
                            try
                            {
                                if (websocket.State.ToString() == "Closing" || websocket.State.ToString() == "Closed")
                                {
                                    //driver.Navigate().Refresh();
                                    Thread.Sleep(10000);
                                    var _cookies2 = driver.Manage().Cookies.AllCookies;
                                    List<KeyValuePair<string, string>> cook12 = new List<KeyValuePair<string, string>>();
                                    foreach (var item in _cookies2)
                                    {
                                        cook12.Add(new KeyValuePair<string, string>(item.Name, item.Value));
                                    }
                                    websocket = new WebSocket4Net.WebSocket("wss://cs.money/ws", "", cook12, version: WebSocket4Net.WebSocketVersion.Rfc6455, userAgent: Properties.Settings.Default.UsetAgent);
                                    websocket.MessageReceived += new EventHandler<WebSocket4Net.MessageReceivedEventArgs>(websocket_Opened);
                                    websocket.Open();
                                    Program.Mess.Enqueue("Статус подключения к сервру:" + websocket.State.ToString());
                                }
                                Thread.Sleep(1000);
                            }
                            catch (Exception ex) { }
                        }

                    }
                    catch (Exception ex) { }
                }).Start();

            }
            catch (Exception ex) { Program.Mess.Enqueue(ex.Message); }

        }
        private void websocket_Opened(object sender, WebSocket4Net.MessageReceivedEventArgs e)
        {
            try
            {
                var da = JsonConvert.DeserializeObject<dynamic>(e.Message.Replace("event", "Event"));
                if (da.Event.Value == "add_items")
                {
                    var countAddItems = da.data.Count;
                    Program.Mess.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|Добавлено предметов на сайт:" + countAddItems);
                    ClickItem1(da);
                }
                if (da.Event.Value == "update_list_change_price")
                {
                    var countAddItems = da.data.Count;
                    Program.Mess.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|[ListChange]Изменило цену, предметов :" + countAddItems);
                    CheckItemsFromUpdate(da);
                }


            }
            catch (Exception ex)
            {
                
            }

        }
        private void Post(HttpClientHandler handler, int id, string botId, string itemsId, string wallet)
        {
            HttpClientHandler handler1 = handler;
            try
            {
                HttpClient client = null;
                client = new HttpClient(handler1);
                client.Timeout = TimeSpan.FromSeconds(60);
                client.DefaultRequestHeaders.Add("User-Agent",
                                                 Properties.Settings.Default.UsetAgent);
                client.DefaultRequestHeaders.Add("Accept", "*/*");
                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
                //Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;/* unixTimestamp.ToString() + DateTime.Now.Millisecond.ToString()*/
                StringContent content = new StringContent("{\"steamid\":\"" + botId + "\",\"peopleItems\":[],\"botItems\":[\"" + itemsId + "\"],\"onWallet\":-" + wallet + ",\"gid\":\"" + "\"}", Encoding.UTF8, "application/json");
                Program.Mess.Enqueue("БОТ[" + ID + "] " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "| Оправил Запрос1|Бот:" + botId + "|IDItems:" + itemsId + "|wallet:" + wallet);
                var response = client.PostAsync("https://cs.money/send_offer", content).Result;
                Program.Mess.Enqueue("БОТ[" + ID + "] " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "| Завершил Запрос1");
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content;
                    string responseString = responseContent.ReadAsStringAsync().Result;
                }
            }
            catch (Exception ex) { }

        }
        private void Get()
        {

            HttpClient client = null;
            
            //handler2.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            while (true)
            {
                try
                {
                    HttpClientHandler handler2 = new HttpClientHandler();
                    var _cookies = driver.Manage().Cookies.AllCookies;
                    foreach (var item in _cookies)
                    {
                        handler2.CookieContainer.Add(new System.Net.Cookie(item.Name, item.Value) { Domain = item.Domain });
                    }
                    handler2.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                    client = new HttpClient(handler2);
                    client.DefaultRequestHeaders.Add("User-Agent",
                           Properties.Settings.Default.UsetAgent);
                    client.DefaultRequestHeaders.Add("Accept", "*/*");
                    client.DefaultRequestHeaders.Add("Accept-Language", "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
                  client.DefaultRequestHeaders.Add("accept-encoding", "gzip, deflate, br");

                    DateTime foo = DateTime.Now;
                    long unixTime = ((DateTimeOffset)foo).ToUnixTimeSeconds();
                    var response = client.GetAsync("https://cs.money/load_bots_inventory?hash="+ unixTime.ToString()).Result;
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = response.Content;
                        string responseString = responseContent.ReadAsStringAsync().Result;
                        var ITEMS = JsonConvert.DeserializeObject<dynamic>(responseString);
                        ClickItem(ITEMS);
                        
                        Program.Mess.Enqueue("БОТ[" + ID + "] " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|" + "Завершил загрузку предметов." + ITEMS.Count.ToString());
                        Thread.Sleep(300000);
                    }

                    //Random random = new Random();

                    //Thread.Sleep(random.Next(50, 700));
                }
                catch (Exception ex) { }
            }
            // return new Data();
        }
        private void ClickItem(dynamic json)
        {

            try
            {
                var da = json;
                var aa = da;
                foreach (var item in da)
                {

                    var b = item.b[0].Value;
                    var id = item.id[0].Value;
                    var m = item.m.Value;
                    var p = item.p.Value;
                    string ee = "";
                    if (item.e != null)
                    {
                        try
                        {
                            ee = item.e.Value;
                        }
                        catch (Exception ex) { Program.Mess.Enqueue("БОТ[" + ID + "] " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|Ошибка1 :" + ex.Message); }
                    }
                    
                    if (ee != "")
                    {
                        string factory = "";
                        switch (ee)
                        {
                            case "MW": factory = "(MinimalWear)"; break;
                            case "FN": factory = "(FactoryNew)"; break;
                            case "FT": factory = "(Field-Tested)"; break;
                            case "BS": factory = "(Battle-Scarred)"; break;
                            case "WW": factory = "(Well-Worn)"; break;
                        }
                        m += factory;
                    }

                    var ItemFind = Program.DataMoney.Find(N => (N.Name.Replace(" ","") == m.Replace(" ", "")));
                    if (ItemFind == null)
                    {
                        DataForMoney DFM = new DataForMoney()
                        {
                            Name = m.ToString(),
                            Id = id.ToString(),
                            Bot = b.ToString(),
                            Price = p,
                            Dt = DateTime.Now
                        };

                        Program.DataMoney.Add(DFM);
                        Program.Mess.Enqueue("БОТ[" + ID + "] " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|Добавил предмет:" + DFM.Name + "|Цена:" + DFM.Price.ToString());
                    }
                    else {
                        if (ItemFind.Price > p)
                        {
                            int ItemIndex = Program.DataMoney.FindIndex(N => (N.Name.Replace(" ", "") == m.Replace(" ", "")));
                            Program.Mess.Enqueue("БОТ[" + ID + "] " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|Изменил предмет:" + Program.DataMoney[ItemIndex].Name + "|ЦенаOLD:" + Program.DataMoney[ItemIndex].Price.ToString() + "NewPrice:" + p.ToString());
                            Program.DataMoney[ItemIndex].Price = p;
                            Program.DataMoney[ItemIndex].Dt = DateTime.Now;
                            
                        }
                    }

                }
            }
            catch (Exception ex) {
                Program.Mess.Enqueue("БОТ[" + ID + "] " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|Ошибка2 :" + ex.Message);
            }
        }
        private void ClickItem1(dynamic json)
        {
            try
            {
                var da = json;
                var aa = da.Event.Value;
                foreach (var item in da.data)
                {

                    var b = item.b[0].Value;
                    var id = item.id[0].Value;
                    var m = item.m.Value;
                    var p = item.p.Value;
                    string ee = "";
                    if (item.e != null)
                    {
                        try
                        {
                            ee = item.e.Value;
                        }
                        catch (Exception ex) { Program.Mess.Enqueue("БОТ[" + ID + "] " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|Ошибка1 :" + ex.Message); }
                    }

                    if (ee != "")
                    {
                        string factory = "";
                        switch (ee)
                        {
                            case "MW": factory = "(MinimalWear)"; break;
                            case "FN": factory = "(FactoryNew)"; break;
                            case "FT": factory = "(Field-Tested)"; break;
                            case "BS": factory = "(Battle-Scarred)"; break;
                            case "WW": factory = "(Well-Worn)"; break;
                        }
                        m += factory;
                    }

                    var ItemFind = Program.DataMoney.Find(N => (N.Name.Replace(" ", "") == m.Replace(" ", "")));
                    if (ItemFind == null)
                    {
                        DataForMoney DFM = new DataForMoney()
                        {
                            Name = m.ToString(),
                            Id = id.ToString(),
                            Bot = b.ToString(),
                            Price = p,
                            Dt = DateTime.Now
                        };

                        Program.DataMoney.Add(DFM);
                        Program.Mess.Enqueue("БОТ[" + ID + "] " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|Добавил предмет:" + DFM.Name + "|Цена:" + DFM.Price.ToString());
                    }
                    else
                    {
                        if (ItemFind.Price != p)
                        {
                            int ItemIndex = Program.DataMoney.FindIndex(N => (N.Name.Replace(" ", "") == m.Replace(" ", "")));
                            Program.Mess.Enqueue("БОТ[" + ID + "] " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|Изменил предмет:" + Program.DataMoney[ItemIndex].Name + "|ЦенаOLD:" + Program.DataMoney[ItemIndex].Price.ToString() + "NewPrice:" + p.ToString());
                            Program.DataMoney[ItemIndex].Price = p;
                            Program.DataMoney[ItemIndex].Dt = DateTime.Now;
                            
                        }
                    }


                }
            }
            catch (Exception ex) { Program.Mess.Enqueue("БОТ[" + ID + "] " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|Ошибка2 :" + ex.Message); }

        }
        private void CheckItemsFromUpdate(dynamic json)
        {
            try
            {
                var da = json;
                var aa = da.Event.Value;
                foreach (var item in da.data)
                {

                    try
                    {
                        var name = item.Name.ToString();
                        var price = item.First.price.Value;

                        int ItemIndex = Program.DataMoney.FindIndex(N => (N.Name.Replace(" ", "") == name.Replace(" ", "")));
                        Program.Mess.Enqueue("БОТ[" + ID + "] " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|[ListChange]Изменил предмет:" + Program.DataMoney[ItemIndex].Name + "|ЦенаOLD:" + Program.DataMoney[ItemIndex].Price.ToString() + "NewPrice:" + price.ToString());
                        Program.DataMoney[ItemIndex].Price = price;
                        Program.DataMoney[ItemIndex].Dt = DateTime.Now;
                    }
                    catch (Exception ex)
                    {
                        Program.Mess.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|[ListChange] :" + ex.Message);
                    }


                }
            }
            catch (Exception ex) {   DateTime.Now.ToString(("yyyy-MM-dd HH:mm:ss:fff") + "|Ошибка2 :" + ex.Message); }

        }
        private void SetListBadPrice(string _Data, string _Site, string _Name, string _OldPrice, string _NewPrice)
        {
            Program.DataViewBadPrice item = new Program.DataViewBadPrice()
            {
                Date = _Data,
                Site = _Site,
                Name = _Name,
                OldPrice = _OldPrice,
                NewPrice = _NewPrice
            };
            Program.BadPrice.Add(item);
        }
        private void TimeToSaveFile()
        {
            while (true)
            {
                try
                {
                    Thread.Sleep(100000);
                    string json = JsonConvert.SerializeObject(Program.DataMoney);
                    File.WriteAllText("dataBaseMoney.txt", json);
                }
                catch (Exception ex) { }
            }
            
        }
    }
}
