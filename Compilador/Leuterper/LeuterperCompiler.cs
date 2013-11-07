using Leuterper.Constructions;
using Leuterper.MachineInstructions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Leuterper
{
    class LeuterperCompiler
    {
        static String tempFile = "temp.txt";
        public String filePath;
        public Program program;

        public int globalVariablesCounter;
        public int mostVaribalesInAFunction;

        public bool compilingTopLeveIActions;

        public int classesCounter;
        private int proceduresCounter;

        public List<int> classDefinitions;
        public List<int> functionsParameters;
        public List<List<MachineInstruction>> functionActions;
        public List<MachineInstructions.Literal> literals;
        public List<MachineInstructions.MachineInstruction> topLeveIActions;

        StandardLibrary standradLibrary;
        public LeuterperCompiler(String filePath)
        {
            StandardLibrary.singleton = new StandardLibrary();
            this.standradLibrary = StandardLibrary.singleton;
            this.classesCounter = this.standradLibrary.standardClasses.Count();
            this.proceduresCounter = this.standradLibrary.standardProcedures.Count() + this.standradLibrary.standardFunctions.Count();
                

            this.globalVariablesCounter = 0;
            this.mostVaribalesInAFunction = 3;
            this.compilingTopLeveIActions = false;


            this.classDefinitions = new List<int>();
            this.functionsParameters = new List<int>();
            this.functionActions = new List<List<MachineInstruction>>();
            this.literals = new List<MachineInstructions.Literal>();
            this.topLeveIActions = new List<MachineInstruction>();

            this.filePath = filePath;
        }

        public void compile()
        {
            parse();
            program.symbolsRegistration(this);
            program.symbolsUnificationPass();
            program.classesGenerationPass();
            program.simplificationAndValidationPass();
            program.codeGenerationPass(this);
            printGeneratedCode();
            printSymbols();

        }
        public void parse()
        {
            Stream s = null;
            try
            {
                StreamWriter temp = new StreamWriter(tempFile);
                StreamReader f = new StreamReader(this.filePath);
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
                throw e;
            }
            finally
            {
                if (s != null)
                {
                    s.Close();
                    File.Delete(tempFile);
                }

            }
        }

        public void printGeneratedCode()
        {
            StreamWriter writer = new StreamWriter("out.txt");
            
            writer.WriteLine(this.literals.Count());
            writer.WriteLine(this.globalVariablesCounter);
            writer.WriteLine(this.mostVaribalesInAFunction);
            foreach (MachineInstructions.Literal literal in this.literals)
            {
                writer.WriteLine(literal);
            }

            writer.WriteLine(this.classDefinitions.Count());
            for (int i = 0; i < this.classDefinitions.Count(); i++)
            {
                writer.WriteLine(this.classDefinitions[i]);
            }

            writer.WriteLine(this.functionActions.Count());
            for (int i = 0; i < this.functionActions.Count(); i++)
            {
                List<MachineInstruction> funcDef = this.functionActions[i];
                writer.WriteLine(this.functionsParameters[i]);
                writer.WriteLine(funcDef.Count());
                foreach (MachineInstruction mi in funcDef)
                {
                    writer.WriteLine(mi);
                }
            }

            writer.WriteLine(this.topLeveIActions.Count());
            foreach (MachineInstruction m in this.topLeveIActions)
            {
                writer.WriteLine(m);
            }

            writer.Close();
        }
        public void addClassDefinition(int numberOfLAttributes)
        {
            this.classDefinitions.Add(numberOfLAttributes);
        }

        public void addAction(MachineInstruction action)
        {
            if (this.compilingTopLeveIActions)
            {
                this.topLeveIActions.Add(action);
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
        public void assignIdentifierToClass(LClass c)
        {
            if (c is LClassSpecial) return;

            c.identifier = this.classesCounter;
            this.classesCounter++;
        }
        public void assignIdentifierToProcedure(Procedure p)
        {
            if (p is FunctionSpecial || p is MethodSpecial || p is ConstructorSpecial) return;

            p.identifier = this.proceduresCounter;
            this.proceduresCounter++;
        }
        public void printSymbols()
        {
            Console.WriteLine("Classes:");
            this.program.classes.ForEach(c => Console.WriteLine(c));
            Console.WriteLine("Functions:");
            this.program.functions.ForEach(f => Console.WriteLine(f));
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
    }
}
