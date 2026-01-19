using System.ComponentModel.DataAnnotations.Schema;
using gop.Enums;
using gop.Interfaces;

namespace gop.Data.Entities;

/// <summary>
/// User Model
/// </summary>
public class User : BaseEntity, IAuditableEntity
{
    private readonly List<Token> _tokens = [];
    public string FullName { get; set; }
    public string Email { get; set; }
    public string? Phone { get; set; }
    public DateTime? LastAccessAt { get; set; }
    public string Ministry { get; set; }
    public string? HashPassword { get; set; }
    public UserRoleEnum Role { get; set; } = UserRoleEnum.Publisher;
    /// <summary>
    /// The lock period will expire at
    /// </summary>
    public DateTime? LockExpiresAt { get; set; }
    /// <summary>
    /// When the user attempts to log in with invalid credentials
    /// </summary>
    public int AccessFailuresCount { get; set; }

    public Guid? ParentId { get; set; }
    
    public ICollection<LoginHistory>? LoginHistories { get; set; }
    
    public IReadOnlyList<Token> Tokens => _tokens.AsReadOnly();

    public void AddToken(Token token) => _tokens.Add(token);

    public void SetLastAccess(DateTime lastAccessDate) => LastAccessAt = lastAccessDate;
    
    /// <summary>
    /// Indicates whether the user account is blocked.
    /// </summary>
    /// <returns>True if the account is blocked; otherwise, false.</returns>
    public bool IsLocked() => LockExpiresAt > DateTime.UtcNow;

    /// <summary>
    /// Indicates whether the user account is active or inactive.
    /// </summary>
    /// <returns>True if the account is active; otherwise, false.</returns>
    public bool IsActive() => Status == UserStatusEnum.Active;

    /// <summary>
    /// To reset the failed attempt count
    /// </summary>
    public void ResetFailedAttempt()
    {
        AccessFailuresCount = 0;
    }

    /// <summary>
    /// This will reset the counter of locked count and reset the lock expired at to null.
    /// When any success attempts made it will reset the status of locked to unlocked.
    /// </summary>
    public void ResetLockStatus()
    {
        this.AccessFailuresCount = 0;
        this.LockExpiresAt = null;
    }

    /// <summary>
    /// Increment the number of failed access attempts.
    /// When the access limit is reached, the account will be blocked for a period of time.
    /// </summary>
    /// <param name="numberAttempts">Maximum number of attempts until the account is blocked.</param>
    /// <param name="lockedTimeSpan">A certain period of time during which the account will remain blocked.</param>
    public void IncrementFailures(int numberAttempts, TimeSpan lockedTimeSpan)
    {
        if (IsLocked())
            return;
        AccessFailuresCount++;
        if (AccessFailuresCount != numberAttempts)
        {
            return;
        }

        AccessFailuresCount = 0;
        LockExpiresAt = DateTime.UtcNow.Add(lockedTimeSpan);
    }

    [Column(TypeName = "nvarchar(100)")]
    public UserStatusEnum Status { get; set; } = UserStatusEnum.Pending;

    public ICollection<Notification>? Notifications { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}