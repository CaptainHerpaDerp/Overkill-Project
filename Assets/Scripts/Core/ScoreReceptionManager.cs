using System;
using System.Diagnostics;

namespace Core
{
    public static class ScoreReceptionManager
    {
        // Define a delegate type for the event
        public delegate void ValueChangedHandler(int newValue);

        // Define an event based on the delegate
        public static Action<int, int> OnValueChanged;

        // Method to trigger the event
        public static void ChangePlayerScore(int playerIndex, int newScore)
        {
            OnValueChanged?.Invoke(playerIndex, newScore);
        }
    }
}