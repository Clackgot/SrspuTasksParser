using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
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
            ScriptEngine engine = Python.CreateEngine();

            var searchPaths = engine.GetSearchPaths();
            searchPaths.Add("../../../PageDownloader/env/Lib");
            engine.SetSearchPaths(searchPaths);

            engine.ExecuteFile("../../../PageDownloader/PageDownloader.py");
            var pageTest = new PageTest("../../index.html");

        }
    }
}
