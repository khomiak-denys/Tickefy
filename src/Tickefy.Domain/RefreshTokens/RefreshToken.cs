using Tickefy.Domain.Common.EntityBase;
using Tickefy.Domain.Primitives;
using Tickefy.Domain.Users;

namespace Tickefy.Domain.RefreshTokens;

public class RefreshToken : EntityBase<TokenId>
{
    public UserId UserId { get; private set; } = null!;
    public DateTime Expires { get; private set; }
    public string Token { get; private set; } = null!;
    public User User { get; private set; } = null!;

    private RefreshToken() { }
    private RefreshToken(UserId userId, DateTime expires, string token)
    {
        Id = new TokenId();
        UserId = userId;
        Expires = expires;
        Token = token;
    }

    public static RefreshToken Create(UserId userId, DateTime expires, string token)
    {
        var entity = new RefreshToken(userId, expires, token);
        entity.OnCreate();
        return entity;
    }
}
