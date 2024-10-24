# Prerequisites

1. .Net 8 SDK (on dev machine)
2. .Net 8 hosting bundle (on target machine)

# Steps

1. `dotnet publish WebApplication1 -o publish`
2. Deploy `publish` folder to target machine IIS
3. Observe the issue
