using Microsoft.EntityFrameworkCore;
using RedisCacheDemo.Model;

namespace RedisCacheDemo.DBContext
{
    public class DBContextClass : DbContext
    {
        public DBContextClass(DbContextOptions<DBContextClass> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
    }
}
