using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

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
