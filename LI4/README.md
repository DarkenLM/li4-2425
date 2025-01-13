# Structure

## LI4
Contains the Blazor Server implementation. Also contains static pages, the layouts and the assets (JS scripts, css, etc), which are imported in App.razor.

## LI4.Client
Contains the Blazor Client implementation, using (mainly) the InteractiveWebAssembly render mode. In order to access the JS Interop, the InteractiveServer render mode needs to be used.

## LI4.Common
Contains common files for both the Server and the Client, such as data objects.

**NOTE:** When the facades are created, move the DAOs into LI4 instead. They currently sit here in order to access them in the Client.