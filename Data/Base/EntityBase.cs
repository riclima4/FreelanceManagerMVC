using System.ComponentModel.DataAnnotations;

namespace FreelanceManager.Data.Base
{
    public class EntityBase
    {
        public EntityBase()
        {
            IsActive = true;
            IsSystem = false;
            IsDeleted = false;
        }

        [Key]
        public Guid Id { get; set; }
        public virtual DateTime CreatedAt { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime? UpdatedAt { get; set; }
        public virtual string UpdatedBy { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual bool IsSystem { get; set; }
        public virtual bool IsDeleted { get; set; }
    }
}