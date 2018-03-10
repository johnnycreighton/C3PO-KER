using System.IO;

namespace Samus.FileManipulation
{
    public class Listeners
    {
        public static FileSystemWatcher BotWatcher = new FileSystemWatcher();
        public static FileSystemWatcher SummaryWatcher = new FileSystemWatcher();

        public static bool BotFileChanged;

        public static bool SummaryFileChanged;

        public static void StartSummaryFileWatcher(string path)
        {
            SummaryFileChanged = false;
            SummaryWatcher.Path = path;
            SummaryWatcher.NotifyFilter = NotifyFilters.LastWrite;
            SummaryWatcher.Filter = "handSummary*";

            SummaryWatcher.Changed += new FileSystemEventHandler(SummaryFileChange);
            // Begin watching.
            SummaryWatcher.EnableRaisingEvents = true;
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
