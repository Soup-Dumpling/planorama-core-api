using Microsoft.EntityFrameworkCore;
using System;

namespace Planorama.User.Infrastructure.UnitTests.Helpers
{
    public static class InMemoryContextHelper
    {
        public static UserDBContext GetContext()
        {
            var builder = new DbContextOptionsBuilder<UserDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());
            return new UserDBContext(builder.Options);
        }
    }
}
