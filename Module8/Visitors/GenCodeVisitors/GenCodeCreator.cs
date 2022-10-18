using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;

namespace SimpleLang.Visitors
{
    class GenCodeCreator
    {
        private DynamicMethod dyn;
        private ILGenerator gen;
        private bool writeCommands = true;
        private static MethodInfo writeLineInt = typeof(Console).GetMethod("WriteLine", new Type[] { typeof(int) });

        public List<string> commands = new List<string>();

        public GenCodeCreator()
        {
            dyn = new DynamicMethod("My", null, null, typeof(void));
            gen = dyn.GetILGenerator();
        }

        public void Emit(OpCode op)
        {
            gen.Emit(op);
            if (writeCommands)
            {
                commands.Add(op.ToString());
            }
        }

        public void Emit(OpCode op, int num)
        {
            gen.Emit(op,num);
            if (writeCommands)
            {
                commands.Add(op.ToString() + " " + num);
            }
        }

        public void Emit(OpCode opCode, LocalBuilder localVariable)
        {
            gen.Emit(opCode, localVariable);
            if (writeCommands)
            {
                commands.Add(opCode.ToString() + " var" + localVariable.LocalIndex);
            }
        }

        public void Emit(OpCode op, Label label)
        {
            gen.Emit(op, label);
            if (writeCommands)
            {
                commands.Add(op.ToString() + " Label" + label.GetHashCode());
            }
        }

        public LocalBuilder DeclareLocal(Type type)
        {
            var localVariable = gen.DeclareLocal(type);
            if (writeCommands)
            {
                commands.Add("DeclareLocal " + "var" + localVariable.LocalIndex + ": " + type);
            }
            return localVariable;
        }

        public Label DefineLabel()
        {
            var label = gen.DefineLabel();
            if (writeCommands)
            {
                commands.Add("DefineLabel" + " Label" + label.GetHashCode());
            }
            return label;
        }

        public void MarkLabel(Label label)
        {
            gen.MarkLabel(label);
            if (writeCommands)
            {
                commands.Add("MarkLabel" + " Label" + label.GetHashCode());
            }
        }

        public void EmitWriteLine()
        {
            gen.Emit(OpCodes.Call, writeLineInt);
            if (writeCommands)
            {
                commands.Add("WriteLine");
            }
        }

        public void EndProgram()
        {
            gen.Emit(OpCodes.Ret);
        }

        public void RunProgram()
        {
            dyn.Invoke(null, null);
        }

        public void WriteCommandsOn()
        {
            writeCommands = true;
        }

        public void WriteCommandsOff()
        {
            writeCommands = false;
        }
    }
}
