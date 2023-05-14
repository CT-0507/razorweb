using Microsoft.AspNetCore.Identity;

namespace App.Services
{
    public class AppIdentityErrorDescriber : IdentityErrorDescriber
    {
        public AppIdentityErrorDescriber()
        {
        }
        public override IdentityError ConcurrencyFailure()
        {

            return base.ConcurrencyFailure();
        }

        public override IdentityError DefaultError()
        {
            return base.DefaultError();
        }
        public override IdentityError DuplicateEmail(string email)
        {
            return base.DuplicateEmail(email);
        }
        public override IdentityError DuplicateRoleName(string role)
        {
            var er = base.DuplicateRoleName(role);
            return new IdentityError()
            {
                Code = er.Code,
                Description = $"Role có tên {role} bị trùng"
            };
        }

        public override IdentityError DuplicateUserName(string userName)
        {
            return base.DuplicateUserName(userName);
        }
    }
}