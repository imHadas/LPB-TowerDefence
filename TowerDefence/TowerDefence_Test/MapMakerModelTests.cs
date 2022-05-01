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
        private GameSaveObject? _mockedGameSaveObject;
        private Table? _mockedTable;
        private Player? _mockedRedPlayer;
        private Player? _mockedBluePlayer;
        private MapMakerModel? _mapMakerModel;
        private Mock<IDataAccess<GameSaveObject>>? _mock;
        private IDataAccess<GameSaveObject>? _dataAccess;

        [TestInitialize]
        public void Init()
        {
            _mockedTable = MakeTable(10, 15);
            _mockedRedPlayer = new Player(PlayerType.RED);
            _mockedBluePlayer = new Player(PlayerType.BLUE);
            _mockedGameSaveObject = new GameSaveObject(_mockedTable, _mockedBluePlayer, _mockedRedPlayer);

            _mock = new Mock<IDataAccess<GameSaveObject>>();
            _mock.Setup(mock => mock.LoadAsync(It.IsAny<String>())).Returns(() => Task.FromResult(_mockedGameSaveObject));

            _mapMakerModel = new MapMakerModel(_mock.Object);

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

        [TestMethod]
        public void MapMakerModelInit()
        {
            _mapMakerModel?.CreateNewMap();
            Assert.IsNotNull(_mapMakerModel);
            Assert.IsNotNull(_mapMakerModel.BP);
            Assert.IsNotNull(_mapMakerModel.RP);
            Assert.IsNotNull(_mapMakerModel.Table);
        }

        [TestMethod]
        public void ChangeTableSizeSmallToLargeTest()
        {
            _mapMakerModel?.CreateNewMap();
            Assert.IsNotNull(_mapMakerModel);

            _mapMakerModel.ChangeTableSize(12, 15);
            Assert.AreEqual(_mapMakerModel.Table.Size.x,12);
            Assert.AreEqual(_mapMakerModel.Table.Size.y,15);
        }

        [TestMethod]
        public void ChangeTableSizeLargeToSmall()
        {
            _mapMakerModel?.CreateNewMap();
            Assert.IsNotNull(_mapMakerModel);

            _mapMakerModel.ChangeTableSize(15, 15);
            Assert.AreEqual(_mapMakerModel.Table.Size.x,15);
            Assert.AreEqual(_mapMakerModel.Table.Size.y, 15);

            _mapMakerModel.ChangeTableSize(5, 5);
            Assert.AreEqual(_mapMakerModel.Table.Size.x, 5);
            Assert.AreEqual(_mapMakerModel.Table.Size.y, 5);
        }

        [TestMethod]
        public void BuildMountainTest()
        {
            _mapMakerModel?.CreateNewMap();
            Assert.IsNotNull(_mapMakerModel);

            _mapMakerModel.SelectField(_mapMakerModel.Table[0, 0]);
            _mapMakerModel.SelectOption(MenuOption.BuildMountain);
            Terrain? terrain = _mapMakerModel?.Table[0, 0]?.Placement as Terrain;
            Assert.IsNotNull(_mapMakerModel?.Table[0, 0]?.Placement);
            Assert.AreEqual(_mapMakerModel?.Table[0, 0]?.Placement?.GetType(),typeof(Terrain));
            Assert.AreEqual(terrain?.Type,TerrainType.Mountain);
        }

        [TestMethod]
        public void BuildLakeTest()
        {
            _mapMakerModel?.CreateNewMap();
            Assert.IsNotNull(_mapMakerModel);

            _mapMakerModel.SelectField(_mapMakerModel.Table[0, 0]);
            _mapMakerModel.SelectOption(MenuOption.BuildLake);
            Terrain? terrain = _mapMakerModel?.Table[0, 0]?.Placement as Terrain;
            Assert.IsNotNull(_mapMakerModel?.Table[0, 0]?.Placement);
            Assert.AreEqual(_mapMakerModel?.Table[0, 0]?.Placement?.GetType(),typeof(Terrain));
            Assert.AreEqual(terrain?.Type,TerrainType.Lake);
        }

        [TestMethod]
        public void BuildCastleTest()
        {
            _mapMakerModel?.CreateNewMap();
            Assert.IsNotNull(_mapMakerModel);

            _mapMakerModel.SelectPlayer(_mapMakerModel.BP);

            _mapMakerModel.SelectField(_mapMakerModel.Table[0, 0]);
            _mapMakerModel.SelectOption(MenuOption.BuildCastle);
            Assert.IsNotNull(_mapMakerModel.Table[0, 0].Placement);
            Assert.AreEqual(_mapMakerModel?.Table[0, 0]?.Placement?.GetType(),typeof(TowerDefenceBackend.Persistence.Castle));
            Assert.AreEqual(_mapMakerModel?.Table[0, 0]?.Placement?.Owner, _mapMakerModel?.BP);

            _mapMakerModel?.SelectPlayer(_mapMakerModel.RP);

            _mapMakerModel?.SelectField(_mapMakerModel.Table[0, 2]);
            _mapMakerModel?.SelectOption(MenuOption.BuildCastle);
            Assert.IsNotNull(_mapMakerModel?.Table[0, 2].Placement);
            Assert.AreEqual(_mapMakerModel?.Table[0, 2]?.Placement?.GetType(),typeof(TowerDefenceBackend.Persistence.Castle));
            Assert.AreEqual(_mapMakerModel?.Table[0, 2]?.Placement?.Owner,_mapMakerModel?.RP);
        }

        [TestMethod]
        public void BuildBarackTest()
        {
            _mapMakerModel?.CreateNewMap();
            Assert.IsNotNull(_mapMakerModel);

            _mapMakerModel?.SelectPlayer(_mapMakerModel?.BP);

            _mapMakerModel?.SelectField(_mapMakerModel.Table[0, 0]);
            _mapMakerModel?.SelectOption(MenuOption.BuildBarrack);
            Assert.IsNotNull(_mapMakerModel?.Table[0, 0].Placement);
            Assert.AreEqual(_mapMakerModel?.Table[0, 0]?.Placement?.GetType(),typeof(Barrack));
            Assert.AreEqual(_mapMakerModel?.Table[0, 0]?.Placement?.Owner, _mapMakerModel?.BP);

            _mapMakerModel?.SelectPlayer(_mapMakerModel?.RP);

            _mapMakerModel?.SelectField(_mapMakerModel.Table[0, 2]);
            _mapMakerModel?.SelectOption(MenuOption.BuildBarrack);
            Assert.IsNotNull(_mapMakerModel?.Table[0, 2].Placement);
            Assert.AreEqual(_mapMakerModel?.Table[0, 2]?.Placement?.GetType(), typeof(Barrack));
            Assert.AreEqual(_mapMakerModel?.Table[0, 2]?.Placement?.Owner, _mapMakerModel?.RP);
        }

        [TestMethod]
        public void DestroyPlacementTest()
        {
            _mapMakerModel?.CreateNewMap();
            Assert.IsNotNull(_mapMakerModel);

            _mapMakerModel?.SelectPlayer(_mapMakerModel?.BP);

            _mapMakerModel?.SelectField(_mapMakerModel.Table[0, 0]);
            _mapMakerModel?.SelectOption(MenuOption.BuildBarrack);
            Assert.IsNotNull(_mapMakerModel?.Table[0, 0].Placement);
            Assert.AreEqual(_mapMakerModel?.Table[0, 0]?.Placement?.GetType(), typeof(Barrack));
            Assert.AreEqual(_mapMakerModel?.Table[0, 0]?.Placement?.Owner, _mapMakerModel?.BP);

            _mapMakerModel?.SelectField(_mapMakerModel.Table[0, 0]);
            _mapMakerModel?.SelectOption(MenuOption.DestroyPlacement);
            Assert.IsNull(_mapMakerModel?.Table[0, 0].Placement);
        }

        [TestMethod]
        public void ObstructedPathForBarrack()
        {
            _mapMakerModel?.CreateNewMap();
            Assert.IsNotNull(_mapMakerModel);

            _mapMakerModel?.SelectPlayer(_mapMakerModel?.BP);

            _mapMakerModel?.SelectField(_mapMakerModel.Table[0, 0]);
            _mapMakerModel?.SelectOption(MenuOption.BuildBarrack);
            Assert.AreEqual(_mapMakerModel?.Table[0, 0]?.Placement?.GetType(),typeof(Barrack));

            _mapMakerModel?.SelectPlayer(null);

            _mapMakerModel?.SelectField(_mapMakerModel.Table[0, 1]);
            _mapMakerModel?.SelectOption(MenuOption.BuildMountain);
            Terrain? terrain = _mapMakerModel?.Table[0, 1]?.Placement as Terrain;
            Assert.AreEqual(terrain?.Type,TerrainType.Mountain);

            _mapMakerModel?.SelectPlayer(_mapMakerModel?.RP);

            _mapMakerModel?.SelectField(_mapMakerModel.Table[0, 2]);
            _mapMakerModel?.SelectOption(MenuOption.BuildCastle);
            Assert.AreEqual(_mapMakerModel?.Table[0, 2]?.Placement?.GetType(),typeof(TowerDefenceBackend.Persistence.Castle));

            _mapMakerModel?.SelectPlayer(null);

            try
            {
                _mapMakerModel?.SelectField(_mapMakerModel.Table[1, 0]);
                _mapMakerModel?.SelectOption(MenuOption.BuildMountain);
            }
            catch (InvalidPlacementException)
            {
                Assert.IsNull(_mapMakerModel?.Table[1, 0].Placement);
            }

        }


    }
}
