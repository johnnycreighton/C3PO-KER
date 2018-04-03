using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Samus.FileManipulation
{
    public class Extractions
    {
        /// <summary>
        /// Check if file is ready.
        /// </summary>
        /// <param name="sFilename"></param>
        /// <returns></returns>
        public static bool IsFileReady(String sFilename)
        {
            FileStream stream = null;
            FileInfo file = new FileInfo(sFilename);
            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return false;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return true;
        }

        /// <summary>
        /// Returns Turn card number
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        internal static int GetTurnCardNumber(string[] lines)
        {
            // File.AppendAllText(Program.DebugBotPath, "Getting TURN card number..." + System.Environment.NewLine);

            foreach (string line in lines) //try catch this
            {
                if (line.Contains("TURN"))
                {
                    string endOfString = line.Substring(line.Length - 3); // line will = ***TURN*** [X][Y][Z] [turn card], i only want the turn card
                    return Convert.ToInt32(Regex.Match(endOfString, @"\d+").Value); //grabs the first number in the line
                }
            }
            throw new Exception("TURN card not here. test this method.");
        }

        /// <summary>
        /// Retrieves flop card numbers from a text file
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static string[] GetFlopCardNumbers(string[] lines)
        {
            //File.AppendAllText(Program.DebugBotPath, "Getting FLOP card numbers..." + System.Environment.NewLine);

            foreach (string line in lines) //try catch this
            {
                if (line.Contains("FLOP"))
                {
                    return Regex.Split(line, @"\D+"); //returns string array of numbers, which are the  flop cards.
                }
            }
            // File.AppendAllText(Program.DebugBotPath, "Flop wasnt found, EXITING PROGRAM." + System.Environment.NewLine);
            System.Environment.Exit(0);
            return null;
            //throw new Exception("FLOP not here. test this method.");
        }

        internal static bool RaiseFound()
        {
            string text = null;
            here:
            while (true)
            {
                if (IsFileReady(Program.CasinoToBot))
                {
                    try
                    {
                        text = File.ReadAllText(Program.CasinoToBot);
                        break;
                    }
                    catch
                    { goto here; }
                }
            }

            string endString = text.Substring(text.Length-4);

            if (endString.Contains("r"))
            {
                return true;
            }
            return false;
        }
    }
}
