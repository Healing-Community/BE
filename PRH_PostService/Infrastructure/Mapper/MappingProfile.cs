﻿using Application.Commons.DTOs;
using AutoMapper;
using Domain.Entities;

namespace Infrastructure.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<Post, PostDto>().ReverseMap();
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Reaction, ReactionDto>().ReverseMap();
            CreateMap<ReactionType, ReactionTypeDto>().ReverseMap();
            CreateMap<Comment, CommentDto>().ReverseMap();
            CreateMap<Report, ReportDto>().ReverseMap();
        }
    }
}
