using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlumpaGrupper
{
    class UI_Controller
    {
        public static event Action FileSaved;

        public static void SaveFile()
        {
            FileSaved?.Invoke();
        }
    }
}
