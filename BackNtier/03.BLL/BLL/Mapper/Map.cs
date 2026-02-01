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
            CreateMap<UserAddDTO, User>().ReverseMap();
            CreateMap<UserUpdateDTO, User>().ReverseMap();

            CreateMap<Call, CallListDTO>().ReverseMap();
            CreateMap<CallAddDTO, Call>().ReverseMap();
            CreateMap<CallUpdateDTO, Call>().ReverseMap();

            CreateMap<Message, MessageListDTO>().ReverseMap();
            CreateMap<MessageAddDTO, Message>().ReverseMap();
            CreateMap<MessageUpdateDTO, Message>().ReverseMap();


            CreateMap<GroupMemberAddDTO, GroupMember>().ReverseMap();
            CreateMap<GroupMemberUpdateDTO, GroupMember>().ReverseMap();
            CreateMap<GroupMember, GroupMemberListDTO>().ReverseMap();

            CreateMap<Group, GroupListDTO>().ReverseMap();
            CreateMap<GroupAddDTO, Group>().ReverseMap();
            CreateMap<GroupUpdateDTO, Group>().ReverseMap();

            CreateMap<GroupMessage, GroupmessageListDTO>().ReverseMap();
            CreateMap<GroupmessageAddDTO, GroupMessage>().ReverseMap();
            CreateMap<GroupmessageUpdateDTO, GroupMessage>().ReverseMap();

            CreateMap<BlockedUser, BlockedUserListDTO>().ReverseMap();
            CreateMap<BlockedUserAddDTO, BlockedUser>().ReverseMap();
            CreateMap<BlockedUserUpdateDTO, BlockedUser>().ReverseMap();

            CreateMap<Contact, ContactListDTO>().ReverseMap();
            CreateMap<ContactAddDTO, Contact>().ReverseMap();
            CreateMap<ContactUpdateDTO, Contact>().ReverseMap();

            CreateMap<Story, StoryListDTO>().ReverseMap();
            CreateMap<StoryAddDTO, Story>().ReverseMap();
            CreateMap<StoryUpdateDTO, Story>().ReverseMap();

            CreateMap<StoryView, StoryViewListDTO>().ReverseMap();
            CreateMap<StoryViewAddDTO, StoryView>().ReverseMap();
        }

    }
}
