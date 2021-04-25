using Goober.Core.Extensions;
using Goober.Config.DAL.Repositories;
using Goober.Config.WebApi.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;
using Goober.Config.WebApi.Enums;

namespace Goober.Config.WebApi.Tests.ConfigRowsApiControllerTests
{
    public class GetChildKeysAndSectionsTests
    {
        private const string GetChildKeysAndSectionsUrlPath = "/api/config/get-childs-keys-and-sections";
        
        [Fact]
        public async Task SectionHasChildsKeysByApplication_ShouldReturnsThem()
        {
            var sut = TestUtils.GenerateSystemUnderTest();

            //arrange
            var application = sut.CreateFixture<string>();
            var environment = sut.CreateFixture<string>();
            var sectionName = sut.CreateFixture<string>();

            var childs = sut.CreateManyFixture<string>();

            var configRowRepository = sut.ServiceProviderAfterMocks.GetRequiredService<IConfigRowRepository>();

            foreach (var iChild in childs)
            {
                await configRowRepository.InsertAsync(new DAL.Entities.ConfigRowResult
                {
                    Application = application,
                    Environment = environment,
                    ChangedUserName = sut.CreateFixture<string>(),
                    RowChangedDate = sut.CreateFixture<DateTime>(),
                    RowCreatedDate = sut.CreateFixture<DateTime>(),
                    ParentKey = sectionName,
                    Id = sut.CreateUniqueIntIdFixture(),
                    Key = iChild,
                    Value = sut.CreateFixture<string>()
                });
            }

            //act
            var res = await sut.ExecutePostAsync<GetPathChildsAndSectionsKeysResponse, GetPathChildsAndSectionsKeysRequest>(
                urlPath: GetChildKeysAndSectionsUrlPath, 
                new GetPathChildsAndSectionsKeysRequest { 
                    Application = application, 
                    Environment = environment, 
                    ParentKey = sectionName 
                });

            //assert
            Assert.NotNull(res);
            Assert.Equal(expected: GetConfigRowSelectConditionTypeEnum.ByApplication, actual: res.KeysSelectedConditionType);
            Assert.Equal(expected: childs.Serialize(), res.Keys.Serialize());
        }

        [Fact]
        public async Task SectionHasChildsKeysByApplication_GetWithWrongEnvironment_ShouldReturnEmpty()
        {
            var sut = TestUtils.GenerateSystemUnderTest();

            //arrange
            var application = sut.CreateFixture<string>();
            var environment = sut.CreateFixture<string>();
            var sectionName = sut.CreateFixture<string>();

            var childs = sut.CreateManyFixture<string>();

            var configRowRepository = sut.ServiceProviderAfterMocks.GetRequiredService<IConfigRowRepository>();

            foreach (var iChild in childs)
            {
                await configRowRepository.InsertAsync(new DAL.Entities.ConfigRowResult
                {
                    Application = application,
                    Environment = environment,
                    ChangedUserName = sut.CreateFixture<string>(),
                    RowChangedDate = sut.CreateFixture<DateTime>(),
                    RowCreatedDate = sut.CreateFixture<DateTime>(),
                    ParentKey = sectionName,
                    Id = sut.CreateUniqueIntIdFixture(),
                    Key = iChild,
                    Value = sut.CreateFixture<string>()
                });
            }

            //act
            var res = await sut.ExecutePostAsync<GetPathChildsAndSectionsKeysResponse, GetPathChildsAndSectionsKeysRequest>(
                urlPath: GetChildKeysAndSectionsUrlPath,
                new GetPathChildsAndSectionsKeysRequest
                {
                    Application = application,
                    Environment = environment + "anotherEnvironment",
                    ParentKey = sectionName
                });

            //assert
            Assert.NotNull(res);
            Assert.Empty(res.Keys);
        }

        [Fact]
        public async Task SectionHasChildsKeysWithoutApplication_ShouldReturnsThem_WithoutApplicationSelectedConditionType()
        {
            var sut = TestUtils.GenerateSystemUnderTest();

            //arrange
            var environment = sut.CreateFixture<string>();
            var sectionName = sut.CreateFixture<string>();

            var childs = sut.CreateManyFixture<string>();

            var configRowRepository = sut.ServiceProviderAfterMocks.GetRequiredService<IConfigRowRepository>();

            foreach (var iChild in childs)
            {
                await configRowRepository.InsertAsync(new DAL.Entities.ConfigRowResult
                {
                    Application = null,
                    Environment = environment,
                    ChangedUserName = sut.CreateFixture<string>(),
                    RowChangedDate = sut.CreateFixture<DateTime>(),
                    RowCreatedDate = sut.CreateFixture<DateTime>(),
                    ParentKey = sectionName,
                    Id = sut.CreateUniqueIntIdFixture(),
                    Key = iChild,
                    Value = sut.CreateFixture<string>()
                });
            }

            //act
            var res = await sut.ExecutePostAsync<GetPathChildsAndSectionsKeysResponse, GetPathChildsAndSectionsKeysRequest>(
                urlPath: GetChildKeysAndSectionsUrlPath,
                new GetPathChildsAndSectionsKeysRequest
                {
                    Application = sut.CreateFixture<string>(),
                    Environment = environment,
                    ParentKey = sectionName
                });

            //assert
            Assert.NotNull(res);
            Assert.Equal(expected: GetConfigRowSelectConditionTypeEnum.WithoutApplication, actual: res.KeysSelectedConditionType);
            Assert.Equal(expected: childs.Serialize(), res.Keys.Serialize());
        }
    }
}
