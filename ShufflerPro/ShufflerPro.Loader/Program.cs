using Ninject;
using ShufflerPro.Core.Interfaces;

namespace ShufflerPro.Loader
{
    class Program
    {
        static void Main()
        {
            var runner = new Runner();
            runner.Run();
        }
    }
}
