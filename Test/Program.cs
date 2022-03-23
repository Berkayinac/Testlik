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

            var DepremV1 = dataManager.DatalariKoordinatlarinaGoreGetir(30, 40);
            var DepremV2 = dataManager.DatalariKoordinatlarinaGoreGetir(25, 20);




            var DepremV3 = dataManager.DatalariKoordinatlarinaGoreGetir(14, 20);
            //1.parametre koordinat x , 2.parametre koordinat y, 3.parametre Baslangic ID, 4. parametre SonId
            var depremV1IkinciSonuc = dataManager.DatalariKoordinatlarinaVeIdAraliginaGoreGetir(30, 40, 341, 629);
            var depremV1UcuncuSonuc = dataManager.DatalariKoordinatlarinaVeIdAraliginaGoreGetir(30, 40, 629, 4502);
            var depremV1DorduncuSonuc = dataManager.DatalariKoordinatlarinaVeIdAraliginaGoreGetir(30, 40, 4502, 4843);

            var idsiAyiklanmisVeriler = IdAyiklama(DepremV1, 341);

            var normalIdsigma = dataManager.SigmaMaxV3(idsiAyiklanmisVeriler);
            var aralikliVerilenIdIkinciSonucSigma = dataManager.SigmaMaxV3(depremV1IkinciSonuc);
            var aralikliVerilenIdUcuncuSonucSigma = dataManager.SigmaMaxV3(depremV1UcuncuSonuc);
            var aralikliVerilenIdDorduncuSonucSigma = dataManager.SigmaMaxV3(depremV1DorduncuSonuc);

            Console.WriteLine("sigma değer: " + normalIdsigma);
            Console.WriteLine("sigma değer: " + aralikliVerilenIdIkinciSonucSigma);
            Console.WriteLine("sigma değer: " + aralikliVerilenIdUcuncuSonucSigma);
            Console.WriteLine("sigma değer: " + aralikliVerilenIdDorduncuSonucSigma);
        }



        public static List<Data> IdAyiklama(List<Data> Depremler, int id)
        {
            List<Data> dataList = new List<Data>();
            foreach (var deprem in Depremler)
            {
                dataList.Add(deprem);
                if (deprem.Id == id)
                {
                    break;
                }
            }
            return dataList;
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
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Veriler;Trusted_Connection=True");
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
