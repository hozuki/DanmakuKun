using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Reflection;
using Microsoft.JScript;

namespace DanmakuKun
{
    public sealed class JSEvaluator
    {

        public static int EvalToInteger(string statement)
        {
            string s = EvalToString(statement);
            return int.Parse(s.ToString());
        }

        public static double EvalToDouble(string statement)
        {
            string s = EvalToString(statement);
            return double.Parse(s);
        }

        public static string EvalToString(string statement)
        {
            object o = EvalToObject(statement);
            return o.ToString();
        }

        // current version with JScriptCodeProvider BEGIN  
        ///*  

        public static object EvalToObject(string statement)
        {
            return _evaluatorType.InvokeMember(
                  "Eval",
                  BindingFlags.InvokeMethod,
                  null,
                  _evaluator,
                  new object[] { statement }
                 );
        }

        public static void SayHello()
        {
            _evaluatorType.InvokeMember(
                "sayHello",
                BindingFlags.InvokeMethod,
                null,
                _evaluator,
                new object[] { }
                );
        }

        static JSEvaluator()
        {
            JScriptCodeProvider compiler = new JScriptCodeProvider();

            CompilerParameters parameters;
            parameters = new CompilerParameters();
            parameters.GenerateInMemory = true;

            CompilerResults results;
            results = compiler.CompileAssemblyFromSource(
                                            parameters, _jscriptSource);
            
            foreach (CompilerError error in results.Errors)
            {
                Debug.Print(error.ErrorText);
            }
            Assembly assembly = results.CompiledAssembly;
            _evaluatorType = assembly.GetType("JSEvaluator.JSEvaluator");

            _evaluator = Activator.CreateInstance(_evaluatorType);
        }

        private static object _evaluator = null;
        private static Type _evaluatorType = null;
        private static readonly string _jscriptSource =
            //          @"package JSEvaluator 
            //      { 
            //         class JSEvaluator 
            //         { 
            //          public function Eval(expr : String) : Object 
            //          { 
            //           return eval(expr); 
            //          } 
            //          public function sayHello()
            //          {
            //           print('hello, world!');
            //          }
            //         } 
            //      }";
                    @"class jse{
                    const epsilon = 0.00000000001; // Some very small number to test against.

                    // Type annotate the function parameters and return type.
                    function integerCheck(a : int, b : int, c : int) : boolean {
                       // The test function for integers.
                       // Return true if a Pythagorean triplet.
                       return ( ((a*a) + (b*b)) == (c*c) );
                    } // End of the integer checking function.

                    function floatCheck(a : double, b : double, c : double) : boolean {
                       // The test function for floating-point numbers.
                       // delta should be zero for a Pythagorean triplet.
                       var delta = Math.abs( ((a*a) + (b*b) - (c*c)) * 100 / (c*c));
                       // Return true if a Pythagorean triplet (if delta is small enough).
                       return (delta < epsilon);
                    } // End of the floating-poing check function.

                    // Type annotation is not used for parameters here. This allows 
                    // the function to accept both integer and floating-point values 
                    // without coercing either type.
                    function checkTriplet(a, b, c) : boolean { 
                       // The main triplet checker function.
                       // First, move the longest side to position c.
                       var d = 0; // Create a temporary variable for swapping values
                       if (b > c) { // Swap b and c.
                          d = c;
                          c = b;
                          b = d;
                       }
                       if (a > c) { // Swap a and c.
                          d = c;
                          c = a;
                          a = d;
                       }

                       // Test all 3 values. Are they integers?
                       if ((int(a) == a) && (int(b) == b) && (int(c) == c)) { // If so, use the precise check.
                          return integerCheck(a, b, c); 
                       } else { // If not, get as close as is reasonably possible.
                          return floatCheck(a, b, c); 
                       }
                    } // End of the triplet check function.

                    // Test the function with several triplets and print the results.
                    // Call with a Pythagorean triplet of integers.
                    print(checkTriplet(3,4,5));
                    // Call with a Pythagorean triplet of floating-point numbers.
                    print(checkTriplet(5.0,Math.sqrt(50.0),5.0));
                    // Call with three integers that do not form a Pythagorean triplet.
                    print(checkTriplet(5,5,5));
                    };";

        //*/  
        // current version with JScriptCodeProvider END  


        // deprecated version with Vsa BEGIN  
        /* 
 
        public static Microsoft.JScript.Vsa.VsaEngine Engine = 
                      Microsoft.JScript.Vsa.VsaEngine.CreateEngine(); 
 
        public static object EvalToObject(string JScript) 
        { 
          object Result = null; 
          try 
          { 
            Result = Microsoft.JScript.Eval.JScriptEvaluate( 
                                                    JScript, Engine); 
          } 
          catch (Exception ex) 
          { 
            return ex.Message; 
          } 
          return Result; 
        } 
 
        */
        // deprecated version with Vsa END  
    }
}