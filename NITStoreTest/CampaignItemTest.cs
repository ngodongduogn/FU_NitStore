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
    public class CampaignItemTest
    {
        private DbContextOptions<NitDbContext> _options;

        [SetUp]
        public void Setup()
        {
            _options = new DbContextOptionsBuilder<NitDbContext>()
                .UseInMemoryDatabase(databaseName: "testDatabase")
                .Options;
        }

        [Test]
        public void CreateCampaignItem_AddNewCampaignItemToDatabase()
        {
            // Arrange
            using var context = new NitDbContext(_options);
            var campaignItem = new CampaignItem
            {
                ProductId = 1,
                CampaignId = 1,
                Discount = 1
            };

            // Act
            CampaignItemsController repo = new CampaignItemsController(context);
            var resultTask = repo.AddCampaignItem(campaignItem);
            resultTask.Wait();
            bool result = resultTask.Result;
            // Assert
            Assert.AreEqual(result, true);
        }

        [Test]
        public void CreateCampaignItemError_AddNewCampaignItemToDatabase()
        {
            // Arrange
            using var context = new NitDbContext(_options);
            var campaignItem = new CampaignItem
            {
                Discount= -1,
            };

            // Act
            CampaignItemsController repo = new CampaignItemsController(context);
            var resultTask = repo.AddCampaignItem(campaignItem);
            resultTask.Wait();
            bool result = resultTask.Result;
            // Assert
            Assert.AreEqual(result, false);
        }

        [Test]
        public void ReadCampaignItems_GetsCampaignItemsFromDatabase()
        {
            // Arrange
            using var context = new NitDbContext(_options);
            var campaignItem = new CampaignItem
            {
                ProductId = 1,
                CampaignId = 1,
                Discount = 1
            };
            context.campaignItems.Add(campaignItem);
            context.SaveChanges();

            // Act
            var savedCampaign = context.campaignItems.FirstOrDefault(c => c.Id == campaignItem.Id);

            // Assert
            Assert.NotNull(savedCampaign);
            Assert.AreEqual(campaignItem.ProductId, savedCampaign.ProductId);
            Assert.AreEqual(campaignItem.CampaignId, savedCampaign.CampaignId);
            Assert.AreEqual(campaignItem.Discount, savedCampaign.Discount);
        }

        [Test]
        public void UpdateCampaignItems_UpdateCampaignItemsInDatabase()
        {
            // Arrange
            using var context = new NitDbContext(_options);
            var campaignItem = new CampaignItem
            {
                ProductId = 1,
                CampaignId = 1,
                Discount = 1
            };
            context.campaignItems.Add(campaignItem);
            context.SaveChanges();

            // Act
            var savedCampaignItem = context.campaignItems.FirstOrDefault(c => c.Id == campaignItem.Id);
            savedCampaignItem.ProductId = 2;
            savedCampaignItem.CampaignId = 2;
            savedCampaignItem.Discount = 2;
            context.SaveChanges();

            // Assert
            var updatedCampaignItem = context.campaignItems.FirstOrDefault(c => c.Id == campaignItem.Id);
            Assert.NotNull(updatedCampaignItem);
            Assert.AreEqual(savedCampaignItem.ProductId, updatedCampaignItem.ProductId);
            Assert.AreEqual(savedCampaignItem.CampaignId, updatedCampaignItem.CampaignId);
            Assert.AreEqual(savedCampaignItem.Discount, updatedCampaignItem.Discount);
        }

        [Test]
        public void DeleteCampaignItems_RemovesCampaignItemsFromDatabase()
        {
            // Arrange
            using var context = new NitDbContext(_options);
            var campaignItem = new CampaignItem
            {
                ProductId = 1,
                CampaignId = 1,
                Discount = 1
            };
            context.campaignItems.Add(campaignItem);
            context.SaveChanges();

            // Act
            context.campaignItems.Remove(campaignItem);
            context.SaveChanges();

            // Assert
            var campaignItemsD = context.campaignItems.FirstOrDefault(c => c.Id == campaignItem.Id);
            Assert.Null(campaignItemsD);
        }
    }
}
