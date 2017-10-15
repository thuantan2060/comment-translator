using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommentTranslator.Parsers
{
    public class Comment
    {
        public string Origin { get; set; }
        public string Content { get; set; }
        public TextPositions Position { get; set; }
        public int Line { get; set; }
        public int MarginTop { get; set; }
    }
}
