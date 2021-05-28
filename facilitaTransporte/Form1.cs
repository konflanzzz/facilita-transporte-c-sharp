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
            FileStream stream = new FileStream("C:/testeDownloadDDFe/xmls/43210500063354950072559200000009261173401688-procNFe.xml", FileMode.Open);
            XmlSerializer desserializador = new XmlSerializer(typeof(TNfeProc));
            TNfeProc NFeAutorizada = (TNfeProc)desserializador.Deserialize(stream);
            TCTe cTe = new TCTe();
            cTe.infCte = new TCTeInfCte();
            cTe.infCte.dest = new TCTeInfCteDest();
            cTe.infCte.dest.Item = NFeAutorizada.NFe.infNFe.dest.Item;
            cTe.infCte.emit.Item = "07364617000135";
        }

        private void lerArquivoXML_Click(object sender, EventArgs e)
        {

            //aqui é a leitura dos arquivos de xml para identificar se é ou nao transportadora ou destinatario
            //essa leitura pode ocorrer logo apos ser gerado o arquivo pela funcao salvar xml
            //ler o arquivo logo que é salvo, se for para emitir documentos, mover para uma outra pasta para fazer as leituras
            //validar se é destinatario ou transportadora


            // Leitura da String XML no retorno da API de DDFe de Download Unico

            //String retorno = DDFeAPI.downloadUnico("07364617000135", "C:/ testeDownloadDDFe","2","","55", "43210500063354950072559200000009261173401688",false,false,false);
            //dynamic respostaJson = JsonConvert.DeserializeObject(retorno);
            //string xmlAutorizado = respostaJson.xml;
            //StringReader stringReader = new StringReader(xmlAutorizado);
            //XmlSerializer desserializador = new XmlSerializer(typeof(NSFacilita_Transporte.src.Classes.NFeAuth.TNfeProc));
            //NSFacilita_Transporte.src.Classes.NFeAuth.TNfeProc NFeAutorizada = (NSFacilita_Transporte.src.Classes.NFeAuth.TNfeProc)desserializador.Deserialize(stringReader);

            // Leitura de arquivo XML
            FileStream stream = new FileStream("C:/testeDownloadDDFe/xmls/43210500063354950072559200000009261173401688-procNFe.xml", FileMode.Open);
            XmlSerializer desserializador = new XmlSerializer(typeof(NSFacilita_Transporte.src.Classes.NFeAuth.TNfeProc));
            NSFacilita_Transporte.src.Classes.NFeAuth.TNfeProc NFeAutorizada = (NSFacilita_Transporte.src.Classes.NFeAuth.TNfeProc)desserializador.Deserialize(stream);

            // Validacao Transportadora x Destinatario

            if (NFeAutorizada.NFe.infNFe.dest.Item.Equals("07364617000135")){
                textBox1.Text = "DESTINATARIO";
            }
            if (NFeAutorizada.NFe.infNFe.transp.transporta.Item.Equals("07364617000135")){
                textBox1.Text = "TRANSPORTADORA";
            }
        }
    }
}

