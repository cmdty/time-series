#region License
// Copyright (c) 2019 Jake Fowler
//
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without 
// restriction, including without limitation the rights to use, 
// copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the 
// Software is furnished to do so, subject to the following 
// conditions:
//
// The above copyright notice and this permission notice shall be 
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Cmdty.TimePeriodValueTypes;
using Cmdty.TimeSeries;

namespace Cmdty.Curves.Samples.TimeSeries
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Creating();
                Console.WriteLine();
                Properties();
                Console.WriteLine();
                AccessingData();
                Console.WriteLine();
                Formatting();
                Console.WriteLine();
                DoubleTimeSeries();
                Console.ReadKey();
            }
            else
            {
                switch (args[1])
                {
                    case "creating":
                        Creating();
                        break;
                    case "properties":
                        Properties();
                        break;
                    case "accessing_data":
                        AccessingData();
                        break;
                    case "formatting":
                        Formatting();
                        break;
                    case "doubletimeseries":
                        DoubleTimeSeries();
                        break;
                }
            }

        }

        private static void Creating()
        {
            #region creating
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

            #endregion
        }

        private static void Properties()
        {
            #region properties
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
            
            #endregion
        }

        private static void AccessingData()
        {
            #region accessing_data
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

            #endregion
        }

        private static void Formatting()
        {
            #region formatting
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
            Console.WriteLine();

            #endregion
        }

        private static void DoubleTimeSeries()
        {
            #region doubletimeseries
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
            Console.WriteLine();

            #endregion
        }

    }
}
