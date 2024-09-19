# Frequently Asked Questions

### I cannot use breakpoints when I debug a Piral.Blazor pilet in VS. What could be wrong?

Make sure you actually emit a PDB and have `Debug` selected as configuration. Also, don't change the configuration to have `<DebugType>Full</DebugType>` or similar in the project file. You'll need a portable PDB (modern format), not a full PDB (legacy format for Windows).

### I think I need more help. Is there support available?

Yes - just get in touch with us and we can figure out a way to support your development efforts (once, more often, or even continuously).

One way to reach us is to join the [Discord server](https://discord.gg/kKJ2FZmK8t) and explain your problem there.
