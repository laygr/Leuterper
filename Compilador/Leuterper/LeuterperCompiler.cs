using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leuterper.Constructions;
using Leuterper.MachineInstructions;

namespace Leuterper
{
    class LeuterperCompiler
    {
        static String tempFile = "temp.txt";
        public String filePath;
        public Program program;
        private List<MachineInstruction> instructions;        

        public LeuterperCompiler(String filePath)
        {
            this.instructions = new List<MachineInstruction>();
            this.filePath = filePath;
        }

        public void addMI(MachineInstruction mi)
        {
            this.instructions.Add(mi);
        }

        public void parse()
        {
            Stream s = null;
            try
            {
                StreamWriter temp = new StreamWriter(tempFile);
                StreamReader f = new StreamReader(this.filePath);
                temp.WriteLine(LeuterperCompiler.standardLibrary());
                temp.WriteLine(f.ReadToEnd());
                temp.WriteLine();
                temp.WriteLine(LeuterperCompiler.callToMain());
                f.Close();
                temp.Close();
                s = File.Open(tempFile, FileMode.Open);

                TheParser parser = new TheParser(s);
                this.program = parser.parse_Program();
                Console.WriteLine("El archivo se parseo con exito.");
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("File not found");
            }
            finally
            {
                s.Close();
                File.Delete(tempFile);
            }
        }

        public void compile()
        {
            try
            {
                parse();
                //program.GetScopeManager().validate();
                program.generateCode(this);
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                Environment.Exit(0);
            }
        }



        public static void Main(String[] args)
        {
            do
            {
                Console.WriteLine("Introduzca el nombre del archivo a compilar.");
                new LeuterperCompiler(Console.ReadLine()).compile();
                Console.WriteLine("El archivo se compilo con exito.");
                Console.WriteLine("Presione 0 para parsear otro archivo");
                if (!Console.ReadLine().Equals("0")) { break; }
            } while (true);
        }

        public static String callToMain()
        {
            return "main(read());\n";
        }

        public static String standardLibrary()
        {
            return
                "class Object { " +
                    "Boolean equals(Object other) {}" +
                    "String toString() { }" +
                "}\n" +

                "class Number {" +
                    "String toString() { }" +
                    "Number +(Number other){ }" +
                    "Number -(Number other){ }" +
                    "Number *(Number other){ }" +
                    "Number /(Number other){ }" +
                "}\n" +

                "class List[A] {" +
                    "String toString() { }" +
                    "void Add(A other) { }" +
                    "Number Count() { }" +
                    "A Get(Number index){ }" +
                    "Void Set(A element, Number index) { }" +
                    
                "}\n" +

                "class Char { " +
                    "String toString() { }" +
                "}\n" +

                "class String inherits List[Char] {" +
                    "String toString() { } " +
                "}\n" +
                
                "class System {" +
                    "Void write(String text) { }" +
                    "String read() { }" +
                    "Void exit() { }" +
                "}\n"
                ;
                    
        }
    }
}
