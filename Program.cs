using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Parser
{
    class Program
    {
        static void Main(string[] args)
        {
            MIB_parser parser = new MIB_parser(@"mibs\RFC1213-MIB.txt");
            parser.Read_from_file();
            parser.Parse_imports();
            parser.Check_for_object_identifier();
            parser.Check_for_data_types();
            parser.Check_for_object_type();
            //Node szukany = parser.tree.Find_OID("1.3.6.1.2.1.1.1");
            Koder coder = new Koder("1.3.6.1.2.1.1.1", parser.tree);
            //List<Byte> data_vector = coder.Create_content_after_validation();
            while (true)
            {
                Console.WriteLine("W celu przerwania wprowadzania wciśnij ESC :");
                if (Console.ReadKey().Key == ConsoleKey.Escape)
                {
                    break;
                }
                Console.WriteLine("Rozpoczynam kodowanie następnego elementu. ");
            }          
            Console.ReadLine();
        }
    }
}
