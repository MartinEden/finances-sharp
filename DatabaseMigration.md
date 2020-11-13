# Database migration
We have no automatic way of updating the Entity Framework-managed database when
the code first schema changes. My current manual method is as follows:

## Set up Entity Framework to create a new blank test database
Update Web.config so that the connection string so that it points at a 
temporary database and change to the root user so that EF can create a new 
database with that name. Something like this:

`server=localhost;port=3306;database=tmp;uid=root;password=SERVER_ROOT_PW`

If you are not comfortable using root, create a user that has the permission 
to create new databases.

In the `FinanceDb` class uncomment the `CreateDatabaseIfNotExists` initializer 
line and comment out the `EmptyInitializer` line. Don't run the app yet.

## Set up MySQL to log what EntityFramework creates
```
mysql -u root -p 
SET GLOBAL general_log=TRUE;
SHOW VARIABLES LIKE '%general_log%';
```

## Write and run the migration
Run the app and trigger the EF initializer by browsing to any page. I then 
manually examine the general log to see how EF has created the tables/columns I
am interested in, so I can make sure my hand-crafted migration matches that.

I then write a new SQL migration file in the Migrations folder and run it 
against the real database:

```
mysql -u finances_sharp -D finances_sharp < Migrations/SOME_MIGRATION.sql
```

## Clean up
Delete the temporary database and disable general logging:

```
mysql -u root -p 
SET GLOBAL general_log=FALSE;
DROP DATABASE tmp;
```
