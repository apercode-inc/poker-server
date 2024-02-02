using System.Collections;

namespace server.Code.GlobalUtils.CustomCollections;

public class RandomList<T> : FixedList<T>
{
    private Random _random;

    public RandomList(int size) : base(size)
    {
        _random = new Random();
    }

    public bool TryRandomRemove(out T value)
    {
        if (Count == 0)
        {
            value = default;
            return false;
        }
        
        var randomItemNumber = _random.Next(0, Count);
        var counter = 0;

        foreach (var item in _data)
        {
            if (item == null)
            {
                continue;
            }

            if (randomItemNumber != counter)
            {
                counter++;
                continue;
            }

            value = item;
            Remove(item);
            return true;
        }

        value = default;
        return false;
    }
}