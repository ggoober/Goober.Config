using AutoFixture;
using Goober.Config.DAL.Entities;
using Goober.Config.DAL.Repositories;
using Goober.Config.WebApi.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace Goober.Config.WebApi.Tests.ConfigRowsApiControllerTests
{
    public class GetConfigRowTests
    {
        private const string GetConfigRowUrlPath = "/api/config/get-row";

        [Fact]
        public async Task ConfigRowWithParentKeyByApplicationExists_ShouldReturnIt_ByApplication()
        {
            var sut = TestUtils.GenerateSystemUnderTest();

            //arrange
            var expectedConfigRow = sut.CreateFixture<ConfigRowResult>();

            var configRowRepository = sut.ServiceProviderAfterMocks.GetRequiredService<IConfigRowRepository>();
            await configRowRepository.InsertAsync(expectedConfigRow);

            //act
            var res = await sut.ExecutePostAsync<GetConfigRowResponseModel, GetConfigRowRequestModel>(urlPath: GetConfigRowUrlPath,
                new GetConfigRowRequestModel
                {
                    Application = expectedConfigRow.Application,
                    Environment = expectedConfigRow.Environment,
                    Key = expectedConfigRow.Key,
                    ParentKey = expectedConfigRow.ParentKey
                });

            //assert
            Assert.NotNull(res);
            Assert.Equal(expected: res.Value, actual: expectedConfigRow.Value);
            Assert.Equal(expected: Enums.GetConfigRowSelectConditionTypeEnum.ByApplication, actual: res.SelectedConditionType);
        }

        [Fact]
        public async Task ConfigRowWithoutParentKeyByApplicationExists_ShouldReturnIt_ByApplication()
        {
            var sut = TestUtils.GenerateSystemUnderTest();

            //arrange
            var expectedConfigRow = sut.BuildFixture<ConfigRowResult>()
                .Without(x => x.ParentKey)
                .Create();

            var configRowRepository = sut.ServiceProviderAfterMocks.GetRequiredService<IConfigRowRepository>();
            await configRowRepository.InsertAsync(expectedConfigRow);

            //act
            var res = await sut.ExecutePostAsync<GetConfigRowResponseModel, GetConfigRowRequestModel>(urlPath: GetConfigRowUrlPath,
                new GetConfigRowRequestModel
                {
                    Application = expectedConfigRow.Application,
                    Environment = expectedConfigRow.Environment,
                    Key = expectedConfigRow.Key,
                    ParentKey = expectedConfigRow.ParentKey
                });

            //assert
            Assert.NotNull(res);
            Assert.Equal(expected: res.Value, actual: expectedConfigRow.Value);
            Assert.Equal(expected: Enums.GetConfigRowSelectConditionTypeEnum.ByApplication, actual: res.SelectedConditionType);
        }

        [Fact]
        public async Task ConfigRowWithoutApplication_ShouldReturnItWithResultType_WithoutApplication()
        {
            var sut = TestUtils.GenerateSystemUnderTest();

            //arrange
            var expectedConfigRow = sut.CreateFixture<ConfigRowResult>();

            var configRowRepository = sut.ServiceProviderAfterMocks.GetRequiredService<IConfigRowRepository>();
            await configRowRepository.InsertAsync(new ConfigRowResult
            {
                Application = null,
                ChangedUserName = expectedConfigRow.ChangedUserName,
                Environment = expectedConfigRow.Environment,
                Id = expectedConfigRow.Id,
                ParentKey = expectedConfigRow.ParentKey,
                Key = expectedConfigRow.Key,
                RowChangedDate = expectedConfigRow.RowChangedDate,
                RowCreatedDate = expectedConfigRow.RowCreatedDate,
                Value = expectedConfigRow.Value
            });

            //act
            var res = await sut.ExecutePostAsync<GetConfigRowResponseModel, GetConfigRowRequestModel>(urlPath: GetConfigRowUrlPath,
                new GetConfigRowRequestModel
                {
                    Application = expectedConfigRow.Application,
                    Environment = expectedConfigRow.Environment,
                    Key = expectedConfigRow.Key,
                    ParentKey = expectedConfigRow.ParentKey
                });

            //assert
            Assert.NotNull(res);
            Assert.Equal(expected: res.Value, actual: expectedConfigRow.Value);
            Assert.Equal(expected: Enums.GetConfigRowSelectConditionTypeEnum.WithoutApplication, actual: res.SelectedConditionType);
        }

        [Fact]
        public async Task ConfigRowWithDifferentApplication_ShouldReturnItWithResultType_IgnoreApplication()
        {
            var sut = TestUtils.GenerateSystemUnderTest();

            //arrange
            var expectedConfigRow = sut.CreateFixture<ConfigRowResult>();

            var configRowRepository = sut.ServiceProviderAfterMocks.GetRequiredService<IConfigRowRepository>();
            await configRowRepository.InsertAsync(new ConfigRowResult
            {
                Application = expectedConfigRow.Application + "Different",
                ChangedUserName = expectedConfigRow.ChangedUserName,
                Environment = expectedConfigRow.Environment,
                Id = expectedConfigRow.Id,
                ParentKey = expectedConfigRow.ParentKey,
                Key = expectedConfigRow.Key,
                RowChangedDate = expectedConfigRow.RowChangedDate,
                RowCreatedDate = expectedConfigRow.RowCreatedDate,
                Value = expectedConfigRow.Value
            });;

            //act
            var res = await sut.ExecutePostAsync<GetConfigRowResponseModel, GetConfigRowRequestModel>(urlPath: GetConfigRowUrlPath,
                new GetConfigRowRequestModel
                {
                    Application = expectedConfigRow.Application,
                    Environment = expectedConfigRow.Environment,
                    Key = expectedConfigRow.Key,
                    ParentKey = expectedConfigRow.ParentKey
                });

            //assert
            Assert.NotNull(res);
            Assert.Equal(expected: res.Value, actual: expectedConfigRow.Value);
            Assert.Equal(expected: Enums.GetConfigRowSelectConditionTypeEnum.IgnoreApplication, actual: res.SelectedConditionType);
        }
    }
}
