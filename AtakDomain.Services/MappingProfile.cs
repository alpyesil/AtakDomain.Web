using AtakDomain.Core.Entity;
using AutoMapper;

namespace App.Application;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<HotelDto, Hotal>();

        CreateMap<FilesDto, Files>();
    }
}