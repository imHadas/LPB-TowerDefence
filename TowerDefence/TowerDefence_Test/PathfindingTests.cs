using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TowerDefenceBackend.Persistence;
using TowerDefenceBackend.Model;
using System;

namespace TowerDefence_Test
{
    [TestClass]
    public class PathfindingTests
    {
        public IPathfinder? Pathfinder { get; set; }

        [TestInitialize]
        public void Init()
        {
            Pathfinder = null;
        }

        #region Utility Methods

        private Table MakeTable(uint width, uint height)
        {
            Table output = new(height, width);
            for (uint i = 0; i < width; i++)
            {
                for (uint j = 0; j < height; j++)
                {
                    output[i, j] = new(i, j);
                }
            }

            return output;
        }

        private IPathfinder? MakePathfinder(Table table)
        {
            return new AStar(table);
        }

        private IPathfinder? MakePathfinder(uint tablewidth, uint tableheight)
        {
            return MakePathfinder(MakeTable(tablewidth, tableheight));
        }

        #endregion
        #region Test Methods

        // Empty = completely empty map
        // Unobstructed = there is a valid path
        // Obstructed = no valid path
        // Blocked = there are obstacles blocking the otherwise shortest path

        [TestMethod]
        public void PathfinderInit()
        {
            Pathfinder = MakePathfinder(2,2);
            Assert.IsNotNull(Pathfinder);
        }

        [TestMethod]
        public void StartIsFinish()
        {
            Pathfinder = MakePathfinder(9,9);
            Assert.IsNotNull(Pathfinder);

            IList<(uint, uint)> path = Pathfinder.FindPath((1, 1), (1, 1));
            Assert.AreEqual(1, path.Count);
            Assert.AreEqual(((uint)1, (uint)1), path[0]);
        }


        [TestMethod, TestCategory("Empty"), TestCategory("Straight")]
        public void StraightEmpty()
        {
            Pathfinder = MakePathfinder(9, 9);
            Assert.IsNotNull(Pathfinder);

            IList<(uint, uint)> path = Pathfinder.FindPath((4, 1), (4, 7));
            Assert.AreEqual(7, path.Count);
            Assert.AreEqual(((uint)4, (uint)7), path[path.Count - 1]);
        }

        [TestMethod, TestCategory("Empty"), TestCategory("Sideways")]
        public void SidewaysEmpty()
        {
            Pathfinder = MakePathfinder(9, 9);
            Assert.IsNotNull(Pathfinder);

            IList<(uint, uint)> path = Pathfinder.FindPath((0, 0), (8, 8));
            Assert.AreEqual(17, path.Count);
            Assert.AreEqual(((uint)8, (uint)8), path[path.Count - 1]);
        }

        [TestMethod, TestCategory("Unobstructed"), TestCategory("Straight")]
        public void StraightUnobstructedClearPath()
        {
            Table table = MakeTable(5, 5);
            table[0, 1].Placement = new Terrain(0, 1, TerrainType.Mountain);
            table[1, 1].Placement = new Terrain(1, 1, TerrainType.Mountain);
            table[2, 1].Placement = new Terrain(2, 1, TerrainType.Mountain);
            table[2, 2].Placement = new Terrain(2, 2, TerrainType.Mountain);
            IPathfinder? pathfinder = MakePathfinder(table);
            Assert.IsNotNull(pathfinder);
            IList<(uint, uint)> path = pathfinder.FindPath((0, 0), (4, 0));
            Assert.AreEqual(5, path.Count);
            Assert.AreEqual(((uint)4, (uint)0), path[path.Count - 1]);
        }

        [TestMethod, TestCategory("Unobstructed"), TestCategory("Straight")]
        public void StraightUnobstructedBlockedPath()
        {
            Table table = MakeTable(5, 5);
            table[0, 1].Placement = new Terrain(0, 1, TerrainType.Mountain);
            table[2, 0].Placement = new Terrain(2, 0, TerrainType.Mountain);
            table[2, 1].Placement = new Terrain(2, 1, TerrainType.Mountain);
            table[2, 2].Placement = new Terrain(2, 2, TerrainType.Mountain);
            IPathfinder? pathfinder = MakePathfinder(table);
            Assert.IsNotNull(pathfinder);
            IList<(uint, uint)> path = pathfinder.FindPath((0, 0), (4, 0));
            Assert.AreEqual(11, path.Count);
            Assert.AreEqual(((uint)4, (uint)0), path[path.Count - 1]);
        }

        [TestMethod, TestCategory("Unobstructed"), TestCategory("Sideways")]
        public void SidewaysUnobstructedBlockedPath()
        {
            Table table = MakeTable(5, 5);
            table[1, 0].Placement = new Terrain(1, 0, TerrainType.Mountain);
            table[1, 1].Placement = new Terrain(1, 1, TerrainType.Mountain);
            table[2, 1].Placement = new Terrain(2, 1, TerrainType.Mountain);
            table[2, 2].Placement = new Terrain(2, 2, TerrainType.Mountain);
            table[1, 4].Placement = new Terrain(1, 4, TerrainType.Mountain);
            IPathfinder? pathfinder = MakePathfinder(table);
            Assert.IsNotNull(pathfinder);
            IList<(uint, uint)> path = pathfinder.FindPath((0, 0), (4,4));
            Assert.AreEqual(9, path.Count);
            Assert.AreEqual(((uint)4, (uint)4), path[path.Count - 1]);
        }

        [TestMethod, TestCategory("Obstructed"), TestCategory("Straight")]
        public void StraightObstructed()
        {
            Table table = MakeTable(5, 5);
            table[1, 0].Placement = new Terrain(1, 0, TerrainType.Mountain);
            table[1, 1].Placement = new Terrain(1, 1, TerrainType.Mountain);
            table[1, 2].Placement = new Terrain(1, 2, TerrainType.Mountain);
            table[1, 3].Placement = new Terrain(1, 3, TerrainType.Mountain);
            table[1, 4].Placement = new Terrain(1, 4, TerrainType.Mountain);
            IPathfinder? pathfinder = MakePathfinder(table);
            Assert.IsNotNull(pathfinder);
            IList<(uint, uint)> path = pathfinder.FindPath((0, 0), (4, 0));
            Assert.AreEqual(0, path.Count);
        }

        [TestMethod, TestCategory("Obstructed"), TestCategory("Sideways")]
        public void SidewaysObstructed()
        {
            Table table = MakeTable(5, 5);
            table[0, 4].Placement = new Terrain(0, 4, TerrainType.Mountain);
            table[1, 3].Placement = new Terrain(1, 3, TerrainType.Mountain);
            table[2, 2].Placement = new Terrain(2, 2, TerrainType.Mountain);
            table[3, 1].Placement = new Terrain(3, 1, TerrainType.Mountain);
            table[4, 0].Placement = new Terrain(4, 0, TerrainType.Mountain);
            IPathfinder? pathfinder = MakePathfinder(table);
            Assert.IsNotNull(pathfinder);
            IList<(uint, uint)> path = pathfinder.FindPath((0, 0), (4, 4));
            Assert.AreEqual(0, path.Count);
        }

        #endregion
    }
}