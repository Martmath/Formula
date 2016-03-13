using System;
using System.Collections.Generic;
using System.Linq;


namespace Formula
{
    public class Test
    {
        public static int I0;
        public static int I1;
        public static long L0;
        public static string OutS = "";
        public static object[] OutVars = null;
        public class TestList : List<object>
        {
            public new object this[int i]
            {
                get
                {
                    if ((i > base.Count - 1) || (base[i] == null) || (base[i].ToString() == ""))
                        return null; 
                    else            
                        return  "(" + base[i].GetType().Name + ")Res" + i.ToString() + "=" + base[i].ToString();
                }
                set
                {
                    if (i> this.Count - 1)
                    {
                        base.AddRange(Enumerable.Repeat("", i - this.Count + 1));
                    }                    
                    base[i]=value;
                }
            }
        }
        public static TestList R = new TestList();
               
        public  int MPlus_i__i_i(int a, int b)
        {
            return a + b;
        }
        public int MMinus_i__i_i(int a, int b)
        {
            return a - b;
        }
        public int MPlus_i__i_l(int a, long b)
        {
            return a + (int)b;
        }
        public int MMinus_i__i_l(int a, long b)
        {
            return a - (int)b;
        }
        public static void SPlus_v__i_l(int a, long b)
        {
            I0 = a; L0 = a+b;
        }
        public static void SMinus_v__i_l(int a, long b)
        {
            I0 = a; L0 = a - b;
        }       
        public static int SPlus_i__i_l(int a, long b)
        {
            I0 = a; L0 = (long)a + b;
            return (int)L0;
        }
        public static Func<object> OutRes(bool IL=true)
        {
            //var HF0 = new Func<string, object[], string>(String.Format);
            R.Clear();
            
            var DynData = ExtFO.FromFunc(String.Format, () => OutS, () => OutVars);
            return ExtFO.FromAction(() => {
                OutVars = ExtFO.ACopy(R[R.Count - 1]);
                if (IL)
                {
                    var StData = ExtFO.FromFunc(String.Format, () => "I0={0},L0 ={1} ", () => ExtFO.ACopy(Test.I0, Test.L0));
                    Console.WriteLine(StData().ToString() + DynData());
                }
                else
                {
                    Console.WriteLine(DynData());
                }
                //  Console.ReadLine();
            });//OutR = ()=>(StData().ToString() + OutR()); - don't work
        }
        public  void TestRun()
        { 
            var OutR = OutRes();
            int a = 7; long b = 9;            
       
            Func<int, int, int> F0 = MPlus_i__i_i;
            Func<object,object, object> F1 = (x, y) => MPlus_i__i_i((int)x, (int)y);
            Func<int, int, int> F2 = new Func<int, int, int>(MPlus_i__i_i);
            var A0 = new Action<int, long>(SMinus_v__i_l);        
            var F3 = new Func<int, long, object>(ExtFO.MakeFunc(A0));

            var FD1 = ExtFO.DelegAction((Action<int, long>)SPlus_v__i_l);
            R[0] = FD1(3, 4);
            var FD0 = ExtFO.DelegFunc((Func<int, int, int>)MPlus_i__i_i);
            R[1] = FD0(1, 2);
            OutS = "Sum result from MPlus_i__i_i = {0}-must be 3, I,L must be 3,7";
            OutR();

            var FF0 = ExtFO.FromAction(SMinus_v__i_l, () => a, () => b);
            R[2] = FF0();
            OutS = "(after minus I,L must be 7,-2)"; 
            OutR();

            a = 75; b = 76;
            R[3] = FF0();
            OutS = "(after minus I,L must be 75,-1)"; 
            OutR();

            a = 2; b = 3;
            var FF1 = ExtFO.FromFunc(SPlus_i__i_l, () => a, () => b);
            R[4] = FF1();
            OutS = "Sum result from MPlus_i__i_i = {0} -must be 5, I,L must be 2,5";            
            OutR();

            a = 11; b = 22;
            R[5] = FF1();
            OutS = "Sum result from MPlus_i__i_i = {0} -must be 33, I,L must be 11,33";            
            OutR();          

            var FF2 = ExtF<object>.FuncFunc(F1);
            R[6] = (int)FF2(new object[] { 55, 4 });
            OutS = "Sum result from MPlus_i__i_i = {0} -must be 59, I,L must be old";            
            OutR();

            var FF3 = ExtF<object>.FuncAction(A0);
            R[7] = FF3(new object[] { 44, 7 });
            OutS = "(after minus I,L must be 44,37)";
            OutR();
            Console.ReadLine();
        }

        public void TestRunNum()
        {       
            var OutR = OutRes();

            WrapNumbers<int> WI = new WrapNumbers<int>();
            WI = 3.4;
            WI = "9";
            char ch = WI;
            R[0] = ch; OutS = "char ch = {0}"; OutR();

            decimal Dm = WI;
            R[1] = ch; OutS = "decimal Dm = {0}"; OutR();
            WI.SetuCO(new WrapNumbers<double>(12));
            R[2] = WI.ToString(); OutS = "from other Wrapper = {0}"; OutR();

            Dm = 16;
            R[3] = WI + (int)Dm;
            OutS = "Sum of different WrapNumbers (16, 12)= {0}"; OutR();
            WrapNumbers<long> WL = new WrapNumbers<long>();
            WL = WI * Dm;//without implicit operators!   
            R[4] = WL.ToString(); OutS = "Result operation * between different WrapNumbers(16,12)= {0}"; OutR();
            WI = ExtFO.FromFunc(MPlus_i__i_l, ()=>I0, () => L0);
            R[5] = (int)WI;
            OutS = "After set Sum Func to var: (int)WI = {0}"; OutR();
            I0 = 2;
            L0 = 3;
            R[6] = (int)WI;
            OutS = "After reset I,L: (int)WI = {0}"; OutR();


            Func<int> T0= ExtF<int>.FromFunc( MMinus_i__i_i,() => I0, () => (int)L0);
            R[7] = T0();
            OutS = "First val of used changeable Func(Minus): T0() = {0}"; OutR();          
            WI = ExtFO.FromFunc(MPlus_i__i_l,()=> T0(), () => L0);
            R[8] = (int)WI;
            OutS = "First val with use changeable Func through Func(Minus) = {0}"; OutR();
            //====
            T0 = ()=>(int)ExtFO.FromFunc(MPlus_i__i_i, () => I0, () => (int)L0)();
            R[9] = T0();
            OutS = "Second val of used changeable Func(Plus): T0() = {0}"; OutR();      
            R[10] = (int)WI;
            OutS = "Second val with use changeable Func through Func(Plus) = {0}"; OutR();          
            I0 = 33;
            R[11] = (int)WI;
            OutS = "Third val with use changeable Func through Func(Plus change input val) = {0}"; OutR();
            //===---
            WI = ExtFO.FromFunc(MPlus_i__i_l, T0, () => L0);   
            R[12] = (int)WI;
            OutS = "First val static changeable Func(after change T0 Func Results are same) = {0}"; OutR();
            T0 = () => (int)ExtFO.FromFunc(MMinus_i__i_i, () => I0, () => (int)L0)();
            R[13] = (int)WI;
            OutS = "Second val static changeable Func(after change T0 Func Results are same) = {0}"; OutR();
            I0 = 1;
            R[14] = (int)WI;
            OutS = "Third val static changeable Func(after change T0 Func Results are same "+
                "but change after input val) = {0}"; OutR();
            Console.ReadLine();
        }

        public void TestRunMulti()
        {
            var OutR = OutRes(false);
            Func<int> F0 = () => I0;
            Func<int> F0_ = () => I0;
            Func<int> F1_ = () => I1;
            Func<int> F1 = () => I1;
            Func<long> F2 = () => L0;
            WrapMultiFunc<int> MF = new WrapMultiFunc<int>();

            MF.AddFunc("PlusFunc", ExtFO.FromFunc(MPlus_i__i_l, F0, F2));
            MF.AddFunc("PlusFunc2", ExtFO.FromFunc(MPlus_i__i_l, F1, F2, true));
            MF.AddFunc("MinusFunc", ExtFO.FromFunc(MMinus_i__i_l, F0, F2));
            MF = "PlusFunc";
            Func<object> H = MF;
            R[0] = (string)MF;
            OutS = "Check string after add and set Func: {0}"; OutR();
            R[1] = (int)MF;
            OutS = "Check Result int after add and set Func: {0}"; OutR();
            R[2] = H.GetUUID() == ExtFO.FromFunc(MPlus_i__i_l, F0, F2).GetUUID();     
            OutS = "Check GetUUID Func for same with create new through FromFunc - same all data: {0}"; OutR();
            R[3] = H.GetUUID() == ExtFO.FromFunc(MPlus_i__i_l, F0, F2).GetUUID();
            OutS = "Check GetUUID Func for same with create new func through FromFunc - not same all data: {0}"; OutR();

            MF = "PlusFunc2";
            H = MF;
            string SR0 = MF;
            R[4] = H.GetUUID() == ExtFO.FromFunc(MPlus_i__i_l, F1, F2, true).GetUUID();
            R[5] = H.GetUUID() == ExtFO.FromFunc(MPlus_i__i_l, F1_, F2, true).GetUUID();
            R[6] = H.GetUUID() == ExtFO.FromFunc(MPlus_i__i_l, () => I1, F2, true).GetUUID();
            Func<object,bool> X = (x) => x.ToString().ToUpper().Contains("TRUE");
            R[7] = X(R[4]) && X(R[5]) && X(R[6]);
            OutS = "Check GetUUID Func for same with create new func through FromFunc -(Second var) not same all data: {0}"; OutR();
            R[8] = H.GetUUID() == ExtFO.FromFunc(MPlus_i__i_l, F0, F2, true).GetUUID();
            OutS = "Check GetUUID Func for same with create new func through FromFunc -(Second var) really not same all data: {0}"; OutR();
            /*Cannot use same Labels!!!!! 
            for nonstatic List<Func> need use set/get methods or overwrite other operation (+ for sample)
            http://stackoverflow.com/questions/19161361/non-static-implicit-operator */

            WrapMultiFunc<int> MF2 = new WrapMultiFunc<int>();
            MF2.AddFunc("PlusFunc3", ExtFO.FromFunc(MPlus_i__i_l, F1_, F2, true));
            MF2 = "PlusFunc3";
            R[9] = MF == MF2;
            OutS = " One more example of second variant setting ID for Func and comparing : {0}"; OutR();
            OutR = OutRes(true);
            I0 = 55;
            MF = "PlusFunc";
            R[0] = (int)MF;
            OutS = " PlusFunc, I0 = 55 : {0}"; OutR();
            I0 = 32;
            R[1] = (int)MF;
            OutS = " PlusFunc, I0 = 32 : {0}"; OutR();
            MF = "MinusFunc";
            R[2] = (int)MF;
            OutS = " MinusFunc, I0 = 32 : {0}"; OutR();
            I0 = 99;
            R[3] = (int)MF;
            OutS = " MinusFunc, I0 = 99 : {0}"; OutR();
            R[4] = (string)MF;
            OutS = " MinusFunc, I0 = 99 to string : {0}"; OutR();
            MF.ToStatic();
            I0 = 7;
            R[5] = (int)MF;
            OutS = " MinusFunc to static before change I0 (so Result same MinusFunc, I0 = 99), I0 = 7 : {0}"; OutR();
            Console.ReadLine();
        }
    }
}
