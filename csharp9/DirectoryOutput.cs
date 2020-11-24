using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Spectre.Console;

namespace DirectorySize
{
    static class DirectoryOutput
    {
        const int MB = 1048576;
        const int MAXCHAR = 45;

        static private string Truncate(string value, int maxChars) => value.Length <= maxChars ? value : "..." + value.Substring((value.Length-maxChars), maxChars);
        static private string ToMB(long size) => (Math.Round((double)size/MB,2)).ToString();

        static public void DisplayResults( ConcurrentDictionary<string,DirectoryStatistics> repo, long count, long size, long time, int errors) 
        {
            var resultsTable = new Table()
                .Centered()
                .Width(Console.WindowWidth)
                .Border(TableBorder.Rounded)
                .AddColumn(new TableColumn("Path").LeftAligned().Footer("Totals").NoWrap())
                .AddColumn(new TableColumn("Files").RightAligned().Footer(new Text(count.ToString())))
                .AddColumn(new TableColumn("Size (mb)").RightAligned().Footer(new Text(ToMB(size).ToString())));
        
            foreach (var directory in repo.OrderByDescending( o => o.Value.DirectorySize))
            {
                resultsTable.AddRow( 
                    new Text(directory.Value.Path), 
                    new Text(directory.Value.FileCount.ToString()), 
                    new Text(ToMB(directory.Value.DirectorySize))
                );
            }
            AnsiConsole.Render(resultsTable);

            var statsTable = new Table()
                .LeftAligned()
                .Border(TableBorder.Rounded)
                .AddColumn(new TableColumn("Stats").LeftAligned())
                .AddColumn(new TableColumn("Data").RightAligned());
            
            statsTable.AddRow(
                new Text("Time Taken (ms)"),
                new Text(time.ToString())
            );

            statsTable.AddRow(
                new Text("Errors"),
                new Text(errors.ToString())
            );
            AnsiConsole.Render(statsTable);
        }

        static public void DisplayErrors( List<DirectoryErrorInfo> errors) 
        {               
            var errorTable = new Table()
                .Centered()
                .Width(Console.WindowWidth)
                .Border(TableBorder.Rounded)
                .AddColumn(new TableColumn("Path").LeftAligned().NoWrap())
                .AddColumn(new TableColumn("Error Details").LeftAligned());

            foreach (var error in errors)
            {
                errorTable.AddRow( 
                    new Text(error.Path), 
                    new Text(error.ErrorDescription) 
                );
            }
            AnsiConsole.Render(errorTable);
        }
    }
}