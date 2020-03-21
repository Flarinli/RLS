using System.Collections.Generic;

namespace RLS
{
    public class CommandPost : Position
    {
        public const int NUMBER = 3;
        public double Safety_Distance { get; set; }
        public List<Target> delegated_targets;
        public CommandPost(double cp_x,
                           double cp_y,
                           double Safety_Distance)
        {
            Current_Position = new MyPoint { X = cp_x, Y = cp_y };
            this.Safety_Distance = Safety_Distance;
            delegated_targets = new List<Target>();
        }
        public void targeting(List<SAMMissile> missiles, List<Target> targets)
        {
            foreach (Target target in targets)
            {
                if (target.b)
                {
                    foreach (SAMMissile missile in missiles)
                    {
                        if ((missile.DelegateTarget == null) && (! delegated_targets.Contains(target)))
                        {
                            missile.IsLose = 0;
                            missile.DelegateTarget = target;
                            delegated_targets.Add(target);
                            break;
                        }
                    }
                }
            }
        }
    }
}
