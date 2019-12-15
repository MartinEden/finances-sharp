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
2. `./build_and_run.sh` (This runs the app using XSP, which is not suitable 
   for production environments)
3. Browse to http://localhost:9000/ (XSP's default)

## Setup a blank database
1. Create a blank database server and enable password login for root:
   ```
   sudo apt install mysql-server
   sudo apt install mysql-client-core-5.7  
   sudo mysql -u root
   ALTER USER 'root'@'localhost' IDENTIFIED WITH mysql_native_password BY '<ROOT_PASSWORD>';
   quit
   ```
1. Make these two changes to the app and run it to create the blank database:
   - Change the connection string to: 
     ```
     connectionString="server=localhost;port=3306;database=finances_sharp;uid=root;pwd=ROOT_PASSWORD"
     ```
   - In FinanceDb's constructor, uncomment the `CreateDatabaseIfNotExists` line, and comment
     out the `EmptyInitializer` line.
   - Run using `build_and_run.sh`   
1. Return the app to normal settings:
   - Update the connection string to a limited user:
     ```
     connectionString="server=localhost;port=3306;database=finances_sharp;uid=finances_sharp"
     ```
   - Restore the `EmptyInitializer` line and comment out the `CreateDatabaseIfNotExists` line
1. Setup the limited user in the server:
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
