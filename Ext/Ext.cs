using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Formula
{
    

    public static class Ext
    {
        public static bool IsNumericType(Type o)
        {
            switch (Type.GetTypeCode(o))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }        
        public const BindingFlags AllNotStaticBF = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        public const BindingFlags AllBF = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        public static T ToEnum<T>(string value)
        { return (T)Enum.Parse(typeof(T), value, true); }
        public static string FromEnum<T>(T value)
        { return Enum.GetName(typeof(T), value); }
        private static T Cast<T>(this object o)
        {
            T d = default(T);
            try
            {
                d = (T)o;
            }
            catch (Exception ex)
            {
            }
            return d;
        }
        #region Maybenextarticle
        #region Sorting by some fields
        private class J<A, B> : Dictionary<A, B> where B :class
        {
            public new B this[A key]
            {
                get
                {
                    B Val;
                    return this.TryGetValue(key, out Val) ? Val : (B)ExtI.GetInstance(typeof(B));
                }
                private set
                {
                    this[key] = value;
                }
            }
            public override string ToString()
            {

                string R = "";
                foreach (KeyValuePair<A, B> E in this)
                {
                    R = R + "(" + E.Key.ToString() + ")" + E.Value.ToString();
                }
                return R;
            }
        }
        private class NotErrorList<T> : List<T>
        {
            public new T this[int i]
            {
                get
                {
                    if (this== null) return default(T);
                    if (i > base.Count - 1)  return default(T);  else return base[i];
                }
                set
                {
                    if (i <= this.Count - 1)
                    {
                        base[i] = value;
                    }
                    
                }
            }
        }
        private class M
        {
            public int I;            
            public char C;       
        }
        private class N
        {
            public M m;   
            public double D;
            public string S;
            public string SR; 
            public N (int i,char c,double d,string s,string sr)
            {
                m = new M() { I = i, C = c };
                D = d;
                S = s;
                SR = sr;
            }         
        }
        private static void CheckSort()
        {
            J<int, J<char, J<double, J<string, NotErrorList<string>>>>> R;
            J<int, J<char,  NotErrorList<N>>> R2;
            List<N> L = new List<N>();
            L.Add(new N(1,'A',1.1,"one","Res1"));
            L.Add(new N(1, 'A', 1.1, "one", "Res2"));
            L.Add(new N(2, 'A', 1.1, "one", "Res3"));
            L.Add(new N(2, 'B', 2.1, "one", "Res4"));
            L.Add(new N(2, 'B', 2.1, "one", "Res5"));
            Sort(L, out R, x => x.m.I, x => x.m.C, x => x.D, x => x.S, x => x.SR);
            Sort(L, out R2, x => x.m.I, x => x.m.C,  x => x);
            var Res=R[1]['A'][1.1]["one"][1];
            var Res2 = R[1]['A'][1.1]["two"][3];
            var Res3 = R2[2]['B'].Count;
        }
        private static void Sort<A, D>(List<A> a, out D d, params Func<A, object>[] b)
        {
            d = (D)ExtI.GetInstance(typeof(D));
            var cc = b.ToList().GetRange(1, b.Count() - 1);
            foreach (A aa in a)
            {
                object HH = d.TryGetorCreate(b[0](aa));
                cc.ForEach(x => HH = HH.TryGetorCreate(x(aa)));
            }
        }
        private static object TryGetorCreate(this object a, object b)
        {
            IDictionary myTest = a as IDictionary;
            object Res = null;
            if (myTest != null)
            {
                if (myTest.Contains(b)) Res = myTest[b];
                else
                {
                    Res = ExtI.GetInstance(myTest.GetType().GetGenericArguments()[1]);
                    myTest.Add(b, Res);
                }
            }
            else
            {
                IList myTest2 = a as IList;
                if (myTest2 != null)
                {
                    Res = b;
                    if (!myTest2.Contains(b)) myTest2.Add(b);
                }
            }
            return Res;
        }
        #endregion
        #region Changeable foreach
        private static void ForEachChange<T>(this List<T> array, Action<List<T>, int> mutator, Func<List<T>, int, bool> predicate = null)
        {
            int i = 0;
            Action Mutator = predicate == null ? () => mutator(array, i) :
                new Action(() => { if (predicate(array, i)) mutator(array, i); });
            for (i = 0; i < array.Count(); i++) Mutator();
            
        }
        private static object ForEachChange<T>(this List<T> array,Type TT, Func<List<T>, int, object, object> mutator, Func<object> First=null, Func<List<T>, int, bool> predicate = null)
        {
            First = First ?? (() => null);
            Func<List<T>, int, object, object> Mutator = (predicate == null) ? mutator : (x, y, z) => {
                if (predicate(x, y)) return mutator(x, y, z); else return z;
            };
            var s = ExtI.GetInstance(TT);
            for (int i = 0; i < array.Count(); i++) s = Mutator(array, i, s);
            return s;
        }
        private static B ForEachChange<T, B>(this List<T> array,  Func<List<T>, int, B, B> mutator, Func<B> First=null, Func<List<T>, int, bool> predicate = null)
        {
            First = First??(() => default(B));
            B s = First(); int i;
            Func<List<T>, int, B, B> Mutator = (predicate == null)? mutator: (x, y, z) => {
                if (predicate(x, y))  return mutator(x, y, z); else return z;
            };
            //Func<B> Mutator = (predicate == null) ? () => mutator(array, i, s) : (Func<B>)(() =>
            //{ if (predicate(array, i)) return mutator(array, i, s); else return s; });
            for (i = 0; i < array.Count(); i++)  s = Mutator(array, i, s);   //Mutator();            
            return s;
        }
        private static void ForEachChange<T>(this T[] array, Action<T[], int> mutator, Func<T[], int, bool> predicate = null)
        {
            int i = 0;
            Action Mutator = predicate == null ? () => mutator(array, i) :
                new Action(() => { if (predicate(array, i)) mutator(array, i); });
            for (i = 0; i < array.Count(); i++) Mutator();

        }
        private static object ForEachChange<T>(this T[] array, Type TT, Func<T[], int, object, object> mutator, Func<object> First = null, Func<T[], int, bool> predicate = null)
        {
            First = First ?? (() => null);
            Func<T[], int, object, object> Mutator = (predicate == null) ? mutator : (x, y, z) => {
                if (predicate(x, y)) return mutator(x, y, z); else return z;
            };
            var s = ExtI.GetInstance(TT);
            for (int i = 0; i < array.Count(); i++) s = Mutator(array, i, s);
            return s;
        }
        private static B ForEachChange<T, B>(this T[] array, Func<T[], int, B, B> mutator, Func<B> First = null, Func<T[], int, bool> predicate = null)
        {
            First = First ?? (() => default(B));
            B s = First(); int i;
            Func<T[], int, B, B> Mutator = (predicate == null) ? mutator : (x, y, z) => {
                if (predicate(x, y)) return mutator(x, y, z); else return z;
            };           
            for (i = 0; i < array.Count(); i++) s = Mutator(array, i, s);          
            return s;
        }
        #endregion        
        public static void Test()
        {
            CheckSort();
            List<double> L = new List<double>{ 1.1, 2.1, 3.3 };          
            double R0 = L.ForEachChange((a, i, c) => c = a[i] * (i+1) + c, () => 1.0);
            double R1 = L.ForEachChange((a, i, c) => c = a[i] + c, () => 0.0);
            double R2 = L.ForEachChange((a, i, c) => c = a[i] * c, () => 1.0);
            double R3= L.ForEachChange((a, i, c) => c = a[i] + c, () => 0.0, (a, i) => (a[i] < 2.0)|| (a[i] > 3.0));
            double R4 = (double)L.ForEachChange(typeof(double),(a, i, c) => c = a[i] + (double)c, () => 0, (a, i) => (a[i] < 2) || (a[i] > 3));
            long[] A1 = new long[] { 2, 4, 6 };
            List<long> L1 = new List<long> { 2, 4, 6 };
            L1.ForEachChange((a, i) => a[i] = i, (a, i) => a[i] < 5);
            A1.ForEachChange((a, i) => a[i]=a[i] + 1, (a, i) => a[i] < 5);      
        }
        #endregion
    }
}
