using Microsoft.EntityFrameworkCore;
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
    public class UserTest
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
        public void CreateUser_AddsNewUserToDatabase()
        {
            // Arrange
            using var context = new NitDbContext(_options);
            var user = new User
            {
                UserName = "Test username",
                Password = "123456",
                Role = 5,
                Status = 1,
                Email = "test@gmail.com"
            };

            // Act
            context.users.Add(user);
            context.SaveChanges();

            // Assert
            var savedUser = context.users.FirstOrDefault(c => c.Id == user.Id);
            Assert.NotNull(savedUser);
            Assert.AreEqual(user.UserName, savedUser.UserName);
            Assert.AreEqual(user.Password, savedUser.Password);
            Assert.AreEqual(user.Role, savedUser.Role);
            Assert.AreEqual(user.Status, savedUser.Status );
            Assert.AreEqual(user.Email, savedUser.Email);
        }

        [Test]
        public void ReadUser_GetsUserFromDatabase()
        {
            // Arrange
            using var context = new NitDbContext(_options);
            var user = new User
            {
                UserName = "Test username",
                Password = "123456",
                Role = 5,
                Status = 1,
                Email = "test@gmail.com"
            };
            context.users.Add(user);
            context.SaveChanges();

            // Act
            var savedUser = context.users.FirstOrDefault(c => c.Id == user.Id);

            // Assert
            Assert.NotNull(savedUser);
            Assert.AreEqual(user.UserName, savedUser.UserName);
            Assert.AreEqual(user.Password, savedUser.Password);
            Assert.AreEqual(user.Role, savedUser.Role);
            Assert.AreEqual(user.Status, savedUser.Status);
            Assert.AreEqual(user.Email, savedUser.Email);
        }

        [Test]
        public void UpdateUser_UpdatesUserInDatabase()
        {
            // Arrange
            using var context = new NitDbContext(_options);
            var user = new User
            {
                UserName = "Test username",
                Password = "123456",
                Role = 5,
                Status = 1,
                Email = "test@gmail.com"
            };
            context.users.Add(user);
            context.SaveChanges();

            // Act
            var savedUser = context.users.FirstOrDefault(c => c.Id == user.Id);
            savedUser.UserName = "Test update username";
            savedUser.Password = "123456";
            savedUser.Role = 6;
            savedUser.Status = 1;
            savedUser.Email = "testupdate@gmail.com";
            context.SaveChanges();

            // Assert
            var updatedCategory = context.users.FirstOrDefault(c => c.Id == user.Id);
            Assert.NotNull(updatedCategory);
            Assert.AreEqual(savedUser.UserName, updatedCategory.UserName);
            Assert.AreEqual(savedUser.Password, updatedCategory.Password);
            Assert.AreEqual(savedUser.Role, updatedCategory.Role);
            Assert.AreEqual(savedUser.Status, updatedCategory.Status);
            Assert.AreEqual(savedUser.Email, updatedCategory.Email);
        }

        [Test]
        public void DeleteUser_RemovesUserFromDatabase()
        {
            // Arrange
            using var context = new NitDbContext(_options);
            var user = new User
            {
                UserName = "Test username",
                Password = "123456",
                Role = 5,
                Status = 1,
                Email = "test@gmail.com"
            };
            context.users.Add(user);
            context.SaveChanges();

            // Act
            context.users.Remove(user);
            context.SaveChanges();

            // Assert
            var deletedCategory = context.users.FirstOrDefault(c => c.Id == user.Id);
            Assert.Null(deletedCategory);
        }
    }
}
