using System;
using System.Security.Cryptography;
using InventarioComputo.Application.Contracts;

namespace InventarioComputo.Infrastructure.Security
{
    // Formatos admitidos:
    // 1) Nuevo:   {iter}.{saltBase64}.{hashBase64}
    // 2) Legado:  {hashBase64}:{saltBase64}:{iter}:{algorithm}
    public sealed class PasswordHasher : IPasswordHasher
    {
        private const int DefaultIterations = 100_000;
        private const int SaltSize = 16;   // 128-bit
        private const int KeySize  = 32;   // 256-bit

        public string HashPassword(string password)
        {
            if (password is null) throw new ArgumentNullException(nameof(password));

            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[SaltSize];
            rng.GetBytes(salt);

            var hash = Pbkdf2(password, salt, DefaultIterations, KeySize, HashAlgorithmName.SHA256);
            return $"{DefaultIterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public bool VerifyPassword(string password, string hashString)
        {
            if (password is null) throw new ArgumentNullException(nameof(password));
            if (string.IsNullOrWhiteSpace(hashString)) return false;

            // Intento con formato nuevo (iter.salt.hash)
            var dotParts = hashString.Split('.', 3);
            if (dotParts.Length == 3 &&
                int.TryParse(dotParts[0], out var iterDot))
            {
                try
                {
                    var salt = Convert.FromBase64String(dotParts[1]);
                    var expectedHash = Convert.FromBase64String(dotParts[2]);
                    var actualHash = Pbkdf2(password, salt, iterDot, expectedHash.Length, HashAlgorithmName.SHA256);
                    return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
                }
                catch
                {
                    // Si falla el parseo base64, intentamos con el formato legado
                }
            }

            // Intento con formato legado (hash:salt:iter:alg)
            var colonParts = hashString.Split(':');
            if (colonParts.Length == 4 &&
                int.TryParse(colonParts[2], out var iterColon))
            {
                try
                {
                    var storedHash = Convert.FromBase64String(colonParts[0]);
                    var salt = Convert.FromBase64String(colonParts[1]);
                    var algName = new HashAlgorithmName(colonParts[3]);
                    var actualHash = Pbkdf2(password, salt, iterColon, storedHash.Length, algName);
                    return CryptographicOperations.FixedTimeEquals(storedHash, actualHash);
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        private static byte[] Pbkdf2(string password, byte[] salt, int iterations, int length, HashAlgorithmName alg)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, alg);
            return pbkdf2.GetBytes(length);
        }
    }
}