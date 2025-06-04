using System;

[Serializable]
public class LoopList<T> where T : class
{   
    private T[] _list;
    private int _maxCount;
    private int _ptr;
    public LoopList(int maxCount){
        _maxCount = maxCount;
        _list = new T[maxCount];
        _ptr = 0;
    }
    public void Add(T item){
        _list[_ptr] = item;
        _ptr = (_ptr + 1) % _maxCount;
    }
    public void ForEach(Action<T> action){
        for(int i = 0; i < _maxCount; i++){
            if(_list[i] != null)action(_list[i]);
        }
    }
    public void RemoveAt(int index){
        for(int i = index; i < _maxCount - 1; i++){
            if(_list[i + 1] == null){
                _ptr = i;
                _list[i] = null;
                return;
            }
            _list[i] = _list[i + 1];
        }
        _list[_maxCount - 1] = null;
        _ptr = _maxCount - 1;
        return;
    }
    public void Remove(T item){
        for(int i = 0; i < _maxCount; i++){
            if(_list[i] == item){
                RemoveAt(i);
                return;
            }
        }
    }
    public bool Contains(T item){ 
        for(int i = 0; i < _maxCount; i++){
            if(_list[i] == item)return true;
        }
        return false;
    }
    public T Find(Func<T, bool> action){
        for(int i = 0; i < _maxCount; i++){
            if(_list[i] != null && action(_list[i])){
                return _list[i];
            }
        }
        return null;
    }
    public bool Exists(Func<T, bool> action){
        return Find(action) != null;
    }
}
