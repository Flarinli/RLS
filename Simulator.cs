using System.IO;
using System.Collections.Generic;

namespace RLC
{
    public class Simulator
    {
        public static readonly int NUMBER = 1;  //Количество целей

        public static readonly double MAX_V;    //НУЖНО
        public double MIN_CRITICAL_FUEL_MASS;   //ЛИ
        public double MIN_MASS_FLOW_OF_FUEL;    //ЭТО
        public static double DT;                //ВСЕ???

        public double Density { get; set; }     //Плотность атмосферы. Принимается постоянной
        public List<Target> targets;
        public List<SAMMissile> SAMMissileList;
        public RLC my_RLS;
        public CommandPost my_CP;
        public readonly double t0, tk;
        public double Current_Time { get; set; }
        private readonly string path;
        public int Count_Target { get; set; } = 0;
        public int Count_CP_Missiles { get; set; } = 0;
        public int Count_Lose_Target { get; set; } = 0;

        public OnMyEventDelegate OnMyEvent { get; set; }
        public event OnMyEventDelegate MyEvent;
        public Simulator(ref RLC my_RLS,
                         ref CommandPost my_CP,
                         double t0,
                         double tk,
                         double dt,
                         double Density,
                         string path,
                         OnMyEventDelegate OnMyEvent)
        {
            targets = new List<Target>();
            SAMMissileList = new List<SAMMissile>();
            this.my_RLS = my_RLS;
            this.my_CP = my_CP;
            this.t0 = t0;
            this.tk = tk;
            DT = dt;
            Current_Time = t0;
            this.Density = Density;
            this.path = path;
            if (File.Exists(path))
            {
                File.WriteAllText(path, string.Empty);
            }
            this.OnMyEvent = OnMyEvent;
            MyEvent += OnMyEvent;
            MyEvent(null);
            MyEvent(this.my_RLS);
            MyEvent(this.my_CP);
        }
        public void AppendTarget(Target target)
        {
            SAMMissile tmp = target as SAMMissile;
            if ( tmp != null)
            {
                tmp.Explosion = Explosion;
                tmp.Destroy += tmp.Explosion;
                SAMMissileList.Add(tmp);
                Count_CP_Missiles++;
                tmp.b = true;
            }
            else
            {
                targets.Add(target);
                target.b = my_RLS.Measure(Current_Time, target);
                Count_Target++;
            }
            MyEvent(target);
        }
        private Euler_Integrator Euler = new Euler_Integrator();
        public void Peleng(StreamWriter sw, Target target)
        {
            const int form = 5;
            target.b = my_RLS.Measure(Current_Time, target);
            target.Move(new double[] { Density, DT }, ref Euler);
            if (target.b && (target.GetType() != typeof(SAMMissile)))
            {
                sw.Write($"Цель {targets.IndexOf(target)} обнаружена!\nТекущее время: {Current_Time,form:f2}; ");
                sw.Write($"Текущие координаты: ({target.Current_Position.X,form:f2};{target.Current_Position.Y,form:f2}); ");
                sw.WriteLine($"Дистанция до цели: {my_RLS.Distance,form:f2}; Азимут цели: {Convertion.RadDegrees(my_RLS.Az),form:f2}\n\n");
            }
        }

        public void Explosion(object O)
        {
            SAMMissile tmp = O as SAMMissile;
            if (tmp.IsLose == -1)
            {
                my_CP.delegated_targets.Remove(tmp.DelegateTarget);
                SAMMissileList.Remove(tmp);
                Count_CP_Missiles--;
            }
            else if (tmp.IsLose == 1)
            {
                targets.Remove(tmp.DelegateTarget);
                SAMMissileList.Remove(tmp);
                Count_Lose_Target++;
                Count_Target--;
                Count_CP_Missiles--;
            }

        }
        public void Run()
        {
            using (StreamWriter sw = new StreamWriter(path: path, append: true))
            {
                while (Current_Time <= tk)
                {
                    if (targets == null) break;
                    foreach (Target target in targets.ToArray())
                    {
                        Peleng(sw, target);
                    }
                    my_CP.targeting(SAMMissileList, targets);
                    foreach (SAMMissile missile in SAMMissileList.ToArray())
                    {
                        missile.Move(new double[] { DT, Density }, ref Euler);
                    }
                    MyEvent(this);
                    Current_Time += DT;
                }
            }
        }
    }
}
