using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace Lab3_ED2_C2_2020
{
    class Huffman
    {
        public string rutaabsserver;
        public string rutaabsarchivooriginal;
        public string nombrearchivooriginal;
        public string nombrearchivooperado;

        private int cantidadcaracteres;
        private List<Caracteres> listacaracteres;
        private Dictionary<char, int> dicapariciones;
        private Dictionary<char, string> dicprefijos;

        private byte[] Bufferlectura;
        private byte[] Bufferescritura;
        private char[] Bufferescrituradescompresion;
        private const int largobuffer = 100;
        public Huffman()
        {
            rutaabsserver = string.Empty;
            rutaabsarchivooriginal = string.Empty;
            nombrearchivooperado = string.Empty;
            nombrearchivooriginal = string.Empty;

            cantidadcaracteres = 0;
            listacaracteres = new List<Caracteres>();
            dicapariciones = new Dictionary<char, int>();
            dicprefijos = new Dictionary<char, string>();
            Bufferlectura = new byte[largobuffer];
            Bufferescritura = new byte[largobuffer];
            Bufferescrituradescompresion = new char[largobuffer];
        }
        //Compresión 
        private void Obtenertablaapariciones()
        {
            using(var file=new FileStream(rutaabsarchivooriginal, FileMode.Open, FileAccess.Read))
            {
                using (var reader=new BinaryReader(file))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        Bufferlectura = reader.ReadBytes(largobuffer);
                        var chars = Encoding.UTF8.GetChars(Bufferlectura);

                        foreach (var caracter in chars)
                        {
                            Predicate<Caracteres> buscadorcaracteres = delegate (Caracteres caracteres)
                               {
                                   return caracteres.caracter == caracter;
                               };

                            if (listacaracteres.Find(buscadorcaracteres)==null)
                            {
                                var caracteres = new Caracteres(caracter, 0, false);
                                caracteres.CantidaRepetido++;
                                listacaracteres.Add(caracteres);
                            }
                            else
                            {
                                listacaracteres.Find(buscadorcaracteres).CantidaRepetido++;
                            }
                            cantidadcaracteres++;
                        }
                    }
                }
            }
            foreach (var caracteres in listacaracteres)
            {
                caracteres.CalcularProbabilidad(cantidadcaracteres);
            }
            listacaracteres.Sort(Caracteres.ordenarporprobabilidad);

            foreach (var caracteres in listacaracteres)
            {
                dicapariciones.Add(caracteres.caracter, caracteres.CantidaRepetido);
            }

            var linea = "";
            foreach(var caracter in dicapariciones)
            {
                if (caracter.Key=='\n')
                {
                    linea += "/n" + caracter.Value + "|";
                }
                else if (caracter.Key=='\t')
                {
                    linea += "/t" + caracter.Value + "|";

                }
                else if (caracter.Key=='\r')
                {
                    linea += "/r" + caracter.Value + "|";
                }
                else if(caracter.Key==' ')
                {
                    linea += "esp" + caracter.Value + "|";
                }
                else
                {
                    linea += caracter.Key + " " + caracter.Value + "|";
                }
            }
            linea = linea.Remove(linea.Length - 1);
            linea += Environment.NewLine;

            var buffertabla = Encoding.UTF8.GetBytes(linea);
            var tabla = Encoding.UTF8.GetChars(buffertabla);
            using(var file=new FileStream(rutaabsserver+nombrearchivooperado,FileMode.Create))
            {
                using(var writer=new BinaryWriter(file))
                {
                    writer.Write(tabla);
                }
            }
        }
        private void recorrido(Caracteres nodo,string prefijos)
        {
            if (nodo != null)
            {
                if (nodo.Codigo!="")
                {
                    prefijos += nodo.Codigo;
                    if (nodo.padre==false)
                    {
                        dicprefijos[nodo.caracter] = prefijos;
                        prefijos = "";
                    }
                    recorrido(nodo.izquierda, prefijos);
                    recorrido(nodo.derecha, prefijos);
                }
                else
                {
                    recorrido(nodo.izquierda, prefijos);
                    recorrido(nodo.derecha, prefijos);
                }
            }
        }
        private void generarcodigo()
        {
            foreach (var caracteres in listacaracteres)
            {
                if (!dicprefijos.ContainsKey(caracteres.caracter))
                {
                    dicprefijos.Add(caracteres.caracter, "");
                }
            }
            while (listacaracteres.Count!=1)
            {
                var nodoizq = listacaracteres[0];
                listacaracteres[0] = null;
                listacaracteres.RemoveAt(0);
                nodoizq.Codigo += "0";

                var nododer = listacaracteres[0];
                listacaracteres[0] = null;
                listacaracteres.RemoveAt(0);
                nododer.Codigo += "1";

                var nodopadre = new Caracteres(' ', 0, true);
                nodopadre.derecha = nododer;
                nodopadre.izquierda = nodoizq;
                nodopadre.Probabilidad = nodopadre.derecha.Probabilidad + nodopadre.izquierda.Probabilidad;
                listacaracteres.Add(nodopadre);
                listacaracteres.Sort(Caracteres.ordenarporprobabilidad);
            }
            var codigoprefijo = "";
            recorrido(listacaracteres[0], codigoprefijo);
            listacaracteres.RemoveAt(0);
            Bufferlectura = new byte[largobuffer];
        }
        public void comprimir()
        {
            nombrearchivooperado = nombrearchivooriginal.Split('.')[0] + ".huff";
            Obtenertablaapariciones();
            generarcodigo();
            Bufferlectura = new byte[largobuffer];
            var lineaprefijos = ""
;           var escribir = 0;
            using (var file=new FileStream(rutaabsarchivooriginal,FileMode.Open))
            {
                using(var reader=new BinaryReader(file))
                {
                    while(reader.BaseStream.Position!=reader.BaseStream.Length)
                    {
                        Bufferlectura = reader.ReadBytes(largobuffer);
                        var chars = Encoding.UTF8.GetChars(Bufferlectura);
                        foreach (var caracter in chars)
                        {
                            if (dicprefijos.ContainsKey(caracter))
                            {
                                lineaprefijos += dicprefijos[caracter];
                            }
                            else
                            {
                                throw new Exception("No se ha encontrado el caracter leído en la tabla de apariciones");
                            }
                        }
                        while (lineaprefijos.Length>=8)
                        {
                            var ochobits = lineaprefijos.Substring(0, 8);
                            lineaprefijos = lineaprefijos.Remove(0, 8);
                            Bufferescritura[escribir] = (byte)Convert.ToInt32(ochobits, 2);
                            escribir++;
                        }
                        if (reader.BaseStream.Position==reader.BaseStream.Length)
                        {
                            if (lineaprefijos.Length>0)
                            {
                                lineaprefijos = lineaprefijos.PadRight(8, '0');
                                Bufferescritura[escribir] = (byte)Convert.ToInt32(lineaprefijos, 2);
                                escribir++;
                            }
                        }
                        escribe(escribir);
                        escribir = 0;
                    }

                }
            }
            File.Delete(rutaabsarchivooriginal);

        }
        private void Escribir(int escribirhasta)
        {
            using (var file = new FileStream(rutaabsserver + nombrearchivooperado, FileMode.Append, FileAccess.Write))
            {
                using (var writer=new BinaryWriter(file))
                {
                    writer.Write(Bufferescritura, 0, escribirhasta);
                }
            }
            Bufferescritura = new byte[largobuffer];
        }

        private void escribe(int escribir)
        {
            using(var file=new FileStream(rutaabsserver+nombrearchivooperado,FileMode.Append,FileAccess.Write))
            {
                using (var writer=new BinaryWriter(file))
                {
                    writer.Write(Bufferescrituradescompresion, 0, escribir);
                }
            }
            Bufferescrituradescompresion = new char[largobuffer];
        }
        //Descompresion
        private int Tabla()
        {
            Bufferlectura = new byte[largobuffer];
            var leerhasta = 0;
            var lineatabla = "";

            using (var file=new FileStream(rutaabsarchivooriginal,FileMode.Open))
            {
                using (var reader =new BinaryReader(file))
                {
                    while(reader.BaseStream.Position!=reader.BaseStream.Length)
                    {
                        Bufferlectura = reader.ReadBytes(largobuffer);
                        for (int i = 0; i < Bufferlectura.Length; i++)
                        {
                            leerhasta++;
                            if (Bufferlectura[i]==13)
                            {
                                reader.BaseStream.Position = reader.BaseStream.Length;
                                i = largobuffer;
                            }
                        }
                    }
                    reader.BaseStream.Position = 0;
                    lineatabla = Encoding.UTF8.GetString(reader.ReadBytes(leerhasta - 1));
                }
            }
            var items = lineatabla.Split('|');

            foreach (var caracteres in items)
            {
                char caracter = ' ';
                if (caracteres!=""&&caracteres!=" ")
                {
                    var repeticion = int.Parse(caracteres.Split(' ')[1]);
                    if (caracteres.Split(' ')[0]=="/r")
                    {
                        caracter = '\r';
                    }
                    else if (caracteres.Split(' ')[0]=="esp")
                    {
                        caracter = ' ';
                    }
                    else if (caracteres.Split(' ')[0]=="/t")
                    {
                        caracter = '\t';
                    }
                    else if (caracteres.Split(' ')[0]=="/n")
                    {
                        caracter = '\n';
                    }
                    else
                    {
                        caracter = Convert.ToChar(caracteres.Split(' ')[0]);
                    }
                    cantidadcaracteres += repeticion;
                    var _caracteres = new Caracteres(caracter, repeticion, false);
                    listacaracteres.Add(_caracteres);
                }
            }
            foreach (var caracteres in listacaracteres)
            {
                caracteres.CalcularProbabilidad(cantidadcaracteres);
            }
            return leerhasta;
        }
        public void Descomprimir()
        {
            nombrearchivooperado = nombrearchivooriginal.Split('.')[0] + ".txt";
            var leerdesde = Tabla() + 1;
            generarcodigo();

            Bufferlectura = new byte[largobuffer];

            var lineaenbits = "";
            var lineaauxbits = "";
            var bitscaracter = "";
            var borrarposicion = 0;
            var escribiren = 0;
            using(var file=new FileStream(rutaabsarchivooriginal,FileMode.Open))
            {
                using(var reader=new BinaryReader(file))
                {
                    while (reader.BaseStream.Position!=reader.BaseStream.Length)
                    {
                        Bufferlectura = reader.ReadBytes(largobuffer);
                        foreach (var caracter in Bufferlectura)
                        {
                            var binario = Convert.ToString(caracter, 2);
                            lineaenbits+= binario.PadLeft(8, '0');
                        }
                        var continuar = false;
                        bitscaracter = "";
                        while (lineaenbits.Length>0&&continuar==false)
                        {
                            lineaauxbits = lineaenbits;
                            while (!dicprefijos.ContainsValue(bitscaracter)&&lineaauxbits!="")
                            {
                                bitscaracter += lineaauxbits.Substring(0, 1);
                                lineaauxbits = lineaauxbits.Remove(0, 1);
                                borrarposicion++;
                                if (lineaauxbits==""&&!dicprefijos.ContainsValue(bitscaracter))
                                {
                                    borrarposicion = 0;
                                    continuar = true;
                                }

                            }

                            lineaenbits = lineaenbits.Remove(0, borrarposicion);
                            borrarposicion = 0;
                            char key = '\0';
                            foreach (var item in dicprefijos)
                            {
                                if (item.Value==bitscaracter)
                                {
                                    key = item.Key;
                                }
                            }

                            if (escribiren<largobuffer)
                            {
                                if (key!='\0')
                                {
                                    Bufferescrituradescompresion[escribiren] = key;
                                    bitscaracter = "";
                                    escribiren++;
                                }
                            }
                            else
                            {
                                if (key!='\0')
                                {
                                    escribe(escribiren);
                                    escribiren = 0;
                                    Bufferescrituradescompresion[escribiren] = key;
                                    bitscaracter = "";
                                    escribiren++;
                                }
                            }
                        }
                        escribe(escribiren);
                        escribiren = 0;
                    }
                    
                }

            }
            File.Delete(rutaabsarchivooriginal);
        }

    }
}
