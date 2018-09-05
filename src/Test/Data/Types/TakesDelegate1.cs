using System;

namespace TestApplication.Types
{
    public class TakesDelegate1 : IFoo
    {
        private Func<IFoo> getInstance;

        public TakesDelegate1(Func<IFoo> getInstance)
        {
            this.getInstance = getInstance;
        }
    }
}