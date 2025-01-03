﻿using Application.Commons.DTOs;
using AutoMapper;
using Domain.Entities;

namespace Infrastructure.Mapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        //CreateMap<User, UserDto>().ReverseMap();
        CreateMap<Report, ReportDto>().ReverseMap();
    }
}