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

        public static void StartBotWatcher(string path)
        {
            BotWatcher.Path = path;
            BotWatcher.NotifyFilter = NotifyFilters.LastWrite;
            BotWatcher.Filter = "bot1.txt";

            BotWatcher.Changed += new FileSystemEventHandler(BotFileChange);
            // Begin watching.
            BotWatcher.EnableRaisingEvents = true;
        }

        public static void StartPlayAreaEvenWatcher(string path)
        {
            PlayAreaEvenWatcher.Path = path;
            PlayAreaEvenWatcher.NotifyFilter = NotifyFilters.LastWrite;
            PlayAreaEvenWatcher.Filter = "*Even.txt";
            PlayAreaEvenFileChanged = false;

            PlayAreaEvenWatcher.Changed += new FileSystemEventHandler(PlayAreaEvenFileChange);

            // Begin watching.
            PlayAreaEvenWatcher.EnableRaisingEvents = true;
        }

        public static void StartPlayAreaOddWatcher(string path)
        {
            PlayAreaOddWatcher.Path = path;
            PlayAreaOddWatcher.NotifyFilter = NotifyFilters.LastWrite;
            PlayAreaOddWatcher.Filter = "*Odd.txt";
            PlayAreaOddFileChanged = false;

            PlayAreaOddWatcher.Changed += new FileSystemEventHandler(PlayAreaOddFileChange);

            // Begin watching.
            PlayAreaOddWatcher.EnableRaisingEvents = true;
        }

        public static void PlayAreaOddFileChange(object sender, FileSystemEventArgs e)
        {
            PlayAreaOddFileChanged = true;
        }
        public static void PlayAreaEvenFileChange(object sender, FileSystemEventArgs e)
        {
            PlayAreaEvenFileChanged = true;
        }

        public static void BotFileChange(object source, FileSystemEventArgs e)
        {
            BotFileChanged = true;
        }

        public static void StopWatcher(string path)
        {
            if (path.Contains("Odd"))
            {
                PlayAreaOddWatcher.EnableRaisingEvents = false;
                PlayAreaOddWatcher.Changed -= new FileSystemEventHandler(PlayAreaOddFileChange);
            }
            else if (path.Contains("Even"))
            {
                PlayAreaEvenWatcher.EnableRaisingEvents = false;
                PlayAreaEvenWatcher.Changed -= new FileSystemEventHandler(PlayAreaEvenFileChange);
            }
            else
            {
                BotWatcher.EnableRaisingEvents = false;
                BotWatcher.Changed -= new FileSystemEventHandler(BotFileChange);
            }
            BotFileChanged = false;
        }
    }
}
