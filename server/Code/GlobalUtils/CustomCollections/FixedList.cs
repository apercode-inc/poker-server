using System.Collections;

namespace server.Code.GlobalUtils.CustomCollections;

public class FixedList<T> : IEnumerable<T>
{
    protected T[] _data;
    protected int _size;

    public int Count { get; private set; }

    public FixedList(int size)
    {
        _data = new T[size];
        _size = size;
        Count = 0;
    }

    public void Add(T value)
    {
        if (Count == _size)
        {
            return;
        }

        for (var index = 0; index < _data.Length; index++)
        {
            if (_data[index] != null)
            {
                continue;
            }
            _data[index] = value;
            break;
        }

        Count++;
    }
    
    public void Remove(T value)
    {
        var index = Array.IndexOf(_data, value);

        if (index == -1)
        {
            return;
        }

        _data[index] = default;

        Count--;
    }

    public IEnumerator<T> GetEnumerator()
    {
        foreach (var item in _data)
        {
            if (item == null)
            {
                continue;
            }
            yield return item;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}