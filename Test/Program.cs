using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DataManager dataManager = new DataManager();

            //1.parametre koordinat x , 2.parametre koordinat y, 3.parametre Baslangic ID, 4. parametre SonId

            var depremV1BirinciSonuc = dataManager.DatalariKoordinatlarinaVeIdAraliginaGoreGetir(40, 50, 0, 3401);
            var depremV1IkinciSonuc = dataManager.DatalariKoordinatlarinaVeIdAraliginaGoreGetir(40, 50, 3401, 4502);
            var depremV1UcuncuSonuc = dataManager.DatalariKoordinatlarinaVeIdAraliginaGoreGetir(40, 50, 4502, 7503);  

            var normalIdsigma = dataManager.SigmaMaxV3(depremV1BirinciSonuc);
            var aralikliVerilenIdIkinciSonucSigma = dataManager.SigmaMaxV3(depremV1IkinciSonuc);
            var aralikliVerilenIdUcuncuSonucSigma = dataManager.SigmaMaxV3(depremV1UcuncuSonuc);

            Console.WriteLine("sigma değer: " + normalIdsigma);
            Console.WriteLine("sigma değer: " + aralikliVerilenIdIkinciSonucSigma);
            Console.WriteLine("sigma değer: " + aralikliVerilenIdUcuncuSonucSigma);

            Console.WriteLine("*************************************************************************************");

            var depremV2BirinciSonuc = dataManager.DatalariKoordinatlarinaVeIdAraliginaGoreGetir(20, 70, 7503, 10904);
            var depremV2IkinciSonuc = dataManager.DatalariKoordinatlarinaVeIdAraliginaGoreGetir(20, 70, 10904, 12005);
            var depremV2UcuncuSonuc = dataManager.DatalariKoordinatlarinaVeIdAraliginaGoreGetir(20, 70, 12005, 15006);

            var normalIdsigmaV2 = dataManager.SigmaMaxV3(depremV1BirinciSonuc);
            var aralikliVerilenIdIkinciSonucSigmaV2 = dataManager.SigmaMaxV3(depremV1IkinciSonuc);
            var aralikliVerilenIdUcuncuSonucSigmaV2 = dataManager.SigmaMaxV3(depremV1UcuncuSonuc);

            Console.WriteLine("sigma değer: " + normalIdsigmaV2);
            Console.WriteLine("sigma değer: " + aralikliVerilenIdIkinciSonucSigmaV2);
            Console.WriteLine("sigma değer: " + aralikliVerilenIdUcuncuSonucSigmaV2);

        }      
    }

    class Data
    {
        public int Id { get; set; }
        public int KoordinatX { get; set; }
        public int KoordinatY { get; set; }
        public double Saniye { get; set; }
        public double Sigma { get; set; }

    }

    class VerilerContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer(@"Server=ELMA\SQLEXPRESS;Database=veriler;Trusted_Connection=True");
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=veriler;Trusted_Connection=True");
            //optionsBuilder.UseSqlServer(@"Server=DESKTOP-IDOOIL9\SQLEXPRESS;Database=veriler;Trusted_Connection=True");
        }

        public DbSet<Data> Veriler { get; set; }
    }

    class EfDataDal
    {
        public List<Data> GetAll(Expression<Func<Data, bool>> filter = null)
        {
            using (VerilerContext context = new VerilerContext())
            {
                return filter == null
                      ? context.Set<Data>().ToList()
                      : context.Set<Data>().Where(filter).ToList();
            }
        }
    }

    class DataManager
    {
        EfDataDal efDataDal;
        public DataManager()
        {
            efDataDal = new EfDataDal();
        }

        public List<Data> DatalariKoordinatlarinaGoreGetir(int koordinatX, int koordinatY)
        {
            return efDataDal.GetAll(d => d.KoordinatX == koordinatX && d.KoordinatY == koordinatY);
        }

        public List<Data> DatalariKoordinatlarinaVeIdsineGoreGetir(int koordinatX, int koordinatY, int id)
        {
            return efDataDal.GetAll(d => d.KoordinatX == koordinatX && d.KoordinatY == koordinatY && (d.Id>0 && d.Id <= id));
        }


        public List<Data> DatalariKoordinatlarinaVeIdAraliginaGoreGetir(int koordinatX, int koordinatY, int baslangicId, int sonId)
        {
            return efDataDal.GetAll(d => d.KoordinatX == koordinatX && d.KoordinatY == koordinatY && (d.Id > baslangicId && d.Id <= sonId));
        }

        public double SigmaMaxV3(List<Data> datalar)
        {
            var result = datalar.Max(d => d.Sigma);
            return result;
        }
    }

}
