# Backgammon

Online game player vs player.

[Codeproject article](https://www.codeproject.com/Articles/5297405/Online-Backgammon)

## These might be the steps to set it up locally

(But dont try this unless you know Angular, .net core and Entity Framework. It might be to complicated, just saying)

- Install Visual Studio 2019
- Get a connection string for a sql server database (use your own).
- Edit the connection string in the source.
- Remove MTT typescript generation in Backgammon.csproj. (Add it back after first build)
- Compile backend
- Create a pw.txt file locally for your database.
- Run `Update-Databas`
- Run Backend Api from Visual Studio.
- Install npm and yarn.
- cd to ui folder.
- Run `yarn install`
- Run `yarn start`
- Open http://localhost:4200 in the browser.
- Open a second browser for the other player.
- Have fun.
