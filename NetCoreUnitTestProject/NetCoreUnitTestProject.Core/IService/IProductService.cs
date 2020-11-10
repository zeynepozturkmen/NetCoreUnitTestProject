using NetCoreUnitTestProject.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreUnitTestProject.Core.IService
{
    public interface IProductService : IBaseService<Product>
    {
        Task<Product> AddProductAsync(Product model);
        Task<Product> ProductByIdAsync(int id);
        Task<Product> UpdateProductAsync(Product model);
        Task<Product> DeleteProductAsync(int Id);

        Task<List<Product>> GetAllProductAsync();
    }
}
