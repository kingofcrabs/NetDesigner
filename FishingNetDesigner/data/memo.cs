using FishingNetDesigner.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingNetDesigner.Data
{
    class Memo
    {
        public Dictionary<string, List<Line>> HistoryLines { get; set; }
        const string Generated = "Generated";
        private int curActionIndex = 0;
        static Memo instance;
        public static Memo Instance
        {
            get
            {
                if (instance == null)
                    instance = new Memo();
                return instance;
            }
        }
        private Memo()
        {
            HistoryLines = new Dictionary<string, List<Line>>();
        }
        public void Create(List<Line> lines)
        {
            HistoryLines.Add(Generated, lines);
        }

        public void Update(List<Line> lines)
        {
            HistoryLines.Add(GetInfoString(), lines);
            curActionIndex = HistoryLines.Count - 1;
        }
        public List<Line> Redo()
        {
            return DoCommand(true);
        }

        public List<Line> Undo()
        {
            return DoCommand(false);
        }

        private List<Line> DoCommand(bool isRedo)
        {
            if (curActionIndex > HistoryLines.Count - 1)
                throw new Exception("已无有效操作！");
            string key = GetInfoString();
            curActionIndex += isRedo ? 1 : -1;
            return HistoryLines[key];
        }
       
        private string GetInfoString()
        {
            return string.Format("删除 {0}", curActionIndex);
        }
    }
}
