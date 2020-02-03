using System;
using System.Collections.Generic;

namespace RLC
{
    public class CommandPost : Position
    {
        public const int NUMBER = 1;
        public double Safety_Distance { get; set; }
        public CommandPost(double cp_x,
                           double cp_y,
                           double Safety_Distance)
        {
            Current_Position = new Point { X = cp_x, Y = cp_y };
            this.Safety_Distance = Safety_Distance;

        }
        public void targeting(List<SAMMissile> missiles, List<Target> targets)
        {
            foreach (Target target in targets)
            {
                if (Math.Sqrt(Math.Pow(target.Current_Position.X - Current_Position.X, 2) + Math.Pow(target.Current_Position.Y - Current_Position.Y, 2)) <= Safety_Distance)
                {
                    foreach (SAMMissile missile in missiles)
                    {
                        if (missile.IsLose == 0)
                        {
                            missile.IsLose = 1;
                            missile.DelegateTarget = target;
                            break;
                        }
                    }
                }
            }
        }
    }
}
