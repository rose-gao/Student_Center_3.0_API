using System;
using System.Security.Cryptography;

namespace Student_Center_3._0_Database.Models
{
    public class temp
    {
        public static void GenerateKeyAndIV()
        {
            using (Aes aes = Aes.Create())
            {
                aes.GenerateKey();
                aes.GenerateIV();

                Console.WriteLine("Key: " + Convert.ToBase64String(aes.Key));
                Console.ReadLine();

                Console.WriteLine("IV: " + Convert.ToBase64String(aes.IV));
                Console.ReadLine();

            }
        }
    }

}
