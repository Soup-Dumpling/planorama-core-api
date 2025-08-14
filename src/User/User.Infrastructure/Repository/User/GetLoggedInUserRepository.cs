using Microsoft.EntityFrameworkCore;
using Planorama.User.Core.UseCases.User.GetLoggedInUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planorama.User.Infrastructure.Repository.User
{
    public class GetLoggedInUserRepository : IGetLoggedInUserRepository
    {
        private readonly UserDBContext context;

        public GetLoggedInUserRepository(UserDBContext context)
        {
            this.context = context;
        }

        public async Task<GetLoggedInUserViewModel> GetLoggedInUserByEmail(string email)
        {
            var result = await context.Users.Where(x => x.UserCredential.EmailAddress == email).Select(x => new GetLoggedInUserViewModel()
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                EmailAddress = x.UserCredential.EmailAddress
            }).AsNoTracking().FirstOrDefaultAsync();
            return result;
        }
    }
}
