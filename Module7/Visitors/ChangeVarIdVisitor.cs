using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    public class ChangeVarIdVisitor : AutoVisitor
    {
        private string _from, _to;
        private HashSet<string> _vars = new HashSet<string>();

        public ChangeVarIdVisitor(string from, string to)
        {
            _from = from;
            _to = to;
        }

        public override void VisitVarDefNode(VarDefNode w)
        {
            w.vars.ForEach(v => Add(v.Name));
            base.VisitVarDefNode(w);
        }

        public override void VisitIdNode(IdNode id)
        {
            if (IsVar(id.Name))
            {
                RenameIfNeeded(id);
            }
        }

        private void Add(string varName)
        {
            _vars.Add(varName);
        }

        private bool IsVar(string idName)
        {
            return _vars.Contains(idName);
        }

        private void RenameIfNeeded(IdNode id)
        {
            if (id.Name == _from)
            {
                id.Name = _to;
            }
        }
    }
}
