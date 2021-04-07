using System;

namespace GenericScale
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            Console.WriteLine(new EqualityScale<int>(5, 5).AreEqual());
        }

    }
}
