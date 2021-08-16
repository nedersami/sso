# SSO

SSO is the standard library for implementing access control simply and quickly in a new .NET Core application. It serves as a starting point and can be customized to create specific fields and screens aimed at controlling access to an application

## Versions

There are two versions that can be used:

- SSO.UI

Library for using access control in an ASP.NET Core MVC application

- SSO.API

Library for using access control in an ASP.NET Core Web API application

## Installation

This project was created to be turned into a nuget package and published to an internal repository.

## Settings

1. Configure Startup.cs
   - In the ConfigureServices method, add the following snippet and configure it according to your needs:

```
services
    .AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
          
services
    .AddControllersWithViews()
    .AddRazorRuntimeCompilation()
    .AddSSORoutes();

services
    .Configure<SettingConfigurationModel>(Configuration.GetSection("SettingConfiguration"))
    .AddScoped<SSODbContext, ApplicationDbContext>()
    .AddSSOUI(Configuration, options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequiredLength = 8;
        options.Password.RequiredUniqueChars = 1;

        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;

        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = true;
    });
```

   - In the Configure method, add the following call:
```
app.UseSSOUI();
```

2. Configure the DbContext
   - Create static DbContextOptionsHelper class

```
  public static class DbContextOptionsHelper
{
public static DbContextOptions<T> ChangeOptionsType<T>(DbContextOptions options) where T : DbContext
{
var sqlExt = options.Extensions.FirstOrDefault(e => e is SqlServerOptionsExtension);
return new DbContextOptionsBuilder<T>()
.UseSqlServer(((SqlServerOptionsExtension)sqlExt).ConnectionString)
.Options;
}
}
```

   - Inherit from SSODbContext, using the class created above in the constructor as Options:

```
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(DbContextOptionsHelper.ChangeOptionsType<SSODbContext>(options))
        {

        }
```


3. Configure appSettings.json

```
"SettingConfiguration": {
    "SecretKey": "esikgt4578ah3oag4r89w37hgow39",
    "UrlDefaultApp": "https://localhost:44389/",
    "ApplicationName": "YourClientApp",
    "SMTP_Host": "smtp.gmail.com",
    "SMTP_Port": "587",
    "SMTP_User": "no-reply@yourclient.com",
    "SMTP_Pass": "test",
    "SMTP_From": "no-reply@yourclient.com",
    "SMTP_From_Alias": "Your Client"
  }
````

4. Run migrations

```
dotnet ef migrations add SSO
dotnet and database update
```

5. Add [Authorize] Attribute to controllers / actions that will be secured by access control.

## Visual Customization

The tool uses SB Adimin 2 as a default template (https://blackrockdigital.github.io/startbootstrap-sb-admin-2/). The login screens, forgotten password, user list and access profiles will use this template as default. If the site uses this template as well, it is not necessary to perform any customization and the application will behave as expected. It is still possible to customize the look.

1. Existing Screens
   - Customize the Logo

   The logo to be displayed is in the path Images/logo.png inside the wwwwroot folder. To change the image, just generate a logo in this format and drop it into the specific folder.

   - Customize the CSS

   It is also possible to use CSS classes to change other visual aspects. For example, to change the background of the login page, just change the behavior of the .bg-login-image class. Use the CSS classes of the SB Admin 2 template as a reference

2. New screens

It is possible to create new screens and completely change their behavior. Just copy the Views from our library into your project's Shared folder and customize as needed.


graph LR<br/>
  db1[(DB 1)] & dbn[(DB n)]<br/>
  db1 & dbn ----> microservice(redshift convertion)<br/>
  microservice <--> redshift[(redshift)]
