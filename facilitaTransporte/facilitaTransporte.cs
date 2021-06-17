using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using NSFacilita_Transporte.src.Classes.CTe;
using NSFacilita_Transporte.src.Classes.MDFe;
using NSFacilita_Transporte.src.Classes.NFeAuth;
using DDFeAPIClientCSharp;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace facilitaTransporte
{
    class facilitaTransporte
    {

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

            TCTe CTe = new TCTe();

            CTe.infCte = new TCTeInfCte
            {
                imp = new TCTeInfCteImp(),
                versao = "3.00"
            };

            CTe.infCte.ide = new TCTeInfCteIde
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
                retira = TCTeInfCteIdeRetira.Item1, // input
                indIEToma = TCTeInfCteIdeIndIEToma.Item9,
                dhCont = NFeRecebida.NFe.infNFe.ide.dhCont,
                xJust = NFeRecebida.NFe.infNFe.ide.xJust
            };

            switch (NFeRecebida.NFe.infNFe.transp.modFrete)
            {
                // Expedidor? // input // nao e possivel obter da NFe

                // Recebedor? // input // nao e possivel obter da NFe

                case TNFeInfNFeTranspModFrete.Item0: // Remetente - CIF

                    CTe.infCte.ide.Item = new TCTeInfCteIdeToma3
                    {
                        toma = TCTeInfCteIdeToma3Toma.Item0 // Remetente
                    };

                    break;

                case TNFeInfNFeTranspModFrete.Item1: // Destinatario - FOB

                    CTe.infCte.ide.Item = new TCTeInfCteIdeToma3
                    {
                        toma = TCTeInfCteIdeToma3Toma.Item3 // Expedidor
                    };

                    break;

                case TNFeInfNFeTranspModFrete.Item2: // Tereceiros

                    CTe.infCte.ide.Item = new TCTeInfCteIdeToma4 // input
                    {
                        toma = TCTeInfCteIdeToma4Toma.Item4, // Outros
                        ItemElementName = NSFacilita_Transporte.src.Classes.CTe.ItemChoiceType.CNPJ,
                        Item = "07364617000135",
                        IE = "01756489705",
                        email = "teste@teste.com",
                        enderToma = new NSFacilita_Transporte.src.Classes.CTe.TEndereco
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
                        },
                    };

                    break;

                case TNFeInfNFeTranspModFrete.Item3: // Transporte por conta do Remetente

                    CTe.infCte.ide.Item = new TCTeInfCteIdeToma3
                    {
                        toma = TCTeInfCteIdeToma3Toma.Item0 // Remetente
                    };

                    break;

                case TNFeInfNFeTranspModFrete.Item4: // Transporte por conta do Destinatario

                    CTe.infCte.ide.Item = new TCTeInfCteIdeToma3
                    {
                        toma = TCTeInfCteIdeToma3Toma.Item3 // Destinatario
                    };

                    break;
            }

            CTe.infCte.emit = new TCTeInfCteEmit
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
            };

            CTe.infCte.rem = new TCTeInfCteRem
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
                    CEP = NFeRecebida.NFe.infNFe.emit.enderEmit.CEP,
                    xCpl = NFeRecebida.NFe.infNFe.emit.enderEmit.xCpl
                }
            };

            CTe.infCte.dest = new TCTeInfCteDest
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
            };

            CTe.infCte.vPrest = new TCTeInfCteVPrest
            {
                vTPrest = "1000.00",
                vRec = "1000.00",
            };

            CTe.infCte.vPrest.Comp[1] = new TCTeInfCteVPrestComp
            {
                vComp = "0.01",
                xNome = "0.01"
            };

            TCTeInfCteInfCTeNormInfCargaInfQ[] infQArray = new TCTeInfCteInfCTeNormInfCargaInfQ[1];
            TCTeInfCteInfCTeNormInfCargaInfQ infQ1 = new TCTeInfCteInfCTeNormInfCargaInfQ
            {
                cUnid = TCTeInfCteInfCTeNormInfCargaInfQCUnid.Item01,
                tpMed = "KILOS",
                qCarga = "15000.00"
            };

            infQArray[1] = infQ1;

            TCTeInfCteInfCTeNormInfCarga infCargaAux = new TCTeInfCteInfCTeNormInfCarga
            {
                vCargaAverb = NFeRecebida.NFe.infNFe.total.ICMSTot.vNF,
                proPred = "DIVERSOS",
                xOutCat = "DIVERSOS",
                vCarga = NFeRecebida.NFe.infNFe.total.ICMSTot.vNF,
                infQ = infQArray
            };

            CTe.infCte.Item = new TCTeInfCteInfCTeNormInfCarga();
            CTe.infCte.Item = infCargaAux;

            CTe.infCte.imp = new TCTeInfCteImp
            {
                infAdFisco = NFeRecebida.NFe.infNFe.infAdic.infAdFisco,
                vTotTrib = "180.00",
                ICMS = new TImp
                {
                    Item = new TImpICMS00
                    {
                        CST = TImpICMS00CST.Item00,
                        pICMS = "18.00",
                        vBC = "1000.00",
                        vICMS = "180.00"
                    },
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

            TMDFe MDFe = new TMDFe();
            MDFe.infMDFe = new TMDFeInfMDFe
            {

            };

            string MDFeJSON = ""; //String do mdfe serializado
            respostaEmissaoMDFe = NSSuite.emitirMDFeSincrono(MDFeJSON, "json", "07364617000135", "XP", "2", "C:/documentosFacilitaTransporte", false, false);
            return respostaEmissaoMDFe;

        }
    }
}