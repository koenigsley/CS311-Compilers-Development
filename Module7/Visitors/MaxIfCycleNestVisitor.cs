using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;


namespace SimpleLang.Visitors
{
    public class MaxIfCycleNestVisitor : MaxNestCyclesVisitor
    {
        public override void VisitIfNode(IfNode i)
        {
            _currentDepth += 1;
            _maxDepth = Math.Max(_maxDepth, _currentDepth);
            base.VisitIfNode(i);
            _currentDepth -= 1;
        }
    }
}
