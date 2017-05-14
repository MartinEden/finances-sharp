# FinancesSharp
FinancesSharp is a web application for tracking your finances.

You can import statements using the Internet Banking CSV format. Then, once the statement is imported you can categorise transactions using your own categorisation system. You can also create rules to apply categorisation automatically on import, for regular spending. Finally, you can review your finances using the reports.

This is just an application I wrote for my own use. There is plenty of scope to, for example, add more reports and to handle more statement import formats. Feel free to contribute pull requests or, failing that, raise issues and I may get time to fix and extend things.

# Linux build instructions
1. If you don't have mono already: `sudo apt-get install mono-complete`
2. `xbuild FinancesSharp.sln`
3. `cd FinancesSharp && xsp` to run an XSP server (usual disclaimers regarding XSP not being suitable for real production deployment)

# Windows build instructions
Use Visual Studio (Community Edition is free) to build. Then use the Build -> Publish option to get it running in IIS.
