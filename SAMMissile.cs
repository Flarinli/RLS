using System;

namespace RLC
{
    public class SAMMissile : Missile
    {
        public Target DelegateTarget { get; set; }
        public double TMax { get; set; }
        public double DLose { get; set; }
        public double PMin { get; set; }
        public double PMax { get; set; }
        public double PReq { get; set; }
        public double Time_Of_Flight { get; set; }
        public int IsLose { get; set; } // 0: летящая ракета, +1: цель поражена, -1: ракета самоуничтожена.
        public OnMyEventDelegate Explosion { get; set; }
        public event OnMyEventDelegate Destroy;

        new public double xR(double[] X) => X[2] * Math.Cos(X[3]);
        new public double yR(double[] X) => X[2] * Math.Sin(X[3]);
        new public double vR(double[] X) => X[4] - X[5];
        public SAMMissile(double x0,
                          double y0,
                          double course,
                          double absV,
                          double Cx,
                          double S,
                          double fuel_mass,
                          double critical_fuel_mass,
                          double mass_flow_of_fuel,
                          double current_time,
                          double TMax,  //Максимальное время полета до самоуничтожения
                          double DLose, //Дальность поражения
                          double PMin,  //Нижняя граница интервала вероятности поражения цели
                          double PMax,  //Верхняя граница интервала вероятности поражения цели
                          double PReq)  //Требуемое значение вероятности поражения цели 
                          : base(x0,
                                 y0,
                                 course,
                                 absV,
                                 Cx,
                                 S,
                                 fuel_mass,
                                 critical_fuel_mass,
                                 mass_flow_of_fuel,
                                 current_time)
        {
            this.TMax = TMax;
            this.DLose = DLose;
            this.PMin = PMin;
            this.PMax = PMax;
            this.PReq = PReq;
            Time_Of_Flight = 0;
            IsLose = 0;
            b = true;

        }
        public double course(Target target)
        {
            if ((target.Current_Position.X - Current_Position.X) > 0)
            {
                return Math.Atan((target.Current_Position.Y - Current_Position.Y) / (target.Current_Position.X - Current_Position.X));
            }
            else if ((target.Current_Position.X - Current_Position.X)  < 0)
            {
                if((target.Current_Position.Y - Current_Position.Y) > 0)
                {
                    return Math.Atan((target.Current_Position.Y - Current_Position.Y) / (target.Current_Position.X - Current_Position.X)) + Math.PI;
                }
                else
                {
                    return Math.Atan((target.Current_Position.Y - Current_Position.Y) / (target.Current_Position.X - Current_Position.X)) - Math.PI;
                }
            }
            else
            {
                return (target.Current_Position.Y - Current_Position.Y) > 0 ? (Math.PI / 2) : (-Math.PI / 2);
            }
        }
        public override void Move(double[] In, ref Euler_Integrator Euler)
        {
            Current_Time += In[0];

            if ((DelegateTarget == null) || (IsLose == 1) || (IsLose == -1)) return;

            Course = course(DelegateTarget);

            double[] X = new double[] { Current_Position.X,
                                        Current_Position.Y,
                                        V,
                                        Course,
                                        P,
                                        this.X,
                                        In[0] };
            X = Euler.Step_Of_Integrator(X, rightValues);
            Current_Position = new MyPoint { X = X[0], Y = X[1] };

            V = X[2];
            if (Fuel_Mass < Critical_Fuel_Mass)
            {
                P *= Fuel_Mass / Critical_Fuel_Mass;
            }
            Fuel_Mass -= Mass_Flow_Of_Fuel * In[0];
            this.X = Cx * In[1] * Math.Pow(V, 2) * S / 12;
            Time_Of_Flight += In[0];

            if (Math.Sqrt(Math.Pow(DelegateTarget.Current_Position.X - Current_Position.X, 2) + Math.Pow(DelegateTarget.Current_Position.Y - Current_Position.Y, 2)) <= DLose)
            {
                Random random = new Random();
                IsLose = ((PMin + (random.NextDouble() * (PMax - PMin))) < PReq) ? -1 : 1;
                Destroy(this);
                return;
            }
            if (Time_Of_Flight >= TMax)
            {
                IsLose = -1;
                Destroy(this);
                return;
            }
        }
    }
}
