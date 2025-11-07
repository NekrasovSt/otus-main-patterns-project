using MassTransit;
using Moq;
using RuleEditor.Interface;
using RuleEditor.Models;
using RuleEditor.Services;
using RuleExecutor;
using ServiceUtils.Broker;
using ServiceUtils.Exceptions;

namespace RuleEditor.Tests;

public class RuleServiceTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("6900b8a5ce7ec3c503c5a3")]
    [InlineData("6900b8a5ce7ec3c503c5a3t1")]
    public async Task ValidateIdТNotValid(string id)
    {
        var service = new RuleService(Mock.Of<IRuleRepository>(), Mock.Of<IPublishEndpoint>());
        await Assert.ThrowsAsync<InvalidPropertyException>(() => service.GetAsync(id, CancellationToken.None));
    }

    [Theory]
    [InlineData("6900b8a5ce7ec3c503c5a3e1")]
    [InlineData("6900b8a5ce7ec3c503c5a3e3")]
    public async Task ValidateIdOk(string id)
    {
        var service = new RuleService(Mock.Of<IRuleRepository>(), Mock.Of<IPublishEndpoint>());
        await service.GetAsync(id, CancellationToken.None);
    }

    [Theory]
    [InlineData("6900b8a5ce7ec3c503c5a3e1")]
    [InlineData("http://")]
    [InlineData("ya.ru")]
    public async Task ValidateLinkNotValid(string link)
    {
        var endpoint = new Mock<IPublishEndpoint>();
        var service = new RuleService(Mock.Of<IRuleRepository>(), endpoint.Object);
        await Assert.ThrowsAsync<InvalidPropertyException>(() =>
            service.AddAsync(
                new Rule()
                {
                    Name = "some name", Link = link,
                    FilterCondition = new FilterCondition() { Field = "language", Operator = "=" }
                }, CancellationToken.None));
        endpoint.Verify(i => i.Publish(It.IsAny<RuleChangedDto>(), It.IsAny<CancellationToken>()), Times.Never());
    }

    [Theory]
    [InlineData("http://ya.ru/100?search='name'")]
    [InlineData("http://ya.ru/100")]
    [InlineData("http://ya.ru")]
    public async Task ValidateLinkOk(string link)
    {
        var endpoint = new Mock<IPublishEndpoint>();
        var service = new RuleService(Mock.Of<IRuleRepository>(), endpoint.Object);
        await service.AddAsync(
            new Rule()
            {
                Name = "some name", Link = link,
                FilterCondition = new FilterCondition() { Field = "language", Operator = "=" }
            }, CancellationToken.None);
        endpoint.Verify(i => i.Publish(It.IsAny<RuleChangedDto>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task DeleteOk()
    {
        var endpoint = new Mock<IPublishEndpoint>();
        var service = new RuleService(Mock.Of<IRuleRepository>(), endpoint.Object);
        await service.DeleteAsync("6900b8a5ce7ec3c503c5a3e3", CancellationToken.None);
        endpoint.Verify(i => i.Publish(It.IsAny<RuleChangedDto>(), It.IsAny<CancellationToken>()), Times.Once());
    }
    
    [Fact]
    public async Task UpdateOk()
    {
        var endpoint = new Mock<IPublishEndpoint>();
        var service = new RuleService(Mock.Of<IRuleRepository>(), endpoint.Object);
        await service.UpdateAsync(
            new Rule()
            {
                Id = "6900b8a5ce7ec3c503c5a3e3",
                Name = "some name", 
                Link = "http://ya.ru/100",
                FilterCondition = new FilterCondition() { Field = "language", Operator = "=" }
            }, CancellationToken.None);
        endpoint.Verify(i => i.Publish(It.IsAny<RuleChangedDto>(), It.IsAny<CancellationToken>()), Times.Once());
    }
    
    
    [Fact]
    public async Task ValidateConditionsEmptyFieldNotValid()
    {
        var endpoint = new Mock<IPublishEndpoint>();
        var service = new RuleService(Mock.Of<IRuleRepository>(), endpoint.Object);
        await Assert.ThrowsAsync<InvalidPropertyException>(() =>
            service.AddAsync(
                new Rule()
                {
                    Name = "some name", Link = "http://ya.ru",
                    FilterCondition = new FilterCondition() { Field = "", Operator = "=" }
                }, CancellationToken.None));
        endpoint.Verify(i => i.Publish(It.IsAny<RuleChangedDto>(), It.IsAny<CancellationToken>()), Times.Never());
    }

    [Fact]
    public async Task ValidateConditionsEmptyFieldSecondLevelNotValid()
    {
        var endpoint = new Mock<IPublishEndpoint>();
        var service = new RuleService(Mock.Of<IRuleRepository>(), endpoint.Object);
        await Assert.ThrowsAsync<InvalidPropertyException>(() =>
            service.AddAsync(
                new Rule()
                {
                    Name = "some name", Link = "http://ya.ru",
                    FilterCondition = new FilterCondition() { Field = "", Conditions = [new FilterCondition()] }
                }, CancellationToken.None));
    }

    [Fact]
    public async Task ValidateConditionsEmptyOperatorNotValid()
    {
        var endpoint = new Mock<IPublishEndpoint>();
        var service = new RuleService(Mock.Of<IRuleRepository>(), endpoint.Object);
        await Assert.ThrowsAsync<InvalidPropertyException>(() =>
            service.AddAsync(
                new Rule()
                {
                    Name = "some name", Link = "http://ya.ru",
                    FilterCondition = new FilterCondition() { Field = "language", Conditions = [] }
                }, CancellationToken.None));
    }

    [Fact]
    public async Task ValidateConditionsWrongOperatorNotValid()
    {
        var endpoint = new Mock<IPublishEndpoint>();
        var service = new RuleService(Mock.Of<IRuleRepository>(), endpoint.Object);
        await Assert.ThrowsAsync<InvalidPropertyException>(() =>
            service.AddAsync(
                new Rule()
                {
                    Name = "some name", Link = "http://ya.ru",
                    FilterCondition = new FilterCondition() { Field = "language", Operator = "->", Conditions = [] }
                }, CancellationToken.None));
    }
}