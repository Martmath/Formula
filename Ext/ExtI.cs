using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Formula
{
    /// <summary>Create Instance of everything</summary>
    public static class ExtI
    {      
        public static object GetInstance(Type T)
        {
            object Result = null;
            T = WithoutRef(T);

            if (T.IsValueType)
            {
                Result = Activator.CreateInstance(T);
            }
            else if (T.Name == "String")
                Result = String.Empty;
            else
            {
                ConstructorInfo C = T.GetConstructor
                (Ext.AllNotStaticBF, null, Type.EmptyTypes, null);
                if (C != null) Result = C.Invoke(null);
                else
                {
                    ConstructorInfo[] CC = T.GetConstructors(); int i = 0;
                    while ((i < CC.Length) && (!ItGoodConstructor(CC[i]))) { i = i + 1; }
                    if (i < CC.Length) Result = ObjectFromConstructor(CC[i]);

                    if ((Result == null) && (CC.Length > 0))
                    {
                        ParameterInfo[] ps = CC[0].GetParameters();
                        List<object> o = new List<object>();
                        object O = null;
                        for (i = 0; i < ps.Length; i++)
                        {
                            O = GetInstance(ps[i].ParameterType);
                            if (O == null) break;
                            o.Add(O);
                        }
                        if (o.Count == ps.Length)
                            try {
                                Result = CC[0].Invoke(o.ToArray());
                            }
                            catch (Exception ex) { }
                    }
                }
            }
            return Result;
        }
        private static Type WithoutRef(Type T)
        {
            if (T.Name[T.Name.Length - 1] == '&') T = Type.GetType(T.FullName.Substring(0, T.FullName.Length - 1));
            return T;
        }
        private static bool ItGoodConstructor(ConstructorInfo C)
        {
            ParameterInfo[] ps = C.GetParameters();
            var cs = from p in ps
                     let prms = p.ParameterType.GetConstructor
                     (Ext.AllNotStaticBF, null, Type.EmptyTypes, null)
                     let pms = p.ParameterType.IsValueType
                     where ((prms != null) || pms)
                     select p;
            return (cs.Count() == ps.Length);
        }
        private static object ObjectFromConstructor(ConstructorInfo C)
        {
            ParameterInfo[] ps = C.GetParameters();
            List<object> o = new List<object>();
            for (int i = 0; i < ps.Length; i++)
            {
                if (ps[i].ParameterType.IsValueType) o.Add(Activator.CreateInstance(ps[i].ParameterType));
                else
                    o.Add(ps[i].ParameterType.GetConstructor
                         (Ext.AllNotStaticBF, null, Type.EmptyTypes, null).Invoke(null));
            }
            object Res = null;
            try {
                Res = C.Invoke(o.ToArray());
            }
            catch (Exception ex) { }
            return Res;
        }
    }
}
