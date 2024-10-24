# Prerequisites

1. .Net 8 SDK (on dev machine)
2. .Net 8 hosting bundle (on target machine)

# Steps

1. `dotnet publish WebApplication1 -o publish`
2. Deploy `publish` folder to target machine IIS
3. Open a browser and navigate to the app you just published (`/swagger`)
4. Observe the issue in the logs folder
   - or alternatively observer the app loads correctly (no issue)
