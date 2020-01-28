# Time Series
This markdown file provides interactive documentation using [Try .NET](https://dotnet.microsoft.com/platform/try-dotnet). If you haven't already, follow the instructions on [README](README.md) to set up and run.

The namespace Cmdty.TimeSeries contains a set of types used to represent an associated map, where the key is always a [Time Period Value Type](https://github.com/cmdty/time-period-value-types). Where data is sorted on the key. Time series types objects are immutable, are implicitly ordered by the index time period.

These types are used extensively within the Cmdty library to represent collection of prices for traded commodities, including forward curves. However, there is nothing commodity-specific within the Time Series API, and hence these types could be used in many other non-commodity business contexts.

### Creating Instances
Instances can be created from a constructor, or a mutable Builder type, which offers an object initialiser for convenience.
```cs --region creating --source-file ./Cmdty.TimeSeries.Samples/Program.cs --project ./Cmdty.TimeSeries.Samples/Cmdty.TimeSeries.Samples.csproj
```

### Accessing Data
The contents of a Time Series can be access using two indexers, one accepting an instance of the [Time Period Value Type](https://github.com/cmdty/time-period-value-types), the other accepting a 32-bit integer. With the integer indexer the results are sorted by the time period key, i.e. passing 0 into the indexer retrieves the value associated with the earliest time period, 2 gives the value for the second earliest period etc. Hence Time Series are similar to the [SortedDictionary](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.sorteddictionary-2?view=netframework-4.8) and [SortedList](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.sortedlist-2?view=netframework-4.8), except Time Series should offer quicker retrieval when using the integer indexer.
```cs --region accessing_data --source-file ./Cmdty.TimeSeries.Samples/Program.cs --project ./Cmdty.TimeSeries.Samples/Cmdty.TimeSeries.Samples.csproj
```

### Properties
The code sample below shows the properties of TimeSeries instances.
```cs --region properties --source-file ./Cmdty.TimeSeries.Samples/Program.cs --project ./Cmdty.TimeSeries.Samples/Cmdty.TimeSeries.Samples.csproj
```

## Formating
The TimeSeries type overrides the [ToString](https://docs.microsoft.com/en-us/dotnet/api/system.object.tostring?view=netframework-4.8) method to provide a human-readable text representation. Where the second type parameter, the data type, implements [IFormattable](https://docs.microsoft.com/en-us/dotnet/api/system.iformattable?view=netframework-4.8) overloads of the ToString are provided which allow control of the formatting of the result using a format string or/and an [IFormatProvider](https://docs.microsoft.com/en-us/dotnet/api/system.iformatprovider?view=netframework-4.8). If just the first type parameter, the time period index type, implements [IFormattable](https://docs.microsoft.com/en-us/dotnet/api/system.iformattable?view=netframework-4.8), the FormatData method produces the same result as ToString, except with the format control using a format string and/or an [IFormatProvider](https://docs.microsoft.com/en-us/dotnet/api/system.iformatprovider?view=netframework-4.8) applied to the index type.

The maximum number of lines of the string produced using these methods is truncated, resulting in a reduced display for Time Series with a large number of elements. All of the formatting methods have overloads with an integer parameter specifying the maximum number of rows before data is truncated from the string representation. If a value of -1 is provide for this parameter, no data is truncated from the string, resulting in all of the elements being displayed.
```cs --region formatting --source-file ./Cmdty.TimeSeries.Samples/Program.cs --project ./Cmdty.TimeSeries.Samples/Cmdty.TimeSeries.Samples.csproj
```

## DoubleTimeSeries Type
```cs --region doubletimeseries --source-file ./Cmdty.TimeSeries.Samples/Program.cs --project ./Cmdty.TimeSeries.Samples/Cmdty.TimeSeries.Samples.csproj
```
