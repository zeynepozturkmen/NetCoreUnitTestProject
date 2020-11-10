using Microsoft.AspNetCore.Mvc;
using Moq;
using NetCoreUnitTestProject.Core.Entities;
using NetCoreUnitTestProject.Core.IService;
using NetCoreUnitTestProject.UI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace NetCoreUnitTestProject.Test
{
    public class ProductControllerTest
    {
        private readonly Mock<IProductService> _mockService;
        private readonly ProductsController _controller;
        private List<Product> products;
        public ProductControllerTest()
        {
            _mockService = new Mock<IProductService>();
            //ProductController mock'landı
            _controller = new ProductsController(_mockService.Object);
            products = new List<Product>() { new Product { Id = 1, Price = 10, CreateDate = DateTime.Now }, new Product { Id = 2, Price = 20, CreateDate = DateTime.Now }, new Product { Id = 3, Price = 30, CreateDate = DateTime.Now } };
        }
        [Fact]
        public async void Index_Action_ReturnView()
        {
            var result = await _controller.Index();
            //Index'in dönüş tipi ViewResult mı test ediliyor
            Assert.IsType<ViewResult>(result);
        }
        [Fact]
        public async void Index_ActionExecutes_ReturnProductList()
        {
            _mockService.Setup(x => x.GetAllProductAsync()).ReturnsAsync(products);

            var result = await _controller.Index();
            var viewResult = Assert.IsType<ViewResult>(result);
            //ViewResult'ın modeli bir ProductList'mi o kontrol ediliyor
            var productList = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.Model);
            //Gelen product sayısı 2'mi
            Assert.Equal<int>(3, productList.Count());
        }

        [Fact]
        public async void Detail_IdIsNull_ReturnRedirectToAction()  {

            var result =await _controller.Details(null);
            //id null is redirecToAction yapacak mı
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            //Index ActionName mi dönecek
            Assert.Equal("Index", redirect.ActionName);        
        }

        [Fact]
        public async void Detail_IdInValid_ReturnNotFound()
        {
            Product product = null;
            _mockService.Setup(x => x.ProductByIdAsync(0)).ReturnsAsync(product);

            var result = await _controller.Details(0);
            var redirect = Assert.IsType<NotFoundResult>(result);
            Assert.Equal<int>(404, redirect.StatusCode);
        }


        [Theory]
        [InlineData(1)]
        public async void Detail_ValidId_ReturnProduct(int productId)
        {
            Product product = products.First(x => x.Id == productId);

            _mockService.Setup(x => x.ProductByIdAsync(productId)).ReturnsAsync(product);

            var result = await _controller.Details(productId);

            var viewResult = Assert.IsType<ViewResult>(result);

            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);

            Assert.Equal(product.Id, resultProduct.Id);
            Assert.Equal(product.Price, resultProduct.Price);
        }

    }
}
