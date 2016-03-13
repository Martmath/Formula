using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Formula
{
    /// <summary>Additional Classes</summary>
    public static class ExtT
    {
        public class Pair<T1, T2>
        {
            public T1 Item1;
            public T2 Item2;
            public void SetData(T1 t1, T2 t2)
            {
                Item1 = t1;
                Item2 = t2;
            }
            public bool Equals(Pair<T1, T2> p)
            {
                return ((p.Item1.GetHashCode() == this.Item1.GetHashCode()) &&
                (p.Item2.GetHashCode() == this.Item2.GetHashCode()));
            }
            public override bool Equals(object obj)
            {
                Pair<T1, T2> p = obj as Pair<T1, T2>;
                if ((object)p == null) return false;
                return Equals(p);
            }
            public static bool operator ==(Pair<T1, T2> a, Pair<T1, T2> b)
            {
                return a.Equals(b);
            }
            public static bool operator !=(Pair<T1, T2> a, Pair<T1, T2> b)
            {
                return !(a == b);
            }
            public override int GetHashCode()
            {
                int Res = this.Item1.GetHashCode();
                Res = Res + (Res % 100) + this.Item2.GetHashCode();
                return Res;
            }
        }
        public class DictListValNullable<TLabel, TValue> : DictList<TLabel, TValue>//where class not allow int?
        {
            static DictListValNullable()
            {
                var DValue = default(TValue);
                if (DValue != null)
                {
                    MethodBase method = MethodBase.GetCurrentMethod();
                    throw new ArgumentException("Value Type isn't Nullable in" +
                        method.ReflectedType.Name + "." + method.Name);
                }
            }
        }
        public class DictList<TLabel, TValue> : List<ExtT.Pair<TLabel, TValue>>//KeyValuePair and tuple not mutable
        {
            public static DictList<TLabel, TValue> DefaultData;
            public static TValue DefValue;
            static DictList()
            {
                DefValue = default(TValue);
                DefaultData = new DictList<TLabel, TValue>();
            }
            public TValue this[TLabel label]
            {
                get
                {
                    int e = this.FindIndex(x => x.Item1.Equals(label));
                    return (e > -1) ? this[e].Item2 : DefValue;
                }

                set
                {
                    int e = this.FindIndex(x => x.Item1.Equals(label));
                    if (e > -1) this[e].Item2 = value;
                    else this.Add(label, value);
                }
            }
            public void Add(TLabel label, TValue PairHCaF)
            {
                int e = this.FindIndex(x => x.Item1.Equals(label));
                if (e > -1)
                    this[e].Item2 = PairHCaF;
                else
                    base.Add(new ExtT.Pair<TLabel, TValue>() { Item1 = label, Item2 = PairHCaF });
            }
            public void Remove(TLabel Label)
            {
                if (this != null)
                {
                    int e = this.FindIndex(x => x.Item1.Equals(Label));
                    if (e > -1) base.RemoveAt(e);
                }
            }
            public void Remove(TValue Value)
            {
                if (this != null)
                {
                    int e = this.FindIndex(x => x.Item2.Equals(Value));
                    if (e > -1) base.RemoveAt(e);
                }
            }
        }
    }
}
