using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.User.Common;

namespace Tickefy.Application.User.GetByLogin
{
    public class GetUserByLoginQuery : IQuery<UserDetailsResult>
    {
        public string Login { get; set; }
        
        public GetUserByLoginQuery(string login)
        {
            Login = login;
        }
    }
}