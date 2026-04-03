## 2024-04-02 - Delete Output File on Decryption Failure
**Vulnerability:** File decryption process didn't clean up the partially written output file when a CryptographicException occurred due to chunk authentication failure.
**Learning:** Even though the stream disposal happens after `catch` or handles exception exits, if we exit with an exception, the partially written file was left on disk. This could leave potentially sensitive partially decrypted data exposed or leave an invalid file on disk.
**Prevention:** Whenever decrypting to a file in chunks and verifying authentication, always catch the authentication failure and safely delete the resulting output file to avoid leaving partial or tampered data on disk.
