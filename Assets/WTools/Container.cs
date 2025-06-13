

[System.Serializable]
public class Container<T> where T : class
{   
    public static Container<T> Done{
        get{
            Container<T> done = new Container<T>();
            done.SetDone(true);
            return done;
        }
    }
    private T obj;
    private bool isDone = false;
    public bool done{get{return obj != null || isDone;}}
    public Container(T obj){
        this.obj = obj;
    }
    public Container(){
        obj = null;
    }
    public void Set(T obj){
        this.obj = obj;
    }
    public T Get(){
        return obj;
    }
    public void SetDone(bool done){
        isDone = done;
    }
    public override string ToString(){
        return $"Container: Type:{typeof(T).Name}\nObj:{obj}\nDoneController:{isDone}\nDone:{done}\n";
    }
}
