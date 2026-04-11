using Vogen;

namespace MiniLibrary.Web.Domain.MemberAggregate;

[ValueObject<Guid>]
public readonly partial struct MemberId
{
  private static Validation Validate(Guid value)
      => value != Guid.Empty ? Validation.Ok : Validation.Invalid("MemberId cannot be empty.");
}
