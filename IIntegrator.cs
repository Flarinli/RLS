using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLC
{
    interface IIntegrator
    {
        double[] Step_Of_Integrator(double[] In, RightValue[] Rvalues);

    }
}
