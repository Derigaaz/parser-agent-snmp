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
    public enum Access_types
    {
        read_only,
        read_write,
        write_only,
        not_accessible
    }
    public enum Status_types
    {
        mandatory,
        optional,
        obsolete
    }
    class Node
    {
        private Access_types Access_type_check(string access_type)
        {
            Access_types check;
            if (access_type.Contains("read-only"))
            {
                check = Access_types.read_only;
            }
            else if (access_type.Contains("read-write"))
            {
                check = Access_types.read_write;
            }
            else if (access_type.Contains("write-only"))
            {
                check = Access_types.write_only;
            }
            else
            {
                check = Access_types.not_accessible;
            }
            return check;
        }
        private Status_types Status_type_check(string parent_type)
        {
            Status_types check;
            if (parent_type.Contains("mandatory"))
            {
                check = Status_types.mandatory;
            }
            else if (parent_type.Contains("optional"))
            {
                check = Status_types.optional;
            }
            else
            {
                check = Status_types.obsolete;
            }
            return check;
        }
        public uint OID;
        public string name;
        public Types_of_Data syntax;
        public Access_types access;
        public Status_types status;
        public string description;
        public string name_of_parent;
        public LinkedList<Node> children;

        public Node(uint id, string name, Types_of_Data syntax, string access, string status, string description, string parent)
        {
            this.OID = id;
            this.name = name;
            this.syntax = syntax;
            this.access = Access_type_check(access);
            this.status = Status_type_check(status);
            this.description = description;
            this.name_of_parent = parent;
            children = new LinkedList<Node>();
        }
        public Node(uint id, string name, string parent)
        {
            this.OID = id;
            this.name = name;
            this.name_of_parent = parent;
            children = new LinkedList<Node>();
        }
        public Node(uint id, string name)
        {
            this.OID = id;
            this.name = name;
        }
        public void MakeAKid(Node kiddo)
        {
            children.AddFirst(kiddo);
            //Console.WriteLine("Utworzono element o nazwie {0} dla rodzica o nazwie: {1}.", kiddo.name, this.name);
        }
        public Node FetchKids(int i)
        {
            foreach (Node n in children)
                if (--i == 0)
                    return n;
            return null;
        }
        public Node Search_kids_by_name(string name)
        {
            foreach (Node n in children)
            {
                if (n.name == name)
                {
                    return n;
                }
            }
            return null;
        }
    };

}
