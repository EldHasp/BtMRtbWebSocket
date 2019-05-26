using CommLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTMRTB
{
    public class TextVM : OnPropertyChangedClass
    {
        private string _text;

        public string Text { get => _text; set { SetProperty(ref _text, value); } }
    }
}
