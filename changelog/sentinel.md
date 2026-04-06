## 2024-04-02 - Delete Output File on Decryption Failure
**Vulnerability:** File decryption process didn't clean up the partially written output file when a CryptographicException occurred due to chunk authentication failure.
**Learning:** Even though the stream disposal happens after `catch` or handles exception exits, if we exit with an exception, the partially written file was left on disk. This could leave potentially sensitive partially decrypted data exposed or leave an invalid file on disk.
**Prevention:** Whenever decrypting to a file in chunks and verifying authentication, always catch the authentication failure and safely delete the resulting output file to avoid leaving partial or tampered data on disk.

## 2026-04-06 - [In-Memory Key Protection]
**Vulnerability:** Immutable strings in .NET can linger in RAM until garbage collection, creating a memory-scraping risk for sensitive passphrase data.
**Learning:** Standard string objects in C# cannot be reliably wiped from memory, which is a significant security flaw when handling cryptographic keys or user-supplied passphrases.
**Prevention:** Always read sensitive user inputs into char[] or byte[] buffers at the UI boundary and immediately wipe them using CryptographicOperations.ZeroMemory() once their purpose is served (e.g., after the AES key is derived). This makes the application highly resistant to memory-dump analysis.
