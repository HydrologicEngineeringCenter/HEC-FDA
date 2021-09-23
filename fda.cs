using System;
using ead;
namespace fda
{
    //we should absolutely use the c# statistics library we already have. this is just to mock in something.
    public static class FDA
    {
        public static void Main(string[] args){
            Simulation sim = new Simulation();
            //load curves with cool info.
            sim.Compute(1234,100);
        }
    }
}