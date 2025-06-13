using System;
using System.Collections.Generic;
using System.Text;

public enum MapFlag{
    /// <summary>
    /// 空地
    /// </summary>
    Empty = 0,
    
    /// <summary>
    /// 普通房间
    /// </summary>
    Normal = 1,
    /// <summary>
    /// 终点房间（单连通）
    /// </summary>
    End = 2,
    /// <summary>
    /// 特殊房间
    /// </summary>
    Special = 3,
    /// <summary>
    /// BOSS房间（或者通往下一层的房间）
    /// </summary>
    Boss = 4,
    /// <summary>
    /// 起始房间
    /// </summary>
    Start = 5,
    /// <summary>
    /// 临时标记
    /// </summary>
    Temp = 6
}

public enum MapType{
    /// <summary>
    /// 随机类型
    /// </summary>
    Random = 0,
    /// <summary>
    /// 中心放射形
    /// </summary>
    Point = 1,
    /// <summary>
    /// 街道形
    /// </summary>
    Street = 2,
    /// <summary>
    /// 迷宫形
    /// </summary>
    Maze = 3,
    /// <summary>
    /// 填满
    /// </summary>
    Full = 4,
}

[Serializable]
// 自定义向量结构体
public class RogueMapCreator
{   
    public const float DEFAULT_SIZE = 0.4f;
    public const float DEFAULT_DISPERSION = 3f;

    public MapFlag[,] map;
    public MapFlag MapGet(Point pos){return map[pos.x, pos.y];}
    public void MapSet(Point pos, MapFlag flag){map[pos.x, pos.y] = flag;}
    public int width, height;
    public float size, dispersion;
    public Random rand;
    public int seed;
    public Point start, boss;
    public (int lw, int rw, int uh, int dh) bounds;
    /// <summary>
    /// 生成概率
    /// </summary>
    public float[,] prob;
    public List<Point> ends; 
    /// <summary>
    /// 每个点对于起始房间的距离
    /// </summary>
    public Dictionary<Point, int> distance;

    public RogueMapCreator(int height, int width, float size = DEFAULT_SIZE, float dispersion = DEFAULT_DISPERSION, int seed = 0){
        this.width = width;
        this.height = height;
        this.size = size;
        if(this.size < 0)this.size = 0;
        if(this.size > 1)this.size = 1;
        this.dispersion = dispersion;
        map = new MapFlag[height, width];
        prob = new float[height, width];
        SetSeed(seed);
    }
    public int SetSeed(int seed){
        this.seed = seed;
        rand = new Random(seed);
        return seed;
    }
    public bool RandCheck(float p){
        if(p <= 0)return false; 
        if(p >= 1)return true;
        return rand.NextDouble() < p;
    }
    public void Init(){
        start = new Point(height / 2, width / 2);
        MapSet(start, MapFlag.Start);
        bounds.lw = start.y;
        bounds.rw = width - 1 - start.y;
        bounds.uh = start.x;
        bounds.dh = height - 1 - start.x;
        Generate();
        ConnectivityCheck();
        SetBoss();
    }
    //检查位置是否超出边界
    bool Limit(Point p){
        if(p.x < 0 || p.y < 0 || p.x >= height || p.y >= width)
            return false;
        return true;
    }
    public bool Limit(int x, int y){
        return Limit(new Point(x, y));
    }
    //遍历map
    void MapGridBy(Action<Point> action){
        for(int x = 0; x < height; x++){
            for(int y = 0; y < width; y++){
                action(new Point(x, y));
            }
        }
    }

    void Generate(){
        //遍历1，生成临时标志
        MapGridBy(p => {
            //计算每个点的基础生成概率pp
            float pxp = Math.Abs(p.x - start.x); 
            float pyp = Math.Abs(p.y - start.y);
            if(p.x < start.x){
                pxp = 1 - pxp / bounds.uh;
            }else if(p.x > start.x){
                pxp = 1 - pxp / bounds.dh;
            }else{
                pxp = 1;
            }
            if(p.y < start.y){
                pyp = 1 - pyp / bounds.lw;
            }else if(p.y > start.y){
                pyp = 1 - pyp / bounds.rw;
            }else{
                pyp = 1;
            }
            float pp = (pxp + pyp) / 2;
            //根据距离和两个参数重新计算概率
            pp = (float)(1 - 1 / (1 + Math.Pow(Math.E, dispersion * pp / (1 - size) - dispersion)));
            //存储概率并公示
            prob[p.x, p.y] = pp;
            if(RandCheck(pp)){
                MapSet(p,MapFlag.Temp);
            }
            if(p.Equals(start)){
                MapSet(p,MapFlag.Start);
            }
        });
    }
    //连通性检查
    void ConnectivityCheck(){
        //BFS检查连通性，将连通的Temp标记为Normal
        Stack<Point> stack = new();
        stack.Push(start);
        List<Point> toward = new()
        {
            new Point(0, 1),
            new Point(1, 0),
            new Point(0, -1),
            new Point(-1, 0)
        };
        distance = new Dictionary<Point, int>();
        distance[start] = 0;
        while(stack.Count > 0){
            Point p = stack.Pop();
            foreach(Point t in toward){
                Point q = new Point(p.x + t.x, p.y + t.y);
                if(Limit(q) && MapGet(q) == MapFlag.Temp){
                    MapSet(q, MapFlag.Normal);
                    distance.Add(q, distance[p] + 1);
                    stack.Push(q);
                }
            }
        }
        //清空未被标记的Temp，那之后将单连接的Nromal房间标记为End
        ends = new List<Point>();
        MapGridBy(p=>{
            if(MapGet(p) == MapFlag.Temp)
                MapSet(p, MapFlag.Empty);
            if(MapGet(p) == MapFlag.Normal){
                int cntn = 0;
                foreach(Point t in toward){
                    Point q = new Point(p.x + t.x, p.y + t.y);
                    if(Limit(q) && MapGet(q) != MapFlag.Empty){
                        cntn++;
                    }
                }
                if(cntn == 1){
                    MapSet(p, MapFlag.End);
                    ends.Add(p);
                }
            }
        }); 
    }

    public void SetBoss(){
        Point boss = ends[0];
        foreach(Point p in ends){
            if(distance[p] >= distance[boss]){
                boss = p;
            }
        }
        MapSet(boss, MapFlag.Boss);
    }

    public override string ToString(){
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Map:Seed{seed}");
        // 添加列号标识
        sb.Append("   ");
        for (int y = 0; y < width; y++){
            sb.AppendFormat("{0,2}", y);
        }
        sb.AppendLine();
        for (int x = 0; x < height; x++){
            // 添加行号标识
            sb.AppendFormat("{0,2}|", x);
            
            for (int y = 0; y < width; y++){
                // 将枚举值转换为数字表示
                sb.AppendFormat("{0,2}", (int)map[x, y]);
            }
            sb.AppendLine();
        }

        // 3. 输出概率矩阵
        sb.AppendLine("\n概率矩阵 prob[,]:");
        sb.Append("   ");
        for (int y = 0; y < width; y++) {
            sb.AppendFormat("{0,10}", y); // 列号
        }
        sb.AppendLine();
        
        for (int x = 0; x < height; x++) {
            sb.AppendFormat("{0,2}|", x); // 行号
            for (int y = 0; y < width; y++) {
                sb.AppendFormat("{0,8:F4}", prob[x, y]); // 保留4位小数
            }
            sb.AppendLine();
        }

        // 4. 新增距离矩阵输出
        sb.AppendLine("\n距离矩阵 distance:");
        sb.Append("   ");
        for (int y = 0; y < width; y++) sb.AppendFormat("{0,5}", y); // 缩窄列宽
        sb.AppendLine();
        for (int x = 0; x < height; x++) {
            sb.AppendFormat("{0,2}|", x);
            for (int y = 0; y < width; y++) {
                Point p = new Point(x, y);
                // 显示"-1"表示不可达，其他显示实际距离
                int dist = distance.ContainsKey(p) ? distance[p] : -1;
                sb.AppendFormat("{0,5}", dist); // 对齐宽度与列号一致
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }
}