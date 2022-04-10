using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TowerDefenceGame_LPB.Persistence;
using TowerDefenceGame_LPB.Model;
using TowerDefenceGame_LPB.DataAccess;
using System;

namespace TowerDefence_Test
{
    [TestClass]
    public class GameModelTest
    {
        private IDataAccess<GameSaveObject> _dataAccess;
        public GameModel Model { get; set; }

        [TestInitialize]
        public void Init()
        {
            _dataAccess = new JsonDataAccess();
        }

        private GameModel MakeGameModel()
        {
            return new GameModel(_dataAccess);
        }

        [TestMethod]
        public void GameModelInit()
        {
            Model = MakeGameModel();
            Assert.IsNotNull(Model);
            Assert.IsNotNull(Model.CurrentPlayer);
            Assert.IsNotNull(Model.OtherPlayer);
            Assert.IsNotNull(Model.Table);
        }

        [TestMethod]
        public void GameAdvance()
        {
            Model = MakeGameModel();
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

        [TestMethod]
        public void NewGame()
        {
            Model = MakeGameModel();
            Assert.IsNotNull(Model);

            Model.Advance();
            Assert.IsTrue(Model.Phase == 2);
            Model.NewGame();
            Assert.IsTrue(Model.Phase == 1);
        }

        [TestMethod, TestCategory("Train unit"), TestCategory("Money"), TestCategory("Basic")]
        public void TrainBasicUnit()
        {
            Model = MakeGameModel();
            Assert.IsNotNull(Model);

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
            Model = MakeGameModel();
            Assert.IsNotNull(Model);

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
            Model = MakeGameModel();
            Assert.IsNotNull(Model);

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
            Model = MakeGameModel();
            Assert.IsNotNull(Model);

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
            Model = MakeGameModel();
            Assert.IsNotNull(Model);

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
            Model = MakeGameModel();
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
            Model = MakeGameModel();
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
            Model = MakeGameModel();
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
            Model = MakeGameModel();
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
            Model = MakeGameModel();
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
