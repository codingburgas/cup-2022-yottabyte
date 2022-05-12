# Troubleshooting
## Errors
### ArgumentNullException: String reference not set to an instance of a String. (Parameter 's')
You most probably forgot to add `appsettings.json` file in the  `cup-2022-yottabyte\webapp\Yottabyte\Server\`. You can see `appsettings.example.json` for example.
  - `JWT:Key` - the secret key for the JWT Token generation
  - `JWT:Issuer` - the issuer of the JWT Token (most probably your domain)
  - `JWT:Audience` - the issuer of the JWT Token (most probably your domain)
  - `AzureCustomVision:PredictionKey` - the prediction key for the Azure Custom Vision
  - `AzureCustomVision:PredictionEndpoint` - the prediction endpoint for the Azure Custom Vision
  - `AzureCustomVision:PredictionModelPublishedName` - the prediction model name for the Azure Custom Vision
  - `AzureCustomVision:PredictionModeId` - the prediction model id for the Azure Custom Vision
  - `AzureMaps:Key` - the key for the Azure Maps API

### Doxygen is not recognized as an internal or external command
We use Doxygen to document our code. This is why you need to download it and add it to the PATH. 
<br>
[Here](https://www.doxygen.nl/download.html) you can find link to the official download page of Graphviz.
<br>
And [here](https://docs.oracle.com/en/database/oracle/machine-learning/oml4r/1.5.1/oread/creating-and-modifying-environment-variables-on-windows.html#GUID-DD6F9982-60D5-48F6-8270-A27EC53807D0) is a guide on how to add the binaries to your PATH environment variable.

### Doxygen cannot find graphviz
We use Graphviz to create diagrams for our code. This is why you need to download it and add it to the PATH. 
<br>
[Here](http://www.graphviz.org/download/) you can find link to the official download page of Graphviz.
<br>
And [here](https://docs.oracle.com/en/database/oracle/machine-learning/oml4r/1.5.1/oread/creating-and-modifying-environment-variables-on-windows.html#GUID-DD6F9982-60D5-48F6-8270-A27EC53807D0) is a guide on how to add the binaries to your PATH environment variable.
