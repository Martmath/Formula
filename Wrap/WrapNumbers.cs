using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Formula
{
    ///<summary>Number class sample with autoconvert to/from primitive type obj and formula</summary> 
    public class WrapNumbers<BaseT> : Wrap<BaseT, WrapNumbers<BaseT>>
    {
        public WrapNumbers() { Value = default(BaseT); }
        public WrapNumbers(BaseT val) { Value = val; }

        static Type T;
        static WrapNumbers()
        {
            if (!Ext.IsNumericType(typeof(BaseT)))
            {
                MethodBase method = MethodBase.GetCurrentMethod();
                throw new ArgumentException("Main Type isn't numeric in" +
                    method.ReflectedType.Name + "." + method.Name);
            }
            T = typeof(WrapNumbers<BaseT>);
        }
        public static implicit operator int(WrapNumbers<BaseT> i)
        { return i.Get<int>(); }
        public static implicit operator WrapNumbers<BaseT>(int i)
        { return WrapNumbers<BaseT>.Factory(i); }

        public static implicit operator long(WrapNumbers<BaseT> i)
        { return i.Get<long>(); }
        public static implicit operator WrapNumbers<BaseT>(long i)
        { return WrapNumbers<BaseT>.Factory(i); }

        public static implicit operator double(WrapNumbers<BaseT> i)
        { return i.Get<double>(); }
        public static implicit operator WrapNumbers<BaseT>(double i)
        { return WrapNumbers<BaseT>.Factory(i); }

        public static implicit operator decimal(WrapNumbers<BaseT> i)
        { return i.Get<decimal>(); }
        public static implicit operator WrapNumbers<BaseT>(decimal i)
        { return WrapNumbers<BaseT>.Factory(i); }

        public static implicit operator char(WrapNumbers<BaseT> i)
        { return i.Get<string>()[0]; }
        public static implicit operator WrapNumbers<BaseT>(char i)
        { return WrapNumbers<BaseT>.Factory(i.ToString()); }

        public static implicit operator string(WrapNumbers<BaseT> i)
        { return i.Get<string>(); }
        public static implicit operator WrapNumbers<BaseT>(string i)
        { return WrapNumbers<BaseT>.Factory(i); }

        public static implicit operator Func<object>(WrapNumbers<BaseT> i)
        { return i.Get<Func<object>>(); }
        public static implicit operator WrapNumbers<BaseT>(Func<object> i)
        { return WrapNumbers<BaseT>.Factory(i); }
    }
}
