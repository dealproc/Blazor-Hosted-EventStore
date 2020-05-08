# Blazor Desktop Application using EventStore. #

## Why do we need this? ##

This came about due to several conversations I had where folks were talking about desktop systems.  With blazor being the new bright and shiny tool for cross-platform development, it seems like a great fit for UI development where there are no differences between platforms.  Blazor is also compiled, vs. using JavaScript, so "technically" it would seem as-if the UI would be much faster.  Blazor applications are typically hosted on various platforms such as GitHub, IIS, Apache, etc.  Any way where you can navigate to a site with your web browser and access a Url.

Electron is a hosting model for JavaScript applications, using Node.js and Chromium.  Slack, Azure Data Studio, and even Visual Studio Code are now being developed using Electron to host JavaScript.  For each vendor, this allows them to build a cross-platform application with rather minimal per-platform development for the application developer.

Going from this, it seems as-if using these two projects in-tandem with each other is only a natural progression.

## Data Access ##

Business networks (both internal and external) are utilizing more bandwith to provide value to users for making critical decisions.  If we backbone the entire development process using an event-based architecture, we can then share _the changes_ the data with other instances of running applications, creating a mesh network vs. a client/server network.  If you have a workstation that suddenly fails, you have data backups on the N-1 workstations.  (Please, please, please... backup your data... This is being proposed solely to avoid downtime.)  To provide this, you will find within the `host` application an implementation of EventStore Embedded.  This allows the ability to store the structural events, and EventStore also provides the synchronization ability with minimal lifting by the application developer.

For providing the data for the user interfaces, we can use libraries such as [SqlDatabase](http://sqldatabase.net/), or even SQLite, to create pageable/readable models of the data, and then use a NoSQL store (still looking) for rendering the details view that typically is created by joining 15 different tables within a RBDMS.  This allows the application to perform faster with less memory.

## Uses ##

* Kiosk applications
* Enterprise applications (esp. those that require a sandbox environment)
* [TBD]

## To Execute the Application ##

You'll need to install a few prerequisites:

* .NET Core v3.1 [revision 3.1.201](https://dotnet.microsoft.com/download/dotnet-core/3.1)
* The Electron.NET Command Line Interface (command: `dotnet tool install ElectronNET.CLI -g`)

This project was entirely built using Visual Studio Code on Linux, so the question of is this really cross-platform should subsequently be answered.

Let me know what you think.  For now, this is just a 24 hour spike to see if it was possible.