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

    class MIB_parser
    {
        public string path;
        public string text;
        public Tree tree = new Tree();
        public List<Types_of_Data> data_types_found = new List<Types_of_Data>();
        public MIB_parser(string path)
        {
            this.path = path;
        }
        public void Read_from_file()
        {
            this.text = System.IO.File.ReadAllText(path);
            RegexOptions options = RegexOptions.Multiline;
            this.text = Regex.Replace(text, @"-- .*", String.Empty, options);
        }
        public void Add_weird_types()
        {
            Types_of_Data net_add = new Types_of_Data("NetworkAddress", 5, "EXPLICIT", "OCTET STRING", 4);
            Types_of_Data dis_str = new Types_of_Data("DisplayString", 6, "EXPLICIT", "OCTET STRING", 0, 255);
            Types_of_Data phys_add = new Types_of_Data("PhysAddress", 7, "EXPLICIT", "OCTET STRING");
            if (!data_types_found.Contains(net_add))
            {
                data_types_found.Add(net_add);
            }
            if (!data_types_found.Contains(dis_str))
            {
                data_types_found.Add(dis_str);
            }
            if (!data_types_found.Contains(phys_add))
            {
                data_types_found.Add(phys_add);
            }
        }
        public Types_of_Data Add_data_type(string name, uint TypeID, string implication, string parent_type)
        {
            
            Types_of_Data type = new Types_of_Data(name, TypeID, implication, parent_type);
            if (parent_type == "")
            {
                foreach (Types_of_Data t in data_types_found)
                {
                    if (t.name == type.name)
                    {
                        type.parent_type = t.parent_type;
                        type.implication = Implication.EXPLICIT;
                        break;
                    }
                    else
                    {
                        type.implication = Implication.IMPLICIT;
                    }
                }
            }
            if (!data_types_found.Exists(x => (x.name == type.name && x.parent_type == type.parent_type && x.implication == type.implication && x.size == type.size && x.min == type.min && x.max == type.max)))
            {
                if (type.typeID == 0)
                {
                    type.typeID = (uint)data_types_found.Count;
                }
                data_types_found.Add(type);
            }
            return type;
        }
        public Types_of_Data Add_data_type_size(string name, uint TypeID, string implication, string parent_type, uint size)
        {
            
            Types_of_Data type = new Types_of_Data(name, TypeID, implication, parent_type, size);
            if (parent_type == "")
            {
                foreach (Types_of_Data t in data_types_found)
                {
                    if (t.name == type.name)
                    {
                        type.parent_type = t.parent_type;
                        type.implication = Implication.EXPLICIT;
                        break;
                    }
                    else
                    {
                        type.implication = Implication.IMPLICIT;
                    }
                }
            }
            if (!data_types_found.Exists(x => (x.name == type.name && x.parent_type == type.parent_type && x.implication == type.implication && x.size == type.size && x.min == type.min && x.max == type.max)))
            {
                if (type.typeID == 0)
                {
                    type.typeID = (uint)data_types_found.Count;
                }
                data_types_found.Add(type);
            }
            return type;
        }
        public Types_of_Data Add_data_type_min_max(string name, uint TypeID, string implication, string parent_type, uint min, uint max)
        {
            Types_of_Data type = new Types_of_Data(name, TypeID, implication, parent_type, min, max);
            if (parent_type == "")
            {
                foreach (Types_of_Data t in data_types_found)
                {
                    if (t.name == type.name)
                    {
                        type.parent_type = t.parent_type;
                        type.implication = Implication.EXPLICIT;
                        break;
                    }
                    else
                    {
                        type.implication = Implication.IMPLICIT;
                    }
                }
            }
            if (!data_types_found.Exists(x => (x.name == type.name && x.parent_type == type.parent_type && x.implication == type.implication && x.size == type.size && x.min == type.min && x.max == type.max)))
            {
                if (type.typeID == 0)
                {
                    type.typeID = (uint)data_types_found.Count;
                }
                data_types_found.Add(type);
            }
            return type;
        }
        public void Check_for_internet()
        {
            string name;
            string name_of_parent1, name_of_parent2, name_of_parent3;
            uint OID, parent1_OID, parent2_OID, parent3_OID;
            Match match;
            RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.CultureInvariant;
            string expression = @"(?<name>\w*(-\d)?)\s*OBJECT\s*IDENTIFIER\s*::=\s*{\s*(?<parent_1>\w*)\s*(?<parent_2>\w*)\((?<parent_2_oid>\d+)\)\s*(?<parent_3>\w*)\((?<parent_3_oid>\d+)\)\s*(?<OID>\d+)\s*}";
            match = Regex.Match(text, expression, options);
            if (match.Success)
            {
                name = match.Groups["name"].Value;
                name_of_parent1 = match.Groups["parent_1"].Value;
                name_of_parent2 = match.Groups["parent_2"].Value;
                name_of_parent3 = match.Groups["parent_3"].Value;
                parent1_OID = 1;
                UInt32.TryParse(match.Groups["parent_2_oid"].Value, out parent2_OID);
                UInt32.TryParse(match.Groups["parent_3_oid"].Value, out parent3_OID);
                UInt32.TryParse(match.Groups["OID"].Value, out OID);
                Types_of_Data new_type = Add_data_type("", 0, "", "");
                tree.Add_a_node(parent1_OID, name_of_parent1, new_type, "", "", "", "");
                tree.Add_a_node(parent2_OID, name_of_parent2, new_type, "", "", "", name_of_parent1);
                tree.Add_a_node(parent3_OID, name_of_parent3, new_type, "", "", "", name_of_parent2);
                tree.Add_a_node(OID, name, new_type, "", "", "", name_of_parent3);                
            }
        }
        public void Check_for_data_types()
        {
            MatchCollection matches;
            Match match, match2;
            RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Multiline;
            string expression_type1 = @"(?<name>\w+)\s*::=\s*\[\s*\w*\s*(?<typeID>\d+)\s*\]\s*(?<implication>\w+)\s+(?<parentType>\w+(\s*\w*))?\s*((?<restrictions>\(?.*?\)\)?))?";
            string expression_type2 = @"\(SIZE\s*\((?<size>\d+)\)\)";
            string expression_type3 = @"\((?<min>\d+)..(?<max>\d+)\)";
            uint size;
            uint max;
            uint min;
            string parent_type;
            string restrictions;
            string name;
            string implication;
            matches = Regex.Matches(text, expression_type1, options);
            foreach (Match m in matches)
            {
                GroupCollection groups = m.Groups;
                name = groups["name"].Value;
                UInt32.TryParse(groups["typeID"].Value, out uint type_id);
                parent_type = groups["parentType"].Value;
                restrictions = groups["restrictions"].Value;
                implication = groups["implication"].Value;
                match = Regex.Match(restrictions, expression_type2, options);
                match2 = Regex.Match(restrictions, expression_type3, options);
                if (match.Success)
                {
                    UInt32.TryParse(match.Groups["size"].Value, out size);
                    //Console.WriteLine("Name {0}, Type ID: {1}, Implicit/Explicit: {2}, Parent type: {3}, Size: {4}", name, type_id, implication, parent_type, size);
                    Add_data_type_size(name, type_id, implication, parent_type, size);
                }
                else if (match2.Success)
                {
                    UInt32.TryParse(match2.Groups["min"].Value, out min);
                    UInt32.TryParse(match2.Groups["max"].Value, out max);
                    //Console.WriteLine("Name {0}, Type ID: {1}, Implicit/Explicit: {2}, Parent type: {3}, Min: {4}, Max: {5}, Restrictions: {6}", name, type_id, implication, parent_type, min, max, restrictions);
                    Add_data_type_min_max(name, type_id, implication, parent_type, min, max);
                }
                else 
                {
                    Add_data_type(name, type_id, implication, parent_type);
                    //Console.WriteLine("Name {0}, Type ID: {1}, Implicit/Explicit: {2}, Parent type: {3}", name, type_id, implication, parent_type);
                }
            }
            Add_weird_types();
        }
        public void Check_for_object_type()
        {
            string name;
            string syntax;
            Types_of_Data data_type;
            string access;
            string status;
            string description;
            string name_of_parent;
            string rest_of_stuff;
            string quote = "\"";
            string syntax_pattern = @"SYNTAX\s*(?<type_of_syntax>\w*( \w*)?( \w*)?)\s*(\(SIZE\s*\((?<min>\d+)..(?<max>\d+)\)\))?(\((?<min_2>\d+)..(?<max_2>\d+)\))?({\s*\w*\((?<min_3>\d+)\).*?\((?<max_3>\d+)\)\s*})?\s*ACCESS";
            string access_pattern = @"ACCESS\s*(?<access_type>[^%\n\s]*)";
            string status_pattern = @"STATUS\s*(?<status>[^%\n\s]*)";
            string description_pattern = @"DESCRIPTION\s*\" + quote + @"(?<descritpion>[^%]*)\" + quote;
            string parent_OID_pattern = @"\s*::=\s*\{\s*(?<parent_name>\w*)\s*(?<OID>\d+)\s*\}";
            uint OID;
            MatchCollection matches;
            Match match;
            List<string> outcome = new List<string>();
            RegexOptions options = RegexOptions.Multiline | RegexOptions.CultureInvariant;
            string expression = @"((?<name>\w*)\s*OBJECT-TYPE\s*)(?<rest_of_the_stuff>SYNTAX[^%]*?)((?=\w*\s*OBJECT-TYPE)|(\w*\s*END))";
            matches = Regex.Matches(text, expression, options);
            foreach (Match m in matches)
            {
                name = m.Groups["name"].Value;
                rest_of_stuff = m.Groups["rest_of_the_stuff"].Value;
                options = RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.CultureInvariant;
                match = Regex.Match(rest_of_stuff, syntax_pattern, options);
                if (match.Success)
                {
                    syntax = match.Groups["type_of_syntax"].Value;
                    if (!String.IsNullOrEmpty(match.Groups["min"].Value))
                    {
                        UInt32.TryParse(match.Groups["min"].Value, out uint syntax_min);
                        UInt32.TryParse(match.Groups["max"].Value, out uint syntax_max);
                        //Console.WriteLine("Min: {0}, Max {1}", syntax_min, syntax_max);
                        data_type = Add_data_type_min_max(syntax, 0, "", "", syntax_min, syntax_max);
                    }
                    else if (!String.IsNullOrEmpty(match.Groups["min_2"].Value))
                    {
                        UInt32.TryParse(match.Groups["min_2"].Value, out uint syntax_min);
                        UInt32.TryParse(match.Groups["max_2"].Value, out uint syntax_max);
                        //Console.WriteLine("Min: {0}, Max {1}", syntax_min, syntax_max);
                        data_type = Add_data_type_min_max(syntax, 0, "", "", syntax_min, syntax_max);
                    }
                    else if (!String.IsNullOrEmpty(match.Groups["min_3"].Value))
                    {
                        UInt32.TryParse(match.Groups["min_3"].Value, out uint syntax_min);
                        UInt32.TryParse(match.Groups["max_3"].Value, out uint syntax_max);
                        //Console.WriteLine("Min: {0}, Max {1}", syntax_min, syntax_max);
                        data_type = Add_data_type_min_max(syntax, 0, "", "", syntax_min, syntax_max);
                    }
                    else
                    {
                        data_type = Add_data_type(syntax, 0, "", "");
                    }
                }
                else 
                { 
                    syntax = null;
                    data_type = new Types_of_Data(syntax);
                }
                options = RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant;
                match = Regex.Match(rest_of_stuff, access_pattern, options);
                if (match.Success)
                {
                    access = match.Groups[1].Value;
                }
                else { access = null; }
                match = Regex.Match(rest_of_stuff, status_pattern, options);
                if (match.Success)
                {
                    status = match.Groups[1].Value;
                }
                else { status = null; }
                match = Regex.Match(rest_of_stuff, description_pattern, options);
                if (match.Success)
                {
                    description = match.Groups[1].Value;
                }
                else { description = null; }
                match = Regex.Match(rest_of_stuff, parent_OID_pattern, options);
                if (match.Success)
                {
                    name_of_parent = match.Groups[1].Value;
                    UInt32.TryParse(match.Groups[2].Value, out OID);
                }
                else { name_of_parent = null; OID = 0; }
                tree.Add_a_node(OID, name, data_type, access, status, description, name_of_parent);
            }

        }
        public void Check_for_object_identifier()
        {
            Check_for_internet();
            Types_of_Data data_type = new Types_of_Data("");
            string name;
            string name_of_parent;
            uint OID;
            MatchCollection matches;
            RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.CultureInvariant;
            string expression = @"(?<name>\w*(-\d)?)\s*OBJECT\s*IDENTIFIER\s*::=\s*{\s*(?<parent>[^ ]+)\s*(?<OID>[0-9]*)\s*}";
            matches = Regex.Matches(text, expression, options);
            foreach (Match m in matches)
            {
                name = m.Groups["name"].Value;
                name_of_parent = m.Groups["parent"].Value;
                UInt32.TryParse(m.Groups["OID"].Value, out OID);
                tree.Add_a_node(OID, name, data_type, "", "", "", name_of_parent);
                /*
                Console.WriteLine("Node's name: {0}", name);
                Console.WriteLine("Parent's name: {0}", name_of_parent);
                Console.WriteLine("OID: {0}", OID);
                */
            }

        }
        public void Parse_imports()
        {
            List<string> importsList = new List<string>();
            Match match;

            string Imports_pattern = @"IMPORTS.*?FROM\s*(?<IMPORT>\w+-\w+).*?FROM\s*(?<IMPORT2>\w+-\w+)";
            RegexOptions options = RegexOptions.Singleline;
            match = Regex.Match(text, Imports_pattern, options);
            importsList.Add(match.Groups[1].Value+".txt");
            importsList.Add(match.Groups[2].Value+".txt");
            foreach (string s in importsList)
            {
                string path = @"mibs\" + s;

                if (File.Exists(path))
                {
                    try
                    {
                        MIB_parser newParser = new MIB_parser(path);
                        newParser.Read_from_file();
                        newParser.Check_for_object_identifier();
                        newParser.Check_for_data_types();
                        newParser.Check_for_object_type();
                        tree = newParser.tree;
                        data_types_found = newParser.data_types_found;
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine("The file could not be read or doesn't exist:");
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }
    }

    
}