# Plastic-Notify-Center
[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)

This is a simple application trying to ease the pain of setting up notifications for events from your Plastic SCM source control server.

## Supported Notifiers
- [x] SMTP Mail
- [ ] add your own

The app is prepared to handle multiple types of notifiers. Please fork this project and help to implement more of them!

## Main Features
- Manage users & groups (with automatic LDAP import)
- Manage notifiers (get notifications anyway you like)
- Manage notification rules
    - Setup notifications for different triggers from Plastic SCM
    - Use filters to send only important notifications
    - Use variables supplied by Plastic SCM triggers
    - Choose different notifiers for your messages
    - Choose users & groups as recipients
- Send notifications automatically when a Plastic SCM trigger is fired

## Requirements

### Building
- dotnet Core 3.1.1 SDK
- dotnet EF tools

### Hosting
- dotnet Core 3.1.1 Runtime

# Build it yourself
Feel free to clone and build this project yourself

## Update Database
The database is not included in the repo. Therefore create it via the dotnet Core EF tools:
```
dotnet-ef database update
```

## Publish
You can publish the project with this:
```
dotnet publish -c Release
```
By default the output is /bin/Release/publish.

# Hosting
Copy the published application to your hosting machine and set it up.
You can choose from different hosting options for dotnet Core applications:
[You should setup a process manager](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/?view=aspnetcore-3.1#set-up-a-process-manager)

## Port configuration
To change the hosting port create a file *appsettings.Production.json*:
```
{
    "Kestrel": {
        "Endpoints": {
            "Http": {
                "Url": "http://localhost:8080"
            }
        }
    },
    "AllowedHosts": "*"
}
```

# Support this project
So my wife always says:
```
Stop this Open-Source nonsence. It's not worth it!
```
Help me prove her wrong ;] (So I'm allowed to continue this project)
- [Donate via PayPal](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=SECLTUNN2B776&source=url)
