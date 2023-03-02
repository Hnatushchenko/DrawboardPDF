using DrawboardPDFApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DrawboardPDFApp.Repository
{
    
    public interface IApplicationContext
    {
        DbSet<PdfFileInfo> OpenedPdfFilesHistory { get; }
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
