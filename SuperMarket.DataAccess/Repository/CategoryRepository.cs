using SuperMarket.DataAccess.Data;
using SuperMarket.DataAccess.Repository.IRepository;
using SuperMarket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMarket.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _db;

        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async void Save()
        {
            await _db.SaveChangesAsync();
        }

        public void Update(Category obj)
        {
            _db.Categories.Update(obj);
        }
    }
}
