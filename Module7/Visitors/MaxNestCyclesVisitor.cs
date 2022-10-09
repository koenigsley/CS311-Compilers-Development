using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    public class MaxNestCyclesVisitor : AutoVisitor
    {
        private int _currentCycleDepth = 0;
        private int _maxCycleDepth = 0;
        public int MaxNest => _maxCycleDepth;

        public override void VisitCycleNode(CycleNode c)
        {
            _currentCycleDepth += 1;
            _maxCycleDepth = Math.Max(_maxCycleDepth, _currentCycleDepth);
            base.VisitCycleNode(c);
            _currentCycleDepth -= 1;
        }
    }
}
