using HistoryService.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace HistoryService.Context;

/// <inheritdoc />
public class ApplicationContext : DbContext
{
    /// <summary>
    /// История выполнения правил
    /// </summary>
    public DbSet<RuleExecutedOrm> Executions { get; init; }

    /// <inheritdoc />
    public ApplicationContext(DbContextOptions options) : base(options)
    {
        Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<RuleExecutedOrm>()
            .ToCollection(nameof(RuleExecuted));
    }
}