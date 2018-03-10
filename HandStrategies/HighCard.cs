namespace Samus.HandStrategies
{
    class HighCard
    {
        private int NoOfRaises;
        private int Raise;
        private bool RaiseCalled;
        private bool HasRaise;
        private bool Call;
        private bool AllIn;
        private bool AllInCall;

        internal static void Action(Player actionplayer, bool flop)
        {

            //Resets.ReuseableMethods.ResetFlags(); // need to reset flags related to this file, currently resetting pre-flop ones.

            ////if drawing, try get to the turn cheap. if not bail.
            //PotOddsTolerance.CalculateTolerance(actionplayer, flop);

            //if (actionplayer.Tolerance == 0)
            //{
                //actionplayer.check = true;
                //return;
            //}

            //int x = 0;


        }
    }
}
