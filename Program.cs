using ConsoleTables;
using System;
using System.Collections.Generic;
using System.Linq;


namespace cw1
{
    class Program
    {
        private static readonly Matcher matcher = new Matcher();

        private static Dictionary<string, bool> RunTests(Rexp r, string[] testCases)
        {
            var results = new Dictionary<string, bool>();

            foreach (string t in testCases)
            {
                results.Add(t, matcher.Match(r, t));
            }

            return results;
        }

        private static void Question3()
        {
            string[] testCases = new string[]
            {
                "",
                "a",
                "aa",
                "aaa",
                "aaaaa",
                "aaaaaa",
            };

            Rexp r1 = new OPTIONAL(new CHAR('a'));
            var r1Results = RunTests(r1, testCases);

            Rexp r2 = new NOT(new CHAR('a'));
            var r2Results = RunTests(r2, testCases);

            Rexp r3 = new NTIMES(new CHAR('a'), 3);
            var r3Results = RunTests(r3, testCases);

            Rexp r4 = new NTIMES(new OPTIONAL(new CHAR('a')), 3);
            var r4Results = RunTests(r4, testCases);

            Rexp r5 = new UPTO(new CHAR('a'), 3);
            var r5Results = RunTests(r5, testCases);

            Rexp r6 = new UPTO(new OPTIONAL(new CHAR('a')), 3);
            var r6Results = RunTests(r6, testCases);

            Rexp r7 = new BETWEEN(new CHAR('a'), 3, 5);
            var r7Results = RunTests(r7, testCases);

            Rexp r8 = new BETWEEN(new OPTIONAL(new CHAR('a')), 3, 5);
            var r8Results = RunTests(r8, testCases);

            Rexp cornerCase = new NTIMES(new CHAR('a'), 0);
            var ccResults = RunTests(cornerCase, testCases);

            var resultsTable = new ConsoleTable("string", "a?", "~a", "a{3}", "(a?){3}", "a{..3}", "(a?){..3}", "a{3..5}", "(a?){3..5}", "a{0}");
            foreach (string tcase in testCases)
            {
                resultsTable.AddRow(
                    tcase,
                    r1Results[tcase],
                    r2Results[tcase],
                    r3Results[tcase],
                    r4Results[tcase],
                    r5Results[tcase],
                    r6Results[tcase],
                    r7Results[tcase],
                    r8Results[tcase],
                    ccResults[tcase]
                );
            }
            resultsTable.Write();

        }
        private static void Question5()
        {

            HashSet<char> set1 = "abcdefghijklmnopqrstuvwxyz0123456789_.-".ToHashSet();
            HashSet<char> set2 = "abcdefghijklmnopqrstuvwxyz0123456789.-".ToHashSet();
            HashSet<char> set3 = "abcdefghijklmnopqrstuvwxyz.".ToHashSet();

            Rexp addr = new SEQ(new PLUS(new RANGE(set1)), new CHAR('@'));
            Rexp domain = new SEQ(new PLUS(new RANGE(set2)), new CHAR('.'));
            Rexp topLevelDomain = new BETWEEN(new RANGE(set3), 2, 6);
            Rexp emailAddr = new SEQ(new SEQ(addr, domain), topLevelDomain);

            string input = "james.legge@kcl.ac.uk";
            bool result = matcher.Match(matcher.simp(emailAddr), input);
            Console.WriteLine($"Is \"{input}\" an email address?: {result}");
        }

        private static void Question6()
        {
            //        \/\*(?!.*\*\/.*)\*\/

            Rexp innerNot =  new NOT(new SEQ(new SEQ(new SEQ(new STAR(new ALL()), new CHAR('*')), new CHAR('/')), new STAR(new ALL())));
            Rexp r = new SEQ(new SEQ ( new SEQ(     new SEQ     (new CHAR('/'), new CHAR('*'))  , innerNot),     new CHAR('*')), new CHAR('/'));

            string[] testCases = new string[]
            {
                "/**/",
                "/*foobar*/",
                "/*test*/test*/",
                "/*test/*test*/"
            };

            foreach (string tCase in testCases)
            {
                bool res = matcher.Match(r, tCase);
                Console.WriteLine($"{tCase}: {res}");
            }
        }

        private static void Question7()
        {
            Rexp r1 = new SEQ(new SEQ(new CHAR('a'), new CHAR('a')), new CHAR('a'));
            Rexp r2 = new SEQ(new BETWEEN(new CHAR('a'), 19, 19), new OPTIONAL(new CHAR('a')));

            Rexp r4 = new PLUS(new PLUS(r1));
            Rexp r5 = new PLUS(new PLUS(r2));

            string[] testCases = new string[]
            {
                "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",

                "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",

                "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"
            };
            
            Console.WriteLine("Testing: r1");
            foreach (string tcase in testCases)
            {
                Console.WriteLine(matcher.Match(r1, tcase));
            }

            Console.WriteLine("\nTesting: r2");
            foreach (string tcase in testCases)
            {
                Console.WriteLine(matcher.Match(r2, tcase));
            }
            
            Console.WriteLine("\nTesting: (r1+)+");
            foreach (string tcase in testCases)
            {
                Console.WriteLine(matcher.Match(r4, tcase));
            }
            
            Console.WriteLine("\nTesting: (r2+)+");
            foreach (string tcase in testCases)
            {
                Console.WriteLine(matcher.Match(r5, tcase));
            }
            
        }

        static void Main()
        { 
            
            Console.WriteLine("\nQUESTION 3:");
            Question3();

            Console.WriteLine("\nQUESTION 5:");
            Question5();

            Console.WriteLine("\nQUESTION 6:");
            Question6();
            
            Console.WriteLine("\nQUESTION 7:");
            Question7();
        }
    }
}
