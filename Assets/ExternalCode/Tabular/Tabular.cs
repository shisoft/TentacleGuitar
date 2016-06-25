using System.Collections.Generic;

namespace TentacleGuitar.Tabular
{
    public class Tabular
    {
        public int BPM { get; set; }

        public Capo Capo { get; set; }

        public List<Staff> Staff { get; set; }

        public Dictionary<long, List<Note>> Notes { get; set; } 
    }
}
