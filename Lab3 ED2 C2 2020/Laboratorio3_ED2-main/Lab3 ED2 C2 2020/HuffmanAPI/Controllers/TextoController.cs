using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Threading.Tasks;
using Lab3_ED2_C2_2020;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HuffmanAPI.Controllers
{
    [Route("api/compress")]
    [ApiController]
    public class TextoController : ControllerBase
    {
        [HttpPost]
        [Route("{name}")]
        public IComparable<string> Post(string name)
        {
            Huffman text = new Huffman();
            return text.nombrearchivooperado;
        }
    }
    [HttpPost("api/descompress")]
    
    
        
    
}
