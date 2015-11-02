using System;
using System.IO;
using System.Linq;
using System.Net;

// also have to add reference to System.Web
using System.Web;
using System.Xml.Linq;

namespace Geocode
{
    class Program
    {
        static string base_uri = "http://maps.googleapis.com/maps/api/geocode/xml?latlng={0},{1}&sensor=false";
        static bool run_forever = true;

        static void Main(string[] args)
        {
            while (run_forever)
            {
                Console.WriteLine("1: Address to coordinates");
                Console.WriteLine("2: Coordinates to address");
                Console.WriteLine("Q: Quit");
                Console.Write("Selection: ");
                string user_input = Console.ReadLine();
                if (string.IsNullOrEmpty(user_input)) continue;

                switch (user_input)
                {
                    case "1":
                        Console.Write("Address: ");
                        string address = Console.ReadLine();
                        double latitude_double = 0;
                        double longitude_double = 0;
                        if (!address_to_coords(address, out latitude_double, out longitude_double))
                        {
                            Console.WriteLine("Failed");
                        }
                        else
                        {
                            Console.WriteLine("Success: " + latitude_double + ", " + longitude_double);
                        }
                        break;

                    case "2":
                        Console.Write("Latitude: ");
                        string latitude_string = Console.ReadLine();
                        Console.Write("Longitude: ");
                        string longitude_string = Console.ReadLine();
                        Console.WriteLine("Result: " + coords_to_address(latitude_string, longitude_string));
                        break;

                    case "Q":
                    case "q":
                        run_forever = false;
                        break;

                    default:
                        continue;
                }
            }

            Console.ReadLine();
        }

        public static string coords_to_address(string lat, string lng)
        {
            try
            {
                string requestUri = string.Format(base_uri, lat, lng);
                string result = "";

                using (WebClient wc = new WebClient())
                {
                    result = wc.DownloadString(new Uri(requestUri));
                }

                return parse_result(result);
            }
            catch (Exception e)
            {
                print_exception(e);
                return null;
            }
        }

        public static bool address_to_coords(string address, out double latitude, out double longitude)
        {
            latitude = 0;
            longitude = 0;

            try
            {
                WebRequest request = WebRequest.Create("http://maps.googleapis.com/maps/api/geocode/xml?sensor=false&address="
                         + HttpUtility.UrlEncode(address));

                using (WebResponse response = request.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        XDocument document = XDocument.Load(new StreamReader(stream));

                        XElement longitude_element = document.Descendants("lng").FirstOrDefault();
                        XElement latitude_element = document.Descendants("lat").FirstOrDefault();

                        if (longitude_element != null && latitude_element != null)
                        {
                            latitude = Convert.ToDouble(latitude_element.Value.ToString());
                            longitude = Convert.ToDouble(longitude_element.Value.ToString());
                        }
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                print_exception(e);
                return false;
            }
        }

        static string parse_result(string result)
        {
            try
            {
                var xml_element = XElement.Parse(result);

                var status = (from elm in xml_element.Descendants()
                              where elm.Name == "status"
                              select elm).FirstOrDefault();

                if (status.Value.ToLower() == "ok")
                {
                    var res = (from elm in xml_element.Descendants()
                               where elm.Name == "formatted_address"
                               select elm).FirstOrDefault();

                    // Console.WriteLine(res.Value);
                    return res.Value;
                }
                else
                {
                    Console.WriteLine("No Address Found");
                    return null;
                }
            }
            catch (Exception e)
            {
                print_exception(e);
                return null;
            }
        }

        static void print_exception(Exception e)
        {
            Console.WriteLine("================================================================================");
            Console.WriteLine(" = Exception Type: " + e.GetType().ToString());
            Console.WriteLine(" = Exception Data: " + e.Data);
            Console.WriteLine(" = Inner Exception: " + e.InnerException);
            Console.WriteLine(" = Exception Message: " + e.Message);
            Console.WriteLine(" = Exception Source: " + e.Source);
            Console.WriteLine(" = Exception StackTrace: " + e.StackTrace);
            Console.WriteLine("================================================================================");
        }
    }
}
