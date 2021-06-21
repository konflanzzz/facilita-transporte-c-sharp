using System;
using System.IO;
using System.Xml.Serialization;
using NSFacilita_Transporte.src.Classes.CTe;
using NSFacilita_Transporte.src.Classes.MDFe;
using NSFacilita_Transporte.src.Classes.NFeAuth;
using DDFeAPIClientCSharp;
using Newtonsoft.Json;

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
            //string respostaEmissaoCTe = gerarCTe(NFeAutorizada);

            // Quarto passo: capturar os dados do objeto NFe e atribui-los ao objeto do MDFe e/ou CTe
            string respostarespostaEmissaoMDFe = gerarMDFe(NFeAutorizada);

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

        public static string cteToXML(object CTe)
        {
            using (var stringwriter = new StringWriter())
            {
                var serializer = new XmlSerializer(CTe.GetType());
                serializer.Serialize(stringwriter, CTe);
                return stringwriter.ToString();
            }
        }

        public static string mdfeToXML(object MDFe)
        {
            using (var stringwriter = new StringWriter())
            {
                var serializer = new XmlSerializer(MDFe.GetType());
                serializer.Serialize(stringwriter, MDFe);
                return stringwriter.ToString();
            }
        }

        // gerar o arquivo de cte para emissao
        public static string gerarCTe(TNfeProc NFeRecebida) 
        // uma funcao gerar os dois documentos? para pegar dados tanto do cte como do mdfe?

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
                CFOP = "5353", // definido pelo usuario,
                natOp = NFeRecebida.NFe.infNFe.ide.natOp,
                mod = TModCT.Item57,
                serie = "0",
                nCT = "2222",
                dhEmi = DateTime.Now.ToString("s") + "-03:00",
                tpImp = TCTeInfCteIdeTpImp.Item2,
                tpEmis = TCTeInfCteIdeTpEmis.Item1,
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
                    xLgr = "ROD RST 470 KM 221 BLOCO A",
                    nro = "221",
                    xBairro = "CAMAQUA",
                    cMun = "4303509",
                    xMun = "GARIBALDI",
                    CEP = "96180000",
                    UF = TUF_sem_EX.RS,
                    fone = "5434638266"
                }
            };

            CTe.infCte.rem = new TCTeInfCteRem
            {
                ItemElementName = (NSFacilita_Transporte.src.Classes.CTe.ItemChoiceType2)NFeRecebida.NFe.infNFe.emit.ItemElementName,
                Item = NFeRecebida.NFe.infNFe.emit.Item,
                IE = NFeRecebida.NFe.infNFe.emit.IE,
                xNome = "CT-E EMITIDO EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL", //NFeRecebida.NFe.infNFe.emit.xNome,
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
                xNome = "CT-E EMITIDO EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL", //NFeRecebida.NFe.infNFe.dest.xNome,
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

            CTe.infCte.imp = new TCTeInfCteImp
            {
                infAdFisco = "Teste de emissao integra",
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

            // Cria um array auxiliar para inserir no array
            TCTeInfCteInfCTeNormInfCargaInfQ[] infQArray = new TCTeInfCteInfCTeNormInfCargaInfQ[1];

            // cria um objeto auxiliar para inserir no array
            TCTeInfCteInfCTeNormInfCargaInfQ infQ0 = new TCTeInfCteInfCTeNormInfCargaInfQ
            {
                cUnid = TCTeInfCteInfCTeNormInfCargaInfQCUnid.Item01,
                tpMed = "KILOS",
                qCarga = "13000.0000"
            };

            // Array para o grupo infQ
            infQArray[0] = infQ0;

            // Cria um array auxiliar para inserir no array
            TCTeInfCteInfCTeNormInfDocInfNF[] infNFArray = new TCTeInfCteInfCTeNormInfDocInfNF[1];

            // cria um objeto auxiliar para inserir no array
            TCTeInfCteInfCTeNormInfDocInfNF infNF0 = new TCTeInfCteInfCTeNormInfDocInfNF
            {
                mod = (TModNF)NFeRecebida.NFe.infNFe.ide.mod,
                serie = NFeRecebida.NFe.infNFe.ide.serie,
                nDoc = NFeRecebida.NFe.infNFe.ide.nNF,
                dEmi = NFeRecebida.NFe.infNFe.ide.dhEmi.Remove(10), // precisa fazer um trim para remover a hora
                vBC = NFeRecebida.NFe.infNFe.total.ICMSTot.vBC,
                vICMS = NFeRecebida.NFe.infNFe.total.ICMSTot.vICMS,
                vBCST = NFeRecebida.NFe.infNFe.total.ICMSTot.vBCST,
                vST = NFeRecebida.NFe.infNFe.total.ICMSTot.vST,
                vProd = NFeRecebida.NFe.infNFe.total.ICMSTot.vProd,
                vNF = NFeRecebida.NFe.infNFe.total.ICMSTot.vNF,
                nCFOP = NFeRecebida.NFe.infNFe.det[0].prod.CFOP,
                nPeso = "13000.000"
            };

            infNFArray[0] = infNF0;

            CTe.infCte.Item = new TCTeInfCteInfCTeNorm
            {
                infCarga = new TCTeInfCteInfCTeNormInfCarga
                {
                    vCargaAverb = NFeRecebida.NFe.infNFe.total.ICMSTot.vNF,
                    proPred = "DIVERSOS",
                    xOutCat = "DIVERSOS",
                    vCarga = NFeRecebida.NFe.infNFe.total.ICMSTot.vNF,
                    infQ = infQArray
                },

                infDoc = new TCTeInfCteInfCTeNormInfDoc
                {
                    Items = infNFArray
                },

                infModal = new TCTeInfCteInfCTeNormInfModal
                {
                    versaoModal = "3.00",
                    rodo = new TCTeRodo
                    {
                        RNTRC = "09549369"
                    }
                }
            };

            string CTeJSON = JsonConvert.SerializeObject(CTe);
            string CTeXML = cteToXML(CTe); // serializar o xml
            respostaEmissaoCTe = NSSuite.emitirCTeSincrono(CTeXML, "57", "xml", "07364617000135", "XP", "2", "C:/documentosFacilitaTransporte", false, false);
            
            // caso queira informar chCTe no MDFe
            //dynamic respostaEmissaoCTeJSON = JsonConvert.DeserializeObject(respostaEmissaoCTe);
            //string chCTeAutorizado = respostaEmissaoCTeJSON.chCTe;
            
            return respostaEmissaoCTe;
        }

        // gera o arquivo de mdfe para emissao
        public static string gerarMDFe(TNfeProc NFeRecebida)
        {
            string respostaEmissaoMDFe = "";

            TMDFe MDFe = new TMDFe();
            
            MDFe.infMDFe = new TMDFeInfMDFe
            {
                versao = "3.00",
                Id = "MDFe",
            };

            // array auxiliar
            TMDFeInfMDFeIdeInfMunCarrega[] infMunCarregaArray = new TMDFeInfMDFeIdeInfMunCarrega[1];
            TMDFeInfMDFeIdeInfMunCarrega infMunCarrega0= new TMDFeInfMDFeIdeInfMunCarrega
            {
                cMunCarrega = NFeRecebida.NFe.infNFe.emit.enderEmit.cMun,
                xMunCarrega = NFeRecebida.NFe.infNFe.emit.enderEmit.xMun,
            };
            infMunCarregaArray[0] = infMunCarrega0;

            MDFe.infMDFe.ide = new TMDFeInfMDFeIde
            {
                cUF = NSFacilita_Transporte.src.Classes.MDFe.TCodUfIBGE.Item43,
                tpAmb = NSFacilita_Transporte.src.Classes.MDFe.TAmb.Homologacao,
                tpEmit = TEmit.Item2,
                tpTransp = TTransp.Item1,
                mod = TModMD.Item58,
                serie = "1",
                nMDF = "11574",
                cMDF = "",
                cDV = "",
                modal = TModalMD.Item1,
                dhEmi = DateTime.Now.ToString("s") + "-03:00",
                tpEmis = TMDFeInfMDFeIdeTpEmis.Item1,
                procEmi = TMDFeInfMDFeIdeProcEmi.Item0,
                verProc = "5.7",
                UFIni = (NSFacilita_Transporte.src.Classes.MDFe.TUf)NFeRecebida.NFe.infNFe.emit.enderEmit.UF,
                UFFim = (NSFacilita_Transporte.src.Classes.MDFe.TUf)NFeRecebida.NFe.infNFe.dest.enderDest.UF,
                infMunCarrega = infMunCarregaArray,
                dhIniViagem = DateTime.Now.ToString("s") + "-03:00",
            };

            MDFe.infMDFe.emit = new TMDFeInfMDFeEmit 
            { 
                ItemElementName = NSFacilita_Transporte.src.Classes.MDFe.ItemChoiceType.CNPJ,
                Item = "07364617000135",
                xNome = "EMISSAO TESTE MDFE",
                xFant = "NS TECNOLOGIA",
                enderEmit = new NSFacilita_Transporte.src.Classes.MDFe.TEndeEmi
                {
                    xLgr = "AV ANTONIO DURO",
                    nro = "777",
                    xCpl = "SALA 01",
                    xBairro = "OLARIA",
                    cMun = "4303509",
                    xMun = "CAMAQUA",
                    CEP = "87265000",
                    UF = NSFacilita_Transporte.src.Classes.MDFe.TUf.RS,
                    fone = "513692112",
                    email = "fernando.konflanz@nstecnologia.com.br"
                }
            };

            TMDFeInfMDFeInfMunDescargaInfNFe[] MDFeInfNFeArray = new TMDFeInfMDFeInfMunDescargaInfNFe[1];
            TMDFeInfMDFeInfMunDescargaInfNFe MDFeInfNFe = new TMDFeInfMDFeInfMunDescargaInfNFe
            {
                chNFe = NFeRecebida.NFe.infNFe.Id,
            };

            //TMDFeInfMDFeInfMunDescargaInfCTe[] MDFeInfCTeArray = new TMDFeInfMDFeInfMunDescargaInfCTe[1];
            //TMDFeInfMDFeInfMunDescargaInfCTe MDFeInfCTe = new TMDFeInfMDFeInfMunDescargaInfCTe
            //{
            //    // chCTe = chCTeAutorizado // caso seja feito 1 metodo que emite os dois documentos
            //};


            TMDFeInfMDFeInfMunDescarga[] infMunDescargaArray = new TMDFeInfMDFeInfMunDescarga[1];
            TMDFeInfMDFeInfMunDescarga infMunDescarga1 = new TMDFeInfMDFeInfMunDescarga
            {
                cMunDescarga = NFeRecebida.NFe.infNFe.dest.enderDest.cMun,
                xMunDescarga = NFeRecebida.NFe.infNFe.dest.enderDest.xMun,
                infNFe = MDFeInfNFeArray,
                //infCTe = MDFeInfCTeArray,
            };

            MDFe.infMDFe.prodPred = new TMDFeInfMDFeProdPred
            {
                tpCarga = TMDFeInfMDFeProdPredTpCarga.Item05,
                xProd = NFeRecebida.NFe.infNFe.det[0].prod.xProd,
                cEAN = NFeRecebida.NFe.infNFe.det[0].prod.cEAN,
                NCM = NFeRecebida.NFe.infNFe.det[0].prod.NCM,
            };

            MDFe.infMDFe.tot = new TMDFeInfMDFeTot
            {
                //qCTe = MDFeInfCTeArray.Length > 0 ? MDFeInfCTeArray.Length.ToString() : "0",
                qNFe = MDFeInfNFeArray.Length > 0 ? MDFeInfNFeArray.Length.ToString() : "0",
                vCarga = NFeRecebida.NFe.infNFe.total.ICMSTot.vNF,
                cUnid = TMDFeInfMDFeTotCUnid.Item01,
                qCarga = "13000.000",
            };

            MDFe.infMDFe.infAdic = new TMDFeInfMDFeInfAdic
            {
                infCpl = "TESTE DE EMISSAO PARA FINS DE DESENVOLVIMENTO",
            };

            rodoVeicTracaoCondutor[] arrayCondutor = new rodoVeicTracaoCondutor[1];
            rodoVeicTracaoCondutor Condutor0 = new rodoVeicTracaoCondutor
            {
                xNome = "JOAO CARRETEIRO",
                CPF = "16904541059"
            };

            arrayCondutor[0] = Condutor0;

            rodoInfANTTInfContratante[] arrayInfContratante = new rodoInfANTTInfContratante[1];
            rodoInfANTTInfContratante infContratante0 = new rodoInfANTTInfContratante
            {
                xNome = "Contratante da Silva",
                Item = "07364617000135",
                ItemElementName = NSFacilita_Transporte.src.Classes.MDFe.ItemChoiceType21.CNPJ,
            };

            arrayInfContratante[0] = infContratante0;

            MDFe.infMDFe.infModal = new TMDFeInfMDFeInfModal
            {
                versaoModal = "3.00",
                rodo = new TMDFeRodo
                {
                    infANTT = new rodoInfANTT
                    {
                        RNTRC = "12345678",
                        infContratante = arrayInfContratante
                    },
                    veicTracao = new rodoVeicTracao
                    {
                        cInt = "1",
                        placa = "IHF4183",
                        RENAVAM = "87408206662",
                        tara = "8500",
                        capKG = "25000",
                        prop = new rodoVeicTracaoProp
                        {
                            Item = "16904541059",
                            ItemElementName = NSFacilita_Transporte.src.Classes.MDFe.ItemChoiceType4.CNPJ,
                            RNTRC = "12345678",
                            xNome = "JOAOZINHO CARRETEIRO",
                            IE = "ISENTO",
                            UF = NSFacilita_Transporte.src.Classes.MDFe.TUf.RS,
                            tpProp = rodoVeicTracaoPropTpProp.Item0
                        },
                        condutor = arrayCondutor,
                        tpCar = rodoVeicTracaoTpCar.Item00,
                        tpRod = rodoVeicTracaoTpRod.Item06,
                        UF = NSFacilita_Transporte.src.Classes.MDFe.TUf.RS,
                    }
                }
            };

            string MDFeJSON = JsonConvert.SerializeObject(MDFe);
            string MDFeXML = mdfeToXML(MDFe); //String do mdfe serializado
            respostaEmissaoMDFe = NSSuite.emitirMDFeSincrono(MDFeXML, "xml", "07364617000135", "XP", "2", "C:/documentosFacilitaTransporte", false, false);
            return respostaEmissaoMDFe;

        }
    }
}