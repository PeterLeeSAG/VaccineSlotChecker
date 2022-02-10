using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinceSlotChecker.Models;
using VaccinceSlotChecker.Handlers;

namespace VaccinceSlotChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            var centers = LoadAllCenters();
            List<CenterSlotResult> availableResults = new List<CenterSlotResult>();

            foreach (var c in centers)
            {
                var isError = false;
                var isEmpty = false;
                List<TimeSlotDetail> availableDetails = new List<TimeSlotDetail>();
                List<TimeSlot> availableSlots = new List<TimeSlot>();
                Console.WriteLine("Checking Center: {0}", c.center_name);
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
                                    }

                                }
                            }
                        }
                    }

                    if (isEmpty)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.Green;
                        Console.WriteLine("Found empty slots:");
                        foreach (var s in availableSlots)
                        {
                            Console.WriteLine("Datetime: " + s.datetime.ToString("yyyy/MM/dd hh:mm:ss") + " (id: "+s.timeslots_id+")");
                        }
                        Console.ResetColor();
                        Console.Beep();

                        availableResults.Add(new CenterSlotResult { center_id = c.center_id, CTC_NATURE = apiResult.CTC_NATURE, freeTimeslots = availableSlots });
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("No empty slots.");
                        Console.ResetColor();
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

            Console.ReadLine();
        }

        static List<VaccineCenter> LoadAllCenters()
        {
            var result = new List<VaccineCenter>();
            //SinoVac
            result.Add(new VaccineCenter { center_id = "144", cv_ctc_type = "HA",  cv_name = "Sinovac", center_name = "堅尼地城賽馬會普通科門診診所" });
            result.Add(new VaccineCenter { center_id = "6",   cv_ctc_type = "HA",  cv_name = "Sinovac", center_name = "西灣河普通科門診診所" });
            result.Add(new VaccineCenter { center_id = "137", cv_ctc_type = "HA",  cv_name = "Sinovac", center_name = "筲箕灣賽馬會普通科門診診所" });
            result.Add(new VaccineCenter { center_id = "149", cv_ctc_type = "CVC", cv_name = "Sinovac", center_name = "渣華道體育館社區疫苗接種中心" });
            result.Add(new VaccineCenter { center_id = "9",   cv_ctc_type = "HA",  cv_name = "Sinovac", center_name = "鴨脷洲普通科門診診所" });
            result.Add(new VaccineCenter { center_id = "145", cv_ctc_type = "HA",  cv_name = "Sinovac", center_name = "九龍灣健康中心普通科門診診所" });
            result.Add(new VaccineCenter { center_id = "14",  cv_ctc_type = "HA",  cv_name = "Sinovac", center_name = "長沙灣賽馬會普通科門診診所" });
            result.Add(new VaccineCenter { center_id = "11",  cv_ctc_type = "HA",  cv_name = "Sinovac", center_name = "東九龍普通科門診診所" });
            result.Add(new VaccineCenter { center_id = "139", cv_ctc_type = "HA",  cv_name = "Sinovac", center_name = "柏立基普通科門診診所" });
            result.Add(new VaccineCenter { center_id = "4",   cv_ctc_type = "CVC", cv_name = "Sinovac", center_name = "官涌體育館社區疫苗接種中心" });
            result.Add(new VaccineCenter { center_id = "177", cv_ctc_type = "HA",  cv_name = "Sinovac", center_name = "大澳賽馬會普通科門診診所" });
            result.Add(new VaccineCenter { center_id = "147", cv_ctc_type = "HA",  cv_name = "Sinovac", center_name = "青衣市區普通科門診診所" });
            result.Add(new VaccineCenter { center_id = "146", cv_ctc_type = "HA",  cv_name = "Sinovac", center_name = "方逸華普通科門診診所" });
            result.Add(new VaccineCenter { center_id = "3",   cv_ctc_type = "CVC", cv_name = "Sinovac", center_name = "源禾路體育館社區疫苗接種中心" });
            result.Add(new VaccineCenter { center_id = "18",  cv_ctc_type = "HA",  cv_name = "Sinovac", center_name = "馬鞍山家庭醫學中心" });
            result.Add(new VaccineCenter { center_id = "16",  cv_ctc_type = "HA",  cv_name = "Sinovac", center_name = "伍若瑜夫人普通科門診診所" });
            result.Add(new VaccineCenter { center_id = "22",  cv_ctc_type = "HA",  cv_name = "Sinovac", center_name = "天水圍(天業路)社區健康中心" });


            //BNT
            result.Add(new VaccineCenter { center_id = "25",  cv_ctc_type = "CVC", cv_name = "BioNTech/Fosun", center_name = "(復必泰)中山紀念公園體育館社區疫苗接種中心" });
            result.Add(new VaccineCenter { center_id = "179", cv_ctc_type = "HA",  cv_name = "BioNTech/Fosun", center_name = "(復必泰)西營盤賽馬會普通科門診診所" });
            result.Add(new VaccineCenter { center_id = "28",  cv_ctc_type = "CVC", cv_name = "BioNTech/Fosun", center_name = "西灣河體育館社區疫苗接種中心" });
            result.Add(new VaccineCenter { center_id = "42",  cv_ctc_type = "CVC", cv_name = "BioNTech/Fosun", center_name = "香港大學駐港怡醫院社區疫苗接種中心" });
            result.Add(new VaccineCenter { center_id = "192", cv_ctc_type = "CVC", cv_name = "BioNTech/Fosun", center_name = "(復必泰)香港大學駐港怡醫院兒童社區疫苗接種中心" });
            result.Add(new VaccineCenter { center_id = "178", cv_ctc_type = "HA",  cv_name = "BioNTech/Fosun", center_name = "(復必泰)貝夫人普通科門診診所" });
            result.Add(new VaccineCenter { center_id = "189", cv_ctc_type = "CVC", cv_name = "BioNTech/Fosun", center_name = "(復必泰)禮頓中心衛星社區疫苗接種中心" });
            result.Add(new VaccineCenter { center_id = "172", cv_ctc_type = "CVC", cv_name = "BioNTech/Fosun", center_name = "教育局九龍塘教育服務中心社區疫苗接種中心" });
            result.Add(new VaccineCenter { center_id = "190", cv_ctc_type = "CVC", cv_name = "BioNTech/Fosun", center_name = "(復必泰)香港兒童醫院兒童社區疫苗接種中心" });
            result.Add(new VaccineCenter { center_id = "46",  cv_ctc_type = "CVC", cv_name = "BioNTech/Fosun", center_name = "曉光街體育館社區疫苗接種中心" });
            result.Add(new VaccineCenter { center_id = "174", cv_ctc_type = "CVC", cv_name = "BioNTech/Fosun", center_name = "九龍灣體育館社區疫苗接種中心(復必泰)" });
            result.Add(new VaccineCenter { center_id = "181", cv_ctc_type = "HA",  cv_name = "BioNTech/Fosun", center_name = "(復必泰)觀塘社區健康中心" });
            result.Add(new VaccineCenter { center_id = "37",  cv_ctc_type = "CVC", cv_name = "BioNTech/Fosun", center_name = "荔枝角公園體育館社區疫苗接種中心" });
            result.Add(new VaccineCenter { center_id = "187", cv_ctc_type = "HA",  cv_name = "BioNTech/Fosun", center_name = "(復必泰)北大嶼山社區健康中心 ( 北大嶼山醫院 )" });
            result.Add(new VaccineCenter { center_id = "142", cv_ctc_type = "CVC", cv_name = "BioNTech/Fosun", center_name = "林士德體育館社區疫苗接種中心" });
            result.Add(new VaccineCenter { center_id = "183", cv_ctc_type = "HA",  cv_name = "BioNTech/Fosun", center_name = "南葵涌賽馬會普通科門診診所" });
            result.Add(new VaccineCenter { center_id = "185", cv_ctc_type = "HA",  cv_name = "BioNTech/Fosun", center_name = "粉嶺家庭醫學中心" });
            result.Add(new VaccineCenter { center_id = "182", cv_ctc_type = "HA",  cv_name = "BioNTech/Fosun", center_name = "將軍澳（寶寧路）普通科門診診所" });
            result.Add(new VaccineCenter { center_id = "36",  cv_ctc_type = "CVC", cv_name = "BioNTech/Fosun", center_name = "香港中文大學醫院社區疫苗接種中心" });
            result.Add(new VaccineCenter { center_id = "191", cv_ctc_type = "CVC", cv_name = "BioNTech/Fosun", center_name = "圓洲角體育館兒童社區疫苗接種中心" });
            result.Add(new VaccineCenter { center_id = "184", cv_ctc_type = "HA",  cv_name = "BioNTech/Fosun", center_name = "大埔賽馬會普通科門診診所" });
            result.Add(new VaccineCenter { center_id = "186", cv_ctc_type = "HA",  cv_name = "BioNTech/Fosun", center_name = "仁愛普通科門診診所" });
            result.Add(new VaccineCenter { center_id = "27",  cv_ctc_type = "CVC", cv_name = "BioNTech/Fosun", center_name = "元朗體育館社區疫苗接種中心" });
            result.Add(new VaccineCenter { center_id = "188", cv_ctc_type = "HA",  cv_name = "BioNTech/Fosun", center_name = "容鳳書健康中心" });

            return result;                              
        }                                               
    }                                                   
}
