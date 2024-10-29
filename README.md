## After downloading the project, ensure you have installed these `NuGet Package`
You can download them in `Visual Studio` by: `Tools` -> `NuGet Package Manager` -> `Manage NuGet Packages for Solution`
>Microsoft.EntityFrameworkCore

>Microsoft.EntityFrameworkCore.SqlServer

>Microsoft.EntityFrameworkCore.Tools

## Connect to your `SqlServer`
`Copy` and `Paste` your `connection string` to `appsettings.json`:
![image](https://github.com/user-attachments/assets/2dfff1ab-e02d-4300-b1fc-6d1e71f77cfe)

## Setup Database
Simply paste this command to your `Package Manager Console`: `dotnet ef database update`. It's will automatically build the database base on the project to `SqlServer` for you.

## Enjoy the project ⭐ ⭐
