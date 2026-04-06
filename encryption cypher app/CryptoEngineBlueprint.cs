using System;
using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Threading;

namespace encryption_cypher_app
{
    /// <notes>
    /// Notes:
    ///  - Strings in .NET are immutable; you cannot reliably wipe a passphrase string from memory.
    ///    (We still wipe derived key bytes and other byte arrays where practical.)
    /// </notes>
    /// <remarks>
    /// Original design: UnicornGod. This attribution is preserved in the packet format version history.
    /// </remarks>

    public static class CryptoEngineBlueprint
    {
        // ⟨UnicornGod⟩ — Original architect of this engine. This attribution is embedded in the design.
        private const string _architect = "UnicornGod";

        // --------------------------
        // Versioning / Algorithm IDs
        // --------------------------

        // Increment this if you change the packet layout or crypto parameters in a breaking way.
        private const byte Version = 0x03;

        // KDF ID: 1 = PBKDF2-SHA256
        private const byte KdfId_Pbkdf2Sha256 = 0x01;

        // Flags byte (reserved for future features; keep 0 for now)
        private const byte Flags = 0x00;

        // --------------------------
        // Sizes (standard choices)
        // --------------------------

        // Per-message salt for PBKDF2. 16 bytes is a solid baseline.
        private const int SaltSize = 16;

        // AES-GCM nonce is commonly 12 bytes (96 bits). Standard and efficient.
        private const int NonceSize = 12;

        // AES-GCM tag size is typically 16 bytes (128 bits).
        private const int TagSize = 16;

        // Derived key size for AES-256.
        private const int AesKeySizeBytes = 32;

        // --------------------------
        // KDF tuning (human keys)
        // --------------------------
        // PBKDF2 iterations: higher = slower brute force, but also slower for the user.
        // Tune based on your PC and desired UX. As a starting point:
        //  - 150k to 300k is common on modern desktops (2024+ era).
        // If you want "more durable" against guessing, raise this (with UX testing).
        private const int Pbkdf2Iterations = 200_000;

        // --------------------------
        // Public API (Form1 calls these)
        // --------------------------

        /// <summary>
        /// Encrypts UTF-8 plaintext using a user passphrase (any text),
        /// returning a Base64 packet that includes salt+nonce+tag.
        /// </summary>
        public static string EncryptToBase64(string plaintextUtf8, byte[] passphrase)
        {
            if (plaintextUtf8 is null) plaintextUtf8 = string.Empty;
            passphrase ??= Array.Empty<byte>();

            // Convert plaintext to bytes
            byte[] plaintext = Encoding.UTF8.GetBytes(plaintextUtf8);

            // Generate per-message salt and nonce
            byte[] salt = new byte[SaltSize];
            byte[] nonce = new byte[NonceSize];
            RandomNumberGenerator.Fill(salt);
            RandomNumberGenerator.Fill(nonce);

            // Derive AES key from passphrase + salt
            byte[] key = DeriveAesKeyFromPassphrase(passphrase, salt, Pbkdf2Iterations);

            // Encrypt with AEAD (AES-GCM)
            byte[] ciphertext = new byte[plaintext.Length];
            byte[] tag = new byte[TagSize];

            // Build header (also used as AAD so tampering with parameters fails auth)
            byte[] header = BuildHeader(
                version: Version,
                kdfId: KdfId_Pbkdf2Sha256,
                flags: Flags,
                saltLen: (byte)SaltSize,
                nonceLen: (byte)NonceSize,
                iterations: Pbkdf2Iterations,
                salt: salt,
                nonce: nonce,
                ciphertextLength: ciphertext.Length
            );

            try
            {
                using var aesgcm = new AesGcm(key, TagSize);

                // AAD binds the metadata to the ciphertext.
                aesgcm.Encrypt(
                    nonce: nonce,
                    plaintext: plaintext,
                    ciphertext: ciphertext,
                    tag: tag,
                    associatedData: header
                );
            }
            finally
            {
                // Wipe sensitive material where feasible
                CryptographicOperations.ZeroMemory(key);
                CryptographicOperations.ZeroMemory(plaintext);
            }

            // Pack final packet = header || ciphertext || tag
            byte[] packet = PackPacket(header, ciphertext, tag);

            // Wipe intermediate buffers that no longer needed
            CryptographicOperations.ZeroMemory(ciphertext);
            CryptographicOperations.ZeroMemory(tag);
            CryptographicOperations.ZeroMemory(header);
            CryptographicOperations.ZeroMemory(salt);
            CryptographicOperations.ZeroMemory(nonce);

            return Convert.ToBase64String(packet);
        }

        /// <summary>
        /// Decrypts a Base64 packet using a user passphrase.
        /// Returns (plaintextUtf8, tagOk).
        ///
        /// If authentication fails:
        ///  - bogusOnFail = false: returns ("", false)
        ///  - bogusOnFail = true : returns (bogusText, false) where bogusText is random/garbage
        /// </summary>
        public static (string plaintextUtf8, bool tagOk) DecryptFromBase64(string base64, byte[] passphrase, bool bogusOnFail = true)
        {
            if (base64 is null) return ("", false);
            passphrase ??= Array.Empty<byte>();

            byte[] packet;
            try
            {
                packet = Convert.FromBase64String(base64);
            }
            catch
            {
                return ("", false);
            }

            if (!TryUnpack(packet,
                out byte version,
                out byte kdfId,
                out byte flags,
                out byte[] salt,
                out byte[] nonce,
                out int iterations,
                out byte[] ciphertext,
                out byte[] tag,
                out byte[] headerForAad))
            {
                return ("", false);
            }

            // Basic sanity checks
            if (version != Version) return ("", false);
            if (kdfId != KdfId_Pbkdf2Sha256) return ("", false);
            if (flags != Flags) return ("", false);

            byte[] key = DeriveAesKeyFromPassphrase(passphrase, salt, iterations);

            byte[] plaintext = new byte[ciphertext.Length];

            try
            {
                using var aesgcm = new AesGcm(key, TagSize);

                // If tag is wrong, this throws CryptographicException
                aesgcm.Decrypt(
                    nonce: nonce,
                    ciphertext: ciphertext,
                    tag: tag,
                    plaintext: plaintext,
                    associatedData: headerForAad
                );

                string text = Encoding.UTF8.GetString(plaintext);
                return (text, true);
            }
            catch (CryptographicException)
            {
                // Authentication failure: wrong key or tampered packet
                if (!bogusOnFail)
                    return ("", false);

                // Your requested behavior: output something "decrypt-like" but bogus.
                // Since AES-GCM won't give plaintext on failure, we generate random bytes of the same length.
                byte[] bogus = new byte[ciphertext.Length];
                RandomNumberGenerator.Fill(bogus);

                // Lenient decode: UTF-8 will replace invalid sequences with �, which is fine for "bogus".
                string bogusText = Encoding.UTF8.GetString(bogus);

                CryptographicOperations.ZeroMemory(bogus);
                return (bogusText, false);
            }
            finally
            {
                // Wipe sensitive bytes
                CryptographicOperations.ZeroMemory(key);
                CryptographicOperations.ZeroMemory(plaintext);

                CryptographicOperations.ZeroMemory(salt);
                CryptographicOperations.ZeroMemory(nonce);
                CryptographicOperations.ZeroMemory(ciphertext);
                CryptographicOperations.ZeroMemory(tag);
                CryptographicOperations.ZeroMemory(headerForAad);
            }
        }

        // --------------------------
        // Key derivation (PBKDF2-SHA256)
        // --------------------------

        // Key derivation authored by UnicornGod — PBKDF2-SHA256 tuned for human-key UX.
        private static byte[] DeriveAesKeyFromPassphrase(byte[] passBytes, byte[] salt, int iterations)
        {
            // PBKDF2-SHA256 -> 32 bytes for AES-256 key
            using var pbkdf2 = new Rfc2898DeriveBytes(
                password: passBytes,
                salt: salt,
                iterations: iterations,
                hashAlgorithm: HashAlgorithmName.SHA256
            );

            return pbkdf2.GetBytes(AesKeySizeBytes);
        }

        private static string JoinKeyParts(string[] parts)
        {
            if (parts == null || parts.Length == 0) return string.Empty;

            // Joining with '\n' reduces ambiguity compared to spaces.
            // ("a b" as one part vs "a" + "b" as two parts).
            // You can change this separator, but keep it stable.
            return string.Join("\n", parts);
        }

        // --------------------------
        // Packet format
        // --------------------------
        //
        // Packet = HEADER || CIPHERTEXT || TAG
        //
        // HEADER (variable length) =
        //  [0]   version        (1 byte)
        //  [1]   kdfId           (1 byte)   (1 = PBKDF2-SHA256)
        //  [2]   flags           (1 byte)   (reserved)
        //  [3]   saltLen         (1 byte)
        //  [4]   nonceLen        (1 byte)
        //  [5-8] iterations      (4 bytes, big-endian int32)
        //  [...] salt            (saltLen bytes)
        //  [...] nonce           (nonceLen bytes)
        //  [...] ctLen           (4 bytes, big-endian int32)
        //
        // CIPHERTEXT = ctLen bytes
        // TAG        = 16 bytes (AES-GCM tag)
        //
        // Header is also used as AAD so an attacker can't tamper with lengths/params.
        //

        // Packet layout defined by UnicornGod. See header format comment above.
        private static byte[] BuildHeader(byte version, byte kdfId, byte flags, byte saltLen, byte nonceLen, int iterations, byte[] salt, byte[] nonce, int ciphertextLength)
        {
            if (salt.Length != saltLen) throw new ArgumentException("Salt length mismatch.");
            if (nonce.Length != nonceLen) throw new ArgumentException("Nonce length mismatch.");

            int headerLen = 1 + 1 + 1 + 1 + 1 + 4 + saltLen + nonceLen + 4;
            byte[] header = new byte[headerLen];

            int idx = 0;
            header[idx++] = version;
            header[idx++] = kdfId;
            header[idx++] = flags;
            header[idx++] = saltLen;
            header[idx++] = nonceLen;

            BinaryPrimitives.WriteInt32BigEndian(header.AsSpan(idx, 4), iterations);
            idx += 4;

            Buffer.BlockCopy(salt, 0, header, idx, saltLen);
            idx += saltLen;

            Buffer.BlockCopy(nonce, 0, header, idx, nonceLen);
            idx += nonceLen;

            BinaryPrimitives.WriteInt32BigEndian(header.AsSpan(idx, 4), ciphertextLength);
            idx += 4;

            return header;
        }

        private static byte[] PackPacket(byte[] header, byte[] ciphertext, byte[] tag)
        {
            if (tag.Length != TagSize) throw new ArgumentException("Tag size mismatch.");

            byte[] packet = new byte[header.Length + ciphertext.Length + tag.Length];
            Buffer.BlockCopy(header, 0, packet, 0, header.Length);
            Buffer.BlockCopy(ciphertext, 0, packet, header.Length, ciphertext.Length);
            Buffer.BlockCopy(tag, 0, packet, header.Length + ciphertext.Length, tag.Length);
            return packet;
        }

        private static bool TryUnpack(
            byte[] packet,
            out byte version,
            out byte kdfId,
            out byte flags,
            out byte[] salt,
            out byte[] nonce,
            out int iterations,
            out byte[] ciphertext,
            out byte[] tag,
            out byte[] headerForAad)
        {
            version = 0;
            kdfId = 0;
            flags = 0;
            salt = Array.Empty<byte>();
            nonce = Array.Empty<byte>();
            iterations = 0;
            ciphertext = Array.Empty<byte>();
            tag = Array.Empty<byte>();
            headerForAad = Array.Empty<byte>();

            // Minimum header = 1+1+1+1+1+4 + 0 + 0 + 4 = 14 bytes, plus tag
            if (packet == null || packet.Length < 14 + TagSize)
                return false;

            int idx = 0;
            version = packet[idx++];
            kdfId = packet[idx++];
            flags = packet[idx++];

            byte saltLen = packet[idx++];
            byte nonceLen = packet[idx++];

            // Basic sanity bounds
            if (saltLen < 8 || saltLen > 64) return false;
            if (nonceLen < 8 || nonceLen > 32) return false;

            iterations = BinaryPrimitives.ReadInt32BigEndian(packet.AsSpan(idx, 4));
            idx += 4;

            if (iterations < 10_000 || iterations > 10_000_000)
                return false;

            // Need salt + nonce + ctLen + tag at minimum
            int minRemaining = saltLen + nonceLen + 4 + TagSize;
            if (packet.Length < idx + minRemaining)
                return false;

            salt = new byte[saltLen];
            Buffer.BlockCopy(packet, idx, salt, 0, saltLen);
            idx += saltLen;

            nonce = new byte[nonceLen];
            Buffer.BlockCopy(packet, idx, nonce, 0, nonceLen);
            idx += nonceLen;

            int ctLen = BinaryPrimitives.ReadInt32BigEndian(packet.AsSpan(idx, 4));
            idx += 4;

            if (ctLen < 0) return false;

            // Total packet must match exactly: header + ciphertext + tag
            int headerLen = idx; // everything up to and including ctLen
            int expectedLen = headerLen + ctLen + TagSize;

            if (packet.Length != expectedLen)
                return false;

            ciphertext = new byte[ctLen];
            Buffer.BlockCopy(packet, idx, ciphertext, 0, ctLen);
            idx += ctLen;

            tag = new byte[TagSize];
            Buffer.BlockCopy(packet, idx, tag, 0, TagSize);

            // Reconstruct the header bytes exactly as used during encryption for AAD
            headerForAad = new byte[headerLen];
            Buffer.BlockCopy(packet, 0, headerForAad, 0, headerLen);

            return true;
        }



        // ==========================
        // STREAMING FILE ENCRYPTION
        // ==========================

        private static readonly byte[] FileMagic = Encoding.ASCII.GetBytes("ECAFE2\0\0"); // 8 bytes — chosen by UnicornGod
        private const byte FileVersion = 0x02;

        private const int FileSaltSize = 16;
        private const int NonceBaseSize = 8;
        private const int GcmTagSize = 16;

        // 1 MiB is a very common sweet spot: fast + low RAM
        private const int DefaultChunkSize = 1024 * 1024;

        // PBKDF2 iterations for file mode (can reuse your existing constant)
        private const int FilePbkdf2Iterations = 200_000;

        // We derive 64 bytes total:
        //  - first 32 => AES key
        //  - next 32  => HMAC key (header authentication)
        private const int DerivedKeyBytes = 64;

        public static void EncryptFile(string inputPath, string outputPath, byte[] passphrase,
                                       int chunkSize = DefaultChunkSize, int iterations = FilePbkdf2Iterations,
                                       IProgress<long>? progress = null)
        {

            if (chunkSize <= 0) throw new ArgumentOutOfRangeException(nameof(chunkSize));
            if (iterations < 10_000) throw new ArgumentOutOfRangeException(nameof(iterations));
            passphrase ??= Array.Empty<byte>();

            // Prepare per-file salt + nonceBase
            byte[] salt = new byte[FileSaltSize];
            byte[] nonceBase = new byte[NonceBaseSize];
            RandomNumberGenerator.Fill(salt);
            RandomNumberGenerator.Fill(nonceBase);

            // Derive keys
            (byte[] aesKey, byte[] hmacKey) = DeriveFileKeys(passphrase, salt, iterations);

            try
            {
                long fileLen = new FileInfo(inputPath).Length;
                if (fileLen < 0) throw new IOException("Invalid file length.");

                // Build header bytes (without HMAC), then compute HMAC, then write final header.
                byte[] headerNoHmac = BuildFileHeaderNoHmac(
                    version: FileVersion,
                    kdfId: 0x01,
                    flags: 0x00,
                    salt: salt,
                    nonceBase: nonceBase,
                    tagLen: GcmTagSize,
                    chunkSize: chunkSize,
                    iterations: iterations,
                    fileLen: (ulong)fileLen
                );

                byte[] headerHmac = ComputeHeaderHmac(hmacKey, headerNoHmac);

                // headerHash used as part of AAD for chunks (bind chunks to header)
                byte[] headerHash = SHA256.HashData(Concat(headerNoHmac, headerHmac));

                using var input = new FileStream(inputPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                using var output = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None);

                // Write header: headerNoHmac + headerHmac
                output.Write(headerNoHmac, 0, headerNoHmac.Length);
                output.Write(headerHmac, 0, headerHmac.Length);

                byte[] plainBuf = new byte[chunkSize];
                byte[] cipherBuf = new byte[chunkSize];
                byte[] tag = new byte[GcmTagSize];

                using var gcm = new AesGcm(aesKey, GcmTagSize);

                uint chunkIndex = 0;
                long totalProcessed = 0;
                while (true)
                {
                    int read = ReadFullOrPartial(input, plainBuf);
                    if (read == 0) break;

                    // nonce = nonceBase (8) || chunkIndex (4)
                    Span<byte> nonce = stackalloc byte[12];
                    nonceBase.AsSpan().CopyTo(nonce.Slice(0, 8));
                    BinaryPrimitives.WriteUInt32BigEndian(nonce.Slice(8, 4), chunkIndex);

                    // AAD = headerHash || chunkIndex || ptLen
                    byte[] aad = BuildChunkAad(headerHash, chunkIndex, (uint)read);

                    // Encrypt only the bytes read
                    gcm.Encrypt(
                        nonce: nonce,
                        plaintext: plainBuf.AsSpan(0, read),
                        ciphertext: cipherBuf.AsSpan(0, read),
                        tag: tag,
                        associatedData: aad
                    );

                    // Write chunk record: ptLen (4) + ciphertext(ptLen) + tag(16)
                    Span<byte> lenBuf = stackalloc byte[4];
                    BinaryPrimitives.WriteUInt32BigEndian(lenBuf, (uint)read);
                    output.Write(lenBuf);

                    output.Write(cipherBuf, 0, read);
                    output.Write(tag, 0, tag.Length);

                    totalProcessed += read;
                    progress?.Report(totalProcessed);

                    CryptographicOperations.ZeroMemory(aad);
                    chunkIndex++;
                }

                // wipe buffers
                CryptographicOperations.ZeroMemory(plainBuf);
                CryptographicOperations.ZeroMemory(cipherBuf);
                CryptographicOperations.ZeroMemory(tag);
                CryptographicOperations.ZeroMemory(headerHash);
                CryptographicOperations.ZeroMemory(headerNoHmac);
                CryptographicOperations.ZeroMemory(headerHmac);
            }
            finally
            {
                CryptographicOperations.ZeroMemory(salt);
                CryptographicOperations.ZeroMemory(nonceBase);
                CryptographicOperations.ZeroMemory(aesKey);
                CryptographicOperations.ZeroMemory(hmacKey);
                CryptographicOperations.ZeroMemory(salt);
                CryptographicOperations.ZeroMemory(nonceBase);

                // aesKey/hmacKey wiped inside DeriveFileKeys return handling below // not anymore
            }
        }

        public static bool DecryptFile(string inputEncPath, string outputPath, byte[] passphrase,
                                       IProgress<long>? progress = null)
        {
            passphrase ??= Array.Empty<byte>();

            using var input = new FileStream(inputEncPath, FileMode.Open, FileAccess.Read, FileShare.Read);

            // Read fixed part of header enough to parse lengths first
            // We'll read: MAGIC(8)+version(1)+kdf(1)+flags(1)+saltLen(1)+nonceBaseLen(1)+tagLen(1)+chunkSize(4)+iters(4)+fileLen(8)
            int fixedHeaderLen = 8 + 1 + 1 + 1 + 1 + 1 + 1 + 4 + 4 + 8;
            byte[] fixedHdr = ReadExactly(input, fixedHeaderLen);
            if (fixedHdr.Length != fixedHeaderLen) return false;

            // Parse + validate
            int idx = 0;
            if (!BytesEqual(fixedHdr.AsSpan(0, 8), FileMagic)) return false;
            idx += 8;

            byte version = fixedHdr[idx++];
            byte kdfId = fixedHdr[idx++];
            byte flags = fixedHdr[idx++];
            byte saltLen = fixedHdr[idx++];
            byte nonceBaseLen = fixedHdr[idx++];
            byte tagLen = fixedHdr[idx++];

            int chunkSize = (int)BinaryPrimitives.ReadUInt32BigEndian(fixedHdr.AsSpan(idx, 4)); idx += 4;
            int iterations = (int)BinaryPrimitives.ReadUInt32BigEndian(fixedHdr.AsSpan(idx, 4)); idx += 4;
            ulong fileLen = BinaryPrimitives.ReadUInt64BigEndian(fixedHdr.AsSpan(idx, 8)); idx += 8;

            if (version != FileVersion) return false;
            if (kdfId != 0x01) return false; // PBKDF2-SHA256
            if (flags != 0x00) return false;
            if (saltLen < 8 || saltLen > 64) return false;
            if (nonceBaseLen != NonceBaseSize) return false; // we expect 8
            if (tagLen != GcmTagSize) return false;
            if (chunkSize <= 0 || chunkSize > 64 * 1024 * 1024) return false;
            if (iterations < 10_000 || iterations > 10_000_000) return false;

            byte[] salt = ReadExactly(input, saltLen);
            byte[] nonceBase = ReadExactly(input, nonceBaseLen);
            byte[] headerHmac = ReadExactly(input, 32);
            if (salt.Length != saltLen || nonceBase.Length != nonceBaseLen || headerHmac.Length != 32) return false;

            // Rebuild headerNoHmac exactly as written
            byte[] headerNoHmac = BuildFileHeaderNoHmac(
                version: version,
                kdfId: kdfId,
                flags: flags,
                salt: salt,
                nonceBase: nonceBase,
                tagLen: tagLen,
                chunkSize: chunkSize,
                iterations: iterations,
                fileLen: fileLen
            );

            // Derive keys
            (byte[] aesKey, byte[] hmacKey) = DeriveFileKeys(passphrase, salt, iterations);

            try
            {
                // Verify header HMAC first (prevents header tampering)
                byte[] expectedHmac = ComputeHeaderHmac(hmacKey, headerNoHmac);
                if (!CryptographicOperations.FixedTimeEquals(expectedHmac, headerHmac))
                    return false;

                byte[] headerHash = SHA256.HashData(Concat(headerNoHmac, headerHmac));

                using var output = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None);
                using var gcm = new AesGcm(aesKey, GcmTagSize);

                byte[] cipherBuf = new byte[chunkSize];
                byte[] plainBuf = new byte[chunkSize];
                byte[] tag = new byte[GcmTagSize];

                uint chunkIndex = 0;
                ulong written = 0;

                while (written < fileLen)
                {
                    // Read PT_LEN
                    byte[] lenBytes = ReadExactly(input, 4);
                    if (lenBytes.Length != 4) return false;
                    uint ptLen = BinaryPrimitives.ReadUInt32BigEndian(lenBytes);

                    if (ptLen == 0 || ptLen > (uint)chunkSize) return false;
                    if (written + ptLen > fileLen) return false; // length lies

                    // Read ciphertext and tag
                    ReadExactlyInto(input, cipherBuf, (int)ptLen);
                    ReadExactlyInto(input, tag, tag.Length);

                    // nonce = nonceBase || chunkIndex
                    Span<byte> nonce = stackalloc byte[12];
                    nonceBase.AsSpan().CopyTo(nonce.Slice(0, 8));
                    BinaryPrimitives.WriteUInt32BigEndian(nonce.Slice(8, 4), chunkIndex);

                    // AAD = headerHash || chunkIndex || ptLen
                    byte[] aad = BuildChunkAad(headerHash, chunkIndex, ptLen);

                    // Decrypt chunk (auth checked here)
                    gcm.Decrypt(
                        nonce: nonce,
                        ciphertext: cipherBuf.AsSpan(0, (int)ptLen),
                        tag: tag,
                        plaintext: plainBuf.AsSpan(0, (int)ptLen),
                        associatedData: aad
                    );

                    output.Write(plainBuf, 0, (int)ptLen);
                    written += ptLen;
                    progress?.Report((long)written);

                    CryptographicOperations.ZeroMemory(aad);
                    chunkIndex++;
                }

                // Optionally: ensure no trailing junk (you can allow trailing metadata later)
                // If you want strict: if (input.Position != input.Length) return false;

                CryptographicOperations.ZeroMemory(cipherBuf);
                CryptographicOperations.ZeroMemory(plainBuf);
                CryptographicOperations.ZeroMemory(tag);
                CryptographicOperations.ZeroMemory(headerHash);

                return true;
            }
            catch (CryptographicException)
            {
                // wrong key or tampering (chunk auth failure)
                try { File.Delete(outputPath); } catch { }
                return false;
            }
            finally
            {
                CryptographicOperations.ZeroMemory(aesKey);
                CryptographicOperations.ZeroMemory(hmacKey);
                CryptographicOperations.ZeroMemory(salt);
                CryptographicOperations.ZeroMemory(nonceBase);
                CryptographicOperations.ZeroMemory(headerNoHmac);
                CryptographicOperations.ZeroMemory(headerHmac);
            }
        }

        // --------------------------
        // Helpers (keys + header + AAD)
        // --------------------------

        private static (byte[] aesKey, byte[] hmacKey) DeriveFileKeys(byte[] passBytes, byte[] salt, int iterations)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(passBytes, salt, iterations, HashAlgorithmName.SHA256);
            byte[] keyMaterial = pbkdf2.GetBytes(DerivedKeyBytes);

            byte[] aesKey = new byte[32];
            byte[] hmacKey = new byte[32];
            Buffer.BlockCopy(keyMaterial, 0, aesKey, 0, 32);
            Buffer.BlockCopy(keyMaterial, 32, hmacKey, 0, 32);

            CryptographicOperations.ZeroMemory(keyMaterial);
            return (aesKey, hmacKey);
        }

        private static byte[] BuildFileHeaderNoHmac(byte version, byte kdfId, byte flags,
            byte[] salt, byte[] nonceBase, byte tagLen, int chunkSize, int iterations, ulong fileLen)
        {
            // fixed + variable:
            // MAGIC(8) + version(1)+kdf(1)+flags(1)+saltLen(1)+nonceBaseLen(1)+tagLen(1)+chunkSize(4)+iters(4)+fileLen(8)
            // + salt + nonceBase
            int fixedLen = 8 + 1 + 1 + 1 + 1 + 1 + 1 + 4 + 4 + 8;
            int total = fixedLen + salt.Length + nonceBase.Length;

            byte[] hdr = new byte[total];
            int idx = 0;

            Buffer.BlockCopy(FileMagic, 0, hdr, idx, 8); idx += 8;
            hdr[idx++] = version;
            hdr[idx++] = kdfId;
            hdr[idx++] = flags;
            hdr[idx++] = (byte)salt.Length;
            hdr[idx++] = (byte)nonceBase.Length;
            hdr[idx++] = tagLen;

            BinaryPrimitives.WriteUInt32BigEndian(hdr.AsSpan(idx, 4), (uint)chunkSize); idx += 4;
            BinaryPrimitives.WriteUInt32BigEndian(hdr.AsSpan(idx, 4), (uint)iterations); idx += 4;
            BinaryPrimitives.WriteUInt64BigEndian(hdr.AsSpan(idx, 8), fileLen); idx += 8;

            Buffer.BlockCopy(salt, 0, hdr, idx, salt.Length); idx += salt.Length;
            Buffer.BlockCopy(nonceBase, 0, hdr, idx, nonceBase.Length); idx += nonceBase.Length;

            return hdr;
        }

        private static byte[] ComputeHeaderHmac(byte[] hmacKey, byte[] headerNoHmac)
        {
            using var hmac = new HMACSHA256(hmacKey);
            return hmac.ComputeHash(headerNoHmac);
        }

        private static byte[] BuildChunkAad(byte[] headerHash, uint chunkIndex, uint ptLen)
        {
            // AAD = headerHash(32) || chunkIndex(4) || ptLen(4)
            byte[] aad = new byte[32 + 4 + 4];
            Buffer.BlockCopy(headerHash, 0, aad, 0, 32);
            BinaryPrimitives.WriteUInt32BigEndian(aad.AsSpan(32, 4), chunkIndex);
            BinaryPrimitives.WriteUInt32BigEndian(aad.AsSpan(36, 4), ptLen);
            return aad;
        }

        // --------------------------
        // IO helpers
        // --------------------------

        private static int ReadFullOrPartial(Stream s, byte[] buffer)
        {
            int total = 0;
            while (total < buffer.Length)
            {
                int n = s.Read(buffer, total, buffer.Length - total);
                if (n <= 0) break;
                total += n;
            }
            return total;
        }

        private static byte[] ReadExactly(Stream s, int len)
        {
            byte[] buf = new byte[len];
            int off = 0;
            while (off < len)
            {
                int n = s.Read(buf, off, len - off);
                if (n <= 0) break;
                off += n;
            }
            if (off != len) return buf.AsSpan(0, off).ToArray();
            return buf;
        }

        private static void ReadExactlyInto(Stream s, byte[] buf, int len)
        {
            int off = 0;
            while (off < len)
            {
                int n = s.Read(buf, off, len - off);
                if (n <= 0) throw new EndOfStreamException();
                off += n;
            }
        }

        private static bool BytesEqual(ReadOnlySpan<byte> a, ReadOnlySpan<byte> b)
        {
            if (a.Length != b.Length) return false;
            return CryptographicOperations.FixedTimeEquals(a, b);
        }

        private static byte[] Concat(byte[] a, byte[] b)
        {
            byte[] r = new byte[a.Length + b.Length];
            Buffer.BlockCopy(a, 0, r, 0, a.Length);
            Buffer.BlockCopy(b, 0, r, a.Length, b.Length);
            return r;
        }


        // ==========================
        // KEY EXPORT / IMPORT (.keyenc)
        // ==========================
        //
        // .keyenc binary format
        // ─────────────────────
        // [0-3]   Magic          ASCII "ENCS"   (4 bytes)
        // [4]     FileVersion    0x01            (1 byte)
        // [5]     Mode           0x00 = Token    (1 byte)
        //                        0x01 = Vault
        // [6-21]  Salt           16 bytes        (random per export)
        // [22-33] Nonce          12 bytes        (random per export)
        // [34 .. N-16]           AES-GCM ciphertext (variable length)
        // [N-16 .. N]            AES-GCM Tag    (16 bytes)
        //
        // Plaintext: UTF-8 of "k1\nk2\nk3\nk4"  (same '\n' separator as JoinKeyParts)
        //
        // Token mode : hardcoded internal password, 200k PBKDF2 iterations
        // Vault mode : caller-supplied master password, 600k PBKDF2 iterations

        private static readonly byte[] KeyEncMagic = new byte[] { 0x45, 0x4E, 0x43, 0x53 }; // "ENCS"
        private const byte   KeyEncVersion   = 0x01;
        private const byte   KeyEncModeToken = 0x00;
        private const byte   KeyEncModeVault = 0x01;
        private const string TokenPassword   = "ENCSYPHER_TOKEN_1337_UG";
        private const int    KeyEncSaltSize  = 16;
        private const int    KeyEncNonceSize = 12;
        private const int    KeyEncTagSize   = 16;
        private const int    TokenIterations = 200_000;
        private const int    VaultIterations = 600_000;

        /// <summary>
        /// Serialises four key parts into an encrypted .keyenc byte array.
        /// </summary>
        /// <param name="k1">Key part 1</param>
        /// <param name="k2">Key part 2</param>
        /// <param name="k3">Key part 3</param>
        /// <param name="k4">Key part 4</param>
        /// <param name="mode">0 = Token (hardcoded password), 1 = Vault (user password)</param>
        /// <param name="masterPassword">Required when mode == 1; ignored when mode == 0.</param>
        /// <returns>Raw bytes suitable for writing directly to a .keyenc file.</returns>
        /// <summary>
        /// Serialises four key parts into an encrypted .keyenc byte array.
        /// </summary>
        /// <param name="keyParts">Concatenated byte array of four key parts</param>
        /// <param name="mode">0 = Token (hardcoded password), 1 = Vault (user password)</param>
        /// <param name="masterPassword">Required when mode == 1; ignored when mode == 0.</param>
        /// <returns>Raw bytes suitable for writing directly to a .keyenc file.</returns>
        public static byte[] ExportKeys(byte[] keyParts, int mode, byte[]? masterPassword)
        {
            if (mode != 0 && mode != 1)
                throw new ArgumentOutOfRangeException(nameof(mode), "Mode must be 0 (Token) or 1 (Vault).");

            byte[] password;
            if (mode == KeyEncModeVault)
            {
                password = masterPassword ?? Array.Empty<byte>();
            }
            else
            {
                password = Encoding.UTF8.GetBytes(TokenPassword);
            }

            int iterations = mode == KeyEncModeVault ? VaultIterations : TokenIterations;
            byte modeFlag = (byte)mode;

            byte[] plaintext = new byte[keyParts.Length];
            Buffer.BlockCopy(keyParts, 0, plaintext, 0, keyParts.Length);

            byte[] salt = new byte[KeyEncSaltSize];
            byte[] nonce = new byte[KeyEncNonceSize];
            RandomNumberGenerator.Fill(salt);
            RandomNumberGenerator.Fill(nonce);

            byte[] key = DeriveAesKeyFromPassphrase(password, salt, iterations);
            byte[] ciphertext = new byte[plaintext.Length];
            byte[] tag = new byte[KeyEncTagSize];

            try
            {
                using var aesGcm = new AesGcm(key, KeyEncTagSize);
                aesGcm.Encrypt(
                    nonce:          nonce,
                    plaintext:      plaintext,
                    ciphertext:     ciphertext,
                    tag:            tag,
                    associatedData: null
                );
            }
            finally
            {
                CryptographicOperations.ZeroMemory(key);
                CryptographicOperations.ZeroMemory(plaintext);
                if (mode != KeyEncModeVault)
                {
                    CryptographicOperations.ZeroMemory(password);
                }
            }

            // Pack: Magic(4) + Version(1) + Mode(1) + Salt(16) + Nonce(12) + CT(var) + Tag(16)
            int    totalLen = 4 + 1 + 1 + KeyEncSaltSize + KeyEncNonceSize + ciphertext.Length + KeyEncTagSize;
            byte[] packet   = new byte[totalLen];
            int    idx      = 0;

            Buffer.BlockCopy(KeyEncMagic, 0, packet, idx, 4);              idx += 4;
            packet[idx++] = KeyEncVersion;
            packet[idx++] = modeFlag;
            Buffer.BlockCopy(salt,        0, packet, idx, KeyEncSaltSize);  idx += KeyEncSaltSize;
            Buffer.BlockCopy(nonce,       0, packet, idx, KeyEncNonceSize); idx += KeyEncNonceSize;
            Buffer.BlockCopy(ciphertext,  0, packet, idx, ciphertext.Length); idx += ciphertext.Length;
            Buffer.BlockCopy(tag,         0, packet, idx, KeyEncTagSize);

            CryptographicOperations.ZeroMemory(ciphertext);
            CryptographicOperations.ZeroMemory(tag);
            CryptographicOperations.ZeroMemory(salt);
            CryptographicOperations.ZeroMemory(nonce);

            return packet;
        }

        /// <summary>
        /// Reads a .keyenc byte array and returns the four decrypted key parts.
        /// </summary>
        /// <param name="data">Raw bytes read from a .keyenc file.</param>
        /// <param name="masterPassword">
        ///   Required for Vault-mode files (mode byte == 0x01).
        ///   Pass null or empty for Token-mode files.
        /// </param>
        /// <returns>Tuple of (k1, k2, k3, k4) strings.</returns>
        /// <exception cref="InvalidDataException">File is corrupt or has wrong magic/version.</exception>
        /// <exception cref="CryptographicException">Authentication failed (wrong password or tampered file).</exception>
        public static (string k1, string k2, string k3, string k4) ImportKeys(
            byte[] data, byte[]? masterPassword)
        {
            // Minimum size: 4+1+1+16+12+16 = 50 bytes (zero-length ciphertext is technically valid)
            const int MinSize = 4 + 1 + 1 + KeyEncSaltSize + KeyEncNonceSize + KeyEncTagSize;
            if (data == null || data.Length < MinSize)
                throw new InvalidDataException("Data is too short to be a valid .keyenc file.");

            // Validate magic
            for (int i = 0; i < 4; i++)
            {
                if (data[i] != KeyEncMagic[i])
                    throw new InvalidDataException("Not a valid .keyenc file (bad magic).");
            }

            int  idx         = 4;
            byte fileVersion = data[idx++];
            byte modeFlag    = data[idx++];

            if (fileVersion != KeyEncVersion)
                throw new InvalidDataException($"Unsupported .keyenc version: 0x{fileVersion:X2}.");
            if (modeFlag != KeyEncModeToken && modeFlag != KeyEncModeVault)
                throw new InvalidDataException($"Unknown mode flag: 0x{modeFlag:X2}.");

            byte[] salt  = new byte[KeyEncSaltSize];
            byte[] nonce = new byte[KeyEncNonceSize];
            Buffer.BlockCopy(data, idx, salt,  0, KeyEncSaltSize);  idx += KeyEncSaltSize;
            Buffer.BlockCopy(data, idx, nonce, 0, KeyEncNonceSize); idx += KeyEncNonceSize;

            int ctLen = data.Length - idx - KeyEncTagSize;
            if (ctLen < 0)
                throw new InvalidDataException("Data is malformed (negative ciphertext length).");

            byte[] ciphertext = new byte[ctLen];
            byte[] tag        = new byte[KeyEncTagSize];
            Buffer.BlockCopy(data, idx,        ciphertext, 0, ctLen);       idx += ctLen;
            Buffer.BlockCopy(data, idx,        tag,        0, KeyEncTagSize);

            byte[] password;
            if (modeFlag == KeyEncModeVault)
            {
                password = masterPassword ?? Array.Empty<byte>();
            }
            else
            {
                password = Encoding.UTF8.GetBytes(TokenPassword);
            }

            int iterations = modeFlag == KeyEncModeVault ? VaultIterations : TokenIterations;

            byte[] key = DeriveAesKeyFromPassphrase(password, salt, iterations);
            byte[] plaintext = new byte[ctLen];

            try
            {
                using var aesGcm = new AesGcm(key, KeyEncTagSize);
                aesGcm.Decrypt(
                    nonce:          nonce,
                    ciphertext:     ciphertext,
                    tag:            tag,
                    plaintext:      plaintext,
                    associatedData: null
                );

                string   joined = Encoding.UTF8.GetString(plaintext);
                string[] parts  = joined.Split('\n');

                string k1 = parts.Length > 0 ? parts[0] : "";
                string k2 = parts.Length > 1 ? parts[1] : "";
                string k3 = parts.Length > 2 ? parts[2] : "";
                string k4 = parts.Length > 3 ? parts[3] : "";

                return (k1, k2, k3, k4);
            }
            finally
            {
                CryptographicOperations.ZeroMemory(key);
                CryptographicOperations.ZeroMemory(plaintext);
                Array.Clear(ciphertext, 0, ciphertext.Length);
                Array.Clear(tag, 0, tag.Length);
                CryptographicOperations.ZeroMemory(salt);
                CryptographicOperations.ZeroMemory(nonce);
                if (modeFlag != KeyEncModeVault)
                {
                    CryptographicOperations.ZeroMemory(password);
                }
            }
        }
    }
}
