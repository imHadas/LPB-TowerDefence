using System.Collections;

namespace TowerDefenceBackend.Persistence
{
    /// <summary>
    /// Class for storing all the <c>Field</c>s of the game board
    /// </summary>
    public class Table : IEnumerable
    {
        private Field[,] fields;

        /// <summary>
        /// The total number of phases the <c>Table</c> has 'been through'
        /// </summary>
        public uint PhaseCounter { get; set; }

        /// <summary>
        /// Get the size of the encapsulated 2d array as a tuple
        /// </summary>
        public (int x, int y) Size => (fields.GetLength(0), fields.GetLength(1));

        public Field this[uint x, uint y]
        {
            get { return fields[x, y]; }
            set { fields[x,y] = value; }
        }
        
        public Field this[(uint x, uint y) c]
        {
            get { return fields[c.x, c.y]; }
            set { fields[c.x,c.y] = value; }
        }

        public Table(uint height, uint width)
        {
            fields = new Field[height, width];
            PhaseCounter = 0;
        }

        public IEnumerator GetEnumerator()  // make it easier to iterate through all fields
        {
            for(uint i = 0; i < fields.GetLength(0); i++)
            {
                for(uint j = 0; j < fields.GetLength(1); j++)
                {
                    yield return fields[i,j];
                }
            }
        }
    }
}
