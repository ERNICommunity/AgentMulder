using System;

namespace TestApplication.Types
{
    public class TakesIntAndInterface : IFoo
    {
        public TakesIntAndInterface(int value, IFoo Instance)
        {
        }
    }
}