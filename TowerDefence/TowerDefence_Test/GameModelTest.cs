using Microsoft.VisualStudio.TestTools.UnitTesting;
using TowerDefenceGame_LPB.Persistence;
using TowerDefenceGame_LPB.Model;
using TowerDefenceGame_LPB.DataAccess;
using System;
using Moq;
using System.Threading.Tasks;

namespace TowerDefence_Test
{
    [TestClass]
    public class GameModelTest
    {
        private GameSaveObject? _mockedGameSaveObject;
        private Table? _mockedTable;
        private Player? _mockedRedPlayer;
        private Player? _mockedBluePlayer;
        private GameModel? _gameModel;
        private Mock<IDataAccess<GameSaveObject>>? _mock;
        private IDataAccess<GameSaveObject>? _dataAccess;

        [TestInitialize]
        public void Init()
        {
            _mockedTable = MakeTable(10,15);
            _mockedRedPlayer = new Player(PlayerType.RED);
            _mockedBluePlayer = new Player(PlayerType.BLUE);
            _mockedGameSaveObject = new GameSaveObject(_mockedTable, _mockedBluePlayer, _mockedRedPlayer);

            Assert.IsNotNull(_mockedTable);
            Assert.IsNotNull(_mockedRedPlayer);

            _mockedTable[0, 0].Placement = new TowerDefenceGame_LPB.Persistence.Castle(_mockedRedPlayer,0,0);
            _mockedTable[0, 1].Placement = new TowerDefenceGame_LPB.Persistence.Barrack(_mockedRedPlayer, 0, 1);
            _mockedTable[0, 2].Placement = new TowerDefenceGame_LPB.Persistence.Barrack(_mockedRedPlayer, 0, 2);

            _mockedTable[2, 0].Placement = new TowerDefenceGame_LPB.Persistence.Castle(_mockedBluePlayer, 2, 0);
            _mockedTable[2, 1].Placement = new TowerDefenceGame_LPB.Persistence.Barrack(_mockedBluePlayer, 2, 1);
            _mockedTable[2, 2].Placement = new TowerDefenceGame_LPB.Persistence.Barrack(_mockedBluePlayer, 2, 1);

            _mock = new Mock<IDataAccess<GameSaveObject>>();
            _mock.Setup(mock => mock.LoadAsync(It.IsAny<String>())).Returns(() => Task.FromResult(_mockedGameSaveObject));

            _gameModel = new GameModel(_mock.Object);

            _dataAccess = new JsonDataAccess();
        }

        private Table MakeTable(uint width, uint height)
        {
            Table output = new(height, width);
            for (uint i = 0; i < height; i++)
            {
                for (uint j = 0; j < width; j++)
                {
                    output[i, j] = new(i, j);
                }
            }

            return output;
        }

        private GameModel MakeGameModel()
        {
            if(_dataAccess is not null)
                return new GameModel(_dataAccess);
            else
            {
                _dataAccess = new JsonDataAccess();
                return new GameModel(_dataAccess);
            }
        }

        [TestMethod]
        public void GameModelInit()
        {
            _gameModel?.NewGame();
            Assert.IsNotNull(_gameModel);
            Assert.IsNotNull(_gameModel.CurrentPlayer);
            Assert.IsNotNull(_gameModel.OtherPlayer);
            Assert.IsNotNull(_gameModel.Table);
        }

        [TestMethod]
        public void GameAdvance()
        {
            _gameModel?.NewGame();
            Assert.IsNotNull(_gameModel);

            Assert.IsTrue(_gameModel.CurrentPlayer.Type == PlayerType.BLUE);
            _gameModel.Advance();
            Assert.IsTrue(_gameModel.Phase == 2);
            Assert.IsTrue(_gameModel.CurrentPlayer.Type == PlayerType.RED);
            Assert.IsTrue(_gameModel.Round == 1);

            while(_gameModel.Phase < 4)
            {
                _gameModel.Advance();
            }

            Assert.IsTrue(_gameModel.Phase == 4);
            Assert.IsTrue(_gameModel.Round == 2);
        }

        [TestMethod]
        public void NewGameTest()
        {
            _gameModel?.NewGame();
            Assert.IsNotNull(_gameModel);

            _gameModel.Advance();
            Assert.IsTrue(_gameModel.Phase == 2);
            _gameModel.NewGame();
            Assert.IsTrue(_gameModel.Phase == 1);
        }

        [TestMethod, TestCategory("Train unit"), TestCategory("Money"), TestCategory("Basic")]
        public void TrainBasicUnit()
        {
            _gameModel?.NewGame();
            Assert.IsNotNull(_gameModel);

            _gameModel.SelectOption(MenuOption.TrainBasic);
            Assert.IsTrue(_gameModel.CurrentPlayer.Units.Count == 1);
            Assert.IsTrue(_gameModel.CurrentPlayer.Money == Constants.PLAYER_STARTING_MONEY-Constants.BASIC_UNIT_COST);

            _gameModel.Advance();
            _gameModel.SelectOption(MenuOption.TrainBasic);
            Assert.IsTrue(_gameModel.CurrentPlayer.Units.Count == 1);
            Assert.IsTrue(_gameModel.OtherPlayer.Units.Count == 1);
            Assert.IsTrue(_gameModel.CurrentPlayer.Money == Constants.PLAYER_STARTING_MONEY - Constants.BASIC_UNIT_COST);
        }

        [TestMethod, TestCategory("Train units"), TestCategory("Money"), TestCategory("Basic")]
        public void TrainBasicUnits()
        {
            _gameModel?.NewGame();
            Assert.IsNotNull(_gameModel);

            while (_gameModel.CurrentPlayer.Money != 0)
            {
                _gameModel.SelectOption(MenuOption.TrainBasic);
            }
            Assert.IsTrue(_gameModel.CurrentPlayer.Units.Count == Constants.PLAYER_STARTING_MONEY/Constants.BASIC_UNIT_COST);
            Assert.IsTrue(_gameModel.CurrentPlayer.Money == 0);
        }

        [TestMethod, TestCategory("Train unit"), TestCategory("Money"), TestCategory("Tank")]
        public void TrainTankUnit()
        {
            _gameModel?.NewGame();
            Assert.IsNotNull(_gameModel);

            _gameModel.SelectOption(MenuOption.TrainTank);
            Assert.IsTrue(_gameModel.CurrentPlayer.Units.Count == 1);
            Assert.IsTrue(_gameModel.CurrentPlayer.Money == Constants.PLAYER_STARTING_MONEY - Constants.TANK_UNIT_COST);

            _gameModel.Advance();
            _gameModel.SelectOption(MenuOption.TrainTank);
            Assert.IsTrue(_gameModel.CurrentPlayer.Units.Count == 1);
            Assert.IsTrue(_gameModel.OtherPlayer.Units.Count == 1);
            Assert.IsTrue(_gameModel.CurrentPlayer.Money == Constants.PLAYER_STARTING_MONEY - Constants.TANK_UNIT_COST);
        }

        [TestMethod, TestCategory("Train units"), TestCategory("Money"), TestCategory("Tank")]
        public void TrainTankUnits()
        {
            _gameModel = MakeGameModel();
            Assert.IsNotNull(_gameModel);

            while (_gameModel.CurrentPlayer.Money != 0)
            {
                _gameModel.SelectOption(MenuOption.TrainTank);
            }
            Assert.IsTrue(_gameModel.CurrentPlayer.Units.Count == Constants.PLAYER_STARTING_MONEY / Constants.TANK_UNIT_COST);
            Assert.IsTrue(_gameModel.CurrentPlayer.Money == 0);
        }

        [TestMethod, TestCategory("Train units"), TestCategory("No money")]
        public void TrainUnitsNoMoney()
        {
            _gameModel?.NewGame();
            Assert.IsNotNull(_gameModel);

            while (_gameModel.CurrentPlayer.Money != 0)
            {
                _gameModel.SelectOption(MenuOption.TrainTank);
            }
            Assert.IsTrue(_gameModel.CurrentPlayer.Units.Count == Constants.PLAYER_STARTING_MONEY / Constants.TANK_UNIT_COST);
            Assert.IsTrue(_gameModel.CurrentPlayer.Money == 0);

            _gameModel.SelectOption(MenuOption.TrainTank);
            Assert.IsTrue(_gameModel.CurrentPlayer.Units.Count == Constants.PLAYER_STARTING_MONEY / Constants.TANK_UNIT_COST);

            _gameModel.SelectOption(MenuOption.TrainBasic);
            Assert.IsTrue(_gameModel.CurrentPlayer.Units.Count == Constants.PLAYER_STARTING_MONEY / Constants.TANK_UNIT_COST);
        }

        [TestMethod, TestCategory("Build tower"), TestCategory("Money"), TestCategory("Basic tower")]
        public void BuildBasicTower()
        {
            _gameModel?.NewGame();
            Assert.IsNotNull(_gameModel);

            _gameModel.SelectField(_gameModel.Table[0, 0]);
            _gameModel.SelectOption(MenuOption.BuildBasic);
            Assert.IsNotNull(_gameModel.Table[0, 0].Placement);
            Assert.IsTrue(_gameModel.Table[0,0]?.Placement?.GetType() == typeof(BasicTower));
            Assert.IsTrue(_gameModel.CurrentPlayer.Money == Constants.PLAYER_STARTING_MONEY - Constants.BASIC_TOWER_COST);
        }

        [TestMethod, TestCategory("Build tower"), TestCategory("Money"), TestCategory("Sniper tower")]
        public void BuildSniperTower()
        {
            _gameModel?.NewGame();
            Assert.IsNotNull(_gameModel);

            _gameModel.SelectField(_gameModel.Table[0, 0]);
            _gameModel.SelectOption(MenuOption.BuildSniper);
            Assert.IsNotNull(_gameModel.Table[0, 0].Placement);
            Assert.IsTrue(_gameModel.Table[0, 0]?.Placement?.GetType() == typeof(SniperTower));
            Assert.IsTrue(_gameModel.CurrentPlayer.Money == Constants.PLAYER_STARTING_MONEY - Constants.SNIPER_TOWER_COST);
        }

        [TestMethod, TestCategory("Build tower"), TestCategory("Money"), TestCategory("Bomber tower")]
        public void BuildBomberTower()
        {
            _gameModel?.NewGame();
            Assert.IsNotNull(_gameModel);

            _gameModel.SelectField(_gameModel.Table[0, 0]);
            _gameModel.SelectOption(MenuOption.BuildBomber);
            Assert.IsNotNull(_gameModel.Table[0, 0].Placement);
            Assert.IsTrue(_gameModel.Table[0, 0]?.Placement?.GetType() == typeof(BomberTower));
            Assert.IsTrue(_gameModel.CurrentPlayer.Money == Constants.PLAYER_STARTING_MONEY - Constants.BOMBER_TOWER_COST);
        }

        [TestMethod, TestCategory("Build tower"), TestCategory("No money")]
        public void BuildAnyTowerNoMoney()
        {
            _gameModel?.NewGame();
            Assert.IsNotNull(_gameModel);

            _gameModel.SelectField(_gameModel.Table[0, 0]);
            _gameModel.SelectOption(MenuOption.BuildSniper);
            Assert.IsNotNull(_gameModel.Table[0, 0].Placement);
            Assert.IsTrue(_gameModel.Table[0, 0]?.Placement?.GetType() == typeof(SniperTower));
            _gameModel.SelectField(_gameModel.Table[0, 1]);
            _gameModel.SelectOption(MenuOption.BuildSniper);
            Assert.IsNotNull(_gameModel.Table[0, 1].Placement);
            Assert.IsTrue(_gameModel.Table[0, 1]?.Placement?.GetType() == typeof(SniperTower));

            Assert.IsTrue(_gameModel.CurrentPlayer.Money == 0);

            _gameModel.SelectField(_gameModel.Table[0, 2]);
            _gameModel.SelectOption(MenuOption.BuildSniper);
            Assert.IsNull(_gameModel.Table[0, 2].Placement);

            Assert.IsTrue(_gameModel.CurrentPlayer.Money == 0);
        }

        [TestMethod, TestCategory("DestroyTower"), TestCategory("With tower")]
        public void DestroyTower()
        {
            _gameModel?.NewGame();
            Assert.IsNotNull(_gameModel);

            _gameModel.SelectField(_gameModel.Table[0, 0]);
            _gameModel.SelectOption(MenuOption.BuildSniper);
            Assert.IsNotNull(_gameModel.Table[0, 0].Placement);
            Assert.IsTrue(_gameModel.Table[0, 0]?.Placement?.GetType() == typeof(SniperTower));
            Assert.IsTrue(_gameModel.CurrentPlayer.Money == Constants.PLAYER_STARTING_MONEY - Constants.SNIPER_TOWER_COST);

            _gameModel.SelectField(_gameModel.Table[0, 0]);
            _gameModel.SelectOption(MenuOption.DestroyTower);
            Assert.IsNull(_gameModel.Table[0, 0].Placement);
            Assert.IsTrue(_gameModel.CurrentPlayer.Money == Constants.PLAYER_STARTING_MONEY );
        }

        [TestMethod, TestCategory("FireTower")]
        public async Task FireTowerTest()
        {
            _gameModel?.NewGame();
            Assert.IsNotNull(_gameModel);

            Assert.IsTrue(_gameModel.CurrentPlayer.Units.Count == 0);
            Assert.IsTrue(_gameModel.OtherPlayer.Units.Count == 0);

            _gameModel.SelectOption(MenuOption.TrainBasic);
            Assert.IsTrue(_gameModel.CurrentPlayer.Units.Count == 1);
            Assert.IsTrue(_gameModel.OtherPlayer.Units.Count == 0);
            Assert.IsTrue(_gameModel.CurrentPlayer.Money == Constants.PLAYER_STARTING_MONEY - Constants.BASIC_UNIT_COST);

            uint X = 0, Y = 0;

            foreach(Barrack barrack in _gameModel.CurrentPlayer.Barracks)
            {
                if(barrack.UnitQueue.Count == 1)
                {
                    X = barrack.Coords.x;
                    Y = barrack.Coords.y;
                }
            }

            if (X < _gameModel.Table.Size.x - 1)
            {
                X++;
            }
            else
            {
                X--;
            }

            Assert.IsTrue(_gameModel.Phase == 1);
            _gameModel.Advance();
            Assert.IsTrue(_gameModel.Phase == 2);

            _gameModel.SelectField(_gameModel.Table[X, Y]);
            _gameModel.SelectOption(MenuOption.BuildBasic);
            Assert.IsNotNull(_gameModel.Table[X, Y].Placement);
            Assert.IsTrue(_gameModel.Table[X, Y]?.Placement?.GetType() == typeof(BasicTower));
            Assert.IsTrue(_gameModel.CurrentPlayer.Money == Constants.PLAYER_STARTING_MONEY - Constants.BASIC_TOWER_COST);

            _gameModel.Advance();
            await Task.Delay(1200);
            Assert.IsTrue(_gameModel.Phase == 3);
            _gameModel.Advance();
            Assert.IsTrue(_gameModel.Phase == 4);

            Assert.IsTrue(_gameModel.CurrentPlayer.Units.Count == 0);
            Assert.IsTrue(_gameModel.OtherPlayer.Units.Count == 0);
            Assert.IsTrue(_gameModel.OtherPlayer.Money == Constants.PLAYER_STARTING_MONEY - Constants.BASIC_TOWER_COST + (Constants.BASIC_UNIT_COST / 2) + Constants.PASSIVE_INCOME);

        }


        [TestMethod, TestCategory("UpgradeTower"), TestCategory("Level two")]
        public void UpgradeTowerLvl2()
        {
            _gameModel?.NewGame();
            Assert.IsNotNull(_gameModel);

            _gameModel.SelectField(_gameModel.Table[0, 0]);
            _gameModel.SelectOption(MenuOption.BuildBasic);
            Assert.IsNotNull(_gameModel.Table[0, 0].Placement);
            Assert.IsTrue(_gameModel.Table[0, 0]?.Placement?.GetType() == typeof(BasicTower));
            Assert.IsTrue(_gameModel.CurrentPlayer.Money == Constants.PLAYER_STARTING_MONEY - Constants.BASIC_TOWER_COST);

            Tower? tower = _gameModel?.Table[0, 0]?.Placement as Tower;
            uint? damage = tower?.Damage;

            _gameModel?.SelectField(_gameModel.Table[0, 0]);
            _gameModel?.SelectOption(MenuOption.UpgradeTower);
            Assert.IsFalse(damage == (tower?.Damage));
            Assert.IsTrue(tower?.Level == 2);
        }

        [TestMethod, TestCategory("UpgradeTower"), TestCategory("Level max")]
        public void UpgradeTowerLvlMax()
        {
            _gameModel?.NewGame();
            Assert.IsNotNull(_gameModel);

            _gameModel.SelectField(_gameModel.Table[0, 0]);
            _gameModel.SelectOption(MenuOption.BuildBasic);
            Assert.IsNotNull(_gameModel.Table[0, 0].Placement);
            Assert.IsTrue(_gameModel.Table[0, 0]?.Placement?.GetType() == typeof(BasicTower));
            Assert.IsTrue(_gameModel.CurrentPlayer.Money == Constants.PLAYER_STARTING_MONEY - Constants.BASIC_TOWER_COST);

            Tower? tower = _gameModel?.Table[0, 0]?.Placement as Tower;

            uint? damage = tower?.Damage;
            uint? speed = tower?.Speed;
            uint? range = tower?.Range;

            _gameModel?.SelectField(_gameModel.Table[0, 0]);
            _gameModel?.SelectOption(MenuOption.UpgradeTower);
            Assert.IsFalse(damage == tower?.Damage);
            Assert.IsTrue(tower?.Level == 2);

            _gameModel?.SelectField(_gameModel.Table[0, 0]);
            _gameModel?.SelectOption(MenuOption.UpgradeTower);
            Assert.IsFalse(speed == tower?.Speed);
            Assert.IsTrue(tower?.Level == 3);
            
            _gameModel?.SelectField(_gameModel.Table[0, 0]);
            _gameModel?.SelectOption(MenuOption.UpgradeTower);
            Assert.IsFalse(range == tower?.Range);
            Assert.IsTrue(tower?.Level == 4);
        }

        [TestMethod]
        public async Task GameOverTest()
        {
            _gameModel?.NewGame();
            Assert.IsNotNull(_gameModel);

            while (_gameModel.CurrentPlayer.Money != 0)
            {
                _gameModel.SelectOption(MenuOption.TrainBasic);
            }

            _gameModel.Advance();
            _gameModel.Advance();
            _gameModel.Advance();
            await Task.Delay(3000);

            Assert.IsTrue(_gameModel.Round == 2);
            Assert.IsFalse(_gameModel.GameOverProp);

            _gameModel.Advance();
            _gameModel.Advance();
            _gameModel.Advance();
            await Task.Delay(3000);

            Assert.IsTrue(_gameModel.Round == 3);
            Assert.IsFalse(_gameModel.GameOverProp);

            _gameModel.Advance();
            _gameModel.Advance();
            _gameModel.Advance();
            await Task.Delay(3000);

            Assert.IsTrue(_gameModel.Round == 4);
            Assert.IsFalse(_gameModel.GameOverProp);

            _gameModel.Advance();
            _gameModel.Advance();
            _gameModel.Advance();
            await Task.Delay(3200);

            Assert.IsTrue(_gameModel.Round == 5);
            Assert.IsTrue(_gameModel.GameOverProp);
            Assert.IsFalse(_gameModel.SaveEnabled);
            Assert.IsFalse(_gameModel.BuildEnabled);
            
        }

        [TestMethod]
        public async Task SaveGameAsyncTest()
        {
            _gameModel?.NewGame();
            Assert.IsNotNull(_gameModel);

            string path = String.Empty;

            try
            {
                await _gameModel.SaveGameAsync(path);
            }
            catch(InvalidOperationException)
            {
                
            }
        }

        [TestMethod]
        public async Task LoadGameAsyncTest()
        {
            _gameModel?.NewGame();
            Assert.IsNotNull(_gameModel);
            
            string path = String.Empty;

            try
            {
                await _gameModel.LoadGameAsync(path);
            }
            catch(InvalidOperationException)
            {

            }
            

        }
    }
}
