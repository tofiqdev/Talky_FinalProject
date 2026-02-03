using Core.Entities;
using Entity.TableModel.Membership;
using Entity.TableModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.TableModel
{
    public class User : BaseEntity
    {
        
        public int? AppUserId { get; set; } 
        public AppUser? AppUser { get; set; }

       
        public string Name {  get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? Avatar { get; set; }
        public string? Bio { get; set; }
        public bool IsOnline { get; set; } = false;
        public bool IsCEO { get; set; } = false;
        public DateTime? LastSeen { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

      
        public ICollection<Message> SentMessages { get; set; } = new List<Message>();
        public ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();
        public ICollection<Call> InitiatedCalls { get; set; } = new List<Call>();
        public ICollection<Call> ReceivedCalls { get; set; } = new List<Call>();
    }
}