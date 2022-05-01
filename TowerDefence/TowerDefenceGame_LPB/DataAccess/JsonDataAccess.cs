using System;
using System.Threading.Tasks;
using TowerDefenceBackend.Persistence;
using System.IO;
using System.Text.Json;  //use the .NET 6 serializer
using System.Collections.Generic;

namespace TowerDefenceBackend.DataAccess
{
    /// <summary>
    /// Abstraction for <c>Unit</c>
    /// </summary>
    internal class StringifiedUnit
    {
        public string Type { get; set; }
        public uint Health { get; set; }

        public StringifiedUnit()
        {

        }

        public StringifiedUnit(Unit unit)
        {
            Type = unit is TankUnit ? "TankUnit" : "BasicUnit";
            Health = unit.Health;
        }
    }

    /// <summary>
    /// Abstraction for <c>Placement</c>
    /// </summary>
    internal class StringifiedPlacement
    {
        public string Type { get; set; }
        public string Owner { get; set; }
        public uint Level { get; set; }

        public StringifiedPlacement()
        {

        }

        public StringifiedPlacement(Placement placement)
        {
            Type = placement.GetType().Name;
            Owner = placement.OwnerType.ToString();
            Level = placement switch
            {
                Tower tower => tower.Level,
                Terrain terrain => terrain.NumericType,
                _ => 0,
            };
        }
    }

    /// <summary>
    /// Abstraction for Field
    /// </summary>
    internal class StringifiedField
    {
        public (uint x, uint y) Coords { get; set; }
        public StringifiedPlacement? Placement { get; set; }
        public (ICollection<StringifiedUnit> red, ICollection<StringifiedUnit> blue) Units { get; set; }

        public StringifiedField()
        {

        }

        public StringifiedField(Field field)
        {
            Coords = field.Coords;
            if (field.Placement != null)
                Placement = new(field.Placement);
            else 
                Placement = null;
            
            List<StringifiedUnit> red = new();
            List<StringifiedUnit> blue = new();
            foreach(Unit unit in field.Units)
            {
                if(unit.Owner.Type == PlayerType.RED)
                {
                    red.Add(new(unit));
                }
                else
                {
                    blue.Add(new(unit));
                }
            }

            Units = (red, blue);
        }
    }


    /// <summary>
    /// Abstraction for Table.
    /// Needed, because you cannot serialize multi-dimensional arrays
    /// </summary>
    internal class StringifiedTable
    {
        public int Width { get; init; }
        public int Height { get; init; }

        public StringifiedField[] Fields { get; set; }

        public StringifiedTable()
        {

        }

        public StringifiedTable(int width, int height)
        {
            Width = width;
            Height = height;
            Fields = new StringifiedField[Width * Height];
        }
        public StringifiedField this[uint x, uint y]
        {
            get => Fields[IndexFromCoords(x, y)];
            set => Fields[IndexFromCoords(x, y)] = value;
        }

        public StringifiedField this[int x, int y]
        {
            get => this[(uint)x, (uint)y];
            set => this[(uint)x, (uint)y] = value;
        }


        public StringifiedField this[(int x, int y) coords]
        {
            get => this[coords.x, coords.y];
            set => this[coords.x, coords.y] = value;
        }

        public StringifiedField this[(uint x, uint y) coords]
        {
            get => this[coords.x, coords.y];
            set => this[coords.x, coords.y] = value;
        }

        private uint IndexFromCoords(uint x, uint y)
        {
            return (uint)(x * Width + y);
        }
    }

    /// <summary>
    /// Abstraction for <c>GameSaveObject</c>
    /// </summary>
    internal class StringifiedGSO
    {
        public StringifiedTable Table { get; set; }
        public uint BPMoney { get; set; }
        public uint RPMoney { get; set; }
        public uint PhaseCounter { get; set; }

        public StringifiedGSO()
        {

        }

        public StringifiedGSO(Table table, Player blueplayer, Player redplayer)
        {
            Table = new (table.Size.y, table.Size.x);
            for (uint i = 0; i < table.Size.x; i++)
            {
                for(uint j = 0; j < table.Size.y; j++)
                {
                    Table[i, j] = new StringifiedField(table[i, j]);
                }
            }
            BPMoney = blueplayer.Money;
            RPMoney = redplayer.Money;
            PhaseCounter = table.PhaseCounter;
        }
    }

    /// <summary>
    /// Implementation of <c>IDataAccess</c> using <c>JSON</c> serialization
    /// </summary>
    public class JsonDataAccess : IDataAccess<GameSaveObject>
    {
        private static readonly JsonSerializerOptions _options
        = new()
        {
            PropertyNameCaseInsensitive = false,    // just in case
            WriteIndented = true,                   // pretty print
            IncludeFields = true,                   // neccessary for tuples and the like
        };

        /// <summary>
        /// Implementation of loading
        /// </summary>
        /// <param name="path">Path of the file to load</param>
        /// <returns>A <c>GameSaveObject</c> containing the game's state</returns>
        /// <exception cref="IOException">Thrown if the <c>Serializer</c> fails to parse the file</exception>
        public async Task<GameSaveObject> LoadAsync(string path)
        {
            StringifiedGSO? sgso;

            using (FileStream fs = File.OpenRead(path))
            {
                sgso = await JsonSerializer.DeserializeAsync<StringifiedGSO>(fs, _options);
            }

            if (sgso == null) throw new IOException("Error reading file");

            return ConstructGSO(sgso);

            return new GameSaveObject(new Table(0,0),new Player(PlayerType.BLUE), new Player(PlayerType.RED));
        }


        /// <summary>
        /// Constructs Field for output Table. Inserts Units and Placements into Players as side effect.
        /// </summary>
        /// <param name="sf">Stringified Field to be parsed</param>
        /// <param name="bp">Reference to output blue Player</param>
        /// <param name="rp">Reference to output red Player</param>
        /// <returns>Parsed Field</returns>
        private static Field ConstructField(StringifiedField sf, Player bp, Player rp)
        {
            Field output = new(sf.Coords.x, sf.Coords.y);
            foreach(StringifiedUnit unit in sf.Units.red)
            {
                Unit nu;
                if (unit.Type == "TankUnit")
                    nu = new TankUnit(rp);
                else
                    nu = new BasicUnit(rp);
                rp.Units.Add(nu);
                output.Units.Add(nu);
            }

            foreach (StringifiedUnit unit in sf.Units.blue)
            {
                Unit nu;
                if (unit.Type == "TankUnit")
                    nu = new TankUnit(bp);
                else
                    nu = new BasicUnit(bp);
                bp.Units.Add(nu);
                output.Units.Add(nu);
            }

            if(sf.Placement is not null)
            {
                Player? owner;
                switch(sf.Placement.Owner)
                {
                    case "RED": owner = rp; break;
                    case "BLUE": owner = bp; break;
                    case "NEUTRAL": owner = null; break;
                    default: throw new Exception($"Owner of placement couldn't be parsed\n{sf.Placement.Owner}");
                }

                if(owner is null)
                {
                    if (sf.Placement.Type != "Terrain") throw new Exception($"Placement of type {sf.Placement.Type} must have owner");
                    output.Placement = new Terrain(sf.Coords.x, sf.Coords.y, (TerrainType)sf.Placement.Level);
                }

                else switch(sf.Placement.Type)
                {
                    case "Castle":
                            if (owner.Castle != null) 
                                throw new Exception($"{owner.Type} has multiple castles");
                            owner.Castle = new(owner, sf.Coords.x, sf.Coords.y);
                            output.Placement = owner.Castle;
                        break;
                    case "Barrack":
                            try
                            {
                                output.Placement = new Barrack(owner, sf.Coords.x, sf.Coords.y);
                                owner.Barracks.Add((Barrack)output.Placement);
                            }
                            catch (ArgumentException)
                            {
                                throw new Exception($"{owner.Type} would have more than 2 barracks");
                            }
                        break;
                    case "BasicTower":
                            output.Placement = new BasicTower(owner, (sf.Coords.x, sf.Coords.y));
                            owner.Towers.Add((Tower)output.Placement);
                        break;
                    case "SniperTower":
                            output.Placement = new SniperTower(owner, (sf.Coords.x, sf.Coords.y));
                            owner.Towers.Add((Tower)output.Placement);
                        break;
                    case "BomberTower":
                            output.Placement = new BomberTower(owner, (sf.Coords.x, sf.Coords.y));
                            owner.Towers.Add((Tower)output.Placement);
                        break;
                }
            }
            else output.Placement = null;


            return output;
        }

        /// <summary>
        /// Constructs <c>GameSaveObject</c> from it's stringified version
        /// </summary>
        /// <param name="sgso">Stringified version of <c>GameSaveObject</c> to be converted</param>
        /// <returns>A <c>GameSaveObject</c> with parsed values</returns>
        /// <exception cref="ArgumentException">Thrown if the input was invalid</exception>
        private static GameSaveObject ConstructGSO(StringifiedGSO sgso)
        {
            Table table = new((uint)sgso.Table.Height, (uint)sgso.Table.Width);
            Player bp = new(PlayerType.BLUE);
            Player rp = new(PlayerType.RED);

            for(uint i = 0; i < table.Size.x; ++i)
            {
                for(uint j = 0; j < table.Size.y; ++j)
                {
                    table[i,j] = ConstructField(sgso.Table[i, j], bp, rp);
                }
            }

            bp.Money = sgso.BPMoney;
            rp.Money = sgso.RPMoney;
            table.PhaseCounter = sgso.PhaseCounter;
            if (bp.Castle == null || rp.Castle == null || bp.Barracks.Count < 2 || rp.Barracks.Count < 2) throw new ArgumentException("Read table was invalid");

            return new GameSaveObject(table, bp, rp);
        }

        /// <summary>
        /// Implementation of saving
        /// </summary>
        /// <param name="path">Path of file to save to</param>
        /// <param name="gameSaveObject"><c>GameSaveObject</c> containing gamestate to save</param>
        /// <returns></returns>
        public async Task SaveAsync(string path, GameSaveObject gameSaveObject)
        {
            StringifiedGSO sgso = new(
                gameSaveObject.Table,
                gameSaveObject.BluePlayer,
                gameSaveObject.RedPlayer
                );

            using FileStream fs = File.Create(path);
            await JsonSerializer.SerializeAsync(fs, sgso, _options);
        }
    }
}
