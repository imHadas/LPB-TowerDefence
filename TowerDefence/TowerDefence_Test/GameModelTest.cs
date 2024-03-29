﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using TowerDefenceBackend.Persistence;
using TowerDefenceBackend.Model;
using TowerDefenceBackend.DataAccess;
using Moq;
using System;
using System.Threading.Tasks;
using System.Linq;
using static TowerDefenceBackend.Model.GameModel;

namespace TowerDefence_Test
{
    [TestClass]
    public class GameModelTest
    {
        private IDataAccess<GameSaveObject> _dataAccess;
        public GameModel? Model { get; set; }


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

            Assert.AreEqual(Model.CurrentPlayer.Type, PlayerType.BLUE);
            Model?.Advance();
            Assert.AreEqual(Model?.Phase, (uint)2);
            Assert.AreEqual(Model?.CurrentPlayer.Type, PlayerType.RED);
            Assert.AreEqual(Model?.Round, (uint)1);

            while(Model?.Phase < 4)
            {
                Model?.Advance();
            }

            Assert.AreEqual(Model?.Phase, (uint)4);
            Assert.AreEqual(Model?.Round, (uint)2);
        }

        [TestMethod, TestCategory("Train unit"), TestCategory("Money"), TestCategory("Basic")]
        public void TrainBasicUnit()
        {
            Assert.IsNotNull(Model);

            if (Model is not null && Model.CurrentPlayer.Castle is not null)
            {
                Model.SelectField(Model.Table[Model.CurrentPlayer.Castle.Coords]);
                Model.SelectOption(MenuOption.TrainBasic);
                Assert.AreEqual(Model.CurrentPlayer.Units.Count, 1);
                Assert.AreEqual(Model.CurrentPlayer.Money, Constants.PLAYER_STARTING_MONEY - Constants.BASIC_UNIT_COST);

                Model?.Advance();
                Model?.SelectOption(MenuOption.TrainBasic);
                Assert.AreEqual(Model?.CurrentPlayer.Units.Count, 1);
                Assert.AreEqual(Model?.OtherPlayer.Units.Count, 1);
                Assert.AreEqual(Model?.CurrentPlayer.Money, Constants.PLAYER_STARTING_MONEY - Constants.BASIC_UNIT_COST);
            }
        }

        [TestMethod, TestCategory("Train units"), TestCategory("Money"), TestCategory("Basic")]
        public void TrainBasicUnits()
        {
            Assert.IsNotNull(Model);
            if (Model is not null && Model.CurrentPlayer.Castle is not null)
            {
                Model?.SelectField(Model.Table[Model.CurrentPlayer.Castle.Coords]);
                while (Model?.CurrentPlayer.Money != 0)
                {
                    Model?.SelectOption(MenuOption.TrainBasic);
                }
                Assert.AreEqual((uint)Model.CurrentPlayer.Units.Count, Constants.PLAYER_STARTING_MONEY / Constants.BASIC_UNIT_COST);
                Assert.AreEqual(Model.CurrentPlayer.Money, (uint)0);
            }
        }

        [TestMethod, TestCategory("Train unit"), TestCategory("Money"), TestCategory("Tank")]
        public void TrainTankUnit()
        {
            Assert.IsNotNull(Model);

            if (Model is not null && Model.CurrentPlayer.Castle is not null)
            {
                Model.SelectField(Model.Table[Model.CurrentPlayer.Castle.Coords]);
                Model.SelectOption(MenuOption.TrainTank);
                Assert.AreEqual(Model?.CurrentPlayer.Units.Count, 1);
                Assert.AreEqual(Model?.CurrentPlayer.Money, Constants.PLAYER_STARTING_MONEY - Constants.TANK_UNIT_COST);

                Model?.Advance();
                Model?.SelectOption(MenuOption.TrainTank);
                Assert.AreEqual(Model?.CurrentPlayer.Units.Count, 1);
                Assert.AreEqual(Model?.OtherPlayer.Units.Count, 1);
                Assert.AreEqual(Model?.CurrentPlayer.Money, Constants.PLAYER_STARTING_MONEY - Constants.TANK_UNIT_COST);
            }
        }

        [TestMethod, TestCategory("Train units"), TestCategory("Money"), TestCategory("Tank")]
        public void TrainTankUnits()
        {
            Assert.IsNotNull(Model);

            if (Model is not null && Model.CurrentPlayer.Castle is not null)
            {
                Model.SelectField(Model.Table[Model.CurrentPlayer.Castle.Coords]);
                while (Model.CurrentPlayer.Money != 0)
                {
                    Model.SelectOption(MenuOption.TrainTank);
                }
                Assert.AreEqual((uint)Model.CurrentPlayer.Units.Count, Constants.PLAYER_STARTING_MONEY / Constants.TANK_UNIT_COST);
                Assert.AreEqual(Model.CurrentPlayer.Money, (uint)0);
            }
        }

        [TestMethod, TestCategory("Train units"), TestCategory("No money")]
        public void TrainUnitsNoMoney()
        {
            Assert.IsNotNull(Model);

            if (Model is not null && Model.CurrentPlayer.Castle is not null)
            {
                Model.SelectField(Model.Table[Model.CurrentPlayer.Castle.Coords]);
                while (Model.CurrentPlayer.Money != 0)
                {
                    Model.SelectOption(MenuOption.TrainTank);
                }
                Assert.AreEqual((uint)Model.CurrentPlayer.Units.Count, Constants.PLAYER_STARTING_MONEY / Constants.TANK_UNIT_COST);
                Assert.AreEqual(Model.CurrentPlayer.Money, (uint)0);

                Model.SelectOption(MenuOption.TrainTank);
                Assert.AreEqual((uint)Model.CurrentPlayer.Units.Count, Constants.PLAYER_STARTING_MONEY / Constants.TANK_UNIT_COST);

                Model.SelectOption(MenuOption.TrainBasic);
                Assert.AreEqual((uint)Model.CurrentPlayer.Units.Count, Constants.PLAYER_STARTING_MONEY / Constants.TANK_UNIT_COST);
            }
        }

        [TestMethod, TestCategory("Build tower"), TestCategory("Money"), TestCategory("Basic tower")]
        public void BuildBasicTower()
        {
            Assert.IsNotNull(Model);

            Model?.SelectField(Model.Table[0, 0]);
            Model?.SelectOption(MenuOption.BuildBasic);
            Assert.IsNotNull(Model?.Table[0, 0].Placement);
            Assert.AreEqual(Model?.Table[0,0]?.Placement?.GetType(), typeof(BasicTower));
            Assert.AreEqual(Model?.CurrentPlayer.Money, Constants.PLAYER_STARTING_MONEY - Constants.BASIC_TOWER_COST);
        }

        [TestMethod, TestCategory("Build tower"), TestCategory("Money"), TestCategory("Sniper tower")]
        public void BuildSniperTower()
        {
            Assert.IsNotNull(Model);

            Model?.SelectField(Model.Table[0, 0]);
            Model?.SelectOption(MenuOption.BuildSniper);
            Assert.IsNotNull(Model?.Table[0, 0].Placement);
            Assert.AreEqual(Model?.Table[0, 0]?.Placement?.GetType(), typeof(SniperTower));
            Assert.AreEqual(Model?.CurrentPlayer.Money, Constants.PLAYER_STARTING_MONEY - Constants.SNIPER_TOWER_COST);
        }

        [TestMethod, TestCategory("Build tower"), TestCategory("Money"), TestCategory("Bomber tower")]
        public void BuildBomberTower()
        {
            Assert.IsNotNull(Model);

            Model.SelectField(Model.Table[0, 0]);
            Model.SelectOption(MenuOption.BuildBomber);
            Assert.IsNotNull(Model.Table[0, 0].Placement);
            Assert.AreEqual(Model.Table[0, 0]?.Placement?.GetType(), typeof(BomberTower));
            Assert.AreEqual(Model.CurrentPlayer.Money, Constants.PLAYER_STARTING_MONEY - Constants.BOMBER_TOWER_COST);
        }

        [TestMethod, TestCategory("Build tower"), TestCategory("No money")]
        public void BuildAnyTowerNoMoney()
        {
            Assert.IsNotNull(Model);

            Model.SelectField(Model.Table[0, 0]);
            Model.SelectOption(MenuOption.BuildSniper);
            Assert.IsNotNull(Model.Table[0, 0].Placement);
            Assert.AreEqual(Model.Table[0, 0]?.Placement?.GetType(), typeof(SniperTower));
            Model.SelectField(Model.Table[0, 1]);
            Model.SelectOption(MenuOption.BuildSniper);
            Assert.IsNotNull(Model.Table[0, 1].Placement);
            Assert.AreEqual(Model.Table[0, 1]?.Placement?.GetType(), typeof(SniperTower));

            Assert.IsTrue(Model.CurrentPlayer.Money == 0);

            Model.SelectField(Model.Table[0, 2]);
            Model.SelectOption(MenuOption.BuildSniper);
            Assert.IsNull(Model.Table[0, 2].Placement);

            Assert.AreEqual(Model.CurrentPlayer.Money, (uint)0);
        }

        [TestMethod, TestCategory("DestroyTower"), TestCategory("With tower")]
        public void DestroyTower()
        {
            Assert.IsNotNull(Model);

            Model?.SelectField(Model.Table[0, 0]);
            Model?.SelectOption(MenuOption.BuildSniper);
            Assert.IsNotNull(Model?.Table[0, 0].Placement);
            Assert.AreEqual(Model?.Table[0, 0]?.Placement?.GetType(), typeof(SniperTower));
            Assert.AreEqual(Model?.CurrentPlayer.Money, Constants.PLAYER_STARTING_MONEY - Constants.SNIPER_TOWER_COST);

            Model?.SelectField(Model.Table[0, 0]);
            Model?.SelectOption(MenuOption.DestroyTower);
            Assert.IsNull(Model?.Table[0, 0].Placement);
            Assert.AreEqual(Model?.CurrentPlayer.Money, Constants.PLAYER_STARTING_MONEY );
        }

        [TestMethod, TestCategory("FireTower")]
        public async Task FireTowerTest()
        {
            Assert.IsNotNull(Model);

            Assert.AreEqual(Model.CurrentPlayer.Units.Count, 0);
            Assert.AreEqual(Model.OtherPlayer.Units.Count, 0);
            Assert.AreEqual(Model.CurrentPlayer.Barracks.Count, 2);
            Assert.AreEqual(Model.OtherPlayer.Barracks.Count, 2);
            Assert.IsNotNull(Model.CurrentPlayer.Castle);
            Assert.IsNotNull(Model.OtherPlayer.Castle);

            Model.SelectField(Model.Table[Model.CurrentPlayer.Castle.Coords]);
            Model.SelectOption(MenuOption.TrainBasic);
            Assert.AreEqual(Model.CurrentPlayer.Units.Count, 1);
            Assert.AreEqual(Model.OtherPlayer.Units.Count, 0);
            Assert.AreEqual(Model.CurrentPlayer.Money, Constants.PLAYER_STARTING_MONEY - Constants.BASIC_UNIT_COST);

            Assert.AreEqual(Model.Phase, (uint)1);
            await Model.Advance();
            Assert.AreEqual(Model.Phase, (uint)2);

            uint beforebuymoney = Model.CurrentPlayer.Money;
            Model.SelectField(Model.Table[1, 8]);
            Model.SelectOption(MenuOption.BuildBasic);
            Assert.IsNotNull(Model.Table[1, 8].Placement);
            Assert.AreEqual(Model.Table[1, 8].Placement?.GetType(), typeof(BasicTower));
            Assert.AreEqual(Model.CurrentPlayer.Money, beforebuymoney - Constants.BASIC_TOWER_COST);
            beforebuymoney = Model.CurrentPlayer.Money;
            Model.SelectField(Model.Table[1, 2]);
            Model.SelectOption(MenuOption.BuildBasic);
            Assert.IsNotNull(Model.Table[1, 2].Placement);
            Assert.AreEqual(Model.Table[1, 2].Placement?.GetType(), typeof(BasicTower));
            Assert.AreEqual(Model.CurrentPlayer.Money, beforebuymoney - Constants.BASIC_TOWER_COST);

            await Model.Advance();
            Assert.AreEqual(Model.Phase,(uint)3);
            await Model.Advance();
            Assert.AreEqual(Model.Phase,(uint)4);

            Assert.AreEqual(Model.CurrentPlayer.Units.Count,0);
            Assert.AreEqual(Model.OtherPlayer.Units.Count, 0);
            Assert.AreEqual(Model.OtherPlayer.Money,Constants.PLAYER_STARTING_MONEY - 2*Constants.BASIC_TOWER_COST + (Constants.BASIC_UNIT_COST / 2) + Constants.PASSIVE_INCOME);
        }


        [TestMethod, TestCategory("UpgradeTower"), TestCategory("Level two")]
        public void UpgradeTowerLvl2()
        {
            Assert.IsNotNull(Model);

            Model.SelectField(Model.Table[0, 0]);
            Model.SelectOption(MenuOption.BuildBasic);
            Assert.IsNotNull(Model.Table[0, 0].Placement);
            Assert.AreEqual(Model.Table[0, 0]?.Placement?.GetType(), typeof(BasicTower));
            Assert.AreEqual(Model.CurrentPlayer.Money, Constants.PLAYER_STARTING_MONEY - Constants.BASIC_TOWER_COST);

            Tower? tower = Model?.Table[0, 0]?.Placement as Tower;
            uint? range = tower?.Range;

            Model?.SelectField(Model.Table[0, 0]);
            Model?.SelectOption(MenuOption.UpgradeTower);
            Assert.IsFalse(range == (tower?.Range));
            Assert.AreEqual(tower?.Level, (uint)2);
        }

        [TestMethod, TestCategory("UpgradeTower"), TestCategory("Level max")]
        public void UpgradeTowerLvlMax()
        {
            Assert.IsNotNull(Model);

            Model.SelectField(Model.Table[0, 0]);
            Model.SelectOption(MenuOption.BuildBasic);
            Assert.IsNotNull(Model.Table[0, 0].Placement);
            Assert.AreEqual(Model.Table[0, 0]?.Placement?.GetType(), typeof(BasicTower));
            Assert.AreEqual(Model.CurrentPlayer.Money, Constants.PLAYER_STARTING_MONEY - Constants.BASIC_TOWER_COST);

            Tower? tower = Model?.Table[0, 0]?.Placement as Tower;

            uint? speed = tower?.Speed;
            uint? range = tower?.Range;

            Model?.SelectField(Model.Table[0, 0]);
            Model?.SelectOption(MenuOption.UpgradeTower);
            Assert.AreNotEqual(range, tower?.Range);
            Assert.AreEqual(tower?.Level, (uint)2);

            Model?.SelectField(Model.Table[0, 0]);
            Model?.SelectOption(MenuOption.UpgradeTower);
            Assert.AreNotEqual(speed, tower?.Speed);
            Assert.AreEqual(tower?.Level, (uint)3);
            
            
        }

        [TestMethod]
        public async Task GameOverTest()
        {
            TaskCompletionSource<bool> tcs = new ();
            Assert.IsNotNull(Model);
            Model.GameOver += new((sender, args) => tcs.TrySetResult(args.Equals(GameOverType.BLUEWIN)));

            Model.SelectField(Model.Table[Model.CurrentPlayer.Barracks.ElementAt(new Random().Next(2)).Coords]);

            while (Model.CurrentPlayer.Money != 0)
            {
                Model.SelectOption(MenuOption.TrainBasic);
            }

            await Model.Advance();
            await Model.Advance();
            await Model.Advance();

            Assert.AreEqual(Model.Round, (uint)2);
            Assert.IsFalse(tcs.Task.IsCompleted);

            await Model.Advance();
            await Model.Advance();
            await Model.Advance();

            Assert.AreEqual(Model.Round, (uint)3);
            Assert.IsFalse(tcs.Task.IsCompleted);

            await Model.Advance();
            await Model.Advance();
            await Model.Advance();

            Assert.AreEqual(Model.Round, (uint)4);
            Assert.IsFalse(tcs.Task.IsCompleted);

            await Model.Advance();
            await Model.Advance();

            Assert.AreEqual(Model.Round, (uint)5);
            Task.WaitAny(Task.Delay(100), tcs.Task);
            Assert.IsTrue(tcs.Task.IsCompleted);
            Assert.IsTrue(tcs.Task.Result);
            Assert.IsFalse(Model.SaveEnabled);
            Assert.IsFalse(Model.BuildEnabled);
            
        }

        [TestMethod]
        public async Task LoadGameAsyncTest()
        {
            Assert.IsNotNull(Model);

            int nCastle = 0;
            int bBarrack = 0;
            int rBarrack = 0;

            try
            {
                await Model.LoadGameAsync(String.Empty);
            }
            catch(InvalidOperationException)
            {

            }

            for(uint i = 0; i < Model.Table?.Size.x; i++)
            {
                for(uint j = 0; j < Model.Table?.Size.y; j++)
                {
                    if (Model.Table[i, j]?.Placement?.GetType() == typeof(TowerDefenceBackend.Persistence.Castle))
                    {
                        nCastle++;
                    }
                    else if (Model.Table[i, j]?.Placement?.GetType() == typeof(TowerDefenceBackend.Persistence.Barrack))
                    {
                        Barrack? barrack = Model.Table[i, j]?.Placement as Barrack;
                        Assert.AreNotEqual(barrack?.OwnerType, PlayerType.NEUTRAL);
                        if (barrack?.OwnerType == PlayerType.RED)
                            rBarrack++;
                        else if (barrack?.OwnerType == PlayerType.BLUE)
                            bBarrack++;
                    }
                    else
                        Assert.IsNull(Model.Table[i,j]?.Placement);
                }
            }

            Assert.AreEqual(nCastle, 2);
            Assert.AreEqual(bBarrack, 2);
            Assert.AreEqual(rBarrack, 2);
        }
    }
}
