namespace Octo.Core.Domain;

public interface IMultiTenantEntity
{
    Guid TenantId { get; }
}