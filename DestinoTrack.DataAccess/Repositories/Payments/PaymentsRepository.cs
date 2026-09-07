using DestinoTrack.DataAccess.Context;
using DestinoTrack.DataAccess.Repositories.GenericRepositories;
using DestinoTrack.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DestinoTrack.DataAccess.Repositories.Payments
{
    public class PaymentsRepository(AppDbContext context) : GenericRepository<Payment>(context), IPaymentsRepository
    {
    }
}
