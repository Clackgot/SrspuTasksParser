using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PageParser
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = new TestModel("index.html");
            Console.WriteLine(test.Author);
        }
    }
}
