using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;

namespace com.vivo.codelibrary
{
    public class EncryptHelper
    {
        #region//DES加密解谜

        //DES加密字符串
        //<param name = "encryptString" > 待加密的字符串 </ param >
        //< param name="encryptKey">加密密钥,要求为8位</param>
        //<returns>加密成功返回加密后的字符串，失败返回源串</returns>
        public static string EncryptDES(string encryptString, string encryptKey)
        {
            try
            {
                DESKey desKey = FindKey(string.Intern(encryptKey));
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                //
                byte[] inputByteArrayCopy = new byte[inputByteArray.Length + 4];
                System.Array.Copy(inputByteArray, 0, inputByteArrayCopy, 4, inputByteArray.Length);
                inputByteArrayCopy[0] = 2;
                inputByteArrayCopy[1] = 0;
                inputByteArrayCopy[2] = 2;
                inputByteArrayCopy[3] = 0;
                inputByteArray = inputByteArrayCopy;
                //
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(desKey.RgbKey, desKey.Keys), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                cStream.Close();
                inputByteArray = null;
                inputByteArrayCopy = null;
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch (System.Exception e)
            {
                VLog.Exception(e);
                return null;
            }
        }

        //DES解密字符串
        //<param name = "decryptString" > 待解密的字符串 </ param >
        //< param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
        //<returns>解密成功返回解密后的字符串，失败返源串</returns>
        public static string DecryptDES(string decryptString, string decryptKey)
        {
            try
            {
                DESKey desKey = FindKey(string.Intern(decryptKey));
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(desKey.RgbKey, desKey.Keys), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                cStream.Close();
                inputByteArray = mStream.ToArray();
                //
                if (inputByteArray[0] != 2 || inputByteArray[1] != 0 || inputByteArray[2] != 2 || inputByteArray[3] != 0)
                {
                    inputByteArray = null;
                    return null;
                }
                byte[] inputByteArrayCopy = new byte[inputByteArray.Length - 4];
                System.Array.Copy(inputByteArray, 4, inputByteArrayCopy, 0, inputByteArrayCopy.Length);
                inputByteArray = inputByteArrayCopy;
                //
                inputByteArrayCopy = null;
                return Encoding.UTF8.GetString(inputByteArray);
            }
            catch (System.Exception e)
            {
                VLog.Exception(e);
                return null;
            }
        }

        //DES加密字符串
        //<param name = "encryptString" > 待加密的字符串 </ param >
        //< param name="encryptKey">加密密钥,要求为8位</param>
        //<returns>加密成功返回加密后的字符串，失败返回源串</returns>
        public static byte[] EncryptDES_B(byte[] inputByteArray, string encryptKey)
        {
            try
            {
                DESKey desKey = FindKey(string.Intern(encryptKey));
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(desKey.RgbKey, desKey.Keys), CryptoStreamMode.Write);
                //
                byte[] inputByteArrayCopy = new byte[inputByteArray.Length + 4];
                System.Array.Copy(inputByteArray, 0, inputByteArrayCopy, 4, inputByteArray.Length);
                inputByteArrayCopy[0] = 2;
                inputByteArrayCopy[1] = 0;
                inputByteArrayCopy[2] = 2;
                inputByteArrayCopy[3] = 0;
                inputByteArray = inputByteArrayCopy;
                //
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                cStream.Close();
                inputByteArray = null;
                inputByteArrayCopy = null;
                return mStream.ToArray();
            }
            catch (System.Exception e)
            {
                VLog.Exception(e);
                return null;
            }
        }

        //DES解密字符串
        //<param name = "decryptString" > 待解密的字符串 </ param >
        //< param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
        //<returns>解密成功返回解密后的字符串，失败返源串</returns>
        public static byte[] DecryptDES_B(byte[] inputByteArray, string decryptKey)
        {
            try
            {
                DESKey desKey = FindKey(string.Intern(decryptKey));
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(desKey.RgbKey, desKey.Keys), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                cStream.Close();
                inputByteArray = mStream.ToArray();
                //
                if (inputByteArray[0] != 2 || inputByteArray[1] != 0 || inputByteArray[2] != 2 || inputByteArray[3] != 0)
                {
                    inputByteArray = null;
                    return null;
                }
                byte[] inputByteArrayCopy = new byte[inputByteArray.Length - 4];
                System.Array.Copy(inputByteArray, 4, inputByteArrayCopy, 0, inputByteArrayCopy.Length);
                inputByteArray = inputByteArrayCopy;
                inputByteArrayCopy = null;
                //
                return inputByteArray;
            }
            catch (System.Exception e)
            {
                VLog.Exception(e);
                return null;
            }
        }


        //DES加密字符串
        //<param name = "encryptString" > 待加密的字符串 </ param >
        //< param name="encryptKey">加密密钥,要求为8位</param>
        //<returns>加密成功返回加密后的字符串，失败返回源串</returns>
        public static string EncryptDES_B_64str(byte[] inputByteArray, string encryptKey)
        {
            try
            {
                DESKey desKey = FindKey(string.Intern(encryptKey));
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(desKey.RgbKey, desKey.Keys), CryptoStreamMode.Write);
                //
                byte[] inputByteArrayCopy = new byte[inputByteArray.Length + 4];
                System.Array.Copy(inputByteArray, 0, inputByteArrayCopy, 4, inputByteArray.Length);
                inputByteArrayCopy[0] = 2;
                inputByteArrayCopy[1] = 0;
                inputByteArrayCopy[2] = 2;
                inputByteArrayCopy[3] = 0;
                inputByteArray = inputByteArrayCopy;
                //
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                cStream.Close();
                inputByteArray = null;
                inputByteArrayCopy = null;
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch (System.Exception e)
            {
                VLog.Exception(e);
                return null;
            }
        }

        //DES解密字符串
        //<param name = "decryptString" > 待解密的字符串 </ param >
        //< param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
        //<returns>解密成功返回解密后的字符串，失败返源串</returns>
        public static byte[] DecryptDES_B_64str(string decryptString, string decryptKey)
        {
            try
            {
                DESKey desKey = FindKey(string.Intern(decryptKey));
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(desKey.RgbKey, desKey.Keys), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                cStream.Close();
                inputByteArray = mStream.ToArray();
                //
                if (inputByteArray[0] != 2 || inputByteArray[1] != 0 || inputByteArray[2] != 2 || inputByteArray[3] != 0)
                {
                    inputByteArray = null;
                    return null;
                }
                byte[] inputByteArrayCopy = new byte[inputByteArray.Length - 4];
                System.Array.Copy(inputByteArray, 4, inputByteArrayCopy, 0, inputByteArrayCopy.Length);
                inputByteArray = inputByteArrayCopy;
                inputByteArrayCopy = null;
                //
                return inputByteArray;
            }
            catch (System.Exception e)
            {
                VLog.Exception(e);
                return null;
            }
        }

        static Dictionary<string, DESKey> desKeys = new Dictionary<string, DESKey>();

        static DESKey FindKey(string key)
        {
            DESKey res = null;
            if (!desKeys.TryGetValue(String.Intern(key) , out res))
            {
                res = new DESKey();
                res.Key = key;
                desKeys.Add(key, res);
            }
            return res;
        }

        #endregion

        #region//MD5加密

        /// <summary>
        /// MD5不可逆加密  一般用于账号密码验证
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static string Encryption_MD5(string inputString)
        {
            MD5 md5Hash = MD5.Create();
            string hashString = GetMD5Hash(md5Hash, inputString);
            return hashString.ToUpper();
        }

        static string GetMD5Hash(MD5 md5Hash, string input)
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder builder = new StringBuilder();
            for (int cnt = 0; cnt < data.Length; ++cnt)
            {
                builder.Append(data[cnt].ToString("x2"));
            }
            return builder.ToString();
        }

        #endregion

        /// <summary>
        /// DES密钥
        /// </summary>
        public class DESKey
        {
            public string Key = "!%o-_d@5";//本地数据加密秘钥(只能是8位秘钥)

            public byte[] Keys = { 0x87, 0xDC, 0xAD, 0xDB, 0xAC, 0xBD, 0x34, 0x70 };

            byte[] rgbKey;

            public byte[] RgbKey
            {
                get
                {
                    if (rgbKey == null)
                    {
                        rgbKey = Encoding.UTF8.GetBytes(Key.Substring(0, 8));
                    }
                    return rgbKey;
                }
            }
        }
    }

}


