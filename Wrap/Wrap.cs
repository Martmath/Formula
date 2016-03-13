using System;
using System.Reflection;

namespace Formula
{    
    ///<summary>Base class wrapper for casting to formula and give methods for converting and creating</summary> 
    public abstract class Wrap<T, A> where A : Wrap<T, A>
    {
        public enum WType { Val, Formula };
        protected static Type TAlive = typeof(Func<object>);
        protected static Type TT= typeof(T);  
        protected T Value;
        protected Func<object> Alive;
        public WType t;

        public override string ToString()
        {
            return "{Value="+Value.ToString() + " t=" +Enum.GetName(typeof(WType), t)+" Alive="+
              ((Alive == null)?"Null":"NOTNull"+"}");
        }
        ///<summary><para>
        ///Get data from "i" object to current obj use conversion operator and Convert.ChangeType
        ///</para><para>Not Use in conversion operator! Use simple Set</para></summary> 
        public bool SetuCO<B>(B i)
        {
            Type TA = this.GetType();
            Type TI = i.GetType(); object o; bool Res = true;
            if (TI == TAlive)
            {
                this.Alive = (System.Func<object>)(object)i;
                this.t = WType.Formula;
            }
            else if (TI == TT)
            {
                this.Value = (T)(object)i;
                this.t = WType.Val;
            }
            else if ((o = ExtC.DynamicCast(i, TA)) != null)
            {
                ExtC.CopyAll(o, this);
            }
            else if ((o = ExtC.DynamicCast(i, TT)) != null)
            {
                this.Value = (T)o;
                this.t = WType.Val;
            }
            else
            {
                try
                {
                    this.Value = (T)Convert.ChangeType(i, TT);
                    this.t = WType.Val;
                }
                catch (SystemException ex)
                {
                    try
                    {
                        ExtC.CopyAll(i, this);
                    }
                    catch (SystemException ex1)
                    {
                        Res = false;
                    }
                }
            }
            return Res;
        }
        ///<summary>Get clone obj casted to type of second level inheritor of Wrap type (A)</summary> 
        public A Copy()
        {
            Type TA = this.GetType();
            var R = (A)ExtI.GetInstance(TA);
            R.Value = this.Value;
            R.t = this.t;
            R.Alive = this.Alive;
            return R;
        }
        ///<summary>Get new obj initialized by "i" obj casted to type of second level inheritor of Wrap type (A)</summary> 
        public static A Factory<B>(B i, Type T)//T for Multilevel Inheritance
        {
            var R = (A)ExtI.GetInstance(T);
            if (!R.Set(i))
            {
                MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();             
                throw new System.InvalidOperationException("Incorrect Input Data in " + method.ReflectedType.Name + "." + method.Name);          
            }                 
            return R;
        }
        ///<summary>Breaks depending from formula with setting wrapped val from formula result</summary> 
        public bool ToStatic()
        {
            bool Res = false;
            if (this.t== WType.Formula)
            {
               Res= Set(this.Alive());              
            }
            return Res;
        }
        ///<summary>Get data from i object to current obj</summary> 
        public bool Set<B>(B i)
        {
            Type TI = typeof(B);   bool Res = true;
            if (TI == TAlive)
            {
                Alive = (System.Func<object>)(object)i;
                t = WType.Formula;
            }
            else if (TI == TT)
            {
                Value = (T)(object)i;
                t = WType.Val;
            }
            else
            {
                try
                {
                    Value = (T)Convert.ChangeType(i, TT);
                    t = WType.Val;
                }
                catch (SystemException ex)
                {
                    try
                    {
                        ExtC.CopyAll(i, this);
                    }
                    catch (SystemException ex1)
                    {
                        Res = false;
                    }
                }
            }
            return Res;
        }
        ///<summary> 
        ///<para>Get data to "i" object from current use conversion operator and Convert.ChangeType</para>
        ///<para>Not Use in conversion operator! Use simple Get</para>
        ///</summary>   
        public bool GetuCO<B>(ref B i)
        {
            object o; object oo; Type TI = typeof(B);
            if ((this.t == WType.Formula) && (TI == TAlive))
            {
                o = this.Alive;
            }
            else if ((this.t == WType.Formula) && (TI != TAlive))
            {
                o = this.Alive();
            }
            else if ((this.t == WType.Val) && (TI == TAlive))
            {
                o = (Func<object>)(() => this.Value);
            }
            else
            {
                o = this.Value;
            }
            bool Res = true;
            if ((TI == TAlive) || (TI == TT))
            {
                i = (B)o;
            }
            else if ((oo = ExtC.DynamicCast(this, TI)) != null)
            {
                i = (B)oo; // ExtC.CopyAll(this,i);
            }
            else if ((oo = ExtC.DynamicCast(o, TI)) != null)
            {
                i = (B)oo;          
            }
            else
            {
                try
                {
                    i = (B)Convert.ChangeType(o, TI);
                }
                catch (SystemException ex)
                {
                    try
                    {
                        ExtC.CopyAll(this,i);
                    }
                    catch (SystemException ex1)
                    {
                        Res = false;
                    }
                }
            }
            return Res;
        }
        ///<summary>Create B type obj from this</summary> 
        public B Get<B>()
        {
            B b = default(B);
            try {
                b = (B)ExtI.GetInstance(typeof(B));
            }
            catch (SystemException ex)
            { }
            Get<B>(ref b);
            return b;
        }
        ///<summary>Get data from this to "i" object</summary> 
        public bool Get<B>(ref B i)
        {
            object o; Type TI = typeof(B);
            if ((this.t == WType.Formula) && (TI == TAlive))
            {
                o = this.Alive;
            }
            else if ((this.t == WType.Formula) && (TI != TAlive))
            {
                o = this.Alive();
            }
            else if ((this.t == WType.Val) && (TI == TAlive))
            {                
                o = (Func<object>)(() => this.Value);
            }
            else
            {
                o = this.Value;
            }
            bool Res = true;
            if ((TI == TAlive)|| (TI == TT))
            {
                i =(B)o;            
            }
            else
            {
                try
                {
                    i = (B)Convert.ChangeType(o, TI);           
                }
                catch (SystemException ex)
                {
                    try
                    {
                        ExtC.CopyAll(this, i);
                    }
                    catch (SystemException ex1)
                    {
                        Res = false;
                    }
                }
            }
            return Res;
        }    
        public bool Equals(Wrap<T, A> p)
        {           
            if (p.t == this.t)
            {
                return p.GetHashCode() == this.GetHashCode();
            }
            return false;           
        }
        public override bool Equals(object obj)
        {
            Wrap<T, A> p = obj as Wrap<T, A>;
            if ((object)p == null)
            {
                return false;
            }
            return Equals(p);
        }
        public static bool operator ==(Wrap<T, A> a, Wrap<T, A> b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(Wrap<T, A> a, Wrap<T, A> b)
        {
            return !(a == b);
        }
        public override int GetHashCode()
        {           
            return (this.t == WType.Val) ? this.Value.GetHashCode():
                (this.Alive.GetUUID()!=0 ? this.Alive.GetUUID().GetHashCode() : this.Alive.Method.GetHashCode());            
        }  
    }
}
