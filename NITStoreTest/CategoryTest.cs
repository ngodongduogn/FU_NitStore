using Microsoft.EntityFrameworkCore;
using NitStore.Controllers;
using NitStore.Data;
using NitStore.Models.Domain;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assert = NUnit.Framework.Assert;

namespace NITStoreTest
{
    [TestFixture]
    public class CategoryTest
    {
        private DbContextOptions<NitDbContext> _options;

        [SetUp]
        public void Setup()
        {
            _options = new DbContextOptionsBuilder<NitDbContext>()
                .UseInMemoryDatabase(databaseName: "NIT")
                .Options;
        }

        [Test]
        public void CreateCategory_AddsNewCategoryToDatabase()
        {
            // Arrange
            using var context = new NitDbContext(_options);
            var category = new Category
            {
                Name = "Test Category",
                Description = "This is a test category"
            };

            // Act
            CategoriesController repo = new CategoriesController(context);
            var resultTask = repo.AddCategory(category);
            resultTask.Wait();
            bool result = resultTask.Result;

            // Assert
            Assert.AreEqual(result, true);
        }

        [Test]
        public void CreateCategoryError_AddsNewCategoryToDatabaseFailed()
        {
            // Arrange
            using var context = new NitDbContext(_options);
            var category = new Category
            {
                Name = "",
                Description = ""
            };

            // Act
            CategoriesController repo = new CategoriesController(context);
            var resultTask = repo.AddCategory(category);
            resultTask.Wait();
            bool result = resultTask.Result;
            // Assert
            Assert.AreEqual(result, false);
        }

        [Test]
        public void ReadCategory_GetsCategoryFromDatabase()
        {
            // Arrange
            using var context = new NitDbContext(_options);
            var category = new Category
            {
                Name = "Test Category",
                Description = "This is a test category"
            };
            context.categories.Add(category);
            context.SaveChanges();

            // Act
            var savedCategory = context.categories.FirstOrDefault(c => c.Id == category.Id);

            // Assert
            Assert.NotNull(savedCategory);
            Assert.AreEqual(category.Name, savedCategory.Name);
            Assert.AreEqual(category.Description, savedCategory.Description);
        }

        [Test]
        public void UpdateCategory_UpdatesCategoryInDatabase()
        {
            // Arrange
            using var context = new NitDbContext(_options);
            var category = new Category
            {
                Name = "Test Category",
                Description = "This is a test category"
            };
            context.categories.Add(category);
            context.SaveChanges();

            // Act
            var savedCategory = context.categories.FirstOrDefault(c => c.Id == category.Id);
            savedCategory.Name = "Updated Test Category";
            savedCategory.Description = "This is an updated test category";
            CategoriesController repo = new CategoriesController(context);
            repo.EditCategory(savedCategory);

            // Assert
            var updatedCategory = context.categories.FirstOrDefault(c => c.Id == category.Id);
            Assert.NotNull(updatedCategory);
            Assert.AreEqual(savedCategory.Name, updatedCategory.Name);
            Assert.AreEqual(savedCategory.Description, updatedCategory.Description);
        }

        [Test]
        public void DeleteCategory_RemovesCategoryFromDatabase()
        {
            // Arrange
            using var context = new NitDbContext(_options);
            var category = new Category
            {
                Name = "Test Category",
                Description = "This is a test category"
            };
            context.categories.Add(category);
            context.SaveChanges();

            // Act
            context.categories.Remove(category);
            context.SaveChanges();

            // Assert
            var deletedCategory = context.categories.FirstOrDefault(c => c.Id == category.Id);
            Assert.Null(deletedCategory);
        }
    }
}
