using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using AndreDM106Final.Models;
using AndreDM106Final.br.com.correios.ws;
using AndreDM106Final.CRMClient;
using System.Globalization;

namespace AndreDM106Final.Controllers
{
    [Authorize]
    [RoutePrefix("api/Orders")]
    public class OrdersController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Orders
        [Authorize(Roles = "ADMIN")]
        public List<Order> GetOrders()
        {
            return db.Orders.Include(order => order.OrderItems).ToList();
        }

        // GET: api/Orders/5
        [Authorize]
        [ResponseType(typeof(Order))]
        public IHttpActionResult GetOrder(int id)
        {
            if (User.IsInRole("USER"))
            {
                String Usuario = User.Identity.Name;
                Order order = db.Orders.Where(e => e.userEmail == Usuario && e.Id == id).FirstOrDefault();

                if (order == null)
                {
                    return Ok("USER - Pedido não Encontrado");
                }
                return Ok(order);
            }
            Order orderAdmin = db.Orders.Find(id);

            if (orderAdmin == null)
            {
                return Ok("ADMIN - Pedido não Encontrado");
            }

            return Ok(orderAdmin);
        }

        [Authorize]
        [ResponseType(typeof(Order))]
        [HttpGet]
        [Route("byemail")]
        public IHttpActionResult GetOrdersByEmail(string email)
        {
            if ((User.IsInRole("ADMIN")) || (email == User.Identity.Name))
            {
                Order order = db.Orders.Where(p => p.userEmail == email).FirstOrDefault();
                if (order == null)
                {
                    return Ok("Pedidos não Encontrados para o email passado como input");
                }
                return Ok(db.Orders.Where(p => p.userEmail == email).ToList());
            }
            return Ok("Não autorizado. Acesso permitido somente ao usuário cadastrado com o email de entrada, ou Admins.");
        }

        // PUT: api/Orders/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutOrder(int id, Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != order.Id)
            {
                return BadRequest();
            }

            db.Entry(order).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Orders
        [Authorize]
        [ResponseType(typeof(Order))]
        public IHttpActionResult PostOrder(Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            order.statusPedido = "novo";
            order.userEmail = User.Identity.Name;
            order.pesoPedido = 0;
            order.precoFrete = 0;
            order.precoPedido = 0;
            order.dataPedido = DateTime.Now;

            db.Orders.Add(order);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = order.Id }, order);
        }

        [Authorize]
        [ResponseType(typeof(Order))]
        [HttpGet]
        [Route("fecharpedido")]
        public IHttpActionResult GetCloseOrder(int id)
        {
            Order order = db.Orders.Where(p => p.Id == id).FirstOrDefault();

            if (order == null)
                return Ok("Pedido não existe. Favor informar id válido.");
            if ((order.userEmail == User.Identity.Name) || (User.IsInRole("ADMIN")))
            {
                if (order.precoFrete == (decimal)0.00)
                    return Ok("ERRO - Valor do frete não calculado anteriormente!");

                order.statusPedido = "fechado";
                db.SaveChanges();
                return Ok("Pedido fechado com sucesso!");
            }
            else
            {
                return Ok("Acesso não autorizado.");
            }
        }

        // DELETE: api/Orders/5
        [Authorize]
        [ResponseType(typeof(Order))]
        public IHttpActionResult DeleteOrder(int id)
        {
            if (User.IsInRole("USER"))
            {
                String Usuario = User.Identity.Name;
                Order order = db.Orders.Where(e => e.userEmail == Usuario && e.Id == id).FirstOrDefault();

                if (order == null)
                {
                    return NotFound();
                }

                db.Orders.Remove(order);
                db.SaveChanges();

                return Ok(order);
            }

            Order orderAdmin = db.Orders.Find(id);

            if (orderAdmin == null)
            {
                return NotFound();
            }

            db.Orders.Remove(orderAdmin);
            db.SaveChanges();

            return Ok(orderAdmin);
        }

        [Authorize]
        [ResponseType(typeof(string))]
        [HttpGet]
        [Route("frete")]
        public IHttpActionResult GetFrete(int idPedido)
        {
            decimal pesototal = 0, alturaTotal = 0, largura = 0, comprimento = 0, diametro = 0, precoTotal = 0, valorFrete = 0;
            String CEPDestino, prazoEntrega;
            cResultado resultado;
            Order order = db.Orders.Where(p => p.Id == idPedido).FirstOrDefault(); 

            if (order == null)
                return Ok("O pedido não existe. Favor fornecer um ID válido.");

            if (order.OrderItems.Count == 0)
                return Ok("Pedido sem itens.");

            if (order.statusPedido != "novo")
                return Ok("Status do pedido diferente de novo.");

            if ((order.userEmail == User.Identity.Name) || (User.IsInRole("ADMIN")))
            {

                CRMRestClient crmClient = new CRMRestClient();
                try
                {
                    Customer customer = crmClient.GetCustomerByEmail("inatel_mobile@inatel.br");
                    CEPDestino = customer.zip;
                }
                catch
                {
                    return Ok("Não foi possivel acessar o serviço de CRM.");
                }

                for (int cont = 0; cont < order.OrderItems.Count; cont++)
                {
                    alturaTotal += order.OrderItems.ElementAt(cont).Product.altura;
                    precoTotal += (order.OrderItems.ElementAt(cont).quantidade * order.OrderItems.ElementAt(cont).Product.preco);
                    pesototal += (order.OrderItems.ElementAt(cont).quantidade * order.OrderItems.ElementAt(cont).Product.peso);

                    if (order.OrderItems.ElementAt(cont).Product.largura > largura)
                        largura = order.OrderItems.ElementAt(cont).Product.largura;

                    if (order.OrderItems.ElementAt(cont).Product.comprimento > comprimento)
                        comprimento = (order.OrderItems.ElementAt(cont).quantidade * order.OrderItems.ElementAt(cont).Product.comprimento);

                    diametro = order.OrderItems.ElementAt(cont).Product.diametro;
                }

                CalcPrecoPrazoWS correios = new CalcPrecoPrazoWS();
                try
                {
                    resultado = correios.CalcPrecoPrazo("", "", "40010", "37550000", CEPDestino, pesototal.ToString(), 1, comprimento, alturaTotal, largura, diametro, "N", 0, "S");
                    prazoEntrega = resultado.Servicos.ElementAt(0).PrazoEntrega;
                }
                catch
                {
                    return Ok("Não foi possivel acessar o serviço dos Correios.");

                }


                NumberFormatInfo nfi = new CultureInfo("pt-BR", false).NumberFormat;
                valorFrete = decimal.Parse(resultado.Servicos.Single().Valor, nfi);

                int prazo = int.Parse(resultado.Servicos.Single().PrazoEntrega);

                DateTime atual = DateTime.Now;

                atual = atual.AddDays(prazo);

                order.pesoPedido = pesototal;
                order.precoFrete = valorFrete;
                order.precoPedido = precoTotal;
                order.dataEntrega = atual;

                db.SaveChanges();

                return Ok("Pedido #: " + idPedido + " calculado com Sucesso. ( Frete: R$ " + resultado.Servicos.Single().Valor + " / Prazo: " + resultado.Servicos.Single().PrazoEntrega + " dias )");
            }
            else
            {
                return Ok("Acesso não autorizado.");
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrderExists(int id)
        {
            return db.Orders.Count(e => e.Id == id) > 0;
        }



    }
}