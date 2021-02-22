using System.Linq;
using Assistance.Operational.Model;
using AutoMapper;
using Dto;

namespace Assistance.Operational.WebApi.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public override string ProfileName => "MissionMappingProfile";

        public MappingProfile()
        {
            CreateMap<Dal.MongoImpl.Entities.Activity, ActivityDto>()
               .ForMember(x => x.DaysNumber, opts => opts.MapFrom(x => x.Days.Sum(d => ConvertDayPartToNumber(d))));

            //CreateMap<UserDto, Dal.MongoImpl.Entities.User>()
            //   .ForMember(x => x.Id, opts => opts.MapFrom(x => new ObjectId(x.Id)));

            CreateMap<Dal.MongoImpl.Entities.UserMission, UserDto>()
                .ForMember(x => x.ExternalId, opts => opts.Ignore())
                .ForMember(x => x.IsDummy, opts => opts.Ignore())
                .ForMember(x => x.LastLoginDate, opts => opts.Ignore())
                .ForMember(x => x.PhoneNumber, opts => opts.Ignore())
                .ForMember(x => x.Role, opts => opts.Ignore())
                .ForMember(x => x.SignatureFileUrl, opts => opts.Ignore())
                .ForMember(x => x.SubscriptionDate, opts => opts.Ignore())
                .ForMember(x => x.Password, opts => opts.Ignore());

            CreateMap<UserDto, Dal.MongoImpl.Entities.UserMission>();
            CreateMap<UserDto, UserActivityDto>()
                .ForMember(x => x.CanSign, opts => opts.Ignore())
                .ForMember(x => x.SignatureDate, opts => opts.Ignore())
                .ForMember(x => x.SignatureUri, opts => opts.Ignore())
                .ForMember(x => x.LastUpdateDate, opts => opts.Ignore());

            CreateMap<ExternalUserDto, UserDto>()
                .ForMember(x => x.Id, opts => opts.Ignore())
                .ForMember(x => x.ExternalId, opts => opts.MapFrom(x => x.UserId))
                .ForMember(x => x.Password, opts => opts.Ignore())
                .ForMember(x => x.Firstname, opts => opts.MapFrom(x => x.GivenName))
                .ForMember(x => x.Lastname, opts => opts.MapFrom(x => x.FamilyName))
                .ForMember(x => x.IsDummy, opts => opts.Ignore())
                .ForMember(x => x.LastLoginDate, opts => opts.Ignore())
                .ForMember(x => x.GlobalRole, opts => opts.Ignore())
                .ForMember(x => x.PhoneNumber, opts => opts.Ignore())
                .ForMember(x => x.ProfilePicUrl, opts => opts.MapFrom(x => x.Picture))
                .ForMember(x => x.Role, opts => opts.Ignore())
                .ForMember(x => x.SignatureFileUrl, opts => opts.Ignore())
                .ForMember(x => x.SubscriptionDate, opts => opts.MapFrom(x => x.CreatedAt));

            CreateMap<Dal.MongoImpl.Entities.Mission, MissionDto>()
               .ForMember(x => x.Events, opts => opts.Ignore())
               .ForMember(x => x.Activities, opts => opts.Ignore())
               .ForMember(x => x.Invoices, opts => opts.Ignore())
               .ForMember(x => x.FrameContract, opts => opts.Ignore());

            //CreateMap<UserModel, PartialUser>()
            //    .ForMember(x => x.Email, opts => opts.MapFrom(y => y.Email));
            //CreateMap<PartialUser, UserModel>()
            //    .ForMember(x => x.Email, opts => opts.MapFrom(y => y.Email))
            //    .ForMember(x => x.Address, opts => opts.Ignore())
            //    .ForMember(x => x.BankDetails, opts => opts.Ignore())
            //    .ForMember(x => x.CompanySiret, opts => opts.Ignore())
            //    .ForMember(x => x.GlobalRole, opts => opts.Ignore())
            //    .ForMember(x => x.IsDummy, opts => opts.Ignore())
            //    .ForMember(x => x.LastLoginDate, opts => opts.Ignore())
            //    .ForMember(x => x.LegalContributionFileUrl, opts => opts.Ignore())
            //    .ForMember(x => x.Password, opts => opts.Ignore())
            //    .ForMember(x => x.SignatureFileUrl, opts => opts.Ignore())
            //    .ForMember(x => x.Role, opts => opts.Ignore())
            //    .ForMember(x => x.Company, opts => opts.Ignore())
            //    .ForMember(x => x.PhoneNumber, opts => opts.Ignore())
            //    .ForMember(x => x.SubscriptionDate, opts => opts.Ignore());
            //CreateMap<PartialUser, UserDto>()
            //    .ForMember(x => x.Email, opts => opts.MapFrom(y => y.Email))
            //    .ForMember(x => x.Address, opts => opts.Ignore())
            //    .ForMember(x => x.Company, opts => opts.Ignore())
            //    .ForMember(x => x.BankDetails, opts => opts.Ignore())
            //    .ForMember(x => x.Role, opts => opts.Ignore())
            //    .ForMember(x => x.GlobalRole, opts => opts.Ignore())
            //    .ForMember(x => x.CompanySiret, opts => opts.Ignore())
            //    .ForMember(x => x.GlobalRole, opts => opts.Ignore())
            //    .ForMember(x => x.IsDummy, opts => opts.Ignore())
            //    .ForMember(x => x.LastLoginDate, opts => opts.Ignore())
            //    .ForMember(x => x.LegalContributionFileUrl, opts => opts.Ignore())
            //    .ForMember(x => x.Password, opts => opts.Ignore())
            //    .ForMember(x => x.PhoneNumber, opts => opts.Ignore())
            //    .ForMember(x => x.SignatureFileUrl, opts => opts.Ignore())
            //    .ForMember(x => x.SubscriptionDate, opts => opts.Ignore());
            //CreateMap<UserDto, PartialUser>()
            //  .ForMember(x => x.Email, opts => opts.MapFrom(y => y.Email));

            //CreateMap<Dal.SwaggerImpl.Model.Activity, ActivityDto>()
            //    .ForMember(x => x.CanModify, opt => opt.Ignore())
            //    .ForMember(x => x.CanSign, opt => opt.Ignore());

            CreateMap<Dal.MongoImpl.Entities.Event, EventDto>();
            CreateMap<EventDto, Dal.MongoImpl.Entities.Event>();
            //.ForMember(x => x.ContextType, opts => opts.MapFrom(y => y.ContextType));

            //CreateMap<Dal.SwaggerImpl.Model.BankDetails, BankDetailsDto>();
            //CreateMap<Dal.SwaggerImpl.Model.Address, AddressDto>();
            //CreateMap<RegisterCompanyRequestDto, Dal.SwaggerImpl.Model.Company>()
            //    .ForMember(x => x.Id, opts => opts.Ignore())
            //    .ForMember(x => x.BankDetails, opts => opts.Ignore());

            //CreateMap<Token, TokenDto>()
            //    .ForMember(x => x.UserFirstname, opts => opts.Ignore())
            //    .ForMember(x => x.UserLastname, opts => opts.Ignore())
            //    .ForMember(x => x.UserMail, opts => opts.Ignore())
            //    .ForMember(x => x.UserSubscriptionDate, opts => opts.Ignore());

            //CreateMap<Dal.SwaggerImpl.Model.User, UserDto>()
            //    .ForMember(x => x.LegalContributionFileUrl, opts => opts.AllowNull())
            //    .ForMember(x => x.BankDetails, opts => opts.AllowNull())
            //    .ForMember(x => x.SignatureFileUrl, opts => opts.AllowNull())
            //    .ForMember(x => x.Address, opts => opts.AllowNull())
            //    .ForMember(x => x.CompanySiret, opts => opts.AllowNull())
            //    .ForMember(x => x.Role, opts => opts.Ignore());
            //CreateMap<Dal.SwaggerImpl.Model.User, UserModel>()
            //    .ForMember(x => x.LegalContributionFileUrl, opts => opts.AllowNull())
            //    .ForMember(x => x.BankDetails, opts => opts.Ignore())
            //    .ForMember(x => x.Role, opts => opts.Ignore())
            //    .ForMember(x => x.SignatureFileUrl, opts => opts.AllowNull())
            //    .ForMember(x => x.Address, opts => opts.AllowNull());

            //CreateMap<UserModel, Dal.SwaggerImpl.Model.User>()
            //    .ForMember(x => x.LegalContributionFileUrl, opts => opts.AllowNull())
            //    .ForMember(x => x.Company, opts => opts.Ignore())
            //    .ForMember(x => x.BankDetails, opts => opts.Ignore())
            //    .ForMember(x => x.SignatureFileUrl, opts => opts.AllowNull())
            //    .ForMember(x => x.Address, opts => opts.AllowNull());

            CreateMap<UserModel, UserDto>()
                .ForMember(x => x.ExternalId, opts => opts.Ignore())
                .ForMember(x => x.Role, opts => opts.Ignore())
                .ForMember(x => x.SignatureFileUrl, opts => opts.AllowNull())
                .ForMember(x => x.Password, opts => opts.Ignore());

            //CreateMap<UserAggregate, UserDto>()
            //    .ForMember(x => x.Role, opts => opts.Ignore());
        }

        private static double ConvertDayPartToNumber(Dal.MongoImpl.Entities.GridDay d)
        {
            switch (d.WorkedPart)
            {
                case Dal.MongoImpl.Entities.DayPartEnum.Afternoon:
                case Dal.MongoImpl.Entities.DayPartEnum.Morning:
                    return 0.5;
                case Dal.MongoImpl.Entities.DayPartEnum.Full:
                    return 1;
                default:
                    return 0;
            }
        }
    }
}