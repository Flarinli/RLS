using System;

namespace RLC
{
    public abstract class Target : Position
    {
        public double P { get; set; }
        public double X { get; set; }
        public double Cx { get; set; }  //Коэффициент силы лобового сопротивления
        public double S { get; set; }   //Площадь Миделя
        public double Fuel_Mass { get; set; }
        public double Start_Fuel_Mass { get; set; }
        public double Critical_Fuel_Mass { get; set; }
        public double Mass_Flow_Of_Fuel { get; set; }
        public double Course { get; set; }
        public double V { get; set; }

        protected Type flObj = Type.None;

        public RightValue[] rightValues;
        public bool b { get; set; }
        public Target(double x0,
                      double y0,
                      double course,
                      double absV,
                      double Cx,
                      double S,
                      double fuel_mass,
                      double critical_fuel_mass,
                      double mass_flow_of_fuel,
                      double current_time)
        {
            init_position.X = x0;
            init_position.Y = y0;
            Current_Position = new MyPoint { X = x0, Y = y0 };
            init_time = current_time;
            Current_Time = current_time;
            V = absV;
            this.Cx = Cx;
            this.S = S;
            Course = course;
            Fuel_Mass = fuel_mass;
            Start_Fuel_Mass = fuel_mass;
            Critical_Fuel_Mass = critical_fuel_mass;
            Mass_Flow_Of_Fuel = mass_flow_of_fuel;
            rightValues = new RightValue[] { xR, yR, vR };
        }
        public double xR(double[] X) => X[2] * Math.Cos(X[3]);
        public double yR(double[] X) => X[2] * Math.Sin(X[3]);
        public double vR(double[] X) => X[4] - X[5];
        public abstract void Move(double[] In, ref Euler_Integrator integrator);
    }
}
