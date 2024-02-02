using System.Collections;

namespace server.Code.GlobalUtils.CustomCollections;

public class MovingMarkersDictionary<T, TM> : IEnumerable<MarkedItem<T, TM>> where TM : Enum
{
    private MarkedItem<T, TM>[] _data;

    public int Count { get; private set; }

    public MovingMarkersDictionary(int size)
    {
        var markers = Enum.GetValues(typeof(TM))
            .Cast<TM>().ToDictionary(enumMember=> enumMember, enumMember => false);
        
        _data = new MarkedItem<T, TM>[size];

        for (var index = 0; index < _data.Length; index++)
        {
            _data[index] = new MarkedItem<T, TM>(index, new Dictionary<TM, bool>(markers));
        }
    }

    public bool Add(int index, T value)
    {
        if (index < 0 || index >= _data.Length)
        {
            throw new IndexOutOfRangeException($"index = {index}");
        }
        
        if (_data[index].Value != null)
        {
            return false;
        }
        
        _data[index].Value = value;
        Count++;

        return true;
    }

    public bool Remove(T value, Dictionary<TM, T> previousMarkerValues = null)
    {
        var index = IndexOf(value);

        if (index < 0 || index >= _data.Length)
        {
            return false;
        }
        
        return Remove(index, previousMarkerValues);
    }

    public void SetMarker(int index, TM marker)
    {
        if (_data[index].Value == null)
        {
            throw new NullReferenceException($"no value with index = {index} ");
        }
        
        foreach (var item in _data)
        {
            if (item.Markers[marker])
            {
                throw new Exception($"Set multiple markers {marker.GetType()}.{marker.ToString()}");
            }
        }
        
        _data[index].Markers[marker] = true;
    }

    public void ResetMarker(int index, TM marker)
    {
        _data[index].Markers[marker] = false;
    }
    
    public bool TryMoveMarker(TM marker, out MarkedItem<T, TM> value)
    {
        for (var index = 0; index < _data.Length; index++)
        {
            if (Count < 2)
            {
                value = default;
                return false;
            }

            if (!_data[index].Markers[marker])
            {
                continue;
            }

            var iterationCount = 0;
            
            for (var i = index + 1; iterationCount < _data.Length; i++)
            {
                var nextIndex = i % _data.Length;
                iterationCount++;

                if (_data[nextIndex].Value == null)
                {
                    continue;
                }
    
                value = _data[nextIndex];
                _data[index].Markers[marker] = false;
                _data[nextIndex].Markers[marker] = true;
                return true;
            }
        }

        value = default;
        return false;
    }

    public bool TryGetValue(int key, out T value)
    {
        foreach (var markedItem in this)
        {
            if (markedItem.Key == key)
            {
                value = markedItem.Value;
                return true;
            }
        }

        value = default;
        return false;
    }

    public bool ContainsKey(int key)
    {
        foreach (var markedItem in this)
        {
            if (markedItem.Key == key)
            {
                return true;
            }
        }
        
        return false;
    }
    
    public bool ContainsValue(T value)
    {
        foreach (var markedItem in this)
        {
            if (markedItem.Value != null && markedItem.Value.Equals(value))
            {
                return true;
            }
        }
        
        return false;
    }

    public bool TryGetValueByMarked(TM marker, out MarkedItem<T, TM> value)
    {
        foreach (var item in _data)
        {
            if (!item.Markers[marker])
            {
                continue;
            }
            
            value = item;
            return true;
        }
        
        value = default;
        return false;
    }

    public IEnumerator<MarkedItem<T, TM>> GetEnumerator()
    {
        foreach (var item in _data)
        {
            if (item.Value == null)
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
    
    //todo сейчас при выходе(удалении) игрока макрер хода, будет также сдвинут назад, а по идеи нужно вперёд
    //Т.е диллера двигать назад, а активность вперёд
    private bool Remove(int index, IDictionary<TM, T> previousValuesByMarker = null) 
    {
        if (_data[index].Value == null)
        {
            return false;
        }
        
        foreach (var marker in _data[index].Markers)
        {
            if (!marker.Value)
            {
                continue;
            }
 
            var iterationIndex = 0;
            var currentIndex = index;

            while (iterationIndex < _data.Length)
            {
                currentIndex = (currentIndex - 1 + _data.Length) % _data.Length;
                iterationIndex++;

                if (_data[currentIndex].Value == null)
                {
                    continue;
                }
                
                _data[currentIndex].Markers[marker.Key] = true;
                _data[index].Markers[marker.Key] = false;
                previousValuesByMarker?.Add(marker.Key, _data[currentIndex].Value);
                
                break;
            }
        }
        
        _data[index].Value = default;
        Count--;
        
        return true;
    }
    
    private int IndexOf(T value)
    {
        var findIndex = -1;
        
        for (var index = 0; index < _data.Length; index++)
        {
            var findValue = _data[index].Value;
            
            if (findValue != null && findValue.Equals(value))
            {
                findIndex = index;
            }
        }

        return findIndex;
    }
}

public struct MarkedItem<T, TM> where TM : Enum
{
    public T Value { get; internal set; }
    public int Key { get; }
    public Dictionary<TM, bool> Markers { get; }

    public MarkedItem(int key, Dictionary<TM, bool> markers)
    {
        Key = key;
        Markers = markers;
    }
}

