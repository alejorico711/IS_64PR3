using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Servicios_64PR
{
    public class Encriptación
    {
        /*
        Cuando veamos encriptacion tenemos que preguntar si esto esta bien,
        Esto fue sacado de un proyecto desarrollado en desarrollo y arquitectura
        de software, junto a jeremias gomez (comision de los miercoles a la mañana)
        */
        private static Encriptación _instancia = null;
        public static Encriptación Instancia
        {
            get
            {
                if (_instancia == null)
                {
                    _instancia = new Encriptación();
                }
                return _instancia;
            }
        }

        // Config de Hash
        private const int SaltSize = 16;         // 16 bytes de Salt
        private const int HashSize = 32;         // 32 de Hash
        private const int Iterations = 10000;    // iteraciones para ralentizar el proceso de hash y darle seguridad, aguanta masivos ataques

        public byte[] Encriptar(string password)
        {
            byte[] salt;
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt = new byte[SaltSize]);
            }

            byte[] hash;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
            {
                hash = pbkdf2.GetBytes(HashSize);
            }

            byte[] finalHashBytes = new byte[SaltSize + HashSize];

            Array.Copy(salt, 0, finalHashBytes, 0, SaltSize);
            Array.Copy(hash, 0, finalHashBytes, SaltSize, HashSize);

            return finalHashBytes;
        }

        public bool VerifyPassword(string password, byte[] storedHashBytes)
        {

            if (storedHashBytes.Length != SaltSize + HashSize) // Verificar el tamaño 
                return false;


            byte[] salt = new byte[SaltSize];   // saco el Salt
            Array.Copy(storedHashBytes, 0, salt, 0, SaltSize);


            byte[] newHash;  //  creo un nuevo Hash con la contra y el Salt 
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
            {
                newHash = pbkdf2.GetBytes(HashSize);
            }


            byte[] storedPasswordHash = new byte[HashSize]; // traigo el Hash original guardado
            Array.Copy(storedHashBytes, SaltSize, storedPasswordHash, 0, HashSize);


            return SlowEquals(storedPasswordHash, newHash); // Comparo los dos hashes byte por byte 
        }
        private bool SlowEquals(byte[] a, byte[] b)
        {
            uint diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
                diff |= (uint)(a[i] ^ b[i]);
            return diff == 0;
        }
    }
}
