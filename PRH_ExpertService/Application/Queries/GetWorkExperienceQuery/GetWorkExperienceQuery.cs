﻿using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Queries.GetWorkExperienceQuery
{
    public record GetWorkExperienceQuery(string WorkExperienceId) : IRequest<BaseResponse<WorkExperience>>;
}