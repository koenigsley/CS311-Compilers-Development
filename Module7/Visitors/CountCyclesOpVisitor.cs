using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    public class CountCyclesOpVisitor : AutoVisitor
    {
        private int _operatorsCount = 0;
        private int _сyclesVisited = 0;
        private int _cyclesEntered = 0;
        private bool InCycle => _cyclesEntered > 0;

        public int MidCount()
        {
            int midCount;
            if (_сyclesVisited != 0)
            {
                midCount = _operatorsCount / _сyclesVisited;
            }
            else
            {
                midCount = 0;
            }

            ResetCounters();

            return midCount;
        }

        public override void VisitBinOpNode(BinOpNode binop)
        {
            CountOperationIfNeeded();
            base.VisitBinOpNode(binop);
        }

        public override void VisitAssignNode(AssignNode a)
        {
            CountOperationIfNeeded();
            base.VisitAssignNode(a);
        }

        public override void VisitWriteNode(WriteNode w)
        {
            CountOperationIfNeeded();
            base.VisitWriteNode(w);
        }

        public override void VisitVarDefNode(VarDefNode w)
        {
            CountOperationIfNeeded();
            base.VisitVarDefNode(w);
        }

        public override void VisitCycleNode(CycleNode c)
        {
            _cyclesEntered += 1;
            c.Stat.Visit(this);
            _cyclesEntered -= 1;
            _сyclesVisited += 1;
        }

        private void CountOperationIfNeeded()
        {
            if (InCycle)
            {
                _operatorsCount += 1;
            }
        }

        private void ResetCounters()
        {
            _operatorsCount = 0;
            _сyclesVisited = 0;
            _cyclesEntered = 0;
        }
    }
}
