using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using AtakDomain.Core.Entity;
using AtakDomain.Repository;
using AtakDomain.Services.Services;
using AtakDomain.Web.Models;
using System.Xml.Serialization;
using Newtonsoft.Json;
using System.Text;
using System.Text.Unicode;
using System.Net;
using AtakDomain.Web.Helper;

namespace AtakDomain.Web.Controllers
{
    public class HotelController : Controller
    {
        private readonly HotelService _hotelService;

        public HotelController(HotelService hotelService)
        {
            this._hotelService = hotelService;
        }

        [HttpGet]
        public IActionResult Index(List<Hotel> hotels = null)
        {
            hotels = hotels == null ? new List<Hotel>() : hotels;

            return View(hotels);
        }

        [HttpPost]
        public IActionResult Index(IFormFile file, [FromServices] Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment, string Save, string button)
        {
            if (Save != null)
            {
                ViewBag.Save = Save.ToLower();
            }

            switch (button)
            {
                case "Upload":

                    #region Upload CSV

                    string fileName = $"{hostingEnvironment.WebRootPath}\\files\\{file.FileName}";
                    using (var fileStream = new FileStream(fileName, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                        fileStream.Flush();
                    }

                    #endregion Upload CSV

                    var hotels = this.GetHotelList(file.FileName);

                    var hotelDto = _hotelService.SaveFiles(new FilesDto
                    {
                        Name = file.FileName,
                    });

                    return View(hotels);

                case "SortName":
                    var hote2ls = GetHotelList(_hotelService.GetLastFiles().Result.Name);
                    hote2ls.Sort((x, y) => x.Name.CompareTo(y.Name));
                    return View(hote2ls);

                case "SortAddress":
                    var hote3ls = GetHotelList(_hotelService.GetLastFiles().Result.Name);
                    hote3ls.Sort((x, y) => x.Address.CompareTo(y.Address));
                    return View(hote3ls);

                case "SortStars":
                    var hote4ls = GetHotelList(_hotelService.GetLastFiles().Result.Name);
                    hote4ls.Sort((x, y) => x.Stars.CompareTo(y.Stars));
                    return View(hote4ls);

                case "SortContact":
                    var hote5ls = GetHotelList(_hotelService.GetLastFiles().Result.Name);
                    hote5ls.Sort((x, y) => x.Contact.CompareTo(y.Contact));
                    return View(hote5ls);

                case "SortPhone":
                    var hote6ls = GetHotelList(_hotelService.GetLastFiles().Result.Name);
                    hote6ls.Sort((x, y) => x.Phone.CompareTo(y.Phone));
                    return View(hote6ls);

                case "SortUrl":
                    var hote7ls = GetHotelList(_hotelService.GetLastFiles().Result.Name);
                    hote7ls.Sort((x, y) => x.Url.CompareTo(y.Url));
                    return View(hote7ls);

                default:
                    return RedirectToAction("Index");
            }
        }

        public List<Hotel> GetHotelList(string fileName)
        {
            var hotels = new List<Hotel>();

            #region Read CSV

            var path = $"{Directory.GetCurrentDirectory()}\\wwwroot\\files\\{fileName}";
            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    var hotel = csv.GetRecord<Hotel>();

                    // Geçerli URL'ye gidip response alabilirse TRUE alamazsa FALSE dödürecek ve hata fıraltacak ekstra olarak yazığım bir valid.
                    //if (!checkWebsite(hotel.Url))
                    //{
                    //    ViewBag.URL = "Hotel URL not valid";
                    //}

                    char[] validCharacter = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', '_', '.' };

                    if (hotel.Name.Any(c => !validCharacter.Contains(c)))
                    {
                        ViewBag.Name = "Hotel Name not valid: " + hotel.Name;
                    }
                    if (HelperValid.checkUrl(hotel.Url) == false)
                    {
                        ViewBag.URL = "Hotel URL not valid: " + hotel.Url;
                    }
                    if (hotel.Name == null)
                    {
                        ViewBag.Name = "Hotel name is required: ";
                    }
                    if (hotel.Stars < 0 || hotel.Stars > 5)
                    {
                        ViewBag.Stars = "Hotel rating must be between 0 and 5: " + hotel.Name;
                    }
                    else
                    {
                        hotels.Add(hotel);
                    }

                    //XML File Saved
                    var xml = new XmlSerializer(typeof(List<Hotel>));
                    using (var writer = new StreamWriter($"{Directory.GetCurrentDirectory()}\\wwwroot\\files\\{fileName}.xml"))
                    {
                        xml.Serialize(writer, hotels);
                    }

                    //JSON File Saved
                    var json = JsonConvert.SerializeObject(hotels);
                    using (var writer = new StreamWriter($"{Directory.GetCurrentDirectory()}\\wwwroot\\files\\{fileName}.json"))
                    {
                        writer.Write(json);
                    }

                    if (ViewBag.Save == "evet")
                    {
                        var hotelDto = _hotelService.SaveHotel(new HotelDto
                        {
                            Name = hotels.Last().Name,
                            Address = hotels.Last().Address,
                            Contact = hotels.Last().Contact,
                            Stars = hotels.Last().Stars,
                            Phone = hotels.Last().Phone,
                            Url = hotels.Last().Url,
                        });
                    }
                }
            }

            #endregion Read CSV

            #region Create CSV

            var path2 = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\FilesTo\"}";
            using (var writer = new StreamWriter(path + "NewFile.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(hotels);
            }

            #endregion Create CSV

            hotels.Sort((x, y) => x.Name.CompareTo(y.Name));

            return hotels;
        }

        public IActionResult Download(string button)
        {
            var fileName = _hotelService.GetLastFiles().Result.Name;

            switch (button)
            {
                case "CSV":
                    return File(new FileStream($"{Directory.GetCurrentDirectory()}\\wwwroot\\files\\{fileName}.csv", FileMode.Open), "text/csv", $"{fileName}.csv");

                case "DownloadXML":
                    return File(new FileStream($"{Directory.GetCurrentDirectory()}\\wwwroot\\files\\{fileName}.xml", FileMode.Open), "text/xml", $"{fileName}.xml");

                case "DownloadJSON":
                    return File(new FileStream($"{Directory.GetCurrentDirectory()}\\wwwroot\\files\\{fileName}.json", FileMode.Open), "text/json", $"{fileName}.json");

                default:
                    return RedirectToAction("Index");
            }
        }
    }
}