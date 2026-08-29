using PharmacyContracts.SharedKernel.Common;


namespace PharmacyContracts.Modules.Companies.Domain.Entities
{
    public class CompanyDepartment : BaseEntity
    {
        public Guid CompanyId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
