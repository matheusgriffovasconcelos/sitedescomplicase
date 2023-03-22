using Auth.Data;
using Auth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Auth.Controllers;


[Authorize(Roles = "admin")]

public class OrcamentoController : Controller
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;

    public OrcamentoController(AppDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    [AllowAnonymous]
    public IActionResult Index()
    {
        var orcamentos = _db.Orcamentos
            .Include(p => p.Fornecedor)
            .Include(p => p.Usuario)
            .AsNoTracking()
            .ToList();
        return View(orcamentos);
    }

    private void CarregarFornecedores(int? idFornecedor = null)
    {
        var fornecedores = _db.Fornecedores.OrderBy(c => c.Nome).ToList();
        ViewBag.Fornecedores = fornecedores;
    }

    private void CarregarCategorias(int? idCategoria = null)
    {
        var categorias = _db.Categorias.OrderBy(c => c.Nome).ToList();
        ViewBag.Categorias = categorias;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Cadastrar(int id)
    {
        var usuario = _db.Usuarios.Find(id);
        CarregarFornecedores();
        CarregarCategorias();
        var orcamento = new OrcamentoModel();
        return View(orcamento);
    }

    [AllowAnonymous]
    [HttpPost]
    public IActionResult Cadastrar(int id, OrcamentoModel orcamento)
    {
        if (!ModelState.IsValid)
        {
            CarregarFornecedores(orcamento.idFornecedor);
            CarregarCategorias(orcamento.Fornecedor.IdCategoria);
            return View(orcamento);
        }
        _db.Orcamentos.Add(orcamento);
        _db.SaveChanges();
        return RedirectToAction("Index");
    }
}