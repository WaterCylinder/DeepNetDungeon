public struct Point{
        public int x, y;
        public Point(int x, int y){
            this.x = x;
            this.y = y;
        }
        public override bool Equals(object obj){
            if(obj.GetType() == typeof(Point)){
                Point p = (Point)obj;
                return x == p.x && y == p.y;
            }
            return false;
        }
        public override int GetHashCode(){
            return x * 1000 + y;
        }
        public override string ToString(){
            return "(" + x + ", " + y + ")";
        }
    }
