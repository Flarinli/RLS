using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLC
{
    public static class Data
    {
        public static Type target_type = Type.aircraft;

        public static double target_x = -10,
                             target_y = -10,
                             target_v = 10,
                             target_K = Convertion.DegreesRad(45),
                             target_Cx = 0.1086,
                             target_S = 12.5,
                             target_fuel_mass = 10.200,
                             target_critical_fuel_mass = 2,
                             target_mass_flow_of_fuel = 0.0004;

        public static int Count_Target = 0;
        public static int Count_CP_Missiles = 0;

        public static double cp_x = -3,
                             cp_y = -3,
                             cp_safety_distance = 4,
                             cp_TMax = 10,
                             cp_DLose = 0.2,
                             cp_PMin = 0.05,
                             cp_PMax = 0.9,
                             cp_PReq = 0.75;

        public static double rlc_x = 0,
                             rlc_y = 0,
                             rlc_det_range = 10;

        public static double simulator_T0 = 0,
                             simulator_Tk = 50,
                             simulator_dT = 0.001,
                             simulator_density = 1.233;

        public static string simulator_path = "Text.txt";

        public static List<Target> targets = new List<Target>();
        public static List<SAMMissile> missiles = new List<SAMMissile>();
    }
}
