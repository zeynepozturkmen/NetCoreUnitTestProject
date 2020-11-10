using Microsoft.EntityFrameworkCore;
using NetCoreUnitTestProject.Core.Entities;
using NetCoreUnitTestProject.Core.IService;
using NetCoreUnitTestProject.Data.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreUnitTest.Service.Service
{
    public class ProductService : BaseService<Product>, IProductService
    {
        public ProductService(UnitTestDbContext dbContext) : base(dbContext)
        {

        }
        public async Task<Product> AddProductAsync(Product model)
        {
            var product = new Product();
            product.Price = model.Price;
            product.CreateDate = DateTime.Now;

            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();

            return product;
        }

        public async Task<Product> ProductByIdAsync(int Id)
        {
            var product = await _dbContext.Products.Where(x => x.Id ==Id).FirstOrDefaultAsync();

            return product;
        }
        public async Task<Product> UpdateProductAsync(Product model)
        {
            var product =await _dbContext.Products.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
            product.Price = model.Price;
            product.CreateDate = DateTime.Now;

            await _dbContext.SaveChangesAsync();

            return product;
        }

        public async Task<Product> DeleteProductAsync(int Id)
        {
            var product = await _dbContext.Products.Where(x => x.Id == Id).FirstOrDefaultAsync();
            product.IsDeleted = true;

            await _dbContext.SaveChangesAsync();

            return product;
        }

        public async Task<List<Product>> GetAllProductAsync()
        {
            var products = await _dbContext.Products.ToListAsync();
            return products;
        }

    }
}
