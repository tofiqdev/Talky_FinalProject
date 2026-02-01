using AutoMapper;
// using Entity.TableModel; removed
using Entity.DataTransferObject.BlockedUserDTO;
using Entity.DataTransferObject.CallDTO;
using Entity.DataTransferObject.ContactDTO;
using Entity.DataTransferObject.GroupDTO;
using Entity.DataTransferObject.GroupMenmberDTO;
using Entity.DataTransferObject.GroupmessageDTO;
using Entity.DataTransferObject.MessageDTO;
using Entity.DataTransferObject.StoryDTO;
using Entity.DataTransferObject.StoryViewDTO;
using Entity.DataTransferObject.UserDTO;
using Entity.TableModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Mapper
{
    public class Map : Profile
    {
        public Map()
        {
            CreateMap<User, UserListDTO>().ReverseMap();
            CreateMap<UserAddDTO, User>()
                .ForMember(dest => dest.AppUser, opt => opt.Ignore())
                .ForMember(dest => dest.SentMessages, opt => opt.Ignore())
                .ForMember(dest => dest.ReceivedMessages, opt => opt.Ignore())
                .ForMember(dest => dest.InitiatedCalls, opt => opt.Ignore())
                .ForMember(dest => dest.ReceivedCalls, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<UserUpdateDTO, User>()
                .ForMember(dest => dest.AppUser, opt => opt.Ignore())
                .ForMember(dest => dest.SentMessages, opt => opt.Ignore())
                .ForMember(dest => dest.ReceivedMessages, opt => opt.Ignore())
                .ForMember(dest => dest.InitiatedCalls, opt => opt.Ignore())
                .ForMember(dest => dest.ReceivedCalls, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<Call, CallListDTO>().ReverseMap();
            CreateMap<CallAddDTO, Call>()
                .ForMember(dest => dest.Caller, opt => opt.Ignore())
                .ForMember(dest => dest.Receiver, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<CallUpdateDTO, Call>()
                .ForMember(dest => dest.Caller, opt => opt.Ignore())
                .ForMember(dest => dest.Receiver, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<Message, MessageListDTO>().ReverseMap();
            CreateMap<MessageAddDTO, Message>()
                .ForMember(dest => dest.Sender, opt => opt.Ignore())
                .ForMember(dest => dest.Receiver, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<MessageUpdateDTO, Message>()
                .ForMember(dest => dest.Sender, opt => opt.Ignore())
                .ForMember(dest => dest.Receiver, opt => opt.Ignore())
                .ReverseMap();


            CreateMap<GroupMemberAddDTO, GroupMember>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Group, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<GroupMemberUpdateDTO, GroupMember>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Group, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<GroupMember, GroupMemberListDTO>().ReverseMap();
            CreateMap<GroupMemberListDTO, GroupMemberUpdateDTO>().ReverseMap();

            CreateMap<Group, GroupListDTO>()
                .ForMember(dest => dest.CreatedByUsername, opt => opt.MapFrom(src => src.CreatedBy.Username))
                .ReverseMap();
            CreateMap<GroupListDTO, GroupUpdateDTO>().ReverseMap();
            CreateMap<GroupAddDTO, Group>()
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Members, opt => opt.Ignore())
                .ForMember(dest => dest.Messages, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<GroupUpdateDTO, Group>()
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Members, opt => opt.Ignore())
                .ForMember(dest => dest.Messages, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<GroupMessage, GroupmessageListDTO>().ReverseMap();
            CreateMap<GroupmessageAddDTO, GroupMessage>()
                .ForMember(dest => dest.Group, opt => opt.Ignore())
                .ForMember(dest => dest.Sender, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<GroupmessageUpdateDTO, GroupMessage>()
                .ForMember(dest => dest.Group, opt => opt.Ignore())
                .ForMember(dest => dest.Sender, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<BlockedUser, BlockedUserListDTO>().ReverseMap();
            CreateMap<BlockedUserAddDTO, BlockedUser>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.BlockedUserNavigation, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<BlockedUserUpdateDTO, BlockedUser>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.BlockedUserNavigation, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<Contact, ContactListDTO>().ReverseMap();
            CreateMap<ContactAddDTO, Contact>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.ContactUser, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<ContactUpdateDTO, Contact>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.ContactUser, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<Story, StoryListDTO>().ReverseMap();
            CreateMap<StoryAddDTO, Story>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Views, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<StoryUpdateDTO, Story>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Views, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<StoryView, StoryViewListDTO>().ReverseMap();
            CreateMap<StoryViewAddDTO, StoryView>()
                .ForMember(dest => dest.Story, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ReverseMap();
        }

    }
}
