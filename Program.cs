using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Formula
{
    class Program
    {
        static void Main(string[] args)
        {
            //(new Equality()).Test();
            //Ext.Test();
            Test T = new Test();
            var M = T.GetType().GetMethods(Ext.AllBF).Where(x => x.Name.IndexOf("TestRun") == 0).ToList();
            M.ForEach(x => x.Invoke(T, null));
        }
    }
}
