using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerDefenceGame_LPB.Persistence;
using TowerDefenceGame_LPB.DataAccess;

namespace TowerDefenceGame_LPB.Model
{
    
    public class GameModel : ModelBase
    {
        #region Variables

        //Table Table; // ModelBase has a Table
        private Player rp;
        private Player bp;
        //Player ne;

        #endregion

        #region Properties

        public bool SaveEnabled { get; private set; }
        public int Round { get; set; }
        public int Phase { get; set; }
        public Player CurrentPlayer { get; private set; }

        public Player OtherPlayer => CurrentPlayer == rp ? bp : rp;

        // public Table Table { get { return table; } } // ModelBase already has a Table

        #endregion

        public GameModel(IDataAccess dataAccess)
        {
            //ne = new Player(PlayerType.NEUTRAL); // do we need a neutral player? we could just make placement owner nullable

            /*Table = new Table(11, 11);
            ICollection<Barrack> rBarracks = new HashSet<Barrack>();
            ICollection<Barrack> bBarracks = new HashSet<Barrack>();
            rp = new Player(PlayerType.RED, rBarracks);
            bp = new Player(PlayerType.BLUE, bBarracks);
            rBarracks.Add(new Barrack(rp, 9, 9)); rBarracks.Add(new Barrack(rp, 9, 1));
            bBarracks.Add(new Barrack(bp, 1, 1)); bBarracks.Add(new Barrack(bp, 1, 9));
            currentPlayer = bp;
            SetupTable();*/

            NewGame();
        }

        public void NewGame()
        {
            Table = new Table(11, 11);
            ICollection<Barrack> rBarracks = new HashSet<Barrack>();
            ICollection<Barrack> bBarracks = new HashSet<Barrack>();
            rBarracks.Add(new Barrack(rp,9,9)); rBarracks.Add(new Barrack(rp,9,1));
            bBarracks.Add(new Barrack(bp,1,1)); bBarracks.Add(new Barrack(bp,1,9));
            rp = new Player(PlayerType.RED, new(rp, 9, 5) ,rBarracks);
            bp = new Player(PlayerType.BLUE, new(bp, 1, 4), bBarracks);
            CurrentPlayer = bp;
            SetupTable();
            SetupBarracks(rp);
            SetupBarracks(bp);
        }

        public void Advance()
        {

        }

        private void MoveUnits()
        {
            
        }

        private void SetupBarracks(Player player)
        {
            foreach(Barrack barrack in player.Barracks)
            {
                uint x = barrack.Coords.x;
                uint y = barrack.Coords.y;
                //table.fields[(uint)x, (uint)y] = new Field(x,y);
                Table[x, y].Placement = new Barrack(player, x, y);
            }
        }

        public override void SelectOption(MenuOption option)
        {
            switch (option)
            {
                case MenuOption.TrainBasic:
                    PlaceUnit(new BasicUnit(CurrentPlayer));
                    break;
                case MenuOption.TrainTank:
                    PlaceUnit(new BasicUnit(CurrentPlayer));
                    break;
                case MenuOption.BuildSniper:
                    break;
                case MenuOption.BuildBasic:
                    break;
                case MenuOption.BuildBomber:
                    break;
                case MenuOption.UpgradeTower:
                    break;
                case MenuOption.DestroyTower:
                    break;
                case MenuOption.ShowUnits:
                    break;
                default:
                    break;
            }
                
        }

        private void PlaceUnit(Unit unit)
        {
            var coords = CurrentPlayer.Barracks
                    .ElementAt(new Random().Next(1)).WhereToPlace;

            if (Table[coords].Placement != null)
                throw new Exception("Unit cannot be placed on (" + coords.Item1 + ";" + coords.Item2 + ")");

            if (CurrentPlayer.Money < unit.Cost)
                throw new Exception("Player does not have enough money to buy unit");
            
            Table[coords].Units.Add(unit);
            unit.NewPath(FindPath(coords, OtherPlayer.Castle.Coords));
        }

        /// <summary>
        /// Method for selecting a field model-side
        /// </summary>
        /// <param name="field">The field to be selected</param>
        /// <returns>List of possible menu options for a field</returns>
        public override ICollection<MenuOption> SelectField(Field field)
        {
            selectedField = field;
            ICollection<MenuOption> options = new List<MenuOption>();
            if(selectedField.Placement is null)
            {
                if (selectedField.Units.Count != 0)
                    options.Add(MenuOption.ShowUnits);
                else
                {
                    options.Add(MenuOption.BuildBasic);
                    options.Add(MenuOption.BuildBomber);
                    options.Add(MenuOption.BuildSniper);
                }
            }
            else if(selectedField.Placement.Owner == CurrentPlayer) 
                switch(selectedField.Placement)
                {
                    case Tower:
                        options.Add(MenuOption.DestroyTower);
                        if (((Tower)selectedField.Placement).Level < Constants.MAX_TOWER_LEVEL)
                            options.Add(MenuOption.UpgradeTower); 
                        break;
                    case Castle: /*FALLTHROUGH*/
                    case Barrack:
                        options.Add(MenuOption.TrainBasic);
                        options.Add(MenuOption.TrainTank);
                        break;
                }

            return options;
        }
    }
}
