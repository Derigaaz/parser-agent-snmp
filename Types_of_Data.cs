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
    enum Parent_Type
    {
        INTEGER,
        OCTET_STRING,
        OBJECT_IDENTIFIER,
        NULL
    }
    enum Implication
    {
        IMPLICIT,
        EXPLICIT,
        unknown
    }
    class Types_of_Data
    {
        public string name;
        public uint typeID;
        public Implication implication;
        public Parent_Type parent_type;
        public uint size;
        public uint min;
        public uint max;
        private Implication Implication_check(string implication)
        {
            Implication check;
            if (implication.Contains("IMPLICIT"))
            {
                check = Implication.IMPLICIT;
            }
            else if (implication.Contains("EXPLICIT"))
            {
                check = Implication.EXPLICIT;
            }
            else
            {
                check = Implication.unknown;
            }
            return check;
        }
        private Parent_Type Parent_Type_check(string parent_type)
        {
            Parent_Type check;
            if (parent_type.Contains("INTEGER"))
            {
                check = Parent_Type.INTEGER;
            }
            else if (parent_type.Contains("OCTET STRING"))
            {
                check = Parent_Type.OCTET_STRING;
            }
            else if (parent_type.Contains("OBJECT IDENTIFIER"))
            {
                check = Parent_Type.OBJECT_IDENTIFIER;
            }
            else
            {
                check = Parent_Type.NULL;
            }
            return check;
        }
        public string Name_check(string a)
        {
            if (a == "")
            {
                a = "NULL";
            }
            return a;
        }
        public Types_of_Data(string name)
        {
            this.name = Name_check(name);
        }
        public Types_of_Data(string name, uint TypeID, string implication, string parent_type, uint size)
        {
            this.name = Name_check(name).Trim();
            this.typeID = TypeID;
            this.implication = Implication_check(implication);
            this.parent_type = Parent_Type_check(parent_type);
            this.size = size;
        }
        public Types_of_Data(string name, uint TypeID, string implication, string parent_type, uint min, uint max)
        {
            this.name = Name_check(name).Trim();
            this.typeID = TypeID;
            this.implication = Implication_check(implication);
            this.parent_type = Parent_Type_check(parent_type);
            this.min = min;
            this.max = max;
        }
        public Types_of_Data(string name, uint TypeID, string implication, string parent_type)
        {
            this.name = Name_check(name).Trim();
            this.typeID = TypeID;
            this.implication = Implication_check(implication);
            this.parent_type = Parent_Type_check(parent_type);
        }
    }
 
}
