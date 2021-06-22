using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using NSFacilita_Transporte.src.Classes.CTe;
using NSFacilita_Transporte.src.Classes.NFeAuth;

namespace facilitaTransporte
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        
        // aqui vai pegar as informacoes lidas para gerar cte e mdfe
        private void emitirDocumento_Click(object sender, EventArgs e)
        {
            // chamar funcao que gerar documentos
            Array retornoEmissao = facilitaTransporte.emitirDocumentos();
        }

        private void lerArquivoXML_Click(object sender, EventArgs e)
        {
        }
    }
}

