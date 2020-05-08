using System;
using System.Collections.Generic;

namespace RLS
{
    public class RLS : Position
    {
        public double Detection_Range { get; set; }
        public double Distance { get; set; }
        public double Az { get; set; }
        public RLS(double x0, double y0, double detection_range, double t0)
        {
            init_position.X = x0;
            init_position.Y = y0;
            Current_Position = new MyPoint { X = x0, Y = y0 };
            Current_Time = t0;
            Detection_Range = detection_range;
            Distance = double.PositiveInfinity;
            Az = double.PositiveInfinity;
            WorkingWithDB.Insert("Objects", new List<string>() { "idExperiment", "InitX", "InitY" }, new List<string>() { WorkingWithDB.ExperimentID, x0.ToString(), y0.ToString() });
            WorkingWithDB.Insert("RLSs", new List<string>() { "idObject", "DetRange" }, new List<string>() { WorkingWithDB.CurID, detection_range.ToString() });
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
