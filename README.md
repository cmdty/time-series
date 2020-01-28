Time Series
=========================
[![Build Status](https://dev.azure.com/cmdty/github/_apis/build/status/cmdty.time-series?branchName=master)](https://dev.azure.com/cmdty/github/_build/latest?definitionId=4&branchName=master)
![Azure DevOps coverage](https://img.shields.io/azure-devops/coverage/cmdty/github/5)
[![NuGet](https://img.shields.io/nuget/v/cmdty.timeseries.svg)](https://www.nuget.org/packages/Cmdty.TimeSeries/)



## Overview
The Cmdty.TimeSeries package contains a set of types used to represent an associated map, where the key is always a [Time Period Value Type](https://github.com/cmdty/time-period-value-types). Where data is sorted on the key. Time series types objects are immutable, are implicitly ordered by the index time period.

These types are used extensively within the Cmdty library to represent collection of prices for traded commodities, including forward curves. However, there is nothing commodity-specific within the Time Series API, and hence these types could be used in many other non-commodity business contexts.

## Installing
From the Visual Studio Package Manager console:
```
PM> Install-Package Cmdty.TimeSeries -Version 1.0.0-alpha1
```
Or from the .NET Core CLI:
```
dotnet add package Cmdty.TimeSeries --version 1.0.0-alpha1
```

## User Guide
### Creating Instances
Instances can be created from a constructor, or a mutable Builder type, which offers an object initialiser for convenience.
```c#
TimeSeries<Quarter, decimal> timeSeriesFromConstructor1 = new TimeSeries<Quarter,decimal>(
                                            Quarter.CreateQuarter1(2020), 
                                            new []{21.3M, 42.4M, 42.5M});
Console.WriteLine(timeSeriesFromConstructor1);
Console.WriteLine();

var timeSeriesFromConstructor2 = new TimeSeries<Day, double>(
        new []{new Day(2019, 9, 17), new Day(2019, 9, 18), new Day(2019, 9, 19)},
        new []{242.4, 224.42, 262.04});
Console.WriteLine(timeSeriesFromConstructor2);
Console.WriteLine();

var builder = new TimeSeries<Month, int>.Builder
{
    {Month.CreateJanuary(2019), 1},
    {Month.CreateFebruary(2019), 2},
    {Month.CreateMarch(2019), 3}
};

builder.Add(Month.CreateApril(2019), 4);

TimeSeries<Month, int> timeSeriesFromBuilder = builder.Build();
Console.WriteLine(timeSeriesFromBuilder);
Console.WriteLine();

TimeSeries<Day, string> emptyTimeSeries = TimeSeries<Day, String>.Empty;
Console.WriteLine(emptyTimeSeries);
```
```
Count = 3
Q1-2020  21.3
Q2-2020  42.4
Q3-2020  42.5

Count = 3
2019-09-17  242.4
2019-09-18  224.42
2019-09-19  262.04

Count = 4
2019-01  1
2019-02  2
2019-03  3
2019-04  4

Count = 0
```

### Accessing Data
The contents of a Time Series can be access using two indexers, one accepting an instance of the [Time Period Value Type](https://github.com/cmdty/time-period-value-types), the other accepting a 32-bit integer. With the integer indexer the results are sorted by the time period key, i.e. passing 0 into the indexer retrieves the value associated with the earliest time period, 2 gives the value for the second earliest period etc. Hence Time Series are similar to the [SortedDictionary](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.sorteddictionary-2?view=netframework-4.8) and [SortedList](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.sortedlist-2?view=netframework-4.8), except Time Series should offer quicker retrieval when using the integer indexer.
```c#
TimeSeries<Month, decimal> timeSeries = new TimeSeries<Month, decimal>.Builder
            {
                {Month.CreateJanuary(2019), 22.53M},
                {Month.CreateFebruary(2019), 19.42M},
                {Month.CreateMarch(2019), 18.25M}
            }.Build();

decimal itemFromTimePeriodIndexer = timeSeries[Month.CreateFebruary(2019)];
Console.WriteLine("Item from Time Period indexer: " + itemFromTimePeriodIndexer);

decimal itemFromIntegerIndexer = timeSeries[0];
Console.WriteLine("Item from int indexer: " + itemFromIntegerIndexer);

Console.WriteLine();

Console.WriteLine("Enumerating Over a TimeSeries");
foreach (TimeSeriesPoint<Month, decimal> timeSeriesPoint in timeSeries)
{
    Console.WriteLine(timeSeriesPoint);
}
```
```
Item from Time Period indexer: 19.42
Item from int indexer: 22.53

Enumerating Over a TimeSeries
Index: 2019-01, Data: 22.53
Index: 2019-02, Data: 19.42
Index: 2019-03, Data: 18.25
```

### Properties
The code sample below shows the properties of TimeSeries instances.
```c#
TimeSeries<Month, int> timeSeries = new TimeSeries<Month, int>.Builder
{
    {Month.CreateJanuary(2019), 45},
    {Month.CreateFebruary(2019), 336},
    {Month.CreateMarch(2019), 846}
}.Build();

Console.WriteLine("TimeSeries Scalar Properties");
Console.WriteLine("Count: " + timeSeries.Count);
Console.WriteLine("Start: " + timeSeries.Start);
Console.WriteLine("End: " + timeSeries.End);
Console.WriteLine("IsEmpty: " + timeSeries.IsEmpty);
Console.WriteLine();

Console.WriteLine("TimeSeries Collection Properties");
IReadOnlyList<Month> timeSeriesIndices = timeSeries.Indices;
Console.WriteLine("Contents of timeSeries.Indices:");
foreach (Month month in timeSeriesIndices)
{
    Console.WriteLine(month);
}
Console.WriteLine();

IReadOnlyList<int> timeSeriesData = timeSeries.Data;
Console.WriteLine("Contents of timeSeries.Data:");
foreach (int dataItem in timeSeriesData)
{
    Console.WriteLine(dataItem);
}
```
```
TimeSeries Scalar Properties
Count: 3
Start: 2019-01
End: 2019-03
IsEmpty: False

TimeSeries Collection Properties
Contents of timeSeries.Indices:
2019-01
2019-02
2019-03

Contents of timeSeries.Data:
45
336
846
```

## Formating
The TimeSeries type overrides the [ToString](https://docs.microsoft.com/en-us/dotnet/api/system.object.tostring?view=netframework-4.8) method to provide a human-readable text representation. Where the second type parameter, the data type, implements [IFormattable](https://docs.microsoft.com/en-us/dotnet/api/system.iformattable?view=netframework-4.8) overloads of the ToString are provided which allow control of the formatting of the result using a format string or/and an [IFormatProvider](https://docs.microsoft.com/en-us/dotnet/api/system.iformatprovider?view=netframework-4.8). If just the first type parameter, the time period index type, implements [IFormattable](https://docs.microsoft.com/en-us/dotnet/api/system.iformattable?view=netframework-4.8), the FormatData method produces the same result as ToString, except with the format control using a format string and/or an [IFormatProvider](https://docs.microsoft.com/en-us/dotnet/api/system.iformatprovider?view=netframework-4.8) applied to the index type.

The maximum number of lines of the string produced using these methods is truncated, resulting in a reduced display for Time Series with a large number of elements. All of the formatting methods have overloads with an integer parameter specifying the maximum number of rows before data is truncated from the string representation. If a value of -1 is provide for this parameter, no data is truncated from the string, resulting in all of the elements being displayed.
```c#
TimeSeries<Month, decimal> monthlyTimeSeries = new TimeSeries<Month, decimal>.Builder
{
    {Month.CreateJanuary(2019), 22.53M},
    {Month.CreateFebruary(2019), 19.42M},
    {Month.CreateMarch(2019), 18.25M}
}.Build();

string timeSeriesDefaultFormatting = monthlyTimeSeries.ToString();
Console.WriteLine("Default formatting");
Console.WriteLine(timeSeriesDefaultFormatting);
Console.WriteLine();

string timeSeriesFormattedWithFormatString = monthlyTimeSeries.FormatData("F5");
Console.WriteLine("Formatted with format string and current thread culture");
Console.WriteLine(timeSeriesFormattedWithFormatString);
Console.WriteLine();

string timeSeriesFormattedWithFormatStringAndCulture =
    monthlyTimeSeries.FormatData("F5", new CultureInfo("fr"));
Console.WriteLine("Formatted with format string and specified culture");
Console.WriteLine(timeSeriesFormattedWithFormatStringAndCulture);
Console.WriteLine();

IEnumerable<double> hourlyTimeSeriesData = Enumerable.Range(0, 16).Select(i => i * 0.06 + 25.4);
TimeSeries<Hour, double> hourlyTimeSeries =
    new TimeSeries<Hour, double>(new Hour(2019, 9, 27, 6), hourlyTimeSeriesData);

string hourlyTimeSeriesFormattedDefaultNumRows = hourlyTimeSeries.ToString("dd/MM/yy hh:mm", "F3");
Console.WriteLine("Index and data formatted with format string and truncated to default max rows");
Console.WriteLine(hourlyTimeSeriesFormattedDefaultNumRows);
Console.WriteLine();

string hourlyTimeSeriesFormattedFullNumRows = hourlyTimeSeries.ToString("dd/MM/yy hh:mm", "F3", -1);
Console.WriteLine("Index and data formatted with format string and no truncation of rows");
Console.WriteLine(hourlyTimeSeriesFormattedFullNumRows);
Console.WriteLine();

string hourlyTimeSeriesFormattedSpecificNumRows = hourlyTimeSeries.ToString("dd/MM/yy hh:mm", "F3", 5);
Console.WriteLine("Index and formatted with format string and truncation to specified number of rows");
Console.WriteLine(hourlyTimeSeriesFormattedSpecificNumRows);

```
```
Default formatting
Count = 3
2019-01  22.53
2019-02  19.42
2019-03  18.25

Formatted with format string and current thread culture
Count = 3
2019-01  22.53000
2019-02  19.42000
2019-03  18.25000

Formatted with format string and specified culture
Count = 3
2019-01  22,53000
2019-02  19,42000
2019-03  18,25000

Index and data formatted with format string and truncated to default max rows
Count = 16
27/09/19 06:00  25.400
27/09/19 07:00  25.460
27/09/19 08:00  25.520
27/09/19 09:00  25.580
......................
27/09/19 06:00  26.120
27/09/19 07:00  26.180
27/09/19 08:00  26.240
27/09/19 09:00  26.300

Index and data formatted with format string and no truncation of rows
Count = 16
27/09/19 06:00  25.400
27/09/19 07:00  25.460
27/09/19 08:00  25.520
27/09/19 09:00  25.580
27/09/19 10:00  25.640
27/09/19 11:00  25.700
27/09/19 12:00  25.760
27/09/19 01:00  25.820
27/09/19 02:00  25.880
27/09/19 03:00  25.940
27/09/19 04:00  26.000
27/09/19 05:00  26.060
27/09/19 06:00  26.120
27/09/19 07:00  26.180
27/09/19 08:00  26.240
27/09/19 09:00  26.300

Index and formatted with format string and truncation to specified number of rows
Count = 16
27/09/19 06:00  25.400
27/09/19 07:00  25.460
......................
27/09/19 09:00  26.300
```

## DoubleTimeSeries Type
The DoubleTimeSeries type is a subclass of TimeSeries which can
only hold data of of the built-in double type. This type offers
a set of arithmetic operators which operate on the numeric data held, as shown in the examples below.
```c#
DoubleTimeSeries<Month> doubleTimeSeries = new DoubleTimeSeries<Month>.Builder()
        {
            {new Month(2020, 1), 45.67 },
            {new Month(2020, 2), 47.01 },
            {new Month(2020, 3), 50.34 },
        }.Build();

Console.WriteLine("DoubleTimeSeries");
Console.WriteLine(doubleTimeSeries.FormatData("F2"));
Console.WriteLine();

DoubleTimeSeries<Month> doubleTimeSeriesAfterMultiply = doubleTimeSeries * 1.3;
Console.WriteLine("DoubleTimeSeries transformed with multiply operator and double");
Console.WriteLine(doubleTimeSeriesAfterMultiply.FormatData("F2"));
Console.WriteLine();

DoubleTimeSeries<Month> doubleTimeSeriesAfterAdd = doubleTimeSeries + 5.5;
Console.WriteLine("DoubleTimeSeries transformed with add operator and double");
Console.WriteLine(doubleTimeSeriesAfterAdd.FormatData("F2"));
Console.WriteLine();

DoubleTimeSeries<Month> otherDoubleTimeSeries = new DoubleTimeSeries<Month>.Builder()
        {
            {new Month(2020, 1), 1.3 },
            {new Month(2020, 2), 1.4 },
            {new Month(2020, 3), 1.35 },
        }.Build();

DoubleTimeSeries<Month> timeSeriesSubtracted = doubleTimeSeries - otherDoubleTimeSeries;
Console.WriteLine("DoubleTimeSeries transformed with minus operator and other DoubleTimeSeries");
Console.WriteLine(timeSeriesSubtracted.FormatData("F2"));
Console.WriteLine();

DoubleTimeSeries<Month> timeSeriesDivided = doubleTimeSeries / otherDoubleTimeSeries;
Console.WriteLine("DoubleTimeSeries transformed with divide operator and other DoubleTimeSeries");
Console.WriteLine(timeSeriesDivided.FormatData("F2"));
```
```
DoubleTimeSeries
Count = 3
2020-01  45.67
2020-02  47.01
2020-03  50.34

DoubleTimeSeries transformed with multiply operator and double
Count = 3
2020-01  59.37
2020-02  61.11
2020-03  65.44

DoubleTimeSeries transformed with add operator and double
Count = 3
2020-01  51.17
2020-02  52.51
2020-03  55.84

DoubleTimeSeries transformed with minus operator and other DoubleTimeSeries
Count = 3
2020-01  44.37
2020-02  45.61
2020-03  48.99

DoubleTimeSeries transformed with divide operator and other DoubleTimeSeries
Count = 3
2020-01  35.13
2020-02  33.58
2020-03  37.29
```

## Interactive Documentation
An interactive version of the above [user guide](#user-guide) has been provided using 
[Try .NET](https://dotnet.microsoft.com/platform/try-dotnet) 
a tool which allows C# code to be run in a browser using Markdown files. See [/samples/README.md](/samples/README.md) for details.

## Building
This section describes how to run a scripted build on a cloned repo, using the [Cake](https://github.com/cake-build/cake) build tool.

#### Build Prerequisites
The following are required on the host machine in order for the build to run.
* The .NET Core SDK. Check the [global.json file](global.json) for the version necessary, taking into account [the matching rules used](https://docs.microsoft.com/en-us/dotnet/core/tools/global-json#matching-rules).

#### Running the Build on Windows
The build is started by running the PowerShell script build.ps1 from a PowerShell console, ISE, or the Visual Studio Package Manager Console.

```
PM> .\build.ps1
```

#### Running the Build on Linux or macOS
Run the build by invoking the [build.sh](build.sh) script.

```
> chmod +x ./build.sh
> ./build.sh
```

Alternatively, if [PowerShell Core](https://github.com/PowerShell/PowerShell) is installed,
the build can be run with the following command:
```
> pwsh ./build.ps1
```
#### Build Artifacts
The result of the build is the NuGet package Cmdty.TimeSeries.[version].nupkg, 
which is saved into the artifacts directory (which itelf will be created in the top 
directory of the repo).

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details