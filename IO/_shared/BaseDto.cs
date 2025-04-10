
using FreelanceManager.Data.Entities.Base;

namespace FreelanceManager.IO._shared
{
    public class BaseDto<T> where T : EntityBase
    {
        public BaseDto() { }

        public BaseDto(T entity)
        {
            Id = entity.Id;
            CreatedAt = entity.CreatedAt;
            CreatedBy = entity.CreatedBy;
            RowVersion = entity.RowVersion;
            IsActive = entity.IsActive;
            IsSystem = entity.IsSystem;
            IsDeleted = entity.IsDeleted;
        }

        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public byte[] RowVersion { get; set; }
        public bool IsActive { get; set; }
        public bool IsSystem { get; set; }
        public bool IsDeleted { get; set; }
    }
}
