using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Lab3_ED2_C2_2020;

namespace HuffmanAPI.Models
{
    public class Archivo
    {
        Huffman huffman1 = new Huffman();
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
}
