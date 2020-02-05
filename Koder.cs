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
    enum Class_type
    {
        universal,
        application,
        contex_specific,
        priv
    }
    class Koder
    {
        public string OID;
        Tree tree_list = new Tree();
        public List<Byte> create_data_frame = new List<Byte>();
        public Class_type classes = new Class_type();
        public Koder (string id, Tree tree)
        {
            OID = id;
            tree_list = tree;
        }
        public List<Byte> Code_integer(int value)
        {
            List<Byte> data_frame = new List<Byte>();
            for (int i = 3; i>=0; --i)
            {
                Byte temp;
                temp = (Byte)(value >> 8 * i);
                data_frame.Add(temp);
            }            
            return data_frame;
        }
        public List<Byte> Code_string(string value)
        {
            List<Byte> data_frame = new List<byte>();
            for (int counter = 0; counter <= value.Length - 1; counter++)
            {
                data_frame.Add(Convert.ToByte(value[counter]));
            }
            return data_frame;
        }
        public List<Byte> Code_sequences()
        {
            List<Byte> partial_data_frame = new List<byte>();
            List<Byte> final_data_frame = new List<byte>();
            
            Console.WriteLine("Rozpoczynanie sekwencji: ");
            while (true)
            {
                partial_data_frame = Create_content();
                final_data_frame.InsertRange(final_data_frame.Count, partial_data_frame);
                Console.WriteLine("Wciśnij Enter aby zakończyć sekwencję. Wciśnięcie innego przycisku sprawi, że sekwencja będzie kontynuowana.");
                if (Console.ReadKey().Key == ConsoleKey.Enter)
                { 
                    break; 
                }
                Console.WriteLine("Kolejny element sekwencji: ");
            }
            Console.WriteLine("Kończenie sekwencji: ");
            return final_data_frame;
        }
        public List<Byte> Create_data_frame()
        {
            //return Create_content();
            return Create_content_after_validation();
        }
        public List<Byte> Create_identifier(Class_type classType, bool is_complex, int tag)
        {
            List<Byte> identifier = new List<byte>();
            Byte byte_z = 0;
            int temp = (int)classType;
            switch (temp)
            {
                case 0:
                    byte_z += 0;
                    break;
                case 1:
                    byte_z += 64;
                    break;
                case 2:
                    byte_z += 128;
                    break;
                case 3:
                    byte_z += 192;
                    break;
                default:
                    byte_z += 0;
                    break;
            }
            if (is_complex) 
                byte_z += 32;

            if (tag > 30)
            {
                byte_z += 31;
                identifier.Add(byte_z);
                while (tag - 127 > 0)
                {
                    identifier.Add(255);
                    tag -= 127;
                }
                byte_z = (byte)tag;
                identifier.Add(byte_z);
            }
            else
            {
                byte_z += (byte)tag;
                identifier.Add(byte_z);
            }
            return identifier;
        }
        public List<Byte> Create_length(int value)
        {
            List<Byte> length = new List<byte>();
            if (value > 127)
            {
                while (value - 127 > 0)
                {
                    length.Add(255);
                    value -= 127;
                }
                length.Add((byte)value);

                if (length.Count > 126)
                {
                    Console.WriteLine("Długość ramki jest zbyt duża.");
                    length.Clear();
                }
                else
                {
                    length.Insert(length[0], (byte)(128 + length.Count));
                }
            }
            else
            {
                length.Add((byte)value);
            }
            return length;
        }
        public List<Byte> Create_content()
        {
            List<Byte> data = new List<byte>();
            List<Byte> length = new List<byte>();
            List<Byte> identifier = new List<byte>();
            List<Byte> data_frame = new List<byte>();
            int tag = 0;
            int classtype;
            char data_type;
            bool is_complex;
            string value;
            Class_type class_type;
            Console.WriteLine("Wybierz typ: ");
            Console.WriteLine("universal - 0");
            Console.WriteLine("application - 1");
            Console.WriteLine("contex_specific - 2");
            Console.WriteLine("private - 3");
            classtype = Convert.ToInt32(Console.ReadLine());
            class_type = (Class_type)classtype;
            Console.WriteLine("Wybierz typ danych: ");
            Console.WriteLine("integer - 1");
            Console.WriteLine("string - 2");
            Console.WriteLine("null - 3");
            Console.WriteLine("sequence - 4");
            Console.WriteLine("object identifier - 5");
            data_type = Console.ReadKey().KeyChar;
            if (data_type == '1' || data_type == '2')
            {
                Console.WriteLine("Wprowadź wartość: ");
                value = Console.ReadLine();
            }
            else
            {
                value = "";
            }
            switch (data_type)
            {
                case '1':
                    bool success = false;
                    while (success != true)
                    {
                        success = Int32.TryParse(value, out int x);
                        if (success == true)
                        {
                            data = Code_integer(x);
                            tag = 2;
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Nieprawidłowa wartość integer.");
                        }
                    }
                    break;

                case '2':
                    data = Code_string(value);
                    tag = 4;
                    break;
                case '3':
                    data.Clear();
                    tag = 5;
                    break;
                case '4':
                    is_complex = true;
                    tag = 16;
                    data = Code_sequences();
                    break;
                case '5':
                    tag = 6;
                    break;
                default:
                    Console.WriteLine("Nieprawidłowy typ danych.");
                    break;
            }
            if (data_type != '4') { is_complex = false; }

            else { is_complex = true; }

            length = Create_length(data.Count);                                              
            identifier = Create_identifier(class_type, is_complex, tag);
            if (data_frame.Count == 0)
            {
                data_frame = identifier;
            }
            else
            {
                data_frame.InsertRange(data_frame.Count, identifier);
                //DataFrame.AddRange(identifier);
            }            
            data_frame.InsertRange(data_frame.Count, length);
            data_frame.InsertRange(data_frame.Count, data);
            return data_frame;
        }
        public List<Byte> Create_content_after_validation()
        {
            List<Byte> data = new List<byte>();
            List<Byte> length = new List<byte>();
            List<Byte> identifier = new List<byte>();
            List<Byte> data_frame = new List<byte>();
            int tag = 0;            
            char data_type;
            bool is_complex;
            string value;
            Class_type class_type = Class_type.universal;             
            Node leaf = tree_list.Find_OID(OID);
            if (leaf == null)
            {
                Console.WriteLine("Nie znaleziono elementu o takim OID. Czyszczenie wektora.");
                data_frame.Clear();
                return data_frame;
            }
            Validator valid = new Validator(leaf);
            data_type = valid.Validate_data_type();
            bool repeat = true;
            switch (data_type)
            {
                case '1':
                    tag = 2;
                    value = "";
                    while (!valid.Validate_int(value))
                    {
                        Console.WriteLine("Wprowadź prawidłową wartość integer: ");
                        value = Console.ReadLine();                        
                    }
                    Int32.TryParse(value, out int x);
                    data = Code_integer(x);
                    break;                   
                case '2':
                    tag = 4;
                    do
                    {
                        Console.WriteLine("Wprowadź tekst: ");
                        value = Console.ReadLine();
                    } while (repeat = !valid.Validate_string(value));
                    data = Code_string(value);
                    break;
                case '3':
                    data.Clear();
                    tag = 5;
                    break;
                case '4':
                    is_complex = true;
                    tag = 16;
                    data = Code_sequences();
                    break;
                case '5':
                    tag = 6;
                    break;
                default:
                    Console.WriteLine("Nieprawidłowy typ danych.");
                    break;
            }
            if (data_type != '4') { is_complex = false; }

            else { is_complex = true; }

            length = Create_length(data.Count);
            identifier = Create_identifier(class_type, is_complex, tag);
            if (data_frame.Count == 0)
            {
                data_frame = identifier;
            }
            else
            {
                data_frame.InsertRange(data_frame.Count, identifier);
            }
            data_frame.InsertRange(data_frame.Count, length);
            data_frame.InsertRange(data_frame.Count, data);
            return data_frame;
        }

    }
}
