using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    public class CommonlyUsedVarVisitor : AutoVisitor
    {
        private Dictionary<string, int> _usedVars = new Dictionary<string, int>();

        public string MostCommonlyUsedVar => _usedVars.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;

        public override void VisitVarDefNode(VarDefNode w)
        {
            Add(w.vars.Select(v => v.Name));
            base.VisitVarDefNode(w);
        }

        public override void VisitIdNode(IdNode id)
        {
            if (IsVar(id.Name))
            {
                Add(id.Name);
            }
            base.VisitIdNode(id);
        }

        private bool IsVar(string idName)
        {
            return _usedVars.ContainsKey(idName);
        }

        private void Add(IEnumerable<string> varNames)
        {
            foreach (string varName in varNames)
            {
                Add(varName);
            }
        }

        private void Add(string varName)
        {
            if (!_usedVars.ContainsKey(varName))
            {
                _usedVars.Add(varName, 1);
            }
            else
            {
                _usedVars[varName] += 1;
            }
        }
    }
}
