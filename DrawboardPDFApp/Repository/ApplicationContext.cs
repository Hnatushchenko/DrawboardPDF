using DrawboardPDFApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace DrawboardPDFApp.Repository
{
    public class ApplicationContext : DbContext, IApplicationContext
    {
        public DbSet<PdfFileInfo> OpenedPdfFilesHistory => Set<PdfFileInfo>();

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PdfFileInfo>().Property(x => x.Id).HasDefaultValue(Guid.NewGuid());
        }
    }
}
