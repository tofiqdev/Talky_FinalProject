using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DataTransferObject.BlockedUserDTO
{
    public class BlockedUserAddDTO
    {
        public int UserId { get; set; }
        public int BlockedUserId { get; set; }
    }
}
