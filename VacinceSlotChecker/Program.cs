using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using VaccinceSlotChecker.Handlers;
using VaccinceSlotChecker.Models;

namespace VaccinceSlotChecker
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var centers = LoadAllCenters();            
            var isComplete = false;
            var isShowFull = false;
            var emptySlots = new List<CenterTimeSlot>();
            var isInputRight = false;
            var isChildrenChioce = false;
            var isChildrenOnly = false;

            while (isInputRight == false)
            { 
                Console.Write("Sinovac (S) OR BioNTech (B)?");
                var input = Console.ReadLine();
                input = input.Trim().ToUpper();

                if (input == "B")
                {
                    while (isChildrenChioce == false)
                    {
                        Console.Write("Children Only? (Y/N)");
                        var cinput = Console.ReadLine();
                        cinput = cinput.Trim().ToUpper();

                        if (cinput == "Y" || cinput == "N")
                        {
                            isChildrenChioce = true;
                            isChildrenOnly = cinput == "Y" ? true : false;
                        }
                    }
                }

                if (input == "B" || input == "S")
                {
                    isInputRight = true;
                    var choice = input == "B" ? "BioNTech/Fosun" : (input == "S" ? "Sinovac" : "");
                    centers = centers.Where(c => c.cv_name == choice).ToList();

                    if (input == "B")
                    {
                        centers = centers.Where(c => isChildrenOnly ? c.center_name.Contains("兒童") : !c.center_name.Contains("兒童")).ToList();
                    }
                }
            }


            while (isComplete == false)
            {
                List<CenterSlotResult> availableResults = new List<CenterSlotResult>();

                foreach (var c in centers)
                {
                    var isError = false;
                    var isEmpty = false;
                    List<TimeSlotDetail> availableDetails = new List<TimeSlotDetail>();
                    List<TimeSlot> availableSlots = new List<TimeSlot>();

                    CenterSlotResult apiResult = null;
                    try
                    {
                        apiResult = ApiHandler.Execute(c.center_id, c.cv_ctc_type, c.cv_name).Result;
                    }
                    catch
                    {
                        isError = true;
                    }

                    if (apiResult != null && !isError)
                    {
                        //Console.WriteLine("Checking Success... ");
                        emptySlots.RemoveAll(s => s.center_id == c.center_id); // Clear the previous list

                        if (apiResult.avalible_timeslots != null && apiResult.avalible_timeslots.Count() != 0)
                        {
                            foreach (var t in apiResult.avalible_timeslots)
                            {
                                if (t.timeslots != null && t.timeslots.Count != 0)
                                {
                                    var slots = t.timeslots.Where(ts => ts.value == 1);

                                    if (slots.Count() != 0)
                                    {
                                        //Found empty!
                                        isEmpty = true;

                                        foreach (var s in slots)
                                        {
                                            availableSlots.Add(s);

                                            emptySlots.Add(new CenterTimeSlot
                                            {
                                                center_id = c.center_id,
                                                datetime = s.datetime,
                                                display_label = s.display_label,
                                                time = s.time,
                                                timeslots_id = s.timeslots_id,
                                                value = s.value
                                            }
                                            );
                                        }
                                    }
                                }
                            }
                        }

                        if (isEmpty)
                        {
                            Console.WriteLine("Checking Center: [{1}] {0}", c.center_name, c.district_name);

                            Console.ForegroundColor = ConsoleColor.White;
                            Console.BackgroundColor = ConsoleColor.Green;
                            Console.WriteLine("Found empty slots:");
                            var groups = availableSlots.GroupBy(s => s.datetime.ToString("yyyy/MM/dd"));

                            //foreach (var s in availableSlots)
                            //{
                            //    Console.WriteLine("Datetime: " + s.datetime.ToString("yyyy/MM/dd hh:mm:ss") + " (id: " + s.timeslots_id + ") @ " + DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
                            //}

                            foreach (var g in groups)
                            {
                                Console.Write("Date: " + g.Key + " ");
                                var displaySlots = "";

                                foreach (var s in g)
                                {
                                    
                                    displaySlots += (string.IsNullOrEmpty(displaySlots)? "" : ", ") + s.datetime.ToString("hh:mm");
                                }

                                Console.WriteLine("TimeSlots: " + displaySlots);
                            }
                            
                            Console.ResetColor();
                            Console.Beep();

                            availableResults.Add(new CenterSlotResult { center_id = c.center_id, CTC_NATURE = apiResult.CTC_NATURE, freeTimeslots = availableSlots });
                        }
                        else
                        {
                            if (isShowFull)
                            {
                                Console.WriteLine("Checking Center: [{1}] {0}", c.center_name, c.district_name);

                                Console.ForegroundColor = ConsoleColor.White;
                                Console.BackgroundColor = ConsoleColor.DarkRed;
                                Console.WriteLine("No empty slots.");
                                Console.ResetColor();
                            }
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.WriteLine("Checking Error!!! ");
                        Console.ResetColor();
                    }

                    isEmpty = false;
                    isError = false;
                    //Summary: availableResults
                }

                if (availableResults.Count != 0)
                {
                    Thread.Sleep(10000); //display longer
                }
                else
                {
                    Thread.Sleep(3000);
                }
                
                Console.Clear();
            }
        }

        private static List<VaccineCenter> LoadAllCenters()
        {
            var result = new List<VaccineCenter>();
            //SinoVac
            result.Add(new VaccineCenter { center_id = "144", cv_ctc_type = "HA", cv_name = "Sinovac", district_name = "中西區", center_name = "堅尼地城賽馬會普通科門診診所" });
            result.Add(new VaccineCenter { center_id = "6", cv_ctc_type = "HA",   cv_name = "Sinovac", district_name = "東區", center_name = "西灣河普通科門診診所" });
            result.Add(new VaccineCenter { center_id = "137", cv_ctc_type = "HA", cv_name = "Sinovac", district_name = "東區", center_name = "筲箕灣賽馬會普通科門診診所" });
            result.Add(new VaccineCenter { center_id = "149", cv_ctc_type = "CVC",cv_name = "Sinovac", district_name = "東區", center_name = "渣華道體育館社區疫苗接種中心" });
            result.Add(new VaccineCenter { center_id = "9", cv_ctc_type = "HA",   cv_name = "Sinovac", district_name = "南區", center_name = "鴨脷洲普通科門診診所" });
            result.Add(new VaccineCenter { center_id = "145", cv_ctc_type = "HA", cv_name = "Sinovac", district_name = "觀塘區", center_name = "九龍灣健康中心普通科門診診所" });
            result.Add(new VaccineCenter { center_id = "14", cv_ctc_type = "HA",  cv_name = "Sinovac", district_name = "深水步區", center_name = "長沙灣賽馬會普通科門診診所" });
            result.Add(new VaccineCenter { center_id = "11", cv_ctc_type = "HA",  cv_name = "Sinovac", district_name = "黃大仙區", center_name = "東九龍普通科門診診所" });
            result.Add(new VaccineCenter { center_id = "139", cv_ctc_type = "HA", cv_name = "Sinovac", district_name = "黃大仙區", center_name = "柏立基普通科門診診所" });
            result.Add(new VaccineCenter { center_id = "4", cv_ctc_type = "CVC",  cv_name = "Sinovac", district_name = "油尖旺區", center_name = "官涌體育館社區疫苗接種中心" });
            result.Add(new VaccineCenter { center_id = "177", cv_ctc_type = "HA", cv_name = "Sinovac", district_name = "離島區", center_name = "大澳賽馬會普通科門診診所" });
            result.Add(new VaccineCenter { center_id = "147", cv_ctc_type = "HA", cv_name = "Sinovac", district_name = "葵青區", center_name = "青衣市區普通科門診診所" });
            result.Add(new VaccineCenter { center_id = "146", cv_ctc_type = "HA", cv_name = "Sinovac", district_name = "西貢區", center_name = "方逸華普通科門診診所" });
            result.Add(new VaccineCenter { center_id = "3", cv_ctc_type = "CVC",  cv_name = "Sinovac", district_name = "沙田區", center_name = "源禾路體育館社區疫苗接種中心" });
            result.Add(new VaccineCenter { center_id = "18", cv_ctc_type = "HA",  cv_name = "Sinovac", district_name = "沙田區", center_name = "馬鞍山家庭醫學中心" });
            result.Add(new VaccineCenter { center_id = "16", cv_ctc_type = "HA",  cv_name = "Sinovac", district_name = "荃灣區", center_name = "伍若瑜夫人普通科門診診所" });
            result.Add(new VaccineCenter { center_id = "22", cv_ctc_type = "HA",  cv_name = "Sinovac", district_name = "天水圍區", center_name = "天水圍(天業路)社區健康中心" });

            ////BNT
            result.Add(new VaccineCenter { center_id = "25", cv_ctc_type = "CVC", cv_name = "BioNTech/Fosun", district_name = "", center_name = "(復必泰)中山紀念公園體育館社區疫苗接種中心" });
            result.Add(new VaccineCenter { center_id = "179", cv_ctc_type = "HA", cv_name = "BioNTech/Fosun", district_name = "", center_name = "(復必泰)西營盤賽馬會普通科門診診所" });
            result.Add(new VaccineCenter { center_id = "28", cv_ctc_type = "CVC", cv_name = "BioNTech/Fosun", district_name = "", center_name = "西灣河體育館社區疫苗接種中心" });
            result.Add(new VaccineCenter { center_id = "42", cv_ctc_type = "CVC", cv_name = "BioNTech/Fosun", district_name = "", center_name = "香港大學駐港怡醫院社區疫苗接種中心" });
            result.Add(new VaccineCenter { center_id = "192", cv_ctc_type = "CVC", cv_name = "BioNTech/Fosun",district_name = "", center_name = "(復必泰)香港大學駐港怡醫院兒童社區疫苗接種中心" });
            result.Add(new VaccineCenter { center_id = "178", cv_ctc_type = "HA", cv_name = "BioNTech/Fosun", district_name = "", center_name = "(復必泰)貝夫人普通科門診診所" });
            result.Add(new VaccineCenter { center_id = "189", cv_ctc_type = "CVC", cv_name = "BioNTech/Fosun",district_name = "", center_name = "(復必泰)禮頓中心衛星社區疫苗接種中心" });
            result.Add(new VaccineCenter { center_id = "172", cv_ctc_type = "CVC", cv_name = "BioNTech/Fosun",district_name = "", center_name = "教育局九龍塘教育服務中心社區疫苗接種中心" });
            result.Add(new VaccineCenter { center_id = "190", cv_ctc_type = "CVC", cv_name = "BioNTech/Fosun",district_name = "", center_name = "(復必泰)香港兒童醫院兒童社區疫苗接種中心" });
            result.Add(new VaccineCenter { center_id = "46", cv_ctc_type = "CVC", cv_name = "BioNTech/Fosun", district_name = "", center_name = "曉光街體育館社區疫苗接種中心" });
            result.Add(new VaccineCenter { center_id = "174", cv_ctc_type = "CVC", cv_name = "BioNTech/Fosun",district_name = "", center_name = "九龍灣體育館社區疫苗接種中心(復必泰)" });
            result.Add(new VaccineCenter { center_id = "181", cv_ctc_type = "HA", cv_name = "BioNTech/Fosun", district_name = "", center_name = "(復必泰)觀塘社區健康中心" });
            result.Add(new VaccineCenter { center_id = "37", cv_ctc_type = "CVC", cv_name = "BioNTech/Fosun", district_name = "", center_name = "荔枝角公園體育館社區疫苗接種中心" });
            result.Add(new VaccineCenter { center_id = "187", cv_ctc_type = "HA", cv_name = "BioNTech/Fosun", district_name = "", center_name = "(復必泰)北大嶼山社區健康中心 ( 北大嶼山醫院 )" });
            result.Add(new VaccineCenter { center_id = "142", cv_ctc_type = "CVC", cv_name = "BioNTech/Fosun",district_name = "", center_name = "林士德體育館社區疫苗接種中心" });
            result.Add(new VaccineCenter { center_id = "183", cv_ctc_type = "HA", cv_name = "BioNTech/Fosun", district_name = "", center_name = "南葵涌賽馬會普通科門診診所" });
            result.Add(new VaccineCenter { center_id = "185", cv_ctc_type = "HA", cv_name = "BioNTech/Fosun", district_name = "", center_name = "粉嶺家庭醫學中心" });
            result.Add(new VaccineCenter { center_id = "182", cv_ctc_type = "HA", cv_name = "BioNTech/Fosun", district_name = "", center_name = "將軍澳（寶寧路）普通科門診診所" });
            result.Add(new VaccineCenter { center_id = "36", cv_ctc_type = "CVC", cv_name = "BioNTech/Fosun", district_name = "", center_name = "香港中文大學醫院社區疫苗接種中心" });
            result.Add(new VaccineCenter { center_id = "191", cv_ctc_type = "CVC", cv_name = "BioNTech/Fosun",district_name = "", center_name = "圓洲角體育館兒童社區疫苗接種中心" });
            result.Add(new VaccineCenter { center_id = "184", cv_ctc_type = "HA", cv_name = "BioNTech/Fosun", district_name = "", center_name = "大埔賽馬會普通科門診診所" });
            result.Add(new VaccineCenter { center_id = "186", cv_ctc_type = "HA", cv_name = "BioNTech/Fosun", district_name = "", center_name = "仁愛普通科門診診所" });
            result.Add(new VaccineCenter { center_id = "27", cv_ctc_type = "CVC", cv_name = "BioNTech/Fosun", district_name = "", center_name = "元朗體育館社區疫苗接種中心" });
            result.Add(new VaccineCenter { center_id = "188", cv_ctc_type = "HA", cv_name = "BioNTech/Fosun", district_name = "", center_name = "容鳳書健康中心" });

            return result;
        }
    }
}