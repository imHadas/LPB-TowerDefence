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

        Table table;
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

        public Table Table { get { return table; } }

        #endregion

        public GameModel(IDataAccess dataAccess)
        {
            table = new Table(11, 11);
            ICollection<Barrack> rBarracks = new HashSet<Barrack>();
            ICollection<Barrack> bBarracks = new HashSet<Barrack>();
            rp = new Player(PlayerType.RED, rBarracks);
            bp = new Player(PlayerType.BLUE, bBarracks);
            ne = new Player(PlayerType.NEUTRAL, new HashSet<Barrack>());
            rBarracks.Add(new Barrack(rp, 9, 9)); rBarracks.Add(new Barrack(rp, 9, 1));
            bBarracks.Add(new Barrack(bp, 1, 1)); bBarracks.Add(new Barrack(bp, 1, 9));
            currentPlayer = bp;
            SetupTable();
        }

        public void NewGame()
        {
            table = new Table(11, 11);
            ICollection<Barrack> rBarracks = new HashSet<Barrack>();
            ICollection<Barrack> bBarracks = new HashSet<Barrack>();
            rBarracks.Add(new Barrack(rp,9,9)); rBarracks.Add(new Barrack(rp,9,1));
            bBarracks.Add(new Barrack(bp,1,1)); bBarracks.Add(new Barrack(bp,1,9));
            rp = new Player(PlayerType.RED, rBarracks);
            bp = new Player(PlayerType.BLUE, bBarracks);
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
            for(int i = 0; i < table.Size.x; i++)
            {
                for(int j = 0; j < table.Size.y; j++)
                {
                    table[(uint)i, (uint)j] = new Field(i,j);
                    table[(uint)i, (uint)j].Placement = new Placement(ne, (i,j));
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
                table[(uint)x, (uint)y].Placement = new Barrack(player, x, y);
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


    }
}
