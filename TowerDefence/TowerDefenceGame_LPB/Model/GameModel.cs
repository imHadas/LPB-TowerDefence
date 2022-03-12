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
        Player rp;
        Player bp;
        Player ne;
        Player currentPlayer;
        bool saveEnabled;

        #endregion

        #region Properties

        public int Round { get; set; }
        public int Phase { get; set; }
        public Player CurrentPlayer { get { return currentPlayer; } }

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
            currentPlayer = bp;
            SetupTable();
        }

        public void Advance()
        {

        }

        private void MoveUnits()
        {
            
        }

        private void SetupTable()
        {
            for(int i = 0; i < Table.Size.x; i++)
            {
                for(int j = 0; j < Table.Size.y; j++)
                {
                    Table[(uint)i, (uint)j] = new Field(i,j);
                    Table[(uint)i, (uint)j].Placement = new Placement((i, j), ne);
                }
            }
            SetupBarracks(rp);
            SetupBarracks(bp);

        }

        private void SetupBarracks(Player player)
        {
            foreach(Barrack barrack in player.Barracks)
            {
                int x = barrack.Coords.x;
                int y = barrack.Coords.y;
                //table.fields[(uint)x, (uint)y] = new Field(x,y);
                Table[(uint)x, (uint)y].Placement = new Barrack(player, x, y);
            }
        }

        public override void SelectOption(MenuOption menus)
        {
            switch (menus)
            {
                case MenuOption.TrainBasic:
                    if (currentPlayer.Type == PlayerType.BLUE)
                    {
                        
                    }
                    break;
                case MenuOption.TrainTank:
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

        /// <summary>
        /// Method for selecting a field model-side
        /// </summary>
        /// <param name="field"></param>
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
            else if(selectedField.Placement.Owner == currentPlayer) 
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
