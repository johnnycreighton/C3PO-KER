namespace Samus.Resets
{
    class ReuseableMethods : PreFlop
    {
        /// <summary>
        /// resetting boolean flags to monitor new hand.
        /// </summary>
        internal static void ResetFlags()
        {
                NoOfRaises = 0;
                HasRaise = false;
                Call = false;
                AllIn = false;
                AllInCall = false;
                RaiseCalled = false; 
        }
    }
}

