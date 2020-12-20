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
        public async void Detail_IdIsNull_ReturnRedirectToAction()
        {

            var result = await _controller.Details(null);
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

        [Fact]
        public void Create_ActionExecutes_ReturnView()
        {
            var result = _controller.Create();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void CreatePOST_InValidModelState_ReturnView()
        {
            //Controller create post metodunda,model.Isvalid ile name alanının dolu olması vs. kontrol edilmiş
            _controller.ModelState.AddModelError("Name", "Name alanı gereklidir");
            //Yukarıda products listesinde "name" alanına veri girilmemiş
            var result = await _controller.Create(products.First());
            //controller'da model.IsValid degilse;return View yapılıyor;Model.IsValid is return RedirectToAction yapılıyor
            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.IsType<Product>(viewResult.Model);
        }
        [Fact]
        public async void CreatePOST_ValidModelState_ReturnRedirectToIndex()
        {
            var result = await _controller.Create(products.First());
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        //Controller CreateProduct post metodunun içinde service'teki db'ye kaydetme metodunun testi
        [Fact]
        public async void CreatePOST_ValidModelState_CreateMethodExecute()
        {
            Product newProduct = null;
            _mockService.Setup(ser => ser.AddProductAsync(It.IsAny<Product>())).Callback<Product>(x => newProduct = x);

            var result = await _controller.Create(products.First());

            _mockService.Verify(x => x.AddProductAsync(It.IsAny<Product>()), Times.Once);

            Assert.Equal(products.First().Id, newProduct.Id);

        }

        [Fact]
        public async void CreatePOST_InValidModelState_NeverCreateExecute()
        {
            _controller.ModelState.AddModelError("Name", "");
            var result = await _controller.Create(products.First());

            _mockService.Verify(repo => repo.AddProductAsync(It.IsAny<Product>()), Times.Never);

        }

        [Fact]
        public async void Edit_IdIsNull_ReturnRedirectoAction()
        {
            var result = await _controller.Edit(null);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);


        }

        //Edit metoduna id'nin null gelmesi durumunda return Index yapması kontrolü
        [Fact]
        public async void Edit_IdIsNullRedirectToIndexAction()
        {
            var result = await _controller.Edit(null);

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);


        }

        //productId=3 database'de yok. Mock productList'te var ama.
        [Theory]
        [InlineData(3)]
        public async void Edit_IdInValid_ReturnNotFound(int productId)
        {

            Product product = null;
            _mockService.Setup(x => x.ProductByIdAsync(productId)).ReturnsAsync(product);

            var result = await _controller.Edit(productId);

            var redirect = Assert.IsType<NotFoundResult>(result);

            Assert.Equal<int>(404, redirect.StatusCode);
        }

        [Theory]
        [InlineData(2)]
        public async void Edit_ActionExecute_ReturnProduct(int productId)
        {
            var product = products.First(x => x.Id == productId);

            _mockService.Setup(x => x.ProductByIdAsync(productId)).ReturnsAsync(product);

            var result = await _controller.Edit(productId);

            var viewResult = Assert.IsType<ViewResult>(result); ;

            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);

            Assert.Equal(product.Id, resultProduct.Id);
            Assert.Equal(product.Price, resultProduct.Price);
        }

        //ProductController-Edit post metoduna ilk parametre productId=1 gönderiliyor.İkinci productParamateresine productId 2 gönderiliyor.Yani güncellenen ve güncellenecek id'ler esit gönderilmiyor.
        [Theory]
        [InlineData(1)]
        public async void EditPOST_IdIsNotEqualProduct_ReturnNotFound(int productId)
        {
            var result = await _controller.Edit(2, products.First(x => x.Id == productId));

            var redirect = Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public async void EditPOST_InValidModelState_ReturnView(int productId)
        {
            _controller.ModelState.AddModelError("Price", "");

            var result = await _controller.Edit(productId, products.First(x => x.Id == productId));

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.IsType<Product>(viewResult.Model);

        }

        [Theory]
        [InlineData(1)]
        public async void EditPOST_ValidModelState_ReturnRedirectToIndexAction(int productId)
        {
            var result = await _controller.Edit(productId, products.First(x => x.Id == productId));

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Theory]
        [InlineData(1)]
        public async void EditPOST_ValidModelState_UpdateMethodExecute(int productId)
        {
            var product = products.First(x => x.Id == productId);
            _mockService.Setup(x => x.UpdateProductAsync(product));

            await _controller.Edit(productId, product);

            _mockService.Verify(repo => repo.UpdateProductAsync(It.IsAny<Product>()), Times.Once);
        }


        [Fact]
        public async void Delete_IdIsNull_ReturnNotFound()
        {
            var result = await _controller.Delete(null);
            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(0)]
        public async void Delete_IdIsNotEqualProduct_ReturnNotFound(int productId)
        {
            Product product = null;

            _mockService.Setup(x => x.ProductByIdAsync(productId)).ReturnsAsync(product);

            var result = await _controller.Delete(productId);

            Assert.IsType<NotFoundResult>(result);
        }


        [Theory]
        [InlineData(2)]
        public async void Delete_ActionExecute_ReturnProduct(int productId)
        {

            var product = products.First(x => x.Id == productId);

            _mockService.Setup(x => x.ProductByIdAsync(productId)).ReturnsAsync(product);

            var result = await _controller.Delete(productId);

            //Dönen tip View'mı onun testi yapılıyor
            var viewResult=Assert.IsType<ViewResult>(result);

            //Dönen tip; product'mı diyekontrol ediliyor.(view(product)
            Assert.IsAssignableFrom<Product>(viewResult.Model);
        }

        [Theory]
        [InlineData(2)]
        public async void DeleteConfirmed_ActionExecute_DeleteMethodExecute(int productId)
        {

            var result = await _controller.DeleteConfirmed(productId);

            //Dönen tip redirectToAction View'mı onun testi yapılıyor
            Assert.IsType<RedirectToActionResult>(result);

        }


        [Theory]
        [InlineData(2)]
        public async void DeleteConfirmed_ActionExecute_ReturnToIndexAction(int productId)
        {

            var product = products.First(x => x.Id == productId);

            _mockService.Setup(x => x.DeleteProductAsync(productId));

            await _controller.DeleteConfirmed(productId);

             _mockService.Verify(repo => repo.DeleteProductAsync(productId), Times.Once);

        }
    }
}
