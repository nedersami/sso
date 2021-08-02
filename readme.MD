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

If you prefer to use the nuget.config file, here is an example:

```
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="FornaxLibraries" value="https://pkgs.dev.azure.com/fornax/Fornax.Libraries/_packaging/Fornax.Libraries/nuget/v3/index.json" />
  </packageSources>
  <packageSourceCredentials>
    <FornaxLibraries>
      <add key="Username" value="YOUR FORNAX TFS LOGIN" />
      <add key="ClearTextPassword" value="YOUR ACCESS TOKEN" />
    </FornaxLibraries>
  </packageSourceCredentials>
</configuration>
```

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
    "SecretKey": "weo9g7ghw9hg39fh3o948hfw4egfdjh",
    "UrlDefaultApp": "https://localhost:44389/",
    "ApplicationName": "Contech",
    "SMTP_Host": "smtp.gmail.com",
    "SMTP_Port": "587",
    "SMTP_User": "sigporto@fornax.com.br",
    "SMTP_Pass": "sigport",
    "SMTP_From": "fxclinic@fornax.com.br",
    "SMTP_From_Alias": "FX Clinic"
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