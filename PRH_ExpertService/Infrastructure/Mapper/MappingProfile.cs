using Application.Commons.DTOs;
using AutoMapper;
using Domain.Entities;

namespace Infrastructure.Mapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ExpertProfile, ExpertProfileDTO>();
        CreateMap<Certificate, CertificateDTO>();
        CreateMap<WorkExperience, WorkExperienceDTO>();
        CreateMap<Appointment, AppointmentDTO>();
        CreateMap<ExpertAvailability, ExpertAvailabilityDTO>();
    }
}