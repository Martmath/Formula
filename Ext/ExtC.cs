using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Formula
{
    /// <summary>Casting and converting type</summary>
    class ExtC
    {
        /// <summary>Use casting source var to destT type with existing conversion operators</summary>
        public static bool DynamicCast<B>(B source, Type destT, out object Res)
        {
            //http://stackoverflow.com/questions/7351289/how-do-i-perform-explicit-operation-casting-from-reflection
            Type srcT = source.GetType();
            if (srcT == destT) { Res = source; return true; }
            Res = null;
            var bf = BindingFlags.Static | BindingFlags.Public;
            MethodInfo castOp = destT.GetMethods(bf).Union(srcT.GetMethods(bf)).FirstOrDefault(M => {
                if (((M.Name != "op_Explicit") && (M.Name != "op_Implicit")) || (M.ReturnType != destT)) return false;
                var ps = M.GetParameters();
                return ps.Length == 1 && ps[0].ParameterType == srcT;
            });
            if (castOp != null) Res = castOp.Invoke(null, new object[] { source });
            else return false;
            return true;
        }
        /// <summary>Return destT type obj from source var through using existing conversion operators</summary>
        public static object DynamicCast<B>(B source, Type destType)
        {
            if (source == null) return null;
            object o = ExtI.GetInstance(destType);
            return DynamicCast(source, destType, out o) ? o : null;
        }
        /// <summary> Checks for at least one match in the fields</summary>
        public static bool CheckT(Type source, Type target)
        {
            var Data = source.GetFields(Ext.AllBF).Select(x =>
              new ExtT.Pair<Type, string>() { Item1 = x.FieldType, Item2 = x.Name });
            var Dummy = new ExtT.Pair<Type, string>();
            return target.GetFields(Ext.AllBF).FirstOrDefault(x => {
                Dummy.SetData(x.FieldType, x.Name);
                return Data.Contains(Dummy);
            }) != null;

        }
        /// <summary> Cope coincidental fields from source to target obj</summary>
        public static void CopyAll<B, C>(B source, C target)
        {
            Type T = typeof(C);
            Type T1 = typeof(B);
            object o; FieldInfo targetF;
            /*PropertyInfo targetP;
            foreach (var sourceP in T.GetProperties(Ext.AllBF))
            {
                targetP = T1.GetProperty(sourceP.Name);                
                if ((targetP != null)&& (targetP.PropertyType == sourceP.PropertyType))
                {
                    o = sourceP.GetValue(source, null);
                    targetP.SetValue(target, o, null);
                }
            }*/
            foreach (var sourceF in T.GetFields(Ext.AllBF))
            {
                targetF = T.GetField(sourceF.Name, Ext.AllBF);
                if ((targetF != null) && (targetF.FieldType == sourceF.FieldType))
                {
                    o = sourceF.GetValue(source);
                    targetF.SetValue(target, o);
                }
            }
        }
    }
}
