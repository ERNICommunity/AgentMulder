using System;

namespace TestApplication.Types
{
    public class TakesDelegate : IFoo
    {
        private Func<IFoo> getInstance;

        public TakesDelegate(Func<IFoo> getInstance)
        {
            this.getInstance = getInstance;
        }
    }
}