using DestinoTrack.DataAccess.Context;
using DestinoTrack.DataAccess.Repositories.GenericRepositories;
using DestinoTrack.Entity.Entities;

namespace DestinoTrack.DataAccess.Repositories.ContactInfos
{
    public class ContactInfoRepository(AppDbContext context): GenericRepository<ContactInfo>(context), IContactInfoRepository
    {
    }
}
