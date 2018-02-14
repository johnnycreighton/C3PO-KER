using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samus
{
    public class Flopper
    {
        private static string BotPathFile = Program.BotPath;
        private static string[] FileText;

        public static void Start(string path, int rank, int position, string hand)
        {
            //position 2 goes first
            //read flop
            //sort hand
            //get rank
            //check draws

            // check or bet

            //go turning*

            FileText = FileManipulation.Extractions.GetFileInfo(path);
            string[] cardNumbers = FileManipulation.Extractions.GetFlopCardNumbers(FileText); //trim start and finish
            string flopCards = FileManipulation.CardTransform.Flop(cardNumbers);


            return;
        }
    }
}
