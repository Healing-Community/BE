using Application.Commons.DTOs;
using AutoMapper;
using Domain.Constants.AMQPMessage;
using Domain.Entities;

namespace Infrastructure.Mapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<ReportMessageDto, ReportMessage>().ReverseMap();
    }
}