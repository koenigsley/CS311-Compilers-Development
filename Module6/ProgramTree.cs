using System.Collections.Generic;

namespace ProgramTree
{
    public enum AssignType { Assign, AssignPlus, AssignMinus, AssignMult, AssignDivide };

    public class Node // базовый класс для всех узлов    
    {
    }

    public class ExprNode : Node // базовый класс для всех выражений
    {
    }

    public class IdNode : ExprNode
    {
        public string Name { get; set; }
        public IdNode(string name) { Name = name; }
    }

    public class IntNumNode : ExprNode
    {
        public int Num { get; set; }
        public IntNumNode(int num) { Num = num; }
    }

    public class StatementNode : Node // базовый класс для всех операторов
    {
    }

    public class AssignNode : StatementNode
    {
        public IdNode Id { get; set; }
        public ExprNode Expr { get; set; }
        public AssignType AssOp { get; set; }
        public AssignNode(IdNode id, ExprNode expr, AssignType assop = AssignType.Assign)
        {
            Id = id;
            Expr = expr;
            AssOp = assop;
        }
    }

    public class CycleNode : StatementNode
    {
        public ExprNode Expr { get; set; }
        public StatementNode Stat { get; set; }
        public CycleNode(ExprNode expr, StatementNode stat)
        {
            Expr = expr;
            Stat = stat;
        }
    }

    public class WhileNode: StatementNode
    {
        public ExprNode Expr { get; set; }
        public StatementNode Stat { get; set; }
        public WhileNode(ExprNode expr, StatementNode stat)
        {
            Expr = expr;
            Stat = stat;
        }
    }

    public class RepeatNode : StatementNode
    {
        public StatementNode Stat { get; set; }
        public ExprNode Expr { get; set; }
        public RepeatNode(StatementNode stat, ExprNode expr)
        {
            Stat = stat;
            Expr = expr;
        }
    }

    public class ForNode : StatementNode
    {
        public IdNode Id { get; set; }
        public ExprNode Expr1 { get; set; }
        public ExprNode Expr2 { get; set; }
        public StatementNode Stat { get; set; }
        public ForNode(IdNode id, ExprNode expr1, ExprNode expr2, StatementNode stat)
        {
            Id = id;
            Expr1 = expr1;
            Expr2 = expr2;
            Stat = stat;
        }
    }

    public class BlockNode : StatementNode
    {
        public List<StatementNode> StList = new List<StatementNode>();
        public BlockNode(StatementNode stat)
        {
            Add(stat);
        }
        public void Add(StatementNode stat)
        {
            StList.Add(stat);
        }
    }

    public class WriteNode : StatementNode
    {
        public ExprNode Expr { get; set; }

        public WriteNode(ExprNode expr)
        {
            Expr = expr;
        }
    }

    public class IfNode : StatementNode
    {
        public ExprNode Expr { get; set; }
        public StatementNode Stat1 { get; set; }
        public StatementNode Stat2 { get; set; }

        public IfNode(ExprNode expr, StatementNode stat1, StatementNode stat2 = null)
        {
            Expr = expr;
            Stat1 = stat1;
            Stat2 = stat2;
        }
    }

    public class VarDefNode : StatementNode
    {
        public List<IdNode> IdList = new List<IdNode>();

        public VarDefNode(IdNode id)
        {
            Add(id);
        }

        public void Add(IdNode id)
        {
            IdList.Add(id);
        }
    }

    public class BinaryNode : ExprNode
    {
        ExprNode LeftExpr { get; set; }
        ExprNode RightExpr { get; set; }
        char Operation { get; set; }

        public BinaryNode(ExprNode leftExpr, ExprNode rightExpr, char operation)
        {
            LeftExpr = leftExpr;
            RightExpr = rightExpr;
            Operation = operation;
        }
    }
}