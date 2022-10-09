using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    public class MaxNestCyclesVisitor : AutoVisitor
    {
        protected int _currentDepth = 0;
        protected int _maxDepth = 0;
        public int MaxNest => _maxDepth;

        public override void VisitCycleNode(CycleNode c)
        {
            _currentDepth += 1;
            _maxDepth = Math.Max(_maxDepth, _currentDepth);
            base.VisitCycleNode(c);
            _currentDepth -= 1;
        }
    }
}
