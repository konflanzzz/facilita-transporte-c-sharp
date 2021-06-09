using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using NSFacilita_Transporte.src.Classes.CTe;
using NSFacilita_Transporte.src.Classes.MDFe;
using NSFacilita_Transporte.src.Classes.NFeAuth;
using DDFeAPIClientCSharp;
using Newtonsoft.Json;

namespace facilitaTransporte
{
    class facilitaTransporte {

        public static string emitirDocumentos()
        {
            string resposta = "";

            // Primeiro passo: obter o xml do documento
            string xmlNFeAutorizada = procurarDocumentos();

            //Segundo pasoso: ler o xml para criar um objeto de NFe autorizada
            TNfeProc NFeAutorizada = lerXML(xmlNFeAutorizada);

            // Terceiro passo: capturar os dados do objeto NFe e atribui-los ao objeto do CTe
            resposta = gerarCTe(NFeAutorizada);
            
            return resposta;
        }

        // faz download de documentos e retorna a string xml
        public static string procurarDocumentos()
        {
            string xmlNFe = "";
            string retorno = DDFeAPI.downloadUnico("07364617000135", "C:/ testeDownloadDDFe", "2", "", "55", "43210500063354950072559200000009261173401688", false, false, false);
            dynamic respostaJson = JsonConvert.DeserializeObject(retorno);
            xmlNFe = respostaJson.xml;
            return xmlNFe;
        }

        // faz a leitura de uma string de xml e retorna um objeto da NFe 
        public static TNfeProc lerXML(string xmlNFe)
        {
            StringReader stringReader = new StringReader(xmlNFe);
            XmlSerializer desserializador = new XmlSerializer(typeof(TNfeProc));
            TNfeProc NFeProc = (TNfeProc)desserializador.Deserialize(stringReader);
            return NFeProc; // Retornar um objeto TNfeProc
        }

        // gerar o arquivo de cte para emissao
        public static string gerarCTe(TNfeProc NFeRecebida)
        {
            string respostaEmissaoCTe = "";

            TCTe CTe = new TCTe
            {
                infCte = new TCTeInfCte
                {
                    versao = "3.00",
                    ide = new TCTeInfCteIde
                    {
                        cUF = (NSFacilita_Transporte.src.Classes.CTe.TCodUfIBGE)NFeRecebida.NFe.infNFe.ide.cUF,
                        cCT = "00000330",
                        CFOP = "", // definido pelo usuario,
                        natOp = NFeRecebida.NFe.infNFe.ide.natOp,
                        mod = TModCT.Item57,
                        serie = "0",
                        nCT = "2219",
                        dhEmi = DateTime.Now.ToString("s") + "-03:00",
                        tpImp = TCTeInfCteIdeTpImp.Item2,
                        tpEmis = TCTeInfCteIdeTpEmis.Item3,
                        cDV = "",
                        tpAmb = (NSFacilita_Transporte.src.Classes.CTe.TAmb)NFeRecebida.NFe.infNFe.ide.tpAmb,
                        tpCTe = TFinCTe.Item0,
                        procEmi = NSFacilita_Transporte.src.Classes.CTe.TProcEmi.Item0,
                        verProc = "1.02",
                        cMunEnv = NFeRecebida.NFe.infNFe.emit.enderEmit.cMun,
                        xMunEnv = NFeRecebida.NFe.infNFe.emit.enderEmit.xMun,
                        UFEnv = (NSFacilita_Transporte.src.Classes.CTe.TUf)NFeRecebida.NFe.infNFe.emit.enderEmit.UF,
                        modal = TModTransp.Item01,
                        tpServ = TCTeInfCteIdeTpServ.Item0,
                        cMunIni = NFeRecebida.NFe.infNFe.emit.enderEmit.cMun,
                        xMunIni = NFeRecebida.NFe.infNFe.emit.enderEmit.xMun,
                        UFIni = (NSFacilita_Transporte.src.Classes.CTe.TUf)NFeRecebida.NFe.infNFe.emit.enderEmit.UF,
                        cMunFim = NFeRecebida.NFe.infNFe.dest.enderDest.cMun,
                        xMunFim = NFeRecebida.NFe.infNFe.dest.enderDest.xMun,
                        UFFim = (NSFacilita_Transporte.src.Classes.CTe.TUf)NFeRecebida.NFe.infNFe.dest.enderDest.UF,
                        retira = TCTeInfCteIdeRetira.Item1,
                        indIEToma = TCTeInfCteIdeIndIEToma.Item9,
                    },
                    emit = new TCTeInfCteEmit
                    {
                        Item = NFeRecebida.NFe.infNFe.transp.transporta.Item,
                        ItemElementName = (NSFacilita_Transporte.src.Classes.CTe.ItemChoiceType1)NFeRecebida.NFe.infNFe.transp.transporta.ItemElementName,
                        IE = NFeRecebida.NFe.infNFe.transp.transporta.IE,
                        xNome = NFeRecebida.NFe.infNFe.transp.transporta.xNome,
                        xFant = NFeRecebida.NFe.infNFe.transp.transporta.xNome,
                        enderEmit = new NSFacilita_Transporte.src.Classes.CTe.TEndeEmi
                        {
                            xLgr = NFeRecebida.NFe.infNFe.transp.transporta.xEnder,
                            xMun = NFeRecebida.NFe.infNFe.transp.transporta.xMun,
                            UF = (TUF_sem_EX)NFeRecebida.NFe.infNFe.transp.transporta.UF
                        }
                    },
                    rem = new TCTeInfCteRem
                    {
                        ItemElementName = (NSFacilita_Transporte.src.Classes.CTe.ItemChoiceType2)NFeRecebida.NFe.infNFe.emit.ItemElementName,
                        Item = NFeRecebida.NFe.infNFe.emit.Item,
                        IE = NFeRecebida.NFe.infNFe.emit.IE,
                        xNome = NFeRecebida.NFe.infNFe.emit.xNome,
                        enderReme = new NSFacilita_Transporte.src.Classes.CTe.TEndereco
                        {
                            xLgr = NFeRecebida.NFe.infNFe.emit.enderEmit.xLgr,
                            nro = NFeRecebida.NFe.infNFe.emit.enderEmit.nro,
                            xBairro = NFeRecebida.NFe.infNFe.emit.enderEmit.xBairro,
                            xMun = NFeRecebida.NFe.infNFe.emit.enderEmit.xMun,
                            UF = (NSFacilita_Transporte.src.Classes.CTe.TUf)NFeRecebida.NFe.infNFe.emit.enderEmit.UF,
                            cMun = NFeRecebida.NFe.infNFe.emit.enderEmit.cMun,
                            xPais = "BRASIL",
                            cPais = "1058",
                            CEP = NFeRecebida.NFe.infNFe.emit.enderEmit.CEP
                        }
                    },
                    dest = new TCTeInfCteDest
                    {
                        ItemElementName = (NSFacilita_Transporte.src.Classes.CTe.ItemChoiceType5)NFeRecebida.NFe.infNFe.dest.ItemElementName,
                        Item = NFeRecebida.NFe.infNFe.dest.Item,
                        IE = NFeRecebida.NFe.infNFe.dest.IE,
                        xNome = NFeRecebida.NFe.infNFe.dest.xNome,
                        email = NFeRecebida.NFe.infNFe.dest.email,
                        ISUF = NFeRecebida.NFe.infNFe.dest.ISUF,
                        enderDest = new NSFacilita_Transporte.src.Classes.CTe.TEndereco
                        {
                            xLgr = NFeRecebida.NFe.infNFe.dest.enderDest.xLgr,
                            nro = NFeRecebida.NFe.infNFe.dest.enderDest.nro,
                            xBairro = NFeRecebida.NFe.infNFe.dest.enderDest.xBairro,
                            xCpl = NFeRecebida.NFe.infNFe.dest.enderDest.xCpl,
                            cMun = NFeRecebida.NFe.infNFe.dest.enderDest.cMun,
                            xMun = NFeRecebida.NFe.infNFe.dest.enderDest.xMun,
                            UF = (NSFacilita_Transporte.src.Classes.CTe.TUf)NFeRecebida.NFe.infNFe.dest.enderDest.UF,
                            cPais = NFeRecebida.NFe.infNFe.dest.enderDest.cPais,
                            xPais = NFeRecebida.NFe.infNFe.dest.enderDest.xPais,
                            CEP = NFeRecebida.NFe.infNFe.dest.enderDest.CEP
                        }
                    },
                    vPrest = new TCTeInfCteVPrest
                    {
                        vTPrest = NFeRecebida.NFe.infNFe.total.ICMSTot.vFrete,
                        vRec = NFeRecebida.NFe.infNFe.total.ICMSTot.vFrete,
                        // Comp = new TCTeInfCteVPrestComp { vComp = "", xNome = "" } // Array
                    },
                    imp = new TCTeInfCteImp
                    {
                        infAdFisco = NFeRecebida.NFe.infNFe.infAdic.infAdFisco,
                        vTotTrib = NFeRecebida.NFe.infNFe.total.ICMSTot.vTotTrib,
                        ICMSUFFim = new TCTeInfCteImpICMSUFFim
                        {
                            pICMSInter = NFeRecebida.NFe.infNFe.det[1].imposto.ICMSUFDest.pICMSInter.ToString(),
                            pFCPUFFim = NFeRecebida.NFe.infNFe.det[1].imposto.ICMSUFDest.pFCPUFDest,
                            pICMSUFFim = NFeRecebida.NFe.infNFe.det[1].imposto.ICMSUFDest.pICMSUFDest,
                            vBCUFFim = NFeRecebida.NFe.infNFe.det[1].imposto.ICMSUFDest.vBCUFDest,
                            vFCPUFFim = NFeRecebida.NFe.infNFe.det[1].imposto.ICMSUFDest.vFCPUFDest,
                            vICMSUFFim = NFeRecebida.NFe.infNFe.det[1].imposto.ICMSUFDest.vICMSUFDest,
                            vICMSUFIni = NFeRecebida.NFe.infNFe.det[1].imposto.ICMSUFDest.vICMSUFRemet,
                        }
                    },
                    Item = new TCTeInfCteInfCTeNorm
                    {
                        
                    }
                }
            };

            string CTeJSON = JsonConvert.SerializeObject(CTe);//String do cte serializado
            //respostaEmissaoCTe = NSSuite.emitirCTeSincrono(CTeJSON, "57", "json", "07364617000135", "XP", "2", "C:/documentosFacilitaTransporte", true, false);
            return CTeJSON;
        }

        // gera o arquivo de mdfe para emissao
        public string gerarMDFe(TNfeProc NFeRecebida)
        {
            string respostaEmissaoMDFe = "";

            string MDFeJSON = ""; //String do mdfe serializado
            respostaEmissaoMDFe = NSSuite.emitirMDFeSincrono(MDFeJSON, "json", "07364617000135", "XP", "2", "C:/documentosFacilitaTransporte",false,false);
            return respostaEmissaoMDFe;

        }
    }
}
