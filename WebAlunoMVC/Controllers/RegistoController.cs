using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAlunoMVC.Models;

namespace WebAlunosMVC.Controllers
{
    public class RegistoController : Controller
    {
        // GET: Registo
        public ActionResult Registo()
        {
            return View();
        }

        //Post
        [HttpPost]
        public ActionResult Registo(Utilizador utilizador)
        {
            if (ModelState.IsValid)
            {
                ConexaoBD conn = new ConexaoBD("localhost", 3306, "root", "", "formacao");
                using (MySqlConnection conexao = conn.ObterConexao())
                {
                    if (conexao != null)
                    {
                        string stm = "insert into utilizadores () values(0,@email,MD5(@password))";
                        using (MySqlCommand cmd = new MySqlCommand(stm, conexao))
                        {
                            cmd.Parameters.AddWithValue("@email", utilizador.Email);
                            cmd.Parameters.AddWithValue("@password", utilizador.Password);

                            int nregistos = cmd.ExecuteNonQuery();

                            if (nregistos == 1)
                                return RedirectToAction("Login");
                        }
                    }
                }
            }
            return RedirectToAction("Registo");
        }
        //GET
        public ActionResult Login()
        {
            return View();
        }

        //Post
        [HttpPost]
        public ActionResult Login(Utilizador utilizador)
        {
            if (ModelState.IsValid)
            {
                ConexaoBD conn = new ConexaoBD("localhost", 3306, "root", "", "formacao");
                using (MySqlConnection conexao = conn.ObterConexao())
                {
                    if (conexao != null)
                    {
                        string stm = "select * from utilizadores  where email=@email and password=MD5(@password)";
                        using (MySqlCommand cmd = new MySqlCommand(stm, conexao))
                        {
                            cmd.Parameters.AddWithValue("@email", utilizador.Email);
                            cmd.Parameters.AddWithValue("@password", utilizador.Password);

                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    Session["login"] = 2;
                                    Session["email"] = utilizador.Email;

                                    return RedirectToAction("ListaAluno", "Aluno");
                                }
                            }

                        }
                    }
                }
            }
            return RedirectToAction("Login", "Registo");
        }

        public ActionResult Logout()
        {
            if (Session["login"] != null)
            {
                Session.Abandon();
            }

            return RedirectToAction("Login", "Registo");
        }
    }
}