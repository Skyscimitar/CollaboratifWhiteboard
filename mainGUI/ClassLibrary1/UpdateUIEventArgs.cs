using System.Collections.Generic;
using ColoredForms;

namespace Network
{
    public class UpdateUIEventArgs
    {
        public string Type { get; set; }
        public int client_id { get; set; }
        public ColoredRectangle Rectangle { get; set; }
        public ColoredCircle Circle { get; set; }
        public ColoredLine Line { get; set; }
        public ColoredPath Path { get; set; }
        public List<object> Forms { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
    }
}
