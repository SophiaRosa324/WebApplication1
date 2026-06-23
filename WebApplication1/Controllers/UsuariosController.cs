using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class UsuariosController : Controller
    {
        private LP2Entities1 db = new LP2Entities1();

        // GET: Usuarios
        public ActionResult Index()
        {
            var usuarios = db.Usuarios.ToList();
            return View(usuarios);
        }

        // GET: Usuarios/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuarios usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // GET: Usuarios/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "nome,email,senha,telefone")] Usuarios usuario)
        {
            if (ModelState.IsValid)
            {
                usuario.data_cadastro = DateTime.Now;
                db.Usuarios.Add(usuario);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuarios usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_usuario,nome,email,senha,telefone")] Usuarios usuario)
        {
            // Não exigir senha durante o Edit — remove validação para o campo senha
            ModelState.Remove("senha");

            if (ModelState.IsValid)
            {
                try
                {
                    var usuarioDb = db.Usuarios.Find(usuario.id_usuario);
                    if (usuarioDb == null)
                    {
                        return HttpNotFound();
                    }

                    // Atualiza apenas os campos editáveis (não tocar em data_cadastro)
                    usuarioDb.nome = usuario.nome;
                    usuarioDb.email = usuario.email;
                    usuarioDb.telefone = usuario.telefone;

                    // Se o usuário informou uma senha, atualiza; caso contrário, mantém a existente
                    if (!string.IsNullOrWhiteSpace(usuario.senha))
                    {
                        usuarioDb.senha = usuario.senha;
                    }

                    db.Entry(usuarioDb).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var eve in ex.EntityValidationErrors)
                    {
                        foreach (var ve in eve.ValidationErrors)
                        {
                            ModelState.AddModelError(ve.PropertyName, ve.ErrorMessage);
                        }
                    }

                    try
                    {
                        var fullMessage = string.Join(" | ",
                            ex.EntityValidationErrors.SelectMany(eve =>
                                eve.ValidationErrors.Select(ve => $"{ve.PropertyName}: {ve.ErrorMessage}")));
                        System.Diagnostics.Debug.WriteLine("DbEntityValidationException: " + fullMessage);
                    }
                    catch
                    {
                        // ignore
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    System.Diagnostics.Debug.WriteLine("Exception ao salvar: " + ex);
                }
            }

            // Em caso de erro de validação, recarrega o usuário do banco para evitar DateTime.MinValue em data_cadastro
            var usuarioOriginal = db.Usuarios.Find(usuario.id_usuario);
            if (usuarioOriginal != null)
            {
                // não enviar a senha para a view
                usuarioOriginal.senha = null;
                return View(usuarioOriginal);
            }

            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuarios usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Usuarios usuario = db.Usuarios.Find(id);
            db.Usuarios.Remove(usuario);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}