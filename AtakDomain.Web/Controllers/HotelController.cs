using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using AtakDomain.Core.Entity;
using AtakDomain.Repository;
using AtakDomain.Services.Services;
using AtakDomain.Web.Models;

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

                    hotels.Add(hotel);
                    ViewData["Hotels"] = hotels;

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
    }
}