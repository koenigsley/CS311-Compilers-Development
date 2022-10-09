using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    public class AssignCountVisitor : AutoVisitor
    {
        private int _count = 0;
        public int Count => _count;

        public override void VisitAssignNode(AssignNode a)
        {
            _count += 1;
        }

        public override void VisitWriteNode(WriteNode w)
        {
        }

        public override void VisitVarDefNode(VarDefNode w)
        {
        }

        public void Reset()
        {
            _count = 0;
        }
    }
}
