# EncSypher

A Windows desktop encryption tool built in C# (.NET 9). Encrypts and decrypts text and files using **AES-256-GCM** with a four-part passphrase key system.

Made by **Unicorn God**.

---

## What it does

- **Text encryption / decryption** — paste text in, get a Base64-encoded encrypted packet out. Share the packet; only someone with the same four keys can read it.
- **File encryption / decryption** — encrypt any file to a `.enc` file. Streamed in 1 MiB chunks so large files work fine.
- **Random key generator** — generate random numeric keys up to a chosen maximum.
- **Key hiding** — toggle password-char masking on the key boxes.

---

## Prerequisites

You need the **.NET 9 SDK** installed. Download it from:

https://dotnet.microsoft.com/download/dotnet/9.0

To check if you already have it:

```
dotnet --version
```

It should say `9.x.x`. Any `9.x.x` version works.

---

## Run from source

```
cd "encryption cypher app"
dotnet run
```

The app window will open.

---

## Build a standalone .exe

Run this command from the **root of the repo** (the folder containing the `.sln` file):

```
dotnet publish "encryption cypher app/encryption cypher app.csproj" -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ./publish
```

When it finishes, your `.exe` is at `./publish/encryption cypher app.exe`. You can copy that single file anywhere and run it — no .NET install needed on the target machine.

> For 32-bit (x86) systems, replace `win-x64` with `win-x86`.

---

## How the keys work

There are four key boxes (Key 1 – Key 4). Internally they are joined with a newline separator and treated as a single passphrase:

```
passphrase = Key1 + "\n" + Key2 + "\n" + Key3 + "\n" + Key4
```

All four boxes must match exactly on both sides to decrypt successfully. Keys can be any text — words, numbers, symbols, or a mix.

---

## Using the crypto engine separately

`CryptoEngineBlueprint.cs` is a standalone static class. You can drop it into any C# project and call it directly without the UI.

### Text encryption

```csharp
// Encrypt
string encrypted = CryptoEngineBlueprint.EncryptToBase64(
    plaintextUtf8: "hello world",
    keyParts: new[] { "key1", "key2", "key3", "key4" }
);

// Decrypt
var (plaintext, tagOk) = CryptoEngineBlueprint.DecryptFromBase64(
    base64: encrypted,
    bogusOnFail: false,         // false = return ("", false) on auth failure
    keyParts: new[] { "key1", "key2", "key3", "key4" }
);

if (tagOk)
    Console.WriteLine(plaintext);   // "hello world"
else
    Console.WriteLine("Wrong key or tampered packet.");
```

Or pass a single pre-joined passphrase string:

```csharp
string encrypted = CryptoEngineBlueprint.EncryptToBase64("hello world", "my-passphrase");
var (plaintext, tagOk) = CryptoEngineBlueprint.DecryptFromBase64(encrypted, "my-passphrase");
```

### File encryption

```csharp
// Encrypt a file
CryptoEngineBlueprint.EncryptFile(
    inputPath:  @"C:\documents\secret.pdf",
    outputPath: @"C:\documents\secret.pdf.enc",
    passphrase: "my-passphrase"
);

// Decrypt a file
bool ok = CryptoEngineBlueprint.DecryptFile(
    inputEncPath: @"C:\documents\secret.pdf.enc",
    outputPath:   @"C:\documents\secret_decrypted.pdf",
    passphrase:   "my-passphrase"
);

if (!ok)
    Console.WriteLine("Decryption failed — wrong key or file was tampered with.");
```

### Return values

| Method | Returns | Notes |
|--------|---------|-------|
| `EncryptToBase64` | `string` | Base64 packet — safe to copy/paste/transmit as text |
| `DecryptFromBase64` | `(string plaintext, bool tagOk)` | `tagOk = false` means wrong key or tampered |
| `EncryptFile` | `void` | Throws on IO error |
| `DecryptFile` | `bool` | `false` = auth failure or IO error; output file is not written on failure |

### `bogusOnFail` flag

When `bogusOnFail: true` (the app's default), a failed decryption returns random garbage text instead of an empty string. This is intentional — it prevents an attacker from easily distinguishing "wrong key" from "this is encrypted data" just by checking the output length.

---

## Security details

| Property | Value |
|----------|-------|
| Cipher | AES-256-GCM (authenticated encryption) |
| Key derivation | PBKDF2-SHA256, 200,000 iterations |
| Salt | 16 bytes, random per message |
| Nonce | 12 bytes, random per message |
| Authentication tag | 16 bytes (GCM tag) |
| File header auth | HMAC-SHA256 |
| File chunking | 1 MiB chunks, each independently authenticated |

Every encrypted message or file carries its own random salt and nonce — encrypting the same plaintext twice always produces a different ciphertext. The authentication tag means any tampering with the ciphertext is detected and rejected before any data is returned.

---

## License

MIT — see [LICENSE](LICENSE).
