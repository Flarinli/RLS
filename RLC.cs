using System;

namespace RLC
{
    public class RLC : Position
    {
        public double Detection_Range { get; set; }
        public double Distance { get; set; }
        public double Az { get; set; }
        public RLC(double x0, double y0, double detection_range, double t0)
        {
            init_position.X = x0;
            init_position.Y = y0;
            Current_Position = new Point { X = x0, Y = y0 };
            Current_Time = t0;
            Detection_Range = detection_range;
            Distance = double.PositiveInfinity;
            Az = double.PositiveInfinity;
        }
        public bool Measure(double Current_Time, Target target)
        {
            this.Current_Time = Current_Time;

            Az = target.Current_Position.X == Current_Position.X

            ? target.Current_Position.Y < 0 ? Az = -(Math.PI / 2) : Math.PI / 2

            : Math.Atan(((target.Current_Position.X - Current_Position.X) / target.Current_Position.Y - Current_Position.Y));

            Distance = Math.Sqrt(Math.Pow(target.Current_Position.X - Current_Position.X, 2) + Math.Pow(target.Current_Position.Y - Current_Position.Y, 2));
            return Distance <= Detection_Range;
        }
    }
}
