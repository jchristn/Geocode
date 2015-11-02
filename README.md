# Simple Google Geocoding Implementation

Uses WebRequest to convert an address to coordinates or coordinates to an address.

## Usage

Open the solution in Visual Studio and click 'Start'.  If you copy the static methods into your own project be sure to add a reference to System.Web (i.e. right click References - a 'using' statement is not enough).

Address to Coordinates:
```
double latitude;
double longitude;
if (address_to_coordinates("1842 N Shoreline Mountain View CA", out latitude, out longitude))
{
	Console.WriteLine(latitude, longitude);
}
else
{
	Console.WriteLine("Failed");
}
```

Coordinates to Address:
```
string address = coordinates_to_address("37.4198583", "-122.078827");
```
