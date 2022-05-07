using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TowerDefenceBackend.Persistence;
using TowerDefenceBackend.DataAccess;


namespace TowerDefenceBackend.Model
{
    public class GameModel : ModelBase
    {
        #region Fields

        private Player rp;
        private Player bp;
        private IList<Unit> AvailableUnits;

        #endregion

        #region Properties

        public bool SaveEnabled { get; private set; }
        public bool BuildEnabled { get; set; }
        public bool GameOverProp { get; private set; }
        public uint Round { get { return Table.PhaseCounter / 3 + 1; } }
        public uint Phase
        {
            get { return Table.PhaseCounter; }
            set { Table.PhaseCounter = value; }
        }
        public Player CurrentPlayer { get; private set; }

        public Player OtherPlayer => CurrentPlayer == rp ? bp : rp;

        #endregion

        #region Events

        public event EventHandler<(ICollection<Unit> friendly, ICollection<Unit> enemy)> ShowUnits;
        public event EventHandler TowerFired;
        public event EventHandler UnitMoved;
        public event EventHandler AttackEnded;
        public event EventHandler<GameOverType> GameOver;

        public event EventHandler GameLoaded;

        #endregion

        #region On-Event methods

        private void OnShowUnit()
        {
            ShowUnits?.Invoke(this,
                (SelectedField.Units.Intersect(CurrentPlayer.Units).ToList(),
                SelectedField.Units.Intersect(OtherPlayer.Units).ToList())
                );
        }

        public enum GameOverType { DRAW, REDWIN, BLUEWIN }

        private void OnGameOver(GameOverType gameOverType)
        {
            BuildEnabled = false;
            SaveEnabled = false;
            GameOverProp = true;
            GameOver?.Invoke(this, gameOverType);
        }

        private void OnGameLoaded()
        {
            pathfinder = new AStar(Table);
            GameLoaded?.Invoke(this, EventArgs.Empty);
        }

        private void OnTowerFired()
        {
            TowerFired?.Invoke(this, EventArgs.Empty);
        }

        private void OnUnitMoved()
        {
            UnitMoved?.Invoke(this, EventArgs.Empty);
        }

        private void OnAttackEnded()
        {
            AttackEnded?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Constructor(s)

        public GameModel(IDataAccess<GameSaveObject> gameDataAccess)
        {
            this.gameDataAccess = gameDataAccess;
            rp = new(PlayerType.RED);
            bp = new(PlayerType.BLUE);
            SaveEnabled = true;
            BuildEnabled = true;
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Takes a collection and removes elements that match the predicate
        /// </summary>
        /// <typeparam name="T">Type of data in the collection</typeparam>
        /// <param name="coll">Collection to remove from</param>
        /// <param name="pred">Predicate to filter with</param>
        private static void RemoveFromCollection<T>(ICollection<T> coll, Predicate<T> pred)
        {
            List<T> toRemove = new();
            foreach (T item in coll)
            {
                if (pred.Invoke(item)) toRemove.Add(item);
            }
            foreach (T item in toRemove)
            {
                coll.Remove(item);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Dumps all units from all <c>Barrack</c>s unit queue
        /// </summary>
        private void PlaceUnits()
        {
            foreach (var barrack in rp.Barracks)
            {
                barrack.NewPath(FindPath(barrack.Coords, bp.Castle.Coords));
                while (barrack.UnitQueue.Count > 0)
                {
                    Table[barrack.WhereToPlace].Units.Add(barrack.UnitQueue.Dequeue());
                }
            }

            foreach (var barrack in bp.Barracks)
            {
                barrack.NewPath(FindPath(barrack.Coords, rp.Castle.Coords));
                while (barrack.UnitQueue.Count > 0)
                {
                    Table[barrack.WhereToPlace].Units.Add(barrack.UnitQueue.Dequeue());
                }
            }
        }

        /// <summary>
        /// Checks for new path, calls MoveUnits() and FireTowers() as long as there are available units.
        /// </summary>
        private async Task Attack()
        {
            AvailableUnits = new List<Unit>(); // List for available units

            for (int i = 0; i < Table.Size.x; i++)
            {
                for (int j = 0; j < Table.Size.y; j++)
                {
                    if (Table[i, j].Units.Count > 0)
                    {
                        IList<(uint x, uint y)> rpPath = FindPath(Table[i, j].Coords, bp.Castle.Coords);
                        IList<(uint x, uint y)> bpPath = FindPath(Table[i, j].Coords, rp.Castle.Coords);

                        foreach (Unit unit in Table[i, j].Units)
                        {
                            unit.ResetStamina();
                            AvailableUnits.Add(unit);
                            if(unit.Owner.Equals(bp))
                            {
                                unit.NewPath(bpPath);
                            }
                            else if(unit.Owner.Equals(rp))
                            {
                                unit.NewPath(rpPath);
                            }
                        }
                    }
                }
            } // sets new path for all units


            while (AvailableUnits.Count > 0)
            {
                await MoveUnits();
                await FireTower();
                RemoveFromCollection(AvailableUnits, e => e.Stamina == 0 || e.Health == 0);
            }

            foreach (Tower tower in bp.Towers)
            {
                tower.ResetCooldown();
            }
            foreach (Tower tower in rp.Towers)
            {
                tower.ResetCooldown();
            }

            OnAttackEnded();

            if (rp.Castle.Health <= 0 && bp.Castle.Health <= 0) OnGameOver(GameOverType.DRAW);
            else if (rp.Castle.Health <= 0) OnGameOver(GameOverType.BLUEWIN);
            else if (bp.Castle.Health <= 0) OnGameOver(GameOverType.REDWIN);
        }

        /// <summary>
        /// Method for moving the units on the table model side.
        /// Moves the units untill they run out of stamina, resets it and removes them from AvailableUnits.
        /// </summary>
        private async Task MoveUnits()
        {
            await Task.Delay(500); // waits 500 millisec/ 0.5 sec

            foreach (Unit unit in AvailableUnits) // move units of RED player by one if they have stamina
            {
                if (unit.Stamina > 0)
                {
                    Table[unit.Path.First.Value].Units.Remove(unit);
                    unit.Moved();
                    Table[unit.Path.First.Value].Units.Add(unit);
                    if (unit.Path.First.Value.Equals(GetOppositePlayer(unit.Owner).Castle.Coords))
                    {
                        GetOppositePlayer(unit.Owner).Castle.Damage();
                        Table[unit.Path.First.Value].Units.Remove(unit);
                        unit.Owner.Units.Remove(unit);
                        unit.Damage(unit.Health);
                    }
                }
            }
            OnUnitMoved();
        }

        /// <summary>
        /// Gets the opposite <c>Player</c>
        /// </summary>
        /// <param name="player"><c>Player</c> to get the opposite of</param>
        /// <returns>The other <c>Player</c></returns>
        private Player GetOppositePlayer(Player player)
        {
            if (player == bp) return rp;
            else return bp;
        }

        /// <summary>
        /// Iterates through all <c>Tower</c>s and damages <c>Unit</c>s in range
        /// </summary>
        /// <returns></returns>
        private async Task FireTower()
        {
            await Task.Delay(350);

            foreach (Tower tower in rp.Towers)
            {
                if (tower.Cooldown > 0)
                {
                    tower.Cool();
                    continue;
                }
                for (int i = Math.Max((int)tower.Coords.x - (int)tower.Range, 0); i < Math.Min((int)tower.Coords.x + (int)tower.Range, Table.Size.x); i++)
                {
                    for (int j = Math.Max((int)tower.Coords.y - (int)tower.Range, 0); j < Math.Min((int)tower.Coords.y + (int)tower.Range, Table.Size.y); j++)
                    {
                        foreach (Unit unit in Table[i, j].Units)
                        {
                            if (unit.Owner != rp)
                            {
                                unit.Damage(tower.Damage);
                            }
                            if (unit.Health == 0)
                            {
                                rp.Money += unit.Cost / 2;
                                unit.Owner.Units.Remove(unit);
                                Table[i, j].Units.Remove(unit);
                                tower.Fire();
                                OnTowerFired();
                                break;
                            }
                        }
                    }
                }
            }

            foreach (Tower tower in bp.Towers)
            {
                if (tower.Cooldown > 0)
                {
                    tower.Cool();
                    continue;
                }
                for (int i = Math.Max((int)tower.Coords.x - (int)tower.Range, 0); i < Math.Min((int)tower.Coords.x + (int)tower.Range, Table.Size.x); i++)
                {
                    for (int j = Math.Max((int)tower.Coords.y - (int)tower.Range, 0); j < Math.Min((int)tower.Coords.y + (int)tower.Range, Table.Size.y); j++)
                    {
                        foreach (Unit unit in Table[i, j].Units)
                        {
                            if (unit.Owner != bp)
                            {
                                unit.Damage(tower.Damage);
                            }
                            if (unit.Health == 0)
                            {
                                bp.Money += unit.Cost / 2;
                                unit.Owner.Units.Remove(unit);
                                Table[i, j].Units.Remove(unit);
                                tower.Fire();
                                OnTowerFired();
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds a unit to the training queue of a random <c>Barrack</c> or the current <c>Player</c>
        /// </summary>
        /// <param name="unit"><c>Unit to train</c></param>
        private void TrainUnit(Unit unit)
        {
            Barrack placeTo = CurrentPlayer.Barracks
                    .ElementAt(new Random().Next(2));

            if (CurrentPlayer.Money < unit.Cost)
                return;

            placeTo.UnitQueue.Enqueue(unit);
            CurrentPlayer.Units.Add(unit);
            CurrentPlayer.Money -= unit.Cost;
        }

        /// <summary>
        /// Builds a <c>Tower</c> on the selected <c>Field</c> if valid
        /// </summary>
        /// <param name="tower"><c>Tower to build</c></param>
        /// <exception cref="InvalidPlacementException">Thrown if placement would be incorrect</exception>
        private void BuildTower(Tower tower)
        {
            if (SelectedField.Placement is not null)
                throw new InvalidPlacementException(SelectedField, "Cannot build tower on non-empty field");
            if (SelectedField.Units.Count > 0)
                throw new InvalidPlacementException(SelectedField, "Cannot build tower on field that contains units");
            if (CurrentPlayer.Money < tower.Cost)
                return;

            SelectedField.Placement = tower;

            if (!ValidatePath())
            {
                SelectedField.Placement = null;
                return;
            }
            pathfinder.ChangeState(SelectedField);
            CurrentPlayer.Towers.Add(tower);
            CurrentPlayer.Money -= tower.Cost;
        }

        /// <summary>
        /// Upgrades <c>Tower</c>
        /// </summary>
        /// <param name="tower"><c>Tower</c> to upgrade</param>
        private static void UpgradeTower(Tower tower)
        {
            tower.LevelUp();
        }

        /// <summary>
        /// Destroys the given <c>Tower</c>, updates pathfinder
        /// </summary>
        /// <param name="tower"><c>Tower</c> to destroy</param>
        private void DestroyTower(Tower tower)
        {
            CurrentPlayer.Money += tower.Cost;
            CurrentPlayer.Towers.Remove(tower);
            Table[tower.Coords].Placement = null;
            pathfinder.ChangeState(Table[tower.Coords]);
        }

        /// <summary>
        /// Validates all necessary paths for game to function correctly
        /// </summary>
        /// <returns>Whether all paths are traversable</returns>
        private bool ValidatePath()
        {
            IList<(uint x, uint y)> path = new List<(uint x, uint y)>();
            foreach (Barrack barrack in rp.Barracks)
            {
                path = FindPath(barrack.Coords, bp.Castle.Coords);
                if (path.Count == 0 || path.Last() != bp.Castle.Coords)
                    return false;
            }
            foreach (Barrack barrack in bp.Barracks)
            {
                path = FindPath(barrack.Coords, rp.Castle.Coords);
                if (path.Count == 0 || path.Last() != rp.Castle.Coords)
                    return false;
            }

            return true;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Method for end turn.
        /// Checks Phase and sets Round.
        /// In attack phase, it disables building and it calls Attack().
        /// </summary>
        public async Task Advance()
        {
            Phase++;
            if (Phase % 3 == 0)
            {
                BuildEnabled = false; // in attack phase you can't build, or place units
                SaveEnabled = false;
                PlaceUnits();
                await Attack();
            }
            else if (Phase % 3 == 1)
            {
                bp.Money += Constants.PASSIVE_INCOME;
                rp.Money += Constants.PASSIVE_INCOME;
                BuildEnabled = true;
                SaveEnabled = true;
                CurrentPlayer = bp;
            }
            else if (Phase % 3 == 2)
            {
                SaveEnabled = true;
                BuildEnabled = true;
                CurrentPlayer = rp;
            }
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
            if (SelectedField.Placement is null)
            {
                if (SelectedField.Units.Count != 0)
                    OnShowUnit();
                else if (BuildEnabled)
                {
                    options.Add(MenuOption.BuildBasic);
                    options.Add(MenuOption.BuildBomber);
                    options.Add(MenuOption.BuildSniper);
                }
            }
            else if (BuildEnabled && SelectedField.Placement.OwnerType == CurrentPlayer.Type)
            {
                switch (SelectedField.Placement)
                {
                    case Tower:
                        if (((Tower)SelectedField.Placement).Level < Constants.MAX_TOWER_LEVEL)
                            options.Add(MenuOption.UpgradeTower);
                        options.Add(MenuOption.DestroyTower);
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

        /// <summary>
        /// Method for selecting an option (related to the selected <c>Field</c>
        /// </summary>
        /// <param name="option">Option to select</param>
        /// <exception cref="InvalidOperationException">Thrown when there is no selected <c>Field</c></exception>
        public override void SelectOption(MenuOption option)
        {
            if (SelectedField is null)
                throw new InvalidOperationException("A field must be selected before selecting an option");
            switch (option)
            {
                case MenuOption.TrainBasic:
                    TrainUnit(new BasicUnit(CurrentPlayer));
                    break;
                case MenuOption.TrainTank:
                    TrainUnit(new TankUnit(CurrentPlayer));
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
                    UpgradeTower((Tower)SelectedField.Placement);
                    break;
                case MenuOption.DestroyTower:
                    DestroyTower((Tower)SelectedField.Placement);
                    break;
                default:
                    break;
            }

        }

        #endregion

        #region Persistence Methods

        public async Task SaveGameAsync(string path)
        {
            if (gameDataAccess == null)
                throw new InvalidOperationException("No data access is provided.");

            await gameDataAccess.SaveAsync(path, new(Table, bp, rp));
        }

        public async Task LoadGameAsync(string path)
        {
            if (gameDataAccess == null)
                throw new InvalidOperationException("No data access is provided.");
            if (path == String.Empty)
                throw new InvalidOperationException("Path is not valid.");
            GameSaveObject save = await gameDataAccess.LoadAsync(path);
            (Table, bp, rp) = (save.Table, save.BluePlayer, save.RedPlayer);
            switch((Table.PhaseCounter - 1) % 3 )
            {
                case 0: CurrentPlayer = bp; break;
                case 1: CurrentPlayer = rp; break;
                case 2: CurrentPlayer = null; break;
            }
            OnGameLoaded();
        }

        #endregion
    }
}
