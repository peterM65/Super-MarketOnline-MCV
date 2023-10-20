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
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Product obj)
        {
            var objFromDb = _db.Products.FirstOrDefault(p => p.Id == obj.Id);
            if (objFromDb != null) 
            {
                objFromDb.Title = obj.Title;
                objFromDb.ASIN = obj.ASIN;
                objFromDb.Price = obj.Price;
                objFromDb.Price12 = obj.Price12;
                objFromDb.ListPrice = obj.ListPrice;
                objFromDb.Price24 = obj.Price24;
                objFromDb.Description = obj.Description;
                objFromDb.CategoryId = obj.CategoryId;
                objFromDb.Brand = obj.Brand;
                if (obj.ImageUrl != null)
                {
                    objFromDb.ImageUrl = obj.ImageUrl;
                }
            }
            
        }
    }
}
