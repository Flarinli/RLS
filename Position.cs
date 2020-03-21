namespace RLS
{
    public abstract class Position
    {
        protected double init_time;
        protected MyPoint init_position;
        public MyPoint Current_Position { get; set; }
        public double Current_Time { get; set; }
    }
}
