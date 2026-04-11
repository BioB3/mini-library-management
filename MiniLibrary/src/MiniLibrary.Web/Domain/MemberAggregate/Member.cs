namespace MiniLibrary.Web.Domain.MemberAggregate;

public class Member : EntityBase<Member, MemberId>, IAggregateRoot
{
  public string UserId { get; private set; }
  public string FirstName { get; private set; }
  public string LastName { get; private set; }
  public DateTime JoinedAt { get; private set; } = DateTime.UtcNow;

  public Member(MemberId id, string userId, string firstName, string lastName)
  {
    Id = id;
    UserId = userId;
    FirstName = firstName;
    LastName = lastName;
  }

  public static Member Create(string userId, string firstName, string lastName)
  {
    return new Member(MemberId.From(Guid.NewGuid()), userId, firstName, lastName);
  }

  public void Update(string firstName, string lastName)
  {
    FirstName = firstName;
    LastName = lastName;
  }
}
