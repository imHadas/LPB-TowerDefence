using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TowerDefenceGame_LPB.Persistence;
using TowerDefenceGame_LPB.Model;
using TowerDefenceGame_LPB.DataAccess;
using System;
using System.Threading.Tasks;

namespace TowerDefence_Test
{
    [TestClass]
    public class MapMakerModelTests
    {
        private IDataAccess<GameSaveObject>? _dataAccess;
        public MapMakerModel? Model { get; set; }

        [TestInitialize]
        public void Init()
        {
            _dataAccess = new JsonDataAccess();
        }

        private MapMakerModel MakeMapMakerModel()
        {
            return new MapMakerModel(_dataAccess);
        }

        [TestMethod]
        public void MapMakerModelInit()
        {
            Model = MakeMapMakerModel();
            Assert.IsNotNull(Model);
            Assert.IsNotNull(Model.BP);
            Assert.IsNotNull(Model.RP);
            Assert.IsNotNull(Model.Table);
        }

        [TestMethod]
        public void ChangeTableSizeSmallToLargeTest()
        {
            Model = MakeMapMakerModel();
            Assert.IsNotNull(Model);

            Model.ChangeTableSize(12, 15);
            Assert.IsTrue(Model.Table.Size.x==12);
            Assert.IsTrue(Model.Table.Size.y==15);
        }

        [TestMethod]
        public void ChangeTableSizeLargeToSmall()
        {
            Model = MakeMapMakerModel();
            Assert.IsNotNull(Model);

            Model.ChangeTableSize(15, 15);
            Assert.IsTrue(Model.Table.Size.x == 15);
            Assert.IsTrue(Model.Table.Size.y == 15);

            Model.ChangeTableSize(5, 5);
            Assert.IsTrue(Model.Table.Size.x == 5);
            Assert.IsTrue(Model.Table.Size.y == 5);
        }

        [TestMethod]
        public void BuildMountainTest()
        {
            Model = MakeMapMakerModel();
            Assert.IsNotNull(Model);

            Model.SelectField(Model.Table[0, 0]);
            Model.SelectOption(MenuOption.BuildMountain);
            Assert.IsNotNull(Model.Table[0, 0].Placement);
            Assert.IsTrue(Model.Table[0, 0].Placement.GetType() == typeof(Terrain));
            Assert.IsTrue(((Terrain)Model.Table[0, 0].Placement).Type == TerrainType.Mountain);
        }

        [TestMethod]
        public void BuildLakeTest()
        {
            Model = MakeMapMakerModel();
            Assert.IsNotNull(Model);

            Model.SelectField(Model.Table[0, 0]);
            Model.SelectOption(MenuOption.BuildLake);
            Assert.IsNotNull(Model.Table[0, 0].Placement);
            Assert.IsTrue(Model.Table[0, 0].Placement.GetType() == typeof(Terrain));
            Assert.IsTrue(((Terrain)Model.Table[0, 0].Placement).Type == TerrainType.Lake);
        }

        [TestMethod]
        public void BuildCastleTest()
        {
            Model = MakeMapMakerModel();
            Assert.IsNotNull(Model);

            Model.SelectedPlayer = Model.BP;

            Model.SelectField(Model.Table[0, 0]);
            Model.SelectOption(MenuOption.BuildCastle);
            Assert.IsNotNull(Model.Table[0, 0].Placement);
            Assert.IsTrue(Model.Table[0, 0].Placement.GetType() == typeof(TowerDefenceGame_LPB.Persistence.Castle));
            Assert.IsTrue(Model.Table[0, 0].Placement.Owner == Model.BP);

            Model.SelectedPlayer = Model.RP;

            Model.SelectField(Model.Table[0, 2]);
            Model.SelectOption(MenuOption.BuildCastle);
            Assert.IsNotNull(Model.Table[0, 2].Placement);
            Assert.IsTrue(Model.Table[0, 2].Placement.GetType() == typeof(TowerDefenceGame_LPB.Persistence.Castle));
            Assert.IsTrue(Model.Table[0, 2].Placement.Owner == Model.RP);
        }

        [TestMethod]
        public void BuildBarackTest()
        {
            Model = MakeMapMakerModel();
            Assert.IsNotNull(Model);

            Model.SelectedPlayer = Model.BP;

            Model.SelectField(Model.Table[0, 0]);
            Model.SelectOption(MenuOption.BuildBarrack);
            Assert.IsNotNull(Model.Table[0, 0].Placement);
            Assert.IsTrue(Model.Table[0, 0].Placement.GetType() == typeof(Barrack));
            Assert.IsTrue(Model.Table[0, 0].Placement.Owner == Model.BP);

            Model.SelectedPlayer = Model.RP;

            Model.SelectField(Model.Table[0, 2]);
            Model.SelectOption(MenuOption.BuildBarrack);
            Assert.IsNotNull(Model.Table[0, 2].Placement);
            Assert.IsTrue(Model.Table[0, 2].Placement.GetType() == typeof(Barrack));
            Assert.IsTrue(Model.Table[0, 2].Placement.Owner == Model.RP);
        }

        [TestMethod]
        public void DestroyPlacementTest()
        {
            Model = MakeMapMakerModel();
            Assert.IsNotNull(Model);

            Model.SelectedPlayer = Model.BP;

            Model.SelectField(Model.Table[0, 0]);
            Model.SelectOption(MenuOption.BuildBarrack);
            Assert.IsNotNull(Model.Table[0, 0].Placement);
            Assert.IsTrue(Model.Table[0, 0].Placement.GetType() == typeof(Barrack));
            Assert.IsTrue(Model.Table[0, 0].Placement.Owner == Model.BP);

            Model.SelectField(Model.Table[0, 0]);
            Model.SelectOption(MenuOption.DestroyPlacement);
            Assert.IsNull(Model.Table[0, 0].Placement);
        }

        [TestMethod]
        public void ObstructedPathForBarrack()
        {
            Model = MakeMapMakerModel();
            Assert.IsNotNull(Model);

            Model.SelectedPlayer = Model.BP;

            Model.SelectField(Model.Table[0, 0]);
            Model.SelectOption(MenuOption.BuildBarrack);
            Assert.AreEqual(Model?.Table[0, 0]?.Placement?.GetType(),typeof(Barrack));

            Model.SelectedPlayer = null;

            Model.SelectField(Model.Table[0, 1]);
            Model.SelectOption(MenuOption.BuildMountain);
            Assert.AreEqual(((Terrain)Model.Table[0, 1].Placement).Type,TerrainType.Mountain);

            Model.SelectedPlayer = Model.RP;

            Model.SelectField(Model.Table[0, 2]);
            Model.SelectOption(MenuOption.BuildCastle);
            Assert.AreEqual(Model?.Table[0, 2]?.Placement?.GetType(),typeof(TowerDefenceGame_LPB.Persistence.Castle));

            Model.SelectedPlayer = null;

            try
            {
                Model.SelectField(Model.Table[1, 0]);
                Model.SelectOption(MenuOption.BuildMountain);
            }
            catch (InvalidPlacementException ex)
            {
                Assert.IsTrue(Model.Table[1, 0].Placement == null);
            }

        }


    }
}
