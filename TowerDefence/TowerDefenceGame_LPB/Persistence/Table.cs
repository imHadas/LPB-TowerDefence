using System.Collections;

namespace TowerDefenceGame_LPB.Persistence
{
    public class Table : IEnumerable
    {
        private Field[,] fields;

        public uint PhaseCounter { get; set; }  //named more accurately and changed to unsigned

        public (int x, int y) Size => (fields.GetLength(0), fields.GetLength(1));

        public Field this[uint x, uint y]
        {
            get { return fields[x, y]; }
            set { fields[x,y] = value; }
        }

        public Table(int height, int width)
        {
            fields = new Field[height, width];
        }

        public IEnumerator GetEnumerator()  // make it easier to iterate through all fields
        {
            throw new System.NotImplementedException();
        }
    }
}
