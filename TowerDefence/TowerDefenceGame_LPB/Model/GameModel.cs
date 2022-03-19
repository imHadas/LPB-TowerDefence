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
        private IList<Unit> AvailableUnits;
        //Player ne;

        #endregion

        #region Properties

        public bool SaveEnabled { get; private set; }
        public bool BuildEnabled { get; set; }
        public int Round { get; set; }
        public int Phase { get; set; }
        public Player CurrentPlayer { get; private set; }

        public Player OtherPlayer => CurrentPlayer == rp ? bp : rp;

        // public Table Table { get { return table; } } // ModelBase already has a Table

        #endregion

        #region Events

        public event EventHandler<(ICollection<Unit> friendly, ICollection<Unit> enemy)> ShowUnits;

        #endregion

        public GameModel(IDataAccess dataAccess)  // it says some fields must contain a non-null value, so we should check this sometime!
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
            Phase = 1;
            Round = 1;
            //Table = new Table(11, 11);
            SetupTable(11,11);
            ICollection<Barrack> rBarracks = new HashSet<Barrack>();
            ICollection<Barrack> bBarracks = new HashSet<Barrack>();
            rBarracks.Add(new Barrack(rp,9,9)); rBarracks.Add(new Barrack(rp,9,1));
            bBarracks.Add(new Barrack(bp,1,1)); bBarracks.Add(new Barrack(bp,1,9));
            rp = new Player(PlayerType.RED, new(rp, 9, 5) ,rBarracks);
            bp = new Player(PlayerType.BLUE, new(bp, 1, 5), bBarracks);
            CurrentPlayer = bp;
            SetupCastles();
            SetupBarracks(rp);
            SetupBarracks(bp);
        }

        /// <summary>
        /// Method for end turn.
        /// Checks Phase and sets Round.
        /// In attack phase, it disables building and it calls Attack().
        /// </summary>
        public void Advance()
        {
            Phase++;
            if(Phase % 3 == 0)
            {
                BuildEnabled = false; // in attack phase you can't build, or place units
                Round++;
                Attack();
            }
            else if(Phase % 2 == 0)
            {
                BuildEnabled=true;
                CurrentPlayer = bp;
            }
            else if(Phase % 2 == 1)
            {
                BuildEnabled = true;
                CurrentPlayer = rp;
            }
        }

        /// <summary>
        /// Checks for new path, calls MoveUnits() and FireTowers() untill there are available units.
        /// </summary>
        private void Attack()
        {
            AvailableUnits = new List<Unit>(); // List for available units

            for (int i = 0; i < Table.Size.x; i++)
            {
                for (int j = 0; j < Table.Size.y; j++)
                {
                    if (Table[(uint)i, (uint)j].Units.Count > 0)
                    {
                        IList<(uint x, uint y)> rpPath = null;
                        IList<(uint x, uint y)> bpPath = null;
                        bool rpTobp = false;
                        bool bpTorp = false;
                        foreach (Unit unit in Table[(uint)i, (uint)j].Units)
                        {
                            if (bpTorp && rpTobp)
                                break;
                            else if (!bpTorp && unit.Owner == bp)
                            {
                                bpTorp = true;
                            }
                            else if (!rpTobp && unit.Owner == rp)
                            {
                                rpTobp = true;
                            }
                        }
                        if (bpTorp || rpTobp)
                        {
                            if (rpTobp)
                                rpPath = FindPath(Table[(uint)i, (uint)j].Coords, rp.Castle.Coords);
                            if (bpTorp)
                                bpPath = FindPath(Table[(uint)i, (uint)j].Coords, rp.Castle.Coords);
                            foreach (Unit unit in Table[(uint)i, (uint)j].Units)
                            {
                                AvailableUnits.Add(unit); // add unit to list

                                if (unit.Owner == bp)
                                {
                                    unit.NewPath(bpPath);
                                }
                                else if (unit.Owner == rp)
                                {
                                    unit.NewPath(rpPath);
                                }
                            }
                        }
                    }
                }
            } // sets new path for all units

            while(AvailableUnits.Count > 0)
            {
                MoveUnits();
                // FireTower();
            }
        }

        /// <summary>
        /// Method for moving the units on the table model side.
        /// Moves the units untill they run out of stamina, resets it and removes them from AvailableUnits.
        /// </summary>
        private async void MoveUnits()
        {
            await Task.Delay(500); // waits 500 millisec/ 0.5 sec

            foreach (Unit unit in AvailableUnits) // move units of RED player by one if they have stamina
            {
                if (unit.Stamina > 0)
                {
                    Table[unit.Path.First.Value.x, unit.Path.First.Value.y].Units.Remove(unit);
                    unit.Moved();
                    Table[unit.Path.First.Value.x, unit.Path.First.Value.y].Units.Add(unit);
                }
                if (unit.Stamina == 0)
                {
                    unit.ResetStamina();
                    AvailableUnits.Remove(unit);
                }
            }

            return;
        }

        private void SetupBarracks(Player player)
        {
            foreach(Barrack barrack in player.Barracks)
            {
                uint x = barrack.Coords.x;
                uint y = barrack.Coords.y;
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
                    BuildTower(new SniperTower(CurrentPlayer, SelectedField.Coords));
                    break;
                case MenuOption.BuildBasic:
                    BuildTower(new BasicTower(CurrentPlayer, SelectedField.Coords));
                    break;
                case MenuOption.BuildBomber:
                    BuildTower(new BomberTower(CurrentPlayer, SelectedField.Coords));
                    break;
                case MenuOption.UpgradeTower:
                    break;
                case MenuOption.DestroyTower:
                    break;
                /*case MenuOption.ShowUnits:      //not an option, but a consequence of selecting a field with units
                    OnShowUnit();
                    break;*/
                default:
                    break;
            }
                
        }

        private void PlaceUnit(Unit unit)
        {
            var coords = CurrentPlayer.Barracks
                    .ElementAt(new Random().Next(1)).WhereToPlace;

            if (Table[coords].Placement != null)
                throw new InvalidPlacementException(Table[coords], "Unit cannot be placed on (" + coords.Item1 + ";" + coords.Item2 + ")");

            if (CurrentPlayer.Money < unit.Cost)
                throw new NotEnoughMoneyException(CurrentPlayer.Money, unit.Cost, "Player does not have enough money to buy unit");
            
            Table[coords].Units.Add(unit);
            unit.NewPath(FindPath(coords, OtherPlayer.Castle.Coords));
            CurrentPlayer.Units.Add(unit);
            CurrentPlayer.Money -= unit.Cost;
        }

        private void BuildTower(Tower tower)
        {
            if (SelectedField.Placement != null)
                throw new InvalidPlacementException(SelectedField, "Cannot build tower on non-empty field");
            if (SelectedField.Units.Count > 0)
                throw new InvalidPlacementException(SelectedField, "Cannot build tower on field that contains units");
            if (CurrentPlayer.Money < tower.Cost)
                throw new NotEnoughMoneyException(CurrentPlayer.Money, tower.Cost, "Player does not have enough money to build this tower");

            SelectedField.Placement = tower;
            CurrentPlayer.Towers.Add(tower);
            CurrentPlayer.Money -= tower.Cost;
        }

        /// <summary>
        /// Method for selecting a field model-side
        /// </summary>
        /// <param name="field">The field to be selected</param>
        /// <returns>List of possible menu options for a field</returns>
        public override ICollection<MenuOption> SelectField(Field field)
        {
            SelectedField = field;
            ICollection<MenuOption> options = new List<MenuOption>();
            if(SelectedField.Placement is null)
            {
                if (SelectedField.Units.Count != 0)
                    OnShowUnit();
                else
                {
                    if (!BuildEnabled)  // you can still view units but can't build
                        return options;
                    options.Add(MenuOption.BuildBasic);
                    options.Add(MenuOption.BuildBomber);
                    options.Add(MenuOption.BuildSniper);
                }
            }
            else if(SelectedField.Placement.Owner == CurrentPlayer)
            {
                if (!BuildEnabled)  // you can still view units but can't upgrade, or destroy towers, or place units
                    return options;
                switch (SelectedField.Placement)
                {
                    case Tower:
                        options.Add(MenuOption.DestroyTower);
                        if (((Tower)SelectedField.Placement).Level < Constants.MAX_TOWER_LEVEL)
                            options.Add(MenuOption.UpgradeTower);
                        break;
                    case Castle: /*FALLTHROUGH*/
                    case Barrack:
                        options.Add(MenuOption.TrainBasic);
                        options.Add(MenuOption.TrainTank);
                        break;
                }
            } 
                

            return options;
        }

        private void OnShowUnit()
        {
            ShowUnits?.Invoke(this,
                (SelectedField.Units.Intersect(CurrentPlayer.Units).ToList(),
                SelectedField.Units.Intersect(OtherPlayer.Units).ToList())
                );
        }
    }
}
