namespace gop.Records;

public sealed record AccessToken(string Token, DateTime CreatedAt, DateTime ExpiresAt);