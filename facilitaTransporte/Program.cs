using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace facilitaTransporte
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Array retornoEmissao = facilitaTransporte.emitirDocumentos();
        }
    }
}
