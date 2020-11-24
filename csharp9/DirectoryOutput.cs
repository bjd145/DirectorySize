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

        static private string ToNumberFormat(long val) => string.Format("{0:#,0}", val);
        static private string ToMB(long val) => (string.Format("{0:#,0.00}", Math.Round((double)val/MB,2)));

        static public void DisplayResults( ConcurrentDictionary<string,DirectoryStatistics> repo, long count, long size, long time, int errors) 
        {
            var resultsTable = new Table()
                .Centered()
                .Width(Console.WindowWidth)
                .Border(TableBorder.Rounded)
                .AddColumn(new TableColumn("Path").LeftAligned().Footer("Totals").NoWrap())
                .AddColumn(new TableColumn("Files").RightAligned().Footer(new Text(ToNumberFormat(count))))
                .AddColumn(new TableColumn("Size (mb)").RightAligned().Footer(new Text(ToMB(size))));
        
            foreach (var directory in repo.OrderByDescending( o => o.Value.DirectorySize))
            {
                resultsTable.AddRow( 
                    new Text(directory.Value.Path), 
                    new Text(ToNumberFormat(directory.Value.FileCount)), 
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
                new Text(ToNumberFormat(time))
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