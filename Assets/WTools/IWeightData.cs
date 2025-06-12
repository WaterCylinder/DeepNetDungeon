using System;
//权重数据
[Serializable]
public struct WeightData<T>
{
    public T data;
    public int weight;
    public WeightData(T data, int weight){
        this.data = data;
        this.weight = weight;
    }
    public WeightData(T data){
        this.data = data;
        this.weight = 1;
    }
}
