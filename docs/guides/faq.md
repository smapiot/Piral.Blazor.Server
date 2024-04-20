# Frequently Asked Questions

1. I cannot use breakpoints when I debug a Piral.Blazor pilet in VS. What could be wrong?

Make sure you actually emit a PDB and have `Debug` selected as configuration. Also, don't change the configuration to have `<DebugType>Full</DebugType>` or similar in the project file. You'll need a portable PDB (modern format), not a full PDB (legacy format for Windows).
