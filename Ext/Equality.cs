using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Formula
{
    public class Equality
    {
        public List<List<string>> Map = new List<List<string>>();
        private bool _CheckT;
        private bool _NotuseMap;
        private bool _CheckCase;
        public Equality(bool CheckT = true, bool NotuseMap = true, bool CheckCase = true)
        {
            _CheckT = CheckT;
            _NotuseMap = NotuseMap;
            _CheckCase = CheckCase;
        }
        private class V
        {
            public string _Name;
            public Type _T;
            public object _O;
            public V(FieldInfo FI, object O)
            {
                _Name = FI.Name; _T = FI.FieldType; _O = FI.GetValue(O);
            }
            public V(string Name, Type T, object O)
            {
                _Name = Name; _T = T; _O = O;
            }
        }
        private static Type StringType = typeof(string);
        private static Type StringBuilderType = typeof(StringBuilder);
        private static Type DecimalType = typeof(Decimal);
        private Func<object, object, Type, Type, bool> _CheckPrimitiveEquals;
        private Func<Type, Type, bool> _CheckType;
        private Func<string, string, bool> _CheckName;
        private Func<string, string> _ConvertName;

        private List<List<string>> _MapinWork;
        private List<V> CloneListV(List<V> x)
        {
            var R = new List<V>();
            x.ForEach(y => R.Add(new V(_ConvertName(y._Name), y._T, y._O)));
            return R;
        }
        public bool EqualsDefault<A, B>(A a, B b)
        {
            return Equals<A, B>(a, b, _CheckT, _NotuseMap, _CheckCase);
        }
        public bool Equals<A, B>(A a, B b, bool CheckT = true, bool NotuseMap = true, bool CheckCase = true)
        {
            _CheckPrimitiveEquals = CheckT ? (Func<object, object, Type, Type, bool>)((a_, b_, x_, y_) => a_.Equals(b_)) :
                (a_, b_, x_, y_) => {
                    try
                    {
                        object r = null;
                        try { r = Convert.ChangeType(a_, y_); }
                        catch (Exception ex)
                        {   //if (!Ext.Cast(a_, y_, out r)) 
                            r = ExtC.DynamicCast(a, y_);
                        }
                        if (r == null) r = a_;
                        return r.Equals(b_);
                    }
                    catch (Exception ex2)
                    {
                        return false;
                    }
                };
            _CheckType = CheckT ? (Func<Type, Type, bool>)((x_, y_) => x_ == y_) : (x_, y_) => true;
            _ConvertName = (CheckCase) ? (Func<string, string>)(x_ => x_) : x_ => x_.ToUpper();

            if ((!NotuseMap) && (CheckCase)) _MapinWork = Map;
            else if (!NotuseMap)
            {
                _MapinWork = new List<List<string>>(Enumerable.Repeat(new List<string>(), Map.Count));
                for (int i = 0; i < _MapinWork.Count; i++)
                { Map[i].ForEach(x => _MapinWork[i].Add(x.ToUpper())); }
            }

            _CheckName = NotuseMap ? (Func<string, string, bool>)((x_, y_) => x_ == y_) :
                (x_, y_) =>
                {
                    if (x_ == y_) return true;
                    x_ = _ConvertName(x_); y_ = _ConvertName(y_);
                    var Dummy = _MapinWork.Where(x => x.Contains(x_));
                    return Dummy == null ? false : (Dummy.FirstOrDefault(x => x.Contains(y_)) != null);
                };

            return EqualsHelper(a, b);
        }
        private bool EqualsHelper<A, B>(A a, B b, object[] O = null)
        {
            try //cause for a limited number of situations
            {
                Type AT = a.GetType(); Type BT = b.GetType();
                if (((a == null) && (b == null)) || ((O != null) && (O.Contains(a)))) return true;
                if ((a == null) && (b != null)) return false;

                object[] Oo = null;
                if (O != null)
                {
                    Oo = new object[O.Length + 1];
                    O.CopyTo(Oo, 0);
                }
                else Oo = new object[1];

                Oo[Oo.Length - 1] = a; var y1 = a as IEnumerable; var y2 = b as IEnumerable;

                if (AT.Equals(StringBuilderType)) return _CheckPrimitiveEquals(a.ToString(), b.ToString(), AT, BT);

                else if (AT.IsGenericType && AT.GetGenericTypeDefinition() == typeof(Nullable<>))
                    //&& AT.GetGenericArguments().Any(t => t.IsValueType && t.IsPrimitive)))
                    return EqualsHelper(AT.GetMethod("GetValueOrDefault", new Type[0]).Invoke(a, null), b);
                else if (y1 != null)
                {
                    var E = y1.GetEnumerator(); var E1 = y2.GetEnumerator();
                    while ((E.MoveNext()) && (E1.MoveNext()))
                        if (!EqualsHelper(E.Current, E1.Current, Oo)) return false;
                    return true;
                }
                else if ((AT == StringType) || (AT == DecimalType) || (AT.IsPrimitive))
                    return _CheckPrimitiveEquals(a, b, AT, BT);
                else {
                    var t = a.GetType().GetFields(Ext.AllBF);
                    var t1 = b.GetType().GetFields(Ext.AllBF);
                    if ((t == t1) && (t == null)) return a.Equals(b);
                    List<V> al = t.Select(x => new V(x, a)).ToList();
                    List<V> bl = t1.Select(x => new V(x, b)).ToList();
                    List<V> aL = CloneListV(al); List<V> bL = CloneListV(bl);
                    for (int i = 0; i < aL.Count; i++)
                    {
                        V v = bl.FirstOrDefault(x => (al[i]._Name == x._Name) && (al[i]._T == x._T)) ?? //use special features if not found identical field
                            bL.FirstOrDefault(x => (_CheckName(aL[i]._Name, x._Name) && _CheckType(aL[i]._T, x._T)));
                        if (v != null)
                        {
                            if (((aL[i]._O == null) && (v._O == null)) || ((Oo != null) && (Oo.Contains(aL[i]._O)))) continue;
                            if (((aL[i]._O == null) && (v._O != null)) || (!EqualsHelper(aL[i]._O, v._O, Oo))) return false;
                        }
                        else return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private class TestEq1
        {
            public StringBuilder T;
            public string t;
            public Dictionary<string, int> d;
        }
        private class TestEq2
        {
            public List<string> a;
            public TestEq1 w;
            public long y;
            public TestEq2 q;
        }
        private class TestEq3
        {
            public List<string> a;
            public TestEq1 w;
            public decimal? y;
            public TestEq3 q;
        }        
        public void Test()
        {
            var q = new TestEq2();
            q.a = new List<string>();
            q.a.Add("xx");
            q.a.Add("yy");
            q.w = new TestEq1();
            q.w.T = new StringBuilder();
            q.w.T.Append("aa");
            q.w.t = "bb";
            q.w.d = new Dictionary<string, int>();
            q.w.d.Add("rr", 78);
            q.w.d.Add("frr", 778);
            q.y = 12;
            q.q = q;
            var q1 = new TestEq3();
            q1.a = new List<string>();
            q1.a.Add("xx");
            q1.a.Add("yy");
            q1.w = new TestEq1();
            q1.w.T = new StringBuilder();
            q1.w.T.Append("aa");
            q1.w.t = "bb";
            q1.w.d = new Dictionary<string, int>();
            q1.w.d.Add("rr", 78);
            q1.w.d.Add("frr", 778);
            q1.y = 12;
            q1.q = q1;
            bool eerr = q.w.T.Equals(q1.w.T);
            List<string> rr = new List<string> { "YY", "y" };
            this.Map.Add(rr);
            bool Res = Equals(q, q1, false, true, true);
        }
    }
}
