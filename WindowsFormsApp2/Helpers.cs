using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

using Cryptography;

namespace Encryption.Helpers
{
    public class Helpers
    {
        private const int HASH_LENGTH = 32;

        private readonly byte[] salt = {
            0xEF, 0x03, 0x33, 0xC4, 0xEB, 0x4A, 0x06, 0x51,
            0x01, 0x17, 0xF8, 0x2E, 0xB4, 0x28, 0x60, 0x33,
            0x06, 0x1E, 0xBC, 0xF2, 0x38, 0x36, 0x62, 0x27,
            0x24, 0x65, 0x72, 0x06, 0xFE, 0xAD, 0x9C, 0xB6,
        };

        private const int PBKDF_ROUNDS = 40000;

        private const int KEY_SIZE = 56;

        private byte[] HashPassword(SecureString password, int size)
        {
            IntPtr bstr = IntPtr.Zero;
            byte[] workArray = null;
            try
            {
                bstr = Marshal.SecureStringToBSTR(password);
                unsafe
                {
                    byte* bstrBytes = (byte*)bstr;
                    workArray = new byte[password.Length * 2];

                    for (int i = 0; i < workArray.Length; i++)
                        workArray[i] = *bstrBytes++;
                }

                using (var rfc2898 = new Rfc2898DeriveBytes(workArray, salt, PBKDF_ROUNDS))
                {
                    return rfc2898.GetBytes(size);
                }
            }
            finally
            {
                if (workArray != null)
                    for (int i = 0; i < workArray.Length; i++)
                        workArray[i] = 0;
                if (bstr != IntPtr.Zero)
                    Marshal.ZeroFreeBSTR(bstr);
            }
        }

        private byte[] MakeMultipleLength(byte[] data, int div)
        {
            int add = (div - (data.Length % div)) % div;

            var rdata = data.AsEnumerable();

            for (int i = 0; i < add; i++)
                rdata = rdata.Concat(new byte[] { 0 });

            return rdata.ToArray();
        }

        private byte[] TrimRightNull(byte[] data)
        {
            int len = data.Length;

            while (len > 0 && data[len - 1] == 0) len--;

            return data.Take(len).ToArray();
        }

        private byte[] EncodeBytes(byte[] data, SecureString password)
        {
            var key = HashPassword(password, KEY_SIZE);

            var scheduledKey = AcedCast5.ScheduleKey(key);
            var iv = AcedCast5.GetOrdinaryIV(scheduledKey);

            byte[] result = (byte[])MakeMultipleLength(data, 8).Clone();

            AcedCast5.EncryptCBC(scheduledKey, result, 0, result.Length, iv);
            AcedCast5.ClearKey(scheduledKey);

            return result;
        }

        private byte[] DecodeBytes(byte[] data, SecureString password)
        {
            var key = HashPassword(password, KEY_SIZE);

            int[] scheduledKey = AcedCast5.ScheduleKey(key);
            long iv = AcedCast5.GetOrdinaryIV(scheduledKey);
            byte[] result = (byte[])data.Clone();
            AcedCast5.DecryptCBC(scheduledKey, result, 0, result.Length, iv);
            AcedCast5.ClearKey(scheduledKey);

            return TrimRightNull(result);
        }

        private byte[] EncodeText(string text)
        {
            return Encoding.UTF8.GetBytes(text);
        }

        private string DecodeText(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }

        protected byte[] Concat(byte[] bytes0, params byte[][] bytes)
        {
            return bytes.Aggregate(bytes0, Concat);
        }

        protected byte[] Concat(byte[] first, byte[] second)
        {
            var ret = new byte[first.Length + second.Length];

            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);

            return ret;
        }
        private byte[] ComputeHash(byte[] data)
        {
            using (var sha = SHA256.Create()) return sha.ComputeHash(data);
        }

        private SecureString MakeSecureInput(string i)
        {
            var s = new SecureString();
            foreach (char c in i) {    s.AppendChar(c);   }

            return s;
        }

        public byte[] Encode(string data, string raw_password)
        {
            var bdata = EncodeText(data);

            SecureString password = MakeSecureInput(raw_password);
            var cdata = EncodeBytes(bdata, password);

            var result = Concat(ComputeHash(bdata), cdata);
            //return Concat(ComputeHash(bdata), cdata);
            return result;
        }

        public string Decode(byte[] data, string raw_password)
        {
            var bdata = data.Skip(HASH_LENGTH).ToArray();
            var hash = data.Take(HASH_LENGTH).ToArray();

            SecureString password = MakeSecureInput(raw_password);
            var cdata = DecodeBytes(bdata, password);

            if (!hash.SequenceEqual(ComputeHash(cdata)))
            {
                return "";
            }

            return DecodeText(cdata);
        }

        public string BytesToString(byte[] input)
        {
            var result = Convert.ToBase64String(input, 0, input.Length,
                                Base64FormattingOptions.InsertLineBreaks);
            return result;
        }
    }
}
