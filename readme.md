#schedulesDirectDotnetCore
##Description
This is a .NET Core console application that pulls data from [Schedules Direct](http://schedulesdirect.org) and stores it in a SQLite database using Entity Framework. This database can then be queried to find out info such as all Science Fiction shows that will be airing, all shows on a specific channel, etc. This application was written as a way to learn .NET Core, and is the first C# code I have written in 10 years. I make no promises about the quality of this code since I have almost no .NET experience but hope someone may find this useful.
##Running
After compiling the program copy 
src/schedulesDirect/config.ini.example filesrc/schedulesDirect/bin/Debug/netcoreapp1to
src/schedulesDirect/bin/Debug/netcoreapp1.1/config.ini
Edit the file adding your username, password, country, and zipcode. Currently this has only been tested with USA as the country. Run the program and if you do not already have a lineup added to your account you will be prompted to add one.
Note I have not tested this with multiple lineups on a single account so do not know how this program will work with an account that has multiple lineups.
 After about 10 minutes and the use of 2gb of RAM you should have a SQLite database in
src/schedulesDirect/bin/Debug/netcoreapp1.1/schedulesdirect.db
Note every time this program is run the SQLite database is deleted and created from scratch. As part of my learning process I discovered there are major limitations to migrations with Entity Framework and SQLite. Since I'm not interested in historical data abut what aired in the passed it was much simpler to just not worry about migrations.