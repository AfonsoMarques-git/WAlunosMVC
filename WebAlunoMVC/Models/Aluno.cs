using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebAlunoMVC.Models
{
    public class Aluno
    {
        [Required]
        [Display(Name = "Numero de Aluno")]
        public int NAluno { get; set; }

        [Required]
        [Display(Name = "Primeiro Nome")]
        public string PrimeiroNome { get; set; }

        [Required]
        [Display(Name = "Último Nome")]
        public string UltimoNome { get; set; }

        [Required]
        public string Morada { get; set; }

        [Required]
        public Sexo Sexo { get; set; }

        [Required]
        [Display(Name = "Data de nascimento")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DataNascimento { get; set; }

        [Required]
        [Range(1, 12)]
        [Display(Name = "Ano de escolaridade")]
        public int AnoEscolaridade { get; set; }

        [Display(Name = "Imagem")]
        public string ImagePath { get; set; }
        public HttpPostedFileBase Imagem { get; set; }
    }
}