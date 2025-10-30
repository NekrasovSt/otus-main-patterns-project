using Moq;
using RuleEditor.Interface;
using RuleEditor.Models;
using RuleEditor.Services;
using RuleExecutor;
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
        var service = new RuleService(Mock.Of<IRuleRepository>());
        await Assert.ThrowsAsync<InvalidPropertyException>(() => service.GetAsync(id, CancellationToken.None));
    }

    [Theory]
    [InlineData("6900b8a5ce7ec3c503c5a3e1")]
    [InlineData("6900b8a5ce7ec3c503c5a3e3")]
    public async Task ValidateIdOk(string id)
    {
        var service = new RuleService(Mock.Of<IRuleRepository>());
        await service.GetAsync(id, CancellationToken.None);
    }

    [Theory]
    [InlineData("6900b8a5ce7ec3c503c5a3e1")]
    [InlineData("http://")]
    [InlineData("ya.ru")]
    public async Task ValidateLinkNotValid(string link)
    {
        var service = new RuleService(Mock.Of<IRuleRepository>());
        await Assert.ThrowsAsync<InvalidPropertyException>(() =>
            service.AddAsync(
                new Rule()
                {
                    Name = "some name", Link = link, FilterCondition = new FilterCondition() { Field = "language", Operator = "="}
                }, CancellationToken.None));
    }

    [Theory]
    [InlineData("http://ya.ru/100?search='name'")]
    [InlineData("http://ya.ru/100")]
    [InlineData("http://ya.ru")]
    public async Task ValidateLinkOk(string link)
    {
        var service = new RuleService(Mock.Of<IRuleRepository>());
        await service.AddAsync(
            new Rule()
            {
                Name = "some name", Link = link, FilterCondition = new FilterCondition() { Field = "language", Operator = "=" }
            }, CancellationToken.None);
    }

    [Fact]
    public async Task ValidateConditionsEmptyFieldNotValid()
    {
        var service = new RuleService(Mock.Of<IRuleRepository>());
        await Assert.ThrowsAsync<InvalidPropertyException>(() =>
            service.AddAsync(
                new Rule()
                {
                    Name = "some name", Link = "http://ya.ru", FilterCondition = new FilterCondition() { Field = "", Operator = "=" }
                }, CancellationToken.None));
    }

    [Fact]
    public async Task ValidateConditionsEmptyFieldSecondLevelNotValid()
    {
        var service = new RuleService(Mock.Of<IRuleRepository>());
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
        var service = new RuleService(Mock.Of<IRuleRepository>());
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
        var service = new RuleService(Mock.Of<IRuleRepository>());
        await Assert.ThrowsAsync<InvalidPropertyException>(() =>
            service.AddAsync(
                new Rule()
                {
                    Name = "some name", Link = "http://ya.ru",
                    FilterCondition = new FilterCondition() { Field = "language", Operator = "->", Conditions = [] }
                }, CancellationToken.None));
    }
}