using System.IO.Compression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Entities.Models;
using Entities.DataTransferObjects;
namespace CompanyEmployees
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //CreateMap<Source,Destination>
            CreateMap<Company,CompanyDto>()
            .ForMember(dest=> dest.FullAddress, opt => opt.MapFrom(src=> string.Join(' ', src.Address, src.Country)));
        }
    }
}