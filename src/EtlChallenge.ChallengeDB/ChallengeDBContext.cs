using EtlChallenge.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtlChallenge.ChallengeDB;

public class ChallengeDBContext(DbContextOptions<ChallengeDBContext> options)
 : DbContext(options)
{
    public DbSet<Policy> Policies => Set<Policy>();
    public DbSet<PolicyFile> PolicyFiles => Set<PolicyFile>();
    public DbSet<Risk> Risks => Set<Risk>();
    public DbSet<RiskFile> RiskFiles => Set<RiskFile>();
}
