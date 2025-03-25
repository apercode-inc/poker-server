using System.Collections;

namespace server.Code.GlobalUtils.CustomCollections;

public class MovingMarkersDictionary<T, TM> : IEnumerable<MarkedItem<T, TM>> where TM : Enum
{
    private MarkedItem<T, TM>[] _data;
    private Dictionary<TM, MarkerSetting> _markerSettings;

    public int Count { get; private set; }

    public MovingMarkersDictionary(int size)
    {
        var markers = Enum.GetValues(typeof(TM))
            .Cast<TM>().ToDictionary(enumMember=> enumMember, enumMember => new bool());
        
        _data = new MarkedItem<T, TM>[size];

        for (var index = 0; index < _data.Length; index++)
        {
            _data[index] = new MarkedItem<T, TM>(index, new Dictionary<TM, bool>(markers));
        }

        _markerSettings = Enum.GetValues(typeof(TM)).Cast<TM>()
            .ToDictionary(enumMember => enumMember, enumMember => new MarkerSetting());
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

    public bool Remove(T value, Dictionary<TM, T> movedValuesByMarker = null)
    {
        var index = IndexOf(value);

        if (index < 0 || index >= _data.Length)
        {
            return false;
        }
        
        return Remove(index, movedValuesByMarker);
    }

    public void SetMarker(T value, TM marker)
    {
        var index = IndexOf(value);
        
        if (index < 0)
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

    public void SetSettingMarker(TM marker, MarkerSettingType settingType, bool value)
    {
        switch (settingType)
        {
            case MarkerSettingType.MoveForwardDirection:
                _markerSettings[marker].IsMoveForwardDirection = value;
                break;
            case MarkerSettingType.MoveWithRemoveForwardDirection:
                _markerSettings[marker].IsMoveWithRemoveForwardDirection = value;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(settingType), settingType, null);
        }
    }

    public void ResetMarkers(params TM[] markers)
    {
        foreach (var item in _data)
        {
            foreach (var marker in markers)
            {
                if (item.Markers[marker])
                {
                    item.Markers[marker] = false;
                }
            }
        }
    }

    public bool TryMoveMarker(TM marker, out MarkedItem<T, TM> newMarkedValue)
    {
        for (var index = 0; index < _data.Length; index++)
        {
            if (!_data[index].Markers[marker])
            {
                continue;
            }
            
            if (Count < 2)
            {
                newMarkedValue = _data[index];
                return true;
            }

            if (_markerSettings[marker].IsMoveForwardDirection)
            {
                if (MoveMarkerForward(marker, out newMarkedValue, index))
                {
                    return true;
                } 
            }
            else
            {
                if (MoveMarkerBackward(marker, out newMarkedValue, index))
                {
                    return true;
                } 
            }
        }

        newMarkedValue = default;
        return false;
    }

    public bool TryGetValueByMarkers(int key, out MarkedItem<T, TM> value)
    {
        foreach (var markedItem in this)
        {
            if (markedItem.Key == key)
            {
                value = markedItem;
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

    public MarkedItem<T, TM> GetFirst()
    {
        foreach (var item in this)
        {
            if (item.Value != null)
            {
                return item;
            }
        }
        
        return default;
    }

    public bool TryGetNext(TM marker, out MarkedItem<T, TM> value)
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

                if (_data[nextIndex].Value == null || _data[nextIndex].IsMoveIgnore)
                {
                    continue;
                }

                value = _data[nextIndex];
                return true;
            }

            value = default;
            return false;
        }

        value = default;
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
    
    private bool Remove(int index, IDictionary<TM, T> movedValuesByMarker = null)
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

            MarkedItem<T, TM> newMarkedItem;
            
            if (_markerSettings[marker.Key].IsMoveWithRemoveForwardDirection)
            {
                if (MoveMarkerForward(marker.Key, out newMarkedItem, index) && movedValuesByMarker != null)
                {
                    movedValuesByMarker.Add(marker.Key, newMarkedItem.Value);
                }
            }
            else
            {
                if (MoveMarkerBackward(marker.Key, out newMarkedItem, index) && movedValuesByMarker != null) 
                {
                    movedValuesByMarker.Add(marker.Key, newMarkedItem.Value);
                }
            }
        }
        
        _data[index].Value = default;
        Count--;
        
        return true;
    }

    private bool MoveMarkerBackward(TM marker, out MarkedItem<T, TM> newMarkedItem, int index)
    {
        var iterationIndex = 0;
        var currentIndex = index;

        while (iterationIndex < _data.Length)
        {
            currentIndex = (currentIndex - 1 + _data.Length) % _data.Length;
            iterationIndex++;

            if (_data[currentIndex].Value == null || _data[currentIndex].IsMoveIgnore)
            {
                continue;
            }

            newMarkedItem = _data[currentIndex];
            _data[currentIndex].Markers[marker] = true;
            _data[index].Markers[marker] = false;

            return true;
        }

        newMarkedItem = default;
        return false;
    }

    private bool MoveMarkerForward(TM marker, out MarkedItem<T, TM> newMarkedItem, int index)
    {
        var iterationCount = 0;

        for (var i = index + 1; iterationCount < _data.Length; i++)
        {
            var nextIndex = i % _data.Length;
            iterationCount++;

            if (_data[nextIndex].Value == null || _data[nextIndex].IsMoveIgnore)
            {
                continue;
            }

            newMarkedItem = _data[nextIndex];
            _data[index].Markers[marker] = false;
            _data[nextIndex].Markers[marker] = true;
            return true;
        }

        newMarkedItem = default;
        return false;
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
    
    private class MarkerSetting
    {
        public bool IsMoveForwardDirection;
        public bool IsMoveWithRemoveForwardDirection;

        public MarkerSetting()
        {
            IsMoveForwardDirection = true;
            IsMoveWithRemoveForwardDirection = true;
        }
    }
}

public enum MarkerSettingType : byte
{
    MoveForwardDirection = 0,
    MoveWithRemoveForwardDirection = 1,
}

public class MarkedItem<T, TM> where TM : Enum
{
    public T Value { get; internal set; }
    public int Key { get; }
    public bool IsMoveIgnore { get; set; }
    public Dictionary<TM, bool> Markers { get; }

    public MarkedItem(int key, Dictionary<TM, bool> markers)
    {
        Key = key;
        Markers = markers;
    }
}

