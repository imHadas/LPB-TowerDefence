using System;
using System.Collections.ObjectModel;

namespace TowerDefenceGame_LPB.ViewModel
{
    public class TestViewModel : ViewModelBase
    {
        public int GridSize { get; set; }
        public ObservableCollection<TestField> Fields { get; set; }
        public TestViewModel()
        {
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
                        BlueBasic = (i+1)*(j+1),
                        BlueTank = (i + 1) * (j + 2),
                        RedBasic = (i + 1) * (j + 3),
                        RedTank = (i + 1) * (j + 4)
                    });
                }
            }
        }
    }
}
