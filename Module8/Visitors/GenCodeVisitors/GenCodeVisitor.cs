﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;
using System.Reflection.Emit;

namespace SimpleLang.Visitors
{
    public class GenCodeVisitor: Visitor
    {
        private Dictionary<string, LocalBuilder> vars;
        private GenCodeCreator genc;

        public GenCodeVisitor()
        {
            vars = new Dictionary<string, LocalBuilder>();
            genc = new GenCodeCreator();
        }

        public override void VisitIdNode(IdNode id) 
        {
            // Этот Visit не вызывается если переменная стоит слева от оператора присваивания !
            // Т.е. он вызывается только если id находится в выражении, а значит, мы просто кладем его значение на стек!
            genc.Emit(OpCodes.Ldloc, vars[id.Name]);
        }

        public override void VisitIntNumNode(IntNumNode num) 
        {
            genc.Emit(OpCodes.Ldc_I4, num.Num);
        }

        public override void VisitBinOpNode(BinOpNode binop) 
        {
            binop.Left.Visit(this);
            binop.Right.Visit(this);
            switch (binop.Op)
            {
                case '+':
                    genc.Emit(OpCodes.Add);
                    break;
                case '-':
                    genc.Emit(OpCodes.Sub);
                    break;
                case '*':
                    genc.Emit(OpCodes.Mul);
                    break;
                case '/':
                    genc.Emit(OpCodes.Div);
                    break;
                case '%':
                    genc.Emit(OpCodes.Rem);
                    break;
            }
        }

        public override void VisitAssignNode(AssignNode a) 
        {
            a.Expr.Visit(this);
            genc.Emit(OpCodes.Stloc, vars[a.Id.Name]);
        }

        public override void VisitCycleNode(CycleNode c) 
        {
            var i = genc.DeclareLocal(typeof(int)); // переменная цикла cycle
            c.Expr.Visit(this); // сгенерировать команды, связанные с вычислением количества итераций цикла
            genc.Emit(OpCodes.Stloc, i); // i := кво итераций

            var beginLoop = genc.DefineLabel();
            var endLoop = genc.DefineLabel();
            
            genc.MarkLabel(beginLoop);

            genc.Emit(OpCodes.Ldloc, i); 
            genc.Emit(OpCodes.Ldc_I4_0);
            genc.Emit(OpCodes.Ble, endLoop); // if i<=0 then goto endLoop

            c.Stat.Visit(this); // выполнить тело цикла

            genc.Emit(OpCodes.Ldloc, i); // положить i на стек
            genc.Emit(OpCodes.Ldc_I4_1); // положить 1 на стек
            genc.Emit(OpCodes.Sub);
            genc.Emit(OpCodes.Stloc, i); // i := i - 1;

            genc.Emit(OpCodes.Br, beginLoop);

            genc.MarkLabel(endLoop);
        }

        public override void VisitIfNode(IfNode cond)
        {
            var ifFalse = genc.DefineLabel();
            var endIf = genc.DefineLabel();

            cond.Expr.Visit(this);
            genc.Emit(OpCodes.Ldc_I4_0);
            genc.Emit(OpCodes.Beq, ifFalse);

            cond.IfTrue.Visit(this);
            genc.Emit(OpCodes.Br, endIf);

            genc.MarkLabel(ifFalse);
            if (cond.IfFalse != null)
            {
                cond.IfFalse.Visit(this);
            }

            genc.MarkLabel(endIf);
        }

        public override void VisitWhileNode(WhileNode w)
        {
            var beginWhile = genc.DefineLabel();
            var endWhile = genc.DefineLabel();

            genc.MarkLabel(beginWhile);

            w.Expr.Visit(this);  // вычислить предусловие
            genc.Emit(OpCodes.Ldc_I4_0);
            genc.Emit(OpCodes.Beq, endWhile);  // проверить предусловие

            w.Stat.Visit(this);  // выполнить тело цикла
            genc.Emit(OpCodes.Br, beginWhile);

            genc.MarkLabel(endWhile);
        }

        public override void VisitRepeatNode(RepeatNode r)
        {
            var beginRepeat = genc.DefineLabel();

            genc.MarkLabel(beginRepeat);

            r.Stat.Visit(this);  // выполнить тело цикла

            r.Expr.Visit(this);  // вычислить постусловие
            genc.Emit(OpCodes.Ldc_I4_0);
            genc.Emit(OpCodes.Beq, beginRepeat);  // проверить постусловие
        }

        public override void VisitBlockNode(BlockNode bl) 
        {
            foreach (var st in bl.StList)
            {
                st.Visit(this);
            }
        }

        public override void VisitWriteNode(WriteNode w) 
        {
            w.Expr.Visit(this);
            genc.EmitWriteLine();
        }

        public override void VisitVarDefNode(VarDefNode w) 
        {
            foreach (var v in w.vars)
            {
                vars[v.Name] = genc.DeclareLocal(typeof(int));
            }
        }

        public void EndProgram()
        {
            genc.EndProgram();
        }

        public void RunProgram()
        {
            genc.RunProgram();
        }

        public void PrintCommands()
        {
            foreach (var s in genc.commands)
            {
                Console.WriteLine(s);
            }
        }
    }
}
