using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSMONEY
{
    class LootFarm
    {
        IWebDriver driver;
        TextBox Log;
        int ID = 0;
        public LootFarm(TextBox Log, int id)
        {
            this.Log = Log;
            ID = id;
        }
        public void start()
        {
            var firstFull = Convert.ToInt32(DateTime.Now.ToString("HHmmss"));
            while (true)
            {
                try
                {
                    while (Program.pauseLoot == true)
                    {
                        Thread.Sleep(200);
                    }
                    Program.MessLoot.Enqueue("БОТ[" + ID + "] " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|" + "Завершил загрузку предметов.");
                    var res = ClickItem(driver.PageSource);
                    if (res == true)
                    {
                        var first = Convert.ToInt32(DateTime.Now.ToString("HHmmss"));
                    }
                    if (Convert.ToInt32(DateTime.Now.ToString("HHmmss")) - firstFull > 200)
                    {
                        bool cheek = false;
                        var dt = DataTa.GetTable();
                        foreach (DataRow item in dt.Rows)
                        {
                            var s = item.ItemArray;
                            if (s[0].ToString() == Properties.Settings.Default.name && s[1].ToString() == Properties.Settings.Default.lootfarm)
                            {
                                cheek = true;
                                if (s[3].ToString() != Properties.Settings.Default.lootfarmVersion)
                                {
                                    MessageBox.Show("Версия ПО устарела");
                                    Application.Exit();
                                }
                                Program.sleepILoot = Convert.ToInt32(s[2].ToString());
                            }
                           
                        }
                        if (cheek == false)
                        {
                            MessageBox.Show("Лицензия не активная.");
                            Application.Exit();
                        }
                        firstFull = Convert.ToInt32(DateTime.Now.ToString("HHmmss"));
                        RefreshBotInventory();
                        Program.MessLoot.Enqueue("БОТ[" + ID + "] " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|" + "{Правило 200сек.}Обновил инвентари!");
                    }
                    Thread.Sleep(Program.sleepMSecondLoot);
                }
                catch (Exception)
                {

                  
                }
                //Program.MessLoot.Enqueue("БОТ[" + ID + "] " + "--------------------------------");
               
            }
        }
        public void INI()
        {
            var driverService = ChromeDriverService.CreateDefaultService();  //скрытие 
            driverService.HideCommandPromptWindow = true;                    //консоли
            driver = new ChromeDriver(driverService);
            driver.Navigate().GoToUrl("https://loot.farm/");
            MessageBox.Show("Введите все данные , после этого программа продолжит работу!");
            driver.Manage().Window.Position = new Point(5000, 5000);
            driver.Navigate().GoToUrl("https://steamcommunity.com/id/me/tradeoffers/privacy#trade_offer_access_url");
            IWebElement offer = driver.FindElement(By.Id("trade_offer_access_url"));

            if (offer.GetAttribute("value") != Properties.Settings.Default.lootfarm)
            {
                MessageBox.Show("Выберите аккаунт к которому привязана программа!");
                driver.Quit();
                return;
            }
            driver.Navigate().GoToUrl("https://loot.farm/");
            driver.Manage().Window.Position = new Point(0, 0);
            MessageBox.Show("Выберите настройки фильтров.");
            start();
        }



        private bool ClickItem(string kode)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(kode);
            try
            {
                var nodes = doc.DocumentNode.SelectNodes("//div[@id=\"bots_inv\"]").FirstOrDefault();
                var nodes1 = nodes.ChildNodes.Where(n => (n.Name == "div"));
                foreach (var item1 in nodes1)
                {
                    foreach (var item2 in Program.DataLoot)
                    {
                        string name1 = item1.FirstChild.Attributes["data-name"].Value.Replace(" ", "");
                        string name2 = (item2.Name ).Replace(" ", "");
                        if (name1 == name2  )
                        {
                            var price = (Convert.ToInt32(item1.FirstChild.Attributes["data-p"].Value) / 100);
                            Program.MessLoot.Enqueue("БОТ[" + ID + "] " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "| Нашел предмет:" + name1);
                            if (price <= item2.Price)
                            {
                                IWebElement item = driver.FindElement(By.Id(item1.FirstChild.Attributes["id"].Value));
                                item.Click();
                                Program.MessLoot.Enqueue("БОТ[" + ID + "] " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "| Выбрал предмет:" + name1);
                                try
                                {
                                    Program.MessLoot.Enqueue("БОТ[" + ID + "] " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "| Нажал обмен:" + name1);
                                    Thread.Sleep(Program.sleepILoot);
                                    IWebElement tradeButton = driver.FindElement(By.Id("tradeButton"));
                                    tradeButton.Click();

                                }
                                catch (Exception ex) { }
                                try
                                {
                                    Thread.Sleep(5000);
                                    IWebElement item3 = driver.FindElement(By.Id(item1.FirstChild.Attributes["id"].Value));
                                    item3.Click();
                                    Program.MessLoot.Enqueue("БОТ[" + ID + "] " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "| Убрал предмет с верхнего блока:" + name1);
                                }
                                catch (Exception ex) { }
                            }
                            else {
                                Program.SetListBadPrice(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"), "loot.farm", name1, item2.Price.ToString(), price.ToString());
                                Program.MessLoot.Enqueue("БОТ[" + ID + "] " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "| Цена не подошла предмет:" + name1 +"|Цена сайта: "+ price.ToString()+"|Цена базы: "+ item2.Price.ToString());
                            }
                        return true;
                        }
                    }
                }
            }
            catch (Exception ex) { }
            return false;
        }
        //private bool ChekItemsOffer(string name)
        //{
        //    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
        //    doc.LoadHtml(driver.PageSource);
        //    string url = "";
        //    try
        //    {
        //        var nodes = doc.DocumentNode.SelectNodes("//div[@id=\"offer_inventory_bot\"]").FirstOrDefault();
        //        var nodes1 = nodes.ChildNodes.Where(n => (n.Name == "div"));
        //        int cc = 0;
        //        foreach (var item1 in nodes1)
        //        {
        //            string name1 = item1.Attributes["hash"].Value.Replace(" ", "");
        //            // GoodItem = "UMP-45|Labyrinth(FactoryNew)";
        //            if (name1 == name)
        //            {
        //                return true;
        //            }
        //            cc++;
        //        }
        //    }
        //    catch (Exception ex) { }
        //    Program.Mess.Enqueue("БОТ[" + ID + "] " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "| Зарезервированная вещь:" + name);
        //    return false;
        //}
        private void RefreshBotInventory()
        {
            IJavaScriptExecutor js = driver as IJavaScriptExecutor;

            Program.MessLoot.Enqueue("БОТ[" + ID + "] " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "| Обновил инвентарь бота");
            string title = (string)js.ExecuteScript("document.getElementById('UpdateBotInv').click();");
        }
        private void CloseModalFrm()
        {
            IJavaScriptExecutor js = driver as IJavaScriptExecutor;

            Program.MessLoot.Enqueue("БОТ[" + ID + "] " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "| Закрыл модальную форму");
            string title = (string)js.ExecuteScript("document.getElementsByClassName('modal__close')[3].click();");
        }
        //private void RefreshUserInventory()
        //{
        //    IJavaScriptExecutor js = driver as IJavaScriptExecutor;

        //    Program.Mess.Enqueue("БОТ[" + ID + "] " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "| Обновил инвентарь пользователя");
        //    string title = (string)js.ExecuteScript("document.getElementById('refresh_user_inventory').click();");
        //}


    }
}
