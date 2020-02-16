using System;

namespace RLC
{
    public class Missile : Target
    {
        public Missile(double x0,
                       double y0,
                       double course,
                       double absV,
                       double Cx,
                       double S,
                       double fuel_mass,
                       double critical_fuel_mass,
                       double mass_flow_of_fuel,
                       double current_time) : base(x0,
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
            flObj = Type.missile;
        }

        public override void Move(double[] In, ref Euler_Integrator Euler)
        {
            if (V == 0) return;
            double[] X = new double[] { Current_Position.X,
                                        Current_Position.Y,
                                        V,
                                        Course,
                                        P,
                                        this.X,
                                        In[1] };


            X = Euler.Step_Of_Integrator(X, rightValues);
            Current_Position = new MyPoint { X = X[0], Y = X[1] };
            V = X[2] > 0 ? X[2] : 0;
            if (Fuel_Mass < Critical_Fuel_Mass)
            {
                P *= Fuel_Mass / Critical_Fuel_Mass;
            }
            Fuel_Mass -= Mass_Flow_Of_Fuel * In[0];
            this.X = Cx * In[1] * Math.Pow(V, 2) * S / 2;
        }
    }
}
