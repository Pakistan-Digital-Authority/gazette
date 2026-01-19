namespace gop.Interfaces;

/// <summary>
/// Auditable for update and created at timestamp
/// </summary>
public interface IAuditableEntity
{
    DateTime UpdatedAt { get; set; }
    DateTime CreatedAt { get; set; }
}