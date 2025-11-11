namespace FinanceApp.Shared.Models;

public class Membership
{
    public Guid UserId { get; set; }
    public Guid OrganizationId { get; set; }
    public MemberRole Role { get; set; }
    public DateTime CreatedAt { get; set; }

    public Organization? Organization { get; set; }
}

public enum MemberRole
{
    Owner,
    Admin,
    Member
}
