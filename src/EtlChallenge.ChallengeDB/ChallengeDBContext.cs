using EtlChallenge.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtlChallenge.ChallengeDB;

public class ChallengeDBContext(DbContextOptions<ChallengeDBContext> options)
 : DbContext(options)
{
    public DbSet<StagedPolicy> StagedPolicies => Set<StagedPolicy>();
    public DbSet<Policy> Policies => Set<Policy>();
    public DbSet<StagedRisk> StagedRisks => Set<StagedRisk>();
    public DbSet<Risk> Risks => Set<Risk>();
}
