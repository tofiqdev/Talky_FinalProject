using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DataTransferObject.BlockedUserDTO
{
    public class BlockedUserListDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int BlockedUserId { get; set; }
        public string BlockedUserName { get; set; }
        public DateTime BlockedAt { get; set; }
    }
}
