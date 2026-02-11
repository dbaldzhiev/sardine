# Sardine - Installation & Build Instructions

## Prerequisites

- **Rhino 8** (Required for .NET 7.0 support)
- **.NET 7.0 SDK** (To build the project)

## Building the Project

1. Open a terminal in the `src` directory.
2. Run the following command to build the solution:
   ```bash
   dotnet build Sardine.slnx
   ```
3. Verify the build succeeded.

## Installation

The build process generates a `.dll` file, which Grasshopper can load, but convention often uses the `.gha` extension.

1. **Close Rhino** completely.
2. Navigate to the build output folder:
   ```
   src\Sardine.Grasshopper\bin\Debug\net7.0\
   ```
3. Locate the following files:
   - `Sardine.Grasshopper.gha`
   - `Sardine.Core.dll`
   
4. Open the **Grasshopper Libraries** folder. You can find this at:
   ```
   %AppData%\Grasshopper\Libraries\
   ```
   *(Usually `C:\Users\<YourUser>\AppData\Roaming\Grasshopper\Libraries\`)*

5. **Copy** `Sardine.Grasshopper.gha` and `Sardine.Core.dll` into the Libraries folder.

6. (Optional) **Unblock Files**: If you downloaded the files, right-click -> Properties -> **Unblock**.

7. **Start Rhino** and run the `Grasshopper` command.
8. Check if the **Sardine** tab appears in the Grasshopper toolbar.

## Troubleshooting

- **"Plugin not found"**: Ensure both `Sardine.Grasshopper.dll` (or `.gha`) AND `Sardine.Core.dll` are in the Libraries folder. The plugin depends on the Core library.
- **COFF Loading Errors**: If you see errors about "COFF" or loading assemblies, try disabling "Memory Load" for the plugin (Grasshopper Developer Settings -> `GrasshopperDeveloperSettings` command in Rhino -> Uncheck "Memory Load *.GHA assemblies using COFF byte arrays").
- **Blocked Files**: If you downloaded the files or unzip them, right-click the `.dll` / `.gha` files -> Properties -> **Unblock** (checkbox at the bottom).
