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

        //reversible
        private const int AesKeySize = 32;   // 256 bits → clave
        private const int AesIvSize = 16;   // 128 bits → IV (tamaño de bloque fijo en AES)
        private static readonly byte[] MasterKey = Encoding.UTF8.GetBytes("Proyecto64PR_ClaveSecreta2026!!!"); // exactamente 32 chars
        private byte[] EncriptarAES(string texto)
        {
            byte[] iv = new byte[AesIvSize];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(iv); // IV aleatorio distinto cada vez → mismo texto da resultados diferentes
            }

            byte[] textoBytes = Encoding.UTF8.GetBytes(texto);
            byte[] cifrado;

            using (var aes = Aes.Create())
            {
                aes.Key = MasterKey;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;   // CBC es el modo estándar y seguro con IV aleatorio
                aes.Padding = PaddingMode.PKCS7;

                using (var encryptor = aes.CreateEncryptor())
                {
                    cifrado = encryptor.TransformFinalBlock(textoBytes, 0, textoBytes.Length);
                }
            }

            // Concatenamos IV + cifrado en un solo array para guardar/transmitir
            byte[] resultado = new byte[AesIvSize + cifrado.Length];
            Array.Copy(iv, 0, resultado, 0, AesIvSize);
            Array.Copy(cifrado, 0, resultado, AesIvSize, cifrado.Length);

            return resultado;
        }

        private string DesencriptarAES(byte[] datos)
        {
            if (datos.Length < AesIvSize)
                throw new ArgumentException("Los datos son demasiado cortos para contener un IV válido.");

            // Separamos IV y datos cifrados
            byte[] iv = new byte[AesIvSize];
            byte[] cifrado = new byte[datos.Length - AesIvSize];

            Array.Copy(datos, 0, iv, 0, AesIvSize);
            Array.Copy(datos, AesIvSize, cifrado, 0, cifrado.Length);

            using (var aes = Aes.Create())
            {
                aes.Key = MasterKey;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var decryptor = aes.CreateDecryptor())
                {
                    byte[] textoBytes = decryptor.TransformFinalBlock(cifrado, 0, cifrado.Length);
                    return Encoding.UTF8.GetString(textoBytes);
                }
            }
        }
        //estos son 2 metodos helper, son a los que realmente tenemos que llamar, esto se hace asi para no tener que cambiar el tipo
        //de dato en la base de datos de nvarchar a varbinary
        public string EncriptarAESBase64(string texto)
        {
            byte[] cifrado = EncriptarAES(texto);
            return Convert.ToBase64String(cifrado); // byte[] → string guardable en nvarchar
        }

        public string DesencriptarAESBase64(string base64)
        {
            byte[] cifrado = Convert.FromBase64String(base64); // string → byte[]
            return DesencriptarAES(cifrado);
        }
    }
}
