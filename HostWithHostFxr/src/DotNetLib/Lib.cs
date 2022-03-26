using System;
using System.Runtime.InteropServices;
using System.Globalization;

namespace DotNetLib
{
    public static class Lib
    {
        private static int s_CallCount = 1;

        [StructLayout(LayoutKind.Sequential)]
        public struct LibArgs
        {
            public IntPtr Message;
            public int Number;
        }

        public static int Hello(IntPtr arg, int argLength)
        {
            if (argLength < System.Runtime.InteropServices.Marshal.SizeOf(typeof(LibArgs)))
            {
                return 1;
            }

            LibArgs libArgs = Marshal.PtrToStructure<LibArgs>(arg);
            Console.WriteLine($"Hello, world! from {nameof(Lib)} [count: {s_CallCount++}]");
            PrintLibArgs(libArgs);
            return 0;
        }

        public static int TestCultureInfoCompareInfo(IntPtr arg, int argLength)
        {
            String[] sign = new String[] { "<", "=", ">" };

            // The code below demonstrates how strings compare
            // differently for different cultures.
            String s1 = "Coté", s2 = "coté", s3 = "côte";

            // Set sort order of strings for French in France.
            CompareInfo ci = new CultureInfo("fr-FR").CompareInfo;
            Console.WriteLine("The LCID for {0} is {1}.", ci.Name, ci.LCID);

            // Display the result using fr-FR Compare of Coté = coté.  	
            Console.WriteLine("fr-FR Compare: {0} {2} {1}",
                s1, s2, sign[ci.Compare(s1, s2, CompareOptions.IgnoreCase) + 1]);

            // Display the result using fr-FR Compare of coté > côte.
            Console.WriteLine("fr-FR Compare: {0} {2} {1}",
                s2, s3, sign[ci.Compare(s2, s3, CompareOptions.None) + 1]);

            // Set sort order of strings for Japanese as spoken in Japan.
            ci = new CultureInfo("ja-JP").CompareInfo;
            Console.WriteLine("The LCID for {0} is {1}.", ci.Name, ci.LCID);

            // Display the result using ja-JP Compare of coté < côte.
            Console.WriteLine("ja-JP Compare: {0} {2} {1}",
                s2, s3, sign[ci.Compare(s2, s3) + 1]);

            return 0;
        }

        public delegate void CustomEntryPointDelegate(LibArgs libArgs);
        public static void CustomEntryPoint(LibArgs libArgs)
        {
            Console.WriteLine("-------");
            TestCultureInfoCompareInfo(libArgs.Message, libArgs.Number);
        }

#if NET5_0
        [UnmanagedCallersOnly]
        public static void CustomEntryPointUnmanaged(LibArgs libArgs)
        {
            CustomEntryPoint(libArgs);
        }
#endif

        private static void PrintLibArgs(LibArgs libArgs)
        {
            string message = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? Marshal.PtrToStringUni(libArgs.Message)
                : Marshal.PtrToStringUTF8(libArgs.Message);

            Console.WriteLine($"-- message: {message}");
            Console.WriteLine($"-- number: {libArgs.Number}");
        }
    }
}
