using Core.Entities;
using Entity.TableModel;
using System;
using System.Collections.Generic;

namespace Entity.TableModel
{
    public class Group : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Avatar { get; set; }
        public int CreatedById { get; set; }
        public bool IsMutedForAll { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public User CreatedBy { get; set; } = null!;
        public ICollection<GroupMember> Members { get; set; } = new List<GroupMember>();
        public ICollection<GroupMessage> Messages { get; set; } = new List<GroupMessage>();
    }
}
