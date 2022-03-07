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
            Fields = new ObservableCollection<TestField>();
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    Fields.Add(new TestField
                    {
                        Coords = (j,i),
                        Number = i * GridSize + j,
                        BlueBasic = 0,
                        BlueTank = 0,
                        RedBasic = 0,
                        RedTank = 0,
                        PlayerType = model.Table.fields[(uint)i,(uint)j].Placement.Owner.Type,
                        Placement = model.Table.fields[(uint)i,(uint)j].Placement,
                        ClickCommand = new DelegateCommand(param=>BuildTower(Convert.ToInt32(param)))
                    });
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
