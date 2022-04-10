using System;
using System.Threading.Tasks;
using TowerDefenceGame_LPB.Persistence;
using System.IO;
using System.Text.Json;  //use the .NET 6 serializer
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace TowerDefenceGame_LPB.DataAccess
{
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
            if(placement is Tower) Level = ((Tower)placement).Level;
            else if(placement is Terrain) Level = (uint)((Terrain)placement).Type;  //a bit hacky
            else Level = 0;
        }
    }

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
    /// You cannot serialize multi-dimensional arrays, that's why
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

    public class JsonDataAccess : IDataAccess<GameSaveObject>
    {
        private static readonly JsonSerializerOptions _options
        = new()
        {
            //ReferenceHandler = ReferenceHandler.Preserve,
            PropertyNameCaseInsensitive = false,
            WriteIndented = true,
            IncludeFields = true,
        };

        public async Task<GameSaveObject> LoadAsync(string path)
        {
            StringifiedGSO? sgso;

            using (FileStream fs = File.OpenRead(path))
            {
                sgso = await JsonSerializer.DeserializeAsync<StringifiedGSO>(fs, _options);
            }

            if (sgso == null) throw new Exception("Error reading file");

            return ConstructGSO(sgso);

        }


        /// <summary>
        /// Constructs Field for output Table. Inserts Units and Placements into Players as side effect.
        /// </summary>
        /// <param name="sf">Stringified Field to be parsed</param>
        /// <param name="bp">Reference to output blue Player</param>
        /// <param name="rp">Reference to output red Player</param>
        /// <returns>Parsed Field</returns>
        private Field ConstructField(StringifiedField sf, Player bp, Player rp)
        {
            Field output = new Field(sf.Coords.Item1, sf.Coords.Item2);
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
                    output.Placement = new Terrain(sf.Coords.Item1, sf.Coords.Item2, (TerrainType)sf.Placement.Level);
                }

                else switch(sf.Placement.Type)
                {
                    case "Castle":
                            if (owner.Castle != null) 
                                throw new Exception($"{owner.Type} has multiple castles");
                            owner.Castle = new(owner, sf.Coords.Item1, sf.Coords.Item2);
                            output.Placement = owner.Castle;
                        break;
                    case "Barrack":
                            try
                            {
                                output.Placement = new Barrack(owner, sf.Coords.Item1, sf.Coords.Item2);
                                owner.AddBarrack((Barrack)output.Placement);
                            }
                            catch (ArgumentException)
                            {
                                throw new Exception($"{owner.Type} would have more than 2 barracks");
                            }
                        break;
                    case "BasicTower":
                            output.Placement = new BasicTower(owner, (sf.Coords.Item1, sf.Coords.Item2));
                            owner.Towers.Add((Tower)output.Placement);
                        break;
                    case "SniperTower":
                            output.Placement = new SniperTower(owner, (sf.Coords.Item1, sf.Coords.Item2));
                            owner.Towers.Add((Tower)output.Placement);
                        break;
                    case "BomberTower":
                            output.Placement = new BomberTower(owner, (sf.Coords.Item1, sf.Coords.Item2));
                            owner.Towers.Add((Tower)output.Placement);
                        break;
                }
            }
            else output.Placement = null;


            return output;
        }

        private GameSaveObject ConstructGSO(StringifiedGSO sgso)
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
            if (bp.Castle == null || rp.Castle == null || bp.Barracks.Count < 2 || rp.Barracks.Count < 2) throw new Exception("Read table was invalid");

            return new GameSaveObject(table, bp, rp);
        }

        public async Task SaveAsync(string path, GameSaveObject gameSaveObject)
        {
            StringifiedGSO sgso = new(
                gameSaveObject.Table,
                gameSaveObject.BluePlayer,
                gameSaveObject.RedPlayer
                );

            using (FileStream fs = File.Create(path))
            {
                await JsonSerializer.SerializeAsync(fs, sgso, _options);
            }
        }
    }
}
