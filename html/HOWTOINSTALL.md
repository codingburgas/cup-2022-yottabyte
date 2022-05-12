# How to install
## Build the WebApp from source
0. Make sure you have Visual Studio 2019 installed with ASP.NET and web development workload.
1. Clone the repository.
    -   Open your favourite terminal and navigate to the directory where you want to clone the repository.
    -   Type: `git clone https://github.com/codingburgas/cup-2022-yottabyte.git`
2. Open the `cup-2022-yottabyte\webapp` directory.
3. To lunch the VS Solution click on `Yottabyte.sln`.
4. Before running the project please make sure that you have created a `appsettings.json` file in the  `cup-2022-yottabyte\webapp\Yottabyte\Server\`. You can see `appsettings.example.json` for example.
  - `JWT:Key` - the secret key for the JWT Token generation
  - `JWT:Issuer` - the issuer of the JWT Token (most probably your domain)
  - `JWT:Audience` - the issuer of the JWT Token (most probably your domain)
  - `AzureCustomVision:PredictionKey` - the prediction key for the Azure Custom Vision
  - `AzureCustomVision:PredictionEndpoint` - the prediction endpoint for the Azure Custom Vision
  - `AzureCustomVision:PredictionModelPublishedName` - the prediction model name for the Azure Custom Vision
  - `AzureCustomVision:PredictionModeId` - the prediction model id for the Azure Custom Vision
  - `AzureMaps:Key` - the key for the Azure Maps API

## Use the release to download our mobile app
### Android
1. Download the release from [GitHub](https://github.com/SSIvanov19/lathraea-rhodopaea/releases/download/v1.0.0/Release.zip) or from the [download page.](DOWNLOAD.md)
2. Install the application by opening `yottabyte.apk` file.
### IOS
1. Download the release from [GitHub](https://github.com/SSIvanov19/lathraea-rhodopaea/releases/download/v1.0.0/Release.zip) or from the [download page.](DOWNLOAD.md)
2. Install the application by opening `yottabyte.ipa` file.