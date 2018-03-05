using System.IO;

namespace Samus.FileManipulation
{
    public static class Listeners
    {
        public static FileSystemWatcher BotWatcher = new FileSystemWatcher();
        public static FileSystemWatcher PlayAreaEvenWatcher = new FileSystemWatcher();
        public static FileSystemWatcher PlayAreaOddWatcher = new FileSystemWatcher();

        public static bool BotFileChanged;
        public static bool PlayAreaEvenFileChanged;
        public static bool PlayAreaOddFileChanged;

        public static bool SummaryFileChanged;

        public static void StartSummaryFileWatcher(string path)
        {
            BotWatcher.Path = path;
            BotWatcher.NotifyFilter = NotifyFilters.LastWrite;
            BotWatcher.Filter = "handSummary*";

            BotWatcher.Changed += new FileSystemEventHandler(SummaryFileChange);
            // Begin watching.
            BotWatcher.EnableRaisingEvents = true;
        }

        public static void StartCasinoWatcher(string path)
        {
            BotWatcher.Path = path;
            BotWatcher.NotifyFilter = NotifyFilters.LastWrite;
            BotWatcher.Filter = "*casinoToBot1*";

            BotWatcher.Changed += new FileSystemEventHandler(BotFileChange);
            // Begin watching.
            BotWatcher.EnableRaisingEvents = true;
        }

        public static void SummaryFileChange(object source, FileSystemEventArgs e)
        {
            SummaryFileChanged = true;
        }

        public static void BotFileChange(object source, FileSystemEventArgs e)
        {
            BotFileChanged = true;
        }

        public static void StopWatcher(string path)
        {
            
            
                BotWatcher.EnableRaisingEvents = false;
                BotWatcher.Changed -= new FileSystemEventHandler(BotFileChange);
            
            BotFileChanged = false;
        }
    }
}
