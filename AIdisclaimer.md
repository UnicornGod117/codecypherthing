# AI
Because i understand people want to know whats ai and what isn't, and people dont to be mislead into thinking something was made by a human when it wasn't ill be addressing that for this project here so there is clarity and transparency.

## Human
98% of the UI stuff, button, ect was directly coded by me (unicorn god) using visual studio, though i had used ai to help me learn the code itself, it did not code or make any of that stuff for me. (the 2 percent was claude code upgrading my random key generator (my one was kinda bad and only did numbers, and i couldnt be bothered to upgrade it myself))

## not human
Originally i had made my own crypto for this project which was cool and i was proud to call it all mine. but ai did not like it, and pointed out numerous faults with it (im not professional cryptographer, sorry) so i just got it to create me a customisable blueprint of a pretty strong algorithm, in which it guided me on what different stuff ment and what kinda stuff i can put into where, what to change ect (i at least wanted to chose something myself).

later on when i uploaded this to github i have employed the use of jules as ai helpers, the .jules folder includes the activities they have undertaken.

## extra
i made this out of my own interest and hobby, but primarily to try and learn (which is why i made the ai make a blueprint and not the whole thing itself)
this is not a serious project, although according to ai the algorithm is strong, don't use this for anything that's important.

---

## v3.4 features

The following features introduced in v3.4 were implemented with the assistance of **Claude Code** (model: `claude-sonnet-4-6`):

- **Key Export/Import (`.keyenc` files)** — Vault Mode (user-supplied master password, 600k PBKDF2 iterations) and Token Mode (hardcoded internal password, 200k iterations), both using AES-256-GCM encryption with full memory-wiping via `CryptographicOperations.ZeroMemory`.
- **Last Used Directory memory** — `AppSettings.cs` persists the last file-dialog directory to `%AppData%\EncSypher\settings.json` using `System.Text.Json`.
- **`changelog/` restructuring** — `.jules/` folder contents moved to `changelog/` to centralise project change logs. From v3.4 onwards all updates are logged there.
- **Version bump to 3.4** — version updated across `.csproj`, UI label, and documentation.

---

## v3.5 features

The following features introduced in v3.5 were implemented with the assistance of **Claude Code** (model: `claude-sonnet-4-6`):

- **Asynchronous File Operations** — Heavy file I/O and cryptographic operations are now performed on a background thread, preventing UI freezing during large file processing.
- **Progress Reporting** — A real-time progress bar and status label provide visual feedback during file encryption and decryption.
- **Enhanced In-Memory Key Protection** — Passphrases and master passwords are now handled as `byte[]` instead of `string` and are immediately wiped from memory using `CryptographicOperations.ZeroMemory` after use, reducing the risk of memory-scraping attacks.
- **Code Refinement** — Fixed obsolete `AesGcm` constructor usage and removed unused code.
- **Version bump to 3.5** — version updated across project files, UI, and documentation.
