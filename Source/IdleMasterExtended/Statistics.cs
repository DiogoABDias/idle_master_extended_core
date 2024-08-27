namespace IdleMasterExtended;

public class Statistics
{
    private uint _sessionMinutesIdled = 0;
    private uint _sessionCardIdled = 0;
    private uint _remainingCards;

    public uint GetSessionMinutesIdled() => _sessionMinutesIdled;

    public uint GetSessionCardIdled() => _sessionCardIdled;

    public uint GetRemainingCards() => _remainingCards;

    public void SetRemainingCards(uint remainingCards) => _remainingCards = remainingCards;

    public void CheckCardRemaining(uint actualCardRemaining)
    {
        if (actualCardRemaining < _remainingCards)
        {
            IncreaseCardIdled(_remainingCards - actualCardRemaining);
            _remainingCards = actualCardRemaining;
        }
        else if (actualCardRemaining > _remainingCards)
        {
            _remainingCards = actualCardRemaining;
        }
    }

    public void IncreaseCardIdled(uint number)
    {
        Properties.Settings.Default.totalCardIdled += number;
        Properties.Settings.Default.Save();
        _sessionCardIdled += number;
    }

    public void IncreaseMinutesIdled()
    {
        Properties.Settings.Default.totalMinutesIdled++;
        Properties.Settings.Default.Save();
        _sessionMinutesIdled++;
    }
}