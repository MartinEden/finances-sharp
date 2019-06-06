# FinancesSharp
FinancesSharp is a web application for tracking your finances.

You can import statements using the Internet Banking CSV format. Then, once the 
statement is imported you can categorise transactions using your own 
categorisation system. You can also create rules to apply categorisation 
automatically on import, for regular spending. Finally, you can review your 
finances using the reports.

This is just an application I wrote for my own use. There is plenty of scope to,
for example, add more reports and to handle more statement import formats. Feel 
free to contribute pull requests or, failing that, raise issues and I may get 
time to fix and extend things.

# Linux build instructions
1. If you don't have mono already: `sudo apt-get install mono-complete`
2. `xbuild FinancesSharp.sln`
3. `cd FinancesSharp && xsp` to run an XSP server (usual disclaimers regarding 
    XSP not being suitable for real production deployment)

## Setup a MySQL instance
1. Create a blank database server:
   ```
   sudo apt install mysql-server
   sudo apt install mysql-client-core-5.7   
   ```
1. Create an empty database:
   ```
   mysql -u root -p
   create database finances_sharp;
   quit
   ```
1. Run the app with connection string
   ```
   connectionString="server=localhost;port=3306;database=finances_sharp;uid=root;pwd=ROOT_PASSWORD"
   ```
   Entity Framework will create a blank database using Code First.
1. Update the connection string to a limited user:
   ```
   connectionString="server=localhost;port=3306;database=finances_sharp;uid=finances_sharp"
   ```
1. Setup that user in the server:
   ```
   mysql -u root -p
   create user finances_sharp;
   grant all privileges on finances_sharp.* to finances_sharp;
   flush privileges;
   ```

## Restore backup (optional)
    mysql -u finances_sharp finances_sharp < path_to_backup.sql

# Windows build instructions
Use Visual Studio (Community Edition is free) to build. Then use the Build -> 
Publish option to get it running in IIS.
