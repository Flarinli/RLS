namespace RLS
{
    public class Euler_Integrator : IIntegrator
    {
        public double[] Step_Of_Integrator(double[] X, RightValue[] rF)

        {
            X[0] += X[2] * rF[0](X) * X[6];
            X[1] += X[2] * rF[1](X) * X[6];
            X[2] += rF[2](X) * X[6];
            return X;
        }
        public Euler_Integrator()
        {

        }
    }
}
