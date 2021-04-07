using System;

namespace BoxOfT
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            Box<int> box = new Box<int>();

            box.Add(1);
            box.Add(2);
            Console.WriteLine("Remove Method Here " + box.Remove());
            box.Add(3);
            box.Add(4);
            Console.WriteLine("Remove Method Here " + box.Remove());
            box.Add(5);
            box.Add(6);
            Console.WriteLine("Remove Method Here " + box.Remove());
            box.Add(7);

            box.PrintAll();
        }
    }
}
