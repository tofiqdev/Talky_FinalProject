namespace TalkyAPI.DTOs.Group
{
    public class GroupDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Avatar { get; set; }
        public int CreatedById { get; set; }
        public string CreatedByUsername { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<GroupMemberDto> Members { get; set; } = new List<GroupMemberDto>();
        public int MemberCount { get; set; }
    }

    public class GroupMemberDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? Avatar { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsOnline { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}
