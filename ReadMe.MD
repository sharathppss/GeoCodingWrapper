GeoCodingWrapper
=====================

GeoCodingWrapper is a .NET standard 2.0 wrapper library for physical address translation with support for multiple geocoding providers.

The library primarly supports retrieval of geo coordinates for any address string with a minimum acceptable accuracy level provided as input to the request. It also allow the client to set the sequence of preferred geoencoding providers which is leveraged when single response requests are processed by the libarary.

Additionally the individual geoencoding providers leveraged by the library are exposed as public classes for seperate consumption.

## Platform Support

GeoCodingWrapper is compiled as a .NET Standard 2.0 library and is available as a nuget package.

## NuGet

Import below nuget package locally for usage.

**Note:** Rename package to *.nupkg extension before use.

[Nuget local feed instructions](https://docs.microsoft.com/en-us/visualstudio/mac/nuget-walkthrough?view=vsmac-2019#adding-package-sources)

[Download Nuget Package](NugetPackage/GeoCodingWrapper.1.0.0.0.nupkg.local)

## Prerequisite
### GeoCodingWrapper Provider API Keys

The client is responsible for obtaining the API keys for individual geocoding providers initialized to the library. Refer to initialization section of AddressTranslator Class for more info.

[Google geocoding API key generation instructions](https://developers.google.com/maps/documentation/geocoding/get-api-key)

[Bing geocoding API key generation instructions](https://docs.microsoft.com/en-us/bingmaps/getting-started/bing-maps-dev-center-help/getting-a-bing-maps-key)
        
## API Documentation

### AddressTranslator Class

#### Initialization

Use the factory methods to create the required geocoding provider services.

**Note:** The order of services in geoService list is accepted as the preferred sequence for address translation. The first element being the most preferred.

```csharp

string googleKey = "<google-service-key>";
string bingKey = "<bing-service-key>";

var gcClient = new HttpClient(new GeoCoding.Utils.Http.GeoHandler(3));

var geoServiceList = new List<IGeoService>();

geoServiceList.Add(GeoServiceFactory.CreateBingService(gcClient, bingKey));
geoServiceList.Add(GeoServiceFactory.CreateGoogleService(gcClient, googleKey));


var translator = new AddressTranslator(geoServices);
```

#### Method - Task\<GeoCordinate\> GetCoordinate(string address, AccuracyLevel threshold)

Retrieve first valid coordinate from geoservice list for specified address string. Coordinate only satisfiying minimum threshold accuracy are considered.

#### Result
Valid GeoCoordinate object on success, null when no valid entries found.

#### Sample Usage
```csharp
string sampleAddress = "ABCD, Los Angeles";

var translator = new AddressTranslator(geoServices);

GeoCordinate singleResult = await translator.GetCoordinate(sampleAddress, GeoCoding.Data.AccuracyLevel.Lowest);
```

#### Method - IList<Task\<GeoCordinate\>> GetAllCoordinates(string address, AccuracyLevel threshold)

Retrieve all valid coordinates from geoservice list for specified address string. Only coordinates satisfiying minimum threshold accuracy are included.

#### Result
List of valid GeoCoordinate objects on success, empty list when no valid entries found.

#### Sample Usage
```csharp
string sampleAddress = "ABCD, Los Angeles";

var translator = new AddressTranslator(geoServices);

List<GeoCordinate> singleResult = await translator.GetAllCoordinate(sampleAddress, GeoCoding.Data.AccuracyLevel.Lowest);
```

### Data Objects
#### GeoCoordinate Response
```csharp
public class GeoCoordinate
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public AccuracyLevel Accuracy { get; set; }
    public ServiceType Service { get; set; }
};
```
#### AccuracyLevel Enum
```csharp
public enum AccuracyLevel
{
    Lowest = 0,
    Low,
    Medium,
    High
};
```

#### ServiceType Enum
```csharp
public enum ServiceType
{
    Bing = 0,
    Google = 1
}
```

### Exception Handling

The library leverage a custom GCException to catch error conditions pertaining to the library and bubble it up to the client with appropriate status code and messaging. The errors conditions not anticipated by the library is sent through to the client unhandled.

Below enum lists the status codes returned through GCException.

```csharp
public enum StatusCode
{
    Success = 0,

    BadHttpResponse, // Non success Http error code recieved from rest calls.
    ApiError, // Geoservice provider api call returned error status. 
    MissingJsonParams, // Json received from geoservice provider has parameters missing.
    
    GenericError = 999 // Other generic libarary errors.
}
```


## Build

Clone the repository and build `GeoCoding.sln` using Visual Studio. NuGet package restore must be enabled.

## Testing

- [x] Unit tests
- [ ] Integration tests
- [x] Test harness tool

## Contributors


## License