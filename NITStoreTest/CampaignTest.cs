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
    public class CampaignTest
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
        public void CreateCampaign_AddNewCampaignToDatabase()
        {
            // Arrange
            using var context = new NitDbContext(_options);
            var campaign = new Campaign
            {
                Name= "Test Campaign",
                Description = "This is a Test",
                StartDate= DateTime.Now,
                EndDate= DateTime.Now
            };

            // Act
            CampaignsController repo = new CampaignsController(context);
            var resultTask = repo.AddCampaign(campaign);
            resultTask.Wait();
            bool result = resultTask.Result;
            // Assert
            Assert.AreEqual(result, true);
        }

        [Test]
        public void CreateCampaignError_AddNewCampaignToDatabase()
        {
            // Arrange
            using var context = new NitDbContext(_options);
            var campaign = new Campaign
            {
                Name = "",
                Description = "This is a Test",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now
            };

            // Act
            CampaignsController repo = new CampaignsController(context);
            var resultTask = repo.AddCampaign(campaign);
            resultTask.Wait();
            bool result = resultTask.Result;
            // Assert
            Assert.AreEqual(result, false);
        }

        [Test]
        public void ReadCampaign_GetsCampaignFromDatabase()
        {
            // Arrange
            using var context = new NitDbContext(_options);
            var campaign = new Campaign
            {
                Name = "Test Campaign",
                Description = "This is a Test",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now
            };
            context.campaigns.Add(campaign);
            context.SaveChanges();

            // Act
            var savedCampaign = context.campaigns.FirstOrDefault(c => c.Id == campaign.Id);

            // Assert
            Assert.NotNull(savedCampaign);
            Assert.AreEqual(campaign.Name, savedCampaign.Name);
            Assert.AreEqual(campaign.Description, savedCampaign.Description);
            Assert.AreEqual(campaign.StartDate, savedCampaign.StartDate);
            Assert.AreEqual(campaign.EndDate, savedCampaign.EndDate);
        }

        [Test]
        public void UpdateCampaign_UpdatesCampaignInDatabase()
        {
            // Arrange
            using var context = new NitDbContext(_options);
            var campaign = new Campaign
            {
                Name = "Test Campaign",
                Description = "This is a Test",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now
            };
            context.campaigns.Add(campaign);
            context.SaveChanges();

            // Act
            var savedCampaign = context.campaigns.FirstOrDefault(c => c.Id == campaign.Id);
            savedCampaign.Name = "Updated Test Campaign";
            savedCampaign.Description = "This is an updated test Campaign";
            context.SaveChanges();

            // Assert
            var updatedCampaign = context.campaigns.FirstOrDefault(c => c.Id == campaign.Id);
            Assert.NotNull(updatedCampaign);
            Assert.AreEqual(savedCampaign.Name, updatedCampaign.Name);
            Assert.AreEqual(savedCampaign.Description, updatedCampaign.Description);
            Assert.AreEqual(savedCampaign.StartDate, updatedCampaign.StartDate);
            Assert.AreEqual(savedCampaign.EndDate, updatedCampaign.EndDate);
        }

        [Test]
        public void DeleteCampaign_RemovesCampaignFromDatabase()
        {
            // Arrange
            using var context = new NitDbContext(_options);
            var campaign = new Campaign
            {
                Name = "Test Campaign",
                Description = "This is a Test",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now
            };
            context.campaigns.Add(campaign);
            context.SaveChanges();

            // Act
            context.campaigns.Remove(campaign);
            context.SaveChanges();

            // Assert
            var deletedCampaignD = context.campaigns.FirstOrDefault(c => c.Id == campaign.Id);
            Assert.Null(deletedCampaignD);
        }
    }
}
