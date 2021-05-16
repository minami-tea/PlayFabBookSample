using System;

namespace PlayFabBook
{
    [Flags]
    public enum TutorialFlag
    {
        SetNameAndGender = 1,
        ShowOpeningEvent = 1 << 1,
        CreateDeck = 1 << 2,
        Synthetic = 1 << 3,
    }
}
