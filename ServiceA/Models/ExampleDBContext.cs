using Microsoft.EntityFrameworkCore;

namespace ServiceA.Models
{
    public class ExampleDBContext : DbContext
    {
        public ExampleDBContext(DbContextOptions options) : base(options)
        {
        }
    }
}
