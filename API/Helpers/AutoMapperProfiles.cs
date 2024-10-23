using API.DTOs;
using API.Entities;
using AutoMapper;

namespace API.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<RegisterDto, AppUser>();
        CreateMap<AppUser, UserDto>();
        
        CreateMap<DateTime, DateTime>().ConvertUsing(x => DateTime.SpecifyKind(x, DateTimeKind.Utc));
        CreateMap<DateTime?, DateTime?>().ConvertUsing(x => x.HasValue 
            ? DateTime.SpecifyKind(x.Value, DateTimeKind.Utc) : null);
    }
}
