namespace RLS
{
    public interface IIntegrator
    {
        double[] Step_Of_Integrator(double[] In, RightValue[] Rvalues);
    }
}
