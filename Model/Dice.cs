namespace TravEx.Model;

public class Dice
{
    private readonly Random _rng;

    public Dice(int seed)
    {
        _rng = new Random(seed);
    }

    public int RollD6s(int amount = 1)
    {
        var sum = 0;
        for (var i = 0; i < amount; i++)
        {
            sum += RollD6();
        }

        return sum;
    }

    public int RollInRange(int upTo, int from = 0) => _rng.Next(from, upTo + 1);

    private int RollD6() => _rng.Next(1, 7);
}