namespace gop.Interfaces;

/// <summary>
/// For hashing Passwords/etc
/// </summary>
public interface IHashService
{
    /// <summary>
    /// Verifies if the hash of the text matches the provided hash.
    /// </summary>
    /// <param name="text">The text to verify.</param>
    /// <param name="hash">The previously encrypted password.</param>
    /// <returns>True if the passwords match; otherwise, false.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown when one or more arguments have null values.</exception>
    /// <exception cref="System.ArgumentException">Thrown when one or more arguments have empty or whitespace values.</exception>
    bool Compare(string text, string hash);

    /// <summary>
    /// Generates the hash of a password.
    /// </summary>
    /// <param name="text">The password to encrypt.</param>
    /// <returns>The encrypted password.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown when the value of <paramref name="text"/> is null.</exception>
    /// <exception cref="System.ArgumentException">Thrown when the value of <paramref name="text"/> is an empty or whitespace string.</exception>
    string Hash(string text);
}