using System;

namespace NFactorial
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(NFactorial(6));
        }

        static public int NFactorial(int n )
        {
            if (n == 1)
            {
                return 1;
            }
            return n * NFactorial(n - 1);
        }
    }
}
