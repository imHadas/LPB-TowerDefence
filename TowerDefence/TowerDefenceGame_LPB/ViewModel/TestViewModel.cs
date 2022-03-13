using System;
using System.Collections.ObjectModel;
using TowerDefenceGame_LPB.Persistence;
using TowerDefenceGame_LPB.Model;

namespace TowerDefenceGame_LPB.ViewModel
{
    public class TestViewModel : ViewModelBase
    {
        private GameModel model;
        public int GridSize { get; set; }
        public ObservableCollection<TestField> Fields { get; set; }
        public TestViewModel(GameModel model)
        {
            this.model = model;
            GridSize = 11;

            GenerateTable();
        }
        private void GenerateTable()
        {
            bool isBarrack = false;
            bool isTower = false;
            bool isCastle = false;
            Fields = new ObservableCollection<TestField>();
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    if(model.Table[(uint)i, (uint)j].Placement != null)
                    {
                        if (model.Table[(uint)i, (uint)j].Placement.GetType() == typeof(Castle))
                        {
                            isCastle = true;
                        }
                        else if (model.Table[(uint)i, (uint)j].Placement.GetType() == typeof(Barrack))
                        {
                            isBarrack = true;
                        }
                        else if (model.Table[(uint)i, (uint)j].Placement.GetType() == typeof(Tower))
                        {
                            isTower = true;
                        }
                    }
                    
                    Fields.Add(new TestField
                    {
                        Coords = (j,i),
                        Number = i * GridSize + j,
                        BlueBasic = 0,
                        BlueTank = 0,
                        RedBasic = 0,
                        RedTank = 0,
                        PlayerType = model.Table[(uint)i,(uint)j].Placement?.Owner?.Type ?? PlayerType.NEUTRAL,
                        Placement = model.Table[(uint)i,(uint)j].Placement,
                        IsBarrack = isBarrack,
                        IsCastle = isCastle,
                        IsTower = isTower,
                        ClickCommand = new DelegateCommand(param=>BuildTower(Convert.ToInt32(param)))
                    });
                    isBarrack = false;
                    isTower = false;
                    isCastle = false;
                }
            }
        }
        public void BuildTower(int index)
        {
            TestField testField = Fields[index];
            testField.IsTower = true;
            testField.PlayerType = PlayerType.BLUE;
        }
    }
}
