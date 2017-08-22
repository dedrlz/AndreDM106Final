using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AndreDM106Final.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "O campo nome é obrigatório")]
        public string nome { get; set; }
        public string descricao { get; set; }
        public string cor { get; set; }
        [Required(ErrorMessage = "O campo modelo é obrigatório")]
        public string modelo { get; set; }
        [Required(ErrorMessage = "O campo codigo é obrigatório")]
        [StringLength(8, ErrorMessage = "O codigo deve ter no máximo 8 caracteres.")]
        public string codigo { get; set; }
        [Range(10, 999, ErrorMessage = "O preço deverá ser entre 10 e 999.")]
        public decimal preco { get; set; }
        [Range(1, 999, ErrorMessage = "O peso deverá ser entre 1 e 999.")]
        public decimal peso { get; set; }
        [Range(1, 99, ErrorMessage = "A altura deverá ser entre 1 e 99.")]
        public decimal altura { get; set; }
        [Range(1, 99, ErrorMessage = "A largura deverá ser entre 1 e 99.")]
        public decimal largura { get; set; }
        [Range(1, 99, ErrorMessage = "O comprimento deverá ser entre 1 e 99.")]
        public decimal comprimento { get; set; }
        [Range(1, 99, ErrorMessage = "O diametro deverá ser entre 1 e 99.")]
        public decimal diametro { get; set; }
        [StringLength(80, ErrorMessage ="A URL deve ter no máximo 80 caracteres.")]
        public string Url { get; set; }
    }
}