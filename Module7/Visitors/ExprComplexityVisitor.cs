using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    public class ExprComplexityVisitor : AutoVisitor
    {
        private int _currentComplexity = 0;
        private List<int> _complexityList = new List<int>();
        // список должен содержать сложность каждого выражения, встреченного при обычном порядке обхода AST
        public List<int> ComplexityList => _complexityList;

        public override void VisitBinOpNode(BinOpNode binop)
        {
            _currentComplexity += GetComplexity(binop.Op);
            base.VisitBinOpNode(binop);
        }

        public override void VisitAssignNode(AssignNode a)
        {
            VisitExpression(a.Expr);
        }

        public override void VisitCycleNode(CycleNode c)
        {
            VisitExpression(c.Expr);
            c.Stat.Visit(this);
        }

        public override void VisitWriteNode(WriteNode w)
        {
            VisitExpression(w.Expr);
        }

        private static int GetComplexity(char operation)
        {
            if (operation == '+' || operation == '-')
            {
                return 1;
            }

            if (operation == '*' || operation == '/')
            {
                return 3;
            }

            throw new ArgumentException("Unknown operation");
        }

        private void VisitExpression(ExprNode exprNode)
        {
            exprNode.Visit(this);
            AddComplexity();
        }

        private void AddComplexity()
        {
            _complexityList.Add(_currentComplexity);
            _currentComplexity = 0;
        }
    }
}
