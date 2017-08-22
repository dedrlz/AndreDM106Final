namespace AndreDM106Final.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Models;

    internal sealed class Configuration : DbMigrationsConfiguration<AndreDM106Final.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(AndreDM106Final.Models.ApplicationDbContext context)
        {
            context.Products.AddOrUpdate(
                    p => p.Id,

                  new Product
                  {
                      Id = 1,
                      nome = "produto 1",
                      descricao = "descrição produto 1",
                      cor = "branco",
                      modelo = "TSI",
                      codigo = "COD1",
                      preco = 111,
                      peso = 11,
                      altura = 12,
                      largura = 13,
                      comprimento = 14,
                      diametro = 15,
                      Url = "www.produto1.com"
                  },
                  new Product
                  {
                      Id = 2,
                      nome = "produto 2",
                      descricao = "descrição produto 2",
                      cor = "preto",
                      modelo = "CVT",
                      codigo = "COD2",
                      preco = 222,
                      peso = 21,
                      altura = 22,
                      largura = 23,
                      comprimento = 24,
                      diametro = 25,
                      Url = "www.produto2.com"
                  },
                  new Product
                  {
                      Id = 1,
                      nome = "produto 3",
                      descricao = "descrição produto 3",
                      cor = "prata",
                      modelo = "STI",
                      codigo = "COD3",
                      preco = 333,
                      peso = 31,
                      altura = 32,
                      largura = 33,
                      comprimento = 34,
                      diametro = 35,
                      Url = "www.produto3.com"
                  }
                );
        }
    }
}
