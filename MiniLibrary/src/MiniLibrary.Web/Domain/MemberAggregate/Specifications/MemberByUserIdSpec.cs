namespace MiniLibrary.Web.Domain.MemberAggregate.Specifications;

public class MemberByUserIdSpec : Specification<Member>
{
  public MemberByUserIdSpec(string userId) =>
    Query.Where(member => member.UserId == userId);
}
