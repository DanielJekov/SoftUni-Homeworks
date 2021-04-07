using System;
using CustomLinkedList;

namespace UsingCustomLinkedList
{
    public class Program
    {
        static void Main(string[] args)
        {
            LinkedList list = new LinkedList();
            for (int i = 0; i < 10; i++)
            {
                list.AddFirst(new Node(i));
            }

            list.PrintList();
        }
    }
}
