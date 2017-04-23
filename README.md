# FinancesSharp
FinancesSharp is a web application for tracking your finances.

You can import statements using the Internet Banking CSV format. Then, once the statement is imported you can categorise transactions using your own categorisation system. You can also create rules to apply categorisation automatically on import, for regular spending. Finally, you can review your finances using the reports.

This is just an application I wrote for my own use. There is plenty of scope to, for example, add more reports and to handle more statement import formats. Feel free to contribute pull requests or, failing that, raise issues and I may get time to fix and extend things.

## Build & deploy
This application isn't currently set up with proper automated build/deploy or continuous integration. So if you want to use it your best best is to download the free Visual Studio Community Edition, open the solution, and use Build > Publish to get it running on your PC. You'll need to IIS installed as a server.

Alternatively, I'd be interested to hear from anyone who has it running under Mono, as then I could containerise the application and publish it on Docker Hub.