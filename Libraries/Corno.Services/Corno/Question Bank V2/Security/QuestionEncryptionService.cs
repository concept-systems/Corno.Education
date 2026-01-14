using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Corno.Logger;

namespace Corno.Services.Corno.Question_Bank_V2.Security
{
    /// <summary>
    /// Encryption service with hardcoded standard keys
    /// </summary>
    public class QuestionEncryptionService
    {
        // Standard Hardcoded Keys - DO NOT CHANGE THESE VALUES
        // Changing these will make all encrypted data unreadable
        
        // Current Key Version
        private const int CURRENT_KEY_VERSION = 1;
        
        // Standard Key V1 (256-bit / 32 bytes)
        private static readonly byte[] STANDARD_KEY_V1 = new byte[]
        {
            0x51, 0x75, 0x65, 0x73, 0x74, 0x69, 0x6F, 0x6E, // "Question"
            0x42, 0x61, 0x6E, 0x6B, 0x56, 0x32, 0x45, 0x6E, // "BankV2En"
            0x63, 0x72, 0x79, 0x70, 0x74, 0x69, 0x6F, 0x6E, // "cryption"
            0x4B, 0x65, 0x79, 0x32, 0x30, 0x32, 0x34, 0x21  // "Key2024!"
        };
        
        // Standard IV V1 (128-bit / 16 bytes)
        private static readonly byte[] STANDARD_IV_V1 = new byte[]
        {
            0x51, 0x42, 0x56, 0x32, 0x49, 0x56, 0x30, 0x31, // "QBV2IV01"
            0x32, 0x30, 0x32, 0x34, 0x53, 0x65, 0x63, 0x21  // "2024Sec!"
        };
        
        // Future Key V2 (for rotation - currently inactive)
        private static readonly byte[] STANDARD_KEY_V2 = new byte[]
        {
            0x51, 0x75, 0x65, 0x73, 0x74, 0x69, 0x6F, 0x6E, // "Question"
            0x42, 0x61, 0x6E, 0x6B, 0x56, 0x32, 0x45, 0x6E, // "BankV2En"
            0x63, 0x72, 0x79, 0x70, 0x74, 0x69, 0x6F, 0x6E, // "cryption"
            0x4B, 0x65, 0x79, 0x32, 0x30, 0x32, 0x35, 0x21  // "Key2025!"
        };
        
        private static readonly byte[] STANDARD_IV_V2 = new byte[]
        {
            0x51, 0x42, 0x56, 0x32, 0x49, 0x56, 0x30, 0x32, // "QBV2IV02"
            0x32, 0x30, 0x32, 0x35, 0x53, 0x65, 0x63, 0x21  // "2025Sec!"
        };
        
        private readonly Dictionary<int, EncryptionKeyInfo> _encryptionKeys;
        
        public QuestionEncryptionService()
        {
            _encryptionKeys = InitializeKeys();
        }
        
        /// <summary>
        /// Initializes hardcoded encryption keys
        /// </summary>
        private Dictionary<int, EncryptionKeyInfo> InitializeKeys()
        {
            var keys = new Dictionary<int, EncryptionKeyInfo>();
            
            // Version 1 (Current - Active)
            keys[1] = new EncryptionKeyInfo
            {
                Version = 1,
                Key = STANDARD_KEY_V1,
                IV = STANDARD_IV_V1,
                IsActive = true
            };
            
            // Version 2 (Future - Inactive, for key rotation)
            keys[2] = new EncryptionKeyInfo
            {
                Version = 2,
                Key = STANDARD_KEY_V2,
                IV = STANDARD_IV_V2,
                IsActive = false
            };
            
            return keys;
        }
        
        /// <summary>
        /// Encrypts question text using current key version
        /// </summary>
        public byte[] Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return null;
            
            return EncryptWithVersion(plainText, CURRENT_KEY_VERSION);
        }
        
        /// <summary>
        /// Encrypts with specific key version
        /// </summary>
        private byte[] EncryptWithVersion(string plainText, int keyVersion)
        {
            if (!_encryptionKeys.ContainsKey(keyVersion))
            {
                throw new Exception($"Encryption key version {keyVersion} not available.");
            }
            
            var keyInfo = _encryptionKeys[keyVersion];
            
            try
            {
                using (var aes = Aes.Create())
                {
                    aes.Key = keyInfo.Key;
                    aes.IV = keyInfo.IV;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    
                    using (var encryptor = aes.CreateEncryptor())
                    using (var ms = new MemoryStream())
                    {
                        // Write encryption marker (0x01) and key version
                        ms.WriteByte(0x01); // Encryption marker
                        ms.WriteByte((byte)keyVersion); // Key version
                        
                        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        using (var sw = new StreamWriter(cs, Encoding.UTF8))
                        {
                            sw.Write(plainText);
                        }
                        
                        return ms.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHandler.LogError(ex);
                throw new Exception($"Encryption failed with key version {keyVersion}.", ex);
            }
        }
        
        /// <summary>
        /// Decrypts question text, auto-detecting key version
        /// </summary>
        public string Decrypt(byte[] encryptedData)
        {
            if (encryptedData == null || encryptedData.Length == 0)
                return null;
            
            try
            {
                // Check encryption marker
                if (encryptedData[0] != 0x01)
                {
                    // Legacy unencrypted data - return as UTF-8 string
                    return Encoding.UTF8.GetString(encryptedData);
                }
                
                // Read key version (second byte)
                int keyVersion = encryptedData[1];
                
                if (!_encryptionKeys.ContainsKey(keyVersion))
                {
                    LogHandler.LogError(new Exception($"Key version {keyVersion} not found. Cannot decrypt data."));
                    throw new Exception($"Cannot decrypt: Key version {keyVersion} is not available. Please ensure all key versions are implemented.");
                }
                
                return DecryptWithVersion(encryptedData, keyVersion);
            }
            catch (Exception ex)
            {
                LogHandler.LogError(ex);
                
                // Last resort: try as plain text (for migration scenarios)
                try
                {
                    return Encoding.UTF8.GetString(encryptedData);
                }
                catch
                {
                    throw new Exception("Failed to decrypt question text. Data may be corrupted or encryption key is missing.", ex);
                }
            }
        }
        
        /// <summary>
        /// Decrypts with specific key version
        /// </summary>
        private string DecryptWithVersion(byte[] encryptedData, int keyVersion)
        {
            if (!_encryptionKeys.ContainsKey(keyVersion))
            {
                throw new Exception($"Key version {keyVersion} not available.");
            }
            
            var keyInfo = _encryptionKeys[keyVersion];
            
            try
            {
                using (var aes = Aes.Create())
                {
                    aes.Key = keyInfo.Key;
                    aes.IV = keyInfo.IV;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    
                    // Skip marker byte (0x01) and version byte
                    var encryptedBytes = new byte[encryptedData.Length - 2];
                    Array.Copy(encryptedData, 2, encryptedBytes, 0, encryptedBytes.Length);
                    
                    using (var decryptor = aes.CreateDecryptor())
                    using (var ms = new MemoryStream(encryptedBytes))
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    using (var sr = new StreamReader(cs, Encoding.UTF8))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHandler.LogError(ex);
                throw new Exception($"Decryption failed with key version {keyVersion}.", ex);
            }
        }
        
        /// <summary>
        /// Gets key version from encrypted data
        /// </summary>
        public int? GetKeyVersion(byte[] encryptedData)
        {
            if (encryptedData == null || encryptedData.Length < 2)
                return null;
            
            if (encryptedData[0] != 0x01)
                return null; // Not encrypted
            
            return encryptedData[1]; // Key version
        }
        
        /// <summary>
        /// Checks if data needs re-encryption
        /// </summary>
        public bool NeedsReEncryption(byte[] encryptedData)
        {
            var version = GetKeyVersion(encryptedData);
            return version.HasValue && version.Value < CURRENT_KEY_VERSION;
        }
        
        /// <summary>
        /// Gets current key version
        /// </summary>
        public int GetCurrentKeyVersion()
        {
            return CURRENT_KEY_VERSION;
        }
    }
    
    /// <summary>
    /// Encryption key information
    /// </summary>
    public class EncryptionKeyInfo
    {
        public int Version { get; set; }
        public byte[] Key { get; set; }
        public byte[] IV { get; set; }
        public bool IsActive { get; set; }
    }
}
