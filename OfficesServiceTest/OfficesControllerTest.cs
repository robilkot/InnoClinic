using AutoMapper;
using Moq;
using OfficesService.Controllers;
using OfficesService.Data.Models;
using OfficesService.Models;
using OfficesService.Models.MapperProfiles;
using OfficesService.Repository;

namespace OfficesServiceTest
{
    public class OfficesControllerTest
    {
        [Fact]
        public async Task GetOffices_ReturnsAListOfOffices()
        {
            // Arrange
            var repoMock = new Mock<IRepository<DbOfficeModel>>();
            var testOffices = GetTestOffices();

            repoMock.Setup(repo => repo.GetAll()).Returns(Task.FromResult(testOffices));
            var config = new MapperConfiguration(opts =>
            {
                opts.AddProfile(new OfficesControllerProfile());
            });

            var controller = new OfficesController(repoMock.Object, config.CreateMapper());

            // Act
            var result = await controller.GetOffices();

            // Assert
            Assert.NotNull(result.Value);
            Assert.IsAssignableFrom<IEnumerable<ClientOfficeModel>>(result.Value);
            Assert.Equal(GetTestOffices().Count(), result.Value.Count());
        }

        [Fact]
        public async Task GetOffice_ValidId_ReturnsOffice()
        {
            // Arrange
            var repoMock = new Mock<IRepository<DbOfficeModel>>();
            var testOffices = GetTestOffices();

            repoMock.Setup(repo => repo.Get(It.IsAny<Guid>()))
                .Returns<Guid>((id) => Task.FromResult(testOffices.FirstOrDefault(o => o.Id == id)));
            var config = new MapperConfiguration(opts =>
            {
                opts.AddProfile(new OfficesControllerProfile());
            });

            var controller = new OfficesController(repoMock.Object, config.CreateMapper());

            var id = testOffices.ToList()[0].Id;

            // Act
            var result = await controller.GetOffice(id);

            // Assert
            Assert.Multiple(
                () => { Assert.NotNull(result.Value); },
                () => { Assert.IsType<ClientOfficeModel>(result.Value); },
                () => { Assert.Equal(id, result.Value?.Id); }
            );
        }

        [Fact]
        public async Task GetOffice_InvalidId_ReturnsNull()
        {
            // Arrange
            var repoMock = new Mock<IRepository<DbOfficeModel>>();
            var testOffices = GetTestOffices();

            repoMock.Setup(repo => repo.Get(It.IsAny<Guid>()))
                .Returns<Guid>((id) => Task.FromResult(testOffices.FirstOrDefault(o => o.Id == id)));
            var config = new MapperConfiguration(opts =>
            {
                opts.AddProfile(new OfficesControllerProfile());
            });

            var controller = new OfficesController(repoMock.Object, config.CreateMapper());

            var id = new Guid();

            // Act
            var result = await controller.GetOffice(id);

            // Assert
            Assert.Null(result.Value);
        }

        private static IEnumerable<DbOfficeModel> GetTestOffices()
        {
            Guid imgGuid = Guid.NewGuid();

            return new[]
            {
                new DbOfficeModel()
                {
                    Id = Guid.NewGuid(),
                    Adress = "Minsk",
                    Active = true,
                    Image = null,
                    ImageId = null,
                    RegistryPhoneNumber = "+123456789011"
                },
                new DbOfficeModel()
                {
                    Id = Guid.NewGuid(),
                    Adress = "Moscow",
                    Active = false,
                    Image = new DbImageModel()
                    {
                        Id = imgGuid,
                        Url = "example.com"
                    },
                    ImageId = imgGuid,
                    RegistryPhoneNumber = "+123456789012"
                },
                new DbOfficeModel()
                {
                    Id = Guid.NewGuid(),
                    Adress = "Sydney",
                    Active = true,
                    Image = new DbImageModel()
                    {
                        Id = imgGuid,
                        Url = "example.com"
                    },
                    ImageId = imgGuid,
                    RegistryPhoneNumber = "+123456789013"
                }
            };
        }
    }
}