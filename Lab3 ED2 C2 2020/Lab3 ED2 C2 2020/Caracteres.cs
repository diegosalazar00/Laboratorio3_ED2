using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3_ED2_C2_2020
{
    public class Caracteres : IComparable
    {
        public char caracter;
        public int CantidaRepetido;
        public double Probabilidad;
        public Caracteres derecha;
        public Caracteres izquierda;
        public bool padre;
        public string Codigo;

        public Caracteres(char caract, int repeticion , bool espadre)
        {
            caracter = caract;
            CantidaRepetido = repeticion;
            padre = espadre;
            Codigo = string.Empty;
        }

        public int CompareTo(object obj)
        {
            var comparer = (Caracteres)obj;
            return Probabilidad.CompareTo(comparer.Probabilidad);
        }
        public static Comparison<Caracteres> ordenarporprobabilidad = 
            delegate (Caracteres caracter1, Caracteres caracter2)
        { return caracter1.CompareTo(caracter2); };

        public void CalcularProbabilidad(int totalcaracteres)
        {
            Probabilidad = (float)CantidaRepetido / totalcaracteres;
        }
    }
}
