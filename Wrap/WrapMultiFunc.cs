using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formula
{
    ///<summary>Add functionality for using some formulas (through L type labels)</summary> 
    public abstract class WrapMultiFuncService<T, L> : Wrap<T, WrapMultiFuncService<T, L>>
    {
        protected static ExtT.DictListValNullable<L, Func<object>> FList;
        static WrapMultiFuncService()
        {
            if (typeof(L) == typeof(T))
                throw new ArgumentException("Func indexer type and the wrapped type is same");
            if (Ext.IsNumericType(typeof(T)) && Ext.IsNumericType(typeof(L)))
                throw new ArgumentException("Func indexer type and wraping type are both numeric");
            FList = new ExtT.DictListValNullable<L, Func<object>>();
        }
                ///<summary> 
        ///<para>Add Func with label</para>
        ///<para>Dont use same label for different variable, if u dont want rewrite Func</para>
        ///</summary>   
        public void AddFunc(L Label, Func<object> f)
        {
            FList.Add(Label, f);
        }
        ///<summary>Del Func with label</summary> 
        public void DelFunc(L Label)
        {
            FList.Remove(Label);
        }
    }
    ///<summary>Number class sample with autoconvert to/from primitive type obj and multiformulas</summary> 
    public class WrapMultiFunc<BaseT> : WrapMultiFuncService<BaseT, string>
    {
        private string NameF = null;
        public WrapMultiFunc() { Value = default(BaseT); }
        public WrapMultiFunc(BaseT val) { Value = val; }

        static Type T = typeof(WrapMultiFunc<BaseT>);

        public static implicit operator int(WrapMultiFunc<BaseT> i)
        { return i.Get<int>(); }
        public static implicit operator WrapMultiFunc<BaseT>(int i)
        { return (WrapMultiFunc<BaseT>)WrapMultiFunc<BaseT>.Factory(i, T); }

        public static implicit operator long(WrapMultiFunc<BaseT> i)
        { return i.Get<long>(); }
        public static implicit operator WrapMultiFunc<BaseT>(long i)
        { return (WrapMultiFunc<BaseT>)WrapMultiFunc<BaseT>.Factory(i, T); }

        public static implicit operator double(WrapMultiFunc<BaseT> i)
        { return i.Get<double>(); }
        public static implicit operator WrapMultiFunc<BaseT>(double i)
        { return (WrapMultiFunc<BaseT>)WrapMultiFunc<BaseT>.Factory(i, T); }

        public static implicit operator decimal(WrapMultiFunc<BaseT> i)
        { return i.Get<decimal>(); }
        public static implicit operator WrapMultiFunc<BaseT>(decimal i)
        { return (WrapMultiFunc<BaseT>)WrapMultiFunc<BaseT>.Factory(i, T); }

        public static implicit operator char(WrapMultiFunc<BaseT> i)
        { return i.Get<string>()[0]; }
        public static implicit operator WrapMultiFunc<BaseT>(char i)
        { return (WrapMultiFunc<BaseT>)WrapMultiFunc<BaseT>.Factory(i, T); }

        public static implicit operator Func<object>(WrapMultiFunc<BaseT> i)
        { return i.Get<Func<object>>(); }
        public static implicit operator WrapMultiFunc<BaseT>(Func<object> i)
        { return (WrapMultiFunc<BaseT>)WrapMultiFunc<BaseT>.Factory(i, T); }

        public static implicit operator string(WrapMultiFunc<BaseT> i)
        {
            string Res = null;
            if (i.t == WType.Formula)
            {
                if ((i.NameF != null) && (i.NameF != ""))
                {
                    Res = i.NameF;
                }
                else
                {
                    ulong Hi = i.Alive.GetUUID();
                    if (Hi != 0)
                    {
                        var e = WrapMultiFunc<BaseT>.FList.FirstOrDefault(x => x.Item2.GetUUID() == Hi);
                        if ((object)e != null) Res = e.Item1;
                    }
                }
            }
            return Res;
        }
        public static implicit operator WrapMultiFunc<BaseT>(string i)
        {
            var I = (WrapMultiFunc<BaseT>)ExtI.GetInstance(T);
            if ((I.Alive = WrapMultiFunc<BaseT>.FList[i]) != null)
            {
                I.Set(I.Alive); I.NameF = i;
            }
            return I;
        }
    }
}
