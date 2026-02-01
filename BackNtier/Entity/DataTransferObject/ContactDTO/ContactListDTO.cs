using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DataTransferObject.ContactDTO
{
    public class ContactListDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int ContactUserId { get; set; }
        public string ContactUserName { get; set; }
        public string? ContactUserAvatar { get; set; }
        public bool ContactUserIsOnline { get; set; }
        public DateTime AddedAt { get; set; }
    }
}
