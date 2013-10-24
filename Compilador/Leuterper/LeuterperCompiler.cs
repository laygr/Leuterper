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

        public int globalVariablesCounter;
        public int mostVaribalesInAFunction;

        public bool compilingTopLevelActions;

        public List<int> classDefinitions;
        public List<int> functionsParameters;
        public List<List<MachineInstruction>> functionActions;
        public List<MachineInstructions.Literal> literals;
        public List<MachineInstructions.MachineInstruction> topLevelActions;


        public LeuterperCompiler(String filePath)
        {
            this.globalVariablesCounter = 0;
            this.mostVaribalesInAFunction = 0;
            this.compilingTopLevelActions = false;

            this.classDefinitions = new List<int>();
            this.functionsParameters = new List<int>();
            this.functionActions = new List<List<MachineInstruction>>();
            this.literals = new List<MachineInstructions.Literal>();
            this.topLevelActions = new List<MachineInstruction>();

            this.filePath = filePath;
        }

        public void compile()
        {
            parse();
            program.secondPass();
            //program.GetScopeManager().validate();
            program.generateCode(this);
            printGeneratedCode();


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
                throw new Exception("File not found");
            }
            finally
            {
                if (s != null)
                {
                    s.Close();
                    //File.Delete(tempFile);
                }

            }
        }

        public void printGeneratedCode()
        {
            StreamWriter writer = new StreamWriter("out.txt");

            writer.WriteLine(this.classDefinitions.Count());
            foreach (int aClassDefinition in this.classDefinitions)
            {
                writer.WriteLine(aClassDefinition);
            }

            writer.WriteLine(this.functionActions.Count());

            int i = 0;
            foreach (List<MachineInstruction> funcDef in this.functionActions)
            {
                writer.WriteLine(this.functionsParameters[i]);
                writer.WriteLine(funcDef.Count());
                foreach (MachineInstruction mi in funcDef)
                {
                    writer.WriteLine(mi);
                }
                i++;
            }

            //Max number of vars in memory
            writer.WriteLine(this.literals.Count() + this.globalVariablesCounter + this.mostVaribalesInAFunction);

            writer.WriteLine(this.literals.Count());
            foreach (MachineInstructions.Literal literal in this.literals)
            {
                writer.WriteLine(literal);
            }

            writer.WriteLine(this.topLevelActions.Count());
            foreach (MachineInstruction m in this.topLevelActions)
            {
                writer.WriteLine(m);
            }
            writer.Close();
        }

        public void addClassDefinition(int numberOfAttributes)
        {
            this.classDefinitions.Add(numberOfAttributes);
        }

        public void addAction(MachineInstruction action)
        {
            if (this.compilingTopLevelActions)
            {
                this.topLevelActions.Add(action);
            }
            else
            {
                int indexOfCurrentFunction = this.functionActions.Count() - 1;
                this.functionActions[indexOfCurrentFunction].Add(action);
            }
        }

        public int getIndexOfNextActionInCurrentFunction()
        {
            int indexOfCurrentFunction = this.functionActions.Count() - 1;
            return this.functionActions[indexOfCurrentFunction].Count();
        }

        public void addLiteral(Literal literal)
        {
            this.literals.Add(literal);
        }

        public static void Main(String[] args)
        {
            try
            {
                Console.WriteLine("Introduzca el nombre del archivo a compilar.");
                new LeuterperCompiler(Console.ReadLine()).compile();
                Console.WriteLine("El archivo se compilo con exito.");
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
            }
        }

        public static String callToMain()
        {
            return "main(read());";
        }

        public static String standardLibrary()
        {
            return
                "class Object { " +
                    "Boolean equals(Object other) { }" +
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

                 "Void write(String text) { }\n" +
                 "String read() { }\n" +
                 "Void exit() { }\n";

        }
    }
}
