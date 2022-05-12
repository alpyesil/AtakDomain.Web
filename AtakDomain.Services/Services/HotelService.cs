using AtakDomain.Core.Entity;
using AtakDomain.Repository;
using AutoMapper;

namespace AtakDomain.Services.Services
{
    public class HotelService
    {
        private readonly AppDbContext _db;
        private readonly IMapper mapper;

        public HotelService(AppDbContext db, IMapper mapper)
        {
            _db = db;
            this.mapper = mapper;
        }

        public async Task<HotelDto> SaveHotel(HotelDto hotel)
        {
            var hotelRegister = mapper.Map<Hotal>(hotel);

            hotelRegister.Name = hotel.Name;
            hotelRegister.Address = hotel.Address;
            hotelRegister.Phone = hotel.Phone;
            hotelRegister.Stars = hotel.Stars;
            hotelRegister.Url = hotel.Url;
            hotelRegister.Contact = hotel.Contact;

            _db.Hotels.Add(hotelRegister);
            await _db.SaveChangesAsync();

            return hotel;
        }

        public async Task<FilesDto> SaveFiles(FilesDto files)
        {
            var filesRegister = mapper.Map<Files>(files);

            filesRegister.Name = files.Name;

            _db.Files.Add(filesRegister);
            await _db.SaveChangesAsync();

            return files;
        }

        public async Task<Files> GetLastFiles()
        {
            var lastFiles = _db.Files.OrderByDescending(x => x.Id).FirstOrDefault();

            return lastFiles;
        }
    }
}