using System;

namespace RecusrionExercise
{
    public class Program
    {
        static void Main(string[] args)
        {
            int[] array = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            Console.WriteLine(Sum(array));
  
        }

        static int Sum(int[] array, int index = 0)
        {
            if (index == array.Length)
            {
                return 0;
            }

            return array[index] + Sum(array, index + 1);

        }
    }
}
