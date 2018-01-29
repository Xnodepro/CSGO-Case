using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Windows.Forms;
using System.Threading;
using HtmlAgilityPack;
using System.Drawing;
using System.Data;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;
using WebSocketSharp;
using System.IO;
using System.Web;

namespace CSMONEY
{
    class Work
    {
        IWebDriver driver;
        int ID = 0;

        List<string> me = new List<string>();
        List<ForItemsUSer> them = new List<ForItemsUSer>();
        DatUser ItemsUser;
        Dat ItemsBot;
        string apiKey = Properties.Settings.Default.ApiKey;

        CookieContainer cookies = new CookieContainer();
        HttpClientHandler handler = new HttpClientHandler();
        #region botInventoryStruc
        public struct Dat
        {
            public bool cached { get; set; }
            public bool success { get; set; }
            public string reason { get; set; }
            public List<ite> marketItems { get; set; }
        }
        public struct ite
        {

            public string assetID { get; set; }
          //  public int bot { get; set; }
            public double price { get; set; }
         //   public List<it> items { get; set; }
            public string itemURLName { get; set; }
        }
       

        #endregion
        #region ForItems
        public struct ForItemsUSer
        {
            public string id { get; set; }
            public int count { get; set; }
        }
        #endregion
        #region UserInventoryStruc
        public struct DatUser
        {
            public List<iteUser> items { get; set; }
        }
        public struct iteUser
        {

            public string name { get; set; }
            public double price { get; set; }
            public List<itUser> items { get; set; }
            public string id { get; set; }
        }
        public struct itUser
        {
            public string id { get; set; }

        }
        #endregion

        public Work(int id)
        {
            ID = id;
        }

        public void INI()
        {
            try
            {
                var driverService = ChromeDriverService.CreateDefaultService();  //скрытие 
                driverService.HideCommandPromptWindow = true;                    //консоли
                driver = new ChromeDriver(driverService);
                driver.Navigate().GoToUrl("https://csgo-case.com/market/");
                MessageBox.Show("Введите все данные , после этого программа продолжит работу!");

                var _cookies = driver.Manage().Cookies.AllCookies;
                foreach (var item in _cookies)
                {
                    handler.CookieContainer.Add(new System.Net.Cookie(item.Name, item.Value) { Domain = item.Domain });
                }
                //Запуск запросов на отслеживания инвентаря ботов
                for (int i = 0; i < Program.threadCount; i++)
                {
                    new System.Threading.Thread(delegate ()
                    {
                        try
                        {
                            Get(handler);
                        }
                        catch (Exception ex) { }
                    }).Start();
                }
                     
                
                new System.Threading.Thread(delegate () {
                        while (true)
                        {
                            try
                            {
                                Thread.Sleep(2000);
                                driver.Navigate().Refresh();
                                Thread.Sleep(300000);
                            }
                            catch (Exception ex) { }

                        }
                    }).Start();
                
            }
            catch (Exception ex) { Program.Mess.Enqueue(ex.Message); }

        }

        public HttpClient Prox(HttpClient client1, HttpClientHandler handler, string paroxyu)
        {

            HttpClient client = client1;
            try
            {
                string
                httpUserName = "webminidead",
                httpPassword = "159357Qq";
                string proxyUri = paroxyu;
                NetworkCredential proxyCreds = new NetworkCredential(
                    httpUserName,
                    httpPassword
                );
                WebProxy proxy = new WebProxy(proxyUri, false)
                {
                    UseDefaultCredentials = false,
                    Credentials = proxyCreds,
                };
                try
                {
                    handler.Proxy = null;
                    handler.Proxy = proxy;
                    handler.PreAuthenticate = true;
                    handler.UseDefaultCredentials = false;
                    handler.Credentials = new NetworkCredential(httpUserName, httpPassword);
                    handler.AllowAutoRedirect = true;
                }
                catch (Exception ex) { }
                client = new HttpClient(handler);
            }
            catch (Exception ex) { }
            return client;
        }
        private void Get(HttpClientHandler handler)
        {
            HttpClientHandler handler1 = handler;

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

                    HttpClient client = null;
                    client = new HttpClient(handler2);
                    client.Timeout = TimeSpan.FromSeconds(30);
                    
                    client.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36");
                    client.DefaultRequestHeaders.Add("Accept", "application/json, text/javascript, */*; q=0.01");
                    client.DefaultRequestHeaders.Add("Accept-Language", "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");


                    var response = client.GetAsync("https://csgo-case.com/data/PHP/nocache/market/get_market_stacks.php").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = response.Content;
                        string responseString = responseContent.ReadAsStringAsync().Result;
                        var ITEMS = JsonConvert.DeserializeObject<Dat>(responseString);
                        ItemsBot = ITEMS;
                        //Program.DataJar = ITEMS;
                        ClickItem(ITEMS);
                        Program.Mess.Enqueue("" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|" + "Завершил загрузку предметов:" + ITEMS.marketItems.Count);
                    }

                    Thread.Sleep(1000);
                }
                catch (Exception ex) { Program.Mess.Enqueue("" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|"  + ex.Message); }
            }
            // return new Data();
        }
       
      
     
        private bool ClickItem(Dat json)
        {

            try
            {
                var da = json;
                foreach (var item in da.marketItems)
                {

                    foreach (var name in Program.Data)
                    {
                        string _name = HttpUtility.UrlDecode(item.itemURLName);
                        if (_name.Replace(" ", "") == (name.Name).Replace(" ", ""))
                        {
                            Program.Mess.Enqueue("БОТ[" + ID + "] " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "| Нашел предмет :" + _name + "|Цена_Сайта:" + item.price + "|Цена_Наша:" + name.Price);
                            if (item.price <= name.Price)
                            {
                                string SRF = "";
                                Program.Mess.Enqueue("БОТ[" + ID + "] " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "| Оправил Запрос |" );
                                var _cookies = driver.Manage().Cookies.AllCookies;
                                foreach (var kok in _cookies)
                                {
                                    if (kok.Name == "loggedInCSRFString")
                                    {
                                        SRF = kok.Value;
                                        break;
                                    }
                                  
                                }
                                IJavaScriptExecutor js = driver as IJavaScriptExecutor;
                                string query = "var xhr = new XMLHttpRequest();var body = \"buyAssID="+ item.assetID+"&CSRFString="+ HttpUtility.UrlEncode( SRF )+ "\"; xhr.open(\"POST\", 'https://csgo-case.com/data/PHP/nocache/market/buyItem.php', true); xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded; charset=UTF-8'); xhr.setRequestHeader('accept', 'application/json, text/javascript, */*; q=0.01'); xhr.send(body); ";
                                string title = (string)js.ExecuteScript(query);
                                Program.Mess.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "Завершил все запросы!");
                                Thread.Sleep(3000);
                               // return false;

                            }
                            else
                            {
                                SetListBadPrice(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"), "cs.money", item.itemURLName.ToString(), name.Price.ToString(), item.price.ToString());
                            }

                        }
                    }

                }
            }
            catch (Exception ex) { Program.Mess.Enqueue("БОТ[" + ID + "] " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|Ошибка2 :" + ex.Message); }
            return false;
        }
       
     
        private void SetListBadPrice(string _Data, string _Site, string _Name, string _OldPrice, string _NewPrice)
        {
            Program.DataViewBadPrice item = new Program.DataViewBadPrice()
            {
                Date = _Data,
                Site = _Site,
                Name =_Name,
                OldPrice = _OldPrice,
                NewPrice =_NewPrice
            };
            Program.BadPrice.Add(item);
        }
    }
}
