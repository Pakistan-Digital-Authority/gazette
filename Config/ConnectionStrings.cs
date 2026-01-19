using gop.Options;

namespace gop.Config;

/// <summary>
/// Connection String for db
/// </summary>
public sealed class ConnectionStrings : IAppOptions
{
    /// <summary>
    /// Connection string
    /// </summary>
    public static string ConfigSectionPath => "ConnectionStrings";

    /// <summary>
    /// Connection string to the relational database.
    /// </summary>
    public string Database { get; private init; }

    /// <summary>
    /// (Optional) Definition of the Collation for the relational database.
    /// REF: https://learn.microsoft.com/en-us/ef/core/miscellaneous/collations-and-case-sensitivity
    /// </summary>
    public string Collation { get; private init; }

    /// <summary>
    /// Connection string to the Cache server.
    /// </summary>
    public string Cache { get; private init; }
}