using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text.RegularExpressions;


namespace Parser
{

    class Tree
    {
        public Node tree_root = null;
        public void Add_a_node(uint id, string name, Types_of_Data syntax, string access, string status, string description, string parent)
        {
            Node child = new Node(id, name, syntax, access, status, description, parent);
            //Node parent_node = new Node(0, "parent", "parent", "parent", "parent", "parent", "parent");
            if (tree_root == null)
            {
                tree_root = child;
            }
            else
            {
                Node parent_node = FindParent(child, tree_root);
                if (parent_node.OID != 0)
                {
                    parent_node.MakeAKid(child);
                }

            }
        }
        public Node FindParent(Node kiddo, Node previous_node)
        {
            //Node parent_node = new Node(0, "parent", "parent", "parent", "parent", "parent", "parent");
            if (kiddo == tree_root)
            {
                //Console.WriteLine("Szukany element nie ma rodzica (jest to korzeń drzewa).");
                return null;
            }
            else if (kiddo.name_of_parent == tree_root.name)
            {
                //Console.WriteLine("Szukany element ma rodzica o nazwie: {0}. Rodzic ten jest korzeniem drzewa.", previous_node.name);
                return previous_node;
            }
            else
            {
                foreach (Node wanted in previous_node.children)
                {
                    if (wanted.name == kiddo.name_of_parent)
                    {
                        //Console.WriteLine("Szukany element ma rodzica o nazwie: {0}.", wanted.name);
                        return wanted;
                    }
                    else
                    {
                        foreach (Node wanted2 in previous_node.children)
                        {
                            Node parent_node = FindParent(kiddo, wanted2);
                            if (parent_node != null)
                            {
                                //Console.WriteLine("Szukany element ma rodzica o nazwie: {0}.", parent_node.name);
                                return parent_node;
                            }
                        }
                    }
                }
            }
            //Console.WriteLine("Nie udało się znaleźć rodzica.");
            return null;
        }
        public Node Find_OID(string input)
        {
            List<uint> list = Parse_OID(input);
            Node output = Find_by_OID(list, tree_root);
            return output;
        }
        public List<uint> Parse_OID(string input)
        {
            Match match;
            MatchCollection matches;
            RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant;
            List<uint> output = new List<uint>();
            string incorrect_pattern = @"([^1-9\.])";
            string OID_parts_pattern = @"(?<oid>\d)";
            match = Regex.Match(input, incorrect_pattern, options);
            if (match.Success)
            {
                Console.WriteLine("Podana wartość nie jest poprawnym formatem OID.");
                return null;
            }
            matches = Regex.Matches(input, OID_parts_pattern, options);
            foreach (Match match2 in matches)
            {
                UInt32.TryParse(match2.Groups["oid"].Value, out uint OID_value);
                output.Add(OID_value);
            }
            if (output[0] == 1)
            {
                output.RemoveAt(0);
            }
            else
            {
                Console.WriteLine("Podana wartość nie jest poprawnym formatem OID.");
            }
            //output.RemoveAt(0);
            //Console.WriteLine("OID: {0}.{1}", output[0], output[1]);
            /*foreach (uint value in output)
            {
                Console.WriteLine("OID: {0}", value);
            }*/
            return output;
        }
        public Node Find_by_OID(List<uint> list, Node previous_node)
        {
            Node n = null;
            foreach (Node n2 in previous_node.children)
            {
                if (n2.OID == list[0])
                {
                    list.RemoveAt(0);
                    if (list.Count == 0)
                    {
                        //Console.WriteLine("Element o szukanym OID ma nazwę: {0}.", n2.name);
                        n = n2;
                        return n;
                    }
                    else
                    {
                        n = Find_by_OID(list, n2);
                        break;
                    }        
                }
            }
            if (n == null)
            {
                Console.WriteLine("Nie udało się znaleźć elementu o takim OID.");
                return n;
            }
            else
            {
                return n;
            }

        }
        /*public string Get_OID_from_name(string wanted, Node previous_node)
        {
            string OID = "";
            string temp = "";
            if (wanted == tree_root.name)
            {
                Console.WriteLine("Szukany element to korzeń drzewa o OID {0}", previous_node.OID);
                OID = previous_node.OID.ToString();
                return OID;
            }
            else
            {
                foreach (Node n in previous_node.children)
                {
                    if (wanted == n.name)
                    {
                        OID = n.OID.ToString();
                        return OID;
                    }
                    else
                    {
                        foreach (Node n2 in previous_node.children)
                        {
                            temp = Get_OID_from_name(wanted, n2);
                        }
                    }
                if (temp != "")
                {
                    OID = previous_node.OID + "." + temp;
                }
                }
                
            }
            return OID;
        }*/

    }
}




