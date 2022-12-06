using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;
using OpenQA.Selenium.Support.UI;
using System.Globalization;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace ReservationGUI
{
    /// <summary>
    /// Класс автоматического управления Chrome
    /// </summary>
    internal class Browser
    {
        private ChromeOptions options { get; set; }
        private ChromeDriverService driverService { get; set; }
        private ChromeDriver driver { get; set; }
        private OpenQA.Selenium.Cookie cookie { get; set; }

        public Browser()
        {
            try
            {
                options = new ChromeOptions();
                options.AddArgument("ignore-certificate-errors");
                options.AddArgument("no-sandbox");
                options.AddArgument("--start-maximized");
                //options.AddArgument("--blink-settings=imagesEnabled=false");
                options.AddArgument("--dns-prefetch-disable");
                options.AddArguments($"user-data-dir={Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\AppData\\Local\\Google\\Chrome\\User Data\\");
                driverService = ChromeDriverService.CreateDefaultService();
                driver = new ChromeDriver(driverService, options);
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }

        }

        /// <summary>
        /// Поиск свободных дней пронирования
        /// </summary>
        /// <param name="valDay4Class">Колличество дней от текущей даты по которое смотрим бронь 4 Class</param>
        /// <param name="valDay5Class">Колличество дней от текущей даты по которое смотрим бронь 5 Class</param>
        /// <returns>Сообщение с уведомлениями</returns>
        public string Get(int valDay4Class, int valDay5Class)
        {
            string result = "";

            // page 1
            //driver.Manage().Cookies.DeleteCookieNamed("ASP.NET_VentusBooking_SessionId");
            driver.Navigate().GoToUrl("https://nqa4.nemoqappointment.com/Booking/Booking/Index/fl34hs86");
            System.Threading.Thread.Sleep(3000);

            try
            {
                //<input class="btn btn-primary btn-large disable-on-save" type="submit" title="Create an appointment" name="StartNextButton" value="Create an appointment">
                driver.FindElement(By.Name("StartNextButton")).Click();
                System.Threading.Thread.Sleep(2000);
            }
            catch (Exception)
            {
                
            }

            // page 2
            try
            {
                //<input class="checkbox" id="AcceptInformationStorage" name="AcceptInformationStorage" required="required" tabindex="0" type="checkbox" value="true">
                driver.FindElement(By.ClassName("checkbox")).Click();
                var submit = driver.FindElement(By.XPath("/html/body/div[2]/div/div/div/form/div[2]/input"));
                string framename = driver.FindElement(By.TagName("iframe")).GetAttribute("name");
                //<iframe title="reCAPTCHA" src="https://www.google.com/recaptcha/api2/anchor?ar=1&amp;k=6LfmYiseAAAAANkARitJE-1lwBZxb_TX63Wa_5vc&amp;co=aHR0cHM6Ly9ucWE0Lm5lbW9xYXBwb2ludG1lbnQuY29tOjQ0Mw..&amp;hl=en&amp;v=Km9gKuG06He-isPsP6saG8cn&amp;size=normal&amp;cb=1f182lgve10t" width="304" height="78" role="presentation" name="a-qxacf4l3el9p" frameborder="0" scrolling="no" sandbox="allow-forms allow-popups allow-same-origin allow-scripts allow-top-navigation allow-modals allow-popups-to-escape-sandbox"></iframe>
                driver.SwitchTo().Frame(framename);
                //<div id="recaptcha-accessible-status" class="rc-anchor-aria-status" aria-hidden="true">Recaptcha requires verification. </div>
                driver.FindElement(By.CssSelector("div.recaptcha-checkbox-border")).Click();
                System.Threading.Thread.Sleep(2000);
                driver.SwitchTo().DefaultContent();
                submit.Click();
                System.Threading.Thread.Sleep(2000);
            }
            catch (Exception) 
            { 

            }

            // page 3
            try
            {
                //<select data-function="data-provider" data-param="regionId" data-url="/Booking/Ajax/GetSections?serviceGroupId=143" id="RegionId" name="RegionId" tabindex="0"><option selected="selected" value="0">Select county..</option>
                //<option value="16">Broward County</option>
                //<option value="21">Leon County- IRP/IFTA - Commercial vehicles only</option>
                //<option value="17">Miami-Dade County</option>
                //</select>
                var county = new SelectElement(driver.FindElement(By.Id("RegionId")));
                county.SelectByText("Broward County");
                System.Threading.Thread.Sleep(1000);

                //<select data-function="data-provider-subscriber" data-key="sections" data-param="sectionId" data-provider="#RegionId" data-url="/Booking/Ajax/GetServiceTypes?serviceGroupId=143" id="SectionId" name="SectionId" tabindex="0"><option value="0">Select office..</option>
                //<option selected="selected" value="152">Lauderdale Lakes - 3718-3 W. Oakland Park Boulevard</option>
                //<option value="155">Margate - 1135 Banks Road</option>
                //<option value="153">Pembroke Pines - 8001 Pembroke Road</option>
                //<option value="177">Pompano Citi Centre - 1955 N. Federal Hwy, Suite J209</option>
                //<option value="154">Sunrise - 3511 N Pine Island Road</option>
                //</select>
                var office = new SelectElement(driver.FindElement(By.Id("SectionId")));
                int count = office.Options.Count;
                for (int i = 1; i < count; i++)
                {
                    result += ParseOffices("Broward County", "4. Class E Knowledge Exam", i, DateTime.Now.AddDays(valDay4Class));
                }


                county = new SelectElement(driver.FindElement(By.Id("RegionId")));
                county.SelectByText("Broward County");
                System.Threading.Thread.Sleep(1000);
                office = new SelectElement(driver.FindElement(By.Id("SectionId")));
                count = office.Options.Count;
                for (int i = 1; i < count; i++)
                {
                    result += ParseOffices("Broward County", "5. Class E Driving Exam", i, DateTime.Now.AddDays(valDay5Class));
                }


                county = new SelectElement(driver.FindElement(By.Id("RegionId")));
                county.SelectByText("Miami-Dade County");
                System.Threading.Thread.Sleep(1000);
                office = new SelectElement(driver.FindElement(By.Id("SectionId")));
                count = office.Options.Count;
                for (int i = 1; i < count; i++)
                {
                    result += ParseOffices("Miami-Dade County", "4. Class E Knowledge Exam", i, DateTime.Now.AddDays(valDay4Class));
                }


                county = new SelectElement(driver.FindElement(By.Id("RegionId")));
                county.SelectByText("Miami-Dade County");
                System.Threading.Thread.Sleep(1000);
                office = new SelectElement(driver.FindElement(By.Id("SectionId")));
                count = office.Options.Count;
                for (int i = 1; i < count; i++)
                {
                    result += ParseOffices("Miami-Dade County", "5. Class E Driving Exam", i, DateTime.Now.AddDays(valDay5Class));
                }
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
                return null;
            }


            driver.Quit();

            return result;
        }

        /// <summary>
        /// Парсит по конкретному офису
        /// </summary>
        /// <param name="strCounty">Выбор региона</param>
        /// <param name="strAppointment">Тип</param>
        /// <param name="indexOffice">Индекс офиса</param>
        /// <param name="date">Дата до окторой искать</param>
        /// <returns></returns>
        private string ParseOffices(string strCounty, string strAppointment, int indexOffice, DateTime date)
        {
            string result = "";

            var county = new SelectElement(driver.FindElement(By.Id("RegionId")));
            county.SelectByText(strCounty);
            System.Threading.Thread.Sleep(2000);
            var office = new SelectElement(driver.FindElement(By.Id("SectionId")));

            string nameOffice = office.Options[indexOffice].Text;
            office.SelectByIndex(indexOffice);
            System.Threading.Thread.Sleep(2000);

            //<select data-function="data-subscriber" data-key="serviceTypes" data-provider="#SectionId" id="ServiceTypeId" name="ServiceTypeId" post-after-data-provided="True" tabindex="0"><option value="837">1. Renew or Replace</option>
            //<option value="840">2. Original </option>
            //<option value="841">3. Reinstatement</option>
            //<option selected="selected" value="844">4. Class E Knowledge Exam</option>
            //<option value="846">5. Class E Driving Exam</option>
            //<option value="842">6. CDL Written/Endorsement Exam/Hazmat/Fingerprint</option>
            //<option value="845">7. Any Service with a Sign Language Interpreter</option>
            //</select>
            var appointmentType = new SelectElement(driver.FindElement(By.Id("ServiceTypeId")));
            try
            {
                appointmentType.SelectByText(strAppointment);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Cannot locate element with text")) return result;
            }
            

            //<input type="submit" class="btn btn-primary" name="TimeSearchFirstAvailableButton" value="First Available Slot">
            driver.FindElement(By.Name("TimeSearchFirstAvailableButton")).Click();
            System.Threading.Thread.Sleep(2000);

            for (int d = 0; d < 52; d++)
            {
                // поиск сроки No available times could be found
                var bodyTag = driver.FindElement(By.TagName("body"));
                if (bodyTag.Text.Contains("No available times could be found")) return result;

                var tbody = driver.FindElement(By.XPath("//*[@id=\"Main\"]/form[2]/div[2]/table/tbody"));
                var list = ParseDate(tbody, date);
                if (list.Count == 0) continue;

                // проверю не вышла дата за интервал
                try
                {
                    DateTime dt = DateTime.ParseExact(list[0], "M/d/yyyy h:m:s tt", CultureInfo.InvariantCulture);
                    if (dt > date) return result;
                }
                catch (Exception ex)
                {
                    Loger.Error(ex);
                    return result;
                }

                if (list.Count > 0)
                {
                    result += $"{strCounty} - {strAppointment} - {nameOffice}\n{String.Join("\n", list.ToArray())} \n\n";
                    break;
                }

                //<input class="btn btn-primary pull-right disable-on-save" id="booking-next" type="submit" name="Next" value="Next" tabindex="5" aria-label="submit">
                driver.FindElement(By.Id("booking-next")).Click();
                System.Threading.Thread.Sleep(2000);
            }

            return result;
        }

        /// <summary>
        /// Парсит список из мест
        /// </summary>
        /// <param name="element">элемент страницы</param>
        /// <param name="date">дата после которой не брать результат</param>
        /// <returns>Возвращает первую список из мест первой свободной даты</returns>
        private List<string> ParseDate(IWebElement element, DateTime date)
        {
            List<string> result = new List<string>(); 
            List<IWebElement> lstTdElem = new List<IWebElement>(element.FindElements(By.TagName("td")));

            if (lstTdElem.Count > 0)
            {
                foreach (var elemTd in lstTdElem)
                {
                    List<IWebElement> lstDiv = new List<IWebElement>(elemTd.FindElements(By.TagName("div")));
                    if (lstDiv.Count == 1) continue;

                    //<div data-function="timeTableCell" data-sectionid="152" data-servicetypeid="844" data-fromdatetime="12/7/2022 9:00:00 AM" class="pointer timecell text-center " style="top: 60px; height:60px; background-color: #FF0000;color:#000000; position: relative" aria-label="Booked" role="row" bis_skin_checked="1">
                    //				<label for="ReservedDateTime_12/7/2022 9:00:00 AM">
                    //								Booked
                    //				</label>
                    //</div>

                    //<div data-function="timeTableCell" data-sectionid="152" data-servicetypeid="846" data-fromdatetime="12/7/2022 8:30:00 AM" class="pointer timecell text-center " style="top: 60px; height:30px; background-color: #FF0000;color:#000000; position: relative" aria-label="Booked" role="row" bis_skin_checked="1">
	                   // <label for="ReservedDateTime_12/7/2022 8:30:00 AM">
					               //     Booked
	                   // </label>
                    //</div>
                    foreach (var div in lstDiv)
                    {
                        if (div.GetAttribute("aria-label") == "Booked") continue;
                        string s = div.GetAttribute("data-fromdatetime");
                        if (s == null) continue;
                        result.Add(s);  // 12/7/2022 11:00:00 AM    //DateTime dt = DateTime.ParseExact("12/7/2022 11:00:00 AM", "M/d/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
                    }

                }
            }

            return result; 
        }

    }
}
