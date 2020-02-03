namespace RLC
{
    public abstract class Position
    {
        protected double init_time;
        protected Point init_position;
        public Point Current_Position { get; set; }
        public double Current_Time { get; set; }
    }
}
