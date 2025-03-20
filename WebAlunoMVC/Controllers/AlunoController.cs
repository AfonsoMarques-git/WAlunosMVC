using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using WebAlunoMVC.Models;

namespace WebAlunoMVC.Controllers
{
    public class AlunoController : Controller
    {
        public ActionResult ListaAluno()
        {
            try
            {
                if (Session["Login"] != null)
                {
                    ConexaoBD conn = new ConexaoBD("localhost", 3306, "root", "", "formacao");
                    List<Aluno> lista = new List<Aluno>();
                    using (MySqlConnection conexao = conn.ObterConexao())
                    {
                        if (conexao != null)
                        {
                            using (MySqlCommand cmd = new MySqlCommand("Select * from alunos", conexao))
                            {
                                using (MySqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        lista.Add(new Aluno()
                                        {
                                            NAluno = reader.GetInt32("id_aluno"),
                                            PrimeiroNome = reader.GetString("primeiro_nome"),
                                            UltimoNome = reader.GetString("ultimo_nome"),
                                            Morada = reader.GetString("morada"),
                                            Sexo = reader.GetString("sexo") == "Masculino" ? Sexo.Feminino : Sexo.Feminino,
                                            DataNascimento = reader.GetDateTime("data_de_nascimento"),
                                            AnoEscolaridade = reader.GetInt16("ano_de_escolaridade"),
                                        });
                                    }
                                }
                            }
                        }
                    }
                    return View(lista);
                }
                else { 
                    return RedirectToAction("Login", "Registo");
                }
            }
            catch (Exception ex) {
                return View("Erro", new HandleErrorInfo(ex, "Aluno", "ListaAluno"));
            }
        }

        // Get: Aluno
        public ActionResult CriaAluno()
        {
            try
            {
                if (Session["Login"] == null) return RedirectToAction("Login", "Registo");
                return View();
            }
            catch (Exception ex)
            {
                return View("Erro", new HandleErrorInfo(ex, "Aluno", "CriaAluno"));
            }
        }

        [HttpPost]
        public ActionResult CriaAluno(Aluno aluno)
        {
            try
            {
                if (Session["Login"] == null) return RedirectToAction("Login", "Registo");

                // Verificar se existe algum erro coma  submissão do formulário
                if (ModelState.IsValid)
                {
                    string ImagemNome = Path.GetFileNameWithoutExtension(aluno.Imagem.FileName);
                    string ImagemExt = Path.GetExtension(aluno.Imagem.FileName);
                    ImagemNome = DateTime.Now.ToString("yyyyMMddHHmmss") + " - " + ImagemNome.Trim() + ImagemExt;
                    aluno.ImagePath = @"\Content\Imagens" + ImagemNome;
                    aluno.Imagem.SaveAs(ControllerContext.HttpContext.Server.MapPath(aluno.ImagePath));
                    // Password do mysql é Admin
                    ConexaoBD conn = new ConexaoBD("localhost", 3306, "root", "", "formacao");

                    using (MySqlConnection conexao = conn.ObterConexao())
                    {
                        if (conexao != null)
                        {
                            string stm = ("insert into aluno values(0, @primeiroNome, @ultimoNome, @morada, " + "@sexo, @dataNascimentom, @ano, @foto)");
                            using (MySqlCommand cmd = new MySqlCommand(stm, conexao))
                            {
                                cmd.Parameters.AddWithValue("@primeiroNome", aluno.PrimeiroNome);
                                cmd.Parameters.AddWithValue("@ultimoNome", aluno.UltimoNome);
                                cmd.Parameters.AddWithValue("@morada", aluno.Morada);
                                cmd.Parameters.AddWithValue("@sexo", aluno.Sexo);
                                cmd.Parameters.AddWithValue("@dataNascimento", aluno.DataNascimento);
                                cmd.Parameters.AddWithValue("@ano", aluno.AnoEscolaridade);
                                cmd.Parameters.AddWithValue("@foto", aluno.ImagePath);

                                int mRegistos = cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
                return RedirectToAction("ListaAluno");
            }
            catch (Exception ex)
            {
                return View("Erro", new HandleErrorInfo(ex, "Aluno", "CriaAluno"));
            }
        }

        public ActionResult DetalheAluno(int? id)
        {
            try
            {
                if (Session["Login"] == null) return RedirectToAction("Login", "Registo");

                ConexaoBD conn = new ConexaoBD("localhost", 3306, "root", "", "formacao");
                Aluno aluno = null;

                using (MySqlConnection conexao = conn.ObterConexao())
                {
                    if (conexao != null)
                    {
                        using (MySqlCommand cmd = new MySqlCommand("Select * from alunos where " + "id_aluno=@idaluno", conexao))
                        {
                            cmd.Parameters.AddWithValue("@idaluno", id);
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    aluno = new Aluno()
                                    {
                                        NAluno = reader.GetInt32("id_aluno"),
                                        PrimeiroNome = reader.GetString("primeiro_nome"),
                                        UltimoNome = reader.GetString("ultimo_nome"),
                                        Morada = reader.GetString("morada"),
                                        Sexo = reader.GetString("sexo") == "Masculino" ? Sexo.Masculino : Sexo.Feminino,
                                        DataNascimento = reader.GetDateTime("data_de_nascimento"),
                                        AnoEscolaridade = reader.GetInt16("ano_de_escolaridade"),
                                        ImagePath = reader.GetString("foto")
                                    };

                                    return View(aluno);
                                }
                            }
                        }
                    }
                }

                return RedirectToAction("ListaAluno");
            }
            catch (Exception ex)
            {
                return View("Erro", new HandleErrorInfo(ex, "Aluno", "DetalheAluno"));
            }
        }

        public ActionResult EditaAluno(int? id)
        {
            try
            {
                if (Session["Login"] == null) return RedirectToAction("Login", "Registo");


                ConexaoBD conn = new ConexaoBD("localhost", 3306, "root", "", "formacao");
                Aluno aluno = null;

                using (MySqlConnection conexao = conn.ObterConexao())
                {
                    if (conexao != null)
                    {

                        using (MySqlCommand cmd = new MySqlCommand("Select * from alunos where id_aluno=@idaluno", conexao))
                        {
                            cmd.Parameters.AddWithValue("@idaluno", id);
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    aluno = new Aluno()
                                    {
                                        NAluno = reader.GetInt32("id_aluno"),
                                        PrimeiroNome = reader.GetString("primeiro_nome"),
                                        UltimoNome = reader.GetString("ultimo_nome"),
                                        Morada = reader.GetString("morada"),
                                        Sexo = reader.GetString("sexo") == "Masculino" ? Sexo.Masculino : Sexo.Feminino,
                                        DataNascimento = reader.GetDateTime("data_de_nascimento"),
                                        AnoEscolaridade = reader.GetInt16("ano_de_escolaridade"),
                                        ImagePath = reader.GetString("foto")
                                    };

                                    return View(aluno);
                                }
                            }
                        }
                    }
                }
                return RedirectToAction("ListaAluno");

            }
            catch (Exception ex)
            {
                return View("Erro", new HandleErrorInfo(ex, "Aluno", "EditaAluno"));
            }
        }

        [HttpPost]
        public ActionResult EditaAluno(Aluno aluno)
        {
            try
            {
                if (Session["Login"] == null) return RedirectToAction("Login", "Registo");

                //Variável booleana da imagem inicializada a false
                bool img = false;
                //Verificar se o utilizador atuali
                if (aluno.Imagem != null)
                {
                    string ImagemNome = Path.GetFileNameWithoutExtension(aluno.Imagem.FileName);
                    string ImagemExt = Path.GetExtension(aluno.Imagem.FileName);
                    ImagemNome = DateTime.Now.ToString("yyyyMMddHHmmss") + " - " + ImagemNome.Trim() + ImagemExt;
                    aluno.ImagePath = @"\Content\Imagens" + ImagemNome;
                    aluno.Imagem.SaveAs(ControllerContext.HttpContext.Server.MapPath(aluno.ImagePath));
                    img = true;
                }
                //Password do mysql é Admin
                ConexaoBD conn = new ConexaoBD("localhost", 3306, "root", "", "formacao");

                using (MySqlConnection conexao = conn.ObterConexao())
                {
                    if (conexao != null)
                    {
                        string strFoto = (img) ? ",foto=@foto" : "";
                        string stm = "update alunos set primeiro_nome=@primeiroNome, " +
                            "ultimo_nome=@ultimoNome, " +
                            "morada=@morada, " +
                            "sexo=@sexo, " +
                            "data_de_nascimento=@dataNascimento, " +
                            "ano_de_escolaridade=@ano " +
                            strFoto + " where id_aluno=@idaluno";
                        using (MySqlCommand cmd = new MySqlCommand(stm, conexao))
                        {
                            cmd.Parameters.AddWithValue("@idaluno", aluno.NAluno);
                            cmd.Parameters.AddWithValue("@primeiroNome", aluno.PrimeiroNome);
                            cmd.Parameters.AddWithValue("@ultimoNome", aluno.UltimoNome);
                            cmd.Parameters.AddWithValue("@morada", aluno.Morada);
                            cmd.Parameters.AddWithValue("@sexo", aluno.Sexo);
                            cmd.Parameters.AddWithValue("@dataNascimento", aluno.DataNascimento);
                            cmd.Parameters.AddWithValue("@ano", aluno.AnoEscolaridade);
                            if (img)
                                cmd.Parameters.AddWithValue("@foto", aluno.ImagePath);

                            int nRegistos = cmd.ExecuteNonQuery();
                        }
                    }
                }

                return RedirectToAction("ListaAluno");

            }
            catch (Exception ex)
            {
                return View("Erro", new HandleErrorInfo(ex, "Aluno", "EditaAluno"));
            }
        }

        public ActionResult EliminaAluno(int? id)
        {
            try
            {
                if (Session["Login"] == null) return RedirectToAction("Login", "Registo");

                ConexaoBD conn = new ConexaoBD("localhost", 3306, "root", "", "formacao");
                Aluno aluno = null;

                using (MySqlConnection conexao = conn.ObterConexao())
                {
                    if (conexao != null)
                    {

                        using (MySqlCommand cmd = new MySqlCommand("Select * from alunos where id_aluno=@idaluno", conexao))
                        {
                            cmd.Parameters.AddWithValue("@idaluno", id);
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    aluno = new Aluno()
                                    {
                                        NAluno = reader.GetInt32("id_aluno"),
                                        PrimeiroNome = reader.GetString("primeiro_nome"),
                                        UltimoNome = reader.GetString("ultimo_nome"),
                                        Morada = reader.GetString("morada"),
                                        Sexo = reader.GetString("sexo") == "Masculino" ? Sexo.Masculino : Sexo.Feminino,
                                        DataNascimento = reader.GetDateTime("data_de_nascimento"),
                                        AnoEscolaridade = reader.GetInt16("ano_de_escolaridade"),
                                        ImagePath = reader.GetString("foto")
                                    };

                                    TempData["ImagemPath"] = aluno.ImagePath;

                                    return View(aluno);
                                }
                            }
                        }
                    }
                }
                return RedirectToAction("ListaAluno");
            }
            catch (Exception ex)
            {
                return View("Erro", new HandleErrorInfo(ex, "Aluno", "EliminaAluno"));
            }
        }

        [HttpPost, ActionName("EliminaAluno")]
        public ActionResult EliminaAlunoConfirmacao(int? id)
        {
            try
            {
                if (Session["Login"] == null) return RedirectToAction("Login", "Registo");

                ConexaoBD conn = new ConexaoBD("localhost", 3306, "root", "", "formacao");

                using (MySqlConnection conexao = conn.ObterConexao())
                {
                    if (conexao != null)
                    {

                        string stm = "delete from alunos where id_aluno=@idaluno";
                        using (MySqlCommand cmd = new MySqlCommand(stm, conexao))
                        {
                            cmd.Parameters.AddWithValue("@idaluno", id);

                            int nRegistos = cmd.ExecuteNonQuery();
                            if (nRegistos == 1)
                            {
                                new FileInfo(ControllerContext.HttpContext.Server.MapPath(TempData["ImagemPath"].ToString())).Delete();
                            }
                        }
                    }
                }
                return RedirectToAction("ListaAluno");
            }
            catch (Exception ex)
            {
                return View("Erro", new HandleErrorInfo(ex, "Aluno", "EliminaAluno"));
            }
        }
    }
}