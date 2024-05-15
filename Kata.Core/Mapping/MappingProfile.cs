using AutoMapper;
using Kata.Core.Dtos;
using Kata.Data.Entities;

namespace Kata.Core.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Appointment, AppointmentDto>();
            CreateMap<TimeSlot, TimeSlotDto>();
        }
    }
}
