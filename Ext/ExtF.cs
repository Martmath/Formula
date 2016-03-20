using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Formula
{
    /// <summary>Helper for Func UUID</summary>
    public static class FuncUUID
    {
        private static ExtT.DictList<object, ulong> FHCatche = new ExtT.DictList<object, ulong>();

        public static ulong GetUUID<T>(this Func<T> F) 
        {
            return  FHCatche[F];
        }
        public static void SetUUID<T>(this Func<T> F,ulong D) 
        {
            FHCatche[F]=D;
        }
    }
    /// <summary>Anything to Func</summary>
    public abstract class ExtF<T> 
    {  
        protected static A CT<A>(object o)
        {
            if (o is A) return (A)o;
            try
            {
                // IConvertible c = o as IConvertible; if (c!= null)
                return (A)Convert.ChangeType(o, typeof(A));
            }
            catch (SystemException ex)
            {
                MethodBase method = MethodBase.GetCurrentMethod();
                throw new InvalidOperationException("Error convert in " + method.ReflectedType.Name + "." + method.Name);
            }//return default(A);
        }
        #region MakeFunc
            /// <summary>Make Func from Action</summary>
            public static Func<T> MakeFunc(Action action)
            { return () => { action(); return default(T); }; }
            public static Func<A, T> MakeFunc<A>(Action<A> action)
            { return (a) => { action(a); return default(T); }; }
            public static Func<A, B, T> MakeFunc<A, B>(Action<A, B> action)
            { return (a, b) => { action(a, b); return default(T); }; }
            public static Func<A, B, C, T> MakeFunc<A, B, C>(Action<A, B, C> action)
            { return (a, b, c) => { action(a, b, c); return default(T); }; }
            public static Func<A, B, C, D, T> MakeFunc<A, B, C, D>(Action<A, B, C, D> action)
            { return (a, b, c, d) => { action(a, b, c, d); return default(T); }; }
            public static Func<A, B, C, D, F, T> MakeFunc<A, B, C, D, F>(Action<A, B, C, D, F> action)
            { return (a, b, c, d, f) => { action(a, b, c, d, f); return default(T); }; }
            public static Func<A, B, C, D, F, G, T> MakeFunc<A, B, C, D, F, G>(Action<A, B, C, D, F, G> action)
            { return (a, b, c, d, f, g) => { action(a, b, c, d, f, g); return default(T); }; }
            public static Func<A, B, C, D, F, G, H, T> MakeFunc<A, B, C, D, F, G, H>(Action<A, B, C, D, F, G, H> action)
            { return (a, b, c, d, f, g, h) => { action(a, b, c, d, f, g, h); return default(T); }; }
        #endregion
        #region FromFunc
            private static ulong ResHC(params int[] i) {
                ulong r = 0;
                for(int j=0;j<i.Length;j++)  r = r + (ulong)(i[j] * (j + 1));        
                return r;
            }
            private static int HC<A>(Func<A> a,bool Hardcore=false) {
                if (Hardcore)
                {
                    ulong R = 0; ulong MaxR = ulong.MaxValue-255;
                    a.Method.GetMethodBody().GetILAsByteArray().ToList().ForEach(x => {
                        R = R + (ulong)x;if (R > MaxR) R = 0; });
                    return R.GetHashCode();
                }
                return a.Method.GetHashCode();
            }
            private static string StrHc<A>(Func<A> a) {
                return a.Method.ToString();
            }
            private static string StrSh(params string[] i) {
                string r = "";
                for (int j = 0; j < i.Length; j++) r = r + i[j];
                return r;
            }

            ///<summary>Generate Func from method with create uuid<summary>
            public static Func<T> FromFunc<R>(Func<R> r, bool hr = false, bool SetHC = true) {
                Func<T> FT = () => CT<T>(r());
                if (SetHC) FT.SetUUID(ResHC(r.Method.GetHashCode()));
                return FT;
            }
            public static Func<T> FromFunc<A, R>(Func<A, R> r, Func<A> a, bool hr = false, bool SetHC = true) {
                Func<T> FT = () => CT<T>(r(a()));
                if (SetHC) FT.SetUUID(ResHC(r.Method.GetHashCode(), HC(a, hr)));
                return FT;          
            }
            public static Func<T> FromFunc<A, B, R>(Func<A, B, R> r, Func<A> a, Func<B> b, bool hr = false, bool SetHC = true) {
                Func<T> FT = () => CT<T>(r(a(), b()));
                if (SetHC) FT.SetUUID(ResHC(r.Method.GetHashCode(), HC(a, hr), HC(b, hr)));
                return FT;      
            }
            public static Func<T> FromFunc<A, B, C, R>(Func<A, B, C, R> r, Func<A> a, Func<B> b, Func<C> c, bool hr = false, bool SetHC = true) {
                Func<T> FT = () => CT<T>(r(a(), b(), c()));
                if (SetHC) FT.SetUUID(ResHC(r.Method.GetHashCode(), HC(a, hr), HC(b, hr), HC(c, hr)));
                return FT;           
            }
            public static Func<T> FromFunc<A, B, C, D, R>(Func<A, B, C, D, R> r, 
                Func<A> a, Func<B> b, Func<C> c, Func<D> d, bool hr = false, bool SetHC = true) {
                Func<T> FT = () => CT<T>(r(a(), b(), c(), d()));
                if (SetHC) FT.SetUUID(ResHC(r.Method.GetHashCode(), HC(a, hr), HC(b, hr), HC(c, hr), HC(d, hr)));
                return FT;           
            }
            public static Func<T> FromFunc<A, B, C, D, F, R>(Func<A, B, C, D, F, R> r, 
                Func<A> a, Func<B> b, Func<C> c, Func<D> d, Func<F> f, bool hr = false, bool SetHC = true) {
                Func<T> FT = () => CT<T>(r(a(), b(), c(), d(), f()));
                if (SetHC) FT.SetUUID(ResHC(r.Method.GetHashCode(), HC(a, hr), HC(b, hr), HC(c, hr), HC(d, hr), HC(f, hr)));
                return FT;
            }
            public static Func<T> FromFunc<A, B, C, D, F, G, R>(Func<A, B, C, D, F, G, R> r,
                Func<A> a, Func<B> b, Func<C> c, Func<D> d, Func<F> f, Func<G> g, bool hr = false, bool SetHC = true)   {
                Func<T> FT = () => CT<T>(r(a(), b(), c(), d(), f(), g()));
                if (SetHC) FT.SetUUID(ResHC(r.Method.GetHashCode(), HC(a, hr), HC(b, hr), HC(c, hr), HC(d, hr), HC(f, hr), HC(g, hr)));
                return FT;
            }
            public static Func<T> FromFunc<A, B, C, D, F, G, H, R>(Func<A, B, C, D, F, G, H, R> r,
                Func<A> a, Func<B> b, Func<C> c, Func<D> d, Func<F> f, Func<G> g, Func<H> h, bool hr = false, bool SetHC = true) {
                Func<T> FT = () => CT<T>(r(a(), b(), c(), d(), f(), g(), h()));
                if (SetHC) FT.SetUUID(ResHC(r.Method.GetHashCode(), HC(a, hr), HC(b, hr), HC(c, hr), HC(d, hr), HC(f, hr), HC(g, hr), HC(h, hr)));
                return FT;
             }
        #endregion
        #region FromAction
        /// <summary>Generate Func from method with create uuid<summary>
        public static Func<T> FromAction(Action r, bool hr = false, bool SetHC = true)
        {
            Func<T> FT = () => MakeFunc(r)();
            if (SetHC) FT.SetUUID(ResHC(r.Method.GetHashCode()));
            return FT;
            }
        public static Func<T> FromAction<A>(Action<A> r, Func<A> a, bool hr = false, bool SetHC = true)
        {
            Func<T> FT = () => MakeFunc<A>(r)(a());
            if (SetHC) FT.SetUUID(ResHC(r.Method.GetHashCode(), HC(a, hr)));
            return FT;
        }
        public static Func<T> FromAction<A, B>(Action<A, B> r, Func<A> a, Func<B> b, bool hr = false, bool SetHC = true)
        {
            Func<T> FT = () => MakeFunc<A, B>(r)(a(), b());/*{ r(a(), b()); return null; };*/
            if (SetHC) FT.SetUUID(ResHC(r.Method.GetHashCode(), HC(a, hr), HC(b, hr)));
            return FT;   
        }
        public static Func<T> FromAction<A, B, C>(Action<A, B, C> r, Func<A> a, Func<B> b, Func<C> c, bool hr = false, bool SetHC = true)
        {
            Func<T> FT = () => MakeFunc<A, B, C>(r)(a(), b(), c());
            if (SetHC) FT.SetUUID(ResHC(r.Method.GetHashCode(), HC(a, hr), HC(b, hr), HC(c, hr)));
            return FT;
            }
        public static Func<T> FromAction<A, B, C, D>(Action<A, B, C, D> r, Func<A> a, Func<B> b, Func<C> c, Func<D> d, bool hr = false, bool SetHC = true)
        {
            Func<T> FT = () => MakeFunc<A, B, C, D>(r)(a(), b(), c(), d());
            if (SetHC) FT.SetUUID(ResHC(r.Method.GetHashCode(), HC(a, hr), HC(b, hr), HC(c, hr), HC(d, hr)));
            return FT;
             }
        public static Func<T> FromAction<A, B, C, D, F>(Action<A, B, C, D, F> r, 
        Func<A> a, Func<B> b, Func<C> c, Func<D> d, Func<F> f, bool hr = false, bool SetHC = true)
        {
            Func<T> FT = () => MakeFunc<A, B, C, D, F>(r)(a(), b(), c(), d(), f());
            if (SetHC) FT.SetUUID(ResHC(r.Method.GetHashCode(), HC(a, hr), HC(b, hr), HC(c, hr), HC(d, hr), HC(f, hr)));
            return FT;
             }
        public static Func<T> FromAction<A, B, C, D, F, G>(Action<A, B, C, D, F, G> r,
        Func<A> a, Func<B> b, Func<C> c, Func<D> d, Func<F> f, Func<G> g, bool hr = false, bool SetHC = true)
        {
            Func<T> FT = () => MakeFunc<A, B, C, D, F, G>(r)(a(), b(), c(), d(), f(), g());
            if (SetHC) FT.SetUUID(ResHC(r.Method.GetHashCode(), HC(a, hr), HC(b, hr), HC(c, hr), HC(d, hr), HC(f, hr), HC(g, hr)));
            return FT;
        }
        public static Func<T> FromAction<A, B, C, D, F, G, H>(Action<A, B, C, D, F, G, H> r,
        Func<A> a, Func<B> b, Func<C> c, Func<D> d, Func<F> f, Func<G> g, Func<H> h, bool hr = false, bool SetHC = true) {
            Func<T> FT = () => MakeFunc<A, B, C, D, F, G, H>(r)(a(), b(), c(), d(), f(), g(), h());
            if (SetHC) FT.SetUUID(ResHC(r.Method.GetHashCode(), HC(a, hr), HC(b, hr), HC(c, hr), HC(d, hr), HC(f, hr), HC(g, hr), HC(h, hr)));
            return FT;
        }
        #endregion
        #region  DieGeheimeEntwicklungDerBundeswehr   
            #region FuncAction
                public static Func<object[], T> FuncAction(Action a)
                { return (x) => MakeFunc(a)(); }
                public static Func<object[], T> FuncAction<A>(Action<A> a)
                { return (x) => MakeFunc(a)(CT<A>(x[0])); }
                public static Func<object[], T> FuncAction<A, B>(Action<A, B> a)
                { return (x) => { a(CT<A>(x[0]), CT<B>(x[1])); return default(T); }; }
                public static Func<object[], T> FuncAction<A, B, C>(Action<A, B, C> a)
                { return (x) => MakeFunc<A, B, C>(a)(CT<A>(x[0]), CT<B>(x[1]), CT<C>(x[2])); }
            #endregion
            #region FuncFunc
                public static Func<object[], T> FuncFunc<R>(Func<R> a)
                { return (x) => CT<T>(a()); }
                public static Func<object[], T> FuncFunc<A,R>(Func<A, R> a)
                { return (x) => CT<T>(a(CT<A>(x[0]))); }
                public static Func<object[], T> FuncFunc<A, B,R>(Func<A, B, R> a)
                { return (x) => CT<T>(a(CT<A>(x[0]), CT<B>(x[1]))); }
                public static Func<object[], T> FuncFunc<A, B, C,R>(Func<A, B, C, R> a)
                { return (x) => CT<T>(a(CT<A>(x[0]), CT<B>(x[1]), CT<C>(x[2]))); }
        #endregion
            public delegate T ActionPar(params object[] args);
            public static object[] ACopy(params object[] o)
            {
                if (o == null) return new object[0];
                var r = new object[o.Length]; o.CopyTo(r, 0);
                return r;
            }
            #region DelegAction
                public static ActionPar DelegAction(Action a)
                { return (x) => FuncAction(a)(new object[0]); }
                public static ActionPar DelegAction<A>(Action<A> a)
                { return (x) => FuncAction<A>(a)(ACopy(x[0])); }
                public static ActionPar DelegAction<A, B>(Action<A, B> a)
                { return (x) => FuncAction<A, B>(a)(ACopy(x[0], x[1])); }
                public static ActionPar DelegAction<A, B, C>(Action<A, B, C> a)
                { return (x) => FuncAction<A, B, C>(a)(ACopy(x[0], x[1], x[2])); }
            #endregion
            #region DelegFunc
                public static ActionPar DelegFunc<R>(Func<R> a)
                { return (x) => FuncFunc(a)(ACopy()); }
                public static ActionPar DelegFunc<A,R>(Func<A, R> a)
                { return (x) => FuncFunc<A,R>(a)(ACopy(x[0])); }
                public static ActionPar DelegFunc<A, B,R>(Func<A, B, R> a)
                { return (x) => FuncFunc<A, B,R>(a)(ACopy(x[0], x[1])); }
                public static ActionPar DelegFunc<A, B, C,R>(Func<A, B, C, R> a)
                { return (x) => FuncFunc<A, B, C,R>(a)(ACopy(x[0], x[1], x[2])); }

        public static Func<A, bool, B, bool, C, bool, R> FuncwithDefault<A, B, C, R>(Func<A, B, C, R> f, A a=default(A), B b = default(B), C c = default(C))
        {
            Func<A,bool, B,bool, C,bool, R> _f = (a_,Da, b_,Db, c_,Dc) =>
            {      
                A _a = Da? a:a_; B _b = Db ? b : b_; C _c = Dc ? c : c_;
                return f(_a, _b,_c);
            };
            return _f;
        }     

        #endregion
        #endregion
    }
    public abstract class ExtFO : ExtF<object> { }
}
