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
    class Validator
    {
        Node leaf = new Node(0, "");
        public Validator(Node leaf)
        {
            this.leaf = leaf;
        }
        public char Validate_data_type()
        {
            if (leaf.syntax.parent_type == Parent_Type.INTEGER)
            {
                return '1';
            }
            if (leaf.syntax.parent_type == Parent_Type.OCTET_STRING)
            {
                return '2';
            }
            return '5';
        }
        public bool Validate_int(string value)
        {
            if (leaf.syntax.max > 0)
            {
                UInt64.TryParse(value, out ulong int_value);
                if ((int_value >= leaf.syntax.min) && (int_value <= leaf.syntax.max))
                { 
                    return true;                     
                }
                else
                {
                    Console.WriteLine("Podana wartość jest spoza zakresu!");
                    return false;
                }                           
            }
            return true;
        }
        public bool Validate_string(string value)
        {
            if (leaf.syntax.max > 0)
            {
                if ((value.Length >= leaf.syntax.min) && (value.Length <= leaf.syntax.max))
                {
                    return true;
                }
                else
                {
                    Console.WriteLine("Podany tekst jest zbyt długi!");
                    return false;
                }
            }
            else if (leaf.syntax.size > 0)
            {
                if (value.Length <= leaf.syntax.size)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine("Podany tekst jest zbyt długi!");
                    return false;
                }
            }
            return true;
        }
    }
}
