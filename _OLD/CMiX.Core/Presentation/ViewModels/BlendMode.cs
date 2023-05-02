using CMiX.MVVM.ViewModels;

namespace CMiX.Studio.ViewModels
{
    public class BlendMode : Sendable
    {
        public BlendMode(Beat masterBeat)
        {
            Mode = ((MVVM.ViewModels.BlendMode)0).ToString();
        }

        private string _mode;
        public string Mode
        {
            get { return _mode; }
            set
            {
                //Mementor.PropertyChange(this, nameof(Mode));
                SetAndNotify(ref _mode, value);
                OnSendChange(this.GetModel(), this.GetMessageAddress());
            }
        }

        public void Reset()
        {
            Mode = ((MVVM.ViewModels.BlendMode)0).ToString();
        }
    }
}
