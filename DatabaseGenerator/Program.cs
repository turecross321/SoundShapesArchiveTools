using System.Diagnostics;
using CommandLine;
using DatabaseGenerator.Common;
using DatabaseGenerator.FromPages;
using DatabaseGenerator.FromPages.Database;
using Microsoft.EntityFrameworkCore;
using NotEnoughLogs;

namespace DatabaseGenerator;

class Program
{
    public class Options
    {
        [Option('c', "clear", Required = false, HelpText = "If the old database should be cleared")]
        public bool ClearDatabase { get; set; }
        
        [Option('p', "pages", Required = false, HelpText = "Path to import html files from.")]
        public string? ImportFromPages { get; set; }
        
        [Option('s', "saves", Required = false, HelpText = "Path to import saves from.")]
        public string? ImportFromSaves { get; set; }
    }
    
    static void Main(string[] args)
    {
        Logger logger = new();
        
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(o =>
            {
                if (o.ImportFromPages != null)
                {
                    using PageDatabaseContext database = new();
                    
                    if (o.ClearDatabase)
                        database.Database.EnsureDeleted();
                    database.Database.EnsureCreated();
                    database.Database.Migrate();
                    
                    Stopwatch stopWatch = new Stopwatch();
                    PageImporter importer = new();
                    
                    stopWatch.Start();
                    importer.Import(logger, o.ImportFromPages);
                    stopWatch.Stop();
                    logger.LogInfo(LogContext.PageImport, $"Finished importing pages in {stopWatch.Elapsed}");

                    logger.LogInfo(LogContext.PageImport, "Adding to database...");
                    stopWatch.Restart();
                    database.AddPageImport(logger, importer);
                    stopWatch.Stop();
                    logger.LogInfo(LogContext.PageImport, $"Finished adding to database in {stopWatch.Elapsed}");
                }
            });
    }
}