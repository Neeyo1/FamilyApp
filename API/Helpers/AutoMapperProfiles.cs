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
        CreateMap<AppUser, MemberDto>();
        CreateMap<Group, GroupDto>()
            .ForMember(x => x.Members, y => y.MapFrom(z => z.UserGroups));
        CreateMap<UserGroup, MemberDto>()
            .ForMember(x => x.Id, y => y.MapFrom(z => z.UserId))
            .ForMember(x => x.KnownAs, y => y.MapFrom(z => z.User.KnownAs));
        CreateMap<Assignment, AssignmentDto>()
            .ForMember(x => x.UsersAssigned, y => y.MapFrom(z => z.UserAssignments));
        CreateMap<UserAssignment, AssignedMemberDto>()
            .ForMember(x => x.Id, y => y.MapFrom(z => z.UserId))
            .ForMember(x => x.KnownAs, y => y.MapFrom(z => z.User.KnownAs));
        CreateMap<Reaction, ReactionDto>();
        CreateMap<IGrouping<string, Reaction>, ReactionDto>()
            .ForMember(x => x.Name, y => y.MapFrom(z => z.Key))
            .ForMember(x => x.Users, y => y.MapFrom(z => z.ToList()));
        CreateMap<Reaction, MemberDto>()
            .ForMember(x => x.Id, y => y.MapFrom(z => z.UserId))
            .ForMember(x => x.KnownAs, y => y.MapFrom(z => z.User.KnownAs));

        CreateMap<DateTime, DateTime>().ConvertUsing(x => DateTime.SpecifyKind(x, DateTimeKind.Utc));
        CreateMap<DateTime?, DateTime?>().ConvertUsing(x => x.HasValue 
            ? DateTime.SpecifyKind(x.Value, DateTimeKind.Utc) : null);
    }
}
