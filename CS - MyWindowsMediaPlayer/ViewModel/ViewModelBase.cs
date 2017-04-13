using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MyWindowsMediaPlayer.ViewModel
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        public bool NotifyPropertyChanged<T>(ref T variable, T value, [CallerMemberName] string property = null)
        {
            if (object.Equals(variable, value))
                return (false);

            variable = value;
            NotifyPropertyChanged(property);

            return (true);
        }
    }
}
