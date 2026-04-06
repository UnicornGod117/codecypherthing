## 2024-03-24 - Visual Feedback for Copy Actions
**Learning:** Users lack confidence when copying data without visual feedback. Temporarily changing button text (e.g., 'Copied!') is a simple, effective pattern for clipboard actions.
**Action:** Always provide immediate visual confirmation when implementing clipboard interactions.

## 2026-04-06 - [Asynchronous Progress Feedback]
**Learning:** Performing long-running file operations synchronously on the main thread causes the UI to become unresponsive, leading to a poor user experience. Providing real-time progress feedback through a progress bar and status label significantly improves perceived performance and user confidence.
**Action:** Always offload heavy file I/O and cryptographic tasks to background threads (e.g., using Task.Run) and use IProgress<T> to safely update the UI from background operations. Disable UI controls during the operation to prevent re-entrancy and verify !IsDisposed before interacting with UI components after an await.

## 2026-04-06 - [Logical UI Grouping and Clearance]
**Learning:** Cluttered UI with overlapping controls (especially error labels) leads to poor accessibility and user confusion. Organizing tools into logical columns and ensuring enough vertical clearance for dynamic labels (like error text) improves visual hierarchy.
**Action:** Group related controls (e.g., Key Generation vs File Tools) into distinct layout columns and provide at least 30-40 pixels of vertical clearance between input rows to accommodate dynamic error labels without overlapping subsequent controls.
