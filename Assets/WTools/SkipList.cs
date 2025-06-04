using System;
using System.Collections;
using System.Collections.Generic;

public class SkipList<T> : IEnumerable<T>, IEnumerable
{
    public class Node{
        public T value;
        public Node[] next;
        public Node(T value,int level){
            this.value = value;
            next = new Node[level];
        }
        public override string ToString(){
            return $"Node value:{value} next:{next}";
        }
    }
    public Node head;
    private System.Random rand;
    public int count;
    public int maxLevel;
    public Func<T, T, bool> Compare;
    public Func<T, T, bool> Equal;
    public SkipList(){
        head = new Node(default, 32);//默认为最高32层
        rand = new System.Random(DateTime.Now.Millisecond);
        Compare = (a, b) => {
            return a.GetHashCode() <= b.GetHashCode();
        };
        Equal = (a, b) => {
            return a.Equals(b);
        };
        count = 0;
    }
    private int RandLevel(){
        maxLevel = Math.Min((int)Math.Pow(count,0.5f), 32);
        return rand.Next(1, maxLevel + 1);
    }
    /// <summary>
    /// 查找节点。
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public Node FindNode(T target){
        Node node = head;
        int level = head.next.Length - 1;
        while(level >= 0){
            while(node.next[level] != null && Compare(node.next[level].value, target)){
                node = node.next[level];
            }
            level --;
        }
        if(node == head || !Equal(node.value, target)){
            return null;
        }
        return node;
    }
    /// <summary>
    /// 添加对象
    /// </summary>
    /// <param name="value"></param>
    /// <param name="canRepeat">为true则可重复，无视存在的相同对象</param>
    /// <param name="replace">为true则遇到相同对象时替换相同对象</param>
    public void Add(T value, bool canRepeat = true, bool replace = true){
        Node node = head;
        Node newNode = new Node(value, RandLevel());
        int level = newNode.next.Length - 1;
        if(!canRepeat){
            Node n = FindNode(value);
            if(n != null){
                if(replace){
                    n.value = value;
                }
                return;
            }
        }
        while(level >= 0){
            while(node.next[level] != null && Compare(node.next[level].value, value)){
                node = node.next[level];
            }
            newNode.next[level] = node.next[level];
            node.next[level] = newNode;
            level --;
        }
        count ++;
    }
    /// <summary>
    /// 移除对象
    /// </summary>
    /// <param name="value"></param>
    public void Remove(T value){
        Node targetNode = FindNode(value);
        if(targetNode == null)
            return;
        Node node = head;
        int level = targetNode.next.Length - 1;
        while(level >= 0){
            while(node.next[level] != null && Compare(node.next[level].value, value) && !Equal(node.next[level].value, value)){
                node = node.next[level];
            }
            node.next[level] = targetNode.next[level];
            level--;
        }
        count --;
    }
    /// <summary>
    /// 遍历操作
    /// </summary>
    /// <param name="Action"></param>
    public void ForEach(Action<Node> Action){
        Node node = head.next[0];
        while(node != null){
            Action(node);
            node = node.next[0];
        }
    }
    public bool Exist(T value){
        return FindNode(value) != null;
    }

    public IEnumerator<T> GetEnumerator()
    {
        Node current = head.next[0];
        while (current != null)
        {
            yield return current.value;
            current = current.next[0];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
public class SkipList<T1, T2> : SkipList<KeyValuePair<T1, T2>>{
    public Node FindNode(T1 key, T2 value = default){
        return FindNode(new KeyValuePair<T1, T2>(key, value));
    }
    public Node FindNode(T2 value){
        return FindNode(new KeyValuePair<T1, T2>(default, value));
    }
    public void Add(T1 key, T2 value = default, bool canRepeat = true, bool replace = true){
        Add(new KeyValuePair<T1, T2>(key, value), canRepeat, replace);
    }
    public void Remove(T1 key){
        Remove(new KeyValuePair<T1, T2>(key, default));
    }    
    public bool Exist (T1 key, T2 value = default){
        return FindNode(key, value) != null;
    }
    public KeyValuePair<T1, T2> Find(T1 key){
        Node node = FindNode(key);
        return node != null ? node.value : default;
    }
}
