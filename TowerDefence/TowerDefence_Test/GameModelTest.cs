using Microsoft.VisualStudio.TestTools.UnitTesting;
using TowerDefenceBackend.Persistence;
using TowerDefenceBackend.Model;
using TowerDefenceBackend.DataAccess;
using Moq;
using System.Threading.Tasks;

namespace TowerDefence_Test
{
    [TestClass]
    public class GameModelTest
    {
        private IDataAccess<GameSaveObject> _dataAccess;
        public GameModel Model { get; set; }


        public void MockDA()
        {
            Table tablemock = new(15,10);
            for(uint i = 0; i < 15; i++)
            {
                for (uint j = 0; j < 10; j++)
                {
                    tablemock[i, j] = new(i, j);
                }
            }
            tablemock.PhaseCounter = 1;
            Player rp = new(PlayerType.RED);
            Player bp = new(PlayerType.BLUE);
            rp.Barracks.Add(new(rp, 9, 9)); rp.Barracks.Add(new(rp, 9, 1));
            bp.Barracks.Add(new(bp, 1, 1)); bp.Barracks.Add(new(bp, 1, 9));
            rp.Castle = new(rp, 9, 5);
            bp.Castle = new(bp, 1, 5);
            tablemock[rp.Castle.Coords].Placement = rp.Castle;
            tablemock[bp.Castle.Coords].Placement = bp.Castle;
            foreach (Barrack barrack in rp.Barracks)
            {
                tablemock[barrack.Coords].Placement = barrack;
            }
            foreach (Barrack barrack in bp.Barracks)
            {
                tablemock[barrack.Coords].Placement = barrack;
            }

            GameSaveObject gsomock = new(tablemock, bp, rp);

            var dbmock = new Mock<IDataAccess<GameSaveObject>>();
            dbmock.Setup(d => d.LoadAsync("1")).Returns(Task.FromResult(gsomock));
            _dataAccess = dbmock.Object;
        }

        public async Task<GameModel> MakeGameModel()
        {
            GameModel newmodel = new(_dataAccess);
            await newmodel.LoadGameAsync("1");
            return newmodel;
        }

        [TestInitialize]
        public async Task TestInit()
        {
            MockDA();
            Model = await MakeGameModel();
        }


        [TestMethod]
        public void GameModelInit()
        {
            Assert.IsNotNull(Model);
            Assert.IsNotNull(Model.CurrentPlayer);
            Assert.IsNotNull(Model.OtherPlayer);
            Assert.IsNotNull(Model.Table);
        }

        [TestMethod]
        public void GameAdvance()
        {
            Assert.IsNotNull(Model);

            Assert.IsTrue(Model.CurrentPlayer.Type == PlayerType.BLUE);
            Model.Advance();
            Assert.IsTrue(Model.Phase == 2);
            Assert.IsTrue(Model.CurrentPlayer.Type == PlayerType.RED);
            Assert.IsTrue(Model.Round == 1);

            while(Model.Phase < 4)
            {
                Model.Advance();
            }

            Assert.IsTrue(Model.Phase == 4);
            Assert.IsTrue(Model.Round == 2);
        }

        [TestMethod, TestCategory("Train unit"), TestCategory("Money"), TestCategory("Basic")]
        public void TrainBasicUnit()
        {
            Assert.IsNotNull(Model);

            Model.SelectField(Model.Table[Model.CurrentPlayer.Castle.Coords]);
            Model.SelectOption(MenuOption.TrainBasic);
            Assert.IsTrue(Model.CurrentPlayer.Units.Count == 1);
            Assert.IsTrue(Model.CurrentPlayer.Money == Constants.PLAYER_STARTING_MONEY-Constants.BASIC_UNIT_COST);

            Model.Advance();
            Model.SelectOption(MenuOption.TrainBasic);
            Assert.IsTrue(Model.CurrentPlayer.Units.Count == 1);
            Assert.IsTrue(Model.OtherPlayer.Units.Count == 1);
            Assert.IsTrue(Model.CurrentPlayer.Money == Constants.PLAYER_STARTING_MONEY - Constants.BASIC_UNIT_COST);
        }

        [TestMethod, TestCategory("Train units"), TestCategory("Money"), TestCategory("Basic")]
        public void TrainBasicUnits()
        {
            Assert.IsNotNull(Model);

            Model.SelectField(Model.Table[Model.CurrentPlayer.Castle.Coords]);
            while (Model.CurrentPlayer.Money != 0)
            {
                Model.SelectOption(MenuOption.TrainBasic);
            }
            Assert.IsTrue(Model.CurrentPlayer.Units.Count == Constants.PLAYER_STARTING_MONEY/Constants.BASIC_UNIT_COST);
            Assert.IsTrue(Model.CurrentPlayer.Money == 0);
        }

        [TestMethod, TestCategory("Train unit"), TestCategory("Money"), TestCategory("Tank")]
        public void TrainTankUnit()
        {
            Assert.IsNotNull(Model);

            Model.SelectField(Model.Table[Model.CurrentPlayer.Castle.Coords]);
            Model.SelectOption(MenuOption.TrainTank);
            Assert.IsTrue(Model.CurrentPlayer.Units.Count == 1);
            Assert.IsTrue(Model.CurrentPlayer.Money == Constants.PLAYER_STARTING_MONEY - Constants.TANK_UNIT_COST);

            Model.Advance();
            Model.SelectOption(MenuOption.TrainTank);
            Assert.IsTrue(Model.CurrentPlayer.Units.Count == 1);
            Assert.IsTrue(Model.OtherPlayer.Units.Count == 1);
            Assert.IsTrue(Model.CurrentPlayer.Money == Constants.PLAYER_STARTING_MONEY - Constants.TANK_UNIT_COST);
        }

        [TestMethod, TestCategory("Train units"), TestCategory("Money"), TestCategory("Tank")]
        public void TrainTankUnits()
        {
            Assert.IsNotNull(Model);

            Model.SelectField(Model.Table[Model.CurrentPlayer.Castle.Coords]);
            while (Model.CurrentPlayer.Money != 0)
            {
                Model.SelectOption(MenuOption.TrainTank);
            }
            Assert.IsTrue(Model.CurrentPlayer.Units.Count == Constants.PLAYER_STARTING_MONEY / Constants.TANK_UNIT_COST);
            Assert.IsTrue(Model.CurrentPlayer.Money == 0);
        }

        [TestMethod, TestCategory("Train units"), TestCategory("No money")]
        public void TrainUnitsNoMoney()
        {
            Assert.IsNotNull(Model);

            Model.SelectField(Model.Table[Model.CurrentPlayer.Castle.Coords]);
            while (Model.CurrentPlayer.Money != 0)
            {
                Model.SelectOption(MenuOption.TrainTank);
            }
            Assert.IsTrue(Model.CurrentPlayer.Units.Count == Constants.PLAYER_STARTING_MONEY / Constants.TANK_UNIT_COST);
            Assert.IsTrue(Model.CurrentPlayer.Money == 0);

            Model.SelectOption(MenuOption.TrainTank);
            Assert.IsTrue(Model.CurrentPlayer.Units.Count == Constants.PLAYER_STARTING_MONEY / Constants.TANK_UNIT_COST);

            Model.SelectOption(MenuOption.TrainBasic);
            Assert.IsTrue(Model.CurrentPlayer.Units.Count == Constants.PLAYER_STARTING_MONEY / Constants.TANK_UNIT_COST);
        }

        [TestMethod, TestCategory("Build tower"), TestCategory("Money"), TestCategory("Basic tower")]
        public void BuildBasicTower()
        {
            Assert.IsNotNull(Model);

            Model.SelectField(Model.Table[0, 0]);
            Model.SelectOption(MenuOption.BuildBasic);
            Assert.IsNotNull(Model.Table[0, 0].Placement);
            Assert.IsTrue(Model.Table[0,0]?.Placement?.GetType() == typeof(BasicTower));
            Assert.IsTrue(Model.CurrentPlayer.Money == Constants.PLAYER_STARTING_MONEY - Constants.BASIC_TOWER_COST);
        }

        [TestMethod, TestCategory("Build tower"), TestCategory("Money"), TestCategory("Sniper tower")]
        public void BuildSniperTower()
        {
            Assert.IsNotNull(Model);

            Model.SelectField(Model.Table[0, 0]);
            Model.SelectOption(MenuOption.BuildSniper);
            Assert.IsNotNull(Model.Table[0, 0].Placement);
            Assert.IsTrue(Model.Table[0, 0]?.Placement?.GetType() == typeof(SniperTower));
            Assert.IsTrue(Model.CurrentPlayer.Money == Constants.PLAYER_STARTING_MONEY - Constants.SNIPER_TOWER_COST);
        }

        [TestMethod, TestCategory("Build tower"), TestCategory("Money"), TestCategory("Bomber tower")]
        public void BuildBomberTower()
        {
            Assert.IsNotNull(Model);

            Model.SelectField(Model.Table[0, 0]);
            Model.SelectOption(MenuOption.BuildBomber);
            Assert.IsNotNull(Model.Table[0, 0].Placement);
            Assert.IsTrue(Model.Table[0, 0]?.Placement?.GetType() == typeof(BomberTower));
            Assert.IsTrue(Model.CurrentPlayer.Money == Constants.PLAYER_STARTING_MONEY - Constants.BOMBER_TOWER_COST);
        }

        [TestMethod, TestCategory("Build tower"), TestCategory("No money")]
        public void BuildAnyTowerNoMoney()
        {
            Assert.IsNotNull(Model);

            Model.SelectField(Model.Table[0, 0]);
            Model.SelectOption(MenuOption.BuildSniper);
            Assert.IsNotNull(Model.Table[0, 0].Placement);
            Assert.IsTrue(Model.Table[0, 0]?.Placement?.GetType() == typeof(SniperTower));
            Model.SelectField(Model.Table[0, 1]);
            Model.SelectOption(MenuOption.BuildSniper);
            Assert.IsNotNull(Model.Table[0, 1].Placement);
            Assert.IsTrue(Model.Table[0, 1]?.Placement?.GetType() == typeof(SniperTower));

            Assert.IsTrue(Model.CurrentPlayer.Money == 0);

            Model.SelectField(Model.Table[0, 2]);
            Model.SelectOption(MenuOption.BuildSniper);
            Assert.IsNull(Model.Table[0, 2].Placement);

            Assert.IsTrue(Model.CurrentPlayer.Money == 0);
        }

        [TestMethod, TestCategory("DestroyTower"), TestCategory("With tower")]
        public void DestroyTower()
        {
            Assert.IsNotNull(Model);

            Model.SelectField(Model.Table[0, 0]);
            Model.SelectOption(MenuOption.BuildSniper);
            Assert.IsNotNull(Model.Table[0, 0].Placement);
            Assert.IsTrue(Model.Table[0, 0]?.Placement?.GetType() == typeof(SniperTower));
            Assert.IsTrue(Model.CurrentPlayer.Money == Constants.PLAYER_STARTING_MONEY - Constants.SNIPER_TOWER_COST);

            Model.SelectField(Model.Table[0, 0]);
            Model.SelectOption(MenuOption.DestroyTower);
            Assert.IsNull(Model.Table[0, 0].Placement);
            Assert.IsTrue(Model.CurrentPlayer.Money == Constants.PLAYER_STARTING_MONEY );
        }
    }
}
