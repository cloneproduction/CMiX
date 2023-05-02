using CMiX.MVVM.ViewModels;
using System.Windows.Input;

namespace CMiX.Studio.ViewModels
{
    public class Counter : ViewModel
    {
        #region CONSTRUCTORS
        public Counter() 
        {
            Count = 1;
            AddCommand = new RelayCommand(p => Add());
            SubCommand = new RelayCommand(p => Sub());
        }
        #endregion

        #region PROPERTIES
        public ICommand AddCommand { get; }
        public ICommand SubCommand { get; }

        private int _count;
        public int Count
        {
            get { return _count; }
            set
            {
                //if(Mementor != null)
                //    Mementor.PropertyChange(this, "Count");
                SetAndNotify(ref _count, value);
                //SendMessages(MessageAddress + nameof(Count), Count);
            }
        }
        #endregion

        #region ADD/SUB
        private void Add()
        {
            Count += 1;
        }

        private void Sub()
        {
            if (Count > 1)
                Count -= 1;
        }
        #endregion

        #region COPY/PASTE/RESET
        public void Reset()
        {
            Count = 1;
        }

        #endregion
    }
}