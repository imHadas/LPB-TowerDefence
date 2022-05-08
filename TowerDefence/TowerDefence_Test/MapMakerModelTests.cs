using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TowerDefenceBackend.Persistence;
using TowerDefenceBackend.Model;
using TowerDefenceBackend.DataAccess;
using System;
using Moq;
using System.Threading.Tasks;

namespace TowerDefence_Test
{
    [TestClass]
    public class MapMakerModelTests
    {
        private IDataAccess<GameSaveObject> _dataAccess;
        public MapMakerModel Model { get; set; }


        public void MockDA()
        {
            Table tablemock = new(15, 10);
            for (uint i = 0; i < 15; i++)
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

        public async Task<MapMakerModel> MakeMapMakerModel()
        {
            MapMakerModel newmodel = new(_dataAccess);
            await newmodel.LoadGameAsync("1");
            return newmodel;
        }

        [TestInitialize]
        public async Task Init()
        {
            MockDA();
            Model = await MakeMapMakerModel();
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

        [TestMethod]
        public void MapMakerModelInit()
        {
            Model?.CreateNewMap();
            Assert.IsNotNull(Model);
            Assert.IsNotNull(Model.BP);
            Assert.IsNotNull(Model.RP);
            Assert.IsNotNull(Model.Table);
        }

        [TestMethod]
        public void ChangeTableSizeSmallToLargeTest()
        {
            Model?.CreateNewMap();
            Assert.IsNotNull(Model);

            Model.ChangeTableSize(12, 15);
            Assert.AreEqual(Model.Table.Size.x,12);
            Assert.AreEqual(Model.Table.Size.y,15);
        }

        [TestMethod]
        public void ChangeTableSizeLargeToSmall()
        {
            Model?.CreateNewMap();
            Assert.IsNotNull(Model);

            Model.ChangeTableSize(15, 15);
            Assert.AreEqual(Model.Table.Size.x,15);
            Assert.AreEqual(Model.Table.Size.y, 15);

            Model.ChangeTableSize(5, 5);
            Assert.AreEqual(Model.Table.Size.x, 5);
            Assert.AreEqual(Model.Table.Size.y, 5);
        }

        [TestMethod]
        public void BuildMountainTest()
        {
            Model?.CreateNewMap();
            Assert.IsNotNull(Model);

            Model.SelectField(Model.Table[0, 0]);
            Model.SelectOption(MenuOption.BuildMountain);
            Terrain? terrain = Model?.Table[0, 0]?.Placement as Terrain;
            Assert.IsNotNull(Model?.Table[0, 0]?.Placement);
            Assert.AreEqual(Model?.Table[0, 0]?.Placement?.GetType(),typeof(Terrain));
            Assert.AreEqual(terrain?.Type,TerrainType.Mountain);
        }

        [TestMethod]
        public void BuildLakeTest()
        {
            Model?.CreateNewMap();
            Assert.IsNotNull(Model);

            Model.SelectField(Model.Table[0, 0]);
            Model.SelectOption(MenuOption.BuildLake);
            Terrain? terrain = Model?.Table[0, 0]?.Placement as Terrain;
            Assert.IsNotNull(Model?.Table[0, 0]?.Placement);
            Assert.AreEqual(Model?.Table[0, 0]?.Placement?.GetType(),typeof(Terrain));
            Assert.AreEqual(terrain?.Type,TerrainType.Lake);
        }

        [TestMethod]
        public void BuildCastleTest()
        {
            Model?.CreateNewMap();
            Assert.IsNotNull(Model);

            Model.SelectPlayer(Model.BP);

            Model.SelectField(Model.Table[0, 0]);
            Model.SelectOption(MenuOption.BuildCastle);
            Assert.IsNotNull(Model.Table[0, 0].Placement);
            Assert.AreEqual(Model?.Table[0, 0]?.Placement?.GetType(),typeof(TowerDefenceBackend.Persistence.Castle));
            Assert.AreEqual(Model?.Table[0, 0]?.Placement?.Owner, Model?.BP);

            Model?.SelectPlayer(Model.RP);

            Model?.SelectField(Model.Table[0, 2]);
            Model?.SelectOption(MenuOption.BuildCastle);
            Assert.IsNotNull(Model?.Table[0, 2].Placement);
            Assert.AreEqual(Model?.Table[0, 2]?.Placement?.GetType(),typeof(TowerDefenceBackend.Persistence.Castle));
            Assert.AreEqual(Model?.Table[0, 2]?.Placement?.Owner,Model?.RP);
        }

        [TestMethod]
        public void BuildBarackTest()
        {
            Model?.CreateNewMap();
            Assert.IsNotNull(Model);

            Model?.SelectPlayer(Model?.BP);

            Model?.SelectField(Model.Table[0, 0]);
            Model?.SelectOption(MenuOption.BuildBarrack);
            Assert.IsNotNull(Model?.Table[0, 0].Placement);
            Assert.AreEqual(Model?.Table[0, 0]?.Placement?.GetType(),typeof(Barrack));
            Assert.AreEqual(Model?.Table[0, 0]?.Placement?.Owner, Model?.BP);

            Model?.SelectPlayer(Model?.RP);

            Model?.SelectField(Model.Table[0, 2]);
            Model?.SelectOption(MenuOption.BuildBarrack);
            Assert.IsNotNull(Model?.Table[0, 2].Placement);
            Assert.AreEqual(Model?.Table[0, 2]?.Placement?.GetType(), typeof(Barrack));
            Assert.AreEqual(Model?.Table[0, 2]?.Placement?.Owner, Model?.RP);
        }

        [TestMethod]
        public void DestroyPlacementTest()
        {
            Model?.CreateNewMap();
            Assert.IsNotNull(Model);

            Model?.SelectPlayer(Model?.BP);

            Model?.SelectField(Model.Table[0, 0]);
            Model?.SelectOption(MenuOption.BuildBarrack);
            Assert.IsNotNull(Model?.Table[0, 0].Placement);
            Assert.AreEqual(Model?.Table[0, 0]?.Placement?.GetType(), typeof(Barrack));
            Assert.AreEqual(Model?.Table[0, 0]?.Placement?.Owner, Model?.BP);

            Model?.SelectField(Model.Table[0, 0]);
            Model?.SelectOption(MenuOption.DestroyPlacement);
            Assert.IsNull(Model?.Table[0, 0].Placement);
        }

        [TestMethod]
        public void ObstructedPathForBarrack()
        {
            Model?.CreateNewMap();
            Assert.IsNotNull(Model);

            Model?.SelectPlayer(Model?.BP);

            Model?.SelectField(Model.Table[0, 0]);
            Model?.SelectOption(MenuOption.BuildBarrack);
            Assert.AreEqual(Model?.Table[0, 0]?.Placement?.GetType(),typeof(Barrack));

            Model?.SelectPlayer(null);

            Model?.SelectField(Model.Table[0, 1]);
            Model?.SelectOption(MenuOption.BuildMountain);
            Terrain? terrain = Model?.Table[0, 1]?.Placement as Terrain;
            Assert.AreEqual(terrain?.Type,TerrainType.Mountain);

            Model?.SelectPlayer(Model?.RP);

            Model?.SelectField(Model.Table[0, 2]);
            Model?.SelectOption(MenuOption.BuildCastle);
            Assert.AreEqual(Model?.Table[0, 2]?.Placement?.GetType(),typeof(TowerDefenceBackend.Persistence.Castle));

            Model?.SelectPlayer(null);

            try
            {
                Model?.SelectField(Model.Table[1, 0]);
                Model?.SelectOption(MenuOption.BuildMountain);
            }
            catch (InvalidPlacementException)
            {
                Assert.IsNull(Model?.Table[1, 0].Placement);
            }

        }


    }
}
