using static System.Console;

namespace Singleton
{
    sealed class Singleton
    {
        static int instanceCounter = 0;
        private static Singleton singleInstance = null;

        public Singleton()
        {
            instanceCounter++;
            WriteLine($"instances created {instanceCounter}");
        }

        public static Singleton SingleInstance
        {
            get
            {
                if (singleInstance == null)
                {
                    singleInstance = new Singleton();
                }
                return singleInstance;
            }
        }
    }
}
